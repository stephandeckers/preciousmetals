/**
 * @Name PreciousMetalsController.cs
 * @Purpose 
 * @Date 22 January 2021, 14:13:06
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Controller
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Plugin.Pricing.PreciousMetals.Model;
	using Nop.Services.Configuration;
	using Nop.Services.Localization;
	using Nop.Services.Messages;
	using Nop.Services.Security;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;
	using Nop.Plugin.Pricing.PreciousMetals;
	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	/// <summary date="22-01-2021, 14:14:24" author="S.Deckers">
	/// PreciousMetalsController
	/// </summary>
    [ Area(AreaNames.Admin)]
    [ AutoValidateAntiforgeryToken]
    public class PreciousMetalsController : BasePluginController
    {
        private readonly ILocalizationService	_localizationService;
        private readonly INotificationService	_notificationService;
        private readonly IPermissionService		_permissionService;
        private readonly ISettingService		_settingService;
        private readonly IStoreContext			_storeContext;

        public PreciousMetalsController
		(
			ILocalizationService	localizationService
		,	INotificationService	notificationService
		,	IPermissionService		permissionService
		,	ISettingService			settingService
		,	IStoreContext			storeContext
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            this._localizationService	= localizationService;
            this._notificationService	= notificationService;
            this._permissionService		= permissionService;
            this._settingService		= settingService;
            this._storeContext			= storeContext;
        }

        public IActionResult Configure()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            int						storeScope				= _storeContext.ActiveStoreScopeConfiguration;

            PreciousMetalsSettings	preciousMetalsSettings	= _settingService.LoadSetting<PreciousMetalsSettings>(storeScope);
            ConfigurationModel		model					= new ConfigurationModel();

			model.CachePeriodInMinutes			= preciousMetalsSettings.CachePeriodInMinutes;
			model.ExcludeFromSubtotalDiscounts	= preciousMetalsSettings.ExcludeFromSubtotalDiscounts;
			model.QuoteProvider					= preciousMetalsSettings.QuoteProvider;
			model.xIgniteToken					= preciousMetalsSettings.xIgniteToken;

			if( storeScope > 0)
			{
				model.CachePeriodInMinutes_OverrideForStore			= _settingService.SettingExists( preciousMetalsSettings, x => x.CachePeriodInMinutes, storeScope);
				model.ExcludeFromSubtotalDiscounts_OverrideForStore	= _settingService.SettingExists( preciousMetalsSettings, x => x.ExcludeFromSubtotalDiscounts, storeScope);
				model.QuoteProvider_OverrideForStore				= _settingService.SettingExists( preciousMetalsSettings, x => x.QuoteProvider, storeScope);
				model.xIgniteToken_OverrideForStore					= _settingService.SettingExists( preciousMetalsSettings, x => x.xIgniteToken, storeScope);
			}

			return View( Constants.ViewLocations.ConfigureView, model);
        }

        [ HttpPost( )]
        public IActionResult Configure( ConfigurationModel model)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            int						storeScope				= _storeContext.ActiveStoreScopeConfiguration;
			PreciousMetalsSettings	preciousMetalsSettings	= _settingService.LoadSetting<PreciousMetalsSettings>(storeScope);

			preciousMetalsSettings.CachePeriodInMinutes			= model.CachePeriodInMinutes;
			preciousMetalsSettings.ExcludeFromSubtotalDiscounts	= model.ExcludeFromSubtotalDiscounts;
			preciousMetalsSettings.QuoteProvider				= model.QuoteProvider;
			preciousMetalsSettings.xIgniteToken					= model.xIgniteToken;
            
			_settingService.SaveSettingOverridablePerStore( preciousMetalsSettings, x => x.CachePeriodInMinutes, model.CachePeriodInMinutes_OverrideForStore, storeScope, false);
			_settingService.SaveSettingOverridablePerStore( preciousMetalsSettings, x => x.ExcludeFromSubtotalDiscounts, model.ExcludeFromSubtotalDiscounts_OverrideForStore, storeScope, false);
			_settingService.SaveSettingOverridablePerStore( preciousMetalsSettings, x => x.QuoteProvider, model.QuoteProvider_OverrideForStore, storeScope, false);
			_settingService.SaveSettingOverridablePerStore( preciousMetalsSettings, x => x.xIgniteToken, model.xIgniteToken_OverrideForStore, storeScope, false);

            // --- Clear caches (20210123 SDE)

            _settingService.ClearCache();

			IStaticCacheManager staticCacheManager	= Nop.Core.Infrastructure.EngineContext.Current.Resolve<IStaticCacheManager>( );
			staticCacheManager.Clear( );

            _notificationService.SuccessNotification( _localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}