
namespace SV.UPnPLite.Protocols.UPnP
{
    using SV.UPnPLite.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     Defines an error that occurred on a UPnP Service.
    /// </summary>    
    public class UPnPServiceException : Exception
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPServiceException"/> class.
        /// </summary>
        /// <param name="errorCode">
        ///     The error code which specifies an error.
        /// </param>
        /// <param name="message">
        ///     The message that describes an error.
        /// </param>
        public UPnPServiceException(int errorCode, string message)
            : this(errorCode, message, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPServiceException"/> class.
        /// </summary>
        /// <param name="errorCode">
        ///     The error code which specifies an error.
        /// </param>
        /// <param name="message">
        ///     The message that describes an error.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. 
        /// </param>
        public UPnPServiceException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Defines an error code which specifies an error.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        ///     Gets an action which caused an error.
        /// </summary>
        public string Action { get; internal set; }

        /// <summary>
        ///     Gets an arguments which were passed into action.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> Arguments { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var description = new StringBuilder();
            description.AppendLine("{0}: {1}".F(this.GetType(), this.Message));

            if (string.IsNullOrEmpty(this.Action) == false)
            {
                var action = "Action: {0}".F(this.Action);
                if (this.Arguments != null && this.Arguments.Any())
                {
                    var arguments = string.Join(", ", this.Arguments.Select(a =>
                        {
                            var f = a.Value is string ? "{0}='{1}'" : "{0}={1}";

                            return f.F(a.Key, a.Value);
                        }));

                    action = string.Concat(action, " ({0})".F(arguments));
                    description.AppendLine(action);
                }
            }

            if (this.InnerException != null)
            {
                description.AppendLine(" ---> {0}".F(this.InnerException));
            }


            if (this.StackTrace != null)
            {
                description.AppendLine();
                description.AppendLine(this.StackTrace);
            }

            return description.ToString();
        }

        #endregion
    }
}
