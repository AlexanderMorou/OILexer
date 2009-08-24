using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using Oilexer.Utilities.Collections;

namespace Oilexer
{
    /// <summary>
    /// Provides a base <see cref="ICultureIdentifier"/> implementation which supports 
    /// the standard set.
    /// </summary>
    public sealed class CultureIdentifier :
        ICultureIdentifier
    {
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="Culture"/>.
        /// </summary>
        private int culture;
        /// <summary>
        /// Data member for <see cref="CountryRegion"/>.
        /// </summary>
        private string countryRegion;
        static CultureIdentifier()
        {
            StaticCheck();
        }
        internal static IReadOnlyDictionary<int, CultureIdentifier> defaultCultureIDByCultureNumber;
        internal static IReadOnlyDictionary<string, CultureIdentifier> defaultCultureIDByCultureName;

        private static void StaticCheck()
        {
            if (defaultCultureIDByCultureNumber == null)
                AcquireCultures();
        }

        private void PropagateMembers(string name)
        {
            CultureIdentifier baseCulture = defaultCultureIDByCultureName[name];
            this.name = baseCulture.name;
            this.culture = baseCulture.culture;
            this.countryRegion = baseCulture.countryRegion;
        }

        private void PropagateMembers(int culture)
        {
            CultureIdentifier baseCulture = defaultCultureIDByCultureNumber[culture];
            this.name = baseCulture.name;
            this.culture = baseCulture.culture;
            this.countryRegion = baseCulture.countryRegion;
        }

        internal CultureIdentifier(string name, int culture, string countryRegion)
        {
            this.name = name;
            this.culture = culture;
            this.countryRegion = countryRegion;
        }
        private static void AcquireCultures()
        {

            CultureIdentifier[] cultureIdentifiers = new CultureIdentifier[] 
	        {
		        new CultureIdentifier(CultureIdentifiers.Names.None, CultureIdentifiers.IDs.None, CultureIdentifiers.CountryRegions.None), 
                new CultureIdentifier(CultureIdentifiers.Names.Afrikaans, CultureIdentifiers.IDs.Afrikaans, CultureIdentifiers.CountryRegions.Afrikaans), 
                new CultureIdentifier(CultureIdentifiers.Names.Afrikaans_SouthAfrica, CultureIdentifiers.IDs.Afrikaans_SouthAfrica, CultureIdentifiers.CountryRegions.Afrikaans_SouthAfrica), 
                new CultureIdentifier(CultureIdentifiers.Names.Albanian, CultureIdentifiers.IDs.Albanian, CultureIdentifiers.CountryRegions.Albanian), 
                new CultureIdentifier(CultureIdentifiers.Names.Albanian_Albania, CultureIdentifiers.IDs.Albanian_Albania, CultureIdentifiers.CountryRegions.Albanian_Albania), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic, CultureIdentifiers.IDs.Arabic, CultureIdentifiers.CountryRegions.Arabic), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Algeria, CultureIdentifiers.IDs.Arabic_Algeria, CultureIdentifiers.CountryRegions.Arabic_Algeria), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Bahrain, CultureIdentifiers.IDs.Arabic_Bahrain, CultureIdentifiers.CountryRegions.Arabic_Bahrain), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Egypt, CultureIdentifiers.IDs.Arabic_Egypt, CultureIdentifiers.CountryRegions.Arabic_Egypt), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Iraq, CultureIdentifiers.IDs.Arabic_Iraq, CultureIdentifiers.CountryRegions.Arabic_Iraq), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Jordan, CultureIdentifiers.IDs.Arabic_Jordan, CultureIdentifiers.CountryRegions.Arabic_Jordan), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Kuwait, CultureIdentifiers.IDs.Arabic_Kuwait, CultureIdentifiers.CountryRegions.Arabic_Kuwait), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Lebanon, CultureIdentifiers.IDs.Arabic_Lebanon, CultureIdentifiers.CountryRegions.Arabic_Lebanon), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Libya, CultureIdentifiers.IDs.Arabic_Libya, CultureIdentifiers.CountryRegions.Arabic_Libya), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Morocco, CultureIdentifiers.IDs.Arabic_Morocco, CultureIdentifiers.CountryRegions.Arabic_Morocco), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Oman, CultureIdentifiers.IDs.Arabic_Oman, CultureIdentifiers.CountryRegions.Arabic_Oman), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Qatar, CultureIdentifiers.IDs.Arabic_Qatar, CultureIdentifiers.CountryRegions.Arabic_Qatar), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_SaudiArabia, CultureIdentifiers.IDs.Arabic_SaudiArabia, CultureIdentifiers.CountryRegions.Arabic_SaudiArabia), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Syria, CultureIdentifiers.IDs.Arabic_Syria, CultureIdentifiers.CountryRegions.Arabic_Syria), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Tunisia, CultureIdentifiers.IDs.Arabic_Tunisia, CultureIdentifiers.CountryRegions.Arabic_Tunisia), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_UnitedArabEmirates, CultureIdentifiers.IDs.Arabic_UnitedArabEmirates, CultureIdentifiers.CountryRegions.Arabic_UnitedArabEmirates), 
                new CultureIdentifier(CultureIdentifiers.Names.Arabic_Yemen, CultureIdentifiers.IDs.Arabic_Yemen, CultureIdentifiers.CountryRegions.Arabic_Yemen), 
                new CultureIdentifier(CultureIdentifiers.Names.Armenian, CultureIdentifiers.IDs.Armenian, CultureIdentifiers.CountryRegions.Armenian), 
                new CultureIdentifier(CultureIdentifiers.Names.Armenian_Armenia, CultureIdentifiers.IDs.Armenian_Armenia, CultureIdentifiers.CountryRegions.Armenian_Armenia), 
                new CultureIdentifier(CultureIdentifiers.Names.Azeri, CultureIdentifiers.IDs.Azeri, CultureIdentifiers.CountryRegions.Azeri), 
                new CultureIdentifier(CultureIdentifiers.Names.Azeri_Cyrillic_Azerbaijan, CultureIdentifiers.IDs.Azeri_Cyrillic_Azerbaijan, CultureIdentifiers.CountryRegions.Azeri_Cyrillic_Azerbaijan), 
                new CultureIdentifier(CultureIdentifiers.Names.Azeri_Latin_Azerbaijan, CultureIdentifiers.IDs.Azeri_Latin_Azerbaijan, CultureIdentifiers.CountryRegions.Azeri_Latin_Azerbaijan), 
                new CultureIdentifier(CultureIdentifiers.Names.Basque, CultureIdentifiers.IDs.Basque, CultureIdentifiers.CountryRegions.Basque), 
                new CultureIdentifier(CultureIdentifiers.Names.Basque_Basque, CultureIdentifiers.IDs.Basque_Basque, CultureIdentifiers.CountryRegions.Basque_Basque), 
                new CultureIdentifier(CultureIdentifiers.Names.Belarusian, CultureIdentifiers.IDs.Belarusian, CultureIdentifiers.CountryRegions.Belarusian), 
                new CultureIdentifier(CultureIdentifiers.Names.Belarusian_Belarus, CultureIdentifiers.IDs.Belarusian_Belarus, CultureIdentifiers.CountryRegions.Belarusian_Belarus), 
                new CultureIdentifier(CultureIdentifiers.Names.Bulgarian, CultureIdentifiers.IDs.Bulgarian, CultureIdentifiers.CountryRegions.Bulgarian), 
                new CultureIdentifier(CultureIdentifiers.Names.Bulgarian_Bulgaria, CultureIdentifiers.IDs.Bulgarian_Bulgaria, CultureIdentifiers.CountryRegions.Bulgarian_Bulgaria), 
                new CultureIdentifier(CultureIdentifiers.Names.Catalan, CultureIdentifiers.IDs.Catalan, CultureIdentifiers.CountryRegions.Catalan), 
                new CultureIdentifier(CultureIdentifiers.Names.Catalan_Catalan, CultureIdentifiers.IDs.Catalan_Catalan, CultureIdentifiers.CountryRegions.Catalan_Catalan), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_HongKongSAR, CultureIdentifiers.IDs.Chinese_HongKongSAR, CultureIdentifiers.CountryRegions.Chinese_HongKongSAR), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_MacaoSAR, CultureIdentifiers.IDs.Chinese_MacaoSAR, CultureIdentifiers.CountryRegions.Chinese_MacaoSAR), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_China, CultureIdentifiers.IDs.Chinese_China, CultureIdentifiers.CountryRegions.Chinese_China), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_Simplified, CultureIdentifiers.IDs.Chinese_Simplified, CultureIdentifiers.CountryRegions.Chinese_Simplified), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_Singapore, CultureIdentifiers.IDs.Chinese_Singapore, CultureIdentifiers.CountryRegions.Chinese_Singapore), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_Taiwan, CultureIdentifiers.IDs.Chinese_Taiwan, CultureIdentifiers.CountryRegions.Chinese_Taiwan), 
                new CultureIdentifier(CultureIdentifiers.Names.Chinese_Traditional, CultureIdentifiers.IDs.Chinese_Traditional, CultureIdentifiers.CountryRegions.Chinese_Traditional), 
                new CultureIdentifier(CultureIdentifiers.Names.Croatian, CultureIdentifiers.IDs.Croatian, CultureIdentifiers.CountryRegions.Croatian), 
                new CultureIdentifier(CultureIdentifiers.Names.Croatian_Croatia, CultureIdentifiers.IDs.Croatian_Croatia, CultureIdentifiers.CountryRegions.Croatian_Croatia), 
                new CultureIdentifier(CultureIdentifiers.Names.Czech, CultureIdentifiers.IDs.Czech, CultureIdentifiers.CountryRegions.Czech), 
                new CultureIdentifier(CultureIdentifiers.Names.Czech_CzechRepublic, CultureIdentifiers.IDs.Czech_CzechRepublic, CultureIdentifiers.CountryRegions.Czech_CzechRepublic), 
                new CultureIdentifier(CultureIdentifiers.Names.Danish, CultureIdentifiers.IDs.Danish, CultureIdentifiers.CountryRegions.Danish), 
                new CultureIdentifier(CultureIdentifiers.Names.Danish_Denmark, CultureIdentifiers.IDs.Danish_Denmark, CultureIdentifiers.CountryRegions.Danish_Denmark), 
                new CultureIdentifier(CultureIdentifiers.Names.Dhivehi, CultureIdentifiers.IDs.Dhivehi, CultureIdentifiers.CountryRegions.Dhivehi), 
                new CultureIdentifier(CultureIdentifiers.Names.Dhivehi_Maldives, CultureIdentifiers.IDs.Dhivehi_Maldives, CultureIdentifiers.CountryRegions.Dhivehi_Maldives), 
                new CultureIdentifier(CultureIdentifiers.Names.Dutch, CultureIdentifiers.IDs.Dutch, CultureIdentifiers.CountryRegions.Dutch), 
                new CultureIdentifier(CultureIdentifiers.Names.Dutch_Belgium, CultureIdentifiers.IDs.Dutch_Belgium, CultureIdentifiers.CountryRegions.Dutch_Belgium), 
                new CultureIdentifier(CultureIdentifiers.Names.Dutch_TheNetherlands, CultureIdentifiers.IDs.Dutch_TheNetherlands, CultureIdentifiers.CountryRegions.Dutch_TheNetherlands), 
                new CultureIdentifier(CultureIdentifiers.Names.English, CultureIdentifiers.IDs.English, CultureIdentifiers.CountryRegions.English), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Australia, CultureIdentifiers.IDs.English_Australia, CultureIdentifiers.CountryRegions.English_Australia), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Belize, CultureIdentifiers.IDs.English_Belize, CultureIdentifiers.CountryRegions.English_Belize), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Canada, CultureIdentifiers.IDs.English_Canada, CultureIdentifiers.CountryRegions.English_Canada), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Caribbean, CultureIdentifiers.IDs.English_Caribbean, CultureIdentifiers.CountryRegions.English_Caribbean), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Ireland, CultureIdentifiers.IDs.English_Ireland, CultureIdentifiers.CountryRegions.English_Ireland), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Jamaica, CultureIdentifiers.IDs.English_Jamaica, CultureIdentifiers.CountryRegions.English_Jamaica), 
                new CultureIdentifier(CultureIdentifiers.Names.English_NewZealand, CultureIdentifiers.IDs.English_NewZealand, CultureIdentifiers.CountryRegions.English_NewZealand), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Philippines, CultureIdentifiers.IDs.English_Philippines, CultureIdentifiers.CountryRegions.English_Philippines), 
                new CultureIdentifier(CultureIdentifiers.Names.English_SouthAfrica, CultureIdentifiers.IDs.English_SouthAfrica, CultureIdentifiers.CountryRegions.English_SouthAfrica), 
                new CultureIdentifier(CultureIdentifiers.Names.English_TrinidadAndTobago, CultureIdentifiers.IDs.English_TrinidadAndTobago, CultureIdentifiers.CountryRegions.English_TrinidadAndTobago), 
                new CultureIdentifier(CultureIdentifiers.Names.English_UnitedKingdom, CultureIdentifiers.IDs.English_UnitedKingdom, CultureIdentifiers.CountryRegions.English_UnitedKingdom), 
                new CultureIdentifier(CultureIdentifiers.Names.English_UnitedStates, CultureIdentifiers.IDs.English_UnitedStates, CultureIdentifiers.CountryRegions.English_UnitedStates), 
                new CultureIdentifier(CultureIdentifiers.Names.English_Zimbabwe, CultureIdentifiers.IDs.English_Zimbabwe, CultureIdentifiers.CountryRegions.English_Zimbabwe), 
                new CultureIdentifier(CultureIdentifiers.Names.Estonian, CultureIdentifiers.IDs.Estonian, CultureIdentifiers.CountryRegions.Estonian), 
                new CultureIdentifier(CultureIdentifiers.Names.Estonian_Estonia, CultureIdentifiers.IDs.Estonian_Estonia, CultureIdentifiers.CountryRegions.Estonian_Estonia), 
                new CultureIdentifier(CultureIdentifiers.Names.Faroese, CultureIdentifiers.IDs.Faroese, CultureIdentifiers.CountryRegions.Faroese), 
                new CultureIdentifier(CultureIdentifiers.Names.Faroese_FaroeIslands, CultureIdentifiers.IDs.Faroese_FaroeIslands, CultureIdentifiers.CountryRegions.Faroese_FaroeIslands), 
                new CultureIdentifier(CultureIdentifiers.Names.Farsi, CultureIdentifiers.IDs.Farsi, CultureIdentifiers.CountryRegions.Farsi), 
                new CultureIdentifier(CultureIdentifiers.Names.Farsi_Iran, CultureIdentifiers.IDs.Farsi_Iran, CultureIdentifiers.CountryRegions.Farsi_Iran), 
                new CultureIdentifier(CultureIdentifiers.Names.Finnish, CultureIdentifiers.IDs.Finnish, CultureIdentifiers.CountryRegions.Finnish), 
                new CultureIdentifier(CultureIdentifiers.Names.Finnish_Finland, CultureIdentifiers.IDs.Finnish_Finland, CultureIdentifiers.CountryRegions.Finnish_Finland), 
                new CultureIdentifier(CultureIdentifiers.Names.French, CultureIdentifiers.IDs.French, CultureIdentifiers.CountryRegions.French), 
                new CultureIdentifier(CultureIdentifiers.Names.French_Belgium, CultureIdentifiers.IDs.French_Belgium, CultureIdentifiers.CountryRegions.French_Belgium), 
                new CultureIdentifier(CultureIdentifiers.Names.French_Canada, CultureIdentifiers.IDs.French_Canada, CultureIdentifiers.CountryRegions.French_Canada), 
                new CultureIdentifier(CultureIdentifiers.Names.French_France, CultureIdentifiers.IDs.French_France, CultureIdentifiers.CountryRegions.French_France), 
                new CultureIdentifier(CultureIdentifiers.Names.French_Luxembourg, CultureIdentifiers.IDs.French_Luxembourg, CultureIdentifiers.CountryRegions.French_Luxembourg), 
                new CultureIdentifier(CultureIdentifiers.Names.French_Monaco, CultureIdentifiers.IDs.French_Monaco, CultureIdentifiers.CountryRegions.French_Monaco), 
                new CultureIdentifier(CultureIdentifiers.Names.French_Switzerland, CultureIdentifiers.IDs.French_Switzerland, CultureIdentifiers.CountryRegions.French_Switzerland), 
                new CultureIdentifier(CultureIdentifiers.Names.Galician, CultureIdentifiers.IDs.Galician, CultureIdentifiers.CountryRegions.Galician), 
                new CultureIdentifier(CultureIdentifiers.Names.Galician_Galician, CultureIdentifiers.IDs.Galician_Galician, CultureIdentifiers.CountryRegions.Galician_Galician), 
                new CultureIdentifier(CultureIdentifiers.Names.Georgian, CultureIdentifiers.IDs.Georgian, CultureIdentifiers.CountryRegions.Georgian), 
                new CultureIdentifier(CultureIdentifiers.Names.Georgian_Georgia, CultureIdentifiers.IDs.Georgian_Georgia, CultureIdentifiers.CountryRegions.Georgian_Georgia), 
                new CultureIdentifier(CultureIdentifiers.Names.German, CultureIdentifiers.IDs.German, CultureIdentifiers.CountryRegions.German), 
                new CultureIdentifier(CultureIdentifiers.Names.German_Austria, CultureIdentifiers.IDs.German_Austria, CultureIdentifiers.CountryRegions.German_Austria), 
                new CultureIdentifier(CultureIdentifiers.Names.German_Germany, CultureIdentifiers.IDs.German_Germany, CultureIdentifiers.CountryRegions.German_Germany), 
                new CultureIdentifier(CultureIdentifiers.Names.German_Liechtenstein, CultureIdentifiers.IDs.German_Liechtenstein, CultureIdentifiers.CountryRegions.German_Liechtenstein), 
                new CultureIdentifier(CultureIdentifiers.Names.German_Luxembourg, CultureIdentifiers.IDs.German_Luxembourg, CultureIdentifiers.CountryRegions.German_Luxembourg), 
                new CultureIdentifier(CultureIdentifiers.Names.German_Switzerland, CultureIdentifiers.IDs.German_Switzerland, CultureIdentifiers.CountryRegions.German_Switzerland), 
                new CultureIdentifier(CultureIdentifiers.Names.Greek, CultureIdentifiers.IDs.Greek, CultureIdentifiers.CountryRegions.Greek), 
                new CultureIdentifier(CultureIdentifiers.Names.Greek_Greece, CultureIdentifiers.IDs.Greek_Greece, CultureIdentifiers.CountryRegions.Greek_Greece), 
                new CultureIdentifier(CultureIdentifiers.Names.Gujarati, CultureIdentifiers.IDs.Gujarati, CultureIdentifiers.CountryRegions.Gujarati), 
                new CultureIdentifier(CultureIdentifiers.Names.Gujarati_India, CultureIdentifiers.IDs.Gujarati_India, CultureIdentifiers.CountryRegions.Gujarati_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Hebrew, CultureIdentifiers.IDs.Hebrew, CultureIdentifiers.CountryRegions.Hebrew), 
                new CultureIdentifier(CultureIdentifiers.Names.Hebrew_Israel, CultureIdentifiers.IDs.Hebrew_Israel, CultureIdentifiers.CountryRegions.Hebrew_Israel), 
                new CultureIdentifier(CultureIdentifiers.Names.Hindi, CultureIdentifiers.IDs.Hindi, CultureIdentifiers.CountryRegions.Hindi), 
                new CultureIdentifier(CultureIdentifiers.Names.Hindi_India, CultureIdentifiers.IDs.Hindi_India, CultureIdentifiers.CountryRegions.Hindi_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Hungarian, CultureIdentifiers.IDs.Hungarian, CultureIdentifiers.CountryRegions.Hungarian), 
                new CultureIdentifier(CultureIdentifiers.Names.Hungarian_Hungary, CultureIdentifiers.IDs.Hungarian_Hungary, CultureIdentifiers.CountryRegions.Hungarian_Hungary), 
                new CultureIdentifier(CultureIdentifiers.Names.Icelandic, CultureIdentifiers.IDs.Icelandic, CultureIdentifiers.CountryRegions.Icelandic), 
                new CultureIdentifier(CultureIdentifiers.Names.Icelandic_Iceland, CultureIdentifiers.IDs.Icelandic_Iceland, CultureIdentifiers.CountryRegions.Icelandic_Iceland), 
                new CultureIdentifier(CultureIdentifiers.Names.Indonesian, CultureIdentifiers.IDs.Indonesian, CultureIdentifiers.CountryRegions.Indonesian), 
                new CultureIdentifier(CultureIdentifiers.Names.Indonesian_Indonesia, CultureIdentifiers.IDs.Indonesian_Indonesia, CultureIdentifiers.CountryRegions.Indonesian_Indonesia), 
                new CultureIdentifier(CultureIdentifiers.Names.Italian, CultureIdentifiers.IDs.Italian, CultureIdentifiers.CountryRegions.Italian), 
                new CultureIdentifier(CultureIdentifiers.Names.Italian_Italy, CultureIdentifiers.IDs.Italian_Italy, CultureIdentifiers.CountryRegions.Italian_Italy), 
                new CultureIdentifier(CultureIdentifiers.Names.Italian_Switzerland, CultureIdentifiers.IDs.Italian_Switzerland, CultureIdentifiers.CountryRegions.Italian_Switzerland), 
                new CultureIdentifier(CultureIdentifiers.Names.Japanese, CultureIdentifiers.IDs.Japanese, CultureIdentifiers.CountryRegions.Japanese), 
                new CultureIdentifier(CultureIdentifiers.Names.Japanese_Japan, CultureIdentifiers.IDs.Japanese_Japan, CultureIdentifiers.CountryRegions.Japanese_Japan), 
                new CultureIdentifier(CultureIdentifiers.Names.Kannada, CultureIdentifiers.IDs.Kannada, CultureIdentifiers.CountryRegions.Kannada), 
                new CultureIdentifier(CultureIdentifiers.Names.Kannada_India, CultureIdentifiers.IDs.Kannada_India, CultureIdentifiers.CountryRegions.Kannada_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Kazakh, CultureIdentifiers.IDs.Kazakh, CultureIdentifiers.CountryRegions.Kazakh), 
                new CultureIdentifier(CultureIdentifiers.Names.Kazakh_Kazakhstan, CultureIdentifiers.IDs.Kazakh_Kazakhstan, CultureIdentifiers.CountryRegions.Kazakh_Kazakhstan), 
                new CultureIdentifier(CultureIdentifiers.Names.Konkani, CultureIdentifiers.IDs.Konkani, CultureIdentifiers.CountryRegions.Konkani), 
                new CultureIdentifier(CultureIdentifiers.Names.Konkani_India, CultureIdentifiers.IDs.Konkani_India, CultureIdentifiers.CountryRegions.Konkani_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Korean, CultureIdentifiers.IDs.Korean, CultureIdentifiers.CountryRegions.Korean), 
                new CultureIdentifier(CultureIdentifiers.Names.Korean_Korea, CultureIdentifiers.IDs.Korean_Korea, CultureIdentifiers.CountryRegions.Korean_Korea), 
                new CultureIdentifier(CultureIdentifiers.Names.Kyrgyz, CultureIdentifiers.IDs.Kyrgyz, CultureIdentifiers.CountryRegions.Kyrgyz), 
                new CultureIdentifier(CultureIdentifiers.Names.Kyrgyz_Kyrgyzstan, CultureIdentifiers.IDs.Kyrgyz_Kyrgyzstan, CultureIdentifiers.CountryRegions.Kyrgyz_Kyrgyzstan), 
                new CultureIdentifier(CultureIdentifiers.Names.Latvian, CultureIdentifiers.IDs.Latvian, CultureIdentifiers.CountryRegions.Latvian), 
                new CultureIdentifier(CultureIdentifiers.Names.Latvian_Latvia, CultureIdentifiers.IDs.Latvian_Latvia, CultureIdentifiers.CountryRegions.Latvian_Latvia), 
                new CultureIdentifier(CultureIdentifiers.Names.Lithuanian, CultureIdentifiers.IDs.Lithuanian, CultureIdentifiers.CountryRegions.Lithuanian), 
                new CultureIdentifier(CultureIdentifiers.Names.Lithuanian_Lithuania, CultureIdentifiers.IDs.Lithuanian_Lithuania, CultureIdentifiers.CountryRegions.Lithuanian_Lithuania), 
                new CultureIdentifier(CultureIdentifiers.Names.Macedonian, CultureIdentifiers.IDs.Macedonian, CultureIdentifiers.CountryRegions.Macedonian), 
                new CultureIdentifier(CultureIdentifiers.Names.Macedonian_FormerYugoslavRepublicOfMacedonia, CultureIdentifiers.IDs.Macedonian_FormerYugoslavRepublicOfMacedonia, CultureIdentifiers.CountryRegions.Macedonian_FormerYugoslavRepublicOfMacedonia), 
                new CultureIdentifier(CultureIdentifiers.Names.Malay, CultureIdentifiers.IDs.Malay, CultureIdentifiers.CountryRegions.Malay), 
                new CultureIdentifier(CultureIdentifiers.Names.Malay_Brunei, CultureIdentifiers.IDs.Malay_Brunei, CultureIdentifiers.CountryRegions.Malay_Brunei), 
                new CultureIdentifier(CultureIdentifiers.Names.Malay_Malaysia, CultureIdentifiers.IDs.Malay_Malaysia, CultureIdentifiers.CountryRegions.Malay_Malaysia), 
                new CultureIdentifier(CultureIdentifiers.Names.Marathi, CultureIdentifiers.IDs.Marathi, CultureIdentifiers.CountryRegions.Marathi), 
                new CultureIdentifier(CultureIdentifiers.Names.Marathi_India, CultureIdentifiers.IDs.Marathi_India, CultureIdentifiers.CountryRegions.Marathi_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Mongolian, CultureIdentifiers.IDs.Mongolian, CultureIdentifiers.CountryRegions.Mongolian), 
                new CultureIdentifier(CultureIdentifiers.Names.Mongolian_Mongolia, CultureIdentifiers.IDs.Mongolian_Mongolia, CultureIdentifiers.CountryRegions.Mongolian_Mongolia), 
                new CultureIdentifier(CultureIdentifiers.Names.Norwegian, CultureIdentifiers.IDs.Norwegian, CultureIdentifiers.CountryRegions.Norwegian), 
                new CultureIdentifier(CultureIdentifiers.Names.Norwegian_Bokmål_Norway, CultureIdentifiers.IDs.Norwegian_Bokmål_Norway, CultureIdentifiers.CountryRegions.Norwegian_Bokmål_Norway), 
                new CultureIdentifier(CultureIdentifiers.Names.Norwegian_Nynorsk_Norway, CultureIdentifiers.IDs.Norwegian_Nynorsk_Norway, CultureIdentifiers.CountryRegions.Norwegian_Nynorsk_Norway), 
                new CultureIdentifier(CultureIdentifiers.Names.Polish, CultureIdentifiers.IDs.Polish, CultureIdentifiers.CountryRegions.Polish), 
                new CultureIdentifier(CultureIdentifiers.Names.Polish_Poland, CultureIdentifiers.IDs.Polish_Poland, CultureIdentifiers.CountryRegions.Polish_Poland), 
                new CultureIdentifier(CultureIdentifiers.Names.Portuguese, CultureIdentifiers.IDs.Portuguese, CultureIdentifiers.CountryRegions.Portuguese), 
                new CultureIdentifier(CultureIdentifiers.Names.Portuguese_Brazil, CultureIdentifiers.IDs.Portuguese_Brazil, CultureIdentifiers.CountryRegions.Portuguese_Brazil), 
                new CultureIdentifier(CultureIdentifiers.Names.Portuguese_Portugal, CultureIdentifiers.IDs.Portuguese_Portugal, CultureIdentifiers.CountryRegions.Portuguese_Portugal), 
                new CultureIdentifier(CultureIdentifiers.Names.Punjabi, CultureIdentifiers.IDs.Punjabi, CultureIdentifiers.CountryRegions.Punjabi), 
                new CultureIdentifier(CultureIdentifiers.Names.Punjabi_India, CultureIdentifiers.IDs.Punjabi_India, CultureIdentifiers.CountryRegions.Punjabi_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Romanian, CultureIdentifiers.IDs.Romanian, CultureIdentifiers.CountryRegions.Romanian), 
                new CultureIdentifier(CultureIdentifiers.Names.Romanian_Romania, CultureIdentifiers.IDs.Romanian_Romania, CultureIdentifiers.CountryRegions.Romanian_Romania), 
                new CultureIdentifier(CultureIdentifiers.Names.Russian, CultureIdentifiers.IDs.Russian, CultureIdentifiers.CountryRegions.Russian), 
                new CultureIdentifier(CultureIdentifiers.Names.Russian_Russia, CultureIdentifiers.IDs.Russian_Russia, CultureIdentifiers.CountryRegions.Russian_Russia), 
                new CultureIdentifier(CultureIdentifiers.Names.Sanskrit, CultureIdentifiers.IDs.Sanskrit, CultureIdentifiers.CountryRegions.Sanskrit), 
                new CultureIdentifier(CultureIdentifiers.Names.Sanskrit_India, CultureIdentifiers.IDs.Sanskrit_India, CultureIdentifiers.CountryRegions.Sanskrit_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Serbian_Cyrillic_Serbia, CultureIdentifiers.IDs.Serbian_Cyrillic_Serbia, CultureIdentifiers.CountryRegions.Serbian_Cyrillic_Serbia), 
                new CultureIdentifier(CultureIdentifiers.Names.Serbian_Latin_Serbia, CultureIdentifiers.IDs.Serbian_Latin_Serbia, CultureIdentifiers.CountryRegions.Serbian_Latin_Serbia), 
                new CultureIdentifier(CultureIdentifiers.Names.Slovak, CultureIdentifiers.IDs.Slovak, CultureIdentifiers.CountryRegions.Slovak), 
                new CultureIdentifier(CultureIdentifiers.Names.Slovak_Slovakia, CultureIdentifiers.IDs.Slovak_Slovakia, CultureIdentifiers.CountryRegions.Slovak_Slovakia), 
                new CultureIdentifier(CultureIdentifiers.Names.Slovenian, CultureIdentifiers.IDs.Slovenian, CultureIdentifiers.CountryRegions.Slovenian), 
                new CultureIdentifier(CultureIdentifiers.Names.Slovenian_Slovenia, CultureIdentifiers.IDs.Slovenian_Slovenia, CultureIdentifiers.CountryRegions.Slovenian_Slovenia), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish, CultureIdentifiers.IDs.Spanish, CultureIdentifiers.CountryRegions.Spanish), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Argentina, CultureIdentifiers.IDs.Spanish_Argentina, CultureIdentifiers.CountryRegions.Spanish_Argentina), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Bolivia, CultureIdentifiers.IDs.Spanish_Bolivia, CultureIdentifiers.CountryRegions.Spanish_Bolivia), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Chile, CultureIdentifiers.IDs.Spanish_Chile, CultureIdentifiers.CountryRegions.Spanish_Chile), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Colombia, CultureIdentifiers.IDs.Spanish_Colombia, CultureIdentifiers.CountryRegions.Spanish_Colombia), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_CostaRica, CultureIdentifiers.IDs.Spanish_CostaRica, CultureIdentifiers.CountryRegions.Spanish_CostaRica), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_DominicanRepublic, CultureIdentifiers.IDs.Spanish_DominicanRepublic, CultureIdentifiers.CountryRegions.Spanish_DominicanRepublic), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Ecuador, CultureIdentifiers.IDs.Spanish_Ecuador, CultureIdentifiers.CountryRegions.Spanish_Ecuador), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_ElSalvador, CultureIdentifiers.IDs.Spanish_ElSalvador, CultureIdentifiers.CountryRegions.Spanish_ElSalvador), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Guatemala, CultureIdentifiers.IDs.Spanish_Guatemala, CultureIdentifiers.CountryRegions.Spanish_Guatemala), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Honduras, CultureIdentifiers.IDs.Spanish_Honduras, CultureIdentifiers.CountryRegions.Spanish_Honduras), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Mexico, CultureIdentifiers.IDs.Spanish_Mexico, CultureIdentifiers.CountryRegions.Spanish_Mexico), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Nicaragua, CultureIdentifiers.IDs.Spanish_Nicaragua, CultureIdentifiers.CountryRegions.Spanish_Nicaragua), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Panama, CultureIdentifiers.IDs.Spanish_Panama, CultureIdentifiers.CountryRegions.Spanish_Panama), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Paraguay, CultureIdentifiers.IDs.Spanish_Paraguay, CultureIdentifiers.CountryRegions.Spanish_Paraguay), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Peru, CultureIdentifiers.IDs.Spanish_Peru, CultureIdentifiers.CountryRegions.Spanish_Peru), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_PuertoRico, CultureIdentifiers.IDs.Spanish_PuertoRico, CultureIdentifiers.CountryRegions.Spanish_PuertoRico), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Spain, CultureIdentifiers.IDs.Spanish_Spain, CultureIdentifiers.CountryRegions.Spanish_Spain), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Uruguay, CultureIdentifiers.IDs.Spanish_Uruguay, CultureIdentifiers.CountryRegions.Spanish_Uruguay), 
                new CultureIdentifier(CultureIdentifiers.Names.Spanish_Venezuela, CultureIdentifiers.IDs.Spanish_Venezuela, CultureIdentifiers.CountryRegions.Spanish_Venezuela), 
                new CultureIdentifier(CultureIdentifiers.Names.Swahili, CultureIdentifiers.IDs.Swahili, CultureIdentifiers.CountryRegions.Swahili), 
                new CultureIdentifier(CultureIdentifiers.Names.Swahili_Kenya, CultureIdentifiers.IDs.Swahili_Kenya, CultureIdentifiers.CountryRegions.Swahili_Kenya), 
                new CultureIdentifier(CultureIdentifiers.Names.Swedish, CultureIdentifiers.IDs.Swedish, CultureIdentifiers.CountryRegions.Swedish), 
                new CultureIdentifier(CultureIdentifiers.Names.Swedish_Finland, CultureIdentifiers.IDs.Swedish_Finland, CultureIdentifiers.CountryRegions.Swedish_Finland), 
                new CultureIdentifier(CultureIdentifiers.Names.Swedish_Sweden, CultureIdentifiers.IDs.Swedish_Sweden, CultureIdentifiers.CountryRegions.Swedish_Sweden), 
                new CultureIdentifier(CultureIdentifiers.Names.Syriac, CultureIdentifiers.IDs.Syriac, CultureIdentifiers.CountryRegions.Syriac), 
                new CultureIdentifier(CultureIdentifiers.Names.Syriac_Syria, CultureIdentifiers.IDs.Syriac_Syria, CultureIdentifiers.CountryRegions.Syriac_Syria), 
                new CultureIdentifier(CultureIdentifiers.Names.Tamil, CultureIdentifiers.IDs.Tamil, CultureIdentifiers.CountryRegions.Tamil), 
                new CultureIdentifier(CultureIdentifiers.Names.Tamil_India, CultureIdentifiers.IDs.Tamil_India, CultureIdentifiers.CountryRegions.Tamil_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Tatar, CultureIdentifiers.IDs.Tatar, CultureIdentifiers.CountryRegions.Tatar), 
                new CultureIdentifier(CultureIdentifiers.Names.Tatar_Russia, CultureIdentifiers.IDs.Tatar_Russia, CultureIdentifiers.CountryRegions.Tatar_Russia), 
                new CultureIdentifier(CultureIdentifiers.Names.Telugu, CultureIdentifiers.IDs.Telugu, CultureIdentifiers.CountryRegions.Telugu), 
                new CultureIdentifier(CultureIdentifiers.Names.Telugu_India, CultureIdentifiers.IDs.Telugu_India, CultureIdentifiers.CountryRegions.Telugu_India), 
                new CultureIdentifier(CultureIdentifiers.Names.Thai, CultureIdentifiers.IDs.Thai, CultureIdentifiers.CountryRegions.Thai), 
                new CultureIdentifier(CultureIdentifiers.Names.Thai_Thailand, CultureIdentifiers.IDs.Thai_Thailand, CultureIdentifiers.CountryRegions.Thai_Thailand), 
                new CultureIdentifier(CultureIdentifiers.Names.Turkish, CultureIdentifiers.IDs.Turkish, CultureIdentifiers.CountryRegions.Turkish), 
                new CultureIdentifier(CultureIdentifiers.Names.Turkish_Turkey, CultureIdentifiers.IDs.Turkish_Turkey, CultureIdentifiers.CountryRegions.Turkish_Turkey), 
                new CultureIdentifier(CultureIdentifiers.Names.Ukrainian, CultureIdentifiers.IDs.Ukrainian, CultureIdentifiers.CountryRegions.Ukrainian), 
                new CultureIdentifier(CultureIdentifiers.Names.Ukrainian_Ukraine, CultureIdentifiers.IDs.Ukrainian_Ukraine, CultureIdentifiers.CountryRegions.Ukrainian_Ukraine), 
                new CultureIdentifier(CultureIdentifiers.Names.Urdu, CultureIdentifiers.IDs.Urdu, CultureIdentifiers.CountryRegions.Urdu), 
                new CultureIdentifier(CultureIdentifiers.Names.Urdu_Pakistan, CultureIdentifiers.IDs.Urdu_Pakistan, CultureIdentifiers.CountryRegions.Urdu_Pakistan), 
                new CultureIdentifier(CultureIdentifiers.Names.Uzbek, CultureIdentifiers.IDs.Uzbek, CultureIdentifiers.CountryRegions.Uzbek), 
                new CultureIdentifier(CultureIdentifiers.Names.Uzbek_Cyrillic_Uzbekistan, CultureIdentifiers.IDs.Uzbek_Cyrillic_Uzbekistan, CultureIdentifiers.CountryRegions.Uzbek_Cyrillic_Uzbekistan), 
                new CultureIdentifier(CultureIdentifiers.Names.Uzbek_Latin_Uzbekistan, CultureIdentifiers.IDs.Uzbek_Latin_Uzbekistan, CultureIdentifiers.CountryRegions.Uzbek_Latin_Uzbekistan), 
                new CultureIdentifier(CultureIdentifiers.Names.Vietnamese, CultureIdentifiers.IDs.Vietnamese, CultureIdentifiers.CountryRegions.Vietnamese), 
                new CultureIdentifier(CultureIdentifiers.Names.Vietnamese_Vietnam, CultureIdentifiers.IDs.Vietnamese_Vietnam, CultureIdentifiers.CountryRegions.Vietnamese_Vietnam)
	        };
            Dictionary<string, CultureIdentifier> dNames = new Dictionary<string, CultureIdentifier>();
            Dictionary<int, CultureIdentifier> dNumbers = new Dictionary<int, CultureIdentifier>();
            foreach (CultureIdentifier ci in cultureIdentifiers)
            {
                dNumbers.Add(ci.culture, ci);
                dNames.Add(ci.name, ci);
            }
            defaultCultureIDByCultureName = new ReadOnlyDictionary<string, CultureIdentifier>(dNames);
            defaultCultureIDByCultureNumber = new ReadOnlyDictionary<int, CultureIdentifier>(dNumbers);
            cultureIdentifiers = null;
        }

        public static CultureIdentifier GetIdentifierByName(string name)
        {
            if (defaultCultureIDByCultureName.ContainsKey(name))
                return defaultCultureIDByCultureName[name];
            else
                throw new ArgumentOutOfRangeException("name");
        }

        public static CultureIdentifier GetIdentifierByID(int id)
        {
            if (defaultCultureIDByCultureNumber.ContainsKey(id))
                return defaultCultureIDByCultureNumber[id];
            else
                throw new ArgumentOutOfRangeException("name");
        }

        #region ICultureIdentifier Members

        /// <summary>
        /// Returns the short-hand name of the culture.
        /// </summary>
        public string Name
        {
            get { return this.name; ; }
        }

        /// <summary>
        /// Returns the numerical identifier of the culture.
        /// </summary>
        public int Culture
        {
            get { return this.culture; }
        }

        /// <summary>
        /// Returns the readable user-friendly description of the country/region.
        /// </summary>
        public string CountryRegion
        {
            get { return this.countryRegion; }
        }

        /// <summary>
        /// Obtains a <see cref="CultureInfo"/> instance relative to the <see cref="ICultureIdentifier"/>.
        /// </summary>
        /// <returns>A <see cref="CultureInfo"/> relative to the <see cref="Culture"/> of the
        /// <see cref="CultureIdentifier"/> instance.</returns>
        public CultureInfo GetCultureInfo()
        {
            return CultureInfo.GetCultureInfo(this.Culture);
        }

        #endregion
    }
}
