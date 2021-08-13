/**
 * @name PreciousMetals.sql
 * @purpose Create Preciousmetals tables
 * @date 22-Jan-2021
 * @author S.Deckers
 */

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id=object_id('[PreciousMetalsDetail]') and NAME='PreciousMetalsDetail')
BEGIN
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
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id=object_id('[PreciousMetalsQuote]') and NAME='PreciousMetalsQuote')
BEGIN
	CREATE TABLE [dbo].[PreciousMetalsQuote](
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
	) ON [PRIMARY]
END
GO
