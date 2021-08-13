/**
 * @Name ProductUpdatedEventConsumer.cs
 * @Purpose 
 * @Date 01 February 2021, 13:11:34
 * @Author S.Deckers, taken from R.Isler
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Services
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Http;

	using Nop.Core;
	using Nop.Core.Events;
	using Nop.Core.Domain.Logging;
	using Nop.Core.Domain.Orders;
	using Nop.Core.Domain.Customers;
	using Nop.Services.Events;
	using Nop.Core.Domain.Catalog;
	using Nop.Services.Logging;

	using Nop.Plugin.Pricing.PreciousMetals.Domain;
	using Nop.Plugin.Pricing.PreciousMetals.Model;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	public class ProductUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Product>>
	{
		private readonly IHttpContextAccessor			_httpContextAccessor;
		private readonly ILogger						_logger;
		private readonly IPreciousMetalsDetailService	_preciousMetalsDetailService;

		public ProductUpdatedEventConsumer
		(
			IHttpContextAccessor			httpContextAccessor
		,	ILogger							logger
		,	IPreciousMetalsDetailService	preciousMetalsDetailService
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			_httpContextAccessor			= httpContextAccessor;
			_logger							= logger;
			_preciousMetalsDetailService	= preciousMetalsDetailService;
		}
		
		/// <summary>
		/// Update PreciousMetalDetails for this product
		/// </summary>
		/// <param name="eventMessage"></param>
		public void HandleEvent( EntityUpdatedEvent<Product> eventMessage)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			Product		product = eventMessage.Entity;
			HttpRequest request = _httpContextAccessor.HttpContext?.Request;

			if( request?.Form == null)
			{
				_logger.Error("Request form is empty");				
				return;
			}

			//foreach( var item in request.Form.Keys)
			//{
			//	request.Form.TryGetValue( item, out Microsoft.Extensions.Primitives.StringValues value);
			//	d.WriteLine( string.Format( "key={0}, value={1}", item, value));
			//}

			// --- If key is present item is should be treated asa PreciousMetal (20210201 SDE)

			if( request.Form.ContainsKey( "IsPreciousMetalEnabled") == false)
			{
				_logger.Error("IsPreciousMetalEnabled key not found");				
				return;
			}

			string s = string.Empty;

			string theValue = request.Form["IsPreciousMetalEnabled"].ToString();
			if( theValue == "false")
			{
				_logger.Error("Value is false");
				s = string.Format( "Item [{0}] is not a preciousmetal, deleting old entries", product.Id);
				this._logger.InsertLog( LogLevel.Information, GetType( ).Name, s, null);
				this._preciousMetalsDetailService.Delete( product.Id);
				return;
			}

			d.WriteLine( string.Format( "Item {0} is a PreciousMetal", product.Id));

			//dumpFormParameters( request.Form);

			ExtendedProductModel model = getProductModel( product.Id, request.Form);

			// --- Update existing item (20210201 SDE)

			PreciousMetalsDetail item = this._preciousMetalsDetailService.GetByProductId( product.Id);	

			if( item != null )
			{
				item = model2PreciousMetalsDetail( item, model);
				this._preciousMetalsDetailService.Update( item);

				s = string.Format( "PreciousMetalsDetail [{0}] updated", product.Id);
				d.WriteLine( s);
				this._logger.InsertLog( LogLevel.Information, GetType( ).Name, s, null);
				return;	
			}

			// --- Insert new item (20210201 SDE)

			item = new PreciousMetalsDetail( );
			item = model2PreciousMetalsDetail( item, model);

			this._preciousMetalsDetailService.Insert( item);
			s = string.Format( "PreciousMetalsDetail [{0}] inserted", product.Id);
			d.WriteLine( s);
			this._logger.InsertLog( LogLevel.Information, GetType( ).Name, s, null);
		}

		private PreciousMetalsDetail model2PreciousMetalsDetail( PreciousMetalsDetail preciousMetalsDetail, ExtendedProductModel model)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			preciousMetalsDetail.ProductId			= model.Id;

			preciousMetalsDetail.MetalType			= model.MetalType;
			preciousMetalsDetail.QuoteType			= model.QuoteType;
			preciousMetalsDetail.MathType			= model.MathType;
			preciousMetalsDetail.Weight				= model.pm_Weight;
			preciousMetalsDetail.WeightId			= model.WeightUnit;
			preciousMetalsDetail.PercentMarkup		= model.PercentMarkup;
			preciousMetalsDetail.FlatMarkup			= model.FlatMarkup;
			preciousMetalsDetail.TierPriceType		= model.TierPriceType;
			preciousMetalsDetail.LowerAmount		= model.LowerAmount;
			preciousMetalsDetail.PriceRounding		= model.PriceRoundingNumber;
			preciousMetalsDetail.PriceRoundingType	= model.PriceRounding;
			return( preciousMetalsDetail);
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

		/// <summary date="01-02-2021, 14:21:24" author="S.Deckers">
		/// Create ExtendedProductModel from forms collection
		/// </summary>
		/// <returns></returns>
		private ExtendedProductModel getProductModel( int productId, IFormCollection items)
		{
			string getFormValue( IFormCollection items, string key)
			{
				items.TryGetValue( key, out Microsoft.Extensions.Primitives.StringValues value);
				return( value);
			}

			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			ExtendedProductModel item = new ExtendedProductModel( );
			item.Id = productId;

			//string theValue = string.Empty;

			// --- Metaltype (20210201 SDE)

			//theValue = getFormValue( items, "MetalType");
			item.MetalType				= (PreciousMetalType)				Enum.Parse( typeof( PreciousMetalType),				getFormValue( items, "MetalType"));
			item.QuoteType				= (PreciousMetalsQuoteType)			Enum.Parse( typeof( PreciousMetalsQuoteType),		getFormValue( items, "QuoteType"));
			item.MathType				= (PreciousPriceCalculationType)	Enum.Parse( typeof( PreciousPriceCalculationType),	getFormValue( items, "MathType"));
			item.TierPriceType			= (PreciousMetalsTierPriceType)		Enum.Parse( typeof( PreciousMetalsTierPriceType),	getFormValue( items, "TierPriceType"));
			item.PriceRounding			= (PriceRoundingType)				Enum.Parse( typeof( PriceRoundingType),				getFormValue( items, "PriceRounding"));

			item.pm_Weight				= System.Convert.ToDecimal	( getFormValue( items, "pm_Weight"));
			item.WeightUnit				= System.Convert.ToInt32	( getFormValue( items, "WeightUnit"));
			item.PercentMarkup			= System.Convert.ToDecimal	( getFormValue( items, "PercentMarkup"));			
			item.FlatMarkup				= System.Convert.ToDecimal	( getFormValue( items, "FlatMarkup"));			
			item.LowerAmount			= System.Convert.ToDecimal	( getFormValue( items, "LowerAmount"));
			item.PriceRoundingNumber	= System.Convert.ToInt32	( getFormValue( items, "PriceRoundingNumber"));			

			item.Dump( );
			//items.Keys[ "MetalType"]
			//item.QuoteType	= PreciousMetalType.Gold;
			//item.MathType	= PreciousMetalType.Gold;
			//item.pm_Weight	= PreciousMetalType.Gold;
			//item.WeightUnit = PreciousMetalType.Gold;
			//item.PercentMarkup = PreciousMetalType.Gold;
			//item.FlatMarkup = PreciousMetalType.Gold;
			//item.TierPriceType = PreciousMetalType.Gold;
			//item.LowerAmount = PreciousMetalType.Gold;
			//item.PriceRoundingNumber = PreciousMetalType.Gold;
			//item.PriceRounding = PreciousMetalType.Gold;



			return( item);
		}

		private void dumpFormParameters( IFormCollection items)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = string.Empty;

			foreach( string key in items.Keys)
			{
				items.TryGetValue( key, out Microsoft.Extensions.Primitives.StringValues value);
				s = string.Format( "key=[{0}], value=[{1}]", key, value );
				d.WriteLine( s);
			}
		}
	}
}
