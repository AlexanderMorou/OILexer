using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{
    partial class GDTokens
    {
        /// <summary>
        /// The type of operator an <see cref="OperatorToken"/> is.
        /// </summary>
        public enum OperatorType
        {
            /// <summary>
            /// The operator used for the name of the production rule and the expression
            /// used by the production rule.
            /// </summary>
            /// <remarks>Literal value: '::='</remarks>
            ProductionRuleSeparator,
            /// <summary>
            /// The operator used for the name of the token and the expression used
            /// by the token.
            /// </summary>
            /// <remarks>Literal value: ':='</remarks>
            TokenSeparator,
            /// <summary>
            /// The operator used to segment the token/production rule parts.
            /// </summary>
            /// <remarks>Literal value: '|'</remarks>
            LeafSeparator,
            LeftCurlyBrace,
            LeftParenthesis,
            RightParenthesis,
            OptionsSeparator,
            RightCurlyBrace,
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>Literal value: '#'</remarks>
            CounterNotification,
            TemplatePartsStart,
            TemplatePartsEnd,
            TemplatePartsSeparator,
            /// <summary>
            /// The operator used to terminate a token/production rule declaration.
            /// </summary>
            /// <remarks>Literal value: ';'</remarks>
            EntryTerminal,
            OneOrMore,
            ZeroOrMore,
            ZeroOrOne,
            /// <summary>
            /// The operator used to delimit an (ProductionRule.Member | Token.Member) combination
            /// or to denote an option at the start of a line outside of a declaration.
            /// </summary>
            Period,
            ErrorSeparator,
            Minus,
            ProductionRuleFlag
        }
        public class OperatorToken :
            GDToken
        {
            private OperatorType type;
            public OperatorToken(OperatorType type, int column, int line, long position)
                : base(column, line, position)
            {
                this.type = type;
            }

            public OperatorType Type
            {
                get
                {
                    return this.type;
                }
            }

            public override int Length
            {
                get
                {
                    switch (type)
                    {
                        case OperatorType.ProductionRuleSeparator:
                            return 3;
                        case OperatorType.TokenSeparator:
                            return 2;
                        case OperatorType.LeafSeparator:
                        case OperatorType.LeftCurlyBrace:
                        case OperatorType.LeftParenthesis:
                        case OperatorType.RightParenthesis:
                        case OperatorType.RightCurlyBrace:
                        case OperatorType.OptionsSeparator:
                        case OperatorType.TemplatePartsStart:
                        case OperatorType.TemplatePartsEnd:
                        case OperatorType.TemplatePartsSeparator:
                        case OperatorType.EntryTerminal:
                        case OperatorType.OneOrMore:
                        case OperatorType.ZeroOrMore:
                        case OperatorType.ZeroOrOne:
                        case OperatorType.Period:
                        case OperatorType.ErrorSeparator:
                        case OperatorType.Minus:
                        case OperatorType.ProductionRuleFlag:
                        case OperatorType.CounterNotification:
                            return 1;
                        default:
                            return 0;
                    }
                }
            }
            
            public override GDTokenType TokenType
            {
                get { return GDTokenType.Operator; }
            }

            public override bool ConsumedFeed
            {
                get { return false; }
            }

            public override string ToString()
            {
                switch (this.Type)
                {
                    case OperatorType.ProductionRuleSeparator:
                        return "::=";
                    case OperatorType.TokenSeparator:
                        return ":=";
                    case  OperatorType.OptionsSeparator:
                        return ":";
                    case OperatorType.LeafSeparator:
                        return "|";
                    case OperatorType.LeftCurlyBrace:
                        return "{";
                    case OperatorType.LeftParenthesis:
                        return "(";
                    case OperatorType.RightParenthesis:
                        return ")";
                    case OperatorType.CounterNotification:
                        return "#";
                    case OperatorType.RightCurlyBrace:
                        return "}";
                    case OperatorType.TemplatePartsStart:
                        return "<";
                    case OperatorType.TemplatePartsEnd:
                        return ">";
                    case OperatorType.TemplatePartsSeparator:
                        return ",";
                    case OperatorType.EntryTerminal:
                        return ";";
                    case OperatorType.OneOrMore:
                        return "+";
                    case OperatorType.ZeroOrMore:
                        return "*";
                    case OperatorType.ZeroOrOne:
                        return "?";
                    case OperatorType.Period:
                        return ".";
                    case OperatorType.ErrorSeparator:
                        return "=";
                    case OperatorType.Minus:
                        return "-";
                    case OperatorType.ProductionRuleFlag:
                        return "!";
                    default:
                        return "/#*NAO*#/";
                }
            }
        }
    }
}