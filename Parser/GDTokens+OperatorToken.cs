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
            None = 0x0,
            /// <summary>
            /// The operator used for the name of the production rule and the expression
            /// used by the production rule.
            /// </summary>
            /// <remarks>Literal value: '::='</remarks>
            ProductionRuleSeparator = 0x1,
            /// <summary>
            /// The operator used for the name of the token and the expression used
            /// by the token.
            /// </summary>
            /// <remarks>Literal value: ':='</remarks>
            TokenSeparator = 0x2,
            /// <summary>
            /// The operator used to segment the token/production rule parts.
            /// </summary>
            /// <remarks>Literal value: '|'</remarks>
            LeafSeparator = 0x4,
            LeftCurlyBrace = 0x8,
            LeftParenthesis = 0x10,
            RightParenthesis = 0x20,
            OptionsSeparator = 0x40,
            RightCurlyBrace = 0x80,
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>Literal value: '#'</remarks>
            CounterNotification = 0x100,
            TemplatePartsStart = 0x200,
            TemplatePartsEnd = 0x400,
            TemplatePartsSeparator = 0x800,
            /// <summary>
            /// The operator used to terminate a token/production rule declaration.
            /// </summary>
            /// <remarks>Literal value: ';'</remarks>
            EntryTerminal = 0x1000,
            OneOrMore = 0x2000,
            ZeroOrMore = 0x4000,
            ZeroOrOne = 0x8000,
            /// <summary>
            /// The operator used to delimit an (ProductionRule.Member | Token.Member) combination
            /// or to denote an option at the start of a line outside of a declaration.
            /// </summary>
            Period = 0x10000,
            ErrorSeparator = 0x20000,
            Minus = 0x40000,
            ProductionRuleFlag = 0x080000,
            ForcedStringForm = 0x100000,
            AndAnd = 0x200000,
            PipePipe = 0x400000,
            ExclaimEqual = 0x800000,
            AtSign = 0x1000000,
            EqualEqual = 0x2000000,
            AsteriskAsterisk = 0x4000000,
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
                        case OperatorType.AsteriskAsterisk:
                        case OperatorType.TokenSeparator:
                        case OperatorType.ExclaimEqual:
                        case OperatorType.EqualEqual:
                        case OperatorType.AndAnd:
                        case OperatorType.PipePipe:
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
                        case OperatorType.ForcedStringForm:
                        case OperatorType.AtSign:
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
                    case OperatorType.AndAnd:
                        return "&&";
                    case OperatorType.AsteriskAsterisk:
                        return "**";
                    case OperatorType.PipePipe:
                        return "||";
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
                    case OperatorType.EqualEqual:
                        return "==";
                    case OperatorType.ExclaimEqual:
                        return "!=";
                    case OperatorType.AtSign:
                        return "@";
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
                    case OperatorType.ForcedStringForm:
                        return "$";
                    default:
                        return "/#*NAO*#/";
                }
            }
        }
    }
}