using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Oilexer.Utilities.Arrays;
using System.Globalization;
 /*---------------------------------------------------------------------\
 | Copyright © 2009 Allen Copeland Jr.                                  |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */


namespace Oilexer
{
    /// <summary>
    /// Provides a string->culture identifier type converter.
    /// </summary>
    public class CultureIdentifierTypeConverter :
        TypeConverter
    {
        /// <summary>
        /// Creates a new <see cref="CultureIdentifierTypeConverter"/>
        /// initialized to a default state.
        /// </summary>
        public CultureIdentifierTypeConverter()
            : base()
        {
        }

        /// <summary>
        /// Returns whether <see cref="CultureIdentifierTypeConverter"/> can convert an object of 
        /// the <paramref name="sourceType"/> to the type of this converter (<see cref="ICultureIdentifier"/>), 
        /// using the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="sourceType">
        /// A <see cref="Type"/> that represents the type you want to convert from.
        /// </param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(System.String))
                return (context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)));
            return base.CanConvertFrom(context, sourceType);
        }


        /// <summary>
        /// Converts the given <paramref name="value"/> to the type of this converter (<see cref="ICultureIdentifier"/>), 
        /// using the specified <paramref name="context"/> and <paramref name="culture"/> information.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
        /// </param>
        /// <param name="culture">
        /// The <see cref="CultureInfo"/> to use as the current culture.
        /// </param>
        /// <param name="value">The <see cref="Object"/> to convert.</param>
        /// <returns>An <see cref="Object"/> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(System.String) && context.PropertyDescriptor != null && context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)))
            {
                foreach (CultureIdentifier c in CultureIdentifiers.defaultCultureIDByCultureNumber.Values)
                {
                    if (c.CountryRegion == value.ToString())
                        return c;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return (context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)));
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null)
                return (context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)));
            else
                return base.GetStandardValuesExclusive(context);
        }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is ICultureIdentifier)
            {
                return ((ICultureIdentifier)value).CountryRegion;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter
        /// is designed for when provided with a format <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides a format context
        /// that can be used to extract additional information about the environment
        /// from which <see cref="CultureIdentifierTypeConverter"/> is invoked. 
        /// This parameter or properties of this parameter can be null.
        /// </param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection"/> that holds
        /// a standard set of valid values, or null if the data type does not support
        /// a standard set of values.
        /// </returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)))
                return new StandardValuesCollection(new List<ICultureIdentifier>(CultureIdentifiers.defaultCultureIDByCultureNumber.Values.ToArray().Cast<ICultureIdentifier>()));
            return base.GetStandardValues(context);
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be
        /// picked from a list, using the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="TypeConverter.GetStandardValues()"/> should be called to find a common set 
        /// of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (context != null && context.PropertyDescriptor != null)
                return (context.PropertyDescriptor.PropertyType.IsAssignableFrom(typeof(ICultureIdentifier)));
            return base.GetStandardValuesSupported(context);
        }
    }
}
