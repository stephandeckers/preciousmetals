/**
 * @Name PreciousMetalsPlugin.cs
 * @Purpose 
 * @Date 12 January 2021, 07:46:04
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Builder;

	using Nop.Core;
	using Nop.Core.Infrastructure;
	using Nop.Services.Cms;
	using Nop.Services.Plugins;
	using Nop.Web.Framework.Menu;
	using Nop.Services.Directory;	
	using Nop.Core.Domain.Directory;	
	using Nop.Services.Localization;	
	using Nop.Services.Configuration;
	using Nop.Core.Domain.Configuration;	
	using Nop.Web.Framework.Infrastructure;
	
	using Nop.Services.Tasks;
	using Nop.Plugin.Pricing.PreciousMetals.Constants;
	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	
	#endregion

	public partial class PreciousMetalsPlugin : 
		BasePlugin
	,	IAdminMenuPlugin
	,	IWidgetPlugin
	{
		private readonly ILocalizationService	_localizationService;
		private readonly IWebHelper				_webHelper;
		private readonly ISettingService		_settingService;
		private readonly IMeasureService		_measureService;
		private readonly MeasureSettings		_measureSettings;

		/// <summary date="14-01-2021, 11:41:08" author="S.Deckers">
		/// Construction
		/// </summary>
		/// <param name="scheduleTaskService"></param>
		/// <param name="settingService"></param>
		public PreciousMetalsPlugin
		( 
			ILocalizationService	localizationService
		,	IWebHelper				webHelper
		,	ISettingService			settingService
		,	IMeasureService			measureService
		,	MeasureSettings			measureSettings
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._localizationService	= localizationService;
			this._webHelper				= webHelper;
			this._settingService		= settingService;
			this._measureService		= measureService;
			this._measureSettings		= measureSettings;
		}

		/// <summary date="22-01-2021, 13:40:23" author="S.Deckers">
		/// GetConfigurationPageUrl
		/// </summary>
		/// <returns></returns>
		public override string GetConfigurationPageUrl()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string storeLocation	= _webHelper.GetStoreLocation();
			string configUrl		= "Admin/PreciousMetals/Configure";
			string theUrl			= string.Format( "{0}{1}", storeLocation, configUrl);
			return( theUrl);
		}

		/// <summary date="22-01-2021, 13:40:31" author="S.Deckers">
		/// ManageSiteMap
		/// </summary>
		/// <param name="rootNode"></param>
		public void ManageSiteMap(SiteMapNode rootNode)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string theUrl = GetConfigurationPageUrl( );

			SiteMapNode menuItem = new SiteMapNode()
			{
				SystemName	= "Precious Metals"
			,	Title		= "Precious Metals Pricing"
			,	Url			= theUrl
			,	Visible		= true
			,	IconClass	= "fa fa-dot-circle-o"
			};

			SiteMapNode pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
			if( pluginNode != null)
			{
				pluginNode.ChildNodes.Add(menuItem);
			}
			else
			{
				rootNode.ChildNodes.Add(menuItem);
			}
		}

		/// <summary>
		/// Install the plugin
		/// </summary>
		public override void Install()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			installResources	( );
			installSettings		( );

			/* --- Enable this for fresh installs, if you have already existing entries in table PreciousMetalDetails the WeightId column needs
					to match existing entries (20210115 SDE)
					
			installTroyOunceWeight	( );
			*/

			base.Install();
		}

		/// <summary date="22-01-2021, 13:37:21" author="S.Deckers">
		/// Install resources
		/// </summary>
		private void installResources ( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.CachePeriodInMinutes,				"Cache period in minutes");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.CachePeriodInMinutesHint,			"Cache period in minutes (set to zero or a negative value to skip caching)");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.ExcludeFromSubtotalDiscounts,		"Exclude from subtotal discounts");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.ExcludeFromSubtotalDiscountsHint,	"Exclude from subtotal discounts");

            _localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.QuoteProvider,					"Quote Provider");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.QuoteProviderHint,				"Quote Provider to use (Kitco/xIgnite)");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.xIgniteToken,						"xIgnite Token");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.xIgniteTokenHint,					"Token for xIgnite (only when xIgnite is chosen as provider)");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.IsPreciousMetalEnabled,			"Enabled");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.IsPreciousMetalEnabledHint,		"Product is a precious metal or not");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.MetalType,						"Metal type");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.MetalTypeHint,					"Type of Metal");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.QuoteType,						"Quote type");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.QuoteTypeHint,					"Type of Quote");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.TierPricingType,					"Tier Pricing Type");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.TierPricingTypeHint,				"Tiers Pricing Type");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.LowerAmount4PriceCalculation,		"Lower amount for price calculation");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.LowerAmount4PriceCalculationHint,	"Lower amount for price calculation");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.Weight,							"Weight");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.WeightHint,						"Weight");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.WeightUnit,						"Weight Unit");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.WeightUnitHint,					"Weight Unit");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.MathType,							"Type of price calculation'");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.MathTypeHint,						"Type of price calculation'");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PercentMarkup,					"Percent Markup");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PercentMarkupHint,				"Percent Markup");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.FlatMarkup,						"Flat markup");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.FlatMarkupHint,					"Flat markup");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PriceRoundingType,				"Price rounding type");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PriceRoundingTypeHint,			"Price rounding type");

			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PriceRoundingNumber,				"Price rounding number");
			_localizationService.AddOrUpdatePluginLocaleResource( Constants.Resources.PriceRoundingNumberHint,			"Price rounding number");
		}

		/// <summary date="10-09-2020, 11:06:18" author="S.Deckers">
		/// Install settings needed for this plugin
		/// </summary>
		private void installSettings()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			PreciousMetalsSettings settings = new PreciousMetalsSettings( ) 
			{ 
				QuoteProvider					= Domain.QuoteProvider.xIgnite
			,	CachePeriodInMinutes			= 5
			,	ExcludeFromSubtotalDiscounts	= false
			,	xIgniteToken					= "2do:update with your actual token"
			};

			_settingService.SaveSetting( settings);
		}

		/// <summary>
		/// Uninstall the plugin
		/// </summary>
		public override void Uninstall()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			uninstallResources	( );
			uninstallSettings	( );

			base.Uninstall();
		}

		/// <summary date="22-01-2021, 13:38:56" author="S.Deckers">
		/// uninstallResources
		/// </summary>
		private void uninstallResources ( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			_localizationService.DeletePluginLocaleResource( Constants.Resources.CachePeriodInMinutes);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.CachePeriodInMinutesHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.ExcludeFromSubtotalDiscounts);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.ExcludeFromSubtotalDiscountsHint);

            _localizationService.DeletePluginLocaleResource( Constants.Resources.QuoteProvider);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.QuoteProviderHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.xIgniteToken);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.xIgniteTokenHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.IsPreciousMetalEnabled);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.IsPreciousMetalEnabledHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.MetalType);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.MetalTypeHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.QuoteType);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.QuoteTypeHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.TierPricingType);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.TierPricingTypeHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.LowerAmount4PriceCalculation);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.LowerAmount4PriceCalculationHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.Weight);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.WeightHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.WeightUnit);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.WeightUnitHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.MathType);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.MathTypeHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.PercentMarkup);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.PercentMarkupHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.FlatMarkup);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.FlatMarkupHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.PriceRoundingType);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.PriceRoundingTypeHint);

			_localizationService.DeletePluginLocaleResource( Constants.Resources.PriceRoundingNumber);
			_localizationService.DeletePluginLocaleResource( Constants.Resources.PriceRoundingNumberHint);
		}

		/// <summary date="10-09-2020, 11:33:30" author="S.Deckers">
		/// Uninstall task
		/// </summary>
		private void uninstallSettings( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			void deleteSetting( string theSetting)
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				Setting setting = _settingService.GetSetting( theSetting);

				if( setting == null)
				{
					return;
				}

				_settingService.DeleteSetting( setting);
			}

			deleteSetting( "preciousmetalssettings.quoteprovider");
			deleteSetting( "preciousmetalssettings.xignitetoken");
			deleteSetting( "preciousmetalssettings.cacheperiodinminutes");
			deleteSetting( "preciousmetalssettings.excludefromsubtotaldiscounts");
		}

		/// <summary>
		/// installTroyOunceWeight
		/// </summary>
		private void installTroyOunceWeight()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			// figure out the correct ratio to use based on one of the four preinstalled system weights
			var primaryWeight = this._measureService.GetMeasureWeightById(this._measureSettings.BaseWeightId);
			decimal ratio;
			switch(primaryWeight.SystemKeyword)
			{
				case "grams":
					ratio = 0.0321507466m;
					break;
				case "ounce":
					ratio = 0.911458333m;
					break;
				case "lb":
					ratio = 14.5833333m;
					break;
				case "kg":
					ratio = 32.1507466m;
					break;
				default:
					ratio = 0;
					break;
			}

			// make sure troyounce weight exists and is correct
			var weight = _measureService.GetMeasureWeightBySystemKeyword("troyounce");
			if (weight != null)
			{
				// if it's not correct, then update it
				if (weight.Ratio != ratio)
				{
					weight.Ratio = ratio;
					_measureService.UpdateMeasureWeight(weight);
				}

				// since it exists, we can quit now.
				return;
			}

			// add the new weight
			var order = this._measureService.GetAllMeasureWeights().Max(m => m.DisplayOrder) + 1;
			weight = new MeasureWeight { Name = "troy ounce(s)", SystemKeyword = "troyounce", Ratio = ratio, DisplayOrder = order };
			this._measureService.InsertMeasureWeight(weight);
		}
	}

	/// <summary>
	/// IWidgetPlugin implementation
	/// </summary>
	public partial class PreciousMetalsPlugin
	{
		public bool HideInWidgetList => false;

		public IList<string> GetWidgetZones()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			return( new List<string>{ AdminWidgetZones.ProductDetailsBlock });
		}

		public string GetWidgetViewComponentName( string widgetZone)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			if (widgetZone.Equals( AdminWidgetZones.ProductDetailsBlock))
			{
				return( Nop.Plugin.Pricing.PreciousMetals.Constants.ViewNames.ComponentViewName);
			}

			return( string.Empty);
		}
	}
}
