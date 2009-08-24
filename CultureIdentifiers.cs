using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer
{
    /// <summary>
    /// Provides access to all variants of the <see cref="CultureIdentifier"/> instances.
    /// </summary>
    public static class CultureIdentifiers
    {
        static CultureIdentifiers()
        {
        }
        ///<summary>
        ///Static culture identifier instance for None.
        ///</summary>
        ///<remarks>Culture ID: 0x007F
        ///Culture Name: <see cref="System.String.Empty"/></remarks>
        public static readonly ICultureIdentifier None = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.None];
        ///<summary>
        ///Static culture identifier instance for Afrikaans.
        ///</summary>
        ///<remarks>Culture ID: 0x0036
        ///Culture Name:af</remarks>
        public static readonly ICultureIdentifier Afrikaans = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Afrikaans];
        ///<summary>
        ///Static culture identifier instance for Afrikaans - South Africa.
        ///</summary>
        ///<remarks>Culture ID: 0x0436
        ///Culture Name:af-ZA</remarks>
        public static readonly ICultureIdentifier Afrikaans_SouthAfrica = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Afrikaans_SouthAfrica];
        ///<summary>
        ///Static culture identifier instance for Albanian.
        ///</summary>
        ///<remarks>Culture ID: 0x001C
        ///Culture Name:sq</remarks>
        public static readonly ICultureIdentifier Albanian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Albanian];
        ///<summary>
        ///Static culture identifier instance for Albanian - Albania.
        ///</summary>
        ///<remarks>Culture ID: 0x041C
        ///Culture Name:sq-AL</remarks>
        public static readonly ICultureIdentifier Albanian_Albania = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Albanian_Albania];
        ///<summary>
        ///Static culture identifier instance for Arabic.
        ///</summary>
        ///<remarks>Culture ID: 0x0001
        ///Culture Name:ar</remarks>
        public static readonly ICultureIdentifier Arabic = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic];
        ///<summary>
        ///Static culture identifier instance for Arabic - Algeria.
        ///</summary>
        ///<remarks>Culture ID: 0x1401
        ///Culture Name:ar-DZ</remarks>
        public static readonly ICultureIdentifier Arabic_Algeria = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Algeria];
        ///<summary>
        ///Static culture identifier instance for Arabic - Bahrain.
        ///</summary>
        ///<remarks>Culture ID: 0x3C01
        ///Culture Name:ar-BH</remarks>
        public static readonly ICultureIdentifier Arabic_Bahrain = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Bahrain];
        ///<summary>
        ///Static culture identifier instance for Arabic - Egypt.
        ///</summary>
        ///<remarks>Culture ID: 0x0C01
        ///Culture Name:ar-EG</remarks>
        public static readonly ICultureIdentifier Arabic_Egypt = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Egypt];
        ///<summary>
        ///Static culture identifier instance for Arabic - Iraq.
        ///</summary>
        ///<remarks>Culture ID: 0x0801
        ///Culture Name:ar-IQ</remarks>
        public static readonly ICultureIdentifier Arabic_Iraq = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Iraq];
        ///<summary>
        ///Static culture identifier instance for Arabic - Jordan.
        ///</summary>
        ///<remarks>Culture ID: 0x2C01
        ///Culture Name:ar-JO</remarks>
        public static readonly ICultureIdentifier Arabic_Jordan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Jordan];
        ///<summary>
        ///Static culture identifier instance for Arabic - Kuwait.
        ///</summary>
        ///<remarks>Culture ID: 0x3401
        ///Culture Name:ar-KW</remarks>
        public static readonly ICultureIdentifier Arabic_Kuwait = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Kuwait];
        ///<summary>
        ///Static culture identifier instance for Arabic - Lebanon.
        ///</summary>
        ///<remarks>Culture ID: 0x3001
        ///Culture Name:ar-LB</remarks>
        public static readonly ICultureIdentifier Arabic_Lebanon = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Lebanon];
        ///<summary>
        ///Static culture identifier instance for Arabic - Libya.
        ///</summary>
        ///<remarks>Culture ID: 0x1001
        ///Culture Name:ar-LY</remarks>
        public static readonly ICultureIdentifier Arabic_Libya = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Libya];
        ///<summary>
        ///Static culture identifier instance for Arabic - Morocco.
        ///</summary>
        ///<remarks>Culture ID: 0x1801
        ///Culture Name:ar-MA</remarks>
        public static readonly ICultureIdentifier Arabic_Morocco = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Morocco];
        ///<summary>
        ///Static culture identifier instance for Arabic - Oman.
        ///</summary>
        ///<remarks>Culture ID: 0x2001
        ///Culture Name:ar-OM</remarks>
        public static readonly ICultureIdentifier Arabic_Oman = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Oman];
        ///<summary>
        ///Static culture identifier instance for Arabic - Qatar.
        ///</summary>
        ///<remarks>Culture ID: 0x4001
        ///Culture Name:ar-QA</remarks>
        public static readonly ICultureIdentifier Arabic_Qatar = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Qatar];
        ///<summary>
        ///Static culture identifier instance for Arabic - Saudi Arabia.
        ///</summary>
        ///<remarks>Culture ID: 0x0401
        ///Culture Name:ar-SA</remarks>
        public static readonly ICultureIdentifier Arabic_SaudiArabia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_SaudiArabia];
        ///<summary>
        ///Static culture identifier instance for Arabic - Syria.
        ///</summary>
        ///<remarks>Culture ID: 0x2801
        ///Culture Name:ar-SY</remarks>
        public static readonly ICultureIdentifier Arabic_Syria = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Syria];
        ///<summary>
        ///Static culture identifier instance for Arabic - Tunisia.
        ///</summary>
        ///<remarks>Culture ID: 0x1C01
        ///Culture Name:ar-TN</remarks>
        public static readonly ICultureIdentifier Arabic_Tunisia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Tunisia];
        ///<summary>
        ///Static culture identifier instance for Arabic - United Arab Emirates.
        ///</summary>
        ///<remarks>Culture ID: 0x3801
        ///Culture Name:ar-AE</remarks>
        public static readonly ICultureIdentifier Arabic_UnitedArabEmirates = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_UnitedArabEmirates];
        ///<summary>
        ///Static culture identifier instance for Arabic - Yemen.
        ///</summary>
        ///<remarks>Culture ID: 0x2401
        ///Culture Name:ar-YE</remarks>
        public static readonly ICultureIdentifier Arabic_Yemen = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Arabic_Yemen];
        ///<summary>
        ///Static culture identifier instance for Armenian.
        ///</summary>
        ///<remarks>Culture ID: 0x002B
        ///Culture Name:hy</remarks>
        public static readonly ICultureIdentifier Armenian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Armenian];
        ///<summary>
        ///Static culture identifier instance for Armenian - Armenia.
        ///</summary>
        ///<remarks>Culture ID: 0x042B
        ///Culture Name:hy-AM</remarks>
        public static readonly ICultureIdentifier Armenian_Armenia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Armenian_Armenia];
        ///<summary>
        ///Static culture identifier instance for Azeri.
        ///</summary>
        ///<remarks>Culture ID: 0x002C
        ///Culture Name:az</remarks>
        public static readonly ICultureIdentifier Azeri = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Azeri];
        ///<summary>
        ///Static culture identifier instance for Azeri (Cyrillic) - Azerbaijan.
        ///</summary>
        ///<remarks>Culture ID: 0x082C
        ///Culture Name:az-AZ-Cyrl</remarks>
        public static readonly ICultureIdentifier Azeri_Cyrillic_Azerbaijan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Azeri_Cyrillic_Azerbaijan];
        ///<summary>
        ///Static culture identifier instance for Azeri (Latin) - Azerbaijan.
        ///</summary>
        ///<remarks>Culture ID: 0x042C
        ///Culture Name:az-AZ-Latn</remarks>
        public static readonly ICultureIdentifier Azeri_Latin_Azerbaijan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Azeri_Latin_Azerbaijan];
        ///<summary>
        ///Static culture identifier instance for Basque.
        ///</summary>
        ///<remarks>Culture ID: 0x002D
        ///Culture Name:eu</remarks>
        public static readonly ICultureIdentifier Basque = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Basque];
        ///<summary>
        ///Static culture identifier instance for Basque - Basque.
        ///</summary>
        ///<remarks>Culture ID: 0x042D
        ///Culture Name:eu-ES</remarks>
        public static readonly ICultureIdentifier Basque_Basque = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Basque_Basque];
        ///<summary>
        ///Static culture identifier instance for Belarusian.
        ///</summary>
        ///<remarks>Culture ID: 0x0023
        ///Culture Name:be</remarks>
        public static readonly ICultureIdentifier Belarusian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Belarusian];
        ///<summary>
        ///Static culture identifier instance for Belarusian - Belarus.
        ///</summary>
        ///<remarks>Culture ID: 0x0423
        ///Culture Name:be-BY</remarks>
        public static readonly ICultureIdentifier Belarusian_Belarus = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Belarusian_Belarus];
        ///<summary>
        ///Static culture identifier instance for Bulgarian.
        ///</summary>
        ///<remarks>Culture ID: 0x0002
        ///Culture Name:bg</remarks>
        public static readonly ICultureIdentifier Bulgarian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Bulgarian];
        ///<summary>
        ///Static culture identifier instance for Bulgarian - Bulgaria.
        ///</summary>
        ///<remarks>Culture ID: 0x0402
        ///Culture Name:bg-BG</remarks>
        public static readonly ICultureIdentifier Bulgarian_Bulgaria = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Bulgarian_Bulgaria];
        ///<summary>
        ///Static culture identifier instance for Catalan.
        ///</summary>
        ///<remarks>Culture ID: 0x0003
        ///Culture Name:ca</remarks>
        public static readonly ICultureIdentifier Catalan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Catalan];
        ///<summary>
        ///Static culture identifier instance for Catalan - Catalan.
        ///</summary>
        ///<remarks>Culture ID: 0x0403
        ///Culture Name:ca-ES</remarks>
        public static readonly ICultureIdentifier Catalan_Catalan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Catalan_Catalan];
        ///<summary>
        ///Static culture identifier instance for Chinese - Hong Kong SAR.
        ///</summary>
        ///<remarks>Culture ID: 0x0C04
        ///Culture Name:zh-HK</remarks>
        public static readonly ICultureIdentifier Chinese_HongKongSAR = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_HongKongSAR];
        ///<summary>
        ///Static culture identifier instance for Chinese - Macao SAR.
        ///</summary>
        ///<remarks>Culture ID: 0x1404
        ///Culture Name:zh-MO</remarks>
        public static readonly ICultureIdentifier Chinese_MacaoSAR = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_MacaoSAR];
        ///<summary>
        ///Static culture identifier instance for Chinese - China.
        ///</summary>
        ///<remarks>Culture ID: 0x0804
        ///Culture Name:zh-CN</remarks>
        public static readonly ICultureIdentifier Chinese_China = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_China];
        ///<summary>
        ///Static culture identifier instance for Chinese (Simplified).
        ///</summary>
        ///<remarks>Culture ID: 0x0004
        ///Culture Name:zh-CHS</remarks>
        public static readonly ICultureIdentifier Chinese_Simplified = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_Simplified];
        ///<summary>
        ///Static culture identifier instance for Chinese - Singapore.
        ///</summary>
        ///<remarks>Culture ID: 0x1004
        ///Culture Name:zh-SG</remarks>
        public static readonly ICultureIdentifier Chinese_Singapore = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_Singapore];
        ///<summary>
        ///Static culture identifier instance for Chinese - Taiwan.
        ///</summary>
        ///<remarks>Culture ID: 0x0404
        ///Culture Name:zh-TW</remarks>
        public static readonly ICultureIdentifier Chinese_Taiwan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_Taiwan];
        ///<summary>
        ///Static culture identifier instance for Chinese (Traditional).
        ///</summary>
        ///<remarks>Culture ID: 0x7C04
        ///Culture Name:zh-CHT</remarks>
        public static readonly ICultureIdentifier Chinese_Traditional = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Chinese_Traditional];
        ///<summary>
        ///Static culture identifier instance for Croatian.
        ///</summary>
        ///<remarks>Culture ID: 0x001A
        ///Culture Name:hr</remarks>
        public static readonly ICultureIdentifier Croatian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Croatian];
        ///<summary>
        ///Static culture identifier instance for Croatian - Croatia.
        ///</summary>
        ///<remarks>Culture ID: 0x041A
        ///Culture Name:hr-HR</remarks>
        public static readonly ICultureIdentifier Croatian_Croatia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Croatian_Croatia];
        ///<summary>
        ///Static culture identifier instance for Czech.
        ///</summary>
        ///<remarks>Culture ID: 0x0005
        ///Culture Name:cs</remarks>
        public static readonly ICultureIdentifier Czech = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Czech];
        ///<summary>
        ///Static culture identifier instance for Czech - Czech Republic.
        ///</summary>
        ///<remarks>Culture ID: 0x0405
        ///Culture Name:cs-CZ</remarks>
        public static readonly ICultureIdentifier Czech_CzechRepublic = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Czech_CzechRepublic];
        ///<summary>
        ///Static culture identifier instance for Danish.
        ///</summary>
        ///<remarks>Culture ID: 0x0006
        ///Culture Name:da</remarks>
        public static readonly ICultureIdentifier Danish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Danish];
        ///<summary>
        ///Static culture identifier instance for Danish - Denmark.
        ///</summary>
        ///<remarks>Culture ID: 0x0406
        ///Culture Name:da-DK</remarks>
        public static readonly ICultureIdentifier Danish_Denmark = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Danish_Denmark];
        ///<summary>
        ///Static culture identifier instance for Dhivehi.
        ///</summary>
        ///<remarks>Culture ID: 0x0065
        ///Culture Name:div</remarks>
        public static readonly ICultureIdentifier Dhivehi = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Dhivehi];
        ///<summary>
        ///Static culture identifier instance for Dhivehi - Maldives.
        ///</summary>
        ///<remarks>Culture ID: 0x0465
        ///Culture Name:div-MV</remarks>
        public static readonly ICultureIdentifier Dhivehi_Maldives = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Dhivehi_Maldives];
        ///<summary>
        ///Static culture identifier instance for Dutch.
        ///</summary>
        ///<remarks>Culture ID: 0x0013
        ///Culture Name:nl</remarks>
        public static readonly ICultureIdentifier Dutch = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Dutch];
        ///<summary>
        ///Static culture identifier instance for Dutch - Belgium.
        ///</summary>
        ///<remarks>Culture ID: 0x0813
        ///Culture Name:nl-BE</remarks>
        public static readonly ICultureIdentifier Dutch_Belgium = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Dutch_Belgium];
        ///<summary>
        ///Static culture identifier instance for Dutch - The Netherlands.
        ///</summary>
        ///<remarks>Culture ID: 0x0413
        ///Culture Name:nl-NL</remarks>
        public static readonly ICultureIdentifier Dutch_TheNetherlands = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Dutch_TheNetherlands];
        ///<summary>
        ///Static culture identifier instance for English.
        ///</summary>
        ///<remarks>Culture ID: 0x0009
        ///Culture Name:en</remarks>
        public static readonly ICultureIdentifier English = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English];
        ///<summary>
        ///Static culture identifier instance for English - Australia.
        ///</summary>
        ///<remarks>Culture ID: 0x0C09
        ///Culture Name:en-AU</remarks>
        public static readonly ICultureIdentifier English_Australia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Australia];
        ///<summary>
        ///Static culture identifier instance for English - Belize.
        ///</summary>
        ///<remarks>Culture ID: 0x2809
        ///Culture Name:en-BZ</remarks>
        public static readonly ICultureIdentifier English_Belize = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Belize];
        ///<summary>
        ///Static culture identifier instance for English - Canada.
        ///</summary>
        ///<remarks>Culture ID: 0x1009
        ///Culture Name:en-CA</remarks>
        public static readonly ICultureIdentifier English_Canada = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Canada];
        ///<summary>
        ///Static culture identifier instance for English - Caribbean.
        ///</summary>
        ///<remarks>Culture ID: 0x2409
        ///Culture Name:en-CB</remarks>
        public static readonly ICultureIdentifier English_Caribbean = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Caribbean];
        ///<summary>
        ///Static culture identifier instance for English - Ireland.
        ///</summary>
        ///<remarks>Culture ID: 0x1809
        ///Culture Name:en-IE</remarks>
        public static readonly ICultureIdentifier English_Ireland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Ireland];
        ///<summary>
        ///Static culture identifier instance for English - Jamaica.
        ///</summary>
        ///<remarks>Culture ID: 0x2009
        ///Culture Name:en-JM</remarks>
        public static readonly ICultureIdentifier English_Jamaica = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Jamaica];
        ///<summary>
        ///Static culture identifier instance for English - New Zealand.
        ///</summary>
        ///<remarks>Culture ID: 0x1409
        ///Culture Name:en-NZ</remarks>
        public static readonly ICultureIdentifier English_NewZealand = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_NewZealand];
        ///<summary>
        ///Static culture identifier instance for English - Philippines.
        ///</summary>
        ///<remarks>Culture ID: 0x3409
        ///Culture Name:en-PH</remarks>
        public static readonly ICultureIdentifier English_Philippines = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Philippines];
        ///<summary>
        ///Static culture identifier instance for English - South Africa.
        ///</summary>
        ///<remarks>Culture ID: 0x1C09
        ///Culture Name:en-ZA</remarks>
        public static readonly ICultureIdentifier English_SouthAfrica = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_SouthAfrica];
        ///<summary>
        ///Static culture identifier instance for English - Trinidad and Tobago.
        ///</summary>
        ///<remarks>Culture ID: 0x2C09
        ///Culture Name:en-TT</remarks>
        public static readonly ICultureIdentifier English_TrinidadAndTobago = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_TrinidadAndTobago];
        ///<summary>
        ///Static culture identifier instance for English - United Kingdom.
        ///</summary>
        ///<remarks>Culture ID: 0x0809
        ///Culture Name:en-GB</remarks>
        public static readonly ICultureIdentifier English_UnitedKingdom = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_UnitedKingdom];
        ///<summary>
        ///Static culture identifier instance for English - United States.
        ///</summary>
        ///<remarks>Culture ID: 0x0409
        ///Culture Name:en-US</remarks>
        public static readonly ICultureIdentifier English_UnitedStates = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_UnitedStates];
        ///<summary>
        ///Static culture identifier instance for English - Zimbabwe.
        ///</summary>
        ///<remarks>Culture ID: 0x3009
        ///Culture Name:en-ZW</remarks>
        public static readonly ICultureIdentifier English_Zimbabwe = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.English_Zimbabwe];
        ///<summary>
        ///Static culture identifier instance for Estonian.
        ///</summary>
        ///<remarks>Culture ID: 0x0025
        ///Culture Name:et</remarks>
        public static readonly ICultureIdentifier Estonian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Estonian];
        ///<summary>
        ///Static culture identifier instance for Estonian - Estonia.
        ///</summary>
        ///<remarks>Culture ID: 0x0425
        ///Culture Name:et-EE</remarks>
        public static readonly ICultureIdentifier Estonian_Estonia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Estonian_Estonia];
        ///<summary>
        ///Static culture identifier instance for Faroese.
        ///</summary>
        ///<remarks>Culture ID: 0x0038
        ///Culture Name:fo</remarks>
        public static readonly ICultureIdentifier Faroese = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Faroese];
        ///<summary>
        ///Static culture identifier instance for Faroese - Faroe Islands.
        ///</summary>
        ///<remarks>Culture ID: 0x0438
        ///Culture Name:fo-FO</remarks>
        public static readonly ICultureIdentifier Faroese_FaroeIslands = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Faroese_FaroeIslands];
        ///<summary>
        ///Static culture identifier instance for Farsi.
        ///</summary>
        ///<remarks>Culture ID: 0x0029
        ///Culture Name:fa</remarks>
        public static readonly ICultureIdentifier Farsi = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Farsi];
        ///<summary>
        ///Static culture identifier instance for Farsi - Iran.
        ///</summary>
        ///<remarks>Culture ID: 0x0429
        ///Culture Name:fa-IR</remarks>
        public static readonly ICultureIdentifier Farsi_Iran = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Farsi_Iran];
        ///<summary>
        ///Static culture identifier instance for Finnish.
        ///</summary>
        ///<remarks>Culture ID: 0x000B
        ///Culture Name:fi</remarks>
        public static readonly ICultureIdentifier Finnish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Finnish];
        ///<summary>
        ///Static culture identifier instance for Finnish - Finland.
        ///</summary>
        ///<remarks>Culture ID: 0x040B
        ///Culture Name:fi-FI</remarks>
        public static readonly ICultureIdentifier Finnish_Finland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Finnish_Finland];
        ///<summary>
        ///Static culture identifier instance for French.
        ///</summary>
        ///<remarks>Culture ID: 0x000C
        ///Culture Name:fr</remarks>
        public static readonly ICultureIdentifier French = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French];
        ///<summary>
        ///Static culture identifier instance for French - Belgium.
        ///</summary>
        ///<remarks>Culture ID: 0x080C
        ///Culture Name:fr-BE</remarks>
        public static readonly ICultureIdentifier French_Belgium = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_Belgium];
        ///<summary>
        ///Static culture identifier instance for French - Canada.
        ///</summary>
        ///<remarks>Culture ID: 0x0C0C
        ///Culture Name:fr-CA</remarks>
        public static readonly ICultureIdentifier French_Canada = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_Canada];
        ///<summary>
        ///Static culture identifier instance for French - France.
        ///</summary>
        ///<remarks>Culture ID: 0x040C
        ///Culture Name:fr-FR</remarks>
        public static readonly ICultureIdentifier French_France = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_France];
        ///<summary>
        ///Static culture identifier instance for French - Luxembourg.
        ///</summary>
        ///<remarks>Culture ID: 0x140C
        ///Culture Name:fr-LU</remarks>
        public static readonly ICultureIdentifier French_Luxembourg = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_Luxembourg];
        ///<summary>
        ///Static culture identifier instance for French - Monaco.
        ///</summary>
        ///<remarks>Culture ID: 0x180C
        ///Culture Name:fr-MC</remarks>
        public static readonly ICultureIdentifier French_Monaco = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_Monaco];
        ///<summary>
        ///Static culture identifier instance for French - Switzerland.
        ///</summary>
        ///<remarks>Culture ID: 0x100C
        ///Culture Name:fr-CH</remarks>
        public static readonly ICultureIdentifier French_Switzerland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.French_Switzerland];
        ///<summary>
        ///Static culture identifier instance for Galician.
        ///</summary>
        ///<remarks>Culture ID: 0x0056
        ///Culture Name:gl</remarks>
        public static readonly ICultureIdentifier Galician = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Galician];
        ///<summary>
        ///Static culture identifier instance for Galician - Galician.
        ///</summary>
        ///<remarks>Culture ID: 0x0456
        ///Culture Name:gl-ES</remarks>
        public static readonly ICultureIdentifier Galician_Galician = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Galician_Galician];
        ///<summary>
        ///Static culture identifier instance for Georgian.
        ///</summary>
        ///<remarks>Culture ID: 0x0037
        ///Culture Name:ka</remarks>
        public static readonly ICultureIdentifier Georgian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Georgian];
        ///<summary>
        ///Static culture identifier instance for Georgian - Georgia.
        ///</summary>
        ///<remarks>Culture ID: 0x0437
        ///Culture Name:ka-GE</remarks>
        public static readonly ICultureIdentifier Georgian_Georgia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Georgian_Georgia];
        ///<summary>
        ///Static culture identifier instance for German.
        ///</summary>
        ///<remarks>Culture ID: 0x0007
        ///Culture Name:de</remarks>
        public static readonly ICultureIdentifier German = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German];
        ///<summary>
        ///Static culture identifier instance for German - Austria.
        ///</summary>
        ///<remarks>Culture ID: 0x0C07
        ///Culture Name:de-AT</remarks>
        public static readonly ICultureIdentifier German_Austria = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German_Austria];
        ///<summary>
        ///Static culture identifier instance for German - Germany.
        ///</summary>
        ///<remarks>Culture ID: 0x0407
        ///Culture Name:de-DE</remarks>
        public static readonly ICultureIdentifier German_Germany = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German_Germany];
        ///<summary>
        ///Static culture identifier instance for German - Liechtenstein.
        ///</summary>
        ///<remarks>Culture ID: 0x1407
        ///Culture Name:de-LI</remarks>
        public static readonly ICultureIdentifier German_Liechtenstein = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German_Liechtenstein];
        ///<summary>
        ///Static culture identifier instance for German - Luxembourg.
        ///</summary>
        ///<remarks>Culture ID: 0x1007
        ///Culture Name:de-LU</remarks>
        public static readonly ICultureIdentifier German_Luxembourg = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German_Luxembourg];
        ///<summary>
        ///Static culture identifier instance for German - Switzerland.
        ///</summary>
        ///<remarks>Culture ID: 0x0807
        ///Culture Name:de-CH</remarks>
        public static readonly ICultureIdentifier German_Switzerland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.German_Switzerland];
        ///<summary>
        ///Static culture identifier instance for Greek.
        ///</summary>
        ///<remarks>Culture ID: 0x0008
        ///Culture Name:el</remarks>
        public static readonly ICultureIdentifier Greek = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Greek];
        ///<summary>
        ///Static culture identifier instance for Greek - Greece.
        ///</summary>
        ///<remarks>Culture ID: 0x0408
        ///Culture Name:el-GR</remarks>
        public static readonly ICultureIdentifier Greek_Greece = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Greek_Greece];
        ///<summary>
        ///Static culture identifier instance for Gujarati.
        ///</summary>
        ///<remarks>Culture ID: 0x0047
        ///Culture Name:gu</remarks>
        public static readonly ICultureIdentifier Gujarati = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Gujarati];
        ///<summary>
        ///Static culture identifier instance for Gujarati - India.
        ///</summary>
        ///<remarks>Culture ID: 0x0447
        ///Culture Name:gu-IN</remarks>
        public static readonly ICultureIdentifier Gujarati_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Gujarati_India];
        ///<summary>
        ///Static culture identifier instance for Hebrew.
        ///</summary>
        ///<remarks>Culture ID: 0x000D
        ///Culture Name:he</remarks>
        public static readonly ICultureIdentifier Hebrew = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hebrew];
        ///<summary>
        ///Static culture identifier instance for Hebrew - Israel.
        ///</summary>
        ///<remarks>Culture ID: 0x040D
        ///Culture Name:he-IL</remarks>
        public static readonly ICultureIdentifier Hebrew_Israel = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hebrew_Israel];
        ///<summary>
        ///Static culture identifier instance for Hindi.
        ///</summary>
        ///<remarks>Culture ID: 0x0039
        ///Culture Name:hi</remarks>
        public static readonly ICultureIdentifier Hindi = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hindi];
        ///<summary>
        ///Static culture identifier instance for Hindi - India.
        ///</summary>
        ///<remarks>Culture ID: 0x0439
        ///Culture Name:hi-IN</remarks>
        public static readonly ICultureIdentifier Hindi_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hindi_India];
        ///<summary>
        ///Static culture identifier instance for Hungarian.
        ///</summary>
        ///<remarks>Culture ID: 0x000E
        ///Culture Name:hu</remarks>
        public static readonly ICultureIdentifier Hungarian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hungarian];
        ///<summary>
        ///Static culture identifier instance for Hungarian - Hungary.
        ///</summary>
        ///<remarks>Culture ID: 0x040E
        ///Culture Name:hu-HU</remarks>
        public static readonly ICultureIdentifier Hungarian_Hungary = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Hungarian_Hungary];
        ///<summary>
        ///Static culture identifier instance for Icelandic.
        ///</summary>
        ///<remarks>Culture ID: 0x000F
        ///Culture Name:is</remarks>
        public static readonly ICultureIdentifier Icelandic = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Icelandic];
        ///<summary>
        ///Static culture identifier instance for Icelandic - Iceland.
        ///</summary>
        ///<remarks>Culture ID: 0x040F
        ///Culture Name:is-IS</remarks>
        public static readonly ICultureIdentifier Icelandic_Iceland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Icelandic_Iceland];
        ///<summary>
        ///Static culture identifier instance for Indonesian.
        ///</summary>
        ///<remarks>Culture ID: 0x0021
        ///Culture Name:id</remarks>
        public static readonly ICultureIdentifier Indonesian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Indonesian];
        ///<summary>
        ///Static culture identifier instance for Indonesian - Indonesia.
        ///</summary>
        ///<remarks>Culture ID: 0x0421
        ///Culture Name:id-ID</remarks>
        public static readonly ICultureIdentifier Indonesian_Indonesia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Indonesian_Indonesia];
        ///<summary>
        ///Static culture identifier instance for Italian.
        ///</summary>
        ///<remarks>Culture ID: 0x0010
        ///Culture Name:it</remarks>
        public static readonly ICultureIdentifier Italian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Italian];
        ///<summary>
        ///Static culture identifier instance for Italian - Italy.
        ///</summary>
        ///<remarks>Culture ID: 0x0410
        ///Culture Name:it-IT</remarks>
        public static readonly ICultureIdentifier Italian_Italy = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Italian_Italy];
        ///<summary>
        ///Static culture identifier instance for Italian - Switzerland.
        ///</summary>
        ///<remarks>Culture ID: 0x0810
        ///Culture Name:it-CH</remarks>
        public static readonly ICultureIdentifier Italian_Switzerland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Italian_Switzerland];
        ///<summary>
        ///Static culture identifier instance for Japanese.
        ///</summary>
        ///<remarks>Culture ID: 0x0011
        ///Culture Name:ja</remarks>
        public static readonly ICultureIdentifier Japanese = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Japanese];
        ///<summary>
        ///Static culture identifier instance for Japanese - Japan.
        ///</summary>
        ///<remarks>Culture ID: 0x0411
        ///Culture Name:ja-JP</remarks>
        public static readonly ICultureIdentifier Japanese_Japan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Japanese_Japan];
        ///<summary>
        ///Static culture identifier instance for Kannada.
        ///</summary>
        ///<remarks>Culture ID: 0x004B
        ///Culture Name:kn</remarks>
        public static readonly ICultureIdentifier Kannada = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kannada];
        ///<summary>
        ///Static culture identifier instance for Kannada - India.
        ///</summary>
        ///<remarks>Culture ID: 0x044B
        ///Culture Name:kn-IN</remarks>
        public static readonly ICultureIdentifier Kannada_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kannada_India];
        ///<summary>
        ///Static culture identifier instance for Kazakh.
        ///</summary>
        ///<remarks>Culture ID: 0x003F
        ///Culture Name:kk</remarks>
        public static readonly ICultureIdentifier Kazakh = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kazakh];
        ///<summary>
        ///Static culture identifier instance for Kazakh - Kazakhstan.
        ///</summary>
        ///<remarks>Culture ID: 0x043F
        ///Culture Name:kk-KZ</remarks>
        public static readonly ICultureIdentifier Kazakh_Kazakhstan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kazakh_Kazakhstan];
        ///<summary>
        ///Static culture identifier instance for Konkani.
        ///</summary>
        ///<remarks>Culture ID: 0x0057
        ///Culture Name:kok</remarks>
        public static readonly ICultureIdentifier Konkani = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Konkani];
        ///<summary>
        ///Static culture identifier instance for Konkani - India.
        ///</summary>
        ///<remarks>Culture ID: 0x0457
        ///Culture Name:kok-IN</remarks>
        public static readonly ICultureIdentifier Konkani_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Konkani_India];
        ///<summary>
        ///Static culture identifier instance for Korean.
        ///</summary>
        ///<remarks>Culture ID: 0x0012
        ///Culture Name:ko</remarks>
        public static readonly ICultureIdentifier Korean = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Korean];
        ///<summary>
        ///Static culture identifier instance for Korean - Korea.
        ///</summary>
        ///<remarks>Culture ID: 0x0412
        ///Culture Name:ko-KR</remarks>
        public static readonly ICultureIdentifier Korean_Korea = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Korean_Korea];
        ///<summary>
        ///Static culture identifier instance for Kyrgyz.
        ///</summary>
        ///<remarks>Culture ID: 0x0040
        ///Culture Name:ky</remarks>
        public static readonly ICultureIdentifier Kyrgyz = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kyrgyz];
        ///<summary>
        ///Static culture identifier instance for Kyrgyz - Kyrgyzstan.
        ///</summary>
        ///<remarks>Culture ID: 0x0440
        ///Culture Name:ky-KG</remarks>
        public static readonly ICultureIdentifier Kyrgyz_Kyrgyzstan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Kyrgyz_Kyrgyzstan];
        ///<summary>
        ///Static culture identifier instance for Latvian.
        ///</summary>
        ///<remarks>Culture ID: 0x0026
        ///Culture Name:lv</remarks>
        public static readonly ICultureIdentifier Latvian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Latvian];
        ///<summary>
        ///Static culture identifier instance for Latvian - Latvia.
        ///</summary>
        ///<remarks>Culture ID: 0x0426
        ///Culture Name:lv-LV</remarks>
        public static readonly ICultureIdentifier Latvian_Latvia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Latvian_Latvia];
        ///<summary>
        ///Static culture identifier instance for Lithuanian.
        ///</summary>
        ///<remarks>Culture ID: 0x0027
        ///Culture Name:lt</remarks>
        public static readonly ICultureIdentifier Lithuanian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Lithuanian];
        ///<summary>
        ///Static culture identifier instance for Lithuanian - Lithuania.
        ///</summary>
        ///<remarks>Culture ID: 0x0427
        ///Culture Name:lt-LT</remarks>
        public static readonly ICultureIdentifier Lithuanian_Lithuania = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Lithuanian_Lithuania];
        ///<summary>
        ///Static culture identifier instance for Macedonian.
        ///</summary>
        ///<remarks>Culture ID: 0x002F
        ///Culture Name:mk</remarks>
        public static readonly ICultureIdentifier Macedonian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Macedonian];
        ///<summary>
        ///Static culture identifier instance for Macedonian - Former Yugoslav Republic of Macedonia.
        ///</summary>
        ///<remarks>Culture ID: 0x042F
        ///Culture Name:mk-MK</remarks>
        public static readonly ICultureIdentifier Macedonian_FormerYugoslavRepublicOfMacedonia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Macedonian_FormerYugoslavRepublicOfMacedonia];
        ///<summary>
        ///Static culture identifier instance for Malay.
        ///</summary>
        ///<remarks>Culture ID: 0x003E
        ///Culture Name:ms</remarks>
        public static readonly ICultureIdentifier Malay = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Malay];
        ///<summary>
        ///Static culture identifier instance for Malay - Brunei.
        ///</summary>
        ///<remarks>Culture ID: 0x083E
        ///Culture Name:ms-BN</remarks>
        public static readonly ICultureIdentifier Malay_Brunei = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Malay_Brunei];
        ///<summary>
        ///Static culture identifier instance for Malay - Malaysia.
        ///</summary>
        ///<remarks>Culture ID: 0x043E
        ///Culture Name:ms-MY</remarks>
        public static readonly ICultureIdentifier Malay_Malaysia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Malay_Malaysia];
        ///<summary>
        ///Static culture identifier instance for Marathi.
        ///</summary>
        ///<remarks>Culture ID: 0x004E
        ///Culture Name:mr</remarks>
        public static readonly ICultureIdentifier Marathi = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Marathi];
        ///<summary>
        ///Static culture identifier instance for Marathi - India.
        ///</summary>
        ///<remarks>Culture ID: 0x044E
        ///Culture Name:mr-IN</remarks>
        public static readonly ICultureIdentifier Marathi_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Marathi_India];
        ///<summary>
        ///Static culture identifier instance for Mongolian.
        ///</summary>
        ///<remarks>Culture ID: 0x0050
        ///Culture Name:mn</remarks>
        public static readonly ICultureIdentifier Mongolian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Mongolian];
        ///<summary>
        ///Static culture identifier instance for Mongolian - Mongolia.
        ///</summary>
        ///<remarks>Culture ID: 0x0450
        ///Culture Name:mn-MN</remarks>
        public static readonly ICultureIdentifier Mongolian_Mongolia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Mongolian_Mongolia];
        ///<summary>
        ///Static culture identifier instance for Norwegian.
        ///</summary>
        ///<remarks>Culture ID: 0x0014
        ///Culture Name:no</remarks>
        public static readonly ICultureIdentifier Norwegian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Norwegian];
        ///<summary>
        ///Static culture identifier instance for Norwegian (Bokmål) - Norway.
        ///</summary>
        ///<remarks>Culture ID: 0x0414
        ///Culture Name:nb-NO</remarks>
        public static readonly ICultureIdentifier Norwegian_Bokmål_Norway = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Norwegian_Bokmål_Norway];
        ///<summary>
        ///Static culture identifier instance for Norwegian (Nynorsk) - Norway.
        ///</summary>
        ///<remarks>Culture ID: 0x0814
        ///Culture Name:nn-NO</remarks>
        public static readonly ICultureIdentifier Norwegian_Nynorsk_Norway = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Norwegian_Nynorsk_Norway];
        ///<summary>
        ///Static culture identifier instance for Polish.
        ///</summary>
        ///<remarks>Culture ID: 0x0015
        ///Culture Name:pl</remarks>
        public static readonly ICultureIdentifier Polish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Polish];
        ///<summary>
        ///Static culture identifier instance for Polish - Poland.
        ///</summary>
        ///<remarks>Culture ID: 0x0415
        ///Culture Name:pl-PL</remarks>
        public static readonly ICultureIdentifier Polish_Poland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Polish_Poland];
        ///<summary>
        ///Static culture identifier instance for Portuguese.
        ///</summary>
        ///<remarks>Culture ID: 0x0016
        ///Culture Name:pt</remarks>
        public static readonly ICultureIdentifier Portuguese = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Portuguese];
        ///<summary>
        ///Static culture identifier instance for Portuguese - Brazil.
        ///</summary>
        ///<remarks>Culture ID: 0x0416
        ///Culture Name:pt-BR</remarks>
        public static readonly ICultureIdentifier Portuguese_Brazil = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Portuguese_Brazil];
        ///<summary>
        ///Static culture identifier instance for Portuguese - Portugal.
        ///</summary>
        ///<remarks>Culture ID: 0x0816
        ///Culture Name:pt-PT</remarks>
        public static readonly ICultureIdentifier Portuguese_Portugal = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Portuguese_Portugal];
        ///<summary>
        ///Static culture identifier instance for Punjabi.
        ///</summary>
        ///<remarks>Culture ID: 0x0046
        ///Culture Name:pa</remarks>
        public static readonly ICultureIdentifier Punjabi = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Punjabi];
        ///<summary>
        ///Static culture identifier instance for Punjabi - India.
        ///</summary>
        ///<remarks>Culture ID: 0x0446
        ///Culture Name:pa-IN</remarks>
        public static readonly ICultureIdentifier Punjabi_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Punjabi_India];
        ///<summary>
        ///Static culture identifier instance for Romanian.
        ///</summary>
        ///<remarks>Culture ID: 0x0018
        ///Culture Name:ro</remarks>
        public static readonly ICultureIdentifier Romanian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Romanian];
        ///<summary>
        ///Static culture identifier instance for Romanian - Romania.
        ///</summary>
        ///<remarks>Culture ID: 0x0418
        ///Culture Name:ro-RO</remarks>
        public static readonly ICultureIdentifier Romanian_Romania = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Romanian_Romania];
        ///<summary>
        ///Static culture identifier instance for Russian.
        ///</summary>
        ///<remarks>Culture ID: 0x0019
        ///Culture Name:ru</remarks>
        public static readonly ICultureIdentifier Russian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Russian];
        ///<summary>
        ///Static culture identifier instance for Russian - Russia.
        ///</summary>
        ///<remarks>Culture ID: 0x0419
        ///Culture Name:ru-RU</remarks>
        public static readonly ICultureIdentifier Russian_Russia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Russian_Russia];
        ///<summary>
        ///Static culture identifier instance for Sanskrit.
        ///</summary>
        ///<remarks>Culture ID: 0x004F
        ///Culture Name:sa</remarks>
        public static readonly ICultureIdentifier Sanskrit = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Sanskrit];
        ///<summary>
        ///Static culture identifier instance for Sanskrit - India.
        ///</summary>
        ///<remarks>Culture ID: 0x044F
        ///Culture Name:sa-IN</remarks>
        public static readonly ICultureIdentifier Sanskrit_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Sanskrit_India];
        ///<summary>
        ///Static culture identifier instance for Serbian (Cyrillic) - Serbia.
        ///</summary>
        ///<remarks>Culture ID: 0x0C1A
        ///Culture Name:sr-SP-Cyrl</remarks>
        public static readonly ICultureIdentifier Serbian_Cyrillic_Serbia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Serbian_Cyrillic_Serbia];
        ///<summary>
        ///Static culture identifier instance for Serbian (Latin) - Serbia.
        ///</summary>
        ///<remarks>Culture ID: 0x081A
        ///Culture Name:sr-SP-Latn</remarks>
        public static readonly ICultureIdentifier Serbian_Latin_Serbia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Serbian_Latin_Serbia];
        ///<summary>
        ///Static culture identifier instance for Slovak.
        ///</summary>
        ///<remarks>Culture ID: 0x001B
        ///Culture Name:sk</remarks>
        public static readonly ICultureIdentifier Slovak = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Slovak];
        ///<summary>
        ///Static culture identifier instance for Slovak - Slovakia.
        ///</summary>
        ///<remarks>Culture ID: 0x041B
        ///Culture Name:sk-SK</remarks>
        public static readonly ICultureIdentifier Slovak_Slovakia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Slovak_Slovakia];
        ///<summary>
        ///Static culture identifier instance for Slovenian.
        ///</summary>
        ///<remarks>Culture ID: 0x0024
        ///Culture Name:sl</remarks>
        public static readonly ICultureIdentifier Slovenian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Slovenian];
        ///<summary>
        ///Static culture identifier instance for Slovenian - Slovenia.
        ///</summary>
        ///<remarks>Culture ID: 0x0424
        ///Culture Name:sl-SI</remarks>
        public static readonly ICultureIdentifier Slovenian_Slovenia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Slovenian_Slovenia];
        ///<summary>
        ///Static culture identifier instance for Spanish.
        ///</summary>
        ///<remarks>Culture ID: 0x000A
        ///Culture Name:es</remarks>
        public static readonly ICultureIdentifier Spanish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish];
        ///<summary>
        ///Static culture identifier instance for Spanish - Argentina.
        ///</summary>
        ///<remarks>Culture ID: 0x2C0A
        ///Culture Name:es-AR</remarks>
        public static readonly ICultureIdentifier Spanish_Argentina = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Argentina];
        ///<summary>
        ///Static culture identifier instance for Spanish - Bolivia.
        ///</summary>
        ///<remarks>Culture ID: 0x400A
        ///Culture Name:es-BO</remarks>
        public static readonly ICultureIdentifier Spanish_Bolivia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Bolivia];
        ///<summary>
        ///Static culture identifier instance for Spanish - Chile.
        ///</summary>
        ///<remarks>Culture ID: 0x340A
        ///Culture Name:es-CL</remarks>
        public static readonly ICultureIdentifier Spanish_Chile = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Chile];
        ///<summary>
        ///Static culture identifier instance for Spanish - Colombia.
        ///</summary>
        ///<remarks>Culture ID: 0x240A
        ///Culture Name:es-CO</remarks>
        public static readonly ICultureIdentifier Spanish_Colombia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Colombia];
        ///<summary>
        ///Static culture identifier instance for Spanish - Costa Rica.
        ///</summary>
        ///<remarks>Culture ID: 0x140A
        ///Culture Name:es-CR</remarks>
        public static readonly ICultureIdentifier Spanish_CostaRica = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_CostaRica];
        ///<summary>
        ///Static culture identifier instance for Spanish - Dominican Republic.
        ///</summary>
        ///<remarks>Culture ID: 0x1C0A
        ///Culture Name:es-DO</remarks>
        public static readonly ICultureIdentifier Spanish_DominicanRepublic = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_DominicanRepublic];
        ///<summary>
        ///Static culture identifier instance for Spanish - Ecuador.
        ///</summary>
        ///<remarks>Culture ID: 0x300A
        ///Culture Name:es-EC</remarks>
        public static readonly ICultureIdentifier Spanish_Ecuador = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Ecuador];
        ///<summary>
        ///Static culture identifier instance for Spanish - El Salvador.
        ///</summary>
        ///<remarks>Culture ID: 0x440A
        ///Culture Name:es-SV</remarks>
        public static readonly ICultureIdentifier Spanish_ElSalvador = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_ElSalvador];
        ///<summary>
        ///Static culture identifier instance for Spanish - Guatemala.
        ///</summary>
        ///<remarks>Culture ID: 0x100A
        ///Culture Name:es-GT</remarks>
        public static readonly ICultureIdentifier Spanish_Guatemala = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Guatemala];
        ///<summary>
        ///Static culture identifier instance for Spanish - Honduras.
        ///</summary>
        ///<remarks>Culture ID: 0x480A
        ///Culture Name:es-HN</remarks>
        public static readonly ICultureIdentifier Spanish_Honduras = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Honduras];
        ///<summary>
        ///Static culture identifier instance for Spanish - Mexico.
        ///</summary>
        ///<remarks>Culture ID: 0x080A
        ///Culture Name:es-MX</remarks>
        public static readonly ICultureIdentifier Spanish_Mexico = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Mexico];
        ///<summary>
        ///Static culture identifier instance for Spanish - Nicaragua.
        ///</summary>
        ///<remarks>Culture ID: 0x4C0A
        ///Culture Name:es-NI</remarks>
        public static readonly ICultureIdentifier Spanish_Nicaragua = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Nicaragua];
        ///<summary>
        ///Static culture identifier instance for Spanish - Panama.
        ///</summary>
        ///<remarks>Culture ID: 0x180A
        ///Culture Name:es-PA</remarks>
        public static readonly ICultureIdentifier Spanish_Panama = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Panama];
        ///<summary>
        ///Static culture identifier instance for Spanish - Paraguay.
        ///</summary>
        ///<remarks>Culture ID: 0x3C0A
        ///Culture Name:es-PY</remarks>
        public static readonly ICultureIdentifier Spanish_Paraguay = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Paraguay];
        ///<summary>
        ///Static culture identifier instance for Spanish - Peru.
        ///</summary>
        ///<remarks>Culture ID: 0x280A
        ///Culture Name:es-PE</remarks>
        public static readonly ICultureIdentifier Spanish_Peru = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Peru];
        ///<summary>
        ///Static culture identifier instance for Spanish - Puerto Rico.
        ///</summary>
        ///<remarks>Culture ID: 0x500A
        ///Culture Name:es-PR</remarks>
        public static readonly ICultureIdentifier Spanish_PuertoRico = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_PuertoRico];
        ///<summary>
        ///Static culture identifier instance for Spanish - Spain.
        ///</summary>
        ///<remarks>Culture ID: 0x0C0A
        ///Culture Name:es-ES</remarks>
        public static readonly ICultureIdentifier Spanish_Spain = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Spain];
        ///<summary>
        ///Static culture identifier instance for Spanish - Uruguay.
        ///</summary>
        ///<remarks>Culture ID: 0x380A
        ///Culture Name:es-UY</remarks>
        public static readonly ICultureIdentifier Spanish_Uruguay = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Uruguay];
        ///<summary>
        ///Static culture identifier instance for Spanish - Venezuela.
        ///</summary>
        ///<remarks>Culture ID: 0x200A
        ///Culture Name:es-VE</remarks>
        public static readonly ICultureIdentifier Spanish_Venezuela = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Spanish_Venezuela];
        ///<summary>
        ///Static culture identifier instance for Swahili.
        ///</summary>
        ///<remarks>Culture ID: 0x0041
        ///Culture Name:sw</remarks>
        public static readonly ICultureIdentifier Swahili = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Swahili];
        ///<summary>
        ///Static culture identifier instance for Swahili - Kenya.
        ///</summary>
        ///<remarks>Culture ID: 0x0441
        ///Culture Name:sw-KE</remarks>
        public static readonly ICultureIdentifier Swahili_Kenya = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Swahili_Kenya];
        ///<summary>
        ///Static culture identifier instance for Swedish.
        ///</summary>
        ///<remarks>Culture ID: 0x001D
        ///Culture Name:sv</remarks>
        public static readonly ICultureIdentifier Swedish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Swedish];
        ///<summary>
        ///Static culture identifier instance for Swedish - Finland.
        ///</summary>
        ///<remarks>Culture ID: 0x081D
        ///Culture Name:sv-FI</remarks>
        public static readonly ICultureIdentifier Swedish_Finland = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Swedish_Finland];
        ///<summary>
        ///Static culture identifier instance for Swedish - Sweden.
        ///</summary>
        ///<remarks>Culture ID: 0x041D
        ///Culture Name:sv-SE</remarks>
        public static readonly ICultureIdentifier Swedish_Sweden = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Swedish_Sweden];
        ///<summary>
        ///Static culture identifier instance for Syriac.
        ///</summary>
        ///<remarks>Culture ID: 0x005A
        ///Culture Name:syr</remarks>
        public static readonly ICultureIdentifier Syriac = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Syriac];
        ///<summary>
        ///Static culture identifier instance for Syriac - Syria.
        ///</summary>
        ///<remarks>Culture ID: 0x045A
        ///Culture Name:syr-SY</remarks>
        public static readonly ICultureIdentifier Syriac_Syria = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Syriac_Syria];
        ///<summary>
        ///Static culture identifier instance for Tamil.
        ///</summary>
        ///<remarks>Culture ID: 0x0049
        ///Culture Name:ta</remarks>
        public static readonly ICultureIdentifier Tamil = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Tamil];
        ///<summary>
        ///Static culture identifier instance for Tamil - India.
        ///</summary>
        ///<remarks>Culture ID: 0x0449
        ///Culture Name:ta-IN</remarks>
        public static readonly ICultureIdentifier Tamil_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Tamil_India];
        ///<summary>
        ///Static culture identifier instance for Tatar.
        ///</summary>
        ///<remarks>Culture ID: 0x0044
        ///Culture Name:tt</remarks>
        public static readonly ICultureIdentifier Tatar = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Tatar];
        ///<summary>
        ///Static culture identifier instance for Tatar - Russia.
        ///</summary>
        ///<remarks>Culture ID: 0x0444
        ///Culture Name:tt-RU</remarks>
        public static readonly ICultureIdentifier Tatar_Russia = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Tatar_Russia];
        ///<summary>
        ///Static culture identifier instance for Telugu.
        ///</summary>
        ///<remarks>Culture ID: 0x004A
        ///Culture Name:te</remarks>
        public static readonly ICultureIdentifier Telugu = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Telugu];
        ///<summary>
        ///Static culture identifier instance for Telugu - India.
        ///</summary>
        ///<remarks>Culture ID: 0x044A
        ///Culture Name:te-IN</remarks>
        public static readonly ICultureIdentifier Telugu_India = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Telugu_India];
        ///<summary>
        ///Static culture identifier instance for Thai.
        ///</summary>
        ///<remarks>Culture ID: 0x001E
        ///Culture Name:th</remarks>
        public static readonly ICultureIdentifier Thai = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Thai];
        ///<summary>
        ///Static culture identifier instance for Thai - Thailand.
        ///</summary>
        ///<remarks>Culture ID: 0x041E
        ///Culture Name:th-TH</remarks>
        public static readonly ICultureIdentifier Thai_Thailand = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Thai_Thailand];
        ///<summary>
        ///Static culture identifier instance for Turkish.
        ///</summary>
        ///<remarks>Culture ID: 0x001F
        ///Culture Name:tr</remarks>
        public static readonly ICultureIdentifier Turkish = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Turkish];
        ///<summary>
        ///Static culture identifier instance for Turkish - Turkey.
        ///</summary>
        ///<remarks>Culture ID: 0x041F
        ///Culture Name:tr-TR</remarks>
        public static readonly ICultureIdentifier Turkish_Turkey = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Turkish_Turkey];
        ///<summary>
        ///Static culture identifier instance for Ukrainian.
        ///</summary>
        ///<remarks>Culture ID: 0x0022
        ///Culture Name:uk</remarks>
        public static readonly ICultureIdentifier Ukrainian = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Ukrainian];
        ///<summary>
        ///Static culture identifier instance for Ukrainian - Ukraine.
        ///</summary>
        ///<remarks>Culture ID: 0x0422
        ///Culture Name:uk-UA</remarks>
        public static readonly ICultureIdentifier Ukrainian_Ukraine = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Ukrainian_Ukraine];
        ///<summary>
        ///Static culture identifier instance for Urdu.
        ///</summary>
        ///<remarks>Culture ID: 0x0020
        ///Culture Name:ur</remarks>
        public static readonly ICultureIdentifier Urdu = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Urdu];
        ///<summary>
        ///Static culture identifier instance for Urdu - Pakistan.
        ///</summary>
        ///<remarks>Culture ID: 0x0420
        ///Culture Name:ur-PK</remarks>
        public static readonly ICultureIdentifier Urdu_Pakistan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Urdu_Pakistan];
        ///<summary>
        ///Static culture identifier instance for Uzbek.
        ///</summary>
        ///<remarks>Culture ID: 0x0043
        ///Culture Name:uz</remarks>
        public static readonly ICultureIdentifier Uzbek = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Uzbek];
        ///<summary>
        ///Static culture identifier instance for Uzbek (Cyrillic) - Uzbekistan.
        ///</summary>
        ///<remarks>Culture ID: 0x0843
        ///Culture Name:uz-UZ-Cyrl</remarks>
        public static readonly ICultureIdentifier Uzbek_Cyrillic_Uzbekistan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Uzbek_Cyrillic_Uzbekistan];
        ///<summary>
        ///Static culture identifier instance for Uzbek (Latin) - Uzbekistan.
        ///</summary>
        ///<remarks>Culture ID: 0x0443
        ///Culture Name:uz-UZ-Latn</remarks>
        public static readonly ICultureIdentifier Uzbek_Latin_Uzbekistan = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Uzbek_Latin_Uzbekistan];
        ///<summary>
        ///Static culture identifier instance for Vietnamese.
        ///</summary>
        ///<remarks>Culture ID: 0x002A
        ///Culture Name:vi</remarks>
        public static readonly ICultureIdentifier Vietnamese = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Vietnamese];
        /// <summary>
        /// Static culture identifier instance for Vietnamese - Vietnam
        /// </summary>
        /// <remarks>
        /// Culture ID: 0x042A
        /// Culture Name: vi-VN
        /// </remarks>
        public static readonly ICultureIdentifier Vietnamese_Vietnam = CultureIdentifier.defaultCultureIDByCultureNumber[IDs.Vietnamese_Vietnam];
        /// <summary>
        /// Provides a series of constant string values that relate to culture information and the 
        /// short-hand names of the cultures.
        /// </summary>
        public static class Names
        {

            /// <summary>
            /// Culture name for None.
            /// </summary>
            /// <remarks>CultureID: 0x007F
            /// CultureName: <see cref="System.String.Empty"/></remarks>
            public const string None = "";
            ///<summary>
            ///Culture name for Afrikaans.
            ///</summary>
            ///<remarks>Culture ID: 0x0036
            ///Culture Name: af</remarks>
            public const string Afrikaans = "af";
            ///<summary>
            ///Culture name for Afrikaans - South Africa.
            ///</summary>
            ///<remarks>Culture ID: 0x0436
            ///Culture Name: af-ZA</remarks>
            public const string Afrikaans_SouthAfrica = "af-ZA";
            ///<summary>
            ///Culture name for Albanian.
            ///</summary>
            ///<remarks>Culture ID: 0x001C
            ///Culture Name: sq</remarks>
            public const string Albanian = "sq";
            ///<summary>
            ///Culture name for Albanian - Albania.
            ///</summary>
            ///<remarks>Culture ID: 0x041C
            ///Culture Name: sq-AL</remarks>
            public const string Albanian_Albania = "sq-AL";
            ///<summary>
            ///Culture name for Arabic.
            ///</summary>
            ///<remarks>Culture ID: 0x0001
            ///Culture Name: ar</remarks>
            public const string Arabic = "ar";
            ///<summary>
            ///Culture name for Arabic - Algeria.
            ///</summary>
            ///<remarks>Culture ID: 0x1401
            ///Culture Name: ar-DZ</remarks>
            public const string Arabic_Algeria = "ar-DZ";
            ///<summary>
            ///Culture name for Arabic - Bahrain.
            ///</summary>
            ///<remarks>Culture ID: 0x3C01
            ///Culture Name: ar-BH</remarks>
            public const string Arabic_Bahrain = "ar-BH";
            ///<summary>
            ///Culture name for Arabic - Egypt.
            ///</summary>
            ///<remarks>Culture ID: 0x0C01
            ///Culture Name: ar-EG</remarks>
            public const string Arabic_Egypt = "ar-EG";
            ///<summary>
            ///Culture name for Arabic - Iraq.
            ///</summary>
            ///<remarks>Culture ID: 0x0801
            ///Culture Name: ar-IQ</remarks>
            public const string Arabic_Iraq = "ar-IQ";
            ///<summary>
            ///Culture name for Arabic - Jordan.
            ///</summary>
            ///<remarks>Culture ID: 0x2C01
            ///Culture Name: ar-JO</remarks>
            public const string Arabic_Jordan = "ar-JO";
            ///<summary>
            ///Culture name for Arabic - Kuwait.
            ///</summary>
            ///<remarks>Culture ID: 0x3401
            ///Culture Name: ar-KW</remarks>
            public const string Arabic_Kuwait = "ar-KW";
            ///<summary>
            ///Culture name for Arabic - Lebanon.
            ///</summary>
            ///<remarks>Culture ID: 0x3001
            ///Culture Name: ar-LB</remarks>
            public const string Arabic_Lebanon = "ar-LB";
            ///<summary>
            ///Culture name for Arabic - Libya.
            ///</summary>
            ///<remarks>Culture ID: 0x1001
            ///Culture Name: ar-LY</remarks>
            public const string Arabic_Libya = "ar-LY";
            ///<summary>
            ///Culture name for Arabic - Morocco.
            ///</summary>
            ///<remarks>Culture ID: 0x1801
            ///Culture Name: ar-MA</remarks>
            public const string Arabic_Morocco = "ar-MA";
            ///<summary>
            ///Culture name for Arabic - Oman.
            ///</summary>
            ///<remarks>Culture ID: 0x2001
            ///Culture Name: ar-OM</remarks>
            public const string Arabic_Oman = "ar-OM";
            ///<summary>
            ///Culture name for Arabic - Qatar.
            ///</summary>
            ///<remarks>Culture ID: 0x4001
            ///Culture Name: ar-QA</remarks>
            public const string Arabic_Qatar = "ar-QA";
            ///<summary>
            ///Culture name for Arabic - Saudi Arabia.
            ///</summary>
            ///<remarks>Culture ID: 0x0401
            ///Culture Name: ar-SA</remarks>
            public const string Arabic_SaudiArabia = "ar-SA";
            ///<summary>
            ///Culture name for Arabic - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x2801
            ///Culture Name: ar-SY</remarks>
            public const string Arabic_Syria = "ar-SY";
            ///<summary>
            ///Culture name for Arabic - Tunisia.
            ///</summary>
            ///<remarks>Culture ID: 0x1C01
            ///Culture Name: ar-TN</remarks>
            public const string Arabic_Tunisia = "ar-TN";
            ///<summary>
            ///Culture name for Arabic - United Arab Emirates.
            ///</summary>
            ///<remarks>Culture ID: 0x3801
            ///Culture Name: ar-AE</remarks>
            public const string Arabic_UnitedArabEmirates = "ar-AE";
            ///<summary>
            ///Culture name for Arabic - Yemen.
            ///</summary>
            ///<remarks>Culture ID: 0x2401
            ///Culture Name: ar-YE</remarks>
            public const string Arabic_Yemen = "ar-YE";
            ///<summary>
            ///Culture name for Armenian.
            ///</summary>
            ///<remarks>Culture ID: 0x002B
            ///Culture Name: hy</remarks>
            public const string Armenian = "hy";
            ///<summary>
            ///Culture name for Armenian - Armenia.
            ///</summary>
            ///<remarks>Culture ID: 0x042B
            ///Culture Name: hy-AM</remarks>
            public const string Armenian_Armenia = "hy-AM";
            ///<summary>
            ///Culture name for Azeri.
            ///</summary>
            ///<remarks>Culture ID: 0x002C
            ///Culture Name: az</remarks>
            public const string Azeri = "az";
            ///<summary>
            ///Culture name for Azeri (Cyrillic) - Azerbaijan.
            ///</summary>
            ///<remarks>Culture ID: 0x082C
            ///Culture Name: az-AZ-Cyrl</remarks>
            public const string Azeri_Cyrillic_Azerbaijan = "az-AZ-Cyrl";
            ///<summary>
            ///Culture name for Azeri (Latin) - Azerbaijan.
            ///</summary>
            ///<remarks>Culture ID: 0x042C
            ///Culture Name: az-AZ-Latn</remarks>
            public const string Azeri_Latin_Azerbaijan = "az-AZ-Latn";
            ///<summary>
            ///Culture name for Basque.
            ///</summary>
            ///<remarks>Culture ID: 0x002D
            ///Culture Name: eu</remarks>
            public const string Basque = "eu";
            ///<summary>
            ///Culture name for Basque - Basque.
            ///</summary>
            ///<remarks>Culture ID: 0x042D
            ///Culture Name: eu-ES</remarks>
            public const string Basque_Basque = "eu-ES";
            ///<summary>
            ///Culture name for Belarusian.
            ///</summary>
            ///<remarks>Culture ID: 0x0023
            ///Culture Name: be</remarks>
            public const string Belarusian = "be";
            ///<summary>
            ///Culture name for Belarusian - Belarus.
            ///</summary>
            ///<remarks>Culture ID: 0x0423
            ///Culture Name: be-BY</remarks>
            public const string Belarusian_Belarus = "be-BY";
            ///<summary>
            ///Culture name for Bulgarian.
            ///</summary>
            ///<remarks>Culture ID: 0x0002
            ///Culture Name: bg</remarks>
            public const string Bulgarian = "bg";
            ///<summary>
            ///Culture name for Bulgarian - Bulgaria.
            ///</summary>
            ///<remarks>Culture ID: 0x0402
            ///Culture Name: bg-BG</remarks>
            public const string Bulgarian_Bulgaria = "bg-BG";
            ///<summary>
            ///Culture name for Catalan.
            ///</summary>
            ///<remarks>Culture ID: 0x0003
            ///Culture Name: ca</remarks>
            public const string Catalan = "ca";
            ///<summary>
            ///Culture name for Catalan - Catalan.
            ///</summary>
            ///<remarks>Culture ID: 0x0403
            ///Culture Name: ca-ES</remarks>
            public const string Catalan_Catalan = "ca-ES";
            ///<summary>
            ///Culture name for Chinese - Hong Kong SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x0C04
            ///Culture Name: zh-HK</remarks>
            public const string Chinese_HongKongSAR = "zh-HK";
            ///<summary>
            ///Culture name for Chinese - Macao SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x1404
            ///Culture Name: zh-MO</remarks>
            public const string Chinese_MacaoSAR = "zh-MO";
            ///<summary>
            ///Culture name for Chinese - China.
            ///</summary>
            ///<remarks>Culture ID: 0x0804
            ///Culture Name: zh-CN</remarks>
            public const string Chinese_China = "zh-CN";
            ///<summary>
            ///Culture name for Chinese (Simplified).
            ///</summary>
            ///<remarks>Culture ID: 0x0004
            ///Culture Name: zh-CHS</remarks>
            public const string Chinese_Simplified = "zh-CHS";
            ///<summary>
            ///Culture name for Chinese - Singapore.
            ///</summary>
            ///<remarks>Culture ID: 0x1004
            ///Culture Name: zh-SG</remarks>
            public const string Chinese_Singapore = "zh-SG";
            ///<summary>
            ///Culture name for Chinese - Taiwan.
            ///</summary>
            ///<remarks>Culture ID: 0x0404
            ///Culture Name: zh-TW</remarks>
            public const string Chinese_Taiwan = "zh-TW";
            ///<summary>
            ///Culture name for Chinese (Traditional).
            ///</summary>
            ///<remarks>Culture ID: 0x7C04
            ///Culture Name: zh-CHT</remarks>
            public const string Chinese_Traditional = "zh-CHT";
            ///<summary>
            ///Culture name for Croatian.
            ///</summary>
            ///<remarks>Culture ID: 0x001A
            ///Culture Name: hr</remarks>
            public const string Croatian = "hr";
            ///<summary>
            ///Culture name for Croatian - Croatia.
            ///</summary>
            ///<remarks>Culture ID: 0x041A
            ///Culture Name: hr-HR</remarks>
            public const string Croatian_Croatia = "hr-HR";
            ///<summary>
            ///Culture name for Czech.
            ///</summary>
            ///<remarks>Culture ID: 0x0005
            ///Culture Name: cs</remarks>
            public const string Czech = "cs";
            ///<summary>
            ///Culture name for Czech - Czech Republic.
            ///</summary>
            ///<remarks>Culture ID: 0x0405
            ///Culture Name: cs-CZ</remarks>
            public const string Czech_CzechRepublic = "cs-CZ";
            ///<summary>
            ///Culture name for Danish.
            ///</summary>
            ///<remarks>Culture ID: 0x0006
            ///Culture Name: da</remarks>
            public const string Danish = "da";
            ///<summary>
            ///Culture name for Danish - Denmark.
            ///</summary>
            ///<remarks>Culture ID: 0x0406
            ///Culture Name: da-DK</remarks>
            public const string Danish_Denmark = "da-DK";
            ///<summary>
            ///Culture name for Dhivehi.
            ///</summary>
            ///<remarks>Culture ID: 0x0065
            ///Culture Name: div</remarks>
            public const string Dhivehi = "div";
            ///<summary>
            ///Culture name for Dhivehi - Maldives.
            ///</summary>
            ///<remarks>Culture ID: 0x0465
            ///Culture Name: div-MV</remarks>
            public const string Dhivehi_Maldives = "div-MV";
            ///<summary>
            ///Culture name for Dutch.
            ///</summary>
            ///<remarks>Culture ID: 0x0013
            ///Culture Name: nl</remarks>
            public const string Dutch = "nl";
            ///<summary>
            ///Culture name for Dutch - Belgium.
            ///</summary>
            ///<remarks>Culture ID: 0x0813
            ///Culture Name: nl-BE</remarks>
            public const string Dutch_Belgium = "nl-BE";
            ///<summary>
            ///Culture name for Dutch - The Netherlands.
            ///</summary>
            ///<remarks>Culture ID: 0x0413
            ///Culture Name: nl-NL</remarks>
            public const string Dutch_TheNetherlands = "nl-NL";
            ///<summary>
            ///Culture name for English.
            ///</summary>
            ///<remarks>Culture ID: 0x0009
            ///Culture Name: en</remarks>
            public const string English = "en";
            ///<summary>
            ///Culture name for English - Australia.
            ///</summary>
            ///<remarks>Culture ID: 0x0C09
            ///Culture Name: en-AU</remarks>
            public const string English_Australia = "en-AU";
            ///<summary>
            ///Culture name for English - Belize.
            ///</summary>
            ///<remarks>Culture ID: 0x2809
            ///Culture Name: en-BZ</remarks>
            public const string English_Belize = "en-BZ";
            ///<summary>
            ///Culture name for English - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x1009
            ///Culture Name: en-CA</remarks>
            public const string English_Canada = "en-CA";
            ///<summary>
            ///Culture name for English - Caribbean.
            ///</summary>
            ///<remarks>Culture ID: 0x2409
            ///Culture Name: en-CB</remarks>
            public const string English_Caribbean = "en-CB";
            ///<summary>
            ///Culture name for English - Ireland.
            ///</summary>
            ///<remarks>Culture ID: 0x1809
            ///Culture Name: en-IE</remarks>
            public const string English_Ireland = "en-IE";
            ///<summary>
            ///Culture name for English - Jamaica.
            ///</summary>
            ///<remarks>Culture ID: 0x2009
            ///Culture Name: en-JM</remarks>
            public const string English_Jamaica = "en-JM";
            ///<summary>
            ///Culture name for English - New Zealand.
            ///</summary>
            ///<remarks>Culture ID: 0x1409
            ///Culture Name: en-NZ</remarks>
            public const string English_NewZealand = "en-NZ";
            ///<summary>
            ///Culture name for English - Philippines.
            ///</summary>
            ///<remarks>Culture ID: 0x3409
            ///Culture Name: en-PH</remarks>
            public const string English_Philippines = "en-PH";
            ///<summary>
            ///Culture name for English - South Africa.
            ///</summary>
            ///<remarks>Culture ID: 0x1C09
            ///Culture Name: en-ZA</remarks>
            public const string English_SouthAfrica = "en-ZA";
            ///<summary>
            ///Culture name for English - Trinidad and Tobago.
            ///</summary>
            ///<remarks>Culture ID: 0x2C09
            ///Culture Name: en-TT</remarks>
            public const string English_TrinidadAndTobago = "en-TT";
            ///<summary>
            ///Culture name for English - United Kingdom.
            ///</summary>
            ///<remarks>Culture ID: 0x0809
            ///Culture Name: en-GB</remarks>
            public const string English_UnitedKingdom = "en-GB";
            ///<summary>
            ///Culture name for English - United States.
            ///</summary>
            ///<remarks>Culture ID: 0x0409
            ///Culture Name: en-US</remarks>
            public const string English_UnitedStates = "en-US";
            ///<summary>
            ///Culture name for English - Zimbabwe.
            ///</summary>
            ///<remarks>Culture ID: 0x3009
            ///Culture Name: en-ZW</remarks>
            public const string English_Zimbabwe = "en-ZW";
            ///<summary>
            ///Culture name for Estonian.
            ///</summary>
            ///<remarks>Culture ID: 0x0025
            ///Culture Name: et</remarks>
            public const string Estonian = "et";
            ///<summary>
            ///Culture name for Estonian - Estonia.
            ///</summary>
            ///<remarks>Culture ID: 0x0425
            ///Culture Name: et-EE</remarks>
            public const string Estonian_Estonia = "et-EE";
            ///<summary>
            ///Culture name for Faroese.
            ///</summary>
            ///<remarks>Culture ID: 0x0038
            ///Culture Name: fo</remarks>
            public const string Faroese = "fo";
            ///<summary>
            ///Culture name for Faroese - Faroe Islands.
            ///</summary>
            ///<remarks>Culture ID: 0x0438
            ///Culture Name: fo-FO</remarks>
            public const string Faroese_FaroeIslands = "fo-FO";
            ///<summary>
            ///Culture name for Farsi.
            ///</summary>
            ///<remarks>Culture ID: 0x0029
            ///Culture Name: fa</remarks>
            public const string Farsi = "fa";
            ///<summary>
            ///Culture name for Farsi - Iran.
            ///</summary>
            ///<remarks>Culture ID: 0x0429
            ///Culture Name: fa-IR</remarks>
            public const string Farsi_Iran = "fa-IR";
            ///<summary>
            ///Culture name for Finnish.
            ///</summary>
            ///<remarks>Culture ID: 0x000B
            ///Culture Name: fi</remarks>
            public const string Finnish = "fi";
            ///<summary>
            ///Culture name for Finnish - Finland.
            ///</summary>
            ///<remarks>Culture ID: 0x040B
            ///Culture Name: fi-FI</remarks>
            public const string Finnish_Finland = "fi-FI";
            ///<summary>
            ///Culture name for French.
            ///</summary>
            ///<remarks>Culture ID: 0x000C
            ///Culture Name: fr</remarks>
            public const string French = "fr";
            ///<summary>
            ///Culture name for French - Belgium.
            ///</summary>
            ///<remarks>Culture ID: 0x080C
            ///Culture Name: fr-BE</remarks>
            public const string French_Belgium = "fr-BE";
            ///<summary>
            ///Culture name for French - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0C
            ///Culture Name: fr-CA</remarks>
            public const string French_Canada = "fr-CA";
            ///<summary>
            ///Culture name for French - France.
            ///</summary>
            ///<remarks>Culture ID: 0x040C
            ///Culture Name: fr-FR</remarks>
            public const string French_France = "fr-FR";
            ///<summary>
            ///Culture name for French - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x140C
            ///Culture Name: fr-LU</remarks>
            public const string French_Luxembourg = "fr-LU";
            ///<summary>
            ///Culture name for French - Monaco.
            ///</summary>
            ///<remarks>Culture ID: 0x180C
            ///Culture Name: fr-MC</remarks>
            public const string French_Monaco = "fr-MC";
            ///<summary>
            ///Culture name for French - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x100C
            ///Culture Name: fr-CH</remarks>
            public const string French_Switzerland = "fr-CH";
            ///<summary>
            ///Culture name for Galician.
            ///</summary>
            ///<remarks>Culture ID: 0x0056
            ///Culture Name: gl</remarks>
            public const string Galician = "gl";
            ///<summary>
            ///Culture name for Galician - Galician.
            ///</summary>
            ///<remarks>Culture ID: 0x0456
            ///Culture Name: gl-ES</remarks>
            public const string Galician_Galician = "gl-ES";
            ///<summary>
            ///Culture name for Georgian.
            ///</summary>
            ///<remarks>Culture ID: 0x0037
            ///Culture Name: ka</remarks>
            public const string Georgian = "ka";
            ///<summary>
            ///Culture name for Georgian - Georgia.
            ///</summary>
            ///<remarks>Culture ID: 0x0437
            ///Culture Name: ka-GE</remarks>
            public const string Georgian_Georgia = "ka-GE";
            ///<summary>
            ///Culture name for German.
            ///</summary>
            ///<remarks>Culture ID: 0x0007
            ///Culture Name: de</remarks>
            public const string German = "de";
            ///<summary>
            ///Culture name for German - Austria.
            ///</summary>
            ///<remarks>Culture ID: 0x0C07
            ///Culture Name: de-AT</remarks>
            public const string German_Austria = "de-AT";
            ///<summary>
            ///Culture name for German - Germany.
            ///</summary>
            ///<remarks>Culture ID: 0x0407
            ///Culture Name: de-DE</remarks>
            public const string German_Germany = "de-DE";
            ///<summary>
            ///Culture name for German - Liechtenstein.
            ///</summary>
            ///<remarks>Culture ID: 0x1407
            ///Culture Name: de-LI</remarks>
            public const string German_Liechtenstein = "de-LI";
            ///<summary>
            ///Culture name for German - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x1007
            ///Culture Name: de-LU</remarks>
            public const string German_Luxembourg = "de-LU";
            ///<summary>
            ///Culture name for German - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0807
            ///Culture Name: de-CH</remarks>
            public const string German_Switzerland = "de-CH";
            ///<summary>
            ///Culture name for Greek.
            ///</summary>
            ///<remarks>Culture ID: 0x0008
            ///Culture Name: el</remarks>
            public const string Greek = "el";
            ///<summary>
            ///Culture name for Greek - Greece.
            ///</summary>
            ///<remarks>Culture ID: 0x0408
            ///Culture Name: el-GR</remarks>
            public const string Greek_Greece = "el-GR";
            ///<summary>
            ///Culture name for Gujarati.
            ///</summary>
            ///<remarks>Culture ID: 0x0047
            ///Culture Name: gu</remarks>
            public const string Gujarati = "gu";
            ///<summary>
            ///Culture name for Gujarati - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0447
            ///Culture Name: gu-IN</remarks>
            public const string Gujarati_India = "gu-IN";
            ///<summary>
            ///Culture name for Hebrew.
            ///</summary>
            ///<remarks>Culture ID: 0x000D
            ///Culture Name: he</remarks>
            public const string Hebrew = "he";
            ///<summary>
            ///Culture name for Hebrew - Israel.
            ///</summary>
            ///<remarks>Culture ID: 0x040D
            ///Culture Name: he-IL</remarks>
            public const string Hebrew_Israel = "he-IL";
            ///<summary>
            ///Culture name for Hindi.
            ///</summary>
            ///<remarks>Culture ID: 0x0039
            ///Culture Name: hi</remarks>
            public const string Hindi = "hi";
            ///<summary>
            ///Culture name for Hindi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0439
            ///Culture Name: hi-IN</remarks>
            public const string Hindi_India = "hi-IN";
            ///<summary>
            ///Culture name for Hungarian.
            ///</summary>
            ///<remarks>Culture ID: 0x000E
            ///Culture Name: hu</remarks>
            public const string Hungarian = "hu";
            ///<summary>
            ///Culture name for Hungarian - Hungary.
            ///</summary>
            ///<remarks>Culture ID: 0x040E
            ///Culture Name: hu-HU</remarks>
            public const string Hungarian_Hungary = "hu-HU";
            ///<summary>
            ///Culture name for Icelandic.
            ///</summary>
            ///<remarks>Culture ID: 0x000F
            ///Culture Name: is</remarks>
            public const string Icelandic = "is";
            ///<summary>
            ///Culture name for Icelandic - Iceland.
            ///</summary>
            ///<remarks>Culture ID: 0x040F
            ///Culture Name: is-IS</remarks>
            public const string Icelandic_Iceland = "is-IS";
            ///<summary>
            ///Culture name for Indonesian.
            ///</summary>
            ///<remarks>Culture ID: 0x0021
            ///Culture Name: id</remarks>
            public const string Indonesian = "id";
            ///<summary>
            ///Culture name for Indonesian - Indonesia.
            ///</summary>
            ///<remarks>Culture ID: 0x0421
            ///Culture Name: id-ID</remarks>
            public const string Indonesian_Indonesia = "id-ID";
            ///<summary>
            ///Culture name for Italian.
            ///</summary>
            ///<remarks>Culture ID: 0x0010
            ///Culture Name: it</remarks>
            public const string Italian = "it";
            ///<summary>
            ///Culture name for Italian - Italy.
            ///</summary>
            ///<remarks>Culture ID: 0x0410
            ///Culture Name: it-IT</remarks>
            public const string Italian_Italy = "it-IT";
            ///<summary>
            ///Culture name for Italian - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0810
            ///Culture Name: it-CH</remarks>
            public const string Italian_Switzerland = "it-CH";
            ///<summary>
            ///Culture name for Japanese.
            ///</summary>
            ///<remarks>Culture ID: 0x0011
            ///Culture Name: ja</remarks>
            public const string Japanese = "ja";
            ///<summary>
            ///Culture name for Japanese - Japan.
            ///</summary>
            ///<remarks>Culture ID: 0x0411
            ///Culture Name: ja-JP</remarks>
            public const string Japanese_Japan = "ja-JP";
            ///<summary>
            ///Culture name for Kannada.
            ///</summary>
            ///<remarks>Culture ID: 0x004B
            ///Culture Name: kn</remarks>
            public const string Kannada = "kn";
            ///<summary>
            ///Culture name for Kannada - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044B
            ///Culture Name: kn-IN</remarks>
            public const string Kannada_India = "kn-IN";
            ///<summary>
            ///Culture name for Kazakh.
            ///</summary>
            ///<remarks>Culture ID: 0x003F
            ///Culture Name: kk</remarks>
            public const string Kazakh = "kk";
            ///<summary>
            ///Culture name for Kazakh - Kazakhstan.
            ///</summary>
            ///<remarks>Culture ID: 0x043F
            ///Culture Name: kk-KZ</remarks>
            public const string Kazakh_Kazakhstan = "kk-KZ";
            ///<summary>
            ///Culture name for Konkani.
            ///</summary>
            ///<remarks>Culture ID: 0x0057
            ///Culture Name: kok</remarks>
            public const string Konkani = "kok";
            ///<summary>
            ///Culture name for Konkani - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0457
            ///Culture Name: kok-IN</remarks>
            public const string Konkani_India = "kok-IN";
            ///<summary>
            ///Culture name for Korean.
            ///</summary>
            ///<remarks>Culture ID: 0x0012
            ///Culture Name: ko</remarks>
            public const string Korean = "ko";
            ///<summary>
            ///Culture name for Korean - Korea.
            ///</summary>
            ///<remarks>Culture ID: 0x0412
            ///Culture Name: ko-KR</remarks>
            public const string Korean_Korea = "ko-KR";
            ///<summary>
            ///Culture name for Kyrgyz.
            ///</summary>
            ///<remarks>Culture ID: 0x0040
            ///Culture Name: ky</remarks>
            public const string Kyrgyz = "ky";
            ///<summary>
            ///Culture name for Kyrgyz - Kyrgyzstan.
            ///</summary>
            ///<remarks>Culture ID: 0x0440
            ///Culture Name: ky-KG</remarks>
            public const string Kyrgyz_Kyrgyzstan = "ky-KG";
            ///<summary>
            ///Culture name for Latvian.
            ///</summary>
            ///<remarks>Culture ID: 0x0026
            ///Culture Name: lv</remarks>
            public const string Latvian = "lv";
            ///<summary>
            ///Culture name for Latvian - Latvia.
            ///</summary>
            ///<remarks>Culture ID: 0x0426
            ///Culture Name: lv-LV</remarks>
            public const string Latvian_Latvia = "lv-LV";
            ///<summary>
            ///Culture name for Lithuanian.
            ///</summary>
            ///<remarks>Culture ID: 0x0027
            ///Culture Name: lt</remarks>
            public const string Lithuanian = "lt";
            ///<summary>
            ///Culture name for Lithuanian - Lithuania.
            ///</summary>
            ///<remarks>Culture ID: 0x0427
            ///Culture Name: lt-LT</remarks>
            public const string Lithuanian_Lithuania = "lt-LT";
            ///<summary>
            ///Culture name for Macedonian.
            ///</summary>
            ///<remarks>Culture ID: 0x002F
            ///Culture Name: mk</remarks>
            public const string Macedonian = "mk";
            ///<summary>
            ///Culture name for Macedonian - Former Yugoslav Republic of Macedonia.
            ///</summary>
            ///<remarks>Culture ID: 0x042F
            ///Culture Name: mk-MK</remarks>
            public const string Macedonian_FormerYugoslavRepublicOfMacedonia = "mk-MK";
            ///<summary>
            ///Culture name for Malay.
            ///</summary>
            ///<remarks>Culture ID: 0x003E
            ///Culture Name: ms</remarks>
            public const string Malay = "ms";
            ///<summary>
            ///Culture name for Malay - Brunei.
            ///</summary>
            ///<remarks>Culture ID: 0x083E
            ///Culture Name: ms-BN</remarks>
            public const string Malay_Brunei = "ms-BN";
            ///<summary>
            ///Culture name for Malay - Malaysia.
            ///</summary>
            ///<remarks>Culture ID: 0x043E
            ///Culture Name: ms-MY</remarks>
            public const string Malay_Malaysia = "ms-MY";
            ///<summary>
            ///Culture name for Marathi.
            ///</summary>
            ///<remarks>Culture ID: 0x004E
            ///Culture Name: mr</remarks>
            public const string Marathi = "mr";
            ///<summary>
            ///Culture name for Marathi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044E
            ///Culture Name: mr-IN</remarks>
            public const string Marathi_India = "mr-IN";
            ///<summary>
            ///Culture name for Mongolian.
            ///</summary>
            ///<remarks>Culture ID: 0x0050
            ///Culture Name: mn</remarks>
            public const string Mongolian = "mn";
            ///<summary>
            ///Culture name for Mongolian - Mongolia.
            ///</summary>
            ///<remarks>Culture ID: 0x0450
            ///Culture Name: mn-MN</remarks>
            public const string Mongolian_Mongolia = "mn-MN";
            ///<summary>
            ///Culture name for Norwegian.
            ///</summary>
            ///<remarks>Culture ID: 0x0014
            ///Culture Name: no</remarks>
            public const string Norwegian = "no";
            /// <summary>
            /// Culture name for Norwegian (Bokmål) - Norway
            /// </summary>
            /// <remarks>Culture ID: 0x0414
            /// Culture Name: nb-NO</remarks>
            public const string Norwegian_Bokmål_Norway = "nb-NO";
            ///<summary>
            ///Culture name for Norwegian (Nynorsk) - Norway.
            ///</summary>
            ///<remarks>Culture ID: 0x0814
            ///Culture Name: nn-NO</remarks>
            public const string Norwegian_Nynorsk_Norway = "nn-NO";
            ///<summary>
            ///Culture name for Polish.
            ///</summary>
            ///<remarks>Culture ID: 0x0015
            ///Culture Name: pl</remarks>
            public const string Polish = "pl";
            ///<summary>
            ///Culture name for Polish - Poland.
            ///</summary>
            ///<remarks>Culture ID: 0x0415
            ///Culture Name: pl-PL</remarks>
            public const string Polish_Poland = "pl-PL";
            ///<summary>
            ///Culture name for Portuguese.
            ///</summary>
            ///<remarks>Culture ID: 0x0016
            ///Culture Name: pt</remarks>
            public const string Portuguese = "pt";
            ///<summary>
            ///Culture name for Portuguese - Brazil.
            ///</summary>
            ///<remarks>Culture ID: 0x0416
            ///Culture Name: pt-BR</remarks>
            public const string Portuguese_Brazil = "pt-BR";
            ///<summary>
            ///Culture name for Portuguese - Portugal.
            ///</summary>
            ///<remarks>Culture ID: 0x0816
            ///Culture Name: pt-PT</remarks>
            public const string Portuguese_Portugal = "pt-PT";
            ///<summary>
            ///Culture name for Punjabi.
            ///</summary>
            ///<remarks>Culture ID: 0x0046
            ///Culture Name: pa</remarks>
            public const string Punjabi = "pa";
            ///<summary>
            ///Culture name for Punjabi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0446
            ///Culture Name: pa-IN</remarks>
            public const string Punjabi_India = "pa-IN";
            ///<summary>
            ///Culture name for Romanian.
            ///</summary>
            ///<remarks>Culture ID: 0x0018
            ///Culture Name: ro</remarks>
            public const string Romanian = "ro";
            ///<summary>
            ///Culture name for Romanian - Romania.
            ///</summary>
            ///<remarks>Culture ID: 0x0418
            ///Culture Name: ro-RO</remarks>
            public const string Romanian_Romania = "ro-RO";
            ///<summary>
            ///Culture name for Russian.
            ///</summary>
            ///<remarks>Culture ID: 0x0019
            ///Culture Name: ru</remarks>
            public const string Russian = "ru";
            ///<summary>
            ///Culture name for Russian - Russia.
            ///</summary>
            ///<remarks>Culture ID: 0x0419
            ///Culture Name: ru-RU</remarks>
            public const string Russian_Russia = "ru-RU";
            ///<summary>
            ///Culture name for Sanskrit.
            ///</summary>
            ///<remarks>Culture ID: 0x004F
            ///Culture Name: sa</remarks>
            public const string Sanskrit = "sa";
            ///<summary>
            ///Culture name for Sanskrit - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044F
            ///Culture Name: sa-IN</remarks>
            public const string Sanskrit_India = "sa-IN";
            ///<summary>
            ///Culture name for Serbian (Cyrillic) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x0C1A
            ///Culture Name: sr-SP-Cyrl</remarks>
            public const string Serbian_Cyrillic_Serbia = "sr-SP-Cyrl";
            ///<summary>
            ///Culture name for Serbian (Latin) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x081A
            ///Culture Name: sr-SP-Latn</remarks>
            public const string Serbian_Latin_Serbia = "sr-SP-Latn";
            ///<summary>
            ///Culture name for Slovak.
            ///</summary>
            ///<remarks>Culture ID: 0x001B
            ///Culture Name: sk</remarks>
            public const string Slovak = "sk";
            ///<summary>
            ///Culture name for Slovak - Slovakia.
            ///</summary>
            ///<remarks>Culture ID: 0x041B
            ///Culture Name: sk-SK</remarks>
            public const string Slovak_Slovakia = "sk-SK";
            ///<summary>
            ///Culture name for Slovenian.
            ///</summary>
            ///<remarks>Culture ID: 0x0024
            ///Culture Name: sl</remarks>
            public const string Slovenian = "sl";
            ///<summary>
            ///Culture name for Slovenian - Slovenia.
            ///</summary>
            ///<remarks>Culture ID: 0x0424
            ///Culture Name: sl-SI</remarks>
            public const string Slovenian_Slovenia = "sl-SI";
            ///<summary>
            ///Culture name for Spanish.
            ///</summary>
            ///<remarks>Culture ID: 0x000A
            ///Culture Name: es</remarks>
            public const string Spanish = "es";
            ///<summary>
            ///Culture name for Spanish - Argentina.
            ///</summary>
            ///<remarks>Culture ID: 0x2C0A
            ///Culture Name: es-AR</remarks>
            public const string Spanish_Argentina = "es-AR";
            ///<summary>
            ///Culture name for Spanish - Bolivia.
            ///</summary>
            ///<remarks>Culture ID: 0x400A
            ///Culture Name: es-BO</remarks>
            public const string Spanish_Bolivia = "es-BO";
            ///<summary>
            ///Culture name for Spanish - Chile.
            ///</summary>
            ///<remarks>Culture ID: 0x340A
            ///Culture Name: es-CL</remarks>
            public const string Spanish_Chile = "es-CL";
            ///<summary>
            ///Culture name for Spanish - Colombia.
            ///</summary>
            ///<remarks>Culture ID: 0x240A
            ///Culture Name: es-CO</remarks>
            public const string Spanish_Colombia = "es-CO";
            ///<summary>
            ///Culture name for Spanish - Costa Rica.
            ///</summary>
            ///<remarks>Culture ID: 0x140A
            ///Culture Name: es-CR</remarks>
            public const string Spanish_CostaRica = "es-CR";
            ///<summary>
            ///Culture name for Spanish - Dominican Republic.
            ///</summary>
            ///<remarks>Culture ID: 0x1C0A
            ///Culture Name: es-DO</remarks>
            public const string Spanish_DominicanRepublic = "es-DO";
            ///<summary>
            ///Culture name for Spanish - Ecuador.
            ///</summary>
            ///<remarks>Culture ID: 0x300A
            ///Culture Name: es-EC</remarks>
            public const string Spanish_Ecuador = "es-EC";
            ///<summary>
            ///Culture name for Spanish - El Salvador.
            ///</summary>
            ///<remarks>Culture ID: 0x440A
            ///Culture Name: es-SV</remarks>
            public const string Spanish_ElSalvador = "es-SV";
            ///<summary>
            ///Culture name for Spanish - Guatemala.
            ///</summary>
            ///<remarks>Culture ID: 0x100A
            ///Culture Name: es-GT</remarks>
            public const string Spanish_Guatemala = "es-GT";
            ///<summary>
            ///Culture name for Spanish - Honduras.
            ///</summary>
            ///<remarks>Culture ID: 0x480A
            ///Culture Name: es-HN</remarks>
            public const string Spanish_Honduras = "es-HN";
            ///<summary>
            ///Culture name for Spanish - Mexico.
            ///</summary>
            ///<remarks>Culture ID: 0x080A
            ///Culture Name: es-MX</remarks>
            public const string Spanish_Mexico = "es-MX";
            ///<summary>
            ///Culture name for Spanish - Nicaragua.
            ///</summary>
            ///<remarks>Culture ID: 0x4C0A
            ///Culture Name: es-NI</remarks>
            public const string Spanish_Nicaragua = "es-NI";
            ///<summary>
            ///Culture name for Spanish - Panama.
            ///</summary>
            ///<remarks>Culture ID: 0x180A
            ///Culture Name: es-PA</remarks>
            public const string Spanish_Panama = "es-PA";
            ///<summary>
            ///Culture name for Spanish - Paraguay.
            ///</summary>
            ///<remarks>Culture ID: 0x3C0A
            ///Culture Name: es-PY</remarks>
            public const string Spanish_Paraguay = "es-PY";
            ///<summary>
            ///Culture name for Spanish - Peru.
            ///</summary>
            ///<remarks>Culture ID: 0x280A
            ///Culture Name: es-PE</remarks>
            public const string Spanish_Peru = "es-PE";
            ///<summary>
            ///Culture name for Spanish - Puerto Rico.
            ///</summary>
            ///<remarks>Culture ID: 0x500A
            ///Culture Name: es-PR</remarks>
            public const string Spanish_PuertoRico = "es-PR";
            ///<summary>
            ///Culture name for Spanish - Spain.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0A
            ///Culture Name: es-ES</remarks>
            public const string Spanish_Spain = "es-ES";
            ///<summary>
            ///Culture name for Spanish - Uruguay.
            ///</summary>
            ///<remarks>Culture ID: 0x380A
            ///Culture Name: es-UY</remarks>
            public const string Spanish_Uruguay = "es-UY";
            ///<summary>
            ///Culture name for Spanish - Venezuela.
            ///</summary>
            ///<remarks>Culture ID: 0x200A
            ///Culture Name: es-VE</remarks>
            public const string Spanish_Venezuela = "es-VE";
            ///<summary>
            ///Culture name for Swahili.
            ///</summary>
            ///<remarks>Culture ID: 0x0041
            ///Culture Name: sw</remarks>
            public const string Swahili = "sw";
            ///<summary>
            ///Culture name for Swahili - Kenya.
            ///</summary>
            ///<remarks>Culture ID: 0x0441
            ///Culture Name: sw-KE</remarks>
            public const string Swahili_Kenya = "sw-KE";
            ///<summary>
            ///Culture name for Swedish.
            ///</summary>
            ///<remarks>Culture ID: 0x001D
            ///Culture Name: sv</remarks>
            public const string Swedish = "sv";
            ///<summary>
            ///Culture name for Swedish - Finland.
            ///</summary>
            ///<remarks>Culture ID: 0x081D
            ///Culture Name: sv-FI</remarks>
            public const string Swedish_Finland = "sv-FI";
            ///<summary>
            ///Culture name for Swedish - Sweden.
            ///</summary>
            ///<remarks>Culture ID: 0x041D
            ///Culture Name: sv-SE</remarks>
            public const string Swedish_Sweden = "sv-SE";
            ///<summary>
            ///Culture name for Syriac.
            ///</summary>
            ///<remarks>Culture ID: 0x005A
            ///Culture Name: syr</remarks>
            public const string Syriac = "syr";
            ///<summary>
            ///Culture name for Syriac - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x045A
            ///Culture Name: syr-SY</remarks>
            public const string Syriac_Syria = "syr-SY";
            ///<summary>
            ///Culture name for Tamil.
            ///</summary>
            ///<remarks>Culture ID: 0x0049
            ///Culture Name: ta</remarks>
            public const string Tamil = "ta";
            ///<summary>
            ///Culture name for Tamil - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0449
            ///Culture Name: ta-IN</remarks>
            public const string Tamil_India = "ta-IN";
            ///<summary>
            ///Culture name for Tatar.
            ///</summary>
            ///<remarks>Culture ID: 0x0044
            ///Culture Name: tt</remarks>
            public const string Tatar = "tt";
            ///<summary>
            ///Culture name for Tatar - Russia.
            ///</summary>
            ///<remarks>Culture ID: 0x0444
            ///Culture Name: tt-RU</remarks>
            public const string Tatar_Russia = "tt-RU";
            ///<summary>
            ///Culture name for Telugu.
            ///</summary>
            ///<remarks>Culture ID: 0x004A
            ///Culture Name: te</remarks>
            public const string Telugu = "te";
            ///<summary>
            ///Culture name for Telugu - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044A
            ///Culture Name: te-IN</remarks>
            public const string Telugu_India = "te-IN";
            ///<summary>
            ///Culture name for Thai.
            ///</summary>
            ///<remarks>Culture ID: 0x001E
            ///Culture Name: th</remarks>
            public const string Thai = "th";
            ///<summary>
            ///Culture name for Thai - Thailand.
            ///</summary>
            ///<remarks>Culture ID: 0x041E
            ///Culture Name: th-TH</remarks>
            public const string Thai_Thailand = "th-TH";
            ///<summary>
            ///Culture name for Turkish.
            ///</summary>
            ///<remarks>Culture ID: 0x001F
            ///Culture Name: tr</remarks>
            public const string Turkish = "tr";
            ///<summary>
            ///Culture name for Turkish - Turkey.
            ///</summary>
            ///<remarks>Culture ID: 0x041F
            ///Culture Name: tr-TR</remarks>
            public const string Turkish_Turkey = "tr-TR";
            ///<summary>
            ///Culture name for Ukrainian.
            ///</summary>
            ///<remarks>Culture ID: 0x0022
            ///Culture Name: uk</remarks>
            public const string Ukrainian = "uk";
            ///<summary>
            ///Culture name for Ukrainian - Ukraine.
            ///</summary>
            ///<remarks>Culture ID: 0x0422
            ///Culture Name: uk-UA</remarks>
            public const string Ukrainian_Ukraine = "uk-UA";
            ///<summary>
            ///Culture name for Urdu.
            ///</summary>
            ///<remarks>Culture ID: 0x0020
            ///Culture Name: ur</remarks>
            public const string Urdu = "ur";
            ///<summary>
            ///Culture name for Urdu - Pakistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0420
            ///Culture Name: ur-PK</remarks>
            public const string Urdu_Pakistan = "ur-PK";
            ///<summary>
            ///Culture name for Uzbek.
            ///</summary>
            ///<remarks>Culture ID: 0x0043
            ///Culture Name: uz</remarks>
            public const string Uzbek = "uz";
            ///<summary>
            ///Culture name for Uzbek (Cyrillic) - Uzbekistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0843
            ///Culture Name: uz-UZ-Cyrl</remarks>
            public const string Uzbek_Cyrillic_Uzbekistan = "uz-UZ-Cyrl";
            ///<summary>
            ///Culture name for Uzbek (Latin) - Uzbekistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0443
            ///Culture Name: uz-UZ-Latn</remarks>
            public const string Uzbek_Latin_Uzbekistan = "uz-UZ-Latn";
            ///<summary>
            ///Culture name for Vietnamese.
            ///</summary>
            ///<remarks>Culture ID: 0x002A
            ///Culture Name: vi</remarks>
            public const string Vietnamese = "vi";
            /// <summary>
            /// Culture name for Vietnamese - Vietnam.
            /// </summary>
            /// <remarks>Culture ID: 0x042A
            /// Culture Name: vi-VN</remarks>
            public const string Vietnamese_Vietnam = "vi-VN";
        }
        /// <summary>
        /// Provides a series of constant int values that relate to the culture information and the 
        /// numerical identifiers of the cultures.
        /// </summary>
        public static class IDs
        {
            ///<summary>
            ///Culture identifier constant for None.
            ///</summary>
            ///<remarks>Culture ID: 0x007F
            ///Culture Name: <see cref="System.String.Empty"/></remarks>
            public const int None = 0x007F;
            ///<summary>
            ///Culture identifier constant for Afrikaans.
            ///</summary>
            ///<remarks>Culture ID: 0x0036
            ///Culture Name:af</remarks>
            public const int Afrikaans = 0x0036;
            ///<summary>
            ///Culture identifier constant for Afrikaans - South Africa.
            ///</summary>
            ///<remarks>Culture ID: 0x0436
            ///Culture Name:af-ZA</remarks>
            public const int Afrikaans_SouthAfrica = 0x0436;
            ///<summary>
            ///Culture identifier constant for Albanian.
            ///</summary>
            ///<remarks>Culture ID: 0x001C
            ///Culture Name:sq</remarks>
            public const int Albanian = 0x001C;
            ///<summary>
            ///Culture identifier constant for Albanian - Albania.
            ///</summary>
            ///<remarks>Culture ID: 0x041C
            ///Culture Name:sq-AL</remarks>
            public const int Albanian_Albania = 0x041C;
            ///<summary>
            ///Culture identifier constant for Arabic.
            ///</summary>
            ///<remarks>Culture ID: 0x0001
            ///Culture Name:ar</remarks>
            public const int Arabic = 0x0001;
            ///<summary>
            ///Culture identifier constant for Arabic - Algeria.
            ///</summary>
            ///<remarks>Culture ID: 0x1401
            ///Culture Name:ar-DZ</remarks>
            public const int Arabic_Algeria = 0x1401;
            ///<summary>
            ///Culture identifier constant for Arabic - Bahrain.
            ///</summary>
            ///<remarks>Culture ID: 0x3C01
            ///Culture Name:ar-BH</remarks>
            public const int Arabic_Bahrain = 0x3C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Egypt.
            ///</summary>
            ///<remarks>Culture ID: 0x0C01
            ///Culture Name:ar-EG</remarks>
            public const int Arabic_Egypt = 0x0C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Iraq.
            ///</summary>
            ///<remarks>Culture ID: 0x0801
            ///Culture Name:ar-IQ</remarks>
            public const int Arabic_Iraq = 0x0801;
            ///<summary>
            ///Culture identifier constant for Arabic - Jordan.
            ///</summary>
            ///<remarks>Culture ID: 0x2C01
            ///Culture Name:ar-JO</remarks>
            public const int Arabic_Jordan = 0x2C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Kuwait.
            ///</summary>
            ///<remarks>Culture ID: 0x3401
            ///Culture Name:ar-KW</remarks>
            public const int Arabic_Kuwait = 0x3401;
            ///<summary>
            ///Culture identifier constant for Arabic - Lebanon.
            ///</summary>
            ///<remarks>Culture ID: 0x3001
            ///Culture Name:ar-LB</remarks>
            public const int Arabic_Lebanon = 0x3001;
            ///<summary>
            ///Culture identifier constant for Arabic - Libya.
            ///</summary>
            ///<remarks>Culture ID: 0x1001
            ///Culture Name:ar-LY</remarks>
            public const int Arabic_Libya = 0x1001;
            ///<summary>
            ///Culture identifier constant for Arabic - Morocco.
            ///</summary>
            ///<remarks>Culture ID: 0x1801
            ///Culture Name:ar-MA</remarks>
            public const int Arabic_Morocco = 0x1801;
            ///<summary>
            ///Culture identifier constant for Arabic - Oman.
            ///</summary>
            ///<remarks>Culture ID: 0x2001
            ///Culture Name:ar-OM</remarks>
            public const int Arabic_Oman = 0x2001;
            ///<summary>
            ///Culture identifier constant for Arabic - Qatar.
            ///</summary>
            ///<remarks>Culture ID: 0x4001
            ///Culture Name:ar-QA</remarks>
            public const int Arabic_Qatar = 0x4001;
            ///<summary>
            ///Culture identifier constant for Arabic - Saudi Arabia.
            ///</summary>
            ///<remarks>Culture ID: 0x0401
            ///Culture Name:ar-SA</remarks>
            public const int Arabic_SaudiArabia = 0x0401;
            ///<summary>
            ///Culture identifier constant for Arabic - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x2801
            ///Culture Name:ar-SY</remarks>
            public const int Arabic_Syria = 0x2801;
            ///<summary>
            ///Culture identifier constant for Arabic - Tunisia.
            ///</summary>
            ///<remarks>Culture ID: 0x1C01
            ///Culture Name:ar-TN</remarks>
            public const int Arabic_Tunisia = 0x1C01;
            ///<summary>
            ///Culture identifier constant for Arabic - United Arab Emirates.
            ///</summary>
            ///<remarks>Culture ID: 0x3801
            ///Culture Name:ar-AE</remarks>
            public const int Arabic_UnitedArabEmirates = 0x3801;
            ///<summary>
            ///Culture identifier constant for Arabic - Yemen.
            ///</summary>
            ///<remarks>Culture ID: 0x2401
            ///Culture Name:ar-YE</remarks>
            public const int Arabic_Yemen = 0x2401;
            ///<summary>
            ///Culture identifier constant for Armenian.
            ///</summary>
            ///<remarks>Culture ID: 0x002B
            ///Culture Name:hy</remarks>
            public const int Armenian = 0x002B;
            ///<summary>
            ///Culture identifier constant for Armenian - Armenia.
            ///</summary>
            ///<remarks>Culture ID: 0x042B
            ///Culture Name:hy-AM</remarks>
            public const int Armenian_Armenia = 0x042B;
            ///<summary>
            ///Culture identifier constant for Azeri.
            ///</summary>
            ///<remarks>Culture ID: 0x002C
            ///Culture Name:az</remarks>
            public const int Azeri = 0x002C;
            ///<summary>
            ///Culture identifier constant for Azeri (Cyrillic) - Azerbaijan.
            ///</summary>
            ///<remarks>Culture ID: 0x082C
            ///Culture Name:az-AZ-Cyrl</remarks>
            public const int Azeri_Cyrillic_Azerbaijan = 0x082C;
            ///<summary>
            ///Culture identifier constant for Azeri (Latin) - Azerbaijan.
            ///</summary>
            ///<remarks>Culture ID: 0x042C
            ///Culture Name:az-AZ-Latn</remarks>
            public const int Azeri_Latin_Azerbaijan = 0x042C;
            ///<summary>
            ///Culture identifier constant for Basque.
            ///</summary>
            ///<remarks>Culture ID: 0x002D
            ///Culture Name:eu</remarks>
            public const int Basque = 0x002D;
            ///<summary>
            ///Culture identifier constant for Basque - Basque.
            ///</summary>
            ///<remarks>Culture ID: 0x042D
            ///Culture Name:eu-ES</remarks>
            public const int Basque_Basque = 0x042D;
            ///<summary>
            ///Culture identifier constant for Belarusian.
            ///</summary>
            ///<remarks>Culture ID: 0x0023
            ///Culture Name:be</remarks>
            public const int Belarusian = 0x0023;
            ///<summary>
            ///Culture identifier constant for Belarusian - Belarus.
            ///</summary>
            ///<remarks>Culture ID: 0x0423
            ///Culture Name:be-BY</remarks>
            public const int Belarusian_Belarus = 0x0423;
            ///<summary>
            ///Culture identifier constant for Bulgarian.
            ///</summary>
            ///<remarks>Culture ID: 0x0002
            ///Culture Name:bg</remarks>
            public const int Bulgarian = 0x0002;
            ///<summary>
            ///Culture identifier constant for Bulgarian - Bulgaria.
            ///</summary>
            ///<remarks>Culture ID: 0x0402
            ///Culture Name:bg-BG</remarks>
            public const int Bulgarian_Bulgaria = 0x0402;
            ///<summary>
            ///Culture identifier constant for Catalan.
            ///</summary>
            ///<remarks>Culture ID: 0x0003
            ///Culture Name:ca</remarks>
            public const int Catalan = 0x0003;
            ///<summary>
            ///Culture identifier constant for Catalan - Catalan.
            ///</summary>
            ///<remarks>Culture ID: 0x0403
            ///Culture Name:ca-ES</remarks>
            public const int Catalan_Catalan = 0x0403;
            ///<summary>
            ///Culture identifier constant for Chinese - Hong Kong SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x0C04
            ///Culture Name:zh-HK</remarks>
            public const int Chinese_HongKongSAR = 0x0C04;
            ///<summary>
            ///Culture identifier constant for Chinese - Macao SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x1404
            ///Culture Name:zh-MO</remarks>
            public const int Chinese_MacaoSAR = 0x1404;
            ///<summary>
            ///Culture identifier constant for Chinese - China.
            ///</summary>
            ///<remarks>Culture ID: 0x0804
            ///Culture Name:zh-CN</remarks>
            public const int Chinese_China = 0x0804;
            ///<summary>
            ///Culture identifier constant for Chinese (Simplified).
            ///</summary>
            ///<remarks>Culture ID: 0x0004
            ///Culture Name:zh-CHS</remarks>
            public const int Chinese_Simplified = 0x0004;
            ///<summary>
            ///Culture identifier constant for Chinese - Singapore.
            ///</summary>
            ///<remarks>Culture ID: 0x1004
            ///Culture Name:zh-SG</remarks>
            public const int Chinese_Singapore = 0x1004;
            ///<summary>
            ///Culture identifier constant for Chinese - Taiwan.
            ///</summary>
            ///<remarks>Culture ID: 0x0404
            ///Culture Name:zh-TW</remarks>
            public const int Chinese_Taiwan = 0x0404;
            ///<summary>
            ///Culture identifier constant for Chinese (Traditional).
            ///</summary>
            ///<remarks>Culture ID: 0x7C04
            ///Culture Name:zh-CHT</remarks>
            public const int Chinese_Traditional = 0x7C04;
            ///<summary>
            ///Culture identifier constant for Croatian.
            ///</summary>
            ///<remarks>Culture ID: 0x001A
            ///Culture Name:hr</remarks>
            public const int Croatian = 0x001A;
            ///<summary>
            ///Culture identifier constant for Croatian - Croatia.
            ///</summary>
            ///<remarks>Culture ID: 0x041A
            ///Culture Name:hr-HR</remarks>
            public const int Croatian_Croatia = 0x041A;
            ///<summary>
            ///Culture identifier constant for Czech.
            ///</summary>
            ///<remarks>Culture ID: 0x0005
            ///Culture Name:cs</remarks>
            public const int Czech = 0x0005;
            ///<summary>
            ///Culture identifier constant for Czech - Czech Republic.
            ///</summary>
            ///<remarks>Culture ID: 0x0405
            ///Culture Name:cs-CZ</remarks>
            public const int Czech_CzechRepublic = 0x0405;
            ///<summary>
            ///Culture identifier constant for Danish.
            ///</summary>
            ///<remarks>Culture ID: 0x0006
            ///Culture Name:da</remarks>
            public const int Danish = 0x0006;
            ///<summary>
            ///Culture identifier constant for Danish - Denmark.
            ///</summary>
            ///<remarks>Culture ID: 0x0406
            ///Culture Name:da-DK</remarks>
            public const int Danish_Denmark = 0x0406;
            ///<summary>
            ///Culture identifier constant for Dhivehi.
            ///</summary>
            ///<remarks>Culture ID: 0x0065
            ///Culture Name:div</remarks>
            public const int Dhivehi = 0x0065;
            ///<summary>
            ///Culture identifier constant for Dhivehi - Maldives.
            ///</summary>
            ///<remarks>Culture ID: 0x0465
            ///Culture Name:div-MV</remarks>
            public const int Dhivehi_Maldives = 0x0465;
            ///<summary>
            ///Culture identifier constant for Dutch.
            ///</summary>
            ///<remarks>Culture ID: 0x0013
            ///Culture Name:nl</remarks>
            public const int Dutch = 0x0013;
            ///<summary>
            ///Culture identifier constant for Dutch - Belgium.
            ///</summary>
            ///<remarks>Culture ID: 0x0813
            ///Culture Name:nl-BE</remarks>
            public const int Dutch_Belgium = 0x0813;
            ///<summary>
            ///Culture identifier constant for Dutch - The Netherlands.
            ///</summary>
            ///<remarks>Culture ID: 0x0413
            ///Culture Name:nl-NL</remarks>
            public const int Dutch_TheNetherlands = 0x0413;
            ///<summary>
            ///Culture identifier constant for English.
            ///</summary>
            ///<remarks>Culture ID: 0x0009
            ///Culture Name:en</remarks>
            public const int English = 0x0009;
            ///<summary>
            ///Culture identifier constant for English - Australia.
            ///</summary>
            ///<remarks>Culture ID: 0x0C09
            ///Culture Name:en-AU</remarks>
            public const int English_Australia = 0x0C09;
            ///<summary>
            ///Culture identifier constant for English - Belize.
            ///</summary>
            ///<remarks>Culture ID: 0x2809
            ///Culture Name:en-BZ</remarks>
            public const int English_Belize = 0x2809;
            ///<summary>
            ///Culture identifier constant for English - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x1009
            ///Culture Name:en-CA</remarks>
            public const int English_Canada = 0x1009;
            ///<summary>
            ///Culture identifier constant for English - Caribbean.
            ///</summary>
            ///<remarks>Culture ID: 0x2409
            ///Culture Name:en-CB</remarks>
            public const int English_Caribbean = 0x2409;
            ///<summary>
            ///Culture identifier constant for English - Ireland.
            ///</summary>
            ///<remarks>Culture ID: 0x1809
            ///Culture Name:en-IE</remarks>
            public const int English_Ireland = 0x1809;
            ///<summary>
            ///Culture identifier constant for English - Jamaica.
            ///</summary>
            ///<remarks>Culture ID: 0x2009
            ///Culture Name:en-JM</remarks>
            public const int English_Jamaica = 0x2009;
            ///<summary>
            ///Culture identifier constant for English - New Zealand.
            ///</summary>
            ///<remarks>Culture ID: 0x1409
            ///Culture Name:en-NZ</remarks>
            public const int English_NewZealand = 0x1409;
            ///<summary>
            ///Culture identifier constant for English - Philippines.
            ///</summary>
            ///<remarks>Culture ID: 0x3409
            ///Culture Name:en-PH</remarks>
            public const int English_Philippines = 0x3409;
            ///<summary>
            ///Culture identifier constant for English - South Africa.
            ///</summary>
            ///<remarks>Culture ID: 0x1C09
            ///Culture Name:en-ZA</remarks>
            public const int English_SouthAfrica = 0x1C09;
            ///<summary>
            ///Culture identifier constant for English - Trinidad and Tobago.
            ///</summary>
            ///<remarks>Culture ID: 0x2C09
            ///Culture Name:en-TT</remarks>
            public const int English_TrinidadAndTobago = 0x2C09;
            ///<summary>
            ///Culture identifier constant for English - United Kingdom.
            ///</summary>
            ///<remarks>Culture ID: 0x0809
            ///Culture Name:en-GB</remarks>
            public const int English_UnitedKingdom = 0x0809;
            ///<summary>
            ///Culture identifier constant for English - United States.
            ///</summary>
            ///<remarks>Culture ID: 0x0409
            ///Culture Name:en-US</remarks>
            public const int English_UnitedStates = 0x0409;
            ///<summary>
            ///Culture identifier constant for English - Zimbabwe.
            ///</summary>
            ///<remarks>Culture ID: 0x3009
            ///Culture Name:en-ZW</remarks>
            public const int English_Zimbabwe = 0x3009;
            ///<summary>
            ///Culture identifier constant for Estonian.
            ///</summary>
            ///<remarks>Culture ID: 0x0025
            ///Culture Name:et</remarks>
            public const int Estonian = 0x0025;
            ///<summary>
            ///Culture identifier constant for Estonian - Estonia.
            ///</summary>
            ///<remarks>Culture ID: 0x0425
            ///Culture Name:et-EE</remarks>
            public const int Estonian_Estonia = 0x0425;
            ///<summary>
            ///Culture identifier constant for Faroese.
            ///</summary>
            ///<remarks>Culture ID: 0x0038
            ///Culture Name:fo</remarks>
            public const int Faroese = 0x0038;
            ///<summary>
            ///Culture identifier constant for Faroese - Faroe Islands.
            ///</summary>
            ///<remarks>Culture ID: 0x0438
            ///Culture Name:fo-FO</remarks>
            public const int Faroese_FaroeIslands = 0x0438;
            ///<summary>
            ///Culture identifier constant for Farsi.
            ///</summary>
            ///<remarks>Culture ID: 0x0029
            ///Culture Name:fa</remarks>
            public const int Farsi = 0x0029;
            ///<summary>
            ///Culture identifier constant for Farsi - Iran.
            ///</summary>
            ///<remarks>Culture ID: 0x0429
            ///Culture Name:fa-IR</remarks>
            public const int Farsi_Iran = 0x0429;
            ///<summary>
            ///Culture identifier constant for Finnish.
            ///</summary>
            ///<remarks>Culture ID: 0x000B
            ///Culture Name:fi</remarks>
            public const int Finnish = 0x000B;
            ///<summary>
            ///Culture identifier constant for Finnish - Finland.
            ///</summary>
            ///<remarks>Culture ID: 0x040B
            ///Culture Name:fi-FI</remarks>
            public const int Finnish_Finland = 0x040B;
            ///<summary>
            ///Culture identifier constant for French.
            ///</summary>
            ///<remarks>Culture ID: 0x000C
            ///Culture Name:fr</remarks>
            public const int French = 0x000C;
            ///<summary>
            ///Culture identifier constant for French - Belgium.
            ///</summary>
            ///<remarks>Culture ID: 0x080C
            ///Culture Name:fr-BE</remarks>
            public const int French_Belgium = 0x080C;
            ///<summary>
            ///Culture identifier constant for French - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0C
            ///Culture Name:fr-CA</remarks>
            public const int French_Canada = 0x0C0C;
            ///<summary>
            ///Culture identifier constant for French - France.
            ///</summary>
            ///<remarks>Culture ID: 0x040C
            ///Culture Name:fr-FR</remarks>
            public const int French_France = 0x040C;
            ///<summary>
            ///Culture identifier constant for French - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x140C
            ///Culture Name:fr-LU</remarks>
            public const int French_Luxembourg = 0x140C;
            ///<summary>
            ///Culture identifier constant for French - Monaco.
            ///</summary>
            ///<remarks>Culture ID: 0x180C
            ///Culture Name:fr-MC</remarks>
            public const int French_Monaco = 0x180C;
            ///<summary>
            ///Culture identifier constant for French - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x100C
            ///Culture Name:fr-CH</remarks>
            public const int French_Switzerland = 0x100C;
            ///<summary>
            ///Culture identifier constant for Galician.
            ///</summary>
            ///<remarks>Culture ID: 0x0056
            ///Culture Name:gl</remarks>
            public const int Galician = 0x0056;
            ///<summary>
            ///Culture identifier constant for Galician - Galician.
            ///</summary>
            ///<remarks>Culture ID: 0x0456
            ///Culture Name:gl-ES</remarks>
            public const int Galician_Galician = 0x0456;
            ///<summary>
            ///Culture identifier constant for Georgian.
            ///</summary>
            ///<remarks>Culture ID: 0x0037
            ///Culture Name:ka</remarks>
            public const int Georgian = 0x0037;
            ///<summary>
            ///Culture identifier constant for Georgian - Georgia.
            ///</summary>
            ///<remarks>Culture ID: 0x0437
            ///Culture Name:ka-GE</remarks>
            public const int Georgian_Georgia = 0x0437;
            ///<summary>
            ///Culture identifier constant for German.
            ///</summary>
            ///<remarks>Culture ID: 0x0007
            ///Culture Name:de</remarks>
            public const int German = 0x0007;
            ///<summary>
            ///Culture identifier constant for German - Austria.
            ///</summary>
            ///<remarks>Culture ID: 0x0C07
            ///Culture Name:de-AT</remarks>
            public const int German_Austria = 0x0C07;
            ///<summary>
            ///Culture identifier constant for German - Germany.
            ///</summary>
            ///<remarks>Culture ID: 0x0407
            ///Culture Name:de-DE</remarks>
            public const int German_Germany = 0x0407;
            ///<summary>
            ///Culture identifier constant for German - Liechtenstein.
            ///</summary>
            ///<remarks>Culture ID: 0x1407
            ///Culture Name:de-LI</remarks>
            public const int German_Liechtenstein = 0x1407;
            ///<summary>
            ///Culture identifier constant for German - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x1007
            ///Culture Name:de-LU</remarks>
            public const int German_Luxembourg = 0x1007;
            ///<summary>
            ///Culture identifier constant for German - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0807
            ///Culture Name:de-CH</remarks>
            public const int German_Switzerland = 0x0807;
            ///<summary>
            ///Culture identifier constant for Greek.
            ///</summary>
            ///<remarks>Culture ID: 0x0008
            ///Culture Name:el</remarks>
            public const int Greek = 0x0008;
            ///<summary>
            ///Culture identifier constant for Greek - Greece.
            ///</summary>
            ///<remarks>Culture ID: 0x0408
            ///Culture Name:el-GR</remarks>
            public const int Greek_Greece = 0x0408;
            ///<summary>
            ///Culture identifier constant for Gujarati.
            ///</summary>
            ///<remarks>Culture ID: 0x0047
            ///Culture Name:gu</remarks>
            public const int Gujarati = 0x0047;
            ///<summary>
            ///Culture identifier constant for Gujarati - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0447
            ///Culture Name:gu-IN</remarks>
            public const int Gujarati_India = 0x0447;
            ///<summary>
            ///Culture identifier constant for Hebrew.
            ///</summary>
            ///<remarks>Culture ID: 0x000D
            ///Culture Name:he</remarks>
            public const int Hebrew = 0x000D;
            ///<summary>
            ///Culture identifier constant for Hebrew - Israel.
            ///</summary>
            ///<remarks>Culture ID: 0x040D
            ///Culture Name:he-IL</remarks>
            public const int Hebrew_Israel = 0x040D;
            ///<summary>
            ///Culture identifier constant for Hindi.
            ///</summary>
            ///<remarks>Culture ID: 0x0039
            ///Culture Name:hi</remarks>
            public const int Hindi = 0x0039;
            ///<summary>
            ///Culture identifier constant for Hindi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0439
            ///Culture Name:hi-IN</remarks>
            public const int Hindi_India = 0x0439;
            ///<summary>
            ///Culture identifier constant for Hungarian.
            ///</summary>
            ///<remarks>Culture ID: 0x000E
            ///Culture Name:hu</remarks>
            public const int Hungarian = 0x000E;
            ///<summary>
            ///Culture identifier constant for Hungarian - Hungary.
            ///</summary>
            ///<remarks>Culture ID: 0x040E
            ///Culture Name:hu-HU</remarks>
            public const int Hungarian_Hungary = 0x040E;
            ///<summary>
            ///Culture identifier constant for Icelandic.
            ///</summary>
            ///<remarks>Culture ID: 0x000F
            ///Culture Name:is</remarks>
            public const int Icelandic = 0x000F;
            ///<summary>
            ///Culture identifier constant for Icelandic - Iceland.
            ///</summary>
            ///<remarks>Culture ID: 0x040F
            ///Culture Name:is-IS</remarks>
            public const int Icelandic_Iceland = 0x040F;
            ///<summary>
            ///Culture identifier constant for Indonesian.
            ///</summary>
            ///<remarks>Culture ID: 0x0021
            ///Culture Name:id</remarks>
            public const int Indonesian = 0x0021;
            ///<summary>
            ///Culture identifier constant for Indonesian - Indonesia.
            ///</summary>
            ///<remarks>Culture ID: 0x0421
            ///Culture Name:id-ID</remarks>
            public const int Indonesian_Indonesia = 0x0421;
            ///<summary>
            ///Culture identifier constant for Italian.
            ///</summary>
            ///<remarks>Culture ID: 0x0010
            ///Culture Name:it</remarks>
            public const int Italian = 0x0010;
            ///<summary>
            ///Culture identifier constant for Italian - Italy.
            ///</summary>
            ///<remarks>Culture ID: 0x0410
            ///Culture Name:it-IT</remarks>
            public const int Italian_Italy = 0x0410;
            ///<summary>
            ///Culture identifier constant for Italian - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0810
            ///Culture Name:it-CH</remarks>
            public const int Italian_Switzerland = 0x0810;
            ///<summary>
            ///Culture identifier constant for Japanese.
            ///</summary>
            ///<remarks>Culture ID: 0x0011
            ///Culture Name:ja</remarks>
            public const int Japanese = 0x0011;
            ///<summary>
            ///Culture identifier constant for Japanese - Japan.
            ///</summary>
            ///<remarks>Culture ID: 0x0411
            ///Culture Name:ja-JP</remarks>
            public const int Japanese_Japan = 0x0411;
            ///<summary>
            ///Culture identifier constant for Kannada.
            ///</summary>
            ///<remarks>Culture ID: 0x004B
            ///Culture Name:kn</remarks>
            public const int Kannada = 0x004B;
            ///<summary>
            ///Culture identifier constant for Kannada - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044B
            ///Culture Name:kn-IN</remarks>
            public const int Kannada_India = 0x044B;
            ///<summary>
            ///Culture identifier constant for Kazakh.
            ///</summary>
            ///<remarks>Culture ID: 0x003F
            ///Culture Name:kk</remarks>
            public const int Kazakh = 0x003F;
            ///<summary>
            ///Culture identifier constant for Kazakh - Kazakhstan.
            ///</summary>
            ///<remarks>Culture ID: 0x043F
            ///Culture Name:kk-KZ</remarks>
            public const int Kazakh_Kazakhstan = 0x043F;
            ///<summary>
            ///Culture identifier constant for Konkani.
            ///</summary>
            ///<remarks>Culture ID: 0x0057
            ///Culture Name:kok</remarks>
            public const int Konkani = 0x0057;
            ///<summary>
            ///Culture identifier constant for Konkani - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0457
            ///Culture Name:kok-IN</remarks>
            public const int Konkani_India = 0x0457;
            ///<summary>
            ///Culture identifier constant for Korean.
            ///</summary>
            ///<remarks>Culture ID: 0x0012
            ///Culture Name:ko</remarks>
            public const int Korean = 0x0012;
            ///<summary>
            ///Culture identifier constant for Korean - Korea.
            ///</summary>
            ///<remarks>Culture ID: 0x0412
            ///Culture Name:ko-KR</remarks>
            public const int Korean_Korea = 0x0412;
            ///<summary>
            ///Culture identifier constant for Kyrgyz.
            ///</summary>
            ///<remarks>Culture ID: 0x0040
            ///Culture Name:ky</remarks>
            public const int Kyrgyz = 0x0040;
            ///<summary>
            ///Culture identifier constant for Kyrgyz - Kyrgyzstan.
            ///</summary>
            ///<remarks>Culture ID: 0x0440
            ///Culture Name:ky-KG</remarks>
            public const int Kyrgyz_Kyrgyzstan = 0x0440;
            ///<summary>
            ///Culture identifier constant for Latvian.
            ///</summary>
            ///<remarks>Culture ID: 0x0026
            ///Culture Name:lv</remarks>
            public const int Latvian = 0x0026;
            ///<summary>
            ///Culture identifier constant for Latvian - Latvia.
            ///</summary>
            ///<remarks>Culture ID: 0x0426
            ///Culture Name:lv-LV</remarks>
            public const int Latvian_Latvia = 0x0426;
            ///<summary>
            ///Culture identifier constant for Lithuanian.
            ///</summary>
            ///<remarks>Culture ID: 0x0027
            ///Culture Name:lt</remarks>
            public const int Lithuanian = 0x0027;
            ///<summary>
            ///Culture identifier constant for Lithuanian - Lithuania.
            ///</summary>
            ///<remarks>Culture ID: 0x0427
            ///Culture Name:lt-LT</remarks>
            public const int Lithuanian_Lithuania = 0x0427;
            ///<summary>
            ///Culture identifier constant for Macedonian.
            ///</summary>
            ///<remarks>Culture ID: 0x002F
            ///Culture Name:mk</remarks>
            public const int Macedonian = 0x002F;
            ///<summary>
            ///Culture identifier constant for Macedonian - Former Yugoslav Republic of Macedonia.
            ///</summary>
            ///<remarks>Culture ID: 0x042F
            ///Culture Name:mk-MK</remarks>
            public const int Macedonian_FormerYugoslavRepublicOfMacedonia = 0x042F;
            ///<summary>
            ///Culture identifier constant for Malay.
            ///</summary>
            ///<remarks>Culture ID: 0x003E
            ///Culture Name:ms</remarks>
            public const int Malay = 0x003E;
            ///<summary>
            ///Culture identifier constant for Malay - Brunei.
            ///</summary>
            ///<remarks>Culture ID: 0x083E
            ///Culture Name:ms-BN</remarks>
            public const int Malay_Brunei = 0x083E;
            ///<summary>
            ///Culture identifier constant for Malay - Malaysia.
            ///</summary>
            ///<remarks>Culture ID: 0x043E
            ///Culture Name:ms-MY</remarks>
            public const int Malay_Malaysia = 0x043E;
            ///<summary>
            ///Culture identifier constant for Marathi.
            ///</summary>
            ///<remarks>Culture ID: 0x004E
            ///Culture Name:mr</remarks>
            public const int Marathi = 0x004E;
            ///<summary>
            ///Culture identifier constant for Marathi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044E
            ///Culture Name:mr-IN</remarks>
            public const int Marathi_India = 0x044E;
            ///<summary>
            ///Culture identifier constant for Mongolian.
            ///</summary>
            ///<remarks>Culture ID: 0x0050
            ///Culture Name:mn</remarks>
            public const int Mongolian = 0x0050;
            ///<summary>
            ///Culture identifier constant for Mongolian - Mongolia.
            ///</summary>
            ///<remarks>Culture ID: 0x0450
            ///Culture Name:mn-MN</remarks>
            public const int Mongolian_Mongolia = 0x0450;
            ///<summary>
            ///Culture identifier constant for Norwegian.
            ///</summary>
            ///<remarks>Culture ID: 0x0014
            ///Culture Name:no</remarks>
            public const int Norwegian = 0x0014;
            ///<summary>
            ///Culture identifier constant for Norwegian (Bokmål) - Norway.
            ///</summary>
            ///<remarks>Culture ID: 0x0414
            ///Culture Name:nb-NO</remarks>
            public const int Norwegian_Bokmål_Norway = 0x0414;
            ///<summary>
            ///Culture identifier constant for Norwegian (Nynorsk) - Norway.
            ///</summary>
            ///<remarks>Culture ID: 0x0814
            ///Culture Name:nn-NO</remarks>
            public const int Norwegian_Nynorsk_Norway = 0x0814;
            ///<summary>
            ///Culture identifier constant for Polish.
            ///</summary>
            ///<remarks>Culture ID: 0x0015
            ///Culture Name:pl</remarks>
            public const int Polish = 0x0015;
            ///<summary>
            ///Culture identifier constant for Polish - Poland.
            ///</summary>
            ///<remarks>Culture ID: 0x0415
            ///Culture Name:pl-PL</remarks>
            public const int Polish_Poland = 0x0415;
            ///<summary>
            ///Culture identifier constant for Portuguese.
            ///</summary>
            ///<remarks>Culture ID: 0x0016
            ///Culture Name:pt</remarks>
            public const int Portuguese = 0x0016;
            ///<summary>
            ///Culture identifier constant for Portuguese - Brazil.
            ///</summary>
            ///<remarks>Culture ID: 0x0416
            ///Culture Name:pt-BR</remarks>
            public const int Portuguese_Brazil = 0x0416;
            ///<summary>
            ///Culture identifier constant for Portuguese - Portugal.
            ///</summary>
            ///<remarks>Culture ID: 0x0816
            ///Culture Name:pt-PT</remarks>
            public const int Portuguese_Portugal = 0x0816;
            ///<summary>
            ///Culture identifier constant for Punjabi.
            ///</summary>
            ///<remarks>Culture ID: 0x0046
            ///Culture Name:pa</remarks>
            public const int Punjabi = 0x0046;
            ///<summary>
            ///Culture identifier constant for Punjabi - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0446
            ///Culture Name:pa-IN</remarks>
            public const int Punjabi_India = 0x0446;
            ///<summary>
            ///Culture identifier constant for Romanian.
            ///</summary>
            ///<remarks>Culture ID: 0x0018
            ///Culture Name:ro</remarks>
            public const int Romanian = 0x0018;
            ///<summary>
            ///Culture identifier constant for Romanian - Romania.
            ///</summary>
            ///<remarks>Culture ID: 0x0418
            ///Culture Name:ro-RO</remarks>
            public const int Romanian_Romania = 0x0418;
            ///<summary>
            ///Culture identifier constant for Russian.
            ///</summary>
            ///<remarks>Culture ID: 0x0019
            ///Culture Name:ru</remarks>
            public const int Russian = 0x0019;
            ///<summary>
            ///Culture identifier constant for Russian - Russia.
            ///</summary>
            ///<remarks>Culture ID: 0x0419
            ///Culture Name:ru-RU</remarks>
            public const int Russian_Russia = 0x0419;
            ///<summary>
            ///Culture identifier constant for Sanskrit.
            ///</summary>
            ///<remarks>Culture ID: 0x004F
            ///Culture Name:sa</remarks>
            public const int Sanskrit = 0x004F;
            ///<summary>
            ///Culture identifier constant for Sanskrit - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044F
            ///Culture Name:sa-IN</remarks>
            public const int Sanskrit_India = 0x044F;
            ///<summary>
            ///Culture identifier constant for Serbian (Cyrillic) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x0C1A
            ///Culture Name:sr-SP-Cyrl</remarks>
            public const int Serbian_Cyrillic_Serbia = 0x0C1A;
            ///<summary>
            ///Culture identifier constant for Serbian (Latin) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x081A
            ///Culture Name:sr-SP-Latn</remarks>
            public const int Serbian_Latin_Serbia = 0x081A;
            ///<summary>
            ///Culture identifier constant for Slovak.
            ///</summary>
            ///<remarks>Culture ID: 0x001B
            ///Culture Name:sk</remarks>
            public const int Slovak = 0x001B;
            ///<summary>
            ///Culture identifier constant for Slovak - Slovakia.
            ///</summary>
            ///<remarks>Culture ID: 0x041B
            ///Culture Name:sk-SK</remarks>
            public const int Slovak_Slovakia = 0x041B;
            ///<summary>
            ///Culture identifier constant for Slovenian.
            ///</summary>
            ///<remarks>Culture ID: 0x0024
            ///Culture Name:sl</remarks>
            public const int Slovenian = 0x0024;
            ///<summary>
            ///Culture identifier constant for Slovenian - Slovenia.
            ///</summary>
            ///<remarks>Culture ID: 0x0424
            ///Culture Name:sl-SI</remarks>
            public const int Slovenian_Slovenia = 0x0424;
            ///<summary>
            ///Culture identifier constant for Spanish.
            ///</summary>
            ///<remarks>Culture ID: 0x000A
            ///Culture Name:es</remarks>
            public const int Spanish = 0x000A;
            ///<summary>
            ///Culture identifier constant for Spanish - Argentina.
            ///</summary>
            ///<remarks>Culture ID: 0x2C0A
            ///Culture Name:es-AR</remarks>
            public const int Spanish_Argentina = 0x2C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Bolivia.
            ///</summary>
            ///<remarks>Culture ID: 0x400A
            ///Culture Name:es-BO</remarks>
            public const int Spanish_Bolivia = 0x400A;
            ///<summary>
            ///Culture identifier constant for Spanish - Chile.
            ///</summary>
            ///<remarks>Culture ID: 0x340A
            ///Culture Name:es-CL</remarks>
            public const int Spanish_Chile = 0x340A;
            ///<summary>
            ///Culture identifier constant for Spanish - Colombia.
            ///</summary>
            ///<remarks>Culture ID: 0x240A
            ///Culture Name:es-CO</remarks>
            public const int Spanish_Colombia = 0x240A;
            ///<summary>
            ///Culture identifier constant for Spanish - Costa Rica.
            ///</summary>
            ///<remarks>Culture ID: 0x140A
            ///Culture Name:es-CR</remarks>
            public const int Spanish_CostaRica = 0x140A;
            ///<summary>
            ///Culture identifier constant for Spanish - Dominican Republic.
            ///</summary>
            ///<remarks>Culture ID: 0x1C0A
            ///Culture Name:es-DO</remarks>
            public const int Spanish_DominicanRepublic = 0x1C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Ecuador.
            ///</summary>
            ///<remarks>Culture ID: 0x300A
            ///Culture Name:es-EC</remarks>
            public const int Spanish_Ecuador = 0x300A;
            ///<summary>
            ///Culture identifier constant for Spanish - El Salvador.
            ///</summary>
            ///<remarks>Culture ID: 0x440A
            ///Culture Name:es-SV</remarks>
            public const int Spanish_ElSalvador = 0x440A;
            ///<summary>
            ///Culture identifier constant for Spanish - Guatemala.
            ///</summary>
            ///<remarks>Culture ID: 0x100A
            ///Culture Name:es-GT</remarks>
            public const int Spanish_Guatemala = 0x100A;
            ///<summary>
            ///Culture identifier constant for Spanish - Honduras.
            ///</summary>
            ///<remarks>Culture ID: 0x480A
            ///Culture Name:es-HN</remarks>
            public const int Spanish_Honduras = 0x480A;
            ///<summary>
            ///Culture identifier constant for Spanish - Mexico.
            ///</summary>
            ///<remarks>Culture ID: 0x080A
            ///Culture Name:es-MX</remarks>
            public const int Spanish_Mexico = 0x080A;
            ///<summary>
            ///Culture identifier constant for Spanish - Nicaragua.
            ///</summary>
            ///<remarks>Culture ID: 0x4C0A
            ///Culture Name:es-NI</remarks>
            public const int Spanish_Nicaragua = 0x4C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Panama.
            ///</summary>
            ///<remarks>Culture ID: 0x180A
            ///Culture Name:es-PA</remarks>
            public const int Spanish_Panama = 0x180A;
            ///<summary>
            ///Culture identifier constant for Spanish - Paraguay.
            ///</summary>
            ///<remarks>Culture ID: 0x3C0A
            ///Culture Name:es-PY</remarks>
            public const int Spanish_Paraguay = 0x3C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Peru.
            ///</summary>
            ///<remarks>Culture ID: 0x280A
            ///Culture Name:es-PE</remarks>
            public const int Spanish_Peru = 0x280A;
            ///<summary>
            ///Culture identifier constant for Spanish - Puerto Rico.
            ///</summary>
            ///<remarks>Culture ID: 0x500A
            ///Culture Name:es-PR</remarks>
            public const int Spanish_PuertoRico = 0x500A;
            ///<summary>
            ///Culture identifier constant for Spanish - Spain.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0A
            ///Culture Name:es-ES</remarks>
            public const int Spanish_Spain = 0x0C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Uruguay.
            ///</summary>
            ///<remarks>Culture ID: 0x380A
            ///Culture Name:es-UY</remarks>
            public const int Spanish_Uruguay = 0x380A;
            ///<summary>
            ///Culture identifier constant for Spanish - Venezuela.
            ///</summary>
            ///<remarks>Culture ID: 0x200A
            ///Culture Name:es-VE</remarks>
            public const int Spanish_Venezuela = 0x200A;
            ///<summary>
            ///Culture identifier constant for Swahili.
            ///</summary>
            ///<remarks>Culture ID: 0x0041
            ///Culture Name:sw</remarks>
            public const int Swahili = 0x0041;
            ///<summary>
            ///Culture identifier constant for Swahili - Kenya.
            ///</summary>
            ///<remarks>Culture ID: 0x0441
            ///Culture Name:sw-KE</remarks>
            public const int Swahili_Kenya = 0x0441;
            ///<summary>
            ///Culture identifier constant for Swedish.
            ///</summary>
            ///<remarks>Culture ID: 0x001D
            ///Culture Name:sv</remarks>
            public const int Swedish = 0x001D;
            ///<summary>
            ///Culture identifier constant for Swedish - Finland.
            ///</summary>
            ///<remarks>Culture ID: 0x081D
            ///Culture Name:sv-FI</remarks>
            public const int Swedish_Finland = 0x081D;
            ///<summary>
            ///Culture identifier constant for Swedish - Sweden.
            ///</summary>
            ///<remarks>Culture ID: 0x041D
            ///Culture Name:sv-SE</remarks>
            public const int Swedish_Sweden = 0x041D;
            ///<summary>
            ///Culture identifier constant for Syriac.
            ///</summary>
            ///<remarks>Culture ID: 0x005A
            ///Culture Name:syr</remarks>
            public const int Syriac = 0x005A;
            ///<summary>
            ///Culture identifier constant for Syriac - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x045A
            ///Culture Name:syr-SY</remarks>
            public const int Syriac_Syria = 0x045A;
            ///<summary>
            ///Culture identifier constant for Tamil.
            ///</summary>
            ///<remarks>Culture ID: 0x0049
            ///Culture Name:ta</remarks>
            public const int Tamil = 0x0049;
            ///<summary>
            ///Culture identifier constant for Tamil - India.
            ///</summary>
            ///<remarks>Culture ID: 0x0449
            ///Culture Name:ta-IN</remarks>
            public const int Tamil_India = 0x0449;
            ///<summary>
            ///Culture identifier constant for Tatar.
            ///</summary>
            ///<remarks>Culture ID: 0x0044
            ///Culture Name:tt</remarks>
            public const int Tatar = 0x0044;
            ///<summary>
            ///Culture identifier constant for Tatar - Russia.
            ///</summary>
            ///<remarks>Culture ID: 0x0444
            ///Culture Name:tt-RU</remarks>
            public const int Tatar_Russia = 0x0444;
            ///<summary>
            ///Culture identifier constant for Telugu.
            ///</summary>
            ///<remarks>Culture ID: 0x004A
            ///Culture Name:te</remarks>
            public const int Telugu = 0x004A;
            ///<summary>
            ///Culture identifier constant for Telugu - India.
            ///</summary>
            ///<remarks>Culture ID: 0x044A
            ///Culture Name:te-IN</remarks>
            public const int Telugu_India = 0x044A;
            ///<summary>
            ///Culture identifier constant for Thai.
            ///</summary>
            ///<remarks>Culture ID: 0x001E
            ///Culture Name:th</remarks>
            public const int Thai = 0x001E;
            ///<summary>
            ///Culture identifier constant for Thai - Thailand.
            ///</summary>
            ///<remarks>Culture ID: 0x041E
            ///Culture Name:th-TH</remarks>
            public const int Thai_Thailand = 0x041E;
            ///<summary>
            ///Culture identifier constant for Turkish.
            ///</summary>
            ///<remarks>Culture ID: 0x001F
            ///Culture Name:tr</remarks>
            public const int Turkish = 0x001F;
            ///<summary>
            ///Culture identifier constant for Turkish - Turkey.
            ///</summary>
            ///<remarks>Culture ID: 0x041F
            ///Culture Name:tr-TR</remarks>
            public const int Turkish_Turkey = 0x041F;
            ///<summary>
            ///Culture identifier constant for Ukrainian.
            ///</summary>
            ///<remarks>Culture ID: 0x0022
            ///Culture Name:uk</remarks>
            public const int Ukrainian = 0x0022;
            ///<summary>
            ///Culture identifier constant for Ukrainian - Ukraine.
            ///</summary>
            ///<remarks>Culture ID: 0x0422
            ///Culture Name:uk-UA</remarks>
            public const int Ukrainian_Ukraine = 0x0422;
            ///<summary>
            ///Culture identifier constant for Urdu.
            ///</summary>
            ///<remarks>Culture ID: 0x0020
            ///Culture Name:ur</remarks>
            public const int Urdu = 0x0020;
            ///<summary>
            ///Culture identifier constant for Urdu - Pakistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0420
            ///Culture Name:ur-PK</remarks>
            public const int Urdu_Pakistan = 0x0420;
            ///<summary>
            ///Culture identifier constant for Uzbek.
            ///</summary>
            ///<remarks>Culture ID: 0x0043
            ///Culture Name:uz</remarks>
            public const int Uzbek = 0x0043;
            ///<summary>
            ///Culture identifier constant for Uzbek (Cyrillic) - Uzbekistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0843
            ///Culture Name:uz-UZ-Cyrl</remarks>
            public const int Uzbek_Cyrillic_Uzbekistan = 0x0843;
            ///<summary>
            ///Culture identifier constant for Uzbek (Latin) - Uzbekistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0443
            ///Culture Name:uz-UZ-Latn</remarks>
            public const int Uzbek_Latin_Uzbekistan = 0x0443;
            ///<summary>
            ///Culture identifier constant for Vietnamese.
            ///</summary>
            ///<remarks>Culture ID: 0x002A
            ///Culture Name:vi</remarks>
            public const int Vietnamese = 0x002A;
            ///<summary>
            ///Culture identifier constant for Vietnamese - Vietnam.
            ///</summary>
            ///<remarks>Culture ID: 0x042A
            ///Culture Name:vi-VN</remarks>
            public const int Vietnamese_Vietnam = 0x042A;
        }
        /// <summary>
        /// Provides a series of constant string values that relate to the culture information and
        /// the string description of known countries/regions.
        /// </summary>
        public static class CountryRegions
        {
            ///<summary>
            ///Country/Region constant for None.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x007F
            ///Culture Name: <see cref="System.String.Empty"/>
            ///</remarks>
            public const string None = "None";
            ///<summary>
            ///Country/Region constant for Afrikaans.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0036
            ///Culture Name: af
            ///</remarks>
            public const string Afrikaans = "Afrikaans";
            ///<summary>
            ///Country/Region constant for Afrikaans - South Africa.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0436
            ///Culture Name: af-ZA
            ///</remarks>
            public const string Afrikaans_SouthAfrica = "Afrikaans - South Africa";
            ///<summary>
            ///Country/Region constant for Albanian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001C
            ///Culture Name: sq
            ///</remarks>
            public const string Albanian = "Albanian";
            ///<summary>
            ///Country/Region constant for Albanian - Albania.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041C
            ///Culture Name: sq-AL
            ///</remarks>
            public const string Albanian_Albania = "Albanian - Albania";
            ///<summary>
            ///Country/Region constant for Arabic.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0001
            ///Culture Name: ar
            ///</remarks>
            public const string Arabic = "Arabic";
            ///<summary>
            ///Country/Region constant for Arabic - Algeria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1401
            ///Culture Name: ar-DZ
            ///</remarks>
            public const string Arabic_Algeria = "Arabic - Algeria";
            ///<summary>
            ///Country/Region constant for Arabic - Bahrain.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3C01
            ///Culture Name: ar-BH
            ///</remarks>
            public const string Arabic_Bahrain = "Arabic - Bahrain";
            ///<summary>
            ///Country/Region constant for Arabic - Egypt.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C01
            ///Culture Name: ar-EG
            ///</remarks>
            public const string Arabic_Egypt = "Arabic - Egypt";
            ///<summary>
            ///Country/Region constant for Arabic - Iraq.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0801
            ///Culture Name: ar-IQ
            ///</remarks>
            public const string Arabic_Iraq = "Arabic - Iraq";
            ///<summary>
            ///Country/Region constant for Arabic - Jordan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2C01
            ///Culture Name: ar-JO
            ///</remarks>
            public const string Arabic_Jordan = "Arabic - Jordan";
            ///<summary>
            ///Country/Region constant for Arabic - Kuwait.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3401
            ///Culture Name: ar-KW
            ///</remarks>
            public const string Arabic_Kuwait = "Arabic - Kuwait";
            ///<summary>
            ///Country/Region constant for Arabic - Lebanon.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3001
            ///Culture Name: ar-LB
            ///</remarks>
            public const string Arabic_Lebanon = "Arabic - Lebanon";
            ///<summary>
            ///Country/Region constant for Arabic - Libya.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1001
            ///Culture Name: ar-LY
            ///</remarks>
            public const string Arabic_Libya = "Arabic - Libya";
            ///<summary>
            ///Country/Region constant for Arabic - Morocco.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1801
            ///Culture Name: ar-MA
            ///</remarks>
            public const string Arabic_Morocco = "Arabic - Morocco";
            ///<summary>
            ///Country/Region constant for Arabic - Oman.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2001
            ///Culture Name: ar-OM
            ///</remarks>
            public const string Arabic_Oman = "Arabic - Oman";
            ///<summary>
            ///Country/Region constant for Arabic - Qatar.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x4001
            ///Culture Name: ar-QA
            ///</remarks>
            public const string Arabic_Qatar = "Arabic - Qatar";
            ///<summary>
            ///Country/Region constant for Arabic - Saudi Arabia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0401
            ///Culture Name: ar-SA
            ///</remarks>
            public const string Arabic_SaudiArabia = "Arabic - Saudi Arabia";
            ///<summary>
            ///Country/Region constant for Arabic - Syria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2801
            ///Culture Name: ar-SY
            ///</remarks>
            public const string Arabic_Syria = "Arabic - Syria";
            ///<summary>
            ///Country/Region constant for Arabic - Tunisia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C01
            ///Culture Name: ar-TN
            ///</remarks>
            public const string Arabic_Tunisia = "Arabic - Tunisia";
            ///<summary>
            ///Country/Region constant for Arabic - United Arab Emirates.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3801
            ///Culture Name: ar-AE
            ///</remarks>
            public const string Arabic_UnitedArabEmirates = "Arabic - United Arab Emirates";
            ///<summary>
            ///Country/Region constant for Arabic - Yemen.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2401
            ///Culture Name: ar-YE
            ///</remarks>
            public const string Arabic_Yemen = "Arabic - Yemen";
            ///<summary>
            ///Country/Region constant for Armenian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x002B
            ///Culture Name: hy
            ///</remarks>
            public const string Armenian = "Armenian";
            ///<summary>
            ///Country/Region constant for Armenian - Armenia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042B
            ///Culture Name: hy-AM
            ///</remarks>
            public const string Armenian_Armenia = "Armenian - Armenia";
            ///<summary>
            ///Country/Region constant for Azeri.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x002C
            ///Culture Name: az
            ///</remarks>
            public const string Azeri = "Azeri";
            ///<summary>
            ///Country/Region constant for Azeri (Cyrillic) - Azerbaijan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x082C
            ///Culture Name: az-AZ-Cyrl
            ///</remarks>
            public const string Azeri_Cyrillic_Azerbaijan = "Azeri (Cyrillic) - Azerbaijan";
            ///<summary>
            ///Country/Region constant for Azeri (Latin) - Azerbaijan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042C
            ///Culture Name: az-AZ-Latn
            ///</remarks>
            public const string Azeri_Latin_Azerbaijan = "Azeri (Latin) - Azerbaijan";
            ///<summary>
            ///Country/Region constant for Basque.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x002D
            ///Culture Name: eu
            ///</remarks>
            public const string Basque = "Basque";
            ///<summary>
            ///Country/Region constant for Basque - Basque.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042D
            ///Culture Name: eu-ES
            ///</remarks>
            public const string Basque_Basque = "Basque - Basque";
            ///<summary>
            ///Country/Region constant for Belarusian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0023
            ///Culture Name: be
            ///</remarks>
            public const string Belarusian = "Belarusian";
            ///<summary>
            ///Country/Region constant for Belarusian - Belarus.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0423
            ///Culture Name: be-BY
            ///</remarks>
            public const string Belarusian_Belarus = "Belarusian - Belarus";
            ///<summary>
            ///Country/Region constant for Bulgarian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0002
            ///Culture Name: bg
            ///</remarks>
            public const string Bulgarian = "Bulgarian";
            ///<summary>
            ///Country/Region constant for Bulgarian - Bulgaria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0402
            ///Culture Name: bg-BG
            ///</remarks>
            public const string Bulgarian_Bulgaria = "Bulgarian - Bulgaria";
            ///<summary>
            ///Country/Region constant for Catalan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0003
            ///Culture Name: ca
            ///</remarks>
            public const string Catalan = "Catalan";
            ///<summary>
            ///Country/Region constant for Catalan - Catalan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0403
            ///Culture Name: ca-ES
            ///</remarks>
            public const string Catalan_Catalan = "Catalan - Catalan";
            ///<summary>
            ///Country/Region constant for Chinese - Hong Kong SAR.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C04
            ///Culture Name: zh-HK
            ///</remarks>
            public const string Chinese_HongKongSAR = "Chinese - Hong Kong SAR";
            ///<summary>
            ///Country/Region constant for Chinese - Macao SAR.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1404
            ///Culture Name: zh-MO
            ///</remarks>
            public const string Chinese_MacaoSAR = "Chinese - Macao SAR";
            ///<summary>
            ///Country/Region constant for Chinese - China.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0804
            ///Culture Name: zh-CN
            ///</remarks>
            public const string Chinese_China = "Chinese - China";
            ///<summary>
            ///Country/Region constant for Chinese (Simplified).
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0004
            ///Culture Name: zh-CHS
            ///</remarks>
            public const string Chinese_Simplified = "Chinese (Simplified)";
            ///<summary>
            ///Country/Region constant for Chinese - Singapore.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1004
            ///Culture Name: zh-SG
            ///</remarks>
            public const string Chinese_Singapore = "Chinese - Singapore";
            ///<summary>
            ///Country/Region constant for Chinese - Taiwan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0404
            ///Culture Name: zh-TW
            ///</remarks>
            public const string Chinese_Taiwan = "Chinese - Taiwan";
            ///<summary>
            ///Country/Region constant for Chinese (Traditional).
            ///</summary>
            ///<remarks>
            ///CultureID: 0x7C04
            ///Culture Name: zh-CHT
            ///</remarks>
            public const string Chinese_Traditional = "Chinese (Traditional)";
            ///<summary>
            ///Country/Region constant for Croatian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001A
            ///Culture Name: hr
            ///</remarks>
            public const string Croatian = "Croatian";
            ///<summary>
            ///Country/Region constant for Croatian - Croatia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041A
            ///Culture Name: hr-HR
            ///</remarks>
            public const string Croatian_Croatia = "Croatian - Croatia";
            ///<summary>
            ///Country/Region constant for Czech.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0005
            ///Culture Name: cs
            ///</remarks>
            public const string Czech = "Czech";
            ///<summary>
            ///Country/Region constant for Czech - Czech Republic.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0405
            ///Culture Name: cs-CZ
            ///</remarks>
            public const string Czech_CzechRepublic = "Czech - Czech Republic";
            ///<summary>
            ///Country/Region constant for Danish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0006
            ///Culture Name: da
            ///</remarks>
            public const string Danish = "Danish";
            ///<summary>
            ///Country/Region constant for Danish - Denmark.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0406
            ///Culture Name: da-DK
            ///</remarks>
            public const string Danish_Denmark = "Danish - Denmark";
            ///<summary>
            ///Country/Region constant for Dhivehi.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0065
            ///Culture Name: div
            ///</remarks>
            public const string Dhivehi = "Dhivehi";
            ///<summary>
            ///Country/Region constant for Dhivehi - Maldives.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0465
            ///Culture Name: div-MV
            ///</remarks>
            public const string Dhivehi_Maldives = "Dhivehi - Maldives";
            ///<summary>
            ///Country/Region constant for Dutch.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0013
            ///Culture Name: nl
            ///</remarks>
            public const string Dutch = "Dutch";
            ///<summary>
            ///Country/Region constant for Dutch - Belgium.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0813
            ///Culture Name: nl-BE
            ///</remarks>
            public const string Dutch_Belgium = "Dutch - Belgium";
            ///<summary>
            ///Country/Region constant for Dutch - The Netherlands.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0413
            ///Culture Name: nl-NL
            ///</remarks>
            public const string Dutch_TheNetherlands = "Dutch - The Netherlands";
            ///<summary>
            ///Country/Region constant for English.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0009
            ///Culture Name: en
            ///</remarks>
            public const string English = "English";
            ///<summary>
            ///Country/Region constant for English - Australia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C09
            ///Culture Name: en-AU
            ///</remarks>
            public const string English_Australia = "English - Australia";
            ///<summary>
            ///Country/Region constant for English - Belize.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2809
            ///Culture Name: en-BZ
            ///</remarks>
            public const string English_Belize = "English - Belize";
            ///<summary>
            ///Country/Region constant for English - Canada.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1009
            ///Culture Name: en-CA
            ///</remarks>
            public const string English_Canada = "English - Canada";
            ///<summary>
            ///Country/Region constant for English - Caribbean.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2409
            ///Culture Name: en-CB
            ///</remarks>
            public const string English_Caribbean = "English - Caribbean";
            ///<summary>
            ///Country/Region constant for English - Ireland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1809
            ///Culture Name: en-IE
            ///</remarks>
            public const string English_Ireland = "English - Ireland";
            ///<summary>
            ///Country/Region constant for English - Jamaica.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2009
            ///Culture Name: en-JM
            ///</remarks>
            public const string English_Jamaica = "English - Jamaica";
            ///<summary>
            ///Country/Region constant for English - New Zealand.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1409
            ///Culture Name: en-NZ
            ///</remarks>
            public const string English_NewZealand = "English - New Zealand";
            ///<summary>
            ///Country/Region constant for English - Philippines.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3409
            ///Culture Name: en-PH
            ///</remarks>
            public const string English_Philippines = "English - Philippines";
            ///<summary>
            ///Country/Region constant for English - South Africa.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C09
            ///Culture Name: en-ZA
            ///</remarks>
            public const string English_SouthAfrica = "English - South Africa";
            ///<summary>
            ///Country/Region constant for English - Trinidad and Tobago.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2C09
            ///Culture Name: en-TT
            ///</remarks>
            public const string English_TrinidadAndTobago = "English - Trinidad and Tobago";
            ///<summary>
            ///Country/Region constant for English - United Kingdom.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0809
            ///Culture Name: en-GB
            ///</remarks>
            public const string English_UnitedKingdom = "English - United Kingdom";
            ///<summary>
            ///Country/Region constant for English - United States.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0409
            ///Culture Name: en-US
            ///</remarks>
            public const string English_UnitedStates = "English - United States";
            ///<summary>
            ///Country/Region constant for English - Zimbabwe.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3009
            ///Culture Name: en-ZW
            ///</remarks>
            public const string English_Zimbabwe = "English - Zimbabwe";
            ///<summary>
            ///Country/Region constant for Estonian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0025
            ///Culture Name: et
            ///</remarks>
            public const string Estonian = "Estonian";
            ///<summary>
            ///Country/Region constant for Estonian - Estonia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0425
            ///Culture Name: et-EE
            ///</remarks>
            public const string Estonian_Estonia = "Estonian - Estonia";
            ///<summary>
            ///Country/Region constant for Faroese.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0038
            ///Culture Name: fo
            ///</remarks>
            public const string Faroese = "Faroese";
            ///<summary>
            ///Country/Region constant for Faroese - Faroe Islands.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0438
            ///Culture Name: fo-FO
            ///</remarks>
            public const string Faroese_FaroeIslands = "Faroese - Faroe Islands";
            ///<summary>
            ///Country/Region constant for Farsi.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0029
            ///Culture Name: fa
            ///</remarks>
            public const string Farsi = "Farsi";
            ///<summary>
            ///Country/Region constant for Farsi - Iran.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0429
            ///Culture Name: fa-IR
            ///</remarks>
            public const string Farsi_Iran = "Farsi - Iran";
            ///<summary>
            ///Country/Region constant for Finnish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000B
            ///Culture Name: fi
            ///</remarks>
            public const string Finnish = "Finnish";
            ///<summary>
            ///Country/Region constant for Finnish - Finland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040B
            ///Culture Name: fi-FI
            ///</remarks>
            public const string Finnish_Finland = "Finnish - Finland";
            ///<summary>
            ///Country/Region constant for French.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000C
            ///Culture Name: fr
            ///</remarks>
            public const string French = "French";
            ///<summary>
            ///Country/Region constant for French - Belgium.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x080C
            ///Culture Name: fr-BE
            ///</remarks>
            public const string French_Belgium = "French - Belgium";
            ///<summary>
            ///Country/Region constant for French - Canada.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C0C
            ///Culture Name: fr-CA
            ///</remarks>
            public const string French_Canada = "French - Canada";
            ///<summary>
            ///Country/Region constant for French - France.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040C
            ///Culture Name: fr-FR
            ///</remarks>
            public const string French_France = "French - France";
            ///<summary>
            ///Country/Region constant for French - Luxembourg.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x140C
            ///Culture Name: fr-LU
            ///</remarks>
            public const string French_Luxembourg = "French - Luxembourg";
            ///<summary>
            ///Country/Region constant for French - Monaco.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x180C
            ///Culture Name: fr-MC
            ///</remarks>
            public const string French_Monaco = "French - Monaco";
            ///<summary>
            ///Country/Region constant for French - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x100C
            ///Culture Name: fr-CH
            ///</remarks>
            public const string French_Switzerland = "French - Switzerland";
            ///<summary>
            ///Country/Region constant for Galician.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0056
            ///Culture Name: gl
            ///</remarks>
            public const string Galician = "Galician";
            ///<summary>
            ///Country/Region constant for Galician - Galician.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0456
            ///Culture Name: gl-ES
            ///</remarks>
            public const string Galician_Galician = "Galician - Galician";
            ///<summary>
            ///Country/Region constant for Georgian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0037
            ///Culture Name: ka
            ///</remarks>
            public const string Georgian = "Georgian";
            ///<summary>
            ///Country/Region constant for Georgian - Georgia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0437
            ///Culture Name: ka-GE
            ///</remarks>
            public const string Georgian_Georgia = "Georgian - Georgia";
            ///<summary>
            ///Country/Region constant for German.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0007
            ///Culture Name: de
            ///</remarks>
            public const string German = "German";
            ///<summary>
            ///Country/Region constant for German - Austria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C07
            ///Culture Name: de-AT
            ///</remarks>
            public const string German_Austria = "German - Austria";
            ///<summary>
            ///Country/Region constant for German - Germany.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0407
            ///Culture Name: de-DE
            ///</remarks>
            public const string German_Germany = "German - Germany";
            ///<summary>
            ///Country/Region constant for German - Liechtenstein.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1407
            ///Culture Name: de-LI
            ///</remarks>
            public const string German_Liechtenstein = "German - Liechtenstein";
            ///<summary>
            ///Country/Region constant for German - Luxembourg.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1007
            ///Culture Name: de-LU
            ///</remarks>
            public const string German_Luxembourg = "German - Luxembourg";
            ///<summary>
            ///Country/Region constant for German - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0807
            ///Culture Name: de-CH
            ///</remarks>
            public const string German_Switzerland = "German - Switzerland";
            ///<summary>
            ///Country/Region constant for Greek.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0008
            ///Culture Name: el
            ///</remarks>
            public const string Greek = "Greek";
            ///<summary>
            ///Country/Region constant for Greek - Greece.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0408
            ///Culture Name: el-GR
            ///</remarks>
            public const string Greek_Greece = "Greek - Greece";
            ///<summary>
            ///Country/Region constant for Gujarati.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0047
            ///Culture Name: gu
            ///</remarks>
            public const string Gujarati = "Gujarati";
            ///<summary>
            ///Country/Region constant for Gujarati - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0447
            ///Culture Name: gu-IN
            ///</remarks>
            public const string Gujarati_India = "Gujarati - India";
            ///<summary>
            ///Country/Region constant for Hebrew.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000D
            ///Culture Name: he
            ///</remarks>
            public const string Hebrew = "Hebrew";
            ///<summary>
            ///Country/Region constant for Hebrew - Israel.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040D
            ///Culture Name: he-IL
            ///</remarks>
            public const string Hebrew_Israel = "Hebrew - Israel";
            ///<summary>
            ///Country/Region constant for Hindi.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0039
            ///Culture Name: hi
            ///</remarks>
            public const string Hindi = "Hindi";
            ///<summary>
            ///Country/Region constant for Hindi - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0439
            ///Culture Name: hi-IN
            ///</remarks>
            public const string Hindi_India = "Hindi - India";
            ///<summary>
            ///Country/Region constant for Hungarian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000E
            ///Culture Name: hu
            ///</remarks>
            public const string Hungarian = "Hungarian";
            ///<summary>
            ///Country/Region constant for Hungarian - Hungary.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040E
            ///Culture Name: hu-HU
            ///</remarks>
            public const string Hungarian_Hungary = "Hungarian - Hungary";
            ///<summary>
            ///Country/Region constant for Icelandic.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000F
            ///Culture Name: is
            ///</remarks>
            public const string Icelandic = "Icelandic";
            ///<summary>
            ///Country/Region constant for Icelandic - Iceland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040F
            ///Culture Name: is-IS
            ///</remarks>
            public const string Icelandic_Iceland = "Icelandic - Iceland";
            ///<summary>
            ///Country/Region constant for Indonesian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0021
            ///Culture Name: id
            ///</remarks>
            public const string Indonesian = "Indonesian";
            ///<summary>
            ///Country/Region constant for Indonesian - Indonesia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0421
            ///Culture Name: id-ID
            ///</remarks>
            public const string Indonesian_Indonesia = "Indonesian - Indonesia";
            ///<summary>
            ///Country/Region constant for Italian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0010
            ///Culture Name: it
            ///</remarks>
            public const string Italian = "Italian";
            ///<summary>
            ///Country/Region constant for Italian - Italy.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0410
            ///Culture Name: it-IT
            ///</remarks>
            public const string Italian_Italy = "Italian - Italy";
            ///<summary>
            ///Country/Region constant for Italian - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0810
            ///Culture Name: it-CH
            ///</remarks>
            public const string Italian_Switzerland = "Italian - Switzerland";
            ///<summary>
            ///Country/Region constant for Japanese.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0011
            ///Culture Name: ja
            ///</remarks>
            public const string Japanese = "Japanese";
            ///<summary>
            ///Country/Region constant for Japanese - Japan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0411
            ///Culture Name: ja-JP
            ///</remarks>
            public const string Japanese_Japan = "Japanese - Japan";
            ///<summary>
            ///Country/Region constant for Kannada.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x004B
            ///Culture Name: kn
            ///</remarks>
            public const string Kannada = "Kannada";
            ///<summary>
            ///Country/Region constant for Kannada - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x044B
            ///Culture Name: kn-IN
            ///</remarks>
            public const string Kannada_India = "Kannada - India";
            ///<summary>
            ///Country/Region constant for Kazakh.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x003F
            ///Culture Name: kk
            ///</remarks>
            public const string Kazakh = "Kazakh";
            ///<summary>
            ///Country/Region constant for Kazakh - Kazakhstan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x043F
            ///Culture Name: kk-KZ
            ///</remarks>
            public const string Kazakh_Kazakhstan = "Kazakh - Kazakhstan";
            ///<summary>
            ///Country/Region constant for Konkani.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0057
            ///Culture Name: kok
            ///</remarks>
            public const string Konkani = "Konkani";
            ///<summary>
            ///Country/Region constant for Konkani - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0457
            ///Culture Name: kok-IN
            ///</remarks>
            public const string Konkani_India = "Konkani - India";
            ///<summary>
            ///Country/Region constant for Korean.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0012
            ///Culture Name: ko
            ///</remarks>
            public const string Korean = "Korean";
            ///<summary>
            ///Country/Region constant for Korean - Korea.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0412
            ///Culture Name: ko-KR
            ///</remarks>
            public const string Korean_Korea = "Korean - Korea";
            ///<summary>
            ///Country/Region constant for Kyrgyz.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0040
            ///Culture Name: ky
            ///</remarks>
            public const string Kyrgyz = "Kyrgyz";
            ///<summary>
            ///Country/Region constant for Kyrgyz - Kyrgyzstan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0440
            ///Culture Name: ky-KG
            ///</remarks>
            public const string Kyrgyz_Kyrgyzstan = "Kyrgyz - Kyrgyzstan";
            ///<summary>
            ///Country/Region constant for Latvian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0026
            ///Culture Name: lv
            ///</remarks>
            public const string Latvian = "Latvian";
            ///<summary>
            ///Country/Region constant for Latvian - Latvia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0426
            ///Culture Name: lv-LV
            ///</remarks>
            public const string Latvian_Latvia = "Latvian - Latvia";
            ///<summary>
            ///Country/Region constant for Lithuanian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0027
            ///Culture Name: lt
            ///</remarks>
            public const string Lithuanian = "Lithuanian";
            ///<summary>
            ///Country/Region constant for Lithuanian - Lithuania.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0427
            ///Culture Name: lt-LT
            ///</remarks>
            public const string Lithuanian_Lithuania = "Lithuanian - Lithuania";
            ///<summary>
            ///Country/Region constant for Macedonian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x002F
            ///Culture Name: mk
            ///</remarks>
            public const string Macedonian = "Macedonian";
            ///<summary>
            ///Country/Region constant for Macedonian - Former Yugoslav Republic of Macedonia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042F
            ///Culture Name: mk-MK
            ///</remarks>
            public const string Macedonian_FormerYugoslavRepublicOfMacedonia = "Macedonian - Former Yugoslav Republic of Macedonia";
            ///<summary>
            ///Country/Region constant for Malay.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x003E
            ///Culture Name: ms
            ///</remarks>
            public const string Malay = "Malay";
            ///<summary>
            ///Country/Region constant for Malay - Brunei.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x083E
            ///Culture Name: ms-BN
            ///</remarks>
            public const string Malay_Brunei = "Malay - Brunei";
            ///<summary>
            ///Country/Region constant for Malay - Malaysia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x043E
            ///Culture Name: ms-MY
            ///</remarks>
            public const string Malay_Malaysia = "Malay - Malaysia";
            ///<summary>
            ///Country/Region constant for Marathi.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x004E
            ///Culture Name: mr
            ///</remarks>
            public const string Marathi = "Marathi";
            ///<summary>
            ///Country/Region constant for Marathi - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x044E
            ///Culture Name: mr-IN
            ///</remarks>
            public const string Marathi_India = "Marathi - India";
            ///<summary>
            ///Country/Region constant for Mongolian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0050
            ///Culture Name: mn
            ///</remarks>
            public const string Mongolian = "Mongolian";
            ///<summary>
            ///Country/Region constant for Mongolian - Mongolia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0450
            ///Culture Name: mn-MN
            ///</remarks>
            public const string Mongolian_Mongolia = "Mongolian - Mongolia";
            ///<summary>
            ///Country/Region constant for Norwegian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0014
            ///Culture Name: no
            ///</remarks>
            public const string Norwegian = "Norwegian";
            ///<summary>
            ///Country/Region constant for Norwegian (Bokmål) - Norway.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0414
            ///Culture Name: nb-NO
            ///</remarks>
            public const string Norwegian_Bokmål_Norway = "Norwegian (Bokmål) - Norway";
            ///<summary>
            ///Country/Region constant for Norwegian (Nynorsk) - Norway.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0814
            ///Culture Name: nn-NO
            ///</remarks>
            public const string Norwegian_Nynorsk_Norway = "Norwegian (Nynorsk) - Norway";
            ///<summary>
            ///Country/Region constant for Polish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0015
            ///Culture Name: pl
            ///</remarks>
            public const string Polish = "Polish";
            ///<summary>
            ///Country/Region constant for Polish - Poland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0415
            ///Culture Name: pl-PL
            ///</remarks>
            public const string Polish_Poland = "Polish - Poland";
            ///<summary>
            ///Country/Region constant for Portuguese.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0016
            ///Culture Name: pt
            ///</remarks>
            public const string Portuguese = "Portuguese";
            ///<summary>
            ///Country/Region constant for Portuguese - Brazil.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0416
            ///Culture Name: pt-BR
            ///</remarks>
            public const string Portuguese_Brazil = "Portuguese - Brazil";
            ///<summary>
            ///Country/Region constant for Portuguese - Portugal.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0816
            ///Culture Name: pt-PT
            ///</remarks>
            public const string Portuguese_Portugal = "Portuguese - Portugal";
            ///<summary>
            ///Country/Region constant for Punjabi.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0046
            ///Culture Name: pa
            ///</remarks>
            public const string Punjabi = "Punjabi";
            ///<summary>
            ///Country/Region constant for Punjabi - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0446
            ///Culture Name: pa-IN
            ///</remarks>
            public const string Punjabi_India = "Punjabi - India";
            ///<summary>
            ///Country/Region constant for Romanian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0018
            ///Culture Name: ro
            ///</remarks>
            public const string Romanian = "Romanian";
            ///<summary>
            ///Country/Region constant for Romanian - Romania.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0418
            ///Culture Name: ro-RO
            ///</remarks>
            public const string Romanian_Romania = "Romanian - Romania";
            ///<summary>
            ///Country/Region constant for Russian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0019
            ///Culture Name: ru
            ///</remarks>
            public const string Russian = "Russian";
            ///<summary>
            ///Country/Region constant for Russian - Russia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0419
            ///Culture Name: ru-RU
            ///</remarks>
            public const string Russian_Russia = "Russian - Russia";
            ///<summary>
            ///Country/Region constant for Sanskrit.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x004F
            ///Culture Name: sa
            ///</remarks>
            public const string Sanskrit = "Sanskrit";
            ///<summary>
            ///Country/Region constant for Sanskrit - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x044F
            ///Culture Name: sa-IN
            ///</remarks>
            public const string Sanskrit_India = "Sanskrit - India";
            ///<summary>
            ///Country/Region constant for Serbian (Cyrillic) - Serbia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C1A
            ///Culture Name: sr-SP-Cyrl
            ///</remarks>
            public const string Serbian_Cyrillic_Serbia = "Serbian (Cyrillic) - Serbia";
            ///<summary>
            ///Country/Region constant for Serbian (Latin) - Serbia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x081A
            ///Culture Name: sr-SP-Latn
            ///</remarks>
            public const string Serbian_Latin_Serbia = "Serbian (Latin) - Serbia";
            ///<summary>
            ///Country/Region constant for Slovak.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001B
            ///Culture Name: sk
            ///</remarks>
            public const string Slovak = "Slovak";
            ///<summary>
            ///Country/Region constant for Slovak - Slovakia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041B
            ///Culture Name: sk-SK
            ///</remarks>
            public const string Slovak_Slovakia = "Slovak - Slovakia";
            ///<summary>
            ///Country/Region constant for Slovenian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0024
            ///Culture Name: sl
            ///</remarks>
            public const string Slovenian = "Slovenian";
            ///<summary>
            ///Country/Region constant for Slovenian - Slovenia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0424
            ///Culture Name: sl-SI
            ///</remarks>
            public const string Slovenian_Slovenia = "Slovenian - Slovenia";
            ///<summary>
            ///Country/Region constant for Spanish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x000A
            ///Culture Name: es
            ///</remarks>
            public const string Spanish = "Spanish";
            ///<summary>
            ///Country/Region constant for Spanish - Argentina.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2C0A
            ///Culture Name: es-AR
            ///</remarks>
            public const string Spanish_Argentina = "Spanish - Argentina";
            ///<summary>
            ///Country/Region constant for Spanish - Bolivia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x400A
            ///Culture Name: es-BO
            ///</remarks>
            public const string Spanish_Bolivia = "Spanish - Bolivia";
            ///<summary>
            ///Country/Region constant for Spanish - Chile.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x340A
            ///Culture Name: es-CL
            ///</remarks>
            public const string Spanish_Chile = "Spanish - Chile";
            ///<summary>
            ///Country/Region constant for Spanish - Colombia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x240A
            ///Culture Name: es-CO
            ///</remarks>
            public const string Spanish_Colombia = "Spanish - Colombia";
            ///<summary>
            ///Country/Region constant for Spanish - Costa Rica.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x140A
            ///Culture Name: es-CR
            ///</remarks>
            public const string Spanish_CostaRica = "Spanish - Costa Rica";
            ///<summary>
            ///Country/Region constant for Spanish - Dominican Republic.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C0A
            ///Culture Name: es-DO
            ///</remarks>
            public const string Spanish_DominicanRepublic = "Spanish - Dominican Republic";
            ///<summary>
            ///Country/Region constant for Spanish - Ecuador.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x300A
            ///Culture Name: es-EC
            ///</remarks>
            public const string Spanish_Ecuador = "Spanish - Ecuador";
            ///<summary>
            ///Country/Region constant for Spanish - El Salvador.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x440A
            ///Culture Name: es-SV
            ///</remarks>
            public const string Spanish_ElSalvador = "Spanish - El Salvador";
            ///<summary>
            ///Country/Region constant for Spanish - Guatemala.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x100A
            ///Culture Name: es-GT
            ///</remarks>
            public const string Spanish_Guatemala = "Spanish - Guatemala";
            ///<summary>
            ///Country/Region constant for Spanish - Honduras.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x480A
            ///Culture Name: es-HN
            ///</remarks>
            public const string Spanish_Honduras = "Spanish - Honduras";
            ///<summary>
            ///Country/Region constant for Spanish - Mexico.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x080A
            ///Culture Name: es-MX
            ///</remarks>
            public const string Spanish_Mexico = "Spanish - Mexico";
            ///<summary>
            ///Country/Region constant for Spanish - Nicaragua.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x4C0A
            ///Culture Name: es-NI
            ///</remarks>
            public const string Spanish_Nicaragua = "Spanish - Nicaragua";
            ///<summary>
            ///Country/Region constant for Spanish - Panama.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x180A
            ///Culture Name: es-PA
            ///</remarks>
            public const string Spanish_Panama = "Spanish - Panama";
            ///<summary>
            ///Country/Region constant for Spanish - Paraguay.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3C0A
            ///Culture Name: es-PY
            ///</remarks>
            public const string Spanish_Paraguay = "Spanish - Paraguay";
            ///<summary>
            ///Country/Region constant for Spanish - Peru.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x280A
            ///Culture Name: es-PE
            ///</remarks>
            public const string Spanish_Peru = "Spanish - Peru";
            ///<summary>
            ///Country/Region constant for Spanish - Puerto Rico.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x500A
            ///Culture Name: es-PR
            ///</remarks>
            public const string Spanish_PuertoRico = "Spanish - Puerto Rico";
            ///<summary>
            ///Country/Region constant for Spanish - Spain.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C0A
            ///Culture Name: es-ES
            ///</remarks>
            public const string Spanish_Spain = "Spanish - Spain";
            ///<summary>
            ///Country/Region constant for Spanish - Uruguay.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x380A
            ///Culture Name: es-UY
            ///</remarks>
            public const string Spanish_Uruguay = "Spanish - Uruguay";
            ///<summary>
            ///Country/Region constant for Spanish - Venezuela.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x200A
            ///Culture Name: es-VE
            ///</remarks>
            public const string Spanish_Venezuela = "Spanish - Venezuela";
            ///<summary>
            ///Country/Region constant for Swahili.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0041
            ///Culture Name: sw
            ///</remarks>
            public const string Swahili = "Swahili";
            ///<summary>
            ///Country/Region constant for Swahili - Kenya.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0441
            ///Culture Name: sw-KE
            ///</remarks>
            public const string Swahili_Kenya = "Swahili - Kenya";
            ///<summary>
            ///Country/Region constant for Swedish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001D
            ///Culture Name: sv
            ///</remarks>
            public const string Swedish = "Swedish";
            ///<summary>
            ///Country/Region constant for Swedish - Finland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x081D
            ///Culture Name: sv-FI
            ///</remarks>
            public const string Swedish_Finland = "Swedish - Finland";
            ///<summary>
            ///Country/Region constant for Swedish - Sweden.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041D
            ///Culture Name: sv-SE
            ///</remarks>
            public const string Swedish_Sweden = "Swedish - Sweden";
            ///<summary>
            ///Country/Region constant for Syriac.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x005A
            ///Culture Name: syr
            ///</remarks>
            public const string Syriac = "Syriac";
            ///<summary>
            ///Country/Region constant for Syriac - Syria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x045A
            ///Culture Name: syr-SY
            ///</remarks>
            public const string Syriac_Syria = "Syriac - Syria";
            ///<summary>
            ///Country/Region constant for Tamil.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0049
            ///Culture Name: ta
            ///</remarks>
            public const string Tamil = "Tamil";
            ///<summary>
            ///Country/Region constant for Tamil - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0449
            ///Culture Name: ta-IN
            ///</remarks>
            public const string Tamil_India = "Tamil - India";
            ///<summary>
            ///Country/Region constant for Tatar.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0044
            ///Culture Name: tt
            ///</remarks>
            public const string Tatar = "Tatar";
            ///<summary>
            ///Country/Region constant for Tatar - Russia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0444
            ///Culture Name: tt-RU
            ///</remarks>
            public const string Tatar_Russia = "Tatar - Russia";
            ///<summary>
            ///Country/Region constant for Telugu.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x004A
            ///Culture Name: te
            ///</remarks>
            public const string Telugu = "Telugu";
            ///<summary>
            ///Country/Region constant for Telugu - India.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x044A
            ///Culture Name: te-IN
            ///</remarks>
            public const string Telugu_India = "Telugu - India";
            ///<summary>
            ///Country/Region constant for Thai.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001E
            ///Culture Name: th
            ///</remarks>
            public const string Thai = "Thai";
            ///<summary>
            ///Country/Region constant for Thai - Thailand.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041E
            ///Culture Name: th-TH
            ///</remarks>
            public const string Thai_Thailand = "Thai - Thailand";
            ///<summary>
            ///Country/Region constant for Turkish.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x001F
            ///Culture Name: tr
            ///</remarks>
            public const string Turkish = "Turkish";
            ///<summary>
            ///Country/Region constant for Turkish - Turkey.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041F
            ///Culture Name: tr-TR
            ///</remarks>
            public const string Turkish_Turkey = "Turkish - Turkey";
            ///<summary>
            ///Country/Region constant for Ukrainian.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0022
            ///Culture Name: uk
            ///</remarks>
            public const string Ukrainian = "Ukrainian";
            ///<summary>
            ///Country/Region constant for Ukrainian - Ukraine.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0422
            ///Culture Name: uk-UA
            ///</remarks>
            public const string Ukrainian_Ukraine = "Ukrainian - Ukraine";
            ///<summary>
            ///Country/Region constant for Urdu.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0020
            ///Culture Name: ur
            ///</remarks>
            public const string Urdu = "Urdu";
            ///<summary>
            ///Country/Region constant for Urdu - Pakistan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0420
            ///Culture Name: ur-PK
            ///</remarks>
            public const string Urdu_Pakistan = "Urdu - Pakistan";
            ///<summary>
            ///Country/Region constant for Uzbek.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0043
            ///Culture Name: uz
            ///</remarks>
            public const string Uzbek = "Uzbek";
            ///<summary>
            ///Country/Region constant for Uzbek (Cyrillic) - Uzbekistan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0843
            ///Culture Name: uz-UZ-Cyrl
            ///</remarks>
            public const string Uzbek_Cyrillic_Uzbekistan = "Uzbek (Cyrillic) - Uzbekistan";
            ///<summary>
            ///Country/Region constant for Uzbek (Latin) - Uzbekistan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0443
            ///Culture Name: uz-UZ-Latn
            ///</remarks>
            public const string Uzbek_Latin_Uzbekistan = "Uzbek (Latin) - Uzbekistan";
            ///<summary>
            ///Country/Region constant for Vietnamese.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x002A
            ///Culture Name: vi
            ///</remarks>
            public const string Vietnamese = "Vietnamese";
            ///<summary>
            ///Country/Region constant for Vietnamese - Vietnam.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042A
            ///Culture Name: vi-VN
            ///</remarks>
            public const string Vietnamese_Vietnam = "Vietnamese - Vietnam";
            //
        }
    }
}