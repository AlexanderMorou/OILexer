using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
/* *
 * Inspired by the color-coded console printing.
 * */
namespace Oilexer.Translation
{
    /// <summary>
    /// Defines properties and methods for translating a grammar description file.
    /// </summary>
    public interface IGDTranslator
    {
        /// <summary>
        /// Translates a rule entry.
        /// </summary>
        /// <param name="entry">The <see cref="IProductionRuleEntry"/>
        /// to translate.</param>
        void Translate(IProductionRuleEntry entry);
        /// <summary>
        /// Translates a rule series within an alternation.
        /// </summary>
        /// <param name="series">The <see cref="IProductionRuleSeries"/> to 
        /// translate.
        /// </param>
        void Translate(IProductionRuleSeries series);
        /// <summary>
        /// Translates a rule expression.
        /// </summary>
        /// <param name="rule">The <see cref="IProductionRule"/>
        /// to translate.</param>
        void Translate(IProductionRule rule);
        /// <summary>
        /// Translates a <see cref="IProductionRuleItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="IProductionRuleItem"/>
        /// to translate.</param>
        void Translate(IProductionRuleItem item);
        /// <summary>
        /// Translates a token reference item.
        /// </summary>
        /// <param name="tokenReference">The <see cref="ITokenReferenceProductionRuleItem"/>
        /// to translate.</param>
        void Translate(ITokenReferenceProductionRuleItem tokenReference);
        /// <summary>
        /// Translates a rule reference item.
        /// </summary>
        /// <param name="ruleReference">The <see cref="IRuleReferenceProductionRuleItem"/>
        /// to translate.</param>
        void Translate(IRuleReferenceProductionRuleItem ruleReference);
        /// <summary>
        /// Translates a soft reference item.
        /// </summary>
        /// <param name="softReference">The <see cref="ISoftReferenceProductionRuleItem"/> to
        /// translate.</param>
        void Translate(ISoftReferenceProductionRuleItem softReference);
        /// <summary>
        /// Translates a literal character reference item.
        /// </summary>
        /// <param name="charReference">The <see cref="ILiteralCharReferenceProductionRuleItem"/>
        /// to translate.</param>
        void Translate(ILiteralCharReferenceProductionRuleItem charReference);
        /// <summary>
        /// Translates a literal character item.
        /// </summary>
        /// <param name="charItem">The <see cref="ILiteralCharProductionRuleItem"/>
        /// to translate.</param>
        void Translate(ILiteralCharProductionRuleItem charItem);
        /// <summary>
        /// Translates a literal string reference item.
        /// </summary>
        /// <param name="strReference">The <see cref="ILiteralStringReferenceProductionRuleItem"/>
        /// to translate.</param>
        void Translate(ILiteralStringReferenceProductionRuleItem strReference);
        /// <summary>
        /// Translates a literal string item.
        /// </summary>
        /// <param name="strItem">The <see cref="ILiteralStringProductionRuleItem"/>
        /// to translate</param>
        void Translate(ILiteralStringProductionRuleItem strItem);
        /// <summary>
        /// Translates a group item.
        /// </summary>
        /// <param name="group">The <see cref="IProductionRuleGroupItem"/> to
        /// translate.</param>
        void Translate(IProductionRuleGroupItem group);
        /// <summary>
        /// Translates a token entry.
        /// </summary>
        /// <param name="entry">The <see cref="ITokenEntry"/> to 
        /// translate.</param>
        void Translate(ITokenEntry entry);
        /// <summary>
        /// Translates a token expression series.
        /// </summary>
        /// <param name="series">The <see cref="ITokenExpressionSeries"/>
        /// to translate.</param>
        void Translate(ITokenExpressionSeries series);
        /// <summary>
        /// Translates a token expression.
        /// </summary>
        /// <param name="expression">The <see cref="ITokenExpression"/>
        /// to translate.</param>
        void Translate(ITokenExpression expression);
        /// <summary>
        /// Translates a token item.
        /// </summary>
        /// <param name="item">The <see cref="ITokenItem"/> to translate.</param>
        void Translate(ITokenItem item);
        /// <summary>
        /// Translates a literal character item.
        /// </summary>
        /// <param name="charReference">The <see cref="ILiteralCharTokenItem"/> to
        /// translate.</param>
        void Translate(ILiteralCharTokenItem charReference);
        /// <summary>
        /// Translates a literal character reference item.
        /// </summary>
        /// <param name="charReference">The <see cref="ILiteralCharReferenceTokenItem"/>
        /// to translate.</param>
        void Translate(ILiteralCharReferenceTokenItem charReference);
        /// <summary>
        /// Translates a literal string reference item.
        /// </summary>
        /// <param name="strReference">The <see cref="ILiteralStringReferenceTokenItem"/>
        /// to translate.</param>
        void Translate(ILiteralStringReferenceTokenItem strReference);
        /// <summary>
        /// Translates a literal string item.
        /// </summary>
        /// <param name="strItem">The <see cref="ILiteralStringTokenItem"/>
        /// to translate.</param>
        void Translate(ILiteralStringTokenItem strItem);
        /// <summary>
        /// Translates a soft reference item.
        /// </summary>
        /// <param name="softReference">The <see cref="ISoftReferenceTokenItem"/> 
        /// to translate.</param>
        void Translate(ISoftReferenceTokenItem softReference);
        /// <summary>
        /// Translates a token reference item.
        /// </summary>
        /// <param name="tokenReference">The <see cref="ITokenReferenceTokenItem"/> 
        /// to translate.</param>
        void Translate(ITokenReferenceTokenItem tokenReference);
        /// <summary>
        /// Translates a group item.
        /// </summary>
        /// <param name="group">The <see cref="ITokenGroupItem"/>
        /// to translate.</param>
        void Translate(ITokenGroupItem group);

    }
}
