using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Translation
{
    partial class IntermediateCodeTranslator
    {
        private class __HTMLFormatter :
            IIntermediateCodeTranslatorFormatter
        {
            private static Dictionary<TranslatorFormatterMemberType, string> memberTypeColorTable = new Dictionary<TranslatorFormatterMemberType, string>();

            static __HTMLFormatter()
            {
                foreach (KeyValuePair<TranslatorFormatterMemberType, string> kvpItem in
                    new KeyValuePair<TranslatorFormatterMemberType, string>[] 
                        { 
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Method, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.MethodSignature, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Property, "#FF7070"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.PropertySignature, "#FF7070"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Event, "#70FF70"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.EventSignature, "#70FF70") ,
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Field, "#C070C0"), 
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Parameter, "#808080"), 
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Local, "#008080") 
                        })
                {
                    ((ICollection<KeyValuePair<TranslatorFormatterMemberType, string>>)(memberTypeColorTable)).Add(kvpItem);
                }
            }

            #region IIntermediateCodeTranslatorFormatter Members

            public string FormatKeywordToken(string keywordToken)
            {
                return string.Format("<span style=\"color:blue;font-family:Courier New;\">{0}</span>", keywordToken);
            }

            public string FormatNameSpace(string nameSpacePath)
            {
                return string.Format("<span style=\"color:#808080;font-family:Courier New;\">{0}</span>", nameSpacePath);
            }

            public string FormatTypeNameToken(string identifierToken, IType type)
            {
                string color = "";
                bool bold = false;
                if (type.IsDelegate)
                {
                    color = "DarkCyan";
                }
                else if (type.IsClass)
                {
                    color = "purple";
                    bold = true;
                }
                else if (type.IsInterface)
                {
                    color = "Magenta";
                }
                else if (type.IsEnumerator)
                {
                    color = "#C08080";
                    bold = true;
                }
                else if (type.IsStructure)
                {
                    color = "Dark Yellow";
                }
                if (bold)
                    return string.Format("<span style=\"color:{0};font-family:Courier New;font-weight:bolder;\">{1}</span>", color, identifierToken);
                else
                    return string.Format("<span style=\"color:{0};font-family:Courier New;\">{1}</span>", color, identifierToken);
            }

            public string FormatCommentToken(string commentToken)
            {
                return string.Format("<span style=\"color:green;font-family:Courier New;\">{0}</span>", commentToken.Replace("<", "&lt;").Replace(">", "&gt;").Replace(" ", "&nbsp;"));
            }

            public string FormatStringToken(string strToken)
            {
                return string.Format("<span style=\"color:green;font-family:Courier New;\">{0}</span>", strToken);
            }

            public string FormatOperatorToken(string oprToken)
            {
                return string.Format("<span style=\"color:#C080C0;font-family:Courier New;\">{0}</span>", oprToken);
            }

            public string FormatNumberToken(string numberToken)
            {
                return string.Format("<span style=\"color:red;font-family:Courier New;\">{0}</span>", numberToken);
            }

            public string FormatOtherToken(string otherToken)
            {
                if (otherToken.Trim() == string.Empty)
                    return otherToken.Replace(" ", "&nbsp;");
                else
                    return string.Format("<span style=\"color:black;font-family:Courier New;\">{0}</span>", otherToken.Replace(" ", "&nbsp;"));

            }

            public string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType, IType parent)
            {
                bool italic = false;


                italic = ((memberType == TranslatorFormatterMemberType.MethodSignature) || (memberType == TranslatorFormatterMemberType.PropertySignature) || (memberType == TranslatorFormatterMemberType.EventSignature));

                if (italic)
                    return string.Format("<span style=\"color:{0};font-family:Courier New;text-decoration:italic\">{1}</span>", memberTypeColorTable[memberType], memberToken);
                else
                    return string.Format("<span style=\"color:{0};font-family:Courier New;\">{1}</span>", memberTypeColorTable[memberType], memberToken);
            }

            public string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType)
            {
                //string color = memberTypeColorTable[memberType];
                return string.Format("<span style=\"color:{0};font-family:Courier New;\">{1}</span>", memberTypeColorTable[memberType], memberToken);
            }

            public string DenoteNewLine()
            {
                return "<br/>";
            }

            #endregion
        }
    }
}