/**
 * @Name SupportedProviders.cs
 * @Purpose 
 * @Date 22 January 2021, 20:58:29
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Domain
{
	#region -- Using directives --
	using LinqToDB.Mapping;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.Rendering;
	#endregion

	public static class SupportedProviders
	{ 
		public static List<SelectListItem> Items
		{
			get
			{
				List<SelectListItem> items = new List<SelectListItem>();
				items.Add( new SelectListItem( ) { Text = "Kitco", Value = "0" });
				items.Add( new SelectListItem( ) { Text = "xIgnite", Value = "1" });
				return( items);
			}
		}
	}
}
