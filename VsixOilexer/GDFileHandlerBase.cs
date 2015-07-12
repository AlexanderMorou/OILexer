using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal abstract class GDFileHandlerBase :
        IDisposable
    {
        private Dictionary<IOilexerGrammarToken, OilexerGrammarTokenType> reclassifications = new Dictionary<IOilexerGrammarToken, OilexerGrammarTokenType>();

        /// <summary>
        /// Data member for <see cref="BufferText"/> to lazily
        /// initialize the property.
        /// </summary>
        private string bufferText;

        /// <summary>
        /// Returns/sets the <see cref="String"/> representing the
        /// buffer presently in use.
        /// </summary>
        public virtual string BufferText
        {
            get
            {
                return this.bufferText;
            }
            set
            {
                this.bufferText = value;
            }
        }

        protected bool ParserEnabled
        {
            get
            {
                return parser != null;
            }
        }

        /// <summary>
        /// Data member for <see cref="Parser"/>.
        /// </summary>
        internal BaseParser parser;

        /// <summary>
        /// The <see cref="OilexerBufferedSimpleParser"/> which defines the 
        /// structural parser that aids the <see cref="Classifier"/>
        /// and <see cref="Outliner"/> taggers.
        /// </summary>
        public BaseParser Parser
        {
            get
            {
                if (this.parser == null)
                    this.parser = InitializeParser();
                return this.parser;
            }
        }

        protected virtual BaseParser InitializeParser()
        {
            return new BaseParser(this);
        }

        private OilexerParser.Lexer lexer;

        public OilexerParser.Lexer Lexer
        {
            get
            {
                if (this.lexer == null)
                    this.lexer = new OilexerParser.Lexer();
                return this.lexer;
            }
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifierToken, IOilexerGrammarProductionRuleEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, OilexerGrammarTokenType.RuleReference);
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifierToken, IOilexerGrammarProductionRuleTemplateEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, OilexerGrammarTokenType.RuleTemplateReference);
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifierToken, IOilexerGrammarTokenEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, OilexerGrammarTokenType.TokenReference);
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifierToken, ITokenItem primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            if (primary is ILiteralCharTokenItem)
                reclassifications.Add(identifierToken, OilexerGrammarTokenType.CharacterLiteral);
            else if (primary is ILiteralStringTokenItem)
                reclassifications.Add(identifierToken, OilexerGrammarTokenType.StringLiteral);
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifier, ReclassificationKind reclassificationKind)
        {
            if (reclassifications.ContainsKey(identifier))
                return;
            switch (reclassificationKind)
            {
                case ReclassificationKind.Keyword:
                case ReclassificationKind.NativeMethod:
                    reclassifications.Add(identifier, OilexerGrammarTokenType.Keyword);
                    break;
                default:
                    break;
            }
        }

        internal void ReclassifyToken(IOilexerGrammarToken token, ReclassificationKind kind)
        {
            switch (kind)
            {
                case ReclassificationKind.Keyword:
                case ReclassificationKind.NativeMethod:
                    reclassifications.Add(token, OilexerGrammarTokenType.Keyword);
                    break;
                case ReclassificationKind.Error:
                    reclassifications.Add(token, OilexerGrammarTokenType.Error);
                    break;
            }
        }

        internal void ReclassifyToken(OilexerGrammarTokens.IdentifierToken identifierToken, IProductionRuleTemplatePart item)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, OilexerGrammarTokenType.RuleTemplateParameterReference);
        }

        public void ReclassifyTokens()
        {
            foreach (var tokenClassificationPair in this.reclassifications)
                this.PerformReclassification(tokenClassificationPair.Key, tokenClassificationPair.Value);
            this.ClearReclassifications();
        }

        protected abstract void PerformReclassification(IOilexerGrammarToken token, OilexerGrammarTokenType newClassification);

        protected void ClearReclassifications()
        {
            this.reclassifications.Clear();
        }

        internal IParserResults<IOilexerGrammarFile> CurrentParseResults { get; set; }

        protected abstract void SetBuffer();

        public virtual void Dispose()
        {
            this.bufferText = null;
            this.reclassifications = null;
            if (this.ParserEnabled)
                this.parser = null;
            if (this.lexer != null)
                this.lexer = null;
        }

        public class BaseParser : 
            OilexerParser
        {
            private GDFileHandlerBase handler;
            internal BaseParser(GDFileHandlerBase handler, bool captureRegions = false)
                : base(false, captureRegions)
            {
                this.handler = handler;
                this.CurrentTokenizer = this.handler.Lexer;
            }

            protected GDFileHandlerBase Handler
            {
                get
                {
                    return this.handler;
                }
            }

            internal void Dispose()
            {
                this.handler = null;
                this.CurrentTokenizer = null;
                this.originalFormTokens.Clear();
                this.originalFormTokens = null;
            }

            public new IParserResults<IOilexerGrammarFile> BeginParse()
            {
                return base.BeginParse();
            }

        }

    }
}
