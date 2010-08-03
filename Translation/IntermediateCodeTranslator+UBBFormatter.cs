using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Statements;

namespace Oilexer.Translation
{
    partial class IntermediateCodeTranslator
    {
        private class __UBBFormatter :
            IIntermediateCodeTranslatorFormatter
        {
            private static Dictionary<TranslatorFormatterMemberType, string> memberTypeColorTable = new Dictionary<TranslatorFormatterMemberType, string>();

            static __UBBFormatter()
            {
                foreach (KeyValuePair<TranslatorFormatterMemberType, string> kvpItem in
                    new KeyValuePair<TranslatorFormatterMemberType, string>[] 
                        { 
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Method, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.MethodSignature, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Property, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.PropertySignature, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Event, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.EventSignature, "#7070FF") ,
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Field, "#7070FF"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Parameter, "#808080") ,
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Local, "#008080") 
                        })
                {
                    ((ICollection<KeyValuePair<TranslatorFormatterMemberType, string>>)(memberTypeColorTable)).Add(kvpItem);
                }
            }

            #region IIntermediateCodeTranslatorFormatter Members

            public string FormatKeywordToken(string keywordToken)
            {
                return string.Format("[color=blue][font=courier new]{0}[/font][/color]", keywordToken);
            }

            public string FormatNameSpace(string nameSpacePath)
            {
                return string.Format("[color=#808080][font=courier new]{0}[/font][/color]", nameSpacePath);
            }

            public string FormatTypeNameToken(string identifierToken, IType type, IIntermediateCodeTranslatorOptions options, bool declarePoint)
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
                    return string.Format("[color={0}][font=courier new][b]{1}[/b][/font][/color]", color, identifierToken);
                else
                    return string.Format("[color={0}][font=courier new]{1}[/font][/color]", color, identifierToken);
            }

            public string FormatCommentToken(string commentToken)
            {
                return string.Format("[color=green][font=courier new]{0}[/font][/color]", commentToken);
            }

            public string FormatStringToken(string strToken)
            {
                return string.Format("[color=green][font=courier new]{0}[/font][/color]", strToken);
            }

            public string FormatOperatorToken(string oprToken)
            {
                return string.Format("[color=#C080C0][font=courier new]{0}[/font][/color]", oprToken);
            }

            public string FormatNumberToken(string numberToken)
            {
                return string.Format("[color=red][font=courier new]{0}[/font][/color]", numberToken);
            }

            public string FormatOtherToken(string otherToken)
            {
                if (otherToken.Trim() == string.Empty)
                    return otherToken;
                else
                    return string.Format("[color=Black][font=courier new]{0}[/font][/color]", otherToken);

            }

            public string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType, IType parent)
            {
                bool italic = false;


                italic = ((memberType == TranslatorFormatterMemberType.MethodSignature) || (memberType == TranslatorFormatterMemberType.PropertySignature) || (memberType == TranslatorFormatterMemberType.EventSignature));

                if (italic)
                    return string.Format("[color={0}][font=Courier New][i]{1}[/i][/font][/color]", memberTypeColorTable[memberType], memberToken);
                else
                    return string.Format("[color={0}][font=Courier New]{1}[/font][/color]", memberTypeColorTable[memberType], memberToken);
            }

            public string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType)
            {
                //string color = memberTypeColorTable[memberType];
                return string.Format("[color={0}][font=Courier New]{1}[/font][/color]", memberTypeColorTable[memberType], memberToken);
            }

            public string DenoteNewLine()
            {
                return String.Empty;
            }

            public string FormatMemberNameToken(string token, IMember member, IIntermediateCodeTranslatorOptions options, bool declarePoint)
            {
                TranslatorFormatterMemberType memberType = TranslatorFormatterMemberType.Local;
                if (member is IMethodMember)
                    memberType = TranslatorFormatterMemberType.Method;
                else if (member is IMethodSignatureMember)
                    memberType = TranslatorFormatterMemberType.MethodSignature;
                else if (member is IPropertyMember || member is IIndexerMember)
                    memberType = TranslatorFormatterMemberType.Property;
                else if (member is IPropertySignatureMember || member is IIndexerSignatureMember)
                    memberType = TranslatorFormatterMemberType.PropertySignature;
                else if (member is IMethodParameterMember ||
                         member is IMethodSignatureParameterMember ||
                         member is IIndexerParameterMember ||
                         member is IIndexerSignatureParameterMember ||
                         member is IConstructorParameterMember)
                    memberType = TranslatorFormatterMemberType.Parameter;
                else if (member is IFieldMember)
                    memberType = TranslatorFormatterMemberType.Field;
                else if (member is IStatementBlockLocalMember)
                    memberType = TranslatorFormatterMemberType.Local;
                return FormatMemberNameToken(token, memberType);
            }


            public string FormatBeginType(IDeclaredType type)
            {
                return string.Empty;
            }

            public string FormatEndType()
            {
                return string.Empty;
            }

            public string FormatBeginNamespace(INameSpaceDeclaration target)
            {
                return string.Empty;
            }

            public string FormatBeginNamespace()
            {
                return string.Empty;
            }

            public string FormatBeginFile()
            {
                return string.Empty;
            }

            public string FormatEndFile()
            {
                return string.Empty;
            }

            #endregion

            #region IIntermediateCodeTranslatorFormatter Members


            public string FormatLabelToken(string labelName, ILabelStatement label, IIntermediateCodeTranslatorOptions options, bool declarePoint)
            {
                return labelName;
            }

            #endregion
        }
    }
}