/**
 * @Name Enum.cs
 * @Purpose 
 * @Date 12 January 2021, 12:12:28
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Domain
{
	#region -- Using directives --
	using LinqToDB.Mapping;
	#endregion

	public enum PreciousMetalType
    {
		[ MapValue( 0)]	Unknown		= 0
	,	[ MapValue( 1)]	Gold		= 1
	,	[ MapValue( 2)]	Silver		= 2
	,	[ MapValue( 3)]	Platinum	= 3
	,	[ MapValue( 4)]	Palladium	= 4
	,	[ MapValue( 5)]	Rhodium		= 5
    }

    public enum PreciousPriceCalculationType
    {
		[ MapValue( 1)]	MultiplyFirstThenAdd = 1
	,	[ MapValue( 2)]	AddFirstThenMultiply = 2
    }

    public enum PreciousMetalsQuoteType
    {
		[ MapValue( 1)]	Bid		= 1
	,	[ MapValue( 2)]	Ask		= 2
	,	[ MapValue( 3)]	Low		= 3
	,	[ MapValue( 4)]	High	= 4
    }

    public enum PreciousMetalsTierPriceType
    {
		[ MapValue( 1)]	DoNotUse		= 1
	,	[ MapValue( 2)]	Percentage		= 2
	,	[ MapValue( 3)]	PriceDiscount	= 3
    }

    public enum PriceRoundingType
    {
		[ MapValue( 1)]	None		= 1
	,	[ MapValue( 2)]	RoundUp		= 2
	,	[ MapValue( 3)]	RoundDown	= 3
    }

	public enum QuoteProvider
	{
		Kitco
	,	xIgnite
	};
}
