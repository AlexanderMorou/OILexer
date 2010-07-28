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
    internal class GDFileBufferedHandler :
        GDFileHandlerBase
    {
        private Dictionary<GDFileHandlerBase, IList<string>> includes;

        /// <summary>
        /// Returns the <see cref="ITextBuffer"/> associated to the
        /// <see cref="GDFileHandlerBase"/>.
        /// </summary>
        public ITextBuffer Buffer { get; private set; }

        private GDResolutionAssistant resolutionAssistant;

        internal _GDResolutionAssistant ResolutionAssistant
        {
            get
            {
                if (this.resolutionAssistant == null)
                    this.resolutionAssistant = new GDResolutionAssistant(this);
                return this.resolutionAssistant;
            }
        }

        public new SimpleParser Parser
        {
            get
            {
                return (SimpleParser)base.Parser;
            }
        }

        /// <summary>
        /// Returns/sets the <see cref="String"/> representing the
        /// buffer presently in use.
        /// </summary>
        public override string BufferText
        {
            get
            {
                if (base.BufferText == null)
                    this.SetBuffer();
                return base.BufferText;
            }
            set
            {
                base.BufferText = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="ITextDocument"/> associated to the
        /// <see cref="GDFileHandlerBase"/>.
        /// </summary>
        public ITextDocument Document { get; private set; }

        /// <summary>
        /// Data member for <see cref="Classifier"/>.
        /// </summary>
        private GDClassifier classifier;
        /// <summary>
        /// Returns the <see cref="GDClassifier"/> which associates classifications
        /// to the tokens within a <see cref="Buffer"/>.
        /// </summary>
        public GDClassifier Classifier
        {
            get
            {
                if (this.classifier == null)
                {
                    this.classifier = new GDClassifier(this);
                    if (base.BufferText == null)
                        this.SetBuffer();
                }
                return this.classifier;
            }
        }

        /// <summary>
        /// The data member which holds the <see cref="Outliner"/>.
        /// </summary>
        private GDOutliner outliner;

        private bool creatingOutliner;

        /// <summary>
        /// The <see cref="GDOutliner"/> which handles defining the outlining
        /// regions for the file.
        /// </summary>
        public GDOutliner Outliner
        {
            get
            {
                if (this.outliner == null)
                {
                    creatingOutliner = true;
                    this.outliner = new GDOutliner(this);
                    this.Reparse();
                    creatingOutliner = false;
                }
                return this.outliner;
            }
        }

        public bool ClassifierEnabled
        {
            get
            {
                return this.classifier != null;
            }
        }

        public bool OutlinerEnabled
        {
            get
            {
                return this.outliner != null;
            }
        }

        /// <summary>
        /// The <see cref="ITextDocumentFactoryService"/> used to obtain
        /// new <see cref="ITextDocument"/> instances.
        /// </summary>
        public ITextDocumentFactoryService DocumentFactory { get; private set; }

        /// <summary>
        /// Returns the <see cref="IContentTypeRegistryService"/> used to obtain
        /// the content type of the buffer.
        /// </summary>
        public IContentTypeRegistryService ContentTypeRegistry { get; private set; }

        /// <summary>
        /// The <see cref="ITextBufferFactoryService"/> used to obtain
        /// new <see cref="ITextBuffer"/> instances.
        /// </summary>
        public ITextBufferFactoryService BufferFactory { get; private set; }

        public IClassificationTypeRegistryService ClassificationRegistry { get; private set; }

        private Dictionary<GDTokenType, IClassificationType> classificationTypes;

        private ReadOnlyDictionary<GDTokenType, IClassificationType> _classificationTypes;

        public IReadOnlyDictionary<GDTokenType, IClassificationType> ClassificationTypes
        {
            get
            {
                if (this._classificationTypes == null)
                {
                    if (this.classificationTypes == null)
                    {
                        this.classificationTypes = new Dictionary<GDTokenType,IClassificationType>();
                        this.classificationTypes.Add(GDTokenType.CharacterLiteral, this.ClassificationRegistry.GetClassificationType("OILexer: Literal: Character"));
                        this.classificationTypes.Add(GDTokenType.CharacterRange, this.ClassificationRegistry.GetClassificationType("OILexer: Character Range"));
                        this.classificationTypes.Add(GDTokenType.Comment, this.ClassificationRegistry.GetClassificationType("OILexer: Comment"));
                        this.classificationTypes.Add(GDTokenType.Identifier, this.ClassificationRegistry.GetClassificationType("OILexer: Identifier"));
                        this.classificationTypes.Add(GDTokenType.TokenReference, this.ClassificationRegistry.GetClassificationType("OILexer: Token Reference"));
                        this.classificationTypes.Add(GDTokenType.RuleReference, this.ClassificationRegistry.GetClassificationType("OILexer: Rule Reference"));
                        this.classificationTypes.Add(GDTokenType.RuleTemplateReference, this.ClassificationRegistry.GetClassificationType("OILexer: Rule Template Reference"));
                        this.classificationTypes.Add(GDTokenType.RuleTemplateParameterReference, this.ClassificationRegistry.GetClassificationType("OILexer: Rule Template Parameter Reference"));
                        this.classificationTypes.Add(GDTokenType.NumberLiteral, this.ClassificationRegistry.GetClassificationType("OILexer: Literal: Number"));
                        this.classificationTypes.Add(GDTokenType.Operator, this.ClassificationRegistry.GetClassificationType("OILexer: Operator"));
                        this.classificationTypes.Add(GDTokenType.PreprocessorDirective, this.ClassificationRegistry.GetClassificationType("OILexer: Preprocessor Directive"));
                        this.classificationTypes.Add(GDTokenType.StringLiteral, this.ClassificationRegistry.GetClassificationType("OILexer: Literal: String"));
                        this.classificationTypes.Add(GDTokenType.Whitespace, this.ClassificationRegistry.GetClassificationType("OILexer: Whitespace"));
                        this.classificationTypes.Add(GDTokenType.Keyword, this.ClassificationRegistry.GetClassificationType("OILexer: Preprocessor Directive"));
                        this.classificationTypes.Add(GDTokenType.Error, this.ClassificationRegistry.GetClassificationType("OILexer: Error"));
                        this.classificationTypes.Add(GDTokenType.SoftReference, this.classificationTypes[GDTokenType.Identifier]);
                    }
                    this._classificationTypes = new ReadOnlyDictionary<GDTokenType, IClassificationType>(this.classificationTypes);
                }
                return this._classificationTypes;
            }
        }


        private bool createdDocument = false;
        private object synchObject = new object();
        private Timer delayedReparseTimer = new Timer(500);
        private bool IsDirty
        {
            set
            {
                if (value)
                {
                    this.TokensInvalidated = true;
                    this.OutlinesInvalidated = true;
                }
            }
        }
        private bool tokensInvalid;
        private bool TokensInvalidated
        {
            get { return this.tokensInvalid; }
            set
            {
                if (!(this.ClassifierEnabled || this.ParserEnabled))
                    return;
                if (tokensInvalid == value)
                    return;
                this.tokensInvalid = value;
                if (value)
                    SetBuffer();
            }
        }
        private bool outlinesInvalidated;
        private bool OutlinesInvalidated
        {
            get { return this.outlinesInvalidated; }
            set
            {
                if (this.OutlinerEnabled)

                if (value == this.outlinesInvalidated)
                    return;
                this.outlinesInvalidated = value;
            }
        }

        private void ClearOutlines()
        {
            /* *
             * Clearing the outlines will indicate
             * that they have changed, and cause any 
             * active enumerator state machines to restart
             * and yield new elements that properly
             * correspond to the current state of the text.
             * */
            this.FinishedOutlining = false;
            lock (this.outliningSpans)
                this.outliningSpans.Clear();
            this.Parser.StreamPosition = 0;
        }

        protected override void SetBuffer()
        {
            this.BufferText = this.Buffer.CurrentSnapshot.GetText();
            var currentBuffer = new StringStream(this.BufferText);
            Lexer.SetStream(currentBuffer);
            /* *
             * Reset the timer on the reparse to ensure that
             * it doesn't repeat frequently.
             * */
            if (ParserEnabled && !CurrentlyParsing)
            {
                this.delayedReparseTimer.Stop();
                this.delayedReparseTimer.Start();
            }
        }

        internal void Reparse()
        {
            /* *
             * If there's presently a parse in progress,
             * exit.  Odds are this shouldn't happen, but
             * it could.  Overlapping parses could lead
             * to thread locks.  The active parse
             * will likely error out due to a rift
             * in the active parser state caused by the
             * text change delta.
             * *
             * Also, if there's no outliner, there's no need
             * to parse the file.
             * */
            if (this.CurrentlyParsing || 
               !this.OutlinerEnabled)
                return;
            this.CurrentlyParsing = true;
            this.ClearReclassifications();
            this.ClearOutlines();
            CurrentParseResults = this.Parser.BeginParse();
            this.ParseIncludes(this.CurrentParseResults.Result.Includes);
            (from handler in this.RelativeScopeFiles.Values
             select handler.CurrentParseResults.Result).Union(new IGDFile[] { this.CurrentParseResults.Result }).InitLookups(this.ResolutionAssistant);
            ((GDFile)this.CurrentParseResults.Result).ResolveTemplates(this.CurrentParseResults.Errors);
            ReclassifyTokens();
            ParserCompilerExtensions.ClearLookups();
            if (!creatingOutliner)
                this.Outliner.MakeTagChanges();
            this.ClearReclassifications();
            this.CurrentlyParsing = false;
        }

        protected internal void ParseIncludes(IList<string> fileIncludes)
        {
            Stack<Tuple<GDFileHandlerBase, IList<string>>> toParse = new Stack<Tuple<GDFileHandlerBase, IList<string>>>();
            toParse.Push(new Tuple<GDFileHandlerBase, IList<string>>(this, fileIncludes));
            do
            {
                var current = toParse.Pop();
                var currentElement = current.Item1;
                var currentSet = current.Item2;
                if (!this.includes.ContainsKey(currentElement))
                {
                    this.includes.Add(currentElement, fileIncludes);
                    foreach (var include in currentSet)
                    {
                        if (include == this.FileName)
                            continue;
                        if (!this.RelativeScopeFiles.ContainsKey(include))
                        {
                            var includedHandler = new GDFileStoredHandler(include, this, this.RelativeScopeFiles);
                            includedHandler.Reparse();
                            this.RelativeScopeFiles.Add(include, includedHandler);
                            toParse.Push(new Tuple<GDFileHandlerBase, IList<string>>(includedHandler, includedHandler.CurrentParseResults.Result.Includes));
                        }
                    }
                }
            }
            while (toParse.Count > 0);
        }

        /// <summary>
        /// Returns whether the current lexical analysis 
        /// pass through the buffer has completed.
        /// </summary>
        /// <remarks>
        /// Used to signify a full scan has been performed
        /// of the current buffer so the <see cref="Classifier"/>
        /// can properly yield a stopping point to the token
        /// cache.</remarks>
        public bool FinishedLexing { get; private set; }

        private Dictionary<IGDToken, GDTagSpan> tokens;
        private Dictionary<IGDRegion, GDOutlinerTagSpan> outliningSpans;


        private bool FinishedOutlining = false;
        internal IEnumerable<GDOutlinerTagSpan> OutliningSpansFrom(int index = 0, Span range = default(Span))
        {
            int offset = 0;
            if (index >= outliningSpans.Count)
            {
                if (this.FinishedOutlining)
                    yield break;
                else
                {
                    offset = this.outliningSpans.Count;
                    goto nextOutline;
                }
            }
            else
            {
                KeyValuePair<IGDRegion, GDOutlinerTagSpan>[] regionOutlineSpanPairs;
                lock (this.outliningSpans)
                    regionOutlineSpanPairs = this.outliningSpans.ToArray();
                foreach (var regionOutlineSpanPair in regionOutlineSpanPairs)
                {
                    var outlineSpan = regionOutlineSpanPair.Value;
                    if (offset++ >= index)
                    {
                        var currentSpan = outlineSpan.Span.Span;
                        if (range.End < currentSpan.Start)
                            continue;
                        if (currentSpan.IntersectsWith(range))
                            yield return outlineSpan;
                    }
                }
                if (FinishedOutlining)
                    yield break;
            }
            goto nextOutline;
        nextOutline:
            foreach (var region in this.CurrentParseResults.Result.Regions)
            {
                if (offset++ >= index)
                {
                    if (outliningSpans.ContainsKey(region))
                        continue;
                    var currentSpan = new Span(region.Start, region.End - region.Start);
                    var currentOutline = new GDOutlinerTagSpan(new SnapshotSpan(this.Buffer.CurrentSnapshot, new Span(region.Start, region.End - region.Start)), new GDOutliningTag(region));
                    lock (outliningSpans)
                        outliningSpans.Add(region, currentOutline);
                    if (currentSpan.IntersectsWith(range))
                        yield return currentOutline;
                }
            }
            FinishedOutlining = true;
        }

        public IEnumerable<GDTagSpan> TokensFrom(int index = 0, Span range = default(Span))
        {
        tokensInvalidatedCheck:
            if (this.TokensInvalidated)
            {
                this.FinishedLexing = false;
                lock (this.tokens)
                    this.tokens.Clear();
                this.TokensInvalidated = false;
            }
            int offset = 0;
            if (index >= tokens.Count)
                if (this.FinishedLexing)
                    yield break;
                else
                {
                    offset = this.tokens.Count;
                    goto nextToken;
                }
            else
            {
                GDTagSpan[] tokensArr;
                lock (this.tokens)
                    tokensArr = this.tokens.Values.ToArray();
                foreach (var currentSpan in tokensArr)
                {
                    if (offset++ >= index)
                    {
                        if (range.End < currentSpan.Span.Start)
                            yield break;
                        if (CustomIntersect(currentSpan.Span, range))
                            yield return currentSpan;
                    }
                }
                if (FinishedLexing)
                    yield break;
                goto nextToken;
            }
        nextToken:
            long lastGoodLocation = Lexer.Position;
            Lexer.NextToken();
            /* *
             * If the stream became dirty during parse, restart
             * tokenization.
             * */
            if (this.TokensInvalidated)
                goto tokensInvalidatedCheck;
            var currentToken = Lexer.CurrentToken;
            if (currentToken != null)
            {
                if (offset++ >= index)
                {
                    var currentSpan = new Span((int)currentToken.Position, (int)currentToken.Length);
                    if (tokens.ContainsKey(currentToken))
                        yield break;
                    var currentTagSpan = new GDTagSpan(new SnapshotSpan(this.Buffer.CurrentSnapshot, currentSpan), currentToken, this.ClassificationTypes[currentToken.TokenType]);
                    lock (this.tokens)
                    {
                        if (!this.tokens.ContainsKey(currentToken))
                            this.tokens.Add(currentToken, currentTagSpan);
                        else
                            goto nextToken;
                    }
                    if (range.End < currentSpan.Start)
                        yield break;
                    if (CustomIntersect(currentSpan, range))
                        yield return currentTagSpan;
                }
                goto nextToken;
            }
            //var currentError = Lexer.CurrentError;
            //if (currentError != null)
            //{
            //    var lastToken = new GDErrorToken(currentError, currentError.Line, currentError.Column, lastGoodLocation);
            //    var lastSpan = new Span((int)lastGoodLocation, Math.Abs((int)(range.End - lastGoodLocation)));
            //    if (range.End < lastSpan.Start)
            //        yield break;
            //    var lastTokenSpanPair = new KeyValuePair<IGDToken, Span>(lastToken, lastSpan);
            //    if (CustomIntersect(lastSpan, range))
            //        yield return lastTokenSpanPair;
            //}
            this.FinishedLexing = true;
        }

        /// <summary>
        /// Custom intersect used to determine whether two spans intersect but
        /// do not start where one ends or vice versa.  
        /// Used in cases where two tokens are justaposed next to one another
        /// and their spans report this.
        /// </summary>
        /// <param name="left">The <see cref="Span"/> for the left-most
        /// token in the current context.</param>
        /// <param name="right">The <see cref="Span"/> for the right-most
        /// token in the current context.</param>
        /// <returns></returns>
        private static bool CustomIntersect(Span left, Span right)
        {
            return (left.End != right.Start &&
                    left.Start != right.End) &&
                    left.IntersectsWith(right);
        }

        /// <summary>
        /// Returns the <see cref="String"/> representing the
        /// name of the file associated to the <see cref="GDFileHandlerBase"/>.
        /// </summary>
        public string FileName
        {
            get
            {
#if WINDOWS
                return this.Document.FilePath.ToLower();
#else
                return this.Document.FilePath;
#endif
            }
        }

        public IDictionary<string, GDFileStoredHandler> RelativeScopeFiles { get; private set; }

        public GDFileBufferedHandler(ITextBuffer buffer, ITextDocumentFactoryService documentFactory, ITextBufferFactoryService bufferFactory, IClassificationTypeRegistryService classificationRegistry, IContentTypeRegistryService contentTypeRegistry)
        {
            /* *
             * Initialize enumerator caches
             * */
            this.tokens = new Dictionary<IGDToken, GDTagSpan>();
            this.outliningSpans = new Dictionary<IGDRegion, GDOutlinerTagSpan>();

            this.includes = new Dictionary<GDFileHandlerBase, IList<string>>();

            /* *
             * It'd be bad if the parser repeated the parse each
             * iteration, but every half a second, assuming there's
             * enough pause, is okay.  If the person using the editor
             * needs to stop and think, the delay needed to reparse 
             * won't be that noticable.
             * */
            this.delayedReparseTimer.Stop();
            this.delayedReparseTimer.Elapsed += new ElapsedEventHandler(delayedReparseTimer_Elapsed);
            this.delayedReparseTimer.AutoReset = false;

            /* *
             * Includes cache dictionary.
             * */
            this.RelativeScopeFiles = new Dictionary<string, GDFileStoredHandler>(); 
            /* *
             * Content/classification registries, buffer factory, and so on.
             * */
            this.ClassificationRegistry = classificationRegistry;

            this.BufferFactory = bufferFactory;
            this.DocumentFactory = documentFactory;
            this.ContentTypeRegistry = contentTypeRegistry;
            this.Buffer = buffer;
            try
            {
                this.Document = buffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
            }
            catch (KeyNotFoundException)
            {
                this.Document = buffer.Properties.GetOrCreateSingletonProperty<ITextDocument>(() =>
                {
                    ITextDocument result;
                    documentFactory.TryGetTextDocument(this.Buffer, out result);
                    return result;
                });
            }
            this.Buffer.ChangedLowPriority += new EventHandler<TextContentChangedEventArgs>(Buffer_ChangedLowPriority);
        }

        void delayedReparseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Reparse.
            this.Reparse();
        }

        void Buffer_ChangedLowPriority(object sender, TextContentChangedEventArgs e)
        {
            if (e.After != this.Buffer.CurrentSnapshot)
                return;
            if (this.Buffer.EditInProgress)
                return;
            
            this.IsDirty = true;
        }

        #region IDisposable Members

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            if (this.resolutionAssistant != null)
            {
                this.resolutionAssistant.handler = null;
                this.resolutionAssistant = null;
            }
            this.delayedReparseTimer.Dispose();
            this.delayedReparseTimer = null;
            this.disposed = true;
            this.Buffer.ChangedLowPriority -= new EventHandler<TextContentChangedEventArgs>(Buffer_ChangedLowPriority);
            var bufferPropKey = typeof(GDFileHandlerBase);
            if (this.Buffer.Properties.ContainsProperty(bufferPropKey))
                this.Buffer.Properties.RemoveProperty(bufferPropKey);
            if (this.createdDocument)
            {
                if (this.Document != null)
                {
                    this.Document.Dispose();
                    this.Document = null;
                }
            }
            var relativeHelperCopy = new Dictionary<string, GDFileStoredHandler>(RelativeScopeFiles);
            RelativeScopeFiles.Clear();
            foreach (var helperFile in relativeHelperCopy.Keys)
                    relativeHelperCopy[helperFile].Dispose();
            if (this.DocumentFactory != null)
                this.DocumentFactory = null;
            if (this.BufferFactory != null)
                this.BufferFactory = null;
            this.outliningSpans.Clear();
            this.tokens.Clear();
            this.RelativeScopeFiles = null;
            if (this.Buffer != null)
                this.Buffer = null;
            this.ClassificationRegistry = null;
            this.BufferText = null;
            this.CurrentParseResults = null;
            this.tokens = null;
            this.outliningSpans = null;
        }

        #endregion

        internal void OnClassifierDisposed()
        {
            this.classifier = null;
            Trace.WriteLine("Classifier Disposed.");
            this.DisposeCheck();
        }

        internal void OnOutlinerDisposed()
        {
            this.outliningSpans.Clear();
            this.parser = null;
            this.outliner = null;
            Trace.WriteLine("Outliner Disposed.");

            this.DisposeCheck();
        }

        private void DisposeCheck()
        {
            if (this.NeedsDisposed)
                this.Dispose();
        }

        public bool NeedsDisposed
        {
            get
            {
                return this.classifier == null &&
                       this.outliner == null;
            }
        }

        public bool CurrentlyParsing { get; set; }

        internal GDOutliner CreateOutliner()
        {
            if (this.outliner != null)
                this.outliner.RelaxDispose();
            return this.Outliner;
        }

        internal GDClassifier CreateClassifier()
        {
            if (this.classifier != null)
                this.classifier.RelaxDispose();
            return this.Classifier;

        }

        protected override void PerformReclassification(IGDToken token, GDTokenType newClassification)
        {
            var currentTokenTag = this.tokens[token];
            var newTokenTag = new GDTagSpan(currentTokenTag.Span, token, this.classificationTypes[newClassification]);
            this.tokens[token] = newTokenTag;
        }

        public sealed class SimpleParser
            : BaseParser
        {
            internal SimpleParser(GDFileBufferedHandler handler)
                : base(handler, true)
            {
                ((Lexer)this.CurrentTokenizer).FileName = this.Handler.FileName;
            }

            protected new GDFileBufferedHandler Handler
            {
                get
                {
                    return (GDFileBufferedHandler)base.Handler;
                }
            }
            protected override IToken LookAheadImpl(int howFar)
            {
                int bufferLength = this.Handler.BufferText.Length;
                if (this.originalFormTokens.Count <= howFar)
                {
                    int index = this.originalFormTokens.Count;
                    var lastElement = this.originalFormTokens.Count > 0 ? this.originalFormTokens[this.originalFormTokens.Count - 1] : null;
                    int startLocale = (int)(lastElement == null ? this.StreamPosition : (lastElement.Position + lastElement.Length));
                    foreach (var tokenTagSpan in this.Handler.TokensFrom(range: new Span((int)startLocale, (int)Math.Max(bufferLength, startLocale) - (int)startLocale)))
                    {
                        this.originalFormTokens.Add(tokenTagSpan.Tag.Token);
                        if (index == howFar)
                            break;
                    }
                }
                lock (this.originalFormTokens)
                    if (howFar < this.originalFormTokens.Count)
                        return this.originalFormTokens[howFar];
                    else
                    {
                        if (this.StreamPosition < bufferLength)
                            this.StreamPosition++;
                        return null;
                    }
            }

            public override char TokenizerLookAhead(int howFar)
            {
                int target = ((int)(this.StreamPosition + howFar));
                if (this.Handler.BufferText.Length <= target)
                    return char.MinValue;
                return this.Handler.BufferText[target];
            }

            internal override void SkipWhitespace()
            {
                /* *
                 * The parser automatically skips whitespace, the 'simple'
                 * parser is simple because it operates off of a stream
                 * of tokens versus obtaining them itself.
                 * *
                 * Since, in most cases the tokens are already parsed, requesting
                 * the parser to move forward when a space is encountered would distort
                 * the stream the file handler functions upon.
                 * *
                 * The trick is to shift the position based upon the actual position
                 * of the first token available, which will not be whitespace, its position
                 * will be the desired target.
                 * */
                var firstToken = this.LookAhead(0);
                if (firstToken == null)
                {
                    if (this.TokenizerLookAhead(0) != char.MinValue)
                        this.StreamPosition++;
                    return;
                }
                if (this.StreamPosition < firstToken.Position)
                    this.StreamPosition = firstToken.Position;
            }

            public override long StreamPosition { get; set; }

            internal override void SetMultiLineMode(bool value)
            {
                /* *
                 * The altered form tokenizer messes up the multiline awareness
                 * */
            }

            protected override void DefineRuleIdentifier(GDTokens.IdentifierToken ruleIdentifier, IProductionRuleEntry ruleEntry)
            {
                this.Handler.ReclassifyToken(ruleIdentifier, ruleEntry);
            }

            protected override void DefineTokenIdentifier(GDTokens.IdentifierToken tokenIdentifier, ITokenEntry tokenEntry)
            {
                this.Handler.ReclassifyToken(tokenIdentifier, tokenEntry);
            }

            protected override void DefineRuleTemplateIdentifier(GDTokens.IdentifierToken ruleTemplateIdentifier, IProductionRuleTemplateEntry ruleTemplateEntry)
            {
                this.Handler.ReclassifyToken(ruleTemplateIdentifier, ruleTemplateEntry);
            }

            protected override void DefineRuleTemplateParameterIdentifier(GDTokens.IdentifierToken id, IProductionRuleTemplatePart currentPart)
            {
                this.Handler.ReclassifyToken(id, currentPart);
            }


            protected override void DefineRuleSoftReference(GDTokens.IdentifierToken primary, GDTokens.IdentifierToken secondary = null)
            {
                primary.SetTokenType(GDTokenType.SoftReference);
                if (secondary != null)
                    secondary.SetTokenType(GDTokenType.SoftReference);
            }

            protected override void DefineBooleanIdentifier(GDTokens.IdentifierToken booleanIdentifier)
            {
                booleanIdentifier.SetTokenType(GDTokenType.Keyword);
            }

            protected override void DefineCommandIdentifier(GDTokens.IdentifierToken commandIdentifier)
            {
                commandIdentifier.SetTokenType(GDTokenType.Keyword);
            }

        }

        protected override BaseParser InitializeParser()
        {
            return new SimpleParser(this);
        }
    }
}
