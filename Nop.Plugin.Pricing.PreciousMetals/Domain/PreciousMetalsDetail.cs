/**
 * @Name PreciousMetalsDetail.cs
 * @Purpose 
 * @Date 12 January 2021, 12:04:23
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

	public class PreciousMetalsDetail : BaseEntity
	{
		public int							ProductId			{ get; set; }

		[ Column( ), NotNull( ) ]	
		public PreciousMetalType			MetalType			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousMetalsQuoteType		QuoteType			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousPriceCalculationType	MathType			{ get; set; }

        public decimal						Weight				{ get; set; }
        public int							WeightId			{ get; set; }
        public decimal						PercentMarkup		{ get; set; }
        public decimal						FlatMarkup			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousMetalsTierPriceType	TierPriceType		{ get; set; }

        public decimal						LowerAmount			{ get; set; }
        public int							PriceRounding		{ get; set; }

		[ Column( ), NotNull( ) ]
		public PriceRoundingType			PriceRoundingType	{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",					this.Id);
			sb.AppendFormat( ", ProductId={0}",			this.ProductId);
			sb.AppendFormat( ", MetalType={0}",			this.MetalType);
			sb.AppendFormat( ", QuoteType={0}",			this.QuoteType);
			sb.AppendFormat( ", MathType={0}",			this.MathType);
			sb.AppendFormat( ", Weight={0}",			this.Weight);
			sb.AppendFormat( ", PercentMarkup={0}",		this.PercentMarkup);
			sb.AppendFormat( ", FlatMarkup={0}",		this.FlatMarkup);
			sb.AppendFormat( ", TierPriceType={0}",		this.TierPriceType);
			sb.AppendFormat( ", LowerAmount={0}",		this.LowerAmount);
			sb.AppendFormat( ", PriceRounding={0}",		this.PriceRounding);
			sb.AppendFormat( ", PriceRoundingType={0}",	this.PriceRoundingType);

			return( sb.ToString( ) );
		}

		public void Dump( )
		{
			d.WriteLine( this.ToString( ) );
		}
	}
}
