/**
 * @Name KitcoQuoteProvider.cs
 * @Purpose 
 * @Date 14 September 2020, 08:50:08
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
	using Nop.Services.Directory;
	using Nop.Core.Domain.Directory;
	
	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	internal class KitcoQuoteProvider
	{
		public static PreciousMetalsQuote GetQuote( PreciousMetalType preciousMetalType, out string errMsg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			PreciousMetalsQuote quote = getKitcoTextQuote( preciousMetalType, out errMsg);
			return( quote);
		}

		/// <summary date="12-09-2020, 07:03:29" author="S.Deckers">
		/// Get text quote
		/// </summary>
		private static PreciousMetalsQuote getKitcoTextQuote
		( 
				PreciousMetalType	preciousMetalType
		,	out string				errMsg
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			errMsg = string.Empty;

			string url		= "http://www.kitco.com/texten/texten.html";
            string response	= getWebSiteContent(url);
            if( response == null)
			{
				return( null);
			}

			string wrdBegin	= "New York Spot Price";
			string wrdEnd	= "Asia / Europe Spot Price";

			// --- Get quotes for NY (20200912 SDE)

			string nyQuotes = Regex.Split( Regex.Split( response, wrdEnd)[ 0], wrdBegin)[ 1];

			// --- Date (20200914 SDE)

			string delim	= Regex.Split( nyQuotes, "Last Update on")[ 1];
			string []s		= Regex.Split( delim, "\r\n");
			
			string			sDate			= s[ 0].Trim( );
			CultureInfo		cultureInfo		= CultureInfo.InvariantCulture;
			DateTimeStyles	dateTimeStyles	= DateTimeStyles.AssumeUniversal | DateTimeStyles.AssumeUniversal;

			DateTime theDate;
			
			if( DateTime.TryParseExact( sDate, "MM/dd/yyyy HH:mm", cultureInfo, dateTimeStyles, out theDate) == false)
			{				
				errMsg = string.Format( "Unable to convert [{0}] to EST datetime", sDate);
				d.WriteLine( errMsg);
				return( null);
			}

			PreciousMetalsQuote quote = getQuote( nyQuotes, preciousMetalType, theDate);

			return( quote);
		}

		/// <summary date="14-09-2020, 08:05:22" author="S.Deckers">
		/// Get quote
		/// </summary>
		/// <param name="stringWithQuotes"></param>
		/// <param name="preciousMetalType"></param>
		/// <param name="date"></param>
		/// <returns></returns>
		private static PreciousMetalsQuote getQuote
		( 
			string				stringWithQuotes
		,	PreciousMetalType	preciousMetalType
		,	DateTime			date
		)
		{
			string pattern = preciousMetalType.ToString( );
			string part		= Regex.Split( stringWithQuotes, pattern)[ 1];

			string [] s = Regex.Split( part, "\r\n");

			string theQuote	= Regex.Split( s[ 0], "\r\n")[ 0];
			string quotes	= Regex.Replace( Regex.Replace( theQuote.Trim( ), "\t{1,}", " "), " {1,}", "\t");

			string[] quoteCollection = Regex.Split( quotes, "\t");

			// --- The quote 

            NumberFormatInfo numberFormat = new NumberFormatInfo
            {
                NumberDecimalDigits		= 2,
                NumberDecimalSeparator	= ".",
                NumberGroupSeparator	= ""
            };

			PreciousMetalsQuote q = new PreciousMetalsQuote( );
			q.MetalType		= preciousMetalType;
			q.DateRetrieved	= DateTime.Now;
			q.Date			= date; 
			q.Bid			= decimal.Parse( quoteCollection[ 0], numberFormat);
			q.Ask			= decimal.Parse( quoteCollection[ 1], numberFormat);
			q.Change		= decimal.Parse( quoteCollection[ 2], numberFormat);
			q.ChangePercent = decimal.Parse( quoteCollection[ 3].TrimEnd( new [] { '%' }));
			q.Low			= decimal.Parse( quoteCollection[ 4], numberFormat);
			q.High			= decimal.Parse( quoteCollection[ 5], numberFormat);

            // Kitco always returns US Dollars.  Convert to the primary store currency before saving the quote

			ICurrencyService 	currencyService	= Nop.Core.Infrastructure.EngineContext.Current.Resolve<ICurrencyService>( );
			Currency			currencyUSD		= currencyService.GetCurrencyByCode("USD");

            q.Bid		= currencyService.ConvertToPrimaryStoreCurrency( q.Bid, currencyUSD);
            q.Ask		= currencyService.ConvertToPrimaryStoreCurrency( q.Ask, currencyUSD);
            q.Low		= currencyService.ConvertToPrimaryStoreCurrency( q.Low, currencyUSD);
            q.High		= currencyService.ConvertToPrimaryStoreCurrency( q.High, currencyUSD);
            q.Change	= currencyService.ConvertToPrimaryStoreCurrency( q.Change, currencyUSD);

			return( q);
		}

		private static string getWebSiteContent(string url)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

            WebRequest	request		= WebRequest.Create(url);
            WebResponse response	= request.GetResponse();
            try
            {
                // --- Get a stream from the response

                using( Stream stream = response.GetResponseStream())
                {
                    // --- The stream shouldn't be null

                    if( stream == null)
					{
                        return null;
					}

                    // --- We'll need a reader to parse the string

                    using( StreamReader reader = new StreamReader( stream))
                    {
                        // --- Get the response string

                        string strResponse = reader.ReadToEnd();

                        // --- Make sure we close everything
                        reader.Close	( );
                        stream.Close	( );
                        response.Close	( );

                        // return it
                        return( strResponse);
                    }
                }
            }
            catch
            {
                // make sure we always close the HTTP channel.
                response.Close();

                throw;
            }
        }
	}
}
