using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;

namespace Oilexer
{
    [TypeConverter(typeof(CultureIdentifierTypeConverter))]
    public interface ICultureIdentifier
    {
        /// <summary>
        /// Returns the short-hand name of the culture.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Returns the numerical identifier of the culture.
        /// </summary>
        int Culture { get; }
        /// <summary>
        /// Returns the readable user-friendly description of the country/region.
        /// </summary>
        string CountryRegion { get; }

        /// <summary>
        /// Obtains a <see cref="CultureInfo"/> instance relative to the <see cref="ICultureIdentifier"/>.
        /// </summary>
        /// <returns>A <see cref="CultureInfo"/> relative to the <see cref="Culture"/> of the
        /// <see cref="ICultureIdentifier"/> implementation.</returns>
        CultureInfo GetCultureInfo();
    }
}
