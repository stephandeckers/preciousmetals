/**
 * @Name xIgniteQuoteProvider.cs
 * @Purpose 
 * @Date 15 September 2020, 13:11:42
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Providers
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Net;
	using System.Text;	
	using System.Globalization;
	using System.Text.RegularExpressions;

	using Nop.Core;

	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	using xIgnite;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	internal class xIgniteQuoteProvider
	{
		internal enum xIgniteCurrency
		{
			Euro
		,	Dollar
		}

		internal static PreciousMetalsQuote GetQuote
		( 
				PreciousMetalType	preciousMetalType
		,		string				token
		,		xIgniteCurrency		currency
		,	out string				errMsg
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			
			string theSymbol = string.Empty;

			switch( preciousMetalType)
			{
				case( PreciousMetalType.Gold):
				{
					theSymbol = "XAU";
					break;
				}

				case( PreciousMetalType.Silver):
				{
					theSymbol = "XAG";
					break;
				}

				default:
				{
					errMsg = string.Format( "Unsupported metaltype:[{0}]", preciousMetalType.ToString( ));
					return( null);
				}
			}

			PreciousMetalsQuote quote = getxIgniteQuote( theSymbol, token, currency, out errMsg);
			return( quote);
		}

		/// <summary date="12-09-2020, 07:03:29" author="S.Deckers">
		/// Get text quote
		/// </summary>
		private static PreciousMetalsQuote getxIgniteQuote
		( 
				string				symbol
		,		string				token
		,		xIgniteCurrency		currency
		,	out string				errMsg
		)
		{
			/// <summary date="15-09-2020, 15:25:37" author="S.Deckers">
			/// Get date from xIginite presentation
			/// </summary>			
			DateTime getDate( string sDate)
			{
				d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

				DateTime		theDate;
				CultureInfo		cultureInfo		= CultureInfo.InvariantCulture;
				DateTimeStyles	dateTimeStyles	= DateTimeStyles.AssumeUniversal | DateTimeStyles.AssumeUniversal;

				if( DateTime.TryParseExact( sDate, "MM/dd/yyyy", cultureInfo, dateTimeStyles, out theDate) == false)
				{				
					return( DateTime.MinValue);
				}

				return( theDate);
			}

			xIgnite.XigniteGlobalMetalsSoapClient client = new xIgnite.XigniteGlobalMetalsSoapClient( xIgnite.XigniteGlobalMetalsSoapClient.EndpointConfiguration.XigniteGlobalMetalsSoap);

			errMsg = string.Empty;

			string theCurrency = "EUR";

			if( currency == xIgniteCurrency.Dollar)
			{
				theCurrency = "USD";
			}

			xIgnite.Header		header		= new Header( ) { Username = token };
			xIgnite.MetalQuote	metalQuote	= client.GetRealTimeMetalQuote( Header:header, Symbol:symbol, Currency:theCurrency);

			if( metalQuote.Outcome != OutcomeTypes.Success)
			{
				errMsg = string.Format( "XigniteGlobalMetals failure:[{0}]", metalQuote.Message);
				return( null);
			}

			PreciousMetalType metalType = PreciousMetalType.Unknown;

			if( symbol == "XAU") metalType = PreciousMetalType.Gold;
			if( symbol == "XAG") metalType = PreciousMetalType.Silver;

			PreciousMetalsQuote q = new PreciousMetalsQuote( );
			q.DateRetrieved = System.DateTime.Now;
			q.MetalType		= metalType;
			q.Bid			= System.Convert.ToDecimal( metalQuote.Bid);
			q.Ask			= System.Convert.ToDecimal( metalQuote.Ask);
			q.Date			= getDate( metalQuote.Date);

			// --- The following properties are not available at xIgnite (20151211 SDE)

			q.Change		= 0.0M;
			q.ChangePercent = 0.0M;
			q.Low			= 0.0M;
			q.High			= 0.0M;
			q.ChangePercent	= 0.0M;

			return( q);
		}
	}

	internal static class PreciousMetalsQuoteExtensions
	{
		public static void Dump( this xIgnite.MetalQuote q)
		{
			StringBuilder sb = new StringBuilder( );
			sb.AppendFormat( "Ask={0}",			q.Ask);
			sb.AppendFormat( ", Bid={0}",		q.Bid);
			sb.AppendFormat( ", Currency={0}",	q.Currency);
			sb.AppendFormat( ", Date={0}",		q.Date);
			sb.AppendFormat( ", Delay={0}",		q.Delay);
			sb.AppendFormat( ", Identity={0}",	q.Identity);
			sb.AppendFormat( ", Message={0}",	q.Message);
			sb.AppendFormat( ", Mid={0}",		q.Mid);
			sb.AppendFormat( ", Name={0}",		q.Name);
			sb.AppendFormat( ", Outcome={0}",	q.Outcome);
			sb.AppendFormat( ", QuoteType={0}",	q.QuoteType);
			sb.AppendFormat( ", Source={0}",	q.Source);
			sb.AppendFormat( ", Spread={0}",	q.Spread);
			sb.AppendFormat( ", Symbol={0}",	q.Symbol);
			sb.AppendFormat( ", Time={0}",		q.Time);
			sb.AppendFormat( ", Unit={0}",		q.Unit);
			d.WriteLine( sb.ToString( ));
		}
	}
}
