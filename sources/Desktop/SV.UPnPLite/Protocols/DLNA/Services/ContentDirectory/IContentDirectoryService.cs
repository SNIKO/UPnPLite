
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Protocols.UPnP;
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	///     Defines members for managing the media on a MediaServer.
	/// </summary>
	public interface IContentDirectoryService
	{
		/// <summary>
		///     Allows the caller to incrementally browse the native hierarchy of the Content Directory objects exposed by the <see cref="ContentDirectoryService"/>, including 
		///     information listing the classes of objects available in any particular object container. 
		/// </summary>
		/// <param name="objectId">
		///     Object currently being browsed.  An ObjectID value of zero corresponds to the root object of the Content Directory.
		/// </param>
		/// <param name="browseFlag">
		///     Specifies a browse option to be used for browsing the Content Directory.
		/// </param>
		/// <param name="filter">
		///     The comma-separated list of property specifiers (including namespaces) indicates which metadata properties are to be 
		///     returned in the results from browsing or searching.
		/// </param>
		/// <param name="startingIndex">
		///     Starting zero based offset to enumerate children under the container specified by <paramref name="objectId"/>. Must be 0 if <paramref name="browseFlag"/> is equal 
		///     to <see cref="BrowseFlag.BrowseMetadata"/>.
		/// </param>
		/// <param name="requestedCount">
		///     Requested number of entries under the object specified by <paramref name="objectId"/>. The value '0' indicates request all entries.
		/// </param>
		/// <param name="sortCriteria">
		///     A CSV list of signed property names, where signed means preceded by ‘+’ or ‘-’ sign.  The ‘+’ and ‘-’Indicate the sort is in ascending or descending order, 
		///     respectively, with regard to the value of its associated property. Properties appear in the list in order of descending sort priority.
		/// </param>
		/// <returns>
		///     A <see cref="BrowseResult"/> instance which contains result of the Browse operation.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<BrowseResult> BrowseAsync(string objectId, BrowseFlag browseFlag, string filter, int startingIndex, int requestedCount, string sortCriteria);

		/// <summary>
		///     Searches the content directory for objects that match some search criteria.
		/// </summary>
		/// <param name="containerId">
		///     The id of the conainer in which to proceed search.
		/// </param>
		/// <param name="searchCriteria">
		///     One or more search criteria to be used for querying the Content Directory.
		/// </param>
		/// <param name="filter">
		///     The comma-separated list of property specifiers (including namespaces) indicates which metadata properties are to be 
		///     returned in the results from browsing or searching.
		/// </param>
		/// <param name="startingIndex">
		///     Starting zero based offset to enumerate children under the container specified by <paramref name="containerId"/>.
		/// </param>
		/// <param name="requestedCount">
		///     Requested number of entries under the object specified by <paramref name="containerId"/>. The value '0' indicates request all entries.
		/// </param>
		/// <param name="sortCriteria">
		///     A CSV list of signed property names, where signed means preceded by ‘+’ or ‘-’ sign.  The ‘+’ and ‘-’Indicate the sort is in ascending or descending order, 
		///     respectively, with regard to the value of its associated property. Properties appear in the list in order of descending sort priority.
		/// </param>
		/// <returns>
		///     A <see cref="BrowseResult"/> instance which contains result of the Search operation.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<BrowseResult> SearchAsync(string containerId, string searchCriteria, string filter, int startingIndex, int requestedCount, string sortCriteria);

		/// <summary>
		///     Returns the searching capabilities that are supported by the device. 
		/// </summary>
		/// <returns>
		///     The list of property names that can be used in search queries. An empty list indicates that the <see cref="IContentDirectoryService"/> does not support any 
		///     kind of searching. A wildcard (‘*’) indicates that the device supports search queries using all tags present in the <see cref="IContentDirectoryService"/>.
		/// </returns>
		/// <exception cref="WebException">
		///     An error occurred when sending request to service.
		/// </exception>
		/// <exception cref="FormatException">
		///     Received result is in a bad format.
		/// </exception>
		/// <exception cref="UPnPServiceException">
		///     An internal service error occurred when executing request.
		/// </exception>
		Task<IEnumerable<string>> GetSearchCapabilitiesAsync();
	}
}