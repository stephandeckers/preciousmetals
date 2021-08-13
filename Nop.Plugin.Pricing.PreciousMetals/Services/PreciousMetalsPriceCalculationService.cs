/**
 * @Name PreciousMetalsPriceCalculationService.cs
 * @Purpose 
 * @Date 12 January 2021, 08:07:54
 * @Author S.Deckers
 * @Description Based on the original plugin from nopCommerce 230, 
 * see https://www.nopcommerce.com/en/boards/topic/12778/dynamic-pricing-for-precious-metals-plugin for details
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Services
{
	#region -- Using directives --
	using System;	
	using System.Linq;
	using System.Collections.Generic;

	using Nop.Core;
	using Nop.Core.Infrastructure;
	using Nop.Core.Caching;
	using Nop.Core.Domain.Logging;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.Customers;	
	using Nop.Core.Domain.Directory;
	using Nop.Core.Domain.Discounts;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;	
	using Nop.Services.Caching;
	using Nop.Services.Customers;
	using Nop.Services.Directory;
	using Nop.Services.Discounts;
	using Nop.Web.Factories;
	using Nop.Web.Models.Catalog;

	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion	

	public class PreciousMetalsPriceCalculationService : PriceCalculationService
	{
		private readonly CatalogSettings				_catalogSettings;
		private readonly ICacheKeyService				_cacheKeyService;
		private readonly IProductService				_productService;
		private readonly IStaticCacheManager			_staticCacheManager;
		private readonly ICustomerService				_customerService;
		private readonly IStoreContext					_storeContext;
		//private readonly IProductModelFactory			_productModelFactory;
		private readonly IPreciousMetalsDetailService	_preciousMetalsDetailService;
		private readonly IPreciousMetalsQuoteService	_preciousMetalsQuoteService;
		private readonly IMeasureService				_measureService;
		private readonly ILogger						_logger;

		/// <summary date="14-01-2021, 12:05:08" author="S.Deckers">
		/// Construction
		/// </summary>
		public PreciousMetalsPriceCalculationService
		(
			CatalogSettings					catalogSettings
		,	CurrencySettings				currencySettings
		,	ICacheKeyService				cacheKeyService
		,	ICategoryService				categoryService
		,	ICurrencyService				currencyService
		,	ICustomerService				customerService
		,	IDiscountService				discountService
		,	IManufacturerService			manufacturerService
		,	IProductAttributeParser			productAttributeParser
		,	IProductService					productService
		,	IStaticCacheManager				staticCacheManager
		,	IStoreContext					storeContext
		,	IWorkContext					workContext
		,	IPreciousMetalsDetailService	preciousMetalsDetailService
		,	IPreciousMetalsQuoteService		preciousMetalsQuoteService
		,	IMeasureService					measureService
		,	ILogger							logger
		)
			: base
			(
				catalogSettings:		catalogSettings
			,	currencySettings:		currencySettings
			,	cacheKeyService:		cacheKeyService
			,	categoryService:		categoryService
			,	currencyService:		currencyService
			,	customerService:		customerService
			,	discountService:		discountService
			,	manufacturerService:	manufacturerService
			,	productAttributeParser:	productAttributeParser
			,	productService:			productService
			,	staticCacheManager:		staticCacheManager
			,	storeContext:			storeContext
			,	workContext:			workContext
			)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			this._catalogSettings				= catalogSettings;
			this._cacheKeyService				= cacheKeyService;
			this._productService				= productService;
			this._staticCacheManager			= staticCacheManager;
			this._customerService				= customerService;
			this._storeContext					= storeContext;
			this._preciousMetalsDetailService	= preciousMetalsDetailService;
			this._preciousMetalsQuoteService	= preciousMetalsQuoteService;
			this._measureService				= measureService;
			this._logger						= logger;
		}

		/// <summary date="12-01-2021, 19:43:30" author="S.Deckers">
		/// GetFinalPrice
		/// </summary>
		/// <param name="product"></param>
		/// <param name="customer"></param>
		/// <param name="overriddenProductPrice"></param>
		/// <param name="additionalCharge"></param>
		/// <param name="includeDiscounts"></param>
		/// <param name="quantity"></param>
		/// <param name="rentalStartDate"></param>
		/// <param name="rentalEndDate"></param>
		/// <param name="discountAmount"></param>
		/// <param name="appliedDiscounts"></param>
		/// <returns></returns>
		public override decimal GetFinalPrice
		(	
			Product				product
		,	Customer			customer
		,	decimal?			overriddenProductPrice
		,	decimal				additionalCharge
		,	bool				includeDiscounts
		,	int					quantity
		,	DateTime?			rentalStartDate
		,	DateTime?			rentalEndDate
		,	out decimal			discountAmount
		,	out List<Discount>	appliedDiscounts
		)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Format( "{0}", product.Id)));

			discountAmount		= 0.0M;
			appliedDiscounts	= null;

			PreciousMetalsDetail preciousMetalsDetail = this._preciousMetalsDetailService.GetByProductId( product.Id);

			if( preciousMetalsDetail == null)
			{
				return( base.GetFinalPrice( product, customer, overriddenProductPrice, additionalCharge, includeDiscounts, quantity, rentalEndDate, rentalEndDate, out discountAmount, out appliedDiscounts));
			}

			appliedDiscounts = new List<Discount>();

			decimal price = GetMetalTierPrice( product, customer, quantity, preciousMetalsDetail);

			appliedDiscounts = new List<Discount>();
			decimal	appliedDiscountAmount = decimal.Zero;

			//discount
			decimal tmpDiscountAmount = GetDiscountAmount( product, customer, price, out var tmpAppliedDiscounts);
			price -= tmpDiscountAmount;

			if (tmpAppliedDiscounts?.Any() ?? false)
			{
				appliedDiscounts.AddRange( tmpAppliedDiscounts);
				appliedDiscountAmount = tmpDiscountAmount;
			}

			return( price);
		}

		/// <summary date="16-01-2021, 09:07:22" author="S.Deckers">
		/// Gets the TierPrice for the metal
		/// </summary>
		/// <param name="product"></param>
		/// <param name="customer"></param>
		/// <param name="quantity"></param>
		/// <param name="detail"></param>
		/// <returns></returns>
		public decimal GetMetalTierPrice
		( 
			Product					product
		,	Customer				customer
		,	int						quantity
		,	PreciousMetalsDetail	detail
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// get the price per piece
			decimal				piecePrice = getMetalPiecePrice( detail);
			IList<TierPrice>	tierPrices = _productService.GetTierPricesByProduct( productId: product.Id);

			if( tierPrices == null)
			{
				return( piecePrice);
			}

			if( tierPrices.Count == 0)
			{
				return( piecePrice);
			}

			// --- Changed:Calculation of tierprices is only done over here and not also in the ProductService (20210116 SDE)

			if( detail.TierPriceType == PreciousMetalsTierPriceType.DoNotUse)
			{
				return( piecePrice);
			}

			int[] customerRoles = _customerService.GetCustomerRoleIds( customer);
			tierPrices = tierPrices.OrderBy(tp => tp.Quantity).ToList().FilterByCustomerRole( customerRoles).ToList();

			int		previousQty = 1;
			decimal previousPrice = piecePrice;
			foreach( TierPrice tierPrice in tierPrices)
			{
				//check quantity
				if( quantity < tierPrice.Quantity)
				{
					continue;
				}
				if( tierPrice.Quantity < previousQty)
				{
					continue;
				}

				//save new price
				previousPrice	= tierPrice.Price;
				previousQty		= tierPrice.Quantity;
			}

			// If there is no matching tier price, return the default single piece price
			if( previousQty == 1)
			{
				return( piecePrice);
			}

			// Calculate tier prices based on type
			
			PreciousMetalsTierPriceType preciousMetalsTierPriceType = detail.TierPriceType;

			switch( preciousMetalsTierPriceType)
			{
				case( PreciousMetalsTierPriceType.Percentage):
				{
					return piecePrice - ((piecePrice / 100m) * previousPrice);
				}
				case( PreciousMetalsTierPriceType.PriceDiscount):
				{
					return piecePrice - previousPrice;
				}
				case( PreciousMetalsTierPriceType.DoNotUse):
				{
					return piecePrice;
				}
				default:
				{
					throw new ArgumentException( "Invalid Tier Price Type!");
				}
			}
		}

		/// <summary date="16-01-2021, 10:05:02" author="S.Deckers">
		/// Return the metal price for the product weight in the primary shop's currency
		/// </summary>
		/// <param name="detail"></param>
		/// <returns></returns>
		private decimal getMetalPiecePrice( PreciousMetalsDetail detail)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			// get the quote for this metal
			PreciousMetalsQuote quote = _preciousMetalsQuoteService.GetQuote( detail.MetalType);

			// determine the unit price
			decimal unitPrice;
			switch( detail.QuoteType)
			{
				case( PreciousMetalsQuoteType.Bid):
				{
					unitPrice = quote.Bid;
					break;
				}

				case( PreciousMetalsQuoteType.Ask):
				{
					unitPrice = quote.Ask;
					break;
				}

				case( PreciousMetalsQuoteType.Low):
				{
					unitPrice = quote.Low;
					break;
				}

				case( PreciousMetalsQuoteType.High):
				{
					unitPrice = quote.High;
					break;
				}

				default:
				{
					throw new ArgumentException( "Invalid Quote Type!");
				}
			}

			decimal weight = detail.Weight;
			if( detail.WeightId > 0) // assume zero means no conversion
			{
				// the unit price is per troy ounce, so convert the detail weight to troy ounces.
				MeasureWeight weightFrom	= _measureService.GetMeasureWeightById( detail.WeightId);
				MeasureWeight weightTo		= _measureService.GetMeasureWeightBySystemKeyword("troyounce");

				weight = _measureService.ConvertWeight( detail.Weight, weightFrom, weightTo, false);
			}

			// the piece price starts from the unit price times the weight
			decimal piecePrice = unitPrice * weight;

			// figure the markup
			decimal markup = 0m;
			switch( detail.MathType)
			{
				case( PreciousPriceCalculationType.AddFirstThenMultiply):
				{
					markup = ((piecePrice + detail.FlatMarkup) * detail.PercentMarkup / 100m);
					break;
				}
				case( PreciousPriceCalculationType.MultiplyFirstThenAdd):
				{
					markup = (piecePrice * detail.PercentMarkup / 100m) + detail.FlatMarkup;
					break;
				}
			}

			// add the markup to the piece price
			piecePrice += markup;

			if( detail.PriceRounding > 0)
			{
				switch( detail.PriceRoundingType)
				{
					case( PriceRoundingType.RoundDown):
					{
						piecePrice = Math.Floor(piecePrice / (decimal)detail.PriceRounding) * ((decimal)detail.PriceRounding);
						break;
					}
					case( PriceRoundingType.RoundUp):
					{
						piecePrice = Math.Ceiling(piecePrice / (decimal)detail.PriceRounding) * ((decimal)detail.PriceRounding);
						break;
					}
				}
			}

			// return the price, but nothing less than the LowerAmount
			decimal thePrice = detail.LowerAmount > piecePrice ? detail.LowerAmount : piecePrice;
			return( thePrice);
		}
	}
}