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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;



#endregion

/*
CREATE TABLE [dbo].[PreciousMetalsDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantId] [int] NULL,
	[MetalType] [int] NOT NULL,
	[QuoteType] [int] NOT NULL,
	[Weight] [decimal](18, 4) NOT NULL,
	[PercentMarkup] [decimal](18, 4) NOT NULL,
	[FlatMarkup] [decimal](18, 4) NOT NULL,
	[TierPriceType] [int] NOT NULL,
	[MathType] [int] NOT NULL,
	[LowerAmount] [decimal](18, 2) NOT NULL,
	[WeightId] [int] NOT NULL,
	[PriceRounding] [int] NOT NULL,
	[PriceRoundingType] [int] NOT NULL,
	[ProductId] [int] NULL,
	[Sku] [nvarchar](400) NULL
) ON [PRIMARY]
*/
	public class PreciousMetalsDetail : BaseEntity
	{
		[ PrimaryKey( )]
		public int							ProductId			{ get; set; }

		[ Column( ), NotNull( ) ]	
		public PreciousMetalType			MetalType			{ get; set; }

		[ Column( ) ]
		public int			ProductVariantId			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousMetalsQuoteType		QuoteType			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousPriceCalculationType	MathType			{ get; set; }

		/*
CREATE TABLE [dbo].[PreciousMetalsDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantId] [int] NULL,
	[MetalType] [int] NOT NULL,
	[QuoteType] [int] NOT NULL,
	[Weight] [decimal](18, 4) NOT NULL,
	[PercentMarkup] [decimal](18, 4) NOT NULL,
	[FlatMarkup] [decimal](18, 4) NOT NULL,
	[TierPriceType] [int] NOT NULL,
	[MathType] [int] NOT NULL,
	[LowerAmount] [decimal](18, 2) NOT NULL,
	[WeightId] [int] NOT NULL,
	[PriceRounding] [int] NOT NULL,
	[PriceRoundingType] [int] NOT NULL,
	[ProductId] [int] NULL,
	[Sku] [nvarchar](400) NULL
) ON [PRIMARY]
*/

		[ Column( ), NotNull( ) ]
		public decimal						Weight				{ get; set; }

		[ Column( ), NotNull( ) ]
		public int							WeightId			{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal						PercentMarkup		{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal						FlatMarkup			{ get; set; }

		[ Column( ), NotNull( ) ]
		public PreciousMetalsTierPriceType	TierPriceType		{ get; set; }

		[ Column( ), NotNull( ) ]
		public decimal						LowerAmount			{ get; set; }

		[ Column( ), NotNull( ) ]
		public int							PriceRounding		{ get; set; }

		[ Column( ), NotNull( ) ]
		public PriceRoundingType			PriceRoundingType	{ get; set; }

		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Id={0}",					this.Id);
			//sb.AppendFormat( ", ProductId={0}",			this.ProductId);
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
