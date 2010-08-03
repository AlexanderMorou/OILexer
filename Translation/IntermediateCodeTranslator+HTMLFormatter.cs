using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Statements;
using System.CodeDom;

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
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Local, "#008080"),
                            new KeyValuePair<TranslatorFormatterMemberType, string>(TranslatorFormatterMemberType.Label, "#808080") 
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
                string result = identifierToken;
                if (declarePoint && type is IDeclaredType)
                    result = string.Format("<a name=\"t:{1}\"></a>{0}", result, type.GetTypeName(options, true));
                if (bold)
                    result = string.Format("<span style=\"color:{0};font-family:Courier New;font-weight:bolder;\">{1}</span>", color, result);
                else
                    result = string.Format("<span style=\"color:{0};font-family:Courier New;\">{1}</span>", color, result);
                if (!declarePoint && options.GetFileNameOf != null && type is IDeclaredType)
                    result = string.Format("<a style=\"text-decoration:none;\" href=\"{1}#t:{2}\">{0}</a>", result, options.GetFileNameOf(type), type.GetTypeName(options, true));
                return result;
            }

            public string FormatCommentToken(string commentToken)
            {
                return string.Format("<span style=\"color:green;font-family:Courier New;\">{0}</span>", HTMLEncode(commentToken));
            }

            private static string HTMLEncode(string toEncode)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in toEncode)
                {
                    switch (c)
                    {
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '&':
                            sb.Append("&amp;");
                            break;
                        case ' ':
                            sb.Append("&nbsp;");
                            break;
                        default:
                            if (c <= 0xFF)
                                sb.Append(c);
                            else
                                sb.AppendFormat("&#{0:000#};", (int)c);
                            break;
                    }
                }
                return sb.ToString();
            }

            public string FormatStringToken(string strToken)
            {
                return string.Format("<span style=\"color:green;font-family:Courier New;\">{0}</span>", HTMLEncode(strToken));
            }

            public string FormatOperatorToken(string oprToken)
            {
                return string.Format("<span style=\"color:#C080C0;font-family:Courier New;\">{0}</span>", HTMLEncode(oprToken));
            }

            public string FormatNumberToken(string numberToken)
            {
                return string.Format("<span style=\"color:red;font-family:Courier New;\">{0}</span>", HTMLEncode(numberToken));
            }

            public string FormatOtherToken(string otherToken)
            {
                return string.Format("<span style=\"color:black;font-family:Courier New;\">{0}</span>", HTMLEncode(otherToken));
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

            #region IIntermediateCodeTranslatorFormatter Members


            public string FormatLabelToken(string labelName, ILabelStatement label, IIntermediateCodeTranslatorOptions options, bool declarePoint)
            {
                var parentTarget = label.SourceBlock;
                Stack<int> indices = new Stack<int>();
                indices.Push(parentTarget.IndexOf(label));
                while (parentTarget.Parent is IStatementBlock)
                {
                    var oldTarget = parentTarget;
                    parentTarget = (IStatementBlock)parentTarget.Parent;
                    indices.Push(parentTarget.IndexOf((IStatement)oldTarget));
                }
                var parentMember = parentTarget.Parent as IMember;
                string uniqueIdentifier;
                string result = labelName;
                var activeType = options.BuildTrail.FirstOrDefault(p => p is IDeclaredType) as IDeclaredType;
                if (parentMember == null)
                    uniqueIdentifier = label.Name;
                else
                    uniqueIdentifier = string.Format("{0}::{{{1}}}", parentMember.GetUniqueIdentifier(), string.Join("}.{", (from i in indices
                                                                                                                             select i.ToString()).ToArray()));
                if (declarePoint)
                    result = string.Format("<a name=\"{0}\"></a>{1}", uniqueIdentifier, result);
                string titleText = string.Format("(label) {0}", labelName);
                result = FormatMemberNameToken(result, TranslatorFormatterMemberType.Label);
                if (!declarePoint)
                {
                    result = string.Format("<a style=\"text-decoration:none;\" {3}href=\"{0}#{1}\">{2}</a>", options.GetFileNameOf(activeType), uniqueIdentifier, result, string.IsNullOrEmpty(titleText) ? string.Empty : string.Format("title=\"{0}\" ", titleText));
                }
                return result;
            }

            public string FormatMemberNameToken(string token, IMember member, IIntermediateCodeTranslatorOptions options, bool declarePoint)
            {
                TranslatorFormatterMemberType memberType = TranslatorFormatterMemberType.Local;
                string titleText = string.Empty;
                if (member is IMethodMember)
                {
                    memberType = TranslatorFormatterMemberType.Method;
                    titleText = string.Format("(method) {0}",member.GetUniqueIdentifier());
                }
                else if (member is IMethodSignatureMember)
                {
                    memberType = TranslatorFormatterMemberType.MethodSignature;
                    titleText = string.Format("(method) {0}", member.GetUniqueIdentifier());
                }
                else if (member is IPropertyMember || member is IIndexerMember)
                {
                    memberType = TranslatorFormatterMemberType.Property;
                    var pMember = ((IPropertyMember)member);
                    titleText = string.Format("(property) {0} {1}", pMember.PropertyType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IPropertySignatureMember || member is IIndexerSignatureMember)
                {
                    memberType = TranslatorFormatterMemberType.PropertySignature;
                    var pMember = ((IPropertySignatureMember)member);
                    titleText = string.Format("(property) {0} {1}", pMember.PropertyType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IMethodParameterMember)
                {
                    memberType = TranslatorFormatterMemberType.Parameter;
                    var pMember = ((IMethodParameterMember)member);
                    titleText = string.Format("(parameter) {0} {1}", pMember.ParameterType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IMethodSignatureParameterMember)
                {
                    memberType = TranslatorFormatterMemberType.Parameter;
                    var pMember = ((IMethodSignatureParameterMember)member);
                    titleText = string.Format("(parameter) {0} {1}", pMember.ParameterType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IIndexerParameterMember)
                {
                    memberType = TranslatorFormatterMemberType.Parameter;
                    var pMember = ((IIndexerParameterMember)member);
                    titleText = string.Format("(parameter) {0} {1}", pMember.ParameterType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IIndexerSignatureParameterMember)
                {
                    memberType = TranslatorFormatterMemberType.Parameter;
                    var pMember = ((IIndexerSignatureParameterMember)member);
                    titleText = string.Format("(parameter) {0} {1}", pMember.ParameterType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IConstructorParameterMember)
                {
                    memberType = TranslatorFormatterMemberType.Parameter;
                    var pMember = ((IConstructorParameterMember)member);
                    titleText = string.Format("(parameter) {0} {1}", pMember.ParameterType.ToString(), member.GetUniqueIdentifier());
                }
                else if (member is IFieldMember)
                {
                    var fMember = ((IFieldMember)member);
                    titleText = string.Format("(field) {0} {1}", fMember.FieldType.ToString(), member.GetUniqueIdentifier());
                    memberType = TranslatorFormatterMemberType.Field;
                }
                else if (member is IStatementBlockLocalMember)
                {
                    var lMember = ((IStatementBlockLocalMember)member);
                    titleText = string.Format("(local variable) {0} {1}", lMember.LocalType.ToString(), member.GetUniqueIdentifier());
                    memberType = TranslatorFormatterMemberType.Local;
                }
                var activeType = options.BuildTrail.FirstOrDefault(p => p is IDeclaredType) as IDeclaredType;
                string result = HTMLEncode(token);
                if (declarePoint)
                {
                    string targetName = string.Format("m:{0}::{1}", activeType.GetTypeName(options, true), GetMemberUniqueIdentifier(member).Replace("<", "[").Replace(">", "]"));
                    result = string.Format("<a name=\"{1}\"></a>{0}", result, targetName);
                }
                result = FormatMemberNameToken(result, memberType);

                var declaringType = GetDeclaringType(member);
                if (!declarePoint && declaringType != null)
                {
                    string targetName = string.Format("m:{0}::{1}", declaringType.GetTypeName(options, true), GetMemberUniqueIdentifier(member).Replace("<", "[").Replace(">", "]"));
                    result = string.Format("<a style=\"text-decoration:none;\" {3}href=\"{0}#{1}\">{2}</a>", options.GetFileNameOf(declaringType), targetName, result, string.IsNullOrEmpty(titleText) ? string.Empty : string.Format("title=\"{0}\" ", titleText));
                }
                return result;
            }

            private static string GetMemberUniqueIdentifier(IMember member)
            {
                if (member is IStatementBlockLocalMember)
                {
                    var lMember = member as IStatementBlockLocalMember;
                    Stack<int> indices = new Stack<int>();
                    
                    var parentTarget = ((IStatementBlockLocalMember)(member)).ParentTarget;
                    indices.Push(parentTarget.Locals.Values.IndexOf(lMember));
                    while (parentTarget.Parent is IStatementBlock)
                    {
                        var oldTarget = parentTarget;
                        parentTarget = (IStatementBlock)parentTarget.Parent;
                        indices.Push(parentTarget.IndexOf((IStatement)oldTarget));
                    }
                    var parentMember = parentTarget.Parent as IMember;
                    if (parentMember == null)
                        return member.GetUniqueIdentifier();
                    else
                        return string.Format("{0}::{1}", parentMember.GetUniqueIdentifier(), string.Join(".", (from i in indices
                                                                                                               select i.ToString()).ToArray()));
                }
                else if (member is IMethodParameterMember)
                    return GetParameterUniqueIdentifier<IMethodParameterMember, CodeMemberMethod, IMemberParentType>((IMethodParameterMember)member);
                else if (member is IMethodSignatureParameterMember)
                    return GetParameterUniqueIdentifier<IMethodSignatureParameterMember, CodeMemberMethod, ISignatureMemberParentType>((IMethodSignatureParameterMember)member);
                else if (member is IIndexerParameterMember)
                    return GetParameterUniqueIdentifier<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>((IIndexerParameterMember)member);
                else if (member is IIndexerSignatureParameterMember)
                    return GetParameterUniqueIdentifier<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>((IIndexerSignatureParameterMember)member);
                else if (member is IConstructorParameterMember)
                    return GetParameterUniqueIdentifier<IConstructorParameterMember, CodeConstructor, IMemberParentType>((IConstructorParameterMember)member);
                else
                    return member.GetUniqueIdentifier();
            }


            private static string GetParameterUniqueIdentifier<TParameter, TParameteredDom, TParent>(TParameter member)
                where TParameter :
                    IParameteredParameterMember<TParameter, TParameteredDom, TParent>
                where TParent :
                    IDeclarationTarget
                where TParameteredDom :
                    CodeObject
            {
                var parent = member.ParentTarget as IMember;
                return string.Format("{0}::{1}", parent.GetUniqueIdentifier(), member.GetUniqueIdentifier());
            }
            #endregion

            #region IIntermediateCodeTranslatorFormatter Members

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
                return "<html><body style=\"font-family:Courier New;font-size:10pt;\">";
            }

            public string FormatEndFile()
            {
                return "</body></html>";
            }

            #endregion
        }
        private static IDeclaredType GetDeclaringType(IDeclaration target)
        {
            var current = target;
            while (target != null && !(current is IDeclaredType))
                if (target is IMember)
                {
                    var parentTarget = ((IMember)target).ParentTarget;
                    if (parentTarget is IStatementBlock)
                    {
                        while (parentTarget is IStatementBlock)
                        {
                            var parentAsBlock = parentTarget as IStatementBlock;
                            parentTarget = parentAsBlock.Parent;
                        }
                    }
                    target = (IDeclaration)parentTarget;
                }
                else
                    break;
            return target as IDeclaredType;
        }
    }
}