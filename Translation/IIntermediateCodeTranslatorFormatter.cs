using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.Reflection;

namespace Oilexer.Translation
{
    /// <summary>
    /// The type of token being formatted.
    /// </summary>
    public enum TranslatorFormatterTokenType
    {
        /// <summary>
        /// The token relates to a number.
        /// </summary>
        Number = 1,
        /// <summary>
        /// The token relates to a namespace.
        /// </summary>
        NameSpace = 2,
        /// <summary>
        /// The token relates to a keyword.
        /// </summary>
        Keyword = 3,
        /// <summary>
        /// The token relates to a comment.
        /// </summary>
        Comment = 4,
        /// <summary>
        /// The token relates to a string.
        /// </summary>
        String = 5,
        /// <summary>
        /// The token relates to an operator.
        /// </summary>
        Operator = 6,
        /// <summary>
        /// The token is something else not described.
        /// </summary>
        Other = 7,
        /// <summary>
        /// The token relates to preformatted text, perhaps from a buffer that called
        /// the formatting prior.
        /// </summary>
        Preformatted = 8,
    }
    /// <summary>
    /// The type of member the token will relate to.
    /// </summary>
    public enum TranslatorFormatterMemberType
    {
        /// <summary>
        /// The member is a method.
        /// </summary>
        Method,
        /// <summary>
        /// The member is a property.
        /// </summary>
        Property,
        /// <summary>
        /// The member is an event.
        /// </summary>
        Event,
        /// <summary>
        /// The member is a field.
        /// </summary>
        Field,
        /// <summary>
        /// The member is a method signature.
        /// </summary>
        MethodSignature,
        /// <summary>
        /// The member is a property signature.
        /// </summary>
        PropertySignature,
        /// <summary>
        /// The member is an event signature.
        /// </summary>
        EventSignature,
        /// <summary>
        /// The member is a method signature parameter.
        /// </summary>
        Parameter,
        /// <summary>
        /// The member is a statement block local.
        /// </summary>
        Local,
        /// <summary>
        /// The member is a statement block label.
        /// </summary>
        Label,
    }

    /// <summary>
    /// Defines properties and methods for a formatter which alters the post-translation code
    /// for formatting purposes.
    /// </summary>
    public interface IIntermediateCodeTranslatorFormatter
    {
        string FormatKeywordToken(string keywordToken);
        string FormatNameSpace(string nameSpacePath);
        string FormatTypeNameToken(string identifierToken, IType type, IIntermediateCodeTranslatorOptions options, bool declarePoint);
        string FormatCommentToken(string commentToken);
        string FormatStringToken(string strToken);
        string FormatOperatorToken(string oprToken);
        string FormatNumberToken(string numberToken);
        string FormatOtherToken(string otherToken);
        /// <summary>
        /// Formats a member name which has  no associated <see cref="IMember"/>.
        /// Typical use is 
        /// </summary>
        /// <param name="memberToken">The string representing the name of the member.</param>
        /// <param name="memberType">The type of member represented by <paramref name="memberToken"/>.</param>
        /// <param name="parent">The parent of the member.</param>
        string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType, IType parent);

        string FormatMemberNameToken(string memberToken, TranslatorFormatterMemberType memberType);
        string DenoteNewLine(IIntermediateProject project, IIntermediateCodeTranslatorOptions options);

        string FormatMemberNameToken(string token, IMember member, IIntermediateCodeTranslatorOptions options, bool declarePoint);
        string FormatBeginType(IDeclaredType type);
        string FormatEndType();
        string FormatBeginNamespace(INameSpaceDeclaration target);
        string FormatBeginNamespace();
        string FormatBeginFile(IIntermediateProject project, IIntermediateCodeTranslatorOptions options);
        string FormatEndFile();

        string FormatLabelToken(string labelName, Statements.ILabelStatement label, IIntermediateCodeTranslatorOptions options, bool declarePoint);
    }
}
