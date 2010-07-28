using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Oilexer.Utilities.Collections;
using Microsoft.VisualStudio.Text.Classification;
using Oilexer.Parser;
using Oilexer.Parser.Builder;
using Microsoft.VisualStudio.Text.Tagging;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.Utilities;
using System.Timers;
using Microsoft.VisualStudio.Text.Editor;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.VSIntegration
{
    internal abstract class GDFileHandlerBase
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

        private GDParser.Lexer lexer;

        public GDParser.Lexer Lexer
        {
            get
            {
                if (this.lexer == null)
                    this.lexer = new GDParser.Lexer();
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

        public class BaseParser : 
            GDParser
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
