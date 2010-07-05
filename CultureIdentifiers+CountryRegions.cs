using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Oilexer
{
    partial class CultureIdentifiers
    {
        /// <summary>
        /// Provides a series of constant string values that relate to the culture information and
        /// the string description of known countries/regions.
        /// </summary>
        /* *
         * Visible by design in cases where users of the code are
         * more interested in the country region names, than the
         * culture identifiers.
         * */
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Algeria = "Arabic - Algeria";
            ///<summary>
            ///Country/Region constant for Arabic - Bahrain.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3C01
            ///Culture Name: ar-BH
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Bahrain = "Arabic - Bahrain";
            ///<summary>
            ///Country/Region constant for Arabic - Egypt.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C01
            ///Culture Name: ar-EG
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Egypt = "Arabic - Egypt";
            ///<summary>
            ///Country/Region constant for Arabic - Iraq.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0801
            ///Culture Name: ar-IQ
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Iraq = "Arabic - Iraq";
            ///<summary>
            ///Country/Region constant for Arabic - Jordan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2C01
            ///Culture Name: ar-JO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Jordan = "Arabic - Jordan";
            ///<summary>
            ///Country/Region constant for Arabic - Kuwait.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3401
            ///Culture Name: ar-KW
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Kuwait = "Arabic - Kuwait";
            ///<summary>
            ///Country/Region constant for Arabic - Lebanon.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3001
            ///Culture Name: ar-LB
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Lebanon = "Arabic - Lebanon";
            ///<summary>
            ///Country/Region constant for Arabic - Libya.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1001
            ///Culture Name: ar-LY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Libya = "Arabic - Libya";
            ///<summary>
            ///Country/Region constant for Arabic - Morocco.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1801
            ///Culture Name: ar-MA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Morocco = "Arabic - Morocco";
            ///<summary>
            ///Country/Region constant for Arabic - Oman.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2001
            ///Culture Name: ar-OM
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Oman = "Arabic - Oman";
            ///<summary>
            ///Country/Region constant for Arabic - Qatar.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x4001
            ///Culture Name: ar-QA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Qatar = "Arabic - Qatar";
            ///<summary>
            ///Country/Region constant for Arabic - Saudi Arabia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0401
            ///Culture Name: ar-SA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_SaudiArabia = "Arabic - Saudi Arabia";
            ///<summary>
            ///Country/Region constant for Arabic - Syria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2801
            ///Culture Name: ar-SY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Syria = "Arabic - Syria";
            ///<summary>
            ///Country/Region constant for Arabic - Tunisia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C01
            ///Culture Name: ar-TN
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_Tunisia = "Arabic - Tunisia";
            ///<summary>
            ///Country/Region constant for Arabic - United Arab Emirates.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3801
            ///Culture Name: ar-AE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Arabic_UnitedArabEmirates = "Arabic - United Arab Emirates";
            ///<summary>
            ///Country/Region constant for Arabic - Yemen.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2401
            ///Culture Name: ar-YE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Azeri_Cyrillic_Azerbaijan = "Azeri (Cyrillic) - Azerbaijan";
            ///<summary>
            ///Country/Region constant for Azeri (Latin) - Azerbaijan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x042C
            ///Culture Name: az-AZ-Latn
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Catalan_Catalan = "Catalan - Catalan";
            ///<summary>
            ///Country/Region constant for Chinese - Hong Kong SAR.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C04
            ///Culture Name: zh-HK
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SAR")]
            public const string Chinese_HongKongSAR = "Chinese - Hong Kong SAR";
            ///<summary>
            ///Country/Region constant for Chinese - Macao SAR.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1404
            ///Culture Name: zh-MO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SAR")]
            public const string Chinese_MacaoSAR = "Chinese - Macao SAR";
            ///<summary>
            ///Country/Region constant for Chinese - China.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0804
            ///Culture Name: zh-CN
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Chinese_China = "Chinese - China";
            ///<summary>
            ///Country/Region constant for Chinese (Simplified).
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0004
            ///Culture Name: zh-CHS
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Chinese_Simplified = "Chinese (Simplified)";
            ///<summary>
            ///Country/Region constant for Chinese - Singapore.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1004
            ///Culture Name: zh-SG
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Chinese_Singapore = "Chinese - Singapore";
            ///<summary>
            ///Country/Region constant for Chinese - Taiwan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0404
            ///Culture Name: zh-TW
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Chinese_Taiwan = "Chinese - Taiwan";
            ///<summary>
            ///Country/Region constant for Chinese (Traditional).
            ///</summary>
            ///<remarks>
            ///CultureID: 0x7C04
            ///Culture Name: zh-CHT
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Dutch_Belgium = "Dutch - Belgium";
            ///<summary>
            ///Country/Region constant for Dutch - The Netherlands.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0413
            ///Culture Name: nl-NL
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Australia = "English - Australia";
            ///<summary>
            ///Country/Region constant for English - Belize.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2809
            ///Culture Name: en-BZ
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Belize = "English - Belize";
            ///<summary>
            ///Country/Region constant for English - Canada.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1009
            ///Culture Name: en-CA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Canada = "English - Canada";
            ///<summary>
            ///Country/Region constant for English - Caribbean.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2409
            ///Culture Name: en-CB
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Caribbean = "English - Caribbean";
            ///<summary>
            ///Country/Region constant for English - Ireland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1809
            ///Culture Name: en-IE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Ireland = "English - Ireland";
            ///<summary>
            ///Country/Region constant for English - Jamaica.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2009
            ///Culture Name: en-JM
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Jamaica = "English - Jamaica";
            ///<summary>
            ///Country/Region constant for English - New Zealand.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1409
            ///Culture Name: en-NZ
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_NewZealand = "English - New Zealand";
            ///<summary>
            ///Country/Region constant for English - Philippines.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3409
            ///Culture Name: en-PH
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_Philippines = "English - Philippines";
            ///<summary>
            ///Country/Region constant for English - South Africa.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C09
            ///Culture Name: en-ZA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_SouthAfrica = "English - South Africa";
            ///<summary>
            ///Country/Region constant for English - Trinidad and Tobago.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x2C09
            ///Culture Name: en-TT
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_TrinidadAndTobago = "English - Trinidad and Tobago";
            ///<summary>
            ///Country/Region constant for English - United Kingdom.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0809
            ///Culture Name: en-GB
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_UnitedKingdom = "English - United Kingdom";
            ///<summary>
            ///Country/Region constant for English - United States.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0409
            ///Culture Name: en-US
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string English_UnitedStates = "English - United States";
            ///<summary>
            ///Country/Region constant for English - Zimbabwe.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3009
            ///Culture Name: en-ZW
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string French_Belgium = "French - Belgium";
            ///<summary>
            ///Country/Region constant for French - Canada.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C0C
            ///Culture Name: fr-CA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string French_Canada = "French - Canada";
            ///<summary>
            ///Country/Region constant for French - France.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x040C
            ///Culture Name: fr-FR
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string French_France = "French - France";
            ///<summary>
            ///Country/Region constant for French - Luxembourg.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x140C
            ///Culture Name: fr-LU
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string French_Luxembourg = "French - Luxembourg";
            ///<summary>
            ///Country/Region constant for French - Monaco.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x180C
            ///Culture Name: fr-MC
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string French_Monaco = "French - Monaco";
            ///<summary>
            ///Country/Region constant for French - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x100C
            ///Culture Name: fr-CH
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string German_Austria = "German - Austria";
            ///<summary>
            ///Country/Region constant for German - Germany.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0407
            ///Culture Name: de-DE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string German_Germany = "German - Germany";
            ///<summary>
            ///Country/Region constant for German - Liechtenstein.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1407
            ///Culture Name: de-LI
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string German_Liechtenstein = "German - Liechtenstein";
            ///<summary>
            ///Country/Region constant for German - Luxembourg.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1007
            ///Culture Name: de-LU
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string German_Luxembourg = "German - Luxembourg";
            ///<summary>
            ///Country/Region constant for German - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0807
            ///Culture Name: de-CH
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Italian_Italy = "Italian - Italy";
            ///<summary>
            ///Country/Region constant for Italian - Switzerland.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0810
            ///Culture Name: it-CH
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Malay_Brunei = "Malay - Brunei";
            ///<summary>
            ///Country/Region constant for Malay - Malaysia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x043E
            ///Culture Name: ms-MY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Norwegian_Bokmål_Norway = "Norwegian (Bokmål) - Norway";
            ///<summary>
            ///Country/Region constant for Norwegian (Nynorsk) - Norway.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0814
            ///Culture Name: nn-NO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Portuguese_Brazil = "Portuguese - Brazil";
            ///<summary>
            ///Country/Region constant for Portuguese - Portugal.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0816
            ///Culture Name: pt-PT
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Sanskrit_India = "Sanskrit - India";
            ///<summary>
            ///Country/Region constant for Serbian (Cyrillic) - Serbia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C1A
            ///Culture Name: sr-SP-Cyrl
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Serbian_Cyrillic_Serbia = "Serbian (Cyrillic) - Serbia";
            ///<summary>
            ///Country/Region constant for Serbian (Latin) - Serbia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x081A
            ///Culture Name: sr-SP-Latn
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Argentina = "Spanish - Argentina";
            ///<summary>
            ///Country/Region constant for Spanish - Bolivia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x400A
            ///Culture Name: es-BO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Bolivia = "Spanish - Bolivia";
            ///<summary>
            ///Country/Region constant for Spanish - Chile.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x340A
            ///Culture Name: es-CL
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Chile = "Spanish - Chile";
            ///<summary>
            ///Country/Region constant for Spanish - Colombia.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x240A
            ///Culture Name: es-CO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Colombia = "Spanish - Colombia";
            ///<summary>
            ///Country/Region constant for Spanish - Costa Rica.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x140A
            ///Culture Name: es-CR
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_CostaRica = "Spanish - Costa Rica";
            ///<summary>
            ///Country/Region constant for Spanish - Dominican Republic.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x1C0A
            ///Culture Name: es-DO
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_DominicanRepublic = "Spanish - Dominican Republic";
            ///<summary>
            ///Country/Region constant for Spanish - Ecuador.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x300A
            ///Culture Name: es-EC
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Ecuador = "Spanish - Ecuador";
            ///<summary>
            ///Country/Region constant for Spanish - El Salvador.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x440A
            ///Culture Name: es-SV
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "El")]
            public const string Spanish_ElSalvador = "Spanish - El Salvador";
            ///<summary>
            ///Country/Region constant for Spanish - Guatemala.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x100A
            ///Culture Name: es-GT
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Guatemala = "Spanish - Guatemala";
            ///<summary>
            ///Country/Region constant for Spanish - Honduras.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x480A
            ///Culture Name: es-HN
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Honduras = "Spanish - Honduras";
            ///<summary>
            ///Country/Region constant for Spanish - Mexico.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x080A
            ///Culture Name: es-MX
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Mexico = "Spanish - Mexico";
            ///<summary>
            ///Country/Region constant for Spanish - Nicaragua.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x4C0A
            ///Culture Name: es-NI
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Nicaragua = "Spanish - Nicaragua";
            ///<summary>
            ///Country/Region constant for Spanish - Panama.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x180A
            ///Culture Name: es-PA
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Panama = "Spanish - Panama";
            ///<summary>
            ///Country/Region constant for Spanish - Paraguay.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x3C0A
            ///Culture Name: es-PY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Paraguay = "Spanish - Paraguay";
            ///<summary>
            ///Country/Region constant for Spanish - Peru.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x280A
            ///Culture Name: es-PE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Peru = "Spanish - Peru";
            ///<summary>
            ///Country/Region constant for Spanish - Puerto Rico.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x500A
            ///Culture Name: es-PR
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_PuertoRico = "Spanish - Puerto Rico";
            ///<summary>
            ///Country/Region constant for Spanish - Spain.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0C0A
            ///Culture Name: es-ES
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Spain = "Spanish - Spain";
            ///<summary>
            ///Country/Region constant for Spanish - Uruguay.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x380A
            ///Culture Name: es-UY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Spanish_Uruguay = "Spanish - Uruguay";
            ///<summary>
            ///Country/Region constant for Spanish - Venezuela.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x200A
            ///Culture Name: es-VE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Swedish_Finland = "Swedish - Finland";
            ///<summary>
            ///Country/Region constant for Swedish - Sweden.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x041D
            ///Culture Name: sv-SE
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Swedish_Sweden = "Swedish - Sweden";
            ///<summary>
            ///Country/Region constant for Syriac.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x005A
            ///Culture Name: syr
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Syriac")]
            public const string Syriac = "Syriac";
            ///<summary>
            ///Country/Region constant for Syriac - Syria.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x045A
            ///Culture Name: syr-SY
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Syriac")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Uzbek_Cyrillic_Uzbekistan = "Uzbek (Cyrillic) - Uzbekistan";
            ///<summary>
            ///Country/Region constant for Uzbek (Latin) - Uzbekistan.
            ///</summary>
            ///<remarks>
            ///CultureID: 0x0443
            ///Culture Name: uz-UZ-Latn
            ///</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const string Vietnamese_Vietnam = "Vietnamese - Vietnam";
        }
    }
}
