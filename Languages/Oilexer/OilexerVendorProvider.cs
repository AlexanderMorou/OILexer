using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OilexerVendorProvider
    {
        /// <summary>
        /// Obtains the <see cref="OilexerLanguage"/> instance
        /// which contains information about the language and
        /// a provider of services.
        /// </summary>
        /// <param name="vendor">The singleton
        /// <see cref="IAllenCopelandLanguageVendor"/>
        /// which will provide the language.</param>
        /// <returns>The singleton <see cref="OilexerLanguage"/> instance.</returns>
        public static OilexerLanguage GetOilexerLanguage(this IAllenCopelandLanguageVendor vendor)
        {
            return OilexerLanguage.LanguageInstance;
        }
    }
}
