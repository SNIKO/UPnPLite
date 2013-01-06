
namespace SV.UPnP.DLNA.Services
{
    using System;
    using Windows.Foundation;

    /// <summary>
    ///     Defines helper methods for parsing the DIDL-Lite XML.
    /// </summary>
    internal static class ParsingHelper
    {
        /// <summary>
        ///     Parses the time span string in format of DIDL-Lite into <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeSpanString">
        ///     The string to parse.
        /// </param>
        /// <returns>
        ///     A <see cref="TimeSpan"/> instance.
        /// </returns>
        /// <remarks>
        ///     The form of the duration string is: H+:MM:SS[.F+], or H+:MM:SS[.F0/F1] where : H+ :  number of digits (including no digits) to indicate elapsed hours, MM : exactly 2 
        ///     digits to indicate minutes (00 to 59), SS : exactly 2 digits to indicate seconds (00 to 59), F+ : any number of digits (including no digits) to indicate fractions of 
        ///     seconds, F0/F1 : a fraction, with F0 and F1 at least one digit long, and F0 less than F1. The string may be preceded by an optional + or – sign, and the decimal point 
        ///     itself may be omitted if there are no fractional second digits.  
        /// </remarks>
        public static TimeSpan ParseTimeSpan(string timeSpanString)
        {
            var splitted = timeSpanString.Split(':');
            var splittedSeconds = splitted[2].Split('.');

            var hours = int.Parse(splitted[0]);
            var minutes = int.Parse(splitted[1]);
            var seconds = int.Parse(splittedSeconds[0]);
            var milliseconds = splittedSeconds.Length == 2 ? int.Parse(splittedSeconds[1]) : 0;

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        ///     Parses the resolution string into <see cref="Size"/>.
        /// </summary>
        /// <param name="resolutionString">
        ///     The string to parse.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Size"/>
        /// </returns>
        /// <remarks>
        ///     String pattern is of the form: [0-9]+x[0-9]+ (one or more digits,'x', followed by one or more digits).
        /// </remarks>
        public static Size ParseResolution(string resolutionString)
        {
            var splitted = resolutionString.Split('x');

            var width = int.Parse(splitted[0]);
            var height = int.Parse(splitted[1]);

            return new Size(width, height);
        }
    }
}
