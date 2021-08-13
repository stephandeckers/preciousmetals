/**
 * @Name PreciousMetalsQuote.cs
 * @Purpose 
 * @Date 12 January 2021, 19:06:32
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Domain
{
	#region -- Using directives --
	using System;
	using System.Text;

	using Nop.Core;
	using LinqToDB.Mapping;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	public class PreciousMetalsQuote : BaseEntity
	{
		public decimal				Ask				{ get; set; }
		public decimal				Bid				{ get; set; }
		public decimal				Change			{ get; set; }
		public decimal				ChangePercent	{ get; set; }
		public DateTime				Date			{ get; set; }
		public decimal				High			{ get; set; }
		public decimal				Low				{ get; set; }
		[ Column( ), NotNull( ) ]	
		public PreciousMetalType	MetalType		{ get; set; }
		public string				Provider		{ get; set; }
		public DateTime				DateRetrieved	{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "id={0}",				this.Id);
			sb.AppendFormat( ", MetalType={0}",		this.MetalType);
			sb.AppendFormat( ", DateRetrieved={0:hh:mm:ss}",	this.DateRetrieved);
			return( sb.ToString( ) );
		}

		public void Dump( )
		{
			d.WriteLine( this.ToString( ) );
		}
	}
}
