/**
 * @Name ProductExtensionComponent.cs
 * @Purpose 
 * @Date 26 January 2021, 18:52:53
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Component
{
	#region -- Using directives --	
	using Nop.Core;
	using Nop.Core.Domain.Logging;
	using Nop.Services.Logging;
	using Microsoft.AspNetCore.Mvc;
	using Nop.Web.Framework.Mvc;
	using Nop.Web.Areas.Admin.Models.Catalog;
	using Nop.Web.Framework.Components;
	using Nop.Plugin.Pricing.PreciousMetals;
	using Nop.Plugin.Pricing.PreciousMetals.Model;
	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	using Nop.Plugin.Pricing.PreciousMetals.Services;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	[ ViewComponent( Name = Nop.Plugin.Pricing.PreciousMetals.Constants.ViewNames.ComponentViewName)]
    public class ProductExtensionComponent : NopViewComponent
    {
		private readonly IPreciousMetalsDetailService	_preciousMetalsDetailService;
		private readonly ILogger						_logger;

		/// <summary date="26-01-2021, 19:37:04" author="S.Deckers">
		/// Ctor
		/// </summary>
		/// <param name="preciousMetalsDetailService"></param>
		public ProductExtensionComponent
		( 
			IPreciousMetalsDetailService	preciousMetalsDetailService
		,	ILogger							logger
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._preciousMetalsDetailService	= preciousMetalsDetailService;
			this._logger						= logger;
		}

        public IViewComponentResult Invoke( string widgetZone, object additionalData)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            if(!(additionalData is ProductModel productModel))
			{
				return Content( "");
			} 

			PreciousMetalsDetail item = this._preciousMetalsDetailService.GetByProductId( productId:productModel.Id);

			ExtendedProductModel model = null;

			if( item == null)
			{	
				this._logger.InsertLog( LogLevel.Information, GetType( ).Name, string.Format( "Product={0} is not a preciousmetal", productModel.Id), null);

				model = new ExtendedProductModel( )
				{
					IsPreciousMetalEnabled	= false
				,	MetalType				= PreciousMetalType.Unknown
				,	QuoteType				= PreciousMetalsQuoteType.Ask
				,	TierPriceType			= PreciousMetalsTierPriceType.Percentage
				,	LowerAmount				= 1.0M
				,	pm_Weight				= 1
				,	WeightUnit				= 2
				,	MathType				= PreciousPriceCalculationType.AddFirstThenMultiply
				,	PercentMarkup			= 0.0M
				,	FlatMarkup				= 0.0M
				,	PriceRounding			= PriceRoundingType.None
				,	PriceRoundingNumber		= 0		
				};

				return View( Constants.ViewLocations.ExtendedProductView, model);
			}

			this._logger.InsertLog( LogLevel.Information, GetType( ).Name, string.Format( "Product={0} is a preciousmetal", productModel.Id), null);

			model = new ExtendedProductModel( )
			{
				IsPreciousMetalEnabled	= true
			,	MetalType				= item.MetalType
			,	QuoteType				= item.QuoteType
			,	TierPriceType			= item.TierPriceType
			,	LowerAmount				= item.LowerAmount
			,	pm_Weight				= item.Weight
			,	WeightUnit				= item.WeightId
			,	MathType				= item.MathType
			,	PercentMarkup			= item.PercentMarkup
			,	FlatMarkup				= item.FlatMarkup
			,	PriceRounding			= item.PriceRoundingType
			,	PriceRoundingNumber		= item.PriceRounding		
			};

			return View( Constants.ViewLocations.ExtendedProductView, model);
		}
		/*
		 * 
		 * PreciousMetalsDetail										ExtendedProductModel	
		   -----------------------------------------------------	-------------------------------------------------
			public int							ProductId			
			public PreciousMetalType			MetalType			public PreciousMetalType			MetalType
			public PreciousMetalsQuoteType		QuoteType			public PreciousMetalsQuoteType		QuoteType
			public PreciousPriceCalculationType	MathType			public PreciousPriceCalculationType MathType
			public decimal						Weight				public decimal						pm_Weight
			public int							WeightId			public int							WeightUnit
			public decimal						PercentMarkup		public decimal						PercentMarkup
			public decimal						FlatMarkup			public decimal						FlatMarkup
			public PreciousMetalsTierPriceType	TierPriceType		public PreciousMetalsTierPriceType	TierPriceType
			public decimal						LowerAmount			public decimal						LowerAmount
			public int							PriceRounding		public int							PriceRoundingNumber
			public PriceRoundingType			PriceRoundingType	public PriceRoundingType			PriceRounding
		*/
	}
}
