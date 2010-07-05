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
        /// Provides a series of constant int values that relate to the culture information and the 
        /// numerical identifiers of the cultures.
        /// </summary>
        /* *
         * Visible by design in cases where users of the code are
         * more interested in the numerical id associated to cultures, 
         * than the full culture identifiers.
         * */
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class NumericIdentifiers
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Algeria = 0x1401;
            ///<summary>
            ///Culture identifier constant for Arabic - Bahrain.
            ///</summary>
            ///<remarks>Culture ID: 0x3C01
            ///Culture Name:ar-BH</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Bahrain = 0x3C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Egypt.
            ///</summary>
            ///<remarks>Culture ID: 0x0C01
            ///Culture Name:ar-EG</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Egypt = 0x0C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Iraq.
            ///</summary>
            ///<remarks>Culture ID: 0x0801
            ///Culture Name:ar-IQ</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Iraq = 0x0801;
            ///<summary>
            ///Culture identifier constant for Arabic - Jordan.
            ///</summary>
            ///<remarks>Culture ID: 0x2C01
            ///Culture Name:ar-JO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Jordan = 0x2C01;
            ///<summary>
            ///Culture identifier constant for Arabic - Kuwait.
            ///</summary>
            ///<remarks>Culture ID: 0x3401
            ///Culture Name:ar-KW</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Kuwait = 0x3401;
            ///<summary>
            ///Culture identifier constant for Arabic - Lebanon.
            ///</summary>
            ///<remarks>Culture ID: 0x3001
            ///Culture Name:ar-LB</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Lebanon = 0x3001;
            ///<summary>
            ///Culture identifier constant for Arabic - Libya.
            ///</summary>
            ///<remarks>Culture ID: 0x1001
            ///Culture Name:ar-LY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Libya = 0x1001;
            ///<summary>
            ///Culture identifier constant for Arabic - Morocco.
            ///</summary>
            ///<remarks>Culture ID: 0x1801
            ///Culture Name:ar-MA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Morocco = 0x1801;
            ///<summary>
            ///Culture identifier constant for Arabic - Oman.
            ///</summary>
            ///<remarks>Culture ID: 0x2001
            ///Culture Name:ar-OM</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Oman = 0x2001;
            ///<summary>
            ///Culture identifier constant for Arabic - Qatar.
            ///</summary>
            ///<remarks>Culture ID: 0x4001
            ///Culture Name:ar-QA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Qatar = 0x4001;
            ///<summary>
            ///Culture identifier constant for Arabic - Saudi Arabia.
            ///</summary>
            ///<remarks>Culture ID: 0x0401
            ///Culture Name:ar-SA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_SaudiArabia = 0x0401;
            ///<summary>
            ///Culture identifier constant for Arabic - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x2801
            ///Culture Name:ar-SY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Syria = 0x2801;
            ///<summary>
            ///Culture identifier constant for Arabic - Tunisia.
            ///</summary>
            ///<remarks>Culture ID: 0x1C01
            ///Culture Name:ar-TN</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_Tunisia = 0x1C01;
            ///<summary>
            ///Culture identifier constant for Arabic - United Arab Emirates.
            ///</summary>
            ///<remarks>Culture ID: 0x3801
            ///Culture Name:ar-AE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Arabic_UnitedArabEmirates = 0x3801;
            ///<summary>
            ///Culture identifier constant for Arabic - Yemen.
            ///</summary>
            ///<remarks>Culture ID: 0x2401
            ///Culture Name:ar-YE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Azeri_Cyrillic_Azerbaijan = 0x082C;
            ///<summary>
            ///Culture identifier constant for Azeri (Latin) - Azerbaijan.
            ///</summary>
            ///<remarks>Culture ID: 0x042C
            ///Culture Name:az-AZ-Latn</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Catalan_Catalan = 0x0403;
            ///<summary>
            ///Culture identifier constant for Chinese - Hong Kong SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x0C04
            ///Culture Name:zh-HK</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SAR")]
            public const int Chinese_HongKongSAR = 0x0C04;
            ///<summary>
            ///Culture identifier constant for Chinese - Macao SAR.
            ///</summary>
            ///<remarks>Culture ID: 0x1404
            ///Culture Name:zh-MO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SAR")]
            public const int Chinese_MacaoSAR = 0x1404;
            ///<summary>
            ///Culture identifier constant for Chinese - China.
            ///</summary>
            ///<remarks>Culture ID: 0x0804
            ///Culture Name:zh-CN</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Chinese_China = 0x0804;
            ///<summary>
            ///Culture identifier constant for Chinese (Simplified).
            ///</summary>
            ///<remarks>Culture ID: 0x0004
            ///Culture Name:zh-CHS</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Chinese_Simplified = 0x0004;
            ///<summary>
            ///Culture identifier constant for Chinese - Singapore.
            ///</summary>
            ///<remarks>Culture ID: 0x1004
            ///Culture Name:zh-SG</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Chinese_Singapore = 0x1004;
            ///<summary>
            ///Culture identifier constant for Chinese - Taiwan.
            ///</summary>
            ///<remarks>Culture ID: 0x0404
            ///Culture Name:zh-TW</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Chinese_Taiwan = 0x0404;
            ///<summary>
            ///Culture identifier constant for Chinese (Traditional).
            ///</summary>
            ///<remarks>Culture ID: 0x7C04
            ///Culture Name:zh-CHT</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Dutch_Belgium = 0x0813;
            ///<summary>
            ///Culture identifier constant for Dutch - The Netherlands.
            ///</summary>
            ///<remarks>Culture ID: 0x0413
            ///Culture Name:nl-NL</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Australia = 0x0C09;
            ///<summary>
            ///Culture identifier constant for English - Belize.
            ///</summary>
            ///<remarks>Culture ID: 0x2809
            ///Culture Name:en-BZ</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Belize = 0x2809;
            ///<summary>
            ///Culture identifier constant for English - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x1009
            ///Culture Name:en-CA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Canada = 0x1009;
            ///<summary>
            ///Culture identifier constant for English - Caribbean.
            ///</summary>
            ///<remarks>Culture ID: 0x2409
            ///Culture Name:en-CB</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Caribbean = 0x2409;
            ///<summary>
            ///Culture identifier constant for English - Ireland.
            ///</summary>
            ///<remarks>Culture ID: 0x1809
            ///Culture Name:en-IE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Ireland = 0x1809;
            ///<summary>
            ///Culture identifier constant for English - Jamaica.
            ///</summary>
            ///<remarks>Culture ID: 0x2009
            ///Culture Name:en-JM</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Jamaica = 0x2009;
            ///<summary>
            ///Culture identifier constant for English - New Zealand.
            ///</summary>
            ///<remarks>Culture ID: 0x1409
            ///Culture Name:en-NZ</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_NewZealand = 0x1409;
            ///<summary>
            ///Culture identifier constant for English - Philippines.
            ///</summary>
            ///<remarks>Culture ID: 0x3409
            ///Culture Name:en-PH</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_Philippines = 0x3409;
            ///<summary>
            ///Culture identifier constant for English - South Africa.
            ///</summary>
            ///<remarks>Culture ID: 0x1C09
            ///Culture Name:en-ZA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_SouthAfrica = 0x1C09;
            ///<summary>
            ///Culture identifier constant for English - Trinidad and Tobago.
            ///</summary>
            ///<remarks>Culture ID: 0x2C09
            ///Culture Name:en-TT</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_TrinidadAndTobago = 0x2C09;
            ///<summary>
            ///Culture identifier constant for English - United Kingdom.
            ///</summary>
            ///<remarks>Culture ID: 0x0809
            ///Culture Name:en-GB</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_UnitedKingdom = 0x0809;
            ///<summary>
            ///Culture identifier constant for English - United States.
            ///</summary>
            ///<remarks>Culture ID: 0x0409
            ///Culture Name:en-US</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int English_UnitedStates = 0x0409;
            ///<summary>
            ///Culture identifier constant for English - Zimbabwe.
            ///</summary>
            ///<remarks>Culture ID: 0x3009
            ///Culture Name:en-ZW</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int French_Belgium = 0x080C;
            ///<summary>
            ///Culture identifier constant for French - Canada.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0C
            ///Culture Name:fr-CA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int French_Canada = 0x0C0C;
            ///<summary>
            ///Culture identifier constant for French - France.
            ///</summary>
            ///<remarks>Culture ID: 0x040C
            ///Culture Name:fr-FR</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int French_France = 0x040C;
            ///<summary>
            ///Culture identifier constant for French - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x140C
            ///Culture Name:fr-LU</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int French_Luxembourg = 0x140C;
            ///<summary>
            ///Culture identifier constant for French - Monaco.
            ///</summary>
            ///<remarks>Culture ID: 0x180C
            ///Culture Name:fr-MC</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int French_Monaco = 0x180C;
            ///<summary>
            ///Culture identifier constant for French - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x100C
            ///Culture Name:fr-CH</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int German_Austria = 0x0C07;
            ///<summary>
            ///Culture identifier constant for German - Germany.
            ///</summary>
            ///<remarks>Culture ID: 0x0407
            ///Culture Name:de-DE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int German_Germany = 0x0407;
            ///<summary>
            ///Culture identifier constant for German - Liechtenstein.
            ///</summary>
            ///<remarks>Culture ID: 0x1407
            ///Culture Name:de-LI</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int German_Liechtenstein = 0x1407;
            ///<summary>
            ///Culture identifier constant for German - Luxembourg.
            ///</summary>
            ///<remarks>Culture ID: 0x1007
            ///Culture Name:de-LU</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int German_Luxembourg = 0x1007;
            ///<summary>
            ///Culture identifier constant for German - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0807
            ///Culture Name:de-CH</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Italian_Italy = 0x0410;
            ///<summary>
            ///Culture identifier constant for Italian - Switzerland.
            ///</summary>
            ///<remarks>Culture ID: 0x0810
            ///Culture Name:it-CH</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Malay_Brunei = 0x083E;
            ///<summary>
            ///Culture identifier constant for Malay - Malaysia.
            ///</summary>
            ///<remarks>Culture ID: 0x043E
            ///Culture Name:ms-MY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Norwegian_Bokmål_Norway = 0x0414;
            ///<summary>
            ///Culture identifier constant for Norwegian (Nynorsk) - Norway.
            ///</summary>
            ///<remarks>Culture ID: 0x0814
            ///Culture Name:nn-NO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Portuguese_Brazil = 0x0416;
            ///<summary>
            ///Culture identifier constant for Portuguese - Portugal.
            ///</summary>
            ///<remarks>Culture ID: 0x0816
            ///Culture Name:pt-PT</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Sanskrit_India = 0x044F;
            ///<summary>
            ///Culture identifier constant for Serbian (Cyrillic) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x0C1A
            ///Culture Name:sr-SP-Cyrl</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Serbian_Cyrillic_Serbia = 0x0C1A;
            ///<summary>
            ///Culture identifier constant for Serbian (Latin) - Serbia.
            ///</summary>
            ///<remarks>Culture ID: 0x081A
            ///Culture Name:sr-SP-Latn</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Argentina = 0x2C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Bolivia.
            ///</summary>
            ///<remarks>Culture ID: 0x400A
            ///Culture Name:es-BO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Bolivia = 0x400A;
            ///<summary>
            ///Culture identifier constant for Spanish - Chile.
            ///</summary>
            ///<remarks>Culture ID: 0x340A
            ///Culture Name:es-CL</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Chile = 0x340A;
            ///<summary>
            ///Culture identifier constant for Spanish - Colombia.
            ///</summary>
            ///<remarks>Culture ID: 0x240A
            ///Culture Name:es-CO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Colombia = 0x240A;
            ///<summary>
            ///Culture identifier constant for Spanish - Costa Rica.
            ///</summary>
            ///<remarks>Culture ID: 0x140A
            ///Culture Name:es-CR</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_CostaRica = 0x140A;
            ///<summary>
            ///Culture identifier constant for Spanish - Dominican Republic.
            ///</summary>
            ///<remarks>Culture ID: 0x1C0A
            ///Culture Name:es-DO</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_DominicanRepublic = 0x1C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Ecuador.
            ///</summary>
            ///<remarks>Culture ID: 0x300A
            ///Culture Name:es-EC</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Ecuador = 0x300A;
            ///<summary>
            ///Culture identifier constant for Spanish - El Salvador.
            ///</summary>
            ///<remarks>Culture ID: 0x440A
            ///Culture Name:es-SV</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "El")]
            public const int Spanish_ElSalvador = 0x440A;
            ///<summary>
            ///Culture identifier constant for Spanish - Guatemala.
            ///</summary>
            ///<remarks>Culture ID: 0x100A
            ///Culture Name:es-GT</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Guatemala = 0x100A;
            ///<summary>
            ///Culture identifier constant for Spanish - Honduras.
            ///</summary>
            ///<remarks>Culture ID: 0x480A
            ///Culture Name:es-HN</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Honduras = 0x480A;
            ///<summary>
            ///Culture identifier constant for Spanish - Mexico.
            ///</summary>
            ///<remarks>Culture ID: 0x080A
            ///Culture Name:es-MX</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Mexico = 0x080A;
            ///<summary>
            ///Culture identifier constant for Spanish - Nicaragua.
            ///</summary>
            ///<remarks>Culture ID: 0x4C0A
            ///Culture Name:es-NI</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Nicaragua = 0x4C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Panama.
            ///</summary>
            ///<remarks>Culture ID: 0x180A
            ///Culture Name:es-PA</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Panama = 0x180A;
            ///<summary>
            ///Culture identifier constant for Spanish - Paraguay.
            ///</summary>
            ///<remarks>Culture ID: 0x3C0A
            ///Culture Name:es-PY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Paraguay = 0x3C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Peru.
            ///</summary>
            ///<remarks>Culture ID: 0x280A
            ///Culture Name:es-PE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Peru = 0x280A;
            ///<summary>
            ///Culture identifier constant for Spanish - Puerto Rico.
            ///</summary>
            ///<remarks>Culture ID: 0x500A
            ///Culture Name:es-PR</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_PuertoRico = 0x500A;
            ///<summary>
            ///Culture identifier constant for Spanish - Spain.
            ///</summary>
            ///<remarks>Culture ID: 0x0C0A
            ///Culture Name:es-ES</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Spain = 0x0C0A;
            ///<summary>
            ///Culture identifier constant for Spanish - Uruguay.
            ///</summary>
            ///<remarks>Culture ID: 0x380A
            ///Culture Name:es-UY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Spanish_Uruguay = 0x380A;
            ///<summary>
            ///Culture identifier constant for Spanish - Venezuela.
            ///</summary>
            ///<remarks>Culture ID: 0x200A
            ///Culture Name:es-VE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Swedish_Finland = 0x081D;
            ///<summary>
            ///Culture identifier constant for Swedish - Sweden.
            ///</summary>
            ///<remarks>Culture ID: 0x041D
            ///Culture Name:sv-SE</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Swedish_Sweden = 0x041D;
            ///<summary>
            ///Culture identifier constant for Syriac.
            ///</summary>
            ///<remarks>Culture ID: 0x005A
            ///Culture Name:syr</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Syriac")]
            public const int Syriac = 0x005A;
            ///<summary>
            ///Culture identifier constant for Syriac - Syria.
            ///</summary>
            ///<remarks>Culture ID: 0x045A
            ///Culture Name:syr-SY</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Syriac")]
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Uzbek_Cyrillic_Uzbekistan = 0x0843;
            ///<summary>
            ///Culture identifier constant for Uzbek (Latin) - Uzbekistan.
            ///</summary>
            ///<remarks>Culture ID: 0x0443
            ///Culture Name:uz-UZ-Latn</remarks>
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
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
            [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
            public const int Vietnamese_Vietnam = 0x042A;
        }
    }
}
