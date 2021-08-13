/**
 * @Name PreciousMetalSettings.cs
 * @Purpose 
 * @Date 14 January 2021, 23:23:11
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals
{
	#region -- Using directives --
	using System.Text;
	using Nop.Core.Configuration;
	using Nop.Plugin.Pricing.PreciousMetals.Domain;
	using d=System.Diagnostics.Debug;
	#endregion

    public class PreciousMetalsSettings : ISettings
    {
        public int				CachePeriodInMinutes			{ get; set; }
        public bool				ExcludeFromSubtotalDiscounts	{ get; set; }
		public string			xIgniteToken					{ get; set; }
		public QuoteProvider	QuoteProvider					{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "CachePeriodInMinutes={0}",			this.CachePeriodInMinutes);
			sb.AppendFormat( ", ExcludeFromSubtotalDiscounts={0}",	this.ExcludeFromSubtotalDiscounts);
			sb.AppendFormat( ", xIgniteToken={0}",					this.xIgniteToken);
			sb.AppendFormat( ", QuoteProvider={0}",					this.QuoteProvider );
			return ( sb.ToString( ) );
		}

		public void Dump( )
		{ 
			d.WriteLine( this.ToString( ) );
		}
    }
}
