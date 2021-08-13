/**
 * @Name PreciousMetalsQuoteService.cs
 * @Purpose 
 * @Date 12 January 2021, 19:04:45
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Services
{
	#region -- Using directives --
	using System;
	using System.Linq;

	using Nop.Core;
	using Nop.Data;
	using Nop.Core.Caching;
	using Nop.Services.Caching;
	using Nop.Services.Caching.Extensions;
	using Nop.Core.Domain.Logging;
	using Nop.Core.Domain.Configuration;
	using Nop.Services.Logging;	
	using Nop.Services.Configuration;	

	using Nop.Plugin.Pricing.PreciousMetals.Domain;
	using Nop.Plugin.Pricing.PreciousMetals.Providers;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	/// <summary date="23-01-2021, 07:17:02" author="S.Deckers">
	/// IPreciousMetalsQuoteService
	/// </summary>
	public interface IPreciousMetalsQuoteService
	{
		PreciousMetalsQuote GetQuote				( PreciousMetalType metalType);
		PreciousMetalsQuote	GetKitcoGoldQuote		( );
		PreciousMetalsQuote	GetKitcoSilverQuote		( );
		PreciousMetalsQuote	GetxIgniteGoldQuote		( );
		PreciousMetalsQuote	GetxIgniteSilverQuote	( );
		void				InsertQuote				( PreciousMetalsQuote preciousMetalsQuote);
	}

	/// <summary date="23-01-2021, 07:17:06" author="S.Deckers">
	/// PreciousMetalsQuoteService
	/// </summary>
	public partial class PreciousMetalsQuoteService : IPreciousMetalsQuoteService
	{
		private readonly ILogger							_logger;
		private readonly IStaticCacheManager				_staticCacheManager;
		private readonly IRepository<PreciousMetalsQuote>	_repository;
		private readonly PreciousMetalsSettings				_preciousMetalsSettings;

		/// <summary date="14-01-2021, 22:07:56" author="S.Deckers">
		/// Construction
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="settingService"></param>
		/// <param name="repository"></param>
		public PreciousMetalsQuoteService
		( 
			ILogger								logger
		,	IStaticCacheManager					staticCacheManager
		,	IRepository<PreciousMetalsQuote>	repository
		,	PreciousMetalsSettings				preciousMetalsSettings
		)
		{ 
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger					= logger;
			this._staticCacheManager		= staticCacheManager;
			this._repository				= repository;
			this._preciousMetalsSettings	= preciousMetalsSettings;
		}

		/// <summary date="13-01-2021, 18:18:04" author="S.Deckers">
		/// GetQuote
		/// </summary>
		/// <param name="metalType"></param>
		/// <returns></returns>
		public PreciousMetalsQuote GetQuote( PreciousMetalType metalType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty ) );

			this._logger.InsertLog( LogLevel.Information, GetType( ).Name, string.Format( "metalType={0}", metalType), null);

			return( handle_GetQuote( metalType));
		}

		/// <summary date="15-01-2021, 12:03:25" author="S.Deckers">
		/// GetxIgniteGoldQuote
		/// </summary>
		/// <returns></returns>
		public PreciousMetalsQuote GetxIgniteGoldQuote( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);
			return( getxIgniteQuoteFromInternet( PreciousMetalType.Gold));
		}

		/// <summary date="15-01-2021, 12:03:28" author="S.Deckers">
		/// GetxIgniteSilverQuote
		/// </summary>
		/// <returns></returns>
		public PreciousMetalsQuote GetxIgniteSilverQuote( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);
			return( getxIgniteQuoteFromInternet( PreciousMetalType.Silver));
		}

		/// <summary date="13-01-2021, 18:17:11" author="S.Deckers">
		/// GetKitcoGoldQuote
		/// </summary>
		/// <returns></returns>
		public PreciousMetalsQuote GetKitcoGoldQuote ( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);
			return( getKitcoQuoteFromInternet( PreciousMetalType.Gold));
		}

		/// <summary date="13-01-2021, 18:17:08" author="S.Deckers">
		/// GetKitcoSilverQuote
		/// </summary>
		/// <returns></returns>
		public PreciousMetalsQuote GetKitcoSilverQuote ( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);
			return( getKitcoQuoteFromInternet( PreciousMetalType.Silver));
		}

		/// <summary date="14-01-2021, 22:10:22" author="S.Deckers">
		/// Store quote in DB
		/// </summary>
		/// <param name="preciousMetalsQuote"></param>
		public void	InsertQuote( PreciousMetalsQuote preciousMetalsQuote)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			_repository.Insert( preciousMetalsQuote);
		}
	}

	public partial class PreciousMetalsQuoteService
	{
		private const string CACHE_KEY = "pricing.preciousMetals.id.{0}";

		private PreciousMetalsQuote handle_GetQuote( PreciousMetalType metalType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string s = string.Empty;

			int cachePeriodInMinutes = this._preciousMetalsSettings.CachePeriodInMinutes;
			string		key = string.Format( "pricing.preciousMetals.id.{0}", metalType);
			CacheKey	cacheKey = new CacheKey( key, cacheTime: cachePeriodInMinutes);

			return _staticCacheManager.Get(cacheKey,  () =>
			{
				PreciousMetalsQuote quote = getQuoteFromInternet(metalType);
				_repository.Insert( quote);
				s = string.Format( "Cache refreshed,id={0}", quote.Id);
				//d.WriteLine( s);
				this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, s, null);
				return( quote);
			});
		}

		/// <summary date="15-01-2021, 12:04:01" author="S.Deckers">
		/// getQuoteFromInternet
		/// </summary>
		/// <param name="metalType"></param>
		/// <returns></returns>
		private PreciousMetalsQuote getQuoteFromInternet( PreciousMetalType metalType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, string.Format( "Getting Quote for {0}", metalType), null);

			QuoteProvider quoteProvider = this._preciousMetalsSettings.QuoteProvider;

			switch( quoteProvider)
			{
				// --- Get quote from xIgnite (20210115 SDE)

				case( QuoteProvider.xIgnite):
				{
					if( metalType == PreciousMetalType.Gold)
					{
						return( getxIgniteQuoteFromInternet( PreciousMetalType.Gold));
					}

					if( metalType == PreciousMetalType.Silver)
					{
						return( getxIgniteQuoteFromInternet( PreciousMetalType.Silver));
					}

					this._logger.Error( string.Format( "Unsupported metalType:{0}", metalType));
					return( null);
				}

				// --- Get quote from Kitco (20210115 SDE)

				case( QuoteProvider.Kitco):
				{
					if( metalType == PreciousMetalType.Gold)
					{
						return( getKitcoQuoteFromInternet( PreciousMetalType.Gold));
					}

					if( metalType == PreciousMetalType.Silver)
					{
						return( getKitcoQuoteFromInternet( PreciousMetalType.Silver));
					}

					this._logger.Error( string.Format( "Unsupported metalType:{0}", metalType));
					return( null);
				}

				// --- Can't handle this (20210115 SDE)

				default:
				{
					this._logger.Error( string.Format( "Unsupported QuoteProvider:{0}", quoteProvider));
					return( null);
				}
			}
		}

		/// <summary date="15-01-2021, 12:05:42" author="S.Deckers">
		/// getxIgniteQuoteFromInternet 
		/// </summary>
		/// <param name="metalType"></param>
		/// <returns></returns>
		private PreciousMetalsQuote getxIgniteQuoteFromInternet( PreciousMetalType metalType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);

			string xIgniteToken = this._preciousMetalsSettings.xIgniteToken;
			string errMsg;
			PreciousMetalsQuote q = xIgniteQuoteProvider.GetQuote( metalType, xIgniteToken, xIgniteQuoteProvider.xIgniteCurrency.Euro, out errMsg);			

			if( string.IsNullOrEmpty( errMsg) == false)
			{ 
				this._logger.Error( string.Format( "{0}:{1}", GetType().Name, errMsg));
				return( null);
			}

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, string.Format( "Quote received:Ask={0}, Bid={1}", q.Ask, q.Bid), null);
			q.Provider = "xIgnite";
			return( q);
		}

		/// <summary date="15-01-2021, 12:08:44" author="S.Deckers">
		/// getKitcoQuoteFromInternet
		/// </summary>
		/// <param name="metalType"></param>
		/// <returns></returns>
		private PreciousMetalsQuote getKitcoQuoteFromInternet( PreciousMetalType metalType)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, "Getting Quote", null);

			string errMsg;
			PreciousMetalsQuote q = KitcoQuoteProvider.GetQuote( metalType, out errMsg);

			if( string.IsNullOrEmpty( errMsg) == false)
			{ 
				this._logger.Error( string.Format( "{0}:{1}", GetType().Name, errMsg));
				return( null);
			}

			this._logger.InsertLog( Nop.Core.Domain.Logging.LogLevel.Information, GetType().Name, string.Format( "Quote received:Ask={0}, Bid={1}", q.Ask, q.Bid), null);
			q.Provider = "Kitco";
			return( q);
		}
	}
}