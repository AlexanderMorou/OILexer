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
        private Dictionary<IGDToken, GDTokenType> reclassifications = new Dictionary<IGDToken, GDTokenType>();

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
        /// The <see cref="GDBufferedSimpleParser"/> which defines the 
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

        private OILexerParser.Lexer lexer;

        public OILexerParser.Lexer Lexer
        {
            get
            {
                if (this.lexer == null)
                    this.lexer = new OILexerParser.Lexer();
                return this.lexer;
            }
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifierToken, IProductionRuleEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, GDTokenType.RuleReference);
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifierToken, IProductionRuleTemplateEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, GDTokenType.RuleTemplateReference);
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifierToken, ITokenEntry primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, GDTokenType.TokenReference);
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifierToken, ITokenItem primary)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            if (primary is ILiteralCharTokenItem)
                reclassifications.Add(identifierToken, GDTokenType.CharacterLiteral);
            else if (primary is ILiteralStringTokenItem)
                reclassifications.Add(identifierToken, GDTokenType.StringLiteral);
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifier, ReclassificationKind reclassificationKind)
        {
            if (reclassifications.ContainsKey(identifier))
                return;
            switch (reclassificationKind)
            {
                case ReclassificationKind.Keyword:
                case ReclassificationKind.NativeMethod:
                    reclassifications.Add(identifier, GDTokenType.Keyword);
                    break;
                default:
                    break;
            }
        }

        internal void ReclassifyToken(IGDToken token, ReclassificationKind kind)
        {
            switch (kind)
            {
                case ReclassificationKind.Keyword:
                case ReclassificationKind.NativeMethod:
                    reclassifications.Add(token, GDTokenType.Keyword);
                    break;
                case ReclassificationKind.Error:
                    reclassifications.Add(token, GDTokenType.Error);
                    break;
            }
        }

        internal void ReclassifyToken(GDTokens.IdentifierToken identifierToken, IProductionRuleTemplatePart item)
        {
            if (reclassifications.ContainsKey(identifierToken))
                return;
            reclassifications.Add(identifierToken, GDTokenType.RuleTemplateParameterReference);
        }

        public void ReclassifyTokens()
        {
            foreach (var tokenClassificationPair in this.reclassifications)
                this.PerformReclassification(tokenClassificationPair.Key, tokenClassificationPair.Value);
            this.ClearReclassifications();
        }

        protected abstract void PerformReclassification(IGDToken token, GDTokenType newClassification);

        protected void ClearReclassifications()
        {
            this.reclassifications.Clear();
        }

        internal IParserResults<IGDFile> CurrentParseResults { get; set; }

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
            OILexerParser
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

            public new IParserResults<IGDFile> BeginParse()
            {
                return base.BeginParse();
            }

        }

    }
}
