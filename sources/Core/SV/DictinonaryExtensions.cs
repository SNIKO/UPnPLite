
namespace SV
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Defines extension methods for <see cref="Dictionary"/>.
    /// </summary>
    public static class DictinonaryExtensions
    {
        /// <summary>
        ///     Gets a value of type <typeparamref name="TValue"/> associated with <see cref="key"/>.
        /// </summary>
        /// <typeparam name="TValue">
        ///     The type of the value to return.
        ///     The next types are supported:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="int"/>
        ///         </item>
        ///         <item>
        ///             <see cref="bool"/>
        ///         </item>
        ///         <item>
        ///             <see cref="Uri"/>
        ///         </item>
        ///         <item>
        ///             <see cref="string"/>
        ///         </item>
        ///     </list>
        /// </typeparam>
        /// <param name="instance">
        ///     In instance of the <see cref="Dictionary"/>.
        /// </param>
        /// <param name="key">
        ///     The key of the value to get.
        /// </param>
        /// <returns>
        ///     If <paramref name="key"/> exists, the value of type <typeparamref name="TValue"/> is returned. If <paramref name="key"/> not exists or 
        ///     the value could not be parsed to <typeparamref name="TValue"/>, then the default value of type <typeparamref name="TValue"/> is returned.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///     The <typeparamref name="TValue"/> is not supported.
        /// </exception>
        public static TValue GetValueOrDefault<TValue>(this Dictionary<string, string> instance, string key)
        {
            instance.EnsureNotNull("instance");
            TValue result;

            string value;
            if (instance.TryGetValue(key, out value))
            {
                if (typeof(TValue) == typeof(int))
                {
                    int intResult;
                    if (int.TryParse(value, out intResult))
                    {
                        result = (TValue)(object)intResult;
                    }
                    else
                    {
                        result = (TValue)(object)default(int);
                    }
                }
                else if (typeof(TValue) == typeof(Uri))
                {
                    Uri uriResult;
                    if (Uri.TryCreate(value, UriKind.Absolute, out uriResult))
                    {
                        result = (TValue)(object)uriResult;
                    }
                    else
                    {
                        result = (TValue)(object)null;
                    }
                }
                else if (typeof(TValue) == typeof(bool))
                {
                    result = (TValue)(object)value.ToBool();
                }
                else if (typeof(TValue) == typeof(string))
                {
                    result = (TValue) (object) value;
                }
                else
                {
                    throw new NotSupportedException("The value type '{0}' is not supported".F(typeof(TValue).Name));
                }
            }
            else
            {
                result = default(TValue);
            }

            return result;
        }
    }
}
