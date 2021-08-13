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

	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

/*
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Ask] [decimal](18, 4) NOT NULL,
		[Bid] [decimal](18, 4) NOT NULL,
		[Change] [decimal](18, 4) NOT NULL,
		[ChangePercent] [decimal](18, 4) NOT NULL,
		[Date] [datetime] NOT NULL,
		[High] [decimal](18, 4) NOT NULL,
		[Low] [decimal](18, 4) NOT NULL,
		[MetalType] [int] NOT NULL,
		[DateRetrieved] [datetime] NOT NULL,
		[Provider] [varchar](10) NULL 
*/
	public class PreciousMetalsQuote : BaseEntity
	{
		[ Column( ), NotNull( ) ]
		public decimal				Ask				{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal				Bid				{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal				Change			{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal				ChangePercent	{ get; set; }

		[ Column( ), NotNull( ) ]
		public DateTime				Date			{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal				High			{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal				Low				{ get; set; }

		[ Column( ), NotNull( ) ]	
		public PreciousMetalType	MetalType		{ get; set; }

		[ Column( ), NotNull( ) ]
		public DateTime				DateRetrieved	{ get; set; }

		[ Column( )]
		public string				Provider		{ get; set; }

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
