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
            ColonColonEquals = 0x1,
            /// <summary>
            /// The operator used for the name of the token and the expression used
            /// by the token.
            /// </summary>
            /// <remarks>Literal value: ':='</remarks>
            ColonEquals = 0x2,
            /// <summary>
            /// The operator used to segment the token/production rule parts.
            /// </summary>
            /// <remarks>Literal value: '|'</remarks>
            Pipe = 0x4,
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
            LessThan = 0x200,
            GreaterThan = 0x400,
            Comma = 0x800,
            /// <summary>
            /// The operator used to terminate a token/production rule declaration.
            /// </summary>
            /// <remarks>Literal value: ';'</remarks>
            SemiColon = 0x1000,
            Plus = 0x2000,
            ZeroOrMore = 0x4000,
            ZeroOrOne = 0x8000,
            /// <summary>
            /// The operator used to delimit an (ProductionRule.Member | Token.Member) combination
            /// or to denote an option at the start of a line outside of a declaration.
            /// </summary>
            Period = 0x10000,
            Equals = 0x20000,
            Minus = 0x40000,
            Exclaim = 0x080000,
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
                        case OperatorType.ColonColonEquals:
                            return 3;
                        case OperatorType.AsteriskAsterisk:
                        case OperatorType.ColonEquals:
                        case OperatorType.ExclaimEqual:
                        case OperatorType.EqualEqual:
                        case OperatorType.AndAnd:
                        case OperatorType.PipePipe:
                            return 2;
                        case OperatorType.Pipe:
                        case OperatorType.LeftCurlyBrace:
                        case OperatorType.LeftParenthesis:
                        case OperatorType.RightParenthesis:
                        case OperatorType.RightCurlyBrace:
                        case OperatorType.OptionsSeparator:
                        case OperatorType.LessThan:
                        case OperatorType.GreaterThan:
                        case OperatorType.Comma:
                        case OperatorType.SemiColon:
                        case OperatorType.Plus:
                        case OperatorType.ZeroOrMore:
                        case OperatorType.ZeroOrOne:
                        case OperatorType.Period:
                        case OperatorType.Equals:
                        case OperatorType.Minus:
                        case OperatorType.Exclaim:
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
                    case OperatorType.ColonColonEquals:
                        return "::=";
                    case OperatorType.AndAnd:
                        return "&&";
                    case OperatorType.AsteriskAsterisk:
                        return "**";
                    case OperatorType.PipePipe:
                        return "||";
                    case OperatorType.ColonEquals:
                        return ":=";
                    case  OperatorType.OptionsSeparator:
                        return ":";
                    case OperatorType.Pipe:
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
                    case OperatorType.LessThan:
                        return "<";
                    case OperatorType.GreaterThan:
                        return ">";
                    case OperatorType.Comma:
                        return ",";
                    case OperatorType.SemiColon:
                        return ";";
                    case OperatorType.Plus:
                        return "+";
                    case OperatorType.ZeroOrMore:
                        return "*";
                    case OperatorType.ZeroOrOne:
                        return "?";
                    case OperatorType.Period:
                        return ".";
                    case OperatorType.Equals:
                        return "=";
                    case OperatorType.Minus:
                        return "-";
                    case OperatorType.Exclaim:
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