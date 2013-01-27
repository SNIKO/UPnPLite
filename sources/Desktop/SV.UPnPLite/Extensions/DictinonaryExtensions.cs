
namespace SV.UPnPLite.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Defines extension methods for <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
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
        ///             <see cref="uint"/>
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
        ///     In instance of the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
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
        public static TValue GetValueOrDefault<TValue>(this IReadOnlyDictionary<string, string> instance, string key)
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
                else if (typeof(TValue) == typeof(uint))
                {
                    uint intResult;
                    if (uint.TryParse(value, out intResult))
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
        ///             <see cref="uint"/>
        ///         </item>
        ///         <item>
        ///             <see cref="bool"/>
        ///         </item>
        ///         <item>
        ///             <see cref="string"/>
        ///         </item>
        ///     </list>
        /// </typeparam>
        /// <param name="instance">
        ///     In instance of the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </param>
        /// <param name="key">
        ///     The key of the value to get.
        /// </param>
        /// <returns>
        ///     The value of type <typeparamref name="TValue"/> if <paramref name="key"/> exists.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///     The <typeparamref name="TValue"/> is not supported.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///     The <paramref name="key"/> not found in the <see cref="instance"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///     The value cannot be converted to <typeparamref name="TValue"/>.
        /// </exception>
        public static TValue GetValue<TValue>(this IReadOnlyDictionary<string, string> instance, string key)
        {
            instance.EnsureNotNull("instance");
            TValue result;

            var value = instance[key];

            if (typeof(TValue) == typeof(int))
            {
                result = (TValue)(object)int.Parse(value);
            }
            else if (typeof(TValue) == typeof(uint))
            {
                result = (TValue)(object)int.Parse(value);
            }
            else if (typeof(TValue) == typeof(bool))
            {
                result = (TValue)(object)value.ToBool();
            }
            else if (typeof(TValue) == typeof(string))
            {
                result = (TValue)(object)value;
            }
            else
            {
                throw new NotSupportedException("The value type '{0}' is not supported".F(typeof(TValue).Name));
            }

            return result;
        }
    }
}
