/**
 * @Name ProductExtensionModel.cs
 * @Purpose 
 * @Date 26 January 2021, 18:54:54
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Model
{
	#region -- Using directives --
	using System;
	using System.Text;
	using Nop.Core;
	using System.Collections;
	using System.ComponentModel;
	using System.Collections.Generic;
	using LinqToDB.Mapping;
	using Nop.Core.Domain.Directory;
	using Nop.Core.Infrastructure;
	using Nop.Services.Directory;
	using Nop.Web.Framework.Mvc.ModelBinding;
	using Nop.Web.Framework.Models;
	using Nop.Plugin.Pricing.PreciousMetals.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;

	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	using d=System.Diagnostics.Debug;
	#endregion

	public class ExtendedProductModel : BaseNopEntityModel
	{		
		public ExtendedProductModel( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			initCollections( );
		}

		private void initCollections( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// --- AvailableMetalTypes (20210126 SDE)

			this.AvailableMetalTypes = new List<SelectListItem>();
			this.AvailableMetalTypes.Add( new SelectListItem( ) { Value = PreciousMetalType.Gold.ToString( ),	Text = PreciousMetalType.Gold.ToString( ) });
			this.AvailableMetalTypes.Add( new SelectListItem( ) { Value = PreciousMetalType.Silver.ToString( ), Text = PreciousMetalType.Silver.ToString( ) });

			// --- AvailableQuoteTypes (20210126 SDE)

			this.AvailableQuoteTypes = new List<SelectListItem>();			
			this.AvailableQuoteTypes.Add( new SelectListItem( ) { Value = PreciousMetalsQuoteType.Ask.ToString( ),	Text = PreciousMetalsQuoteType.Ask.ToString( ) });
			this.AvailableQuoteTypes.Add( new SelectListItem( ) { Value = PreciousMetalsQuoteType.Bid.ToString( ),	Text = PreciousMetalsQuoteType.Bid.ToString( ) });			

			// --- TierPricingTypes (20210126 SDE)

			this.AvailableTierPricingTypes = new List<SelectListItem>();			

			this.AvailableTierPricingTypes.Add( new SelectListItem( ) { Value = PreciousMetalsTierPriceType.DoNotUse.ToString( ),		Text = "Do Not Use" });
			this.AvailableTierPricingTypes.Add( new SelectListItem( ) { Value = PreciousMetalsTierPriceType.Percentage.ToString( ),		Text = "Percentage"});
			this.AvailableTierPricingTypes.Add( new SelectListItem( ) { Value = PreciousMetalsTierPriceType.PriceDiscount.ToString( ),	Text = "Price Discount" });

			// --- AvailableWeightUnits (20210126 SDE)

			this.AvailableWeightUnits = new List<SelectListItem>();

			IMeasureService			measureService = EngineContext.Current.Resolve<IMeasureService>();
			IList<MeasureWeight>	items			= measureService.GetAllMeasureWeights( );

			foreach( MeasureWeight item in items)
			{
				this.AvailableWeightUnits.Add( new SelectListItem( ) { Value = item.Id.ToString(), Text = item.Name } );
			}

			// --- AvailableMathTypes (20210126 SDE)

			this.AvailableMathTypes = new List<SelectListItem>();			

			this.AvailableMathTypes.Add( new SelectListItem( ) { Value = PreciousPriceCalculationType.MultiplyFirstThenAdd.ToString( ),		Text = "Multiply First Then Add" });
			this.AvailableMathTypes.Add( new SelectListItem( ) { Value = PreciousPriceCalculationType.AddFirstThenMultiply.ToString( ),		Text = "Add First Then Multiply" });			

			// --- AvailablePriceRoundingTypes (20210126 SDE)

			this.AvailablePriceRoundingTypes = new List<SelectListItem>();			

			this.AvailablePriceRoundingTypes.Add( new SelectListItem( ) { Value = PriceRoundingType.None.ToString( ),		Text = "None" });
			this.AvailablePriceRoundingTypes.Add( new SelectListItem( ) { Value = PriceRoundingType.RoundUp.ToString( ),	Text = "Round Up" });
			this.AvailablePriceRoundingTypes.Add( new SelectListItem( ) { Value = PriceRoundingType.RoundDown.ToString( ),	Text = "Round Down" });
			
			// --- Primary currency (20210126 SDE)

			ICurrencyService	currencyService				= EngineContext.Current.Resolve<ICurrencyService>();
			CurrencySettings	currencySettings			= EngineContext.Current.Resolve<CurrencySettings>();

			int					primaryStoreCurrencyId		= currencySettings.PrimaryStoreCurrencyId;
			string				primaryStoreCurrencyCode	= currencyService.GetCurrencyById( primaryStoreCurrencyId).CurrencyCode;

			this.PrimaryStoreCurrencyCode = primaryStoreCurrencyCode;
		}

		public int ProductId												{ get; set; }

		[ NopResourceDisplayName( Resources.IsPreciousMetalEnabled )]
		public bool IsPreciousMetalEnabled									{ get; set; }

		//public bool SimpleBoolean											{ get; set; }
		//public string SimpleString											{ get; set; }
		
		[ NopResourceDisplayName( Resources.MetalType )]
		public PreciousMetalType			MetalType						{ get; set; }
		public IList<SelectListItem>		AvailableMetalTypes				{ get; set; }

		[ NopResourceDisplayName( Resources.QuoteType )]
		public PreciousMetalsQuoteType		QuoteType						{ get; set; }
		public IList<SelectListItem>		AvailableQuoteTypes				{ get; set; }

		[ NopResourceDisplayName( Resources.TierPricingType )]
		public PreciousMetalsTierPriceType	TierPriceType					{ get; set; }
		public IList<SelectListItem>		AvailableTierPricingTypes		{ get; set; }

		[ NopResourceDisplayName( Resources.LowerAmount4PriceCalculation )]
		public decimal						LowerAmount						{ get; set; }

		[ NopResourceDisplayName( Resources.Weight )]
		public decimal						pm_Weight						{ get; set; }

		[ NopResourceDisplayName( Resources.WeightUnit )]
		public int							WeightUnit						{ get; set; }
		public IList<SelectListItem>		AvailableWeightUnits			{ get; set; }
	
		[ NopResourceDisplayName( Resources.MathType )]
		public PreciousPriceCalculationType MathType						{ get; set; }
		public IList<SelectListItem>		AvailableMathTypes				{ get; set; }

		[ NopResourceDisplayName( Resources.PercentMarkup )]
		public decimal						PercentMarkup					{ get; set; }

		[ NopResourceDisplayName( Resources.FlatMarkup )]
		public decimal						FlatMarkup						{ get; set; }

		[ NopResourceDisplayName( Resources.PriceRoundingType )]
		public PriceRoundingType			PriceRounding					{ get; set; }
		public IList<SelectListItem>		AvailablePriceRoundingTypes		{ get; set; }

		[ NopResourceDisplayName( Resources.PriceRoundingNumber )]
		public int							PriceRoundingNumber				{ get; set; }

		public string PrimaryStoreCurrencyCode								{ get; set; }

		public void Dump( )
		{
			d.WriteLine( this.ToString( ) );
		}

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "ProductId={0}",				this.ProductId);
			sb.AppendFormat( ", MetalType={0}",				this.MetalType);
			sb.AppendFormat( ", QuoteType={0}",				this.QuoteType);
			sb.AppendFormat( ", MathType={0}",				this.MathType);
			sb.AppendFormat( ", pm_Weight={0}",				this.pm_Weight);
			sb.AppendFormat( ", WeightUnit={0}",			this.WeightUnit);
			sb.AppendFormat( ", PercentMarkup={0}",			this.PercentMarkup);
			sb.AppendFormat( ", FlatMarkup={0}",			this.FlatMarkup);
			sb.AppendFormat( ", TierPriceType={0}",			this.TierPriceType);
			sb.AppendFormat( ", LowerAmount={0}",			this.LowerAmount);
			sb.AppendFormat( ", PriceRoundingNumber={0}",	this.PriceRoundingNumber);
			sb.AppendFormat( ", PriceRounding={0}",			this.PriceRounding);
			return( sb.ToString( ) );
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
