/**
 * @Name Configuration.cs
 * @Purpose 
 * @Date 22 January 2021, 13:58:48
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Model
{
	#region -- Using directives --
	using System.Collections.Generic;
	using Nop.Web.Framework.Models;
	using Nop.Web.Framework.Mvc.ModelBinding;
	using Nop.Plugin.Pricing.PreciousMetals.Domain;
	using Nop.Plugin.Pricing.PreciousMetals.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	#endregion

	public class ConfigurationModel : BaseNopModel
	{
		public int ActiveStoreScopeConfiguration { get; set; }

		[ NopResourceDisplayName( Constants.Resources.CachePeriodInMinutes )]
		public int				CachePeriodInMinutes							{ get; set; }
		public bool				CachePeriodInMinutes_OverrideForStore			{ get; set; }

		[ NopResourceDisplayName( Constants.Resources.ExcludeFromSubtotalDiscounts )]
		public bool				ExcludeFromSubtotalDiscounts					{ get; set; }
		public bool				ExcludeFromSubtotalDiscounts_OverrideForStore	{ get; set; }

		[ NopResourceDisplayName( Constants.Resources.QuoteProvider )]
		public QuoteProvider	QuoteProvider									{ get; set; }
		public bool				QuoteProvider_OverrideForStore					{ get; set; }

		[ NopResourceDisplayName( Constants.Resources.xIgniteToken )]
		public string			xIgniteToken									{ get; set; }
		public bool				xIgniteToken_OverrideForStore					{ get; set; }
	}
}
