
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using SV.UPnPLite.Logging;

	public class PersonContainer : MediaContainer
	{
		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="PersonContainer"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public PersonContainer(ILogManager logManager)
			: base(logManager)
		{
		}

		#endregion
	}
}
