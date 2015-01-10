
namespace SV.UPnPLite.Protocols.SSDP.Messages
{
	using SV.UPnPLite.Extensions;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///     Defines a response to a MSearch request message.
	/// </summary>
	internal class SearchResponseMessage : SSDPMessage
	{
		private const string RFC1123DateFormat = "ddd, dd MMM yyyy hh:mm:ss EST";

		/// <summary>
		///     Gets the date when response was generated.
		/// </summary>
		public DateTime Date { get; private set; }

		/// <summary>
		///     Gets the target of the search request.
		/// </summary>
		public string SearchTarget { get; private set; }

		/// <summary>
		///     Creates a new instance of <see cref="SearchResponseMessage"/> from the M-Search response.
		/// </summary>
		/// <param name="message">
		///     The response received for M-Search request.
		/// </param>
		/// <returns>
		///     A new instance of <see cref="SearchResponseMessage"/>.
		/// </returns>
		/// <exception cref="KeyNotFoundException">
		///     One of the reuqired headers not found.
		/// </exception>
		/// <exception cref="ArgumentException">
		///     The <paramref name="message"/> is not valid search repsonse message.
		/// </exception>
		internal static SearchResponseMessage Create(string message)
		{
			var response = new SearchResponseMessage();
			var lines = message.SplitIntoLines();

			if (lines.Count() > 1)
			{
				var statusString = lines[0];
				if (StringComparer.OrdinalIgnoreCase.Compare(statusString, "HTTP/1.1 200 OK") == 0)
				{
					var headers = ParseHeaders(lines.Skip(1));

					try
					{
						response.MaxAge 	  = ParseMaxAge(headers.GetValue<string>("CACHE-CONTROL"));
						response.Location 	  = headers.GetValue<string>("LOCATION");
						response.SearchTarget = headers.GetValue<string>("ST");
						response.Server 	  = headers.GetValue<string>("SERVER");
						response.USN 		  = headers.GetValue<string>("USN");

						response.BootId 	= headers.GetValueOrDefault<int>("BOOTID.UPNP.ORG");
						response.ConfigId 	= headers.GetValueOrDefault<int>("CONFIGID.UPNP.ORG");
						response.SearchPort = headers.GetValueOrDefault<int>("SEARCHPORT.UPNP.ORG");

						// TODO: Parse Date
						////message.Date = headers.GetValueOrDefault<string>("DATE");
					}
					catch (KeyNotFoundException ex)
					{
						throw new ArgumentException("The given message is not valid search response message", "message", ex);
					}
					catch (FormatException ex)
					{
						throw new ArgumentException("The given message is not valid search response message", "message", ex);
					}
				}
				else
				{
					throw new ArgumentException("The given message is not valid search response message", "message");
				}
			}
			else
			{
				throw new ArgumentException("The given message is not valid search response message", "message");
			}

			return response;
		}
	}
}
