
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	/// <summary>
	///     Specifies a browse option to be used for browsing the Content Directory.
	/// </summary>    
	public enum BrowseFlag
	{
		/// <summary>
		///     Indicates that the properties of the object specified by the ObjectID parameter will be returned in the result.   
		/// </summary>
		BrowseMetadata,

		/// <summary>
		///     Indicates that first level objects under the object specified by ObjectID parameter will be returned in the result, as well as the metadata of all objects specified.
		/// </summary>
		BrowseDirectChildren
	}
}
