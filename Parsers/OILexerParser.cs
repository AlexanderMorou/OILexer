using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System.Diagnostics;
using AllenCopeland.Abstraction.Utilities.Collections;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers
{
    public partial class OilexerParser :
        Parser<IOilexerGrammarToken, IOilexerGrammarTokenizer, IOilexerGrammarFile>,
        IOilexerGrammarParser
    {
        private List<string> includeDirectives = new List<string>();
        private List<string> parsedIncludeDirectives = new List<string>();
        private List<int> commandDepths = new List<int>();
        private OilexerGrammarParserResults currentTarget;
        private int templateDepth = 0;
        private int parameterDepth = 0;
        private IDictionary<string, string> definedSymbols = new Dictionary<string, string>();
        private delegate bool SimpleParseDelegate<T>(ref T target);
        private Dictionary<string, TimeSpan> parseTimes = new Dictionary<string, TimeSpan>();
        private Dictionary<string, TimeSpan> previousParseTimes = null;
        private bool parseIncludes;
        private bool captureRegions;
        public OilexerParser()
            : this(true)
        {

        }
        public OilexerParser(bool parseIncludes, bool captureRegions = false, IList<IToken> originalFormTokens = null)
            : base(originalFormTokens)
        {
            this.parseIncludes = parseIncludes;
            this.captureRegions = captureRegions;
        }

        public void Define(string directiveName)
        {
            Define(directiveName, null);
        }

        public void Define(string directiveName, string directiveValue)
        {
            if (!definedSymbols.ContainsKey(directiveName))
                definedSymbols.Add(directiveName, directiveValue);
            else
                throw new ArgumentException("Directive already exists.");
        }

        /// <summary>
        /// Parses the <paramref name="fileName"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>An instance of an implementation of <see cref="IParserResults{T}"/> 
        /// that indicates the success of the operation, with an instance of 
        /// <see cref="IOilexerGrammarFile"/>, if successful.</returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown when 
        /// <paramref name="fileName"/> is not found.</exception>
        public override IParserResults<IOilexerGrammarFile> Parse(string fileName)
        {
            if (!Path.IsPathRooted(fileName))
                fileName = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The file to parse was not found.", fileName);
#if WINDOWS
            /* *
             * Users on windows should be used to case leniency, not doing this
             * could lead to locked file runtime errors, when the same file is
             * parsed twice.  Notably when there are case inconsistencies in the
             * grammar itself or the application is invoked using a case variant
             * of the actual file name.
             * */
            fileName = fileName.GetFilenameProperCasing();
#endif
            IParserResults<IOilexerGrammarFile> result = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            result = this.Parse(fs, fileName);
            fs.Close();
            if (result.Result != null)
            {
                var resultFile = (OilexerGrammarFile)result.Result;
                resultFile.DefinedSymbols = this.definedSymbols;
            }
            return result;
        }

        public IEnumerable<Tuple<string, TimeSpan>> GetParseTimes()
        {
            foreach (var fileName in this.previousParseTimes.Keys.OrderBy(k=>Path.GetFileName(k)))
                yield return Tuple.Create(fileName, this.previousParseTimes[fileName]);
        }

        /// <summary>
        /// Parses the <paramref name="stream"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="s">The stream to parse.</param>
        /// <param name="fileName">The file name used provided an error is encountered in the <see cref="Stream"/>, <paramref name="s"/>.</param>
        /// <returns>An instance of an implementation of <see cref="IParserResults{T}"/> 
        /// that indicates the success of the operation with an instance of 
        /// <see cref="IOilexerGrammarFile"/>, if successful.</returns>
        public override IParserResults<IOilexerGrammarFile> Parse(Stream s, string fileName)
        {
            bool startedParseSet = parseTimes.Count == 0;
            this.parseTimes[fileName] = TimeSpan.Zero;
            Stopwatch sw = Stopwatch.StartNew();
            int count = includeDirectives.Count;
            bool addedInclude = false;
            base.CurrentTokenizer = new Lexer(s, fileName);
            //If the include already exists in the include directives, this is 
            //a recursive call.
            if (!(includeDirectives.Contains(fileName)))
            {
                includeDirectives.Add(fileName);
                addedInclude = true;
                parsedIncludeDirectives.Add(fileName);
            }
            IParserResults<IOilexerGrammarFile> gf = this.BeginParse();
            /* *
             * Only remove it if it was added by this call.  Which would be when
             * Count = initial_Count, this should effectively clean out the 
             * parsed/include directive listing.
             * */ 
            if (addedInclude)
                for (int i = count; i < includeDirectives.Count; i++)
                {
                    if (parsedIncludeDirectives.Contains(includeDirectives[i]))
                        parsedIncludeDirectives.Remove(includeDirectives[i]);
                    includeDirectives.RemoveAt(i--);
                }
            s.Close();
            sw.Stop();
            var myIndex = parseTimes.Keys.GetIndexOf(fileName);
            if (myIndex == parseTimes.Count - 1)
                parseTimes[fileName] = sw.Elapsed;
            else
                parseTimes[fileName] = sw.Elapsed - parseTimes.Values.Skip(myIndex + 1).Aggregate((a, b) => a + b);

            if (startedParseSet)
            {
                previousParseTimes = parseTimes;
                parseTimes = new Dictionary<string, TimeSpan>();
            }
            return gf;
        }

        /* *
         * Initializes the parse process.
         * */
        protected IParserResults<IOilexerGrammarFile> BeginParse()
        {
            //Instantiate the parser results and give it a blank file to work with.
            currentTarget = new OilexerGrammarParserResults();
            currentTarget.SetResult(new OilexerGrammarFile(this.CurrentTokenizer.FileName));
            while (true)
            {
                SetMultiLineMode(true);
                //Clear the ahead, hack.
                this.ClearAhead();
                char c = LookPastAndSkip();

                //EOF
                if (c == char.MinValue)
                    break;

                SetMultiLineMode(false);
                //Expected comment.
                if (c == '/')
                {
                    SetMultiLineMode(true);
                    var its = this.GetAhead(1);
                    if (its.Count == 0)
                    {
                        Expect("comment");
                        PopAhead();
                        continue;
                    }
                    IOilexerGrammarToken it = its[0];
                    if (it == null || it.TokenType != OilexerGrammarTokenType.Comment)
                    {
                        Expect("comment");
                        PopAhead();
                    }
                    var commentToken = ((OilexerGrammarTokens.CommentToken)(it));
                    if (captureRegions && commentToken.MultiLine)
                        (this.currentTarget.Result as OilexerGrammarFile).AddCommentRegion(commentToken);
                    this.currentTarget.Result.Add(new OilexerGrammarCommentEntry(commentToken.Comment, CurrentTokenizer.FileName, it.Column, it.Line, it.Position));
                }
                //Expected preprocessor.
                else if (c == '#')
                {
                    OilexerGrammarTokens.PreprocessorDirective preprocessorToken = this.LookAhead(0) as OilexerGrammarTokens.PreprocessorDirective;
                    if (preprocessorToken != null)
                    {
                        switch (preprocessorToken.Type)
                        {
                            case OilexerGrammarTokens.PreprocessorType.IfDirective:
                            case OilexerGrammarTokens.PreprocessorType.IfDefinedDirective:
                            case OilexerGrammarTokens.PreprocessorType.IfNotDefinedDirective:
                                PopAhead();
                                IPreprocessorCLogicalOrConditionExp condition = null;
                                if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                                {
                                    var ifEntry = ParseIfDirective(condition, preprocessorToken, PreprocessorContainer.File);
                                    OilexerGrammarProcessIfEntry(ifEntry);
                                }
                                break;
                            case OilexerGrammarTokens.PreprocessorType.DefineDirective:
                                IPreprocessorDirective ipd = ParsePreprocessor(PreprocessorContainer.File);
                                var defineDirective = ipd as IPreprocessorDefineSymbolDirective;
                                if (defineDirective != null)
                                    if (!definedSymbols.ContainsKey(defineDirective.SymbolName))
                                        definedSymbols.Add(defineDirective.SymbolName, defineDirective.Value);
                                break;
                            case OilexerGrammarTokens.PreprocessorType.ThrowDirective:
                                break;
                            default:
                                goto parseSimple;
                        }
                        goto postSimple;
                        ;
                    parseSimple:
                        ProcessStringTerminal(ParseStringTerminalDirective());
                    postSimple:
                        ;

                    }
                    else
                    {
                        Expect("#if, #throw, #ifdef, #ifndef, #endif");
                        break;
                    }
                }
                else if (Lexer.IsIdentifierChar(c))
                {
                    /* *
                     * All declarations must begin at the firstSeries column 
                     * of the line.
                     * */
                    if (this.CurrentTokenizer.GetColumnIndex(this.StreamPosition) != 1)
                    {
                        LogError(OilexerGrammarParserErrors.ExpectedEndOfLine);
                        GetAhead(1);
                        continue;
                    }
                    SetMultiLineMode(true);
                    EntryScanMode esm = EntryScanMode.Inherited;
                    var idOp = GetAhead<OilexerGrammarTokens.IdentifierToken, OilexerGrammarTokens.OperatorToken>(2);
                    /* *
                     * expected patterns:
                     * ID   = | * Language defined error 
                     * ID  := | * Token entry
                     * ID ::= | * Production entry
                     * ID   < | * Template begin
                     * ID   > | * Token with precedence information defined.
                     * ID   + | * Change line-mode to multi-line (not used)
                     * ID   -   * Change line-mode to single-line (not used)
                     * */
                    if (idOp.FailPoint == -1)
                    //if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
                    {
                        List<OilexerGrammarTokens.IdentifierToken> lowerPrecedences = new List<OilexerGrammarTokens.IdentifierToken>();
                        //OilexerGrammarTokens.IdentifierToken id = idOp.Token1;
                        OilexerGrammarTokens.OperatorToken ot = idOp.Token2;
                        bool elementsAreChildren = false;
                        bool forcedRecognizer = false;
                    checkOp:
                        switch (ot.Type)
                        {
                            //ID$
                            case OilexerGrammarTokens.OperatorType.ForcedStringForm:
                                forcedRecognizer = true;
                                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Operator)
                                {
                                    ot = (OilexerGrammarTokens.OperatorToken)PopAhead();
                                    goto checkOp;
                                }
                                else
                                {
                                    ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.GreaterThan, true);
                                    PopAhead();
                                }
                                break;
                            //ID ::=
                            case OilexerGrammarTokens.OperatorType.ColonColonEquals:
                                if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                    ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                {
                                    elementsAreChildren = true;
                                    PopAhead();
                                }
                                SetMultiLineMode(true);
                                ParseProductionRule(idOp.Token1, EntryScanMode.Inherited, idOp.Token1.Position, elementsAreChildren, forcedRecognizer, null);
                                break;
                            //ID :=
                            case OilexerGrammarTokens.OperatorType.ColonEquals:
                                {
                                    SetMultiLineMode(true);
                                    bool unhinged = false, contextual = false;
                                    if (TokenizerLookAhead(0) == '*')
                                    {
                                        //ID :=*
                                        /* *
                                         * Token is unbounded by grammar
                                         * and can appear anywhere.
                                         * */
                                        this.PopAhead();
                                        unhinged = true;
                                        if (TokenizerLookAhead(0) == '-')
                                        {
                                            //ID :=*-
                                            /* *
                                             * It's not valid to have both unhinged and contextual.
                                             * */
                                            LogError(OilexerGrammarParserErrors.Unexpected, "-");
                                            this.PopAhead();
                                        }
                                    }
                                    if (TokenizerLookAhead(0) == '-')
                                    {
                                        //ID :=-
                                        /* *
                                         * Token is a contextual token and might be ambiguous with others
                                         * special care needs taken when handling them to avoid incorrect
                                         * parse paths.
                                         * */
                                        this.PopAhead();
                                        contextual = true;
                                        if (TokenizerLookAhead(0) == '*')
                                        {
                                            //ID :=-*
                                            /* *
                                             * It's not valid to have both contextual and unhinged.
                                             * */
                                            LogError(OilexerGrammarParserErrors.Unexpected, "*");
                                            this.PopAhead();
                                        }
                                    }
                                    ParseToken(idOp.Token1, EntryScanMode.Inherited, idOp.Token1.Position, unhinged, lowerPrecedences, forcedRecognizer, contextual);
                                }
                                break;
                            //ID =
                            case OilexerGrammarTokens.OperatorType.Equals:
                                ParseError(idOp.Token1.Name, idOp.Token1.Position);
                                break;
                            //ID<
                            case OilexerGrammarTokens.OperatorType.LessThan:
                                SetMultiLineMode(true);
                                IList<IProductionRuleTemplatePart> parts = this.ParseProductionRuleTemplateParts(ot);
                                if (parts == null)
                                    continue;
                                else
                                {
                                    IOilexerGrammarToken g = null;
                                    OilexerGrammarTokens.OperatorToken gdt = (g = PopAhead()) as OilexerGrammarTokens.OperatorToken;
                                    if (gdt == null)
                                        ExpectOperator(g, OilexerGrammarTokens.OperatorType.ColonColonEquals | OilexerGrammarTokens.OperatorType.Plus | OilexerGrammarTokens.OperatorType.Minus, true);
                                    //Expect("+, -, or ::=");
                                    if (gdt.Type == OilexerGrammarTokens.OperatorType.Minus)
                                    {
                                        gdt = (g = PopAhead()) as OilexerGrammarTokens.OperatorToken;
                                        esm = EntryScanMode.SingleLine;
                                    }
                                    else if (gdt.Type == OilexerGrammarTokens.OperatorType.Plus)
                                    {
                                        gdt = (g = PopAhead()) as OilexerGrammarTokens.OperatorToken;
                                        esm = EntryScanMode.Multiline;
                                    }
                                    if (gdt == null || !ExpectOperator(gdt, OilexerGrammarTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        ExpectOperator(g, OilexerGrammarTokens.OperatorType.ColonColonEquals, true);
                                        //Expect("::=");
                                        break;
                                    }
                                    this.ParseProductionRuleTemplate(idOp.Token1, parts, esm, idOp.Token1.Position);
                                }
                                break;
                            //ID>
                            case OilexerGrammarTokens.OperatorType.GreaterThan:
                                //Case of a token defining precedence.
                                while (LookAhead(0).TokenType == OilexerGrammarTokenType.Identifier)
                                {
                                    var lowerPrecedenceToken = ((OilexerGrammarTokens.IdentifierToken)LookAhead(0));
                                    lowerPrecedences.Add(lowerPrecedenceToken);
                                    PopAhead();
                                    OilexerGrammarTokens.OperatorToken cOp = null;
                                    //','  * More precedences exist.
                                    if ((cOp = (LookAhead(0) as OilexerGrammarTokens.OperatorToken)) != null &&
                                        cOp.Type == OilexerGrammarTokens.OperatorType.Comma)
                                        PopAhead();
                                    //":=" * Final precedence reached.
                                    else if (cOp != null &&
                                        cOp.Type == OilexerGrammarTokens.OperatorType.ColonEquals)
                                    {
                                        PopAhead();
                                        goto case OilexerGrammarTokens.OperatorType.ColonEquals;
                                    }
                                    else
                                    {
                                        //Syntax error.
                                        ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.Comma, true);
                                        break;
                                    }
                                }
                                break;
                            //ID{
                            case OilexerGrammarTokens.OperatorType.LeftCurlyBrace:
                                var curlyAhead = PopAhead();
                                if (curlyAhead.TokenType == OilexerGrammarTokenType.NumberLiteral)
                                {
                                    var curlyNumber = curlyAhead as OilexerGrammarTokens.NumberLiteral;
                                    IOilexerGrammarToken colonColonEq = null;
                                    if (ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.RightCurlyBrace, true) &&
                                        ExpectOperator(colonColonEq = PopAhead(), OilexerGrammarTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        SetMultiLineMode(true);
                                        if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                            ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                        {
                                            elementsAreChildren = true;
                                            PopAhead();
                                        }
                                        ParseProductionRule(idOp.Token1, esm, idOp.Token1.Position, elementsAreChildren, forcedRecognizer, curlyNumber.GetCleanValue());
                                        break;
                                    }
                                }
                                else
                                    Expect("Number");
                                break;
                            //ID-
                            case OilexerGrammarTokens.OperatorType.Minus:
                            //ID+
                            case OilexerGrammarTokens.OperatorType.Plus:
                                esm = ot.Type == OilexerGrammarTokens.OperatorType.Minus ? EntryScanMode.SingleLine : EntryScanMode.Multiline;
                                IOilexerGrammarToken ahead = PopAhead();
                                if (ahead.TokenType == OilexerGrammarTokenType.Operator)
                                {
                                    ot = ((OilexerGrammarTokens.OperatorToken)(ahead));
                                    switch (ot.Type)
                                    {
                                        case OilexerGrammarTokens.OperatorType.ColonColonEquals:
                                            SetMultiLineMode(true);
                                            if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                                ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                            {
                                                elementsAreChildren = true;
                                                PopAhead();
                                            }
                                            ParseProductionRule(idOp.Token1, esm, idOp.Token1.Position, elementsAreChildren, forcedRecognizer, null);
                                            break;
                                        case OilexerGrammarTokens.OperatorType.ColonEquals:
                                            {
                                                SetMultiLineMode(true);
                                                bool unhinged = false, contextual = false;
                                                if (TokenizerLookAhead(0) == '*')
                                                {
                                                    //ID :=*
                                                    /* *
                                                     * Token is unbounded by grammar
                                                     * and can appear anywhere.
                                                     * */
                                                    this.PopAhead();
                                                    unhinged = true;
                                                    if (TokenizerLookAhead(0) == '-')
                                                    {
                                                        //ID :=*-
                                                        /* *
                                                         * It's not valid to have both unhinged and contextual.
                                                         * */
                                                        LogError(OilexerGrammarParserErrors.Unexpected, "-");
                                                        this.PopAhead();
                                                    }
                                                }
                                                if (TokenizerLookAhead(0) == '-')
                                                {
                                                    //ID :=-
                                                    /* *
                                                     * Token is a contextual token and might be ambiguous with others
                                                     * special care needs taken when handling them to avoid incorrect
                                                     * parse paths.
                                                     * */
                                                    this.PopAhead();
                                                    contextual = true;
                                                    if (TokenizerLookAhead(0) == '*')
                                                    {
                                                        //ID :=-*
                                                        /* *
                                                         * It's not valid to have both contextual and unhinged.
                                                         * */
                                                        LogError(OilexerGrammarParserErrors.Unexpected, "*");
                                                        this.PopAhead();
                                                    }
                                                }
                                                ParseToken(idOp.Token1, esm, idOp.Token1.Position, unhinged, lowerPrecedences, forcedRecognizer, contextual);
                                            }
                                            break;
                                        case OilexerGrammarTokens.OperatorType.Equals:
                                            break;
                                    }
                                }
                                else
                                    ExpectOperator(ahead, OilexerGrammarTokens.OperatorType.ColonColonEquals | OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.Equals, true);
                                break;
                            default:
                                ExpectOperator(ot, OilexerGrammarTokens.OperatorType.ColonColonEquals | OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.Equals | OilexerGrammarTokens.OperatorType.LessThan | OilexerGrammarTokens.OperatorType.Minus | OilexerGrammarTokens.OperatorType.Plus, true);
                                break;
                        }
                    }
                    else
                    {
                        var k = LookAhead(1);
                        ExpectOperator(k, OilexerGrammarTokens.OperatorType.ColonColonEquals | OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.Equals | OilexerGrammarTokens.OperatorType.LessThan | OilexerGrammarTokens.OperatorType.Minus | OilexerGrammarTokens.OperatorType.Plus, true);
                        PopAhead();
                        continue;
                    }
                }
                else if (Lexer.IsWhitespaceChar(c))
                    //Skip
                    SkipWhitespace();
                else
                {
                    LogError(OilexerGrammarParserErrors.ExpectedEndOfLine);
                    GetAhead(1);
                    break;
                }
            }
            OilexerGrammarParserResults gdpr = currentTarget;
            currentTarget = null;
            if (this.parseIncludes)
                ParseIncludes(gdpr);
            return gdpr;
        }

        internal virtual void SkipWhitespace()
        {
            ((Lexer)(this.CurrentTokenizer)).ParseWhitespaceInternal();
        }

        private void OilexerGrammarProcessIfEntry(IPreprocessorIfDirective ifEntry)
        {
            OilexerGrammarProcessIfEntry(ifEntry, PreprocessorContainer.File, (IOilexerGrammarEntry p) => this.currentTarget.Result.Add(p));
        }

        private void OilexerGrammarProcessIfEntry<T>(IPreprocessorIfDirective ifEntry, PreprocessorContainer container, Action<T> adder)
            where T :
                class
        {
            switch (ifEntry.Type)
            {
                case EntryPreprocessorType.If:
                case EntryPreprocessorType.ElseIf:
                    if (ProcessCondition(ifEntry.Condition))
                        ProcessIfBody(ifEntry.Body, container, adder);
                    else if (ifEntry.Next != null)
                        OilexerGrammarProcessIfEntry(ifEntry.Next);
                    break;
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIfDefined:
                    bool check = ProcessDefinedCondition(ifEntry.Condition, ifEntry.Type != EntryPreprocessorType.IfNotDefined);
                    if (check)
                        ProcessIfBody(ifEntry.Body, container, adder);
                    else if (ifEntry.Next != null)
                        OilexerGrammarProcessIfEntry(ifEntry.Next, container, adder);
                    break;
                case EntryPreprocessorType.Else:
                    ProcessIfBody(ifEntry.Body, container, adder);
                    break;
            }
        }

        private bool ProcessCondition(IPreprocessorCLogicalOrConditionExp orExp)
        {
            if (orExp.Left == null)
                return ProcessCondition(orExp.Right);
            else
                return ProcessCondition(orExp.Left) || ProcessCondition(orExp.Right);
        }

        private bool ProcessCondition(IPreprocessorCLogicalAndConditionExp andExp)
        {
            if (andExp.Left == null)
                return ProcessCondition(andExp.Right);
            else
                return ProcessCondition(andExp.Left) || ProcessCondition(andExp.Right);
        }

        private bool ProcessCondition(IPreprocessorCEqualityExp eqExp)
        {
            // 1 - PreprocessorCEqualityExp "==" PreprocessorCPrimary, 
            // 2 - PreprocessorCEqualityExp "!=" PreprocessorCPrimary,
            // 3 - PreprocessorCPrimary</remarks>
            switch (eqExp.Rule)
            {
                case 1:
                case 2:
                    if (eqExp.PreCEqualityExp.Rule != 3 ||
                        !(eqExp.PreCEqualityExp.PreCPrimary.Rule == 1 ||
                          eqExp.PreCEqualityExp.PreCPrimary.Rule == 4))
                        Expect("identifier or string", eqExp.PreCEqualityExp.Position);
                    if (!(eqExp.PreCPrimary.Rule == 1 ||
                          eqExp.PreCPrimary.Rule == 4))
                        Expect("identifier or string", eqExp.PreCPrimary.Position);
                    string left = null,
                           right = null;
                    if (eqExp.PreCEqualityExp.PreCPrimary.Rule == 1)
                        left = eqExp.PreCEqualityExp.PreCPrimary.String.GetCleanValue();
                    else if (eqExp.PreCEqualityExp.PreCPrimary.Rule == 4)
                    {
                        left = eqExp.PreCEqualityExp.PreCPrimary.Identifier.Name;
                        if (definedSymbols.ContainsKey(left))
                            left = definedSymbols[left];
                        else
                        {
                            LogError(OilexerGrammarParserErrors.UnknownSymbol, left, eqExp.PreCEqualityExp.PreCPrimary.Identifier.Position);
                            return false;
                        }
                    }
                    if (eqExp.PreCPrimary.Rule == 1)
                        right = eqExp.PreCPrimary.String.GetCleanValue();
                    else if (eqExp.PreCPrimary.Rule == 4)
                    {
                        right = eqExp.PreCPrimary.Identifier.Name;
                        if (definedSymbols.ContainsKey(right))
                            right = definedSymbols[right];
                        else
                        {
                            LogError(OilexerGrammarParserErrors.UnknownSymbol, right, eqExp.PreCPrimary.Identifier.Position);
                            return false;
                        }
                    }
                    if (eqExp.Rule == 1)
                        return left == right;
                    else
                        return left != right;
                case 3:
                    ExpectOperator(eqExp.PreCPrimary.Token, OilexerGrammarTokens.OperatorType.EqualEqual | OilexerGrammarTokens.OperatorType.ExclaimEqual, true);
                    return false;
                default:
                    LogError(OilexerGrammarParserErrors.Unexpected, "Unexpected kind of equality expression", eqExp.Position);
                    return false;
            }
        }


        private void ProcessIfBody<T>(IPreprocessorDirectives body, PreprocessorContainer container, Action<T> adder)
        {
            foreach (var item in body)
            {
                if (item == null)
                    continue;
                switch (item.Type)
                {
                    case EntryPreprocessorType.If:
                    case EntryPreprocessorType.IfNotDefined:
                    case EntryPreprocessorType.IfDefined:
                        OilexerGrammarProcessIfEntry((IPreprocessorIfDirective)item);
                        break;
                    case EntryPreprocessorType.DefineRule:
                        LogError(OilexerGrammarParserErrors.Unexpected, "Unexpected rule declaration command.", item.Position);
                        break;
                    case EntryPreprocessorType.AddRule:
                        LogError(OilexerGrammarParserErrors.Unexpected, "Unexpected add rule command.", item.Position);
                        break;
                    case EntryPreprocessorType.Throw:
                        throw new NotImplementedException();
                    case EntryPreprocessorType.Return:
                        LogError(OilexerGrammarParserErrors.Unexpected, "Unexpected return command.", item.Position);
                        break;
                    case EntryPreprocessorType.StringTerminal:
                        if (container == PreprocessorContainer.File)
                            ProcessStringTerminal((IPreprocessorStringTerminalDirective)(item));
                        //ToDo: Add code here to handle String Termianls within conditions.
                        break;
                    case EntryPreprocessorType.EntryContainer:
                        if (container == PreprocessorContainer.File)
                            adder(((T)(((PreprocessorEntryContainer)(item)).Contained)));
                        break;
                    case EntryPreprocessorType.DefineSymbol:
                        if (container == PreprocessorContainer.File)
                        {
                            IPreprocessorDefineSymbolDirective defineDirective = ((IPreprocessorDefineSymbolDirective)(item));
                            if (!definedSymbols.ContainsKey(defineDirective.SymbolName))
                                definedSymbols.Add(defineDirective.SymbolName, defineDirective.Value);
                            else
                                LogError(OilexerGrammarParserErrors.Unexpected, "define directive: symbol already exists", defineDirective.Position);
                        }
                        break;
                    case EntryPreprocessorType.ProductionRuleSeries:
                        if (container == PreprocessorContainer.Rule)
                        {
                            foreach (var currentRule in ((PreprocessorProductionRuleSeries)(item)))
                                adder((T)(currentRule));
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ProcessStringTerminal(IPreprocessorStringTerminalDirective item)
        {
            if (item == null)
                return;
            var stringTerminal = item;
            switch (stringTerminal.Kind)
            {
                case StringTerminalKind.Unknown:
                    LogError(OilexerGrammarParserErrors.UnknownSymbol, stringTerminal.Position);
                    break;
                case StringTerminalKind.Include:
                    ParseInclude(stringTerminal);
                    break;
                case StringTerminalKind.Root:
                    currentTarget.Result.Options.StartEntry = stringTerminal.Literal;
                    break;
                case StringTerminalKind.AssemblyName:
                    currentTarget.Result.Options.AssemblyName = stringTerminal.Literal;
                    break;
                case StringTerminalKind.LexerName:
                    currentTarget.Result.Options.LexerName = stringTerminal.Literal;
                    break;
                case StringTerminalKind.GrammarName:
                    currentTarget.Result.Options.GrammarName = stringTerminal.Literal;
                    break;
                case StringTerminalKind.ParserName:
                    currentTarget.Result.Options.ParserName = stringTerminal.Literal;
                    break;
                case StringTerminalKind.TokenPrefix:
                    currentTarget.Result.Options.TokenPrefix = stringTerminal.Literal;
                    break;
                case StringTerminalKind.TokenSuffix:
                    currentTarget.Result.Options.TokenSuffix = stringTerminal.Literal;
                    break;
                case StringTerminalKind.RulePrefix:
                    currentTarget.Result.Options.RulePrefix = stringTerminal.Literal;
                    break;
                case StringTerminalKind.RuleSuffix:
                    currentTarget.Result.Options.RuleSuffix = stringTerminal.Literal;
                    break;
                case StringTerminalKind.Namespace:
                    currentTarget.Result.Options.Namespace = stringTerminal.Literal;
                    break;
                default:
                    return;
            }
        }

        private bool ProcessDefinedCondition(IPreprocessorCLogicalOrConditionExp orExp, bool checkForExists)
        {
            if (orExp.Left == null)
                return ProcessDefinedCondition(orExp.Right, checkForExists);
            else
                return ProcessDefinedCondition(orExp.Left, checkForExists) || ProcessDefinedCondition(orExp.Right, checkForExists);
        }

        private bool ProcessDefinedCondition(IPreprocessorCLogicalAndConditionExp andExp, bool checkForExists)
        {
            if (andExp.Left == null)
                return ProcessDefinedCondition(andExp.Right, checkForExists);
            else
                return ProcessDefinedCondition(andExp.Left, checkForExists) && ProcessDefinedCondition(andExp.Right, checkForExists);
        }

        private bool ProcessDefinedCondition(IPreprocessorCEqualityExp equalityExp, bool checkForExists)
        {
            if (equalityExp.Rule != 3)
            {
                Expect("Expected symbol reference, not [in]equality check.", equalityExp.Position);
                return false;
            }
            return ProcessDefinedCondition(equalityExp.PreCPrimary, checkForExists);
        }

        private bool ProcessDefinedCondition(IPreprocessorCPrimary primary, bool checkForExists)
        {
            if (primary.Rule == 3)
                return ProcessDefinedCondition(primary.PreCLogicalOrExp, checkForExists);
            else if (primary.Rule == 4)
                if (checkForExists)
                    return this.definedSymbols.ContainsKey(primary.Identifier.Name);
                else
                    return !this.definedSymbols.ContainsKey(primary.Identifier.Name);
            else
                Expect("identifier", primary.Position);
            return false;
        }

        private char LookPastAndSkip(int position = 0)
        {
            if (Lexer.IsWhitespaceChar(TokenizerLookAhead(position)))
                this.SkipWhitespace();
            return TokenizerLookAhead(position);
        }

        private void ParseError(string name, long position)
        {
            ParseError(name, position, null);
        }
        private void ParseError(string name, long position, PreprocessorIfDirective.DirectiveBody target)
        {
            var its = GetAhead(4);
            if (SeriesMatch(its, OilexerGrammarTokenType.StringLiteral, OilexerGrammarTokenType.Operator, OilexerGrammarTokenType.NumberLiteral, OilexerGrammarTokenType.Operator))
            {
                if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.Comma, true) &&
                    ExpectOperator(its[3], OilexerGrammarTokens.OperatorType.SemiColon, true))
                {

                    IOilexerGrammarErrorEntry iee = new OilexerGrammarErrorEntry(name, ((OilexerGrammarTokens.StringLiteralToken)(its[0])).GetCleanValue(), ((OilexerGrammarTokens.NumberLiteral)(its[2])).GetCleanValue(), this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetColumnIndex(position), this.CurrentTokenizer.GetLineIndex(position), position);
                    if (target != null)
                        target.Add(new PreprocessorEntryContainer(iee, iee.Column, iee.Line, iee.Position));
                    else
                        this.currentTarget.Result.Add(iee);
                }
            }
            else if (its[0].TokenType == OilexerGrammarTokenType.StringLiteral)
            {
                var second = its[1];
                if (second.TokenType == OilexerGrammarTokenType.Operator && (((OilexerGrammarTokens.OperatorToken)(second)).Type == OilexerGrammarTokens.OperatorType.Comma))
                {
                    if (its[2].TokenType == OilexerGrammarTokenType.NumberLiteral)
                        ExpectOperator(its[3], OilexerGrammarTokens.OperatorType.SemiColon, true);
                    else
                        Expect("number", its[2].Position);
                }
                else
                    ExpectOperator(second, OilexerGrammarTokens.OperatorType.Comma, true);
            }
            else
                Expect("string", its[0].Position);
        }

        private void ParseInclude(IPreprocessorStringTerminalDirective str)
        {
            string include = str.Literal;

            if (!Path.IsPathRooted(include))
                //Files are always relative to the one the current tokenizer is reading 
                //from.
                include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(CurrentTokenizer.FileName), include));
#if WINDOWS
            /* *
             * Win32 only because it's not case-sensitive on filenames.
             * This ensures that the same file isn't included twice due to case
             * inconsistencies.
             * */
            include = include.GetFilenameProperCasing();
#endif

            if (File.Exists(include))
            {
                if (!(currentTarget.Result.Includes.Contains(include)))
                    currentTarget.Result.Includes.Add(include);
            }
            else
                LogError(OilexerGrammarParserErrors.IncludeFileNotFound, include, str.Position);
        }

        private void ParseIncludes(OilexerGrammarParserResults currentTarget)
        {
            var currentIncludes = (from include in currentTarget.Result.Includes
                                   where !parsedIncludeDirectives.Contains(include)
                                   select include).ToArray();
            for (int i = 0; i < currentIncludes.Length; i++)
            {
                string s = currentIncludes[i];
                if (!parsedIncludeDirectives.Contains(s) && File.Exists(s))
                {
                    parsedIncludeDirectives.Add(s);
                    IParserResults<IOilexerGrammarFile> includedFile = this.Parse(s);
                    currentTarget.SetResult(currentTarget.Result + (OilexerGrammarFile)includedFile.Result);
                    if (!includedFile.Successful)
                        foreach (var ce in includedFile.SyntaxErrors.Where(k => k is IParserSyntaxError).Cast<IParserSyntaxError>())
                            currentTarget.SyntaxErrors.SyntaxError(ce);
                }
            }
        }

        private IList<IProductionRuleTemplatePart> ParseProductionRuleTemplateParts(OilexerGrammarTokens.OperatorToken currentOperator)
        {
            var currentToken = currentOperator;
            if (!(currentOperator != null &&
                currentOperator.Type == OilexerGrammarTokens.OperatorType.LessThan))
            {
                Expect("<");
                return null;
            }
            List<IProductionRuleTemplatePart> result = new List<IProductionRuleTemplatePart>();
            while (true)
            {
                bool currentIsSeries = false;
                string expectTarget = null;
                TemplatePartExpectedSpecial? special = null;
                ProductionRuleTemplatePart currentPart = null;
                var its = GetAhead(2);
                if (its.Count < 2)
                {
                    Expect("+, (, or >");
                    return null;
                }
                if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
                {
                    OilexerGrammarTokens.OperatorToken ot = its[1] as OilexerGrammarTokens.OperatorToken;
                    OilexerGrammarTokens.IdentifierToken id = its[0] as OilexerGrammarTokens.IdentifierToken;
                    switch (ot.Type)
                    {
                        case OilexerGrammarTokens.OperatorType.OptionsSeparator:
                            its = GetAhead(5);
                            if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator, OilexerGrammarTokenType.Operator))
                                if (ExpectIdentifier(its[0], "expect", StringComparison.InvariantCultureIgnoreCase, true))
                                {
                                    special = TemplatePartExpectedSpecial.None;
                                    if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.Equals, true))
                                        if (ExpectIdentifier(its[2], "token", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.Token;
                                        else if (ExpectIdentifier(its[2], "tokenorrule", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.TokenOrRule;
                                        else if (ExpectIdentifier(its[2], "rule", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.Rule;
                                        else
                                        {
                                            special = null;
                                            expectTarget = ((OilexerGrammarTokens.IdentifierToken)(its[2])).Name;
                                        }
                                    if (ExpectOperator(its[3], OilexerGrammarTokens.OperatorType.SemiColon, true))
                                    {
                                        ot = its[4] as OilexerGrammarTokens.OperatorToken;
                                        if (ot.Type == OilexerGrammarTokens.OperatorType.Comma)
                                            goto case OilexerGrammarTokens.OperatorType.Comma;
                                        else if (ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                            goto case OilexerGrammarTokens.OperatorType.GreaterThan;
                                        else if (ot.Type == OilexerGrammarTokens.OperatorType.Plus)
                                            goto case OilexerGrammarTokens.OperatorType.Plus;
                                        else
                                            Expect("'+', ',' or '>'", ot.Position);
                                    }
                                }
                                else
                                    return null;
                            else
                                return null;
                            break;
                        case OilexerGrammarTokens.OperatorType.Comma:
                            if (special == null)
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, expectTarget, id.Line, id.Column, id.Position);
                            else
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, special.Value, id.Line, id.Column, id.Position);
                            this.DefineRuleTemplateParameterIdentifier(id, currentPart);
                            result.Add(currentPart);
                            break;
                        case OilexerGrammarTokens.OperatorType.Plus:
                            currentIsSeries = true;
                            IOilexerGrammarToken gdt = PopAhead();
                            if (ExpectOperator(gdt, OilexerGrammarTokens.OperatorType.Comma, false))
                                goto case OilexerGrammarTokens.OperatorType.Comma;
                            else if (ExpectOperator(gdt, OilexerGrammarTokens.OperatorType.GreaterThan, false))
                                goto case OilexerGrammarTokens.OperatorType.GreaterThan;
                            else
                                Expect("',' or '>'");
                            break;
                        case OilexerGrammarTokens.OperatorType.GreaterThan:
                            if (special == null)
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, expectTarget, id.Line, id.Column, id.Position);
                            else
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, special.Value, id.Line, id.Column, id.Position);
                            this.DefineRuleTemplateParameterIdentifier(id, currentPart);
                            result.Add(currentPart);
                            goto _ter;
                        default:
                            Expect("'+', ':', ',' or '>'");
                            return null;
                    }
                }
                else if ((its[0]).TokenType == OilexerGrammarTokenType.Identifier)
                {
                    Expect("'+', '(', ',' or '>'");
                    return null;
                }
                else
                    Expect("identifier");
            }
        _ter:
            return result;
        }

        private bool ExpectIdentifier(IOilexerGrammarToken cur, string idValue, StringComparison comparisonType, bool error)
        {
            if (cur.TokenType == OilexerGrammarTokenType.Identifier && (string.Compare(((OilexerGrammarTokens.IdentifierToken)(cur)).Name, idValue, comparisonType) == 0))
                return true;
            else if (error)
                Expect(idValue);
            return false;
        }

        private bool ExpectOperator(IOilexerGrammarToken gdt, OilexerGrammarTokens.OperatorType operatorType, bool error)
        {
            if (gdt != null && gdt.TokenType == OilexerGrammarTokenType.Operator && (((OilexerGrammarTokens.OperatorToken)(gdt)).Type & operatorType) != OilexerGrammarTokens.OperatorType.None)
                return true;
            else if (error)
                if (gdt == null)
                    Expect(operatorType.ToString(), CurrentTokenizer.Position);
                else
                    Expect(operatorType.ToString(), gdt.Position);
            return false;
        }

        private void ParseProductionRuleTemplate(OilexerGrammarTokens.IdentifierToken identifier, IList<IProductionRuleTemplatePart> parts, EntryScanMode scanMode, long position)
        {
            ParseProductionRuleTemplate(identifier, parts, scanMode, position, null);
        }

        private void ParseProductionRuleTemplate(OilexerGrammarTokens.IdentifierToken identifier, IList<IProductionRuleTemplatePart> parts, EntryScanMode scanMode, long position, PreprocessorIfDirective.DirectiveBody target)
        {
            long bodyStart = this.StreamPosition;
            IOilexerGrammarProductionRuleTemplateEntry iprte = new OilexerGrammarProductionRuleTemplateEntry(identifier.Name, scanMode, parts, CurrentTokenizer.FileName, CurrentTokenizer.GetColumnIndex(position), CurrentTokenizer.GetLineIndex(position), position);
            ParseProductionRule(((OilexerGrammarProductionRuleTemplateEntry)iprte).BaseCollection, PreprocessorContainer.Template);
            long bodyEnd = this.StreamPosition;
            this.DefineRuleTemplateIdentifier(identifier, iprte);
            if (captureRegions)
            {
                int startLine = this.CurrentTokenizer.GetLineIndex(bodyStart);
                int endLine = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (endLine > startLine + 1)
                {
                    var gdResult = this.currentTarget.Result as OilexerGrammarFile;
                    gdResult.AddRuleRegion(iprte, bodyStart, bodyEnd);
                }
            }
            if (target == null)
                this.currentTarget.Result.Add(iprte);
            else
                target.Add(new PreprocessorEntryContainer(iprte, iprte.Column, iprte.Line, iprte.Position));
        }

        private void ParseProductionRule(ICollection<IProductionRule> iprte, PreprocessorContainer container)
        {
            while (true)
            {
                IOilexerGrammarToken la;
                ClearAhead();
                char c = LookPastAndSkip();
                if (c == char.MinValue)
                    break;
                else if (c == ';')
                {
                    ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.SemiColon, true);
                    break;
                }
                else if (c == '|')
                {
                    if (!(ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.Pipe, true)))
                        break;
                    ParseProductionRuleBody(iprte, container);
                    continue;
                }
                else if (c == ')' && parameterDepth > 0)
                    break;
                else if (c == '>' && templateDepth > 0 ||
                         c == ',' && templateDepth > 0)
                    break;
                else if (Lexer.IsIdentifierChar(c) || c == '\'' || c == '(' || c == '"' || c == '#' || c == '@')
                {
                    if (c == '#')
                    {
                        var preprocessorAhead = (OilexerGrammarTokens.PreprocessorDirective)LookAhead(0);
                        if (preprocessorAhead.Type == OilexerGrammarTokens.PreprocessorType.EndIfDirective ||
                            preprocessorAhead.Type == OilexerGrammarTokens.PreprocessorType.ElseIfDirective ||
                            preprocessorAhead.Type == OilexerGrammarTokens.PreprocessorType.ElseIfInDirective ||
                            preprocessorAhead.Type == OilexerGrammarTokens.PreprocessorType.ElseDirective ||
                            preprocessorAhead.Type == OilexerGrammarTokens.PreprocessorType.ElseIfDefinedDirective)
                            break;
                    }
                    ParseProductionRuleBody(iprte, container);
                }
                else if ((la = LookAhead(0)) != null && la.TokenType == OilexerGrammarTokenType.Comment)
                {
                    ParseProductionRuleBody(iprte, container);
                }
                else if (Lexer.IsWhitespaceChar(c))
                    //Skip
                    SkipWhitespace();
                else
                {
                    if (parameterDepth > 0)
                        Expect("string, char, identifier, preprocessor, or ')'");
                    else if (templateDepth > 0)
                        Expect("string, char, identifier, preprocessor, or '>'");
                    else
                        Expect("string, char, identifier, preprocessor, or ';'");
                    PopAhead();
                    break;
                }
            }
        }

        private void ParseProductionRuleBody(ICollection<IProductionRule> series, PreprocessorContainer container)
        {
            int l = this.CurrentTokenizer.GetLineIndex(this.StreamPosition);
            int ci = this.CurrentTokenizer.GetColumnIndex(this.StreamPosition);
            long p = this.StreamPosition;
            IList<IProductionRuleItem> seriesItemMembers = new List<IProductionRuleItem>();
            while (true)
            {
                IProductionRuleItem item = null;
                IOilexerGrammarToken igdt = LookAhead(0);
                if (igdt == null && CurrentTokenizer.CurrentError != null)
                {
                    var ce = CurrentTokenizer.CurrentError;
                    this.currentTarget.SyntaxErrors.SyntaxError(ce);
                    return;
                }
                else if (igdt == null)
                    goto yield;
                switch (igdt.TokenType)
                {
                    case OilexerGrammarTokenType.Comment:
                        var commentToken = igdt as OilexerGrammarTokens.CommentToken;
                        if (captureRegions && commentToken.MultiLine)
                        {
                            var gdResult = currentTarget.Result as OilexerGrammarFile;
                            gdResult.AddCommentRegion(commentToken);
                        }
                        PopAhead();
                        continue;
                    case OilexerGrammarTokenType.CharacterLiteral:
                        item = ParseProductionRuleCharLiteral();
                        break;
                    case OilexerGrammarTokenType.StringLiteral:
                        item = ParseProductionRuleStringLiteral();
                        break;
                    case OilexerGrammarTokenType.Identifier:
                        item = ParseProductionRuleReferenceItem(container);
                        break;
                    case OilexerGrammarTokenType.CharacterRange:
                        LogError(OilexerGrammarParserErrors.Unexpected, "character range", igdt.Position);
                        PopAhead();
                        break;
                    //case OilexerGrammarTokenType.CharacterRangeCommand:
                    //    LogError(OilexerGrammarParserErrors.Unexpected, "CharRange(*", igdt.Position);
                    //    break;
                    case OilexerGrammarTokenType.NumberLiteral:
                        LogError(OilexerGrammarParserErrors.Unexpected, "number", igdt.Position);
                        PopAhead();
                        break;
                    case OilexerGrammarTokenType.PreprocessorDirective:
                        OilexerGrammarTokens.PreprocessorDirective ppDirective = ((OilexerGrammarTokens.PreprocessorDirective)(igdt));
                        if (ppDirective.Type == OilexerGrammarTokens.PreprocessorType.EndIfDirective ||
                            ppDirective.Type == OilexerGrammarTokens.PreprocessorType.ElseIfDirective ||
                            ppDirective.Type == OilexerGrammarTokens.PreprocessorType.ElseIfInDirective ||
                            ppDirective.Type == OilexerGrammarTokens.PreprocessorType.ElseDirective ||
                            ppDirective.Type == OilexerGrammarTokens.PreprocessorType.ElseIfDefinedDirective)
                            goto yield;
                        if (container == PreprocessorContainer.Template)
                        {
                            IProductionRulePreprocessorDirective iprpd = ParseProductionRulePreprocessor(container == PreprocessorContainer.Template ? container : PreprocessorContainer.Rule);
                            if (container == PreprocessorContainer.Template)
                                if (iprpd != null)
                                    seriesItemMembers.Add(iprpd);
                                else
                                    return;
                        }
                        else
                        {
                            switch (ppDirective.Type)
                            {
                                case OilexerGrammarTokens.PreprocessorType.IfDirective:
                                case OilexerGrammarTokens.PreprocessorType.IfDefinedDirective:
                                case OilexerGrammarTokens.PreprocessorType.IfNotDefinedDirective:
                                case OilexerGrammarTokens.PreprocessorType.IfInDirective:
                                    PopAhead();
                                    IPreprocessorCLogicalOrConditionExp condition = null;
                                    if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                                    {
                                        var ifEntry = ParseIfDirective(condition, ppDirective, PreprocessorContainer.Rule);
                                        List<IProductionRule> set = new List<IProductionRule>();
                                        //bool firstSeries = true, yieldFast = false;
                                        if (ifEntry.Type != EntryPreprocessorType.IfIn)
                                        {
                                            OilexerGrammarProcessIfEntry(ifEntry, PreprocessorContainer.Rule, (IProductionRule current) =>
                                                {
                                                    set.Add(current);
                                                });
                                        }

                                        if (set.Count > 1)
                                            series.Add(new ProductionRule(new List<IProductionRuleItem>(new[] { new ProductionRuleGroupItem(set.ToArray(), set[0].Column, set[0].Line, set[0].Position) }), CurrentTokenizer.FileName, set[0].Column, set[0].Line, set[0].Position));
                                        else if (set.Count == 1)
                                            series.Add(set[0]);
                                        //if (yieldFast)
                                        //    goto yield;
                                    }
                                    break;
                                case OilexerGrammarTokens.PreprocessorType.DefineDirective:
                                    LogError(OilexerGrammarParserErrors.Unexpected, "Define directive inside production rule.", ppDirective.Position);
                                    PopAhead();
                                    break;
                                case OilexerGrammarTokens.PreprocessorType.AddRuleDirective:
                                    LogError(OilexerGrammarParserErrors.Unexpected, "Add rule directive inside production rule.", ppDirective.Position);
                                    PopAhead();
                                    break;
                                default:
                                    Expect("#if, #ifdef, #ifndef,identifier, '(' or ';'", ppDirective.Position);
                                    PopAhead();
                                    break;
                            }
                        }
                        break;
                    case OilexerGrammarTokenType.Operator:
                        switch (((OilexerGrammarTokens.OperatorToken)(igdt)).Type)
                        {
                            case OilexerGrammarTokens.OperatorType.Pipe:
                                ClearAhead();
                                goto yield;
                            case OilexerGrammarTokens.OperatorType.LeftParenthesis:
                                ClearAhead();
                                parameterDepth++;
                                item = (IProductionRuleGroupItem)ParseProductionRuleGroup(container);
                                parameterDepth--;
                                break;
                            case OilexerGrammarTokens.OperatorType.RightParenthesis:
                                goto yield;
                            case OilexerGrammarTokens.OperatorType.GreaterThan:
                            case OilexerGrammarTokens.OperatorType.Comma:
                                if (templateDepth <= 0)
                                {
                                    LogError(OilexerGrammarParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(', or ';'");
                                    PopAhead();
                                }
                                goto yield;
                            case OilexerGrammarTokens.OperatorType.SemiColon:
                                ClearAhead();
                                if (templateDepth != 0 || parameterDepth != 0)
                                {
                                    Expect((templateDepth > 0 ? ">" : string.Empty) + (parameterDepth > 0 ? ")" : string.Empty), this.StreamPosition);
                                    PopAhead();
                                }
                                goto yield;
                            default:
                                goto outerDefault;
                        }
                        break;
                    default:
                    outerDefault: { }
                        {
                            if (parameterDepth > 0)
                                LogError(OilexerGrammarParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(', ')'");
                            else if (templateDepth > 0)
                            {
                                LogError(OilexerGrammarParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(', ',' or '>'");
                            }
                            else
                                LogError(OilexerGrammarParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(' or ';'");
                            PopAhead();
                            break;
                        }
                }
                if (item != null)
                {
                    ClearAhead();
                    if (TokenizerLookAhead(0) == ':')
                    {
                        ParseItemOptions(item);
                    }
                    ParseItemRepeatOptions(item);
                    seriesItemMembers.Add(item);
                }
            }

        yield:
            if (seriesItemMembers.Count > 0)
                series.Add(new ProductionRule(seriesItemMembers, CurrentTokenizer.FileName, l, ci, p));
        }

        private IProductionRuleGroupItem ParseProductionRuleGroup(PreprocessorContainer container)
        {

            OilexerGrammarTokens.OperatorToken ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken;
            if (ot != null && ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.LeftParenthesis, true))
            {
                List<IProductionRule> items = new List<IProductionRule>();
                ParseProductionRule(items, container);
                IProductionRuleGroupItem result = new ProductionRuleGroupItem(items.ToArray(), ot.Column, ot.Line, ot.Position);
                var endTok = this.PopAhead();
                if (ExpectOperator(endTok, OilexerGrammarTokens.OperatorType.RightParenthesis, true))
                {
                    long groupEnd = endTok.Position;
                    if (captureRegions)
                    {
                        var gdResult = this.currentTarget.Result as OilexerGrammarFile;
                        int lineStart = ot.Line;
                        int lineEnd = endTok.Line;
                        if (lineEnd > lineStart + 1)
                            gdResult.AddRuleGroupRegion(result, ot.Position, groupEnd);
                    }
                }
                return result;
            }
            return null;
        }

        private void ParseItemOptions(IScannableEntryItem item)
        {
            OilexerGrammarTokens.OperatorToken ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken;
            if (ot != null && ExpectOperator(ot, OilexerGrammarTokens.OperatorType.OptionsSeparator, true))
            {
                PopAhead();
                var its = GetAhead(2);
                if (its.Count != 2)
                {
                    Expect("Name and ';'");
                    return;
                }
                if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
                {
                    OilexerGrammarTokens.IdentifierToken idt = its[0] as OilexerGrammarTokens.IdentifierToken;
                    OilexerGrammarTokens.OperatorToken op = its[1] as OilexerGrammarTokens.OperatorToken;
                    if (ExpectOperator(op, OilexerGrammarTokens.OperatorType.SemiColon, true))
                        item.Name = idt.Name;
                }
                ParseFlagOption(item);
                ParseDefaultOption(item);
            }
        }

        private void ParseDefaultOption(IScannableEntryItem item)
        {
            if (item is ITokenItem)
            {
                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Identifier &&
                    LookAhead(1).TokenType == OilexerGrammarTokenType.Operator &&
                    LookAhead(2).TokenType == OilexerGrammarTokenType.Identifier &&
                    LookAhead(3).TokenType == OilexerGrammarTokenType.Operator)
                {
                    if (ExpectIdentifier(LookAhead(0), "default", StringComparison.InvariantCultureIgnoreCase, false) &&
                        ExpectOperator(LookAhead(1), OilexerGrammarTokens.OperatorType.Equals, false) &&
                        //At this point, we can expect beyond a doubt, so error if there is no
                        //';' operator.
                        ExpectOperator(LookAhead(3), OilexerGrammarTokens.OperatorType.SemiColon, true))
                    {
                        string v = ((OilexerGrammarTokens.IdentifierToken)LookAhead(2)).Name;
                        if (item is TokenGroupItem)
                            ((TokenGroupItem)item).DefaultSoftRefOrValue = v;
                        else if (item is TokenItem)
                            ((TokenItem)(item)).DefaultSoftRefOrValue = v;
                        GetAhead(4);
                    }
                }
            }
        }

        private AheadStream<T1, T2> GetAhead<T1, T2>(int count)
            where T1 : class, IToken
            where T2 : class, IToken
        {
            var ahead1 = this.LookAhead(0);
            var ahead2 = this.LookAhead(1);
            if (!(ahead1 is T1))
                return new AheadStream<T1, T2>(null, null);
            else if (!(ahead1 is T1 && ahead2 is T2))
                return new AheadStream<T1, T2>((T1)ahead1, null);
            this.ClearAhead();
            this.StreamPosition = ahead2.Position + ahead2.Length;
            return new AheadStream<T1, T2>((T1)ahead1, (T2)ahead2);
        }

        private AheadStream<T1, T2, T3> GetAhead<T1, T2, T3>(int count)
            where T1 : class, IToken
            where T2 : class, IToken
            where T3 : class, IToken
        {
            var ahead1 = this.LookAhead(0);
            var ahead2 = this.LookAhead(1);
            var ahead3 = this.LookAhead(2);
            if (!(ahead1 is T1))
                return new AheadStream<T1, T2, T3>(null, null, null);
            else if (!(ahead2 is T2))
                return new AheadStream<T1, T2, T3>((T1)ahead1, null, null);
            else if (!(ahead3 is T3))
                return new AheadStream<T1, T2, T3>((T1)ahead1, (T2)ahead2, null);
            this.ClearAhead();
            this.StreamPosition = ahead3.Position + ahead3.Length;
            return new AheadStream<T1, T2, T3>((T1)ahead1, (T2)ahead2, (T3)ahead3);
        }

        private AheadStream<T1, T2, T3, T4> GetAhead<T1, T2, T3, T4>(int count)
            where T1 : class, IToken
            where T2 : class, IToken
            where T3 : class, IToken
            where T4 : class, IToken
        {
            var ahead1 = this.LookAhead(0);
            var ahead2 = this.LookAhead(1);
            var ahead3 = this.LookAhead(2);
            var ahead4 = this.LookAhead(3);
            if (!(ahead1 is T1))
                return new AheadStream<T1, T2, T3, T4>(null, null, null, null);
            else if (!(ahead2 is T2))
                return new AheadStream<T1, T2, T3, T4>((T1)ahead1, null, null, null);
            else if (!(ahead3 is T3))
                return new AheadStream<T1, T2, T3, T4>((T1)ahead1, (T2)ahead2, null, null);
            else if (!(ahead4 is T4))
                return new AheadStream<T1, T2, T3, T4>((T1)ahead1, (T2)ahead2, (T3)ahead3, null);
            this.ClearAhead();
            this.StreamPosition = ahead4.Position + ahead4.Length;
            return new AheadStream<T1, T2, T3, T4>((T1)ahead1, (T2)ahead2, (T3)ahead3, (T4)ahead4);
        }

        private AheadStream<T1, T2, T3, T4, T5> GetAhead<T1, T2, T3, T4, T5>(int count)
            where T1 : class, IToken
            where T2 : class, IToken
            where T3 : class, IToken
            where T4 : class, IToken
            where T5 : class, IToken
        {
            var ahead1 = this.LookAhead(0);
            var ahead2 = this.LookAhead(1);
            var ahead3 = this.LookAhead(2);
            var ahead4 = this.LookAhead(3);
            var ahead5 = this.LookAhead(4);
            if (!(ahead1 is T1))
                return new AheadStream<T1, T2, T3, T4, T5>(null, null, null, null, null);
            else if (!(ahead2 is T2))
                return new AheadStream<T1, T2, T3, T4, T5>((T1)ahead1, null, null, null, null);
            else if (!(ahead3 is T3))
                return new AheadStream<T1, T2, T3, T4, T5>((T1)ahead1, (T2)ahead2, null, null, null);
            else if (!(ahead4 is T4))
                return new AheadStream<T1, T2, T3, T4, T5>((T1)ahead1, (T2)ahead2, (T3)ahead3, null, null);
            else if (!(ahead5 is T5))
                return new AheadStream<T1, T2, T3, T4, T5>((T1)ahead1, (T2)ahead2, (T3)ahead3, (T4)ahead4, null);
            this.ClearAhead();
            this.StreamPosition = ahead5.Position + ahead5.Length;
            return new AheadStream<T1, T2, T3, T4, T5>((T1)ahead1, (T2)ahead2, (T3)ahead3, (T4)ahead4, (T5)ahead5);
        }

        private void ParseFlagOption(IScannableEntryItem item)
        {

            bool isCharItem;
            if ((isCharItem = (item is LiteralCharTokenItem)) ||
                item is LiteralStringTokenItem)
            {
                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Identifier &&
                    LookAhead(1).TokenType == OilexerGrammarTokenType.Operator &&
                    LookAhead(2).TokenType == OilexerGrammarTokenType.Identifier &&
                    LookAhead(3).TokenType == OilexerGrammarTokenType.Operator)
                {
                    if (ExpectIdentifier(LookAhead(0), "flag", StringComparison.InvariantCultureIgnoreCase, false) &&
                        ExpectOperator(LookAhead(1), OilexerGrammarTokens.OperatorType.Equals, false) &&
                        //At this point, we can expect beyond a doubt, so error if there is no
                        //boolean or ';' operator.
                        ExpectBoolean(LookAhead(2), true) &&
                        ExpectOperator(LookAhead(3), OilexerGrammarTokens.OperatorType.SemiColon, true))
                    {
                        var aheadFour = GetAhead<OilexerGrammarTokens.IdentifierToken, OilexerGrammarTokens.OperatorToken, OilexerGrammarTokens.IdentifierToken, OilexerGrammarTokens.OperatorToken>(4);
                        if (isCharItem)
                        {
                            var charItem = item as LiteralCharTokenItem;
                            charItem.IsFlag = bool.Parse(aheadFour.Token3.Name);
                            charItem.IsFlagToken = aheadFour.Token3;
                        }
                        else//LiteralStringTokenItem
                        {
                            var stringItem = item as LiteralStringTokenItem;
                            stringItem.IsFlag = bool.Parse(aheadFour.Token3.Name);
                            stringItem.IsFlagToken = aheadFour.Token3;
                        }
                        DefineKeywordIdentifier(aheadFour.Token1);
                        DefineKeywordIdentifier(aheadFour.Token3);
                    }
                }
            }
        }

        private IProductionRulePreprocessorDirective ParseProductionRulePreprocessor(PreprocessorContainer container)
        {
            OilexerGrammarTokens.PreprocessorDirective ppd = LookAhead(0) as OilexerGrammarTokens.PreprocessorDirective;
            if (ppd != null)
            {
                IPreprocessorDirective ippd = ParsePreprocessor(container);
                if (ippd != null)
                    return new ProductionRulePreprocessorDirective(ippd, ppd.Column, ppd.Line, ppd.Position);
            }
            Expect("Preprocessor Directive");
            PopAhead();
            return null;
        }

        private IPreprocessorDirective ParsePreprocessor(PreprocessorContainer container)
        {
            IPreprocessorDirective result = null;
            bool ml = ((Lexer)this.CurrentTokenizer).MultiLineMode;
            this.SetMultiLineMode(false);
            OilexerGrammarTokens.PreprocessorDirective ppd = LookAhead(0) as OilexerGrammarTokens.PreprocessorDirective;
            if (ppd != null)
            {
                switch (ppd.Type)
                {
                    case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                    case OilexerGrammarTokens.PreprocessorType.RootDirective:
                    case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                        if (container == PreprocessorContainer.File)
                            result = ParseStringTerminalDirective();
                        else
                        {
                            string ppdS = ppd.Type.ToString();
                            ppdS = ppdS.Substring(0, ppdS.Length - 9);
                            LogError(OilexerGrammarParserErrors.Unexpected, string.Format("{0} directive.", ppdS), ppd.Position);
                        }
                        break;
                    case OilexerGrammarTokens.PreprocessorType.IfDefinedDirective:
                    case OilexerGrammarTokens.PreprocessorType.IfNotDefinedDirective:
                    case OilexerGrammarTokens.PreprocessorType.IfDirective:
                        PopAhead();
                        IPreprocessorCLogicalOrConditionExp condition = null;
                        if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                        {
                            result = ParseIfDirective(condition, ppd, container);
                        }
                        break;
                    case OilexerGrammarTokens.PreprocessorType.AddRuleDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseAddRuleDirective();
                        break;
                    case OilexerGrammarTokens.PreprocessorType.DefineDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseDefineRuleDirective();
                        else if (container == PreprocessorContainer.File)
                            result = ParseDefineSymbolDirective(ppd);
                        else
                            LogError(OilexerGrammarParserErrors.Unexpected, "Define directive.", ppd.Position);
                        break;
                    case OilexerGrammarTokens.PreprocessorType.ReturnDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseReturnDirective();
                        else
                            LogError(OilexerGrammarParserErrors.Unexpected, "Return directive.", ppd.Position);
                        break;
                    case OilexerGrammarTokens.PreprocessorType.ThrowDirective:
                        result = ParseThrowDirective();
                        break;
                    default:
                        break;
                }
            }
            else if (container == PreprocessorContainer.Rule)
            {
                ICollection<IProductionRule> containedSet = new List<IProductionRule>();
                int line = this.CurrentTokenizer.GetLineIndex(this.StreamPosition), column = this.CurrentTokenizer.GetColumnIndex(this.StreamPosition);
                long position = this.StreamPosition;
                bool multiLine = ((Lexer)(this.CurrentTokenizer)).MultiLineMode;
                this.SetMultiLineMode(true);
                this.ParseProductionRule(containedSet, container);
                this.SetMultiLineMode(multiLine);
                return new PreprocessorProductionRuleSeries(containedSet, column, line, position);
            }
            return result;
        }

        private IPreprocessorDirective ParseDefineSymbolDirective(OilexerGrammarTokens.PreprocessorDirective preprocessor)
        {
            var stream = GetAhead(3);
            if (SeriesMatch(stream, OilexerGrammarTokenType.PreprocessorDirective, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
            {
                OilexerGrammarTokens.PreprocessorDirective def = (OilexerGrammarTokens.PreprocessorDirective)stream[0];
                OilexerGrammarTokens.IdentifierToken id = (OilexerGrammarTokens.IdentifierToken)stream[1];
                OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)stream[2];
                string name = id.Name;
                string value = null;
                if (op.Type == OilexerGrammarTokens.OperatorType.Equals)
                {
                    var tok = LookAhead(0);
                    if (tok.TokenType == OilexerGrammarTokenType.Identifier)
                        value = ((OilexerGrammarTokens.IdentifierToken)(tok)).Name;
                    else if (tok.TokenType == OilexerGrammarTokenType.StringLiteral)
                        value = ((OilexerGrammarTokens.StringLiteralToken)(tok)).GetCleanValue();
                    else
                    {
                        Expect("string or identifier", tok.Position);
                        PopAhead();
                        return null;
                    }
                    PopAhead();
                    tok = PopAhead();
                    if (!ExpectOperator(tok, OilexerGrammarTokens.OperatorType.SemiColon, true))
                        return null;

                }
                else if (!ExpectOperator(op, OilexerGrammarTokens.OperatorType.SemiColon, true))
                    return null;
                return new PreprocessorDefineSymbolDirective(name, value, preprocessor.Column, preprocessor.Line, preprocessor.Position);
            }
            else if (stream.Count == 3 && (stream[1]).TokenType == OilexerGrammarTokenType.Identifier)
            {
                PopAhead();
                ExpectOperator(stream[2], OilexerGrammarTokens.OperatorType.Comma | OilexerGrammarTokens.OperatorType.SemiColon, true);
            }
            else if (stream.Count > 1)
                Expect("identifier", stream[1].Position);
            else if (stream.Count == 1)
                Expect("identifier", stream[0].Position + stream[0].Length);

            return null;
        }

        private IPreprocessorStringTerminalDirective ParseStringTerminalDirective()
        {
            OilexerGrammarTokens.PreprocessorDirective pp = LookAhead(0) as OilexerGrammarTokens.PreprocessorDirective;

            if (pp == null)
            {
                Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                PopAhead();
                return null;
            }

            IOilexerGrammarToken one = LookAhead(1);
            if (one == null)
            {
                switch (pp.Type)
                {
                    case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                    case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.RootDirective:
                    case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                        break;
                    default:
                        Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                        this.PopAhead();
                        return null;
                }
                currentTarget.SyntaxErrors.SyntaxError(this.CurrentTokenizer.CurrentError);//OilexerGrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.CurrentError.Location.Line, this.CurrentTokenizer.CurrentError.Location.Column, OilexerGrammarParserErrors.Expected, "string"));
                this.PopAhead();
                return null;
            }
            else if (this.LookAhead(2) == null)
            {
                switch (pp.Type)
                {
                    case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                    case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.RootDirective:
                    case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                        if (one.TokenType != OilexerGrammarTokenType.StringLiteral)
                        {
                            Expect("string", one.Position);
                            this.GetAhead(2);
                            return null;
                        }
                        break;
                    default:
                        Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                        this.GetAhead(2);
                        return null;
                }
                currentTarget.SyntaxErrors.SyntaxError(this.CurrentTokenizer.CurrentError);//OilexerGrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.CurrentError.Location.Line, this.CurrentTokenizer.CurrentError.Location.Column, OilexerGrammarParserErrors.Expected, ";"));
                this.GetAhead(2);
                return null;
            }
            var its = GetAhead(3);

            if (its.Count == 3 && SeriesMatch(its, OilexerGrammarTokenType.PreprocessorDirective, OilexerGrammarTokenType.StringLiteral, OilexerGrammarTokenType.Operator))
            {
                StringTerminalKind kind = StringTerminalKind.Unknown;
                OilexerGrammarTokens.StringLiteralToken str = (OilexerGrammarTokens.StringLiteralToken)its[1];
                OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)its[2];
                if (op.Type == OilexerGrammarTokens.OperatorType.SemiColon)
                {
                    string literal = str.GetCleanValue();
                    switch (pp.Type)
                    {
                        case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                            kind = StringTerminalKind.Include;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.RootDirective:
                            kind = StringTerminalKind.Root;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                            kind = StringTerminalKind.AssemblyName;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                            kind = StringTerminalKind.LexerName;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                            kind = StringTerminalKind.GrammarName;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                            kind = StringTerminalKind.ParserName;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                            kind = StringTerminalKind.TokenPrefix;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                            kind = StringTerminalKind.TokenSuffix;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                            kind = StringTerminalKind.RulePrefix;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                            kind = StringTerminalKind.RuleSuffix;
                            break;
                        case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                            kind = StringTerminalKind.Namespace;
                            break;
                        default:
                            Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                            break;
                    }
                    return new PreprocessorStringTerminalDirective(kind, literal, pp, str, pp.Column, pp.Line, pp.Position);
                }
            }
            else
            {
                switch (pp.Type)
                {
                    case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                    case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                    case OilexerGrammarTokens.PreprocessorType.RootDirective:
                    case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                    case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                    case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                        if (its.Count > 1)
                        {
                            IOilexerGrammarToken second = its[1];
                            if (second.TokenType == OilexerGrammarTokenType.StringLiteral)
                                ExpectOperator(its[2], OilexerGrammarTokens.OperatorType.SemiColon, true);
                            else
                                Expect("string", second.Position);
                        }
                        break;
                    default:
                        Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                        if (its.Count > 1)
                            this.StreamPosition = its[1].Position;
                        break;
                }
            }
            return null;
        }

        private IPreprocessorConditionalReturnDirective ParseReturnDirective()
        {
            IPreprocessorConditionalReturnDirective result = null;
            bool mlm = ((Lexer)(CurrentTokenizer)).MultiLineMode;
            this.SetMultiLineMode(true);
            if (LookAhead(0).TokenType != OilexerGrammarTokenType.PreprocessorDirective &&
                ((OilexerGrammarTokens.PreprocessorDirective)(LookAhead(0))).Type == OilexerGrammarTokens.PreprocessorType.ReturnDirective)
            {
                Expect("#return");
                return null;
            }
            IOilexerGrammarToken igdt = PopAhead();
            List<IProductionRule> icipr = new List<IProductionRule>();
            ParseProductionRule(icipr, PreprocessorContainer.Template);
            result = new PreprocessorConditionalReturnDirective(icipr.ToArray(), igdt.Column, igdt.Line, igdt.Position);
            this.SetMultiLineMode(mlm);
            return result;
        }

        private IPreprocessorAddRuleDirective ParseAddRuleDirective()
        {
            IPreprocessorAddRuleDirective result = null;
            bool mlm = ((Lexer)(CurrentTokenizer)).MultiLineMode;
            this.SetMultiLineMode(false);
            if (LookAhead(0).TokenType != OilexerGrammarTokenType.PreprocessorDirective &&
                ((OilexerGrammarTokens.PreprocessorDirective)(LookAhead(0))).Type == OilexerGrammarTokens.PreprocessorType.AddRuleDirective)
            {
                Expect("#addrule");
                return null;
            }
            IOilexerGrammarToken igdt = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
            {
                OilexerGrammarTokens.IdentifierToken target = its[0] as OilexerGrammarTokens.IdentifierToken;
                OilexerGrammarTokens.OperatorToken op = its[1] as OilexerGrammarTokens.OperatorToken;
                if (!ExpectOperator(op, OilexerGrammarTokens.OperatorType.Comma, true))
                    return null;
                if (target == null)
                {
                    Expect("identifier", target.Position);
                    return null;
                }
                ICollection<IProductionRule> icipr = new System.Collections.ObjectModel.Collection<IProductionRule>();
                this.SetMultiLineMode(true);
                ParseProductionRule(icipr, PreprocessorContainer.Template);
                result = new PreprocessorAddRuleDirective(target.Name, new List<IProductionRule>(icipr).ToArray(), igdt.Column, igdt.Line, igdt.Position);
            }
            this.SetMultiLineMode(mlm);
            return result;
        }

        private IPreprocessorDefineRuleDirective ParseDefineRuleDirective()
        {
            IPreprocessorDefineRuleDirective result = null;
            bool mlm = ((Lexer)(CurrentTokenizer)).MultiLineMode;
            this.SetMultiLineMode(true);
            if (LookAhead(0).TokenType != OilexerGrammarTokenType.PreprocessorDirective &&
                ((OilexerGrammarTokens.PreprocessorDirective)(LookAhead(0))).Type == OilexerGrammarTokens.PreprocessorType.DefineDirective)
            {
                Expect("#define");
                return null;
            }
            IOilexerGrammarToken igdt = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
            {
                OilexerGrammarTokens.IdentifierToken target = its[0] as OilexerGrammarTokens.IdentifierToken;
                OilexerGrammarTokens.OperatorToken op = its[1] as OilexerGrammarTokens.OperatorToken;
                if (!ExpectOperator(op, OilexerGrammarTokens.OperatorType.Equals, true))
                    return null;
                if (target == null)
                {
                    Expect("identifier", target.Position);
                    return null;
                }
                ICollection<IProductionRule> icipr = new System.Collections.ObjectModel.Collection<IProductionRule>();
                this.SetMultiLineMode(true);
                ParseProductionRule(icipr, PreprocessorContainer.Template);
                result = new PreprocessorDefineRuleDirective(target.Name, new List<IProductionRule>(icipr).ToArray(), igdt.Column, igdt.Line, igdt.Position);
            }

            this.SetMultiLineMode(mlm);
            return result;
        }

        private IPreprocessorThrowDirective ParseThrowDirective()
        {
            IPreprocessorThrowDirective result = null;
            bool mlm = ((Lexer)(CurrentTokenizer)).MultiLineMode;
            this.SetMultiLineMode(false);
            if (LookAhead(0).TokenType != OilexerGrammarTokenType.PreprocessorDirective &&
                ((OilexerGrammarTokens.PreprocessorDirective)(LookAhead(0))).Type == OilexerGrammarTokens.PreprocessorType.ThrowDirective)
            {
                Expect("#throw");
                return null;
            }
            IOilexerGrammarToken preproc = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
            {
                OilexerGrammarTokens.IdentifierToken id = (OilexerGrammarTokens.IdentifierToken)its[0];
                if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.SemiColon, false))
                    result = new PreprocessorThrowDirective(this.currentTarget.Result, id.Name, new IOilexerGrammarToken[0], preproc.Column, preproc.Line, preproc.Position);
                else if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.Comma, true))
                {
                    List<IOilexerGrammarToken> args = new List<IOilexerGrammarToken>();
                    while (true)
                    {
                        its = GetAhead(2);
                        IOilexerGrammarToken arg = its[0];
                        if (its.Count != 2)
                            break;
                        if (arg.TokenType == OilexerGrammarTokenType.Identifier || arg.TokenType == OilexerGrammarTokenType.StringLiteral || arg.TokenType == OilexerGrammarTokenType.CharacterLiteral)
                            args.Add(arg);
                        else
                        {
                            Expect("string, char or identifier");
                            break;
                        }
                        if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.SemiColon, false))
                        {
                            result = new PreprocessorThrowDirective(this.currentTarget.Result, id.Name, args.ToArray(), preproc.Column, preproc.Line, preproc.Position);
                            break;
                        }
                        else if (ExpectOperator(its[1], OilexerGrammarTokens.OperatorType.Comma, true))
                            continue;
                        else
                            break;
                    }
                }
            }
            else if (its.Count > 0 && its[0].TokenType == OilexerGrammarTokenType.Identifier)
            {
                ExpectOperator(its[0], OilexerGrammarTokens.OperatorType.SemiColon, true);
            }
            else
                Expect("identifier");

            this.SetMultiLineMode(mlm);
            return result;
        }

        private IPreprocessorIfDirective ParseIfDirective(IPreprocessorCLogicalOrConditionExp condition, OilexerGrammarTokens.PreprocessorDirective preprocessor, PreprocessorContainer container)
        {
            IPreprocessorIfDirective result = null;
            bool secondHalf = true;
            switch (preprocessor.Type)
            {
                case OilexerGrammarTokens.PreprocessorType.IfDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.If, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case OilexerGrammarTokens.PreprocessorType.IfInDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.IfIn, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case OilexerGrammarTokens.PreprocessorType.IfDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.IfDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case OilexerGrammarTokens.PreprocessorType.IfNotDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.IfNotDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case OilexerGrammarTokens.PreprocessorType.ElseIfDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.ElseIf, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                case OilexerGrammarTokens.PreprocessorType.ElseIfInDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.ElseIfIn, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                case OilexerGrammarTokens.PreprocessorType.ElseIfDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.ElseIfDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                case OilexerGrammarTokens.PreprocessorType.ElseDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.Else, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                default:
                    break;
            }
            ClearAhead();
            IPreprocessorIfDirective current = result;
            if (current == null)
                return null;
            if (!secondHalf)
                return result;
            IPreprocessorIfDirective last = current;
            bool final = false;
            IOilexerGrammarToken igdt = null;
            this.SetMultiLineMode(true);
            while (
                (igdt = LookAhead(0)) != null && 
               (igdt.TokenType == OilexerGrammarTokenType.PreprocessorDirective ||
                igdt.TokenType == OilexerGrammarTokenType.Comment))
            {
                OilexerGrammarTokens.PreprocessorDirective ppd = igdt as OilexerGrammarTokens.PreprocessorDirective;
                IPreprocessorCLogicalOrConditionExp subCondition = null;
                if (ppd != null)
                {
                    PopAhead();
                    switch (ppd.Type)
                    {
                        case OilexerGrammarTokens.PreprocessorType.ElseIfInDirective:
                        case OilexerGrammarTokens.PreprocessorType.ElseIfDirective:
                        case OilexerGrammarTokens.PreprocessorType.ElseIfDefinedDirective:
                            if (final)
                            {
                                Expect("#endif");
                                goto endWhile;
                            }
                            this.SetMultiLineMode(false);
                            if (ParsePreprocessorCLogicalOrConditionExp(ref subCondition))
                            {
                                current = ParseIfDirective(subCondition, ppd, container);
                            }
                            else
                            {
                                Expect("condition");
                                goto endWhile;
                            }
                            break;
                        case OilexerGrammarTokens.PreprocessorType.ElseDirective:
                            final = true;
                            current = ParseIfDirective(null, ppd, container);
                            break;
                        case OilexerGrammarTokens.PreprocessorType.EndIfDirective:
                            //There's no break² operation.
                            goto endWhile;
                        default:
                            break;
                    }
                }
                else if (igdt.TokenType == OilexerGrammarTokenType.Comment)
                {
                    var commentToken = igdt as OilexerGrammarTokens.CommentToken;
                    if (captureRegions && commentToken.MultiLine)
                    {
                        var gdResult = currentTarget.Result as OilexerGrammarFile;
                        gdResult.AddCommentRegion(commentToken);
                    }
                    PopAhead();
                }
                if (last != current)
                {
                    ((PreprocessorIfDirective)(last)).Next = current;
                    ((PreprocessorIfDirective)(current)).Previous = last;
                }
                last = current;
                this.SetMultiLineMode(true);
            }
        endWhile:
            return result;
        }

        private void ParseIfDirectiveBody(IPreprocessorIfDirective ipid, PreprocessorContainer container)
        {
            var ppidb = ((PreprocessorIfDirective.DirectiveBody)ipid.Body);
            Action<IOilexerGrammarEntry> preprocessorInserter = p => ppidb.Add(new PreprocessorEntryContainer(p, p.Column, p.Line, p.Position));
            long bodyStart = ipid.Position;
            while (true)
            {
                this.SetMultiLineMode(true);
                LookPastAndSkip();

                var la0 = this.LookAhead(0);
                if (la0 == null)
                    break;
                OilexerGrammarTokens.PreprocessorDirective ppd = la0 as OilexerGrammarTokens.PreprocessorDirective;
                if (ppd != null)
                {
                    switch (ppd.Type)
                    {
                        case OilexerGrammarTokens.PreprocessorType.IfDirective:
                        case OilexerGrammarTokens.PreprocessorType.IfInDirective:
                        case OilexerGrammarTokens.PreprocessorType.IfDefinedDirective:
                        case OilexerGrammarTokens.PreprocessorType.IfNotDefinedDirective:
                        case OilexerGrammarTokens.PreprocessorType.DefineDirective:
                        case OilexerGrammarTokens.PreprocessorType.ThrowDirective:
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                                break;
                            }
                        case OilexerGrammarTokens.PreprocessorType.AddRuleDirective:
                        case OilexerGrammarTokens.PreprocessorType.ReturnDirective:
                            if (container == PreprocessorContainer.Template)
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                            }
                            else
                                LogError(OilexerGrammarParserErrors.Unexpected, string.Format("{0} directive.", ppd.Type == OilexerGrammarTokens.PreprocessorType.AddRuleDirective ? "Add rule" : "Return"), ppd.Position);
                            break;
                        case OilexerGrammarTokens.PreprocessorType.ElseIfDirective:
                        case OilexerGrammarTokens.PreprocessorType.ElseIfDefinedDirective:
                        case OilexerGrammarTokens.PreprocessorType.ElseDirective:
                        case OilexerGrammarTokens.PreprocessorType.ElseIfInDirective:
                        case OilexerGrammarTokens.PreprocessorType.EndIfDirective:
                            goto endBody;
                        case OilexerGrammarTokens.PreprocessorType.IncludeDirective:
                        case OilexerGrammarTokens.PreprocessorType.RootDirective:
                        case OilexerGrammarTokens.PreprocessorType.AssemblyNameDirective:
                        case OilexerGrammarTokens.PreprocessorType.LexerNameDirective:
                        case OilexerGrammarTokens.PreprocessorType.GrammarNameDirective:
                        case OilexerGrammarTokens.PreprocessorType.ParserNameDirective:
                        case OilexerGrammarTokens.PreprocessorType.TokenPrefixDirective:
                        case OilexerGrammarTokens.PreprocessorType.TokenSuffixDirective:
                        case OilexerGrammarTokens.PreprocessorType.RulePrefixDirective:
                        case OilexerGrammarTokens.PreprocessorType.RuleSuffixDirective:
                        case OilexerGrammarTokens.PreprocessorType.NamespaceDirective:
                            if (container == PreprocessorContainer.File)
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                            }
                            else
                                LogError(OilexerGrammarParserErrors.Unexpected, "StringTerminal directive.", ppd.Position);
                            break;
                        default:
                            break;
                    }
                }
                else if (container == PreprocessorContainer.File && la0.TokenType == OilexerGrammarTokenType.Identifier)
                {
                    /* *
                     * All declarations must begin at the firstSeries column 
                     * of the line.
                     * */
                    if (this.CurrentTokenizer.GetColumnIndex(this.StreamPosition) != 1)
                    {
                        LogError(OilexerGrammarParserErrors.ExpectedEndOfLine);
                        GetAhead(1);
                        continue;
                    }
                    EntryScanMode esm = EntryScanMode.Inherited;
                    var its = GetAhead(2);
                    /* *
                     * expected patterns:
                     * ID   = | * Language defined error 
                     * ID  := | * Token entry
                     * ID ::= | * Production entry
                     * ID   < | * Template begin
                     * ID   > | * Token with precedence information defined.
                     * ID   + | * Change line-mode to multi-line (not used)
                     * ID   -   * Change line-mode to single-line (not used)
                     * */
                    if (SeriesMatch(its, OilexerGrammarTokenType.Identifier, OilexerGrammarTokenType.Operator))
                    {
                        List<OilexerGrammarTokens.IdentifierToken> lowerPrecedences = new List<OilexerGrammarTokens.IdentifierToken>();
                        OilexerGrammarTokens.IdentifierToken id = ((OilexerGrammarTokens.IdentifierToken)(its[0]));
                        OilexerGrammarTokens.OperatorToken ot = ((OilexerGrammarTokens.OperatorToken)(its[1]));
                        bool elementsAreChildren = false;
                        bool forcedRecognizer = false;
                    checkOp:
                        switch (ot.Type)
                        {
                            //ID$
                            case OilexerGrammarTokens.OperatorType.ForcedStringForm:
                                forcedRecognizer = true;
                                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Operator)
                                {
                                    ot = (OilexerGrammarTokens.OperatorToken)PopAhead();
                                    goto checkOp;
                                }
                                else
                                {
                                    ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.ColonEquals | OilexerGrammarTokens.OperatorType.GreaterThan, true);
                                    PopAhead();
                                }
                                break;
                            //ID ::=
                            case OilexerGrammarTokens.OperatorType.ColonColonEquals:
                                if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                    ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                {
                                    elementsAreChildren = true;
                                    PopAhead();
                                }
                                SetMultiLineMode(true);
                                ParseProductionRule(id, EntryScanMode.Inherited, id.Position, elementsAreChildren, preprocessorInserter, container);
                                break;
                            //ID :=
                            case OilexerGrammarTokens.OperatorType.ColonEquals:
                                {
                                    SetMultiLineMode(true);
                                    bool unhinged = false, contextual = false;
                                    if (TokenizerLookAhead(0) == '*')
                                    {
                                        //ID :=*
                                        /* *
                                         * Token is unbounded by grammar
                                         * and can appear anywhere.
                                         * */
                                        this.PopAhead();
                                        unhinged = true;
                                        if (TokenizerLookAhead(0) == '-')
                                        {
                                            //ID :=*-
                                            /* *
                                             * It's not valid to have both unhinged and contextual.
                                             * */
                                            LogError(OilexerGrammarParserErrors.Unexpected, "-");
                                            this.PopAhead();
                                        }
                                    }
                                    if (TokenizerLookAhead(0) == '-')
                                    {
                                        //ID :=-
                                        /* *
                                         * Token is a contextual token and might be ambiguous with others
                                         * special care needs taken when handling them to avoid incorrect
                                         * parse paths.
                                         * */
                                        this.PopAhead();
                                        contextual = true;
                                        if (TokenizerLookAhead(0) == '*')
                                        {
                                            //ID :=-*
                                            /* *
                                             * It's not valid to have both contextual and unhinged.
                                             * */
                                            LogError(OilexerGrammarParserErrors.Unexpected, "*");
                                            this.PopAhead();
                                        }
                                    }
                                    ParseToken(id, EntryScanMode.Inherited, id.Position, unhinged, lowerPrecedences, forcedRecognizer, ppidb, contextual);
                                }
                                break;
                            //ID =
                            case OilexerGrammarTokens.OperatorType.Equals:
                                ParseError(id.Name, id.Position, ppidb);
                                break;
                            //ID<
                            case OilexerGrammarTokens.OperatorType.LessThan:
                                SetMultiLineMode(true);
                                IList<IProductionRuleTemplatePart> parts = this.ParseProductionRuleTemplateParts(ot);
                                if (parts == null)
                                    continue;
                                else
                                {
                                    IOilexerGrammarToken g = null;
                                    OilexerGrammarTokens.OperatorToken gdt = (g = PopAhead()) as OilexerGrammarTokens.OperatorToken;
                                    if (gdt == null)
                                        Expect("+, -, or ::=");
                                    if (gdt.Type == OilexerGrammarTokens.OperatorType.Minus)
                                    {
                                        gdt = PopAhead() as OilexerGrammarTokens.OperatorToken;
                                        esm = EntryScanMode.SingleLine;
                                    }
                                    else if (gdt.Type == OilexerGrammarTokens.OperatorType.Plus)
                                    {
                                        gdt = PopAhead() as OilexerGrammarTokens.OperatorToken;
                                        esm = EntryScanMode.Multiline;
                                    }
                                    if (gdt == null || !ExpectOperator(gdt, OilexerGrammarTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        Expect("::=");
                                        break;
                                    }
                                    this.ParseProductionRuleTemplate(id, parts, esm, id.Position, ppidb);
                                }
                                break;
                            //ID>
                            case OilexerGrammarTokens.OperatorType.GreaterThan:
                                //Case of a token defining precedence.
                                while (LookAhead(0).TokenType == OilexerGrammarTokenType.Identifier)
                                {
                                    var lowerPrecedenceToken = ((OilexerGrammarTokens.IdentifierToken)LookAhead(0));
                                    lowerPrecedences.Add(lowerPrecedenceToken);
                                    PopAhead();
                                    OilexerGrammarTokens.OperatorToken cOp = null;
                                    //','  * More precedences exist.
                                    if ((cOp = (LookAhead(0) as OilexerGrammarTokens.OperatorToken)) != null &&
                                        cOp.Type == OilexerGrammarTokens.OperatorType.Comma)
                                        PopAhead();
                                    //":=" * Final precedence reached.
                                    else if (cOp != null &&
                                        cOp.Type == OilexerGrammarTokens.OperatorType.ColonEquals)
                                    {
                                        PopAhead();
                                        goto case OilexerGrammarTokens.OperatorType.ColonEquals;
                                    }
                                    else
                                    {
                                        //Syntax error.
                                        ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.ColonEquals, true);
                                        break;
                                    }
                                }
                                break;
                            //ID{
                            case OilexerGrammarTokens.OperatorType.LeftCurlyBrace: /* *Must* be a production rule entry, with a k-limit. */
                                var curlyAhead = PopAhead();
                                if (curlyAhead.TokenType == OilexerGrammarTokenType.NumberLiteral)
                                {
                                    var curlyNumber = curlyAhead as OilexerGrammarTokens.NumberLiteral;
                                    IOilexerGrammarToken colonColonEq = null;
                                    if (ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.RightCurlyBrace, true) &&
                                        ExpectOperator(colonColonEq = PopAhead(), OilexerGrammarTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        SetMultiLineMode(true);
                                        if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                            ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                        {
                                            elementsAreChildren = true;
                                            PopAhead();
                                        }
                                        ParseProductionRule(id, esm, id.Position, elementsAreChildren, preprocessorInserter, container, maxK: curlyNumber.GetCleanValue());
                                        break;
                                    }
                                }
                                else
                                    Expect("Number");
                                break;
                            //ID-
                            case OilexerGrammarTokens.OperatorType.Minus:
                            //ID+
                            case OilexerGrammarTokens.OperatorType.Plus:
                                esm = ot.Type == OilexerGrammarTokens.OperatorType.Minus ? EntryScanMode.SingleLine : EntryScanMode.Multiline;
                                IOilexerGrammarToken ahead = PopAhead();
                                if (ahead.TokenType == OilexerGrammarTokenType.Operator)
                                {
                                    ot = ((OilexerGrammarTokens.OperatorToken)(ahead));
                                    switch (ot.Type)
                                    {
                                        case OilexerGrammarTokens.OperatorType.ColonColonEquals:
                                            SetMultiLineMode(true);
                                            if ((ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken) != null &&
                                                ot.Type == OilexerGrammarTokens.OperatorType.GreaterThan)
                                            {
                                                elementsAreChildren = true;
                                                PopAhead();
                                            }
                                            ParseProductionRule(id, esm, id.Position, elementsAreChildren, preprocessorInserter, container);
                                            break;
                                        case OilexerGrammarTokens.OperatorType.ColonEquals:
                                            {
                                                SetMultiLineMode(true);
                                                bool unhinged = false, contextual = false;
                                                if (TokenizerLookAhead(0) == '*')
                                                {
                                                    //ID :=*
                                                    /* *
                                                     * Token is unbounded by grammar
                                                     * and can appear anywhere.
                                                     * */
                                                    this.PopAhead();
                                                    unhinged = true;
                                                    if (TokenizerLookAhead(0) == '-')
                                                    {
                                                        //ID :=*-
                                                        /* *
                                                         * It's not valid to have both unhinged and contextual.
                                                         * */
                                                        LogError(OilexerGrammarParserErrors.Unexpected, "-");
                                                        this.PopAhead();
                                                    }
                                                }
                                                if (TokenizerLookAhead(0) == '-')
                                                {
                                                    //ID :=-
                                                    /* *
                                                     * Token is a contextual token and might be ambiguous with others
                                                     * special care needs taken when handling them to avoid incorrect
                                                     * parse paths.
                                                     * */
                                                    this.PopAhead();
                                                    contextual = true;
                                                    if (TokenizerLookAhead(0) == '*')
                                                    {
                                                        //ID :=-*
                                                        /* *
                                                         * It's not valid to have both contextual and unhinged.
                                                         * */
                                                        LogError(OilexerGrammarParserErrors.Unexpected, "*");
                                                        this.PopAhead();
                                                    }
                                                }
                                                ParseToken(id, esm, id.Position, unhinged, lowerPrecedences, forcedRecognizer, ppidb, contextual);
                                            }
                                            break;
                                        case OilexerGrammarTokens.OperatorType.Equals:
                                            break;
                                    }
                                }
                                else
                                    Expect("::=, :=, or =");
                                break;
                            default:
                                Expect("::=, :=, =, <, - or +");
                                break;
                        }
                    }
                    else
                    {
                        Expect("::=, :=, =, or <");
                        continue;
                    }
                }
                else if (container == PreprocessorContainer.Rule)
                {
                    IPreprocessorDirective ipd = ParsePreprocessor(container);
                    ppidb.Add(ipd);
                }
                else if (la0.TokenType == OilexerGrammarTokenType.Comment)
                {
                    var commentToken = la0 as OilexerGrammarTokens.CommentToken;
                    if (captureRegions && commentToken.MultiLine)
                    {
                        var gdResult = currentTarget.Result as OilexerGrammarFile;
                        gdResult.AddCommentRegion(commentToken);
                    }

                    PopAhead();
                }
                else
                {
                    Expect("#endif, #else, or #elif");
                    break;
                }
            }
        endBody:
            if (captureRegions)
            {
                var gdResult = this.currentTarget.Result as OilexerGrammarFile;
                long bodyEnd = this.StreamPosition;
                int startLine = this.CurrentTokenizer.GetLineIndex(bodyStart) + 1;
                int endLine = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (endLine > startLine + 1)
                {
                    var nlLen = Environment.NewLine.Length;
                    bodyStart = this.CurrentTokenizer.GetPositionFromLine(startLine) - nlLen;

                    bodyEnd = this.CurrentTokenizer.GetPositionFromLine(endLine) - nlLen;

                    gdResult.AddIfRegion(ipid, bodyStart, bodyEnd);
                }
            }
            ;
        }

        internal virtual void SetMultiLineMode(bool value)
        {
            var ct = ((Lexer)(this.CurrentTokenizer));
            ct.MultiLineMode = value;
        }

        private T ParseContinuous<T>(T incoming, SimpleParseDelegate<T> parseMethod)
            where T :
                class,
                IPreprocessorCExp
        {
            if (parseMethod == null)
                throw new ArgumentNullException("parseMethod");
            T current = incoming;
            T result = current;
            PushAhead(new OilexerGrammarTokens.ReferenceToken(current, current.Column, current.Line, current.Position));
            while (true)
            {
                parseMethod(ref result);
                /* *
                 * If the result hasn't changed since parsing further,
                 * the continuous parse is finished.
                 * */
                if (result == current)
                    break;
                else if (result == null)
                    LogError(OilexerGrammarParserErrors.Unexpected, "SourceError");
                else
                    PushAhead(new OilexerGrammarTokens.ReferenceToken(result, result.Column, result.Line, result.Position));
                current = result;
            }
            return current;
        }

        private bool ParsePreprocessorCLogicalOrConditionExp(ref IPreprocessorCLogicalOrConditionExp rResult)
        {
            /* *
             * LogicalOrExp ::= 
             *     LogicalOrExp '||' LogicalAndExp |
             *     LogicalAndExp;
             * --
             * Therefore, to solve the circular reference of the expression referencing itself
             * (LogicalOr ::= LogicalOr ...), we look for 'LogicalAndExp' transitionFirst, create a 
             * new LogicalOrExp using the LogicalAndExp if it parses properly.
             * Then parse continuously until it doesn't change (because there's no more expression
             * left).
             * */
            if (AheadLength == 0 || !ExpectReference<IPreprocessorCLogicalOrConditionExp>())
            {
                IPreprocessorCLogicalAndConditionExp singleState = null;
                if (ParsePreprocessorCLogicalAndConditionExp(ref singleState))
                {
                    rResult = ParseContinuous(new PreprocessorCLogicalOrConditionExp(singleState, singleState.Column, singleState.Line, singleState.Position), new SimpleParseDelegate<IPreprocessorCLogicalOrConditionExp>(ParsePreprocessorCLogicalOrConditionExp));
                    return true;
                }
                return false;
            }
            else
            {
                /* *
                 * Self-referencing trick, it was pushed to the stack as a reference
                 * token.
                 * */
                IPreprocessorCLogicalOrConditionExp last = GetReference<IPreprocessorCLogicalOrConditionExp>();
                //Check for the required '||'...
                if (LookPastAndSkip() == '|' && TokenizerLookAhead(1) == '|')
                    this.StreamPosition += 2;
                else if (rResult == null)
                {
                    Expect("||", CurrentTokenizer.Position);
                    this.StreamPosition += 1;
                    return false;
                }
                else
                {
                    return false;
                }
                IPreprocessorCLogicalAndConditionExp andExp = null;
                //second half, LogicalAndExp.
                if (ParsePreprocessorCLogicalAndConditionExp(ref andExp))
                {
                    rResult = new PreprocessorCLogicalOrConditionExp(last, andExp, last.Column, last.Line, last.Position);
                    return true;
                }
                else
                {
                    //If the last input was null, error.
                    this.PopAhead();
                    Expect("AndExp");
                }
            }
            return false;
        }

        private bool ParsePreprocessorCLogicalAndConditionExp(ref IPreprocessorCLogicalAndConditionExp rResult)
        {
            if (!ExpectReference<IPreprocessorCLogicalAndConditionExp>())
            {
                /* *
                 * LogicalAndExp ::= 
                 *     LogicalAndExp '&&' EqualityExp |
                 *     EqualityExp;
                 * --
                 * Therefore, to solve the circular reference of the expression referencing itself
                 * (LogicalAndExp ::= LogicalAndExp ...), we look for 'EqualityExp' transitionFirst, create a 
                 * new LogicalAndExp using the EqualityExp if it parses properly.
                 * Then parse continuously until it doesn't change (because there's no more expression
                 * left).
                 * */
                IPreprocessorCEqualityExp singleState = null;
                if (ParsePreprocessorCEqualityExp(ref singleState))
                {
                    rResult = ParseContinuous(new PreprocessorCLogicalAndConditionExp(singleState, singleState.Column, singleState.Line, singleState.Position), new SimpleParseDelegate<IPreprocessorCLogicalAndConditionExp>(ParsePreprocessorCLogicalAndConditionExp));
                    return true;
                }
                return false;
            }
            else
            {
                IPreprocessorCLogicalAndConditionExp last = GetReference<IPreprocessorCLogicalAndConditionExp>();
                //Required '&&'
                if (LookPastAndSkip() == '&' && TokenizerLookAhead(1) == '&')
                    this.StreamPosition += 2;
                else if (rResult == null)
                {
                    Expect("&&", CurrentTokenizer.Position);
                    this.StreamPosition += 1;
                    return false;
                }
                else
                {
                    return false;
                }
                IPreprocessorCEqualityExp eqExp = null;
                //Next half, parse the EqualityExp.
                if (ParsePreprocessorCEqualityExp(ref eqExp))
                {
                    rResult = new PreprocessorCLogicalAndConditionExp(last, eqExp, last.Column, last.Line, last.Position);
                    return true;
                }
                else if (rResult == null)
                {
                    //If the last input was null, error.
                    this.PopAhead();
                    Expect("EqualityExp");
                }
            }
            return false;
        }

        private bool ParsePreprocessorCEqualityExp(ref IPreprocessorCEqualityExp rResult)
        {
            if (!ExpectReference<IPreprocessorCEqualityExp>())
            {
                /* *
                 * EqualityExp ::= 
                 *     EqualityExp "!=" Primary |
                 *     EqualityExp "==" Primary |
                 *     Primary;
                 * --
                 * Therefore, to solve the circular reference of the expression referencing itself
                 * (EqualityExp ::= EqualityExp ...), we look for 'Primary' transitionFirst, create a 
                 * new EqualityExp using the Primary if it parses properly.
                 * Then parse continuously until it doesn't change (because there's no more expression
                 * left).
                 * */
                IPreprocessorCPrimary singleState = null;
                if (ParsePreprocessorCPrimary(ref singleState))
                {
                    rResult = ParseContinuous(new PreprocessorCEqualityExp(singleState, singleState.Column, singleState.Line, singleState.Position), new SimpleParseDelegate<IPreprocessorCEqualityExp>(ParsePreprocessorCEqualityExp));
                    return true;
                }
                return false;
            }
            else
            {
                IPreprocessorCEqualityExp last = GetReference<IPreprocessorCEqualityExp>();
                bool isEq = false;
                LookPastAndSkip();
                //Required '!=' or '=='
                if (TokenizerLookAhead(1) == '=')
                {
                    if (TokenizerLookAhead(0) == '=')
                        isEq = true;
                    else if (TokenizerLookAhead(0) != '!' && rResult == null)
                    {
                        Expect("== or !=", CurrentTokenizer.Position);
                        this.StreamPosition += 1;
                        return false;
                    }
                    this.PopAhead();
                }
                else if (rResult == null)
                {
                    Expect("== or !=", CurrentTokenizer.Position);
                    this.PopAhead();
                    return false;
                }
                else
                    return false;
                IPreprocessorCPrimary cPrimary = null;
                if (ParsePreprocessorCPrimary(ref cPrimary))
                {
                    rResult = new PreprocessorCEqualityExp(last, isEq, cPrimary, last.Column, last.Line, last.Position);
                    return true;
                }
                else if (rResult == null)
                {
                    //If the last input was null, error.
                    Expect("EqExp");
                }
            }
            return false;
        }

        private bool ParsePreprocessorCPrimary(ref IPreprocessorCPrimary cPrimary)
        {
            //if the next token is a '(' then we're in a sub-expression...
            if (LookPastAndSkip() == '(')
            {
                var leftParenthesis = this.PopAhead() as OilexerGrammarTokens.OperatorToken;
                IPreprocessorCLogicalOrConditionExp orExp = null;
                if (ParsePreprocessorCLogicalOrConditionExp(ref orExp))
                {
                    if (LookPastAndSkip() == ')')
                        //Skip it, not a token but a literal for CPrimary.
                        this.PopAhead();
                    else
                    {
                        Expect(")", CurrentTokenizer.Position);
                        this.PopAhead();
                        return false;
                    }
                    cPrimary = new PreprocessorCPrimary(leftParenthesis, orExp, orExp.Column, orExp.Line, orExp.Position);
                    return true;
                }
                else
                {
                    Expect("expression");
                    return false;
                }
            }
            else
            {
                IOilexerGrammarToken ahead = this.LookAhead(0);
                if (ahead.TokenType == OilexerGrammarTokenType.Identifier ||
                    ahead.TokenType == OilexerGrammarTokenType.StringLiteral ||
                    ahead.TokenType == OilexerGrammarTokenType.CharacterLiteral ||
                    ahead.TokenType == OilexerGrammarTokenType.NumberLiteral)
                    PopAhead();
                else
                {
                    /* *
                     * This guess was wrong, ready the stream for the
                     * next guess.
                     * * 
                     * Typically occurs when the expression terminates
                     * and the stream needs prepared for the next parse.
                     * Case in point when ')' is encountered while in a
                     * sub-expression, forgetting to clear the look ahead
                     * would result in an improper forward glance (when the
                     * caller looks for the required right parenthesis).
                     * */
                    ClearAhead();
                    return false;
                }
                if (ahead.TokenType == OilexerGrammarTokenType.Identifier)
                    cPrimary = new PreprocessorCPrimary(((OilexerGrammarTokens.IdentifierToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == OilexerGrammarTokenType.StringLiteral)
                    cPrimary = new PreprocessorCPrimary(((OilexerGrammarTokens.StringLiteralToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == OilexerGrammarTokenType.CharacterLiteral)
                    cPrimary = new PreprocessorCPrimary(((OilexerGrammarTokens.CharLiteralToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == OilexerGrammarTokenType.NumberLiteral)
                    cPrimary = new PreprocessorCPrimary(((OilexerGrammarTokens.NumberLiteral)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                return true;
            }
        }

        private T GetReference<T>()
            where T :
                class
        {
            if (LookAhead(0).TokenType == OilexerGrammarTokenType.ReferenceToken && ((OilexerGrammarTokens.ReferenceToken)(LookAhead(0))).Reference is T)
            {
                return (T)((OilexerGrammarTokens.ReferenceToken)(PopAhead(false))).Reference;
            }
            return null;
        }

        private bool ExpectReference<T>()
        {
            var ahead = LookAhead(0);
            if (ahead.TokenType == OilexerGrammarTokenType.ReferenceToken && ((OilexerGrammarTokens.ReferenceToken)(ahead)).Reference is T)
                return true;
            PopAhead(false);
            return false;
        }

        private IProductionRuleItem ParseProductionRuleReferenceItem(PreprocessorContainer container)
        {
            ISoftReferenceProductionRuleItem isrpri = null;
            OilexerGrammarTokens.IdentifierToken id = this.LookAhead(0) as OilexerGrammarTokens.IdentifierToken;
            if (id != null)
            {
                OilexerGrammarTokens.IdentifierToken id2 = null;
                PopAhead();
                if (LookAhead(0) == null && CurrentTokenizer.CurrentError != null)
                {
                    if (!(currentTarget.SyntaxErrors.Contains(CurrentTokenizer.CurrentError)))
                        currentTarget.SyntaxErrors.SyntaxError(CurrentTokenizer.CurrentError);
                    return null;
                }
                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Operator &&
                    ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.Period, false))
                {
                    id2 = LookAhead(1) as OilexerGrammarTokens.IdentifierToken;
                    if (id2 != null)
                        GetAhead(2);
                    else
                    {
                        var lookAhead = LookAhead(1);
                        if (lookAhead != null)
                        {
                            Expect("Identifier", lookAhead.Position);
                        }
                        else
                        {
                            Expect("Identifier");
                        }
                        goto YieldResult;
                    }
                    bool isFlag;
                    bool isCounter;
                    GetIsFlagOrCounter(out isFlag, out isCounter);
                    DefineRuleSoftReference(id, id2);
                    isrpri = new SoftReferenceProductionRuleItem(id.Name, id2.Name, id.Line, id.Column, id.Position, isFlag, isCounter)
                        {
                            PrimaryToken = id,
                            SecondaryToken = id2
                        };
                }
                else
                {
                    ClearAhead();
                    if (LookPastAndSkip() == '<')
                    {
                        //bool mlMode = this.CurrentTokenizer.
                        bool mlMode = ((Lexer)(this.CurrentTokenizer)).MultiLineMode;
                        SetMultiLineMode(true);
                        isrpri = ParseTemplateReference(id, container);
                        SetMultiLineMode(mlMode);
                    }
                    else
                    {
                        bool isFlag;
                        bool isCounter;
                        GetIsFlagOrCounter(out isFlag, out isCounter);
                        DefineRuleSoftReference(id);
                        isrpri = new SoftReferenceProductionRuleItem(id.Name, null, id.Line, id.Column, id.Position, isFlag, isCounter)
                            {
                                PrimaryToken = id
                            };
                    }
                }
            }
            else
                Expect("identifier");
        YieldResult:
            return isrpri;
        }

        protected virtual void DefineRuleIdentifier(OilexerGrammarTokens.IdentifierToken identifier, IOilexerGrammarProductionRuleEntry ruleEntry)
        {
        }

        protected virtual void DefineRuleTemplateIdentifier(OilexerGrammarTokens.IdentifierToken ruleTemplateIdentifier, IOilexerGrammarProductionRuleTemplateEntry ruleTemplateEntry)
        {
        }

        protected virtual void DefineCommandIdentifier(OilexerGrammarTokens.IdentifierToken commandIdentifier)
        {
        }
        protected virtual void DefineKeywordIdentifier(OilexerGrammarTokens.IdentifierToken keywordIdentifier)
        {
        }

        protected virtual void DefineTokenIdentifier(OilexerGrammarTokens.IdentifierToken identifier, IOilexerGrammarTokenEntry tokenEntry)
        {

        }

        protected virtual void DefineRuleSoftReference(OilexerGrammarTokens.IdentifierToken primary, OilexerGrammarTokens.IdentifierToken secondary = null)
        {
        }

        protected virtual void DefineRuleTemplateParameterIdentifier(OilexerGrammarTokens.IdentifierToken id, IProductionRuleTemplatePart currentPart)
        {
        }

        private void GetIsFlagOrCounter(out bool isFlag, out bool isCounter)
        {
            isFlag = false;
            isCounter = false;

            if (this.TokenizerLookAhead(0) == '!')
            {
                LookAhead(0);
                PopAhead();
                isFlag = true;
                if (this.TokenizerLookAhead(0) == '#')
                {
                    LookAhead(0);
                    PopAhead();
                    isCounter = true;
                }
            }
            else if (this.TokenizerLookAhead(0) == '#')
            {
                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Operator)
                {
                    PopAhead();
                    isCounter = true;
                    if (this.TokenizerLookAhead(0) == '!')
                    {
                        LookAhead(0);
                        PopAhead();
                        isFlag = true;
                    }
                }
            }
        }

        private ISoftTemplateReferenceProductionRuleItem ParseTemplateReference(OilexerGrammarTokens.IdentifierToken id, PreprocessorContainer container)
        {
            if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.LessThan, true))
            {
                List<IList<IProductionRule>> serii = new List<IList<IProductionRule>>();
                PopAhead();
                templateDepth++;
                while (true)
                {
                    IList<IProductionRule> current = new List<IProductionRule>();
                    ParseProductionRule(current, container);
                    ClearAhead();
                    serii.Add(current);
                    if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.Comma, false))
                    {
                        PopAhead();
                        continue;
                    }
                    else if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.GreaterThan, true))
                    {
                        PopAhead();
                        break;
                    }
                    else
                        break;
                }
                templateDepth--;
                return new SoftTemplateReferenceProductionRuleItem(serii.ToArray(), id.Name, id.Line, id.Column, id.Position)
                    {
                        PrimaryToken = id
                    };
            }
            return null;
        }

        private ILiteralStringProductionRuleItem ParseProductionRuleStringLiteral()
        {
            OilexerGrammarTokens.StringLiteralToken slt = LookAhead(0) as OilexerGrammarTokens.StringLiteralToken;
            if (slt == null)
            {
                Expect("\"string\"");
                return null;
            }
            PopAhead();
            bool isFlag = false, isCounter = false;
            if (this.TokenizerLookAhead(0) == '!')
            {
                PopAhead();
                isFlag = true;
                if (this.TokenizerLookAhead(0) == '#')
                {
                    PopAhead();
                    isCounter = true;
                }
            }
            else if (this.TokenizerLookAhead(0) == '#')
            {
                PopAhead();
                isCounter = true;
                if (this.TokenizerLookAhead(0) == '!')
                {
                    PopAhead();
                    isFlag = true;
                }
            }
            return new LiteralStringProductionRuleItem(slt.GetCleanValue(), slt.CaseInsensitive, slt.Column, slt.Line, slt.Position, isFlag, isCounter);
        }

        private IProductionRuleItem ParseProductionRuleCharLiteral()
        {
            OilexerGrammarTokens.CharLiteralToken slt = LookAhead(0) as OilexerGrammarTokens.CharLiteralToken;
            if (slt == null)
            {
                Expect("'char'");
                return null;
            }
            PopAhead();
            bool isFlag = false, isCounter = false;
            if (this.TokenizerLookAhead(0) == '!')
            {
                LookAhead(0);
                PopAhead();
                isFlag = true;
                if (this.TokenizerLookAhead(0) == '#')
                {
                    LookAhead(0);
                    PopAhead();
                    isCounter = true;
                }
            }
            else if (this.TokenizerLookAhead(0) == '#')
            {
                LookAhead(0);
                PopAhead();
                isCounter = true;
                if (this.TokenizerLookAhead(0) == '!')
                {
                    LookAhead(0);
                    PopAhead();
                    isFlag = true;
                }
            }
            return new LiteralCharProductionRuleItem(slt.GetCleanValue(), slt.CaseInsensitive, slt.Column, slt.Line, slt.Position, isFlag, isCounter);
        }

        private void ParseProductionRule(OilexerGrammarTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool elementsAreChildren, bool maxReduce, int? maxK)
        {
            ParseProductionRule(identifier, scanMode, position, elementsAreChildren, p => this.currentTarget.Result.Add(p), maxReduce: maxReduce, maxK: maxK);
        }
        private void ParseProductionRule(OilexerGrammarTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool elementsAreChildren, Action<IOilexerGrammarEntry> adder, PreprocessorContainer container = PreprocessorContainer.File, bool maxReduce = false, int? maxK = null)
        {
            long bodyStart = this.StreamPosition;
            IOilexerGrammarProductionRuleEntry ipre = new OilexerGrammarProductionRuleEntry(identifier.Name, scanMode, CurrentTokenizer.FileName, CurrentTokenizer.GetColumnIndex(position), CurrentTokenizer.GetLineIndex(position), position);
            ipre.MaxReduce = maxReduce;
            ipre.LookaheadTokenLimit = maxK;
            ipre.IsRuleCollapsePoint = elementsAreChildren;
            ParseProductionRule(((OilexerGrammarProductionRuleEntry)ipre).BaseCollection, container);
            long bodyEnd = this.StreamPosition;
            adder(ipre);
            DefineRuleIdentifier(identifier, ipre);
            if (captureRegions)
            {
                int lineStart = this.CurrentTokenizer.GetLineIndex(bodyStart);
                int lineEnd = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (lineEnd > lineStart + 1)
                {
                    var gdResult = (OilexerGrammarFile)currentTarget.Result;
                    gdResult.AddRuleRegion(ipre, bodyStart, bodyEnd);
                }
            }
        }

        private void ParseToken(OilexerGrammarTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool unhinged, List<OilexerGrammarTokens.IdentifierToken> lowerPrecedences, bool forcedRecognizer, bool contextual)
        {
            ParseToken(identifier, scanMode, position, unhinged, lowerPrecedences, forcedRecognizer, null, contextual);
        }

        private void ParseToken(OilexerGrammarTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool unhinged, List<OilexerGrammarTokens.IdentifierToken> lowerPrecedences, bool forcedRecognizer, PreprocessorIfDirective.DirectiveBody target, bool contextual)
        {
            long bodyStart = this.StreamPosition;
            IOilexerGrammarTokenEntry ite = new OilexerGrammarTokenEntry(identifier.Name, ParseTokenExpressionSeries(), scanMode, this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetColumnIndex(position), this.CurrentTokenizer.GetLineIndex(position), position, unhinged, lowerPrecedences, forcedRecognizer) { Contextual=contextual };
            if (target == null)
                this.currentTarget.Result.Add(ite);
            else
                target.Add(new PreprocessorEntryContainer(ite, ite.Column, ite.Line, ite.Position));
            long bodyEnd = this.StreamPosition;
            this.DefineTokenIdentifier(identifier, ite);
            if (captureRegions)
            {
                int lineStart = this.CurrentTokenizer.GetLineIndex(bodyStart);
                int lineEnd = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (lineEnd > lineStart + 1)
                {
                    var gdResult = (OilexerGrammarFile)currentTarget.Result;
                    gdResult.AddTokenRegion(ite, bodyStart, bodyEnd);
                }
            }
        }

        private ITokenExpressionSeries ParseTokenExpressionSeries()
        {
            int l = this.CurrentTokenizer.GetLineIndex(this.StreamPosition);
            int ci = this.CurrentTokenizer.GetColumnIndex(this.StreamPosition);
            long p = this.StreamPosition;

            List<ITokenExpression> expressions = new List<ITokenExpression>();
            ParseTokenExpressions(expressions);
            return new TokenExpressionSeries(expressions.ToArray(), l, ci, p, this.CurrentTokenizer.FileName);
        }

        private void ParseTokenExpressions(List<ITokenExpression> expressions)
        {
            while (true)
            {
                ClearAhead();
                char c = LookPastAndSkip();
                if (c == ';')
                {
                    ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.SemiColon, true);
                    break;
                }
                else if (c == '|')
                {
                    if (!(ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.Pipe, true)))
                        break;
                    ParseTokenBody(expressions);
                    continue;
                }
                else if (c == '/' && (TokenizerLookAhead(1) == '*' || TokenizerLookAhead(1) == '/'))
                {
                    var igdt = LookAhead(0);
                    if (igdt.TokenType == OilexerGrammarTokenType.Comment)
                    {
                        var commentToken = (OilexerGrammarTokens.CommentToken)igdt;
                        if (captureRegions && commentToken.MultiLine)
                        {
                            var gdResult = (OilexerGrammarFile)currentTarget.Result;
                            gdResult.AddCommentRegion(commentToken);
                        }

                        PopAhead();
                    }
                    else
                        goto Expect;
                }
                else if (c == ')' && parameterDepth > 0)
                    break;
                else if (c == ',' && parameterDepth > 0)
                    if (commandDepths.Contains(parameterDepth))
                        break;
                    else
                        goto Expect;
                else if (Lexer.IsIdentifierChar(c) || c == '(' || c == '\'' || c == '"' || c == '@' || c == '[')
                    ParseTokenBody(expressions);
                else
                    goto Expect;
                continue;
            Expect:
                Expect("string, char, identifier, preprocessor, or ';'");
                break;
            }
        }

        private ITokenGroupItem ParseTokenGroup()
        {
            OilexerGrammarTokens.OperatorToken ot = LookAhead(0) as OilexerGrammarTokens.OperatorToken;

            if (ot != null && ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.LeftParenthesis, true))
            {
                List<ITokenExpression> items = new List<ITokenExpression>();
                parameterDepth++;
                ParseTokenExpressions(items);
                parameterDepth--;
                ITokenGroupItem result = new TokenGroupItem(this.CurrentTokenizer.FileName, items.ToArray(), ot.Column, ot.Line, ot.Position);
                var endTok = PopAhead();
                if (captureRegions)
                {
                    var gdResult = this.currentTarget.Result as OilexerGrammarFile;
                    int lineStart = ot.Line;
                    int lineEnd = endTok.Line;
                    if (lineEnd > lineStart + 1)
                        gdResult.AddTokenGroupRegion(result, ot.Position, endTok.Position);
                }
                ExpectOperator(endTok, OilexerGrammarTokens.OperatorType.RightParenthesis, true);
                return result;
            }
            return null;
        }

        private void ParseTokenBody(List<ITokenExpression> expressions)
        {
            int l = this.CurrentTokenizer.GetLineIndex(this.StreamPosition);
            int ci = this.CurrentTokenizer.GetColumnIndex(this.StreamPosition);
            long p = this.StreamPosition;
            List<ITokenItem> seriesItemMembers = new List<ITokenItem>();
            while (true)
            {
                ITokenItem item = null;
                IOilexerGrammarToken igdt = LookAhead(0);
                if (igdt == null && CurrentTokenizer.CurrentError != null)
                {
                    currentTarget.SyntaxErrors.SyntaxError(CurrentTokenizer.CurrentError);

                    break;
                }
                else
                {
                    if (igdt == null)
                    {
                        LogError(OilexerGrammarParserErrors.Expected, "identifier, '(' or ';'");
                        return;
                    }
                    switch (igdt.TokenType)
                    {
                        case OilexerGrammarTokenType.Comment:
                            var commentToken = (OilexerGrammarTokens.CommentToken)igdt;
                            if (captureRegions && commentToken.MultiLine)
                            {
                                var gdResult = (OilexerGrammarFile)currentTarget.Result;
                                gdResult.AddCommentRegion(commentToken);
                            }
                            PopAhead();
                            continue;
                        case OilexerGrammarTokenType.CharacterLiteral:
                            item = ParseTokenCharLiteral();
                            break;
                        case OilexerGrammarTokenType.StringLiteral:
                            item = ParseTokenStringLiteral();
                            break;
                        case OilexerGrammarTokenType.Identifier:
                            item = ParseTokenReferenceItem();
                            break;
                        case OilexerGrammarTokenType.CharacterRange:
                            item = ParseTokenCharacterRange();
                            break;
                        case OilexerGrammarTokenType.NumberLiteral:
                            LogError(OilexerGrammarParserErrors.Unexpected, "number", igdt.Position);
                            goto yield;
                        case OilexerGrammarTokenType.PreprocessorDirective:
                            LogError(OilexerGrammarParserErrors.Unexpected, "preprocessor", igdt.Position);
                            goto yield;
                        case OilexerGrammarTokenType.Operator:
                            switch (((OilexerGrammarTokens.OperatorToken)(igdt)).Type)
                            {
                                case OilexerGrammarTokens.OperatorType.Pipe:
                                    ClearAhead();
                                    goto yield;
                                case OilexerGrammarTokens.OperatorType.LeftParenthesis:
                                    ClearAhead();
                                    item = (ITokenGroupItem)ParseTokenGroup();
                                    break;
                                case OilexerGrammarTokens.OperatorType.RightParenthesis:
                                    if (parameterDepth <= 0)
                                        LogError(OilexerGrammarParserErrors.Expected, ";", igdt.Position);
                                    goto yield;
                                case OilexerGrammarTokens.OperatorType.GreaterThan:
                                case OilexerGrammarTokens.OperatorType.Comma:
                                    if (commandDepths.Contains(parameterDepth))
                                    {
                                        ClearAhead();
                                        goto yield;
                                    }
                                    PopAhead();
                                    LogError(OilexerGrammarParserErrors.Expected, "identifier, '(', or ';'");
                                    PopAhead();
                                    goto yield;
                                case OilexerGrammarTokens.OperatorType.SemiColon:
                                    ClearAhead();
                                    if (templateDepth != 0 || parameterDepth != 0)
                                    {
                                        Expect((templateDepth == 0 ? ">" : string.Empty) + (parameterDepth > 0 ? ")" : string.Empty), this.StreamPosition);
                                    }
                                    goto yield;
                                default:
                                    goto default_2;
                            }
                            break;
                        default:
                        default_2:
                            LogError(OilexerGrammarParserErrors.Expected, "identifier, '(' or ';'");
                            PopAhead();
                            break;
                    }
                }
                if (item != null)
                {
                    ClearAhead();
                    if (TokenizerLookAhead(0) == ':')
                    {
                        ParseItemOptions(item);
                    }
                    ParseItemRepeatOptions(item);
                    seriesItemMembers.Add(item);
                }
            }
        yield:
            expressions.Add(new TokenExpression(seriesItemMembers, CurrentTokenizer.FileName, l, ci, p));
        }

        private void ParseItemRepeatOptions(IScannableEntryItem item, bool itemIsRuleItem = false)
        {
            //Just in case -_-
            ClearAhead();
            if (TokenizerLookAhead(0) == '$')
            {
                var dollar = this.PopAhead();

                if (TokenizerLookAhead(0) == '?')
                {
                    item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrOne | ScannableEntryItemRepeatInfo.MaxReduce;
                    this.PopAhead();
                }
                else if (TokenizerLookAhead(0) == '+')
                {
                    item.RepeatOptions = ScannableEntryItemRepeatInfo.OneOrMore | ScannableEntryItemRepeatInfo.MaxReduce;
                    this.PopAhead();
                }
                else if (TokenizerLookAhead(0) == '*')
                {
                    if (TokenizerLookAhead(1) == '*' && item is TokenItem)
                    {
                        this.PopAhead();
                        ((TokenItem)(item)).SiblingAmbiguity = true;
                        if (TokenizerLookAhead(0) == '*')
                        {
                            item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore | ScannableEntryItemRepeatInfo.MaxReduce;
                            this.PopAhead();
                        }
                    }
                    else if (TokenizerLookAhead(1) == '*' && item is TokenGroupItem)
                    {
                        this.PopAhead();
                        ((TokenGroupItem)(item)).SiblingAmbiguity = true;
                        if (TokenizerLookAhead(0) == '*')
                        {
                            item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore | ScannableEntryItemRepeatInfo.MaxReduce;
                            this.PopAhead();
                        }
                    }
                    else
                    {
                        item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore | ScannableEntryItemRepeatInfo.MaxReduce;
                        this.PopAhead();
                    }
                }
                else if (TokenizerLookAhead(0) == '{')
                {
                    this.PopAhead();
                    var numberOp = this.GetAhead(2);
                    //Either min unbounded or error.
                    if (!(SeriesMatch(numberOp, OilexerGrammarTokenType.NumberLiteral, OilexerGrammarTokenType.Operator) ||
                          SeriesMatch(numberOp, OilexerGrammarTokenType.Operator, OilexerGrammarTokenType.NumberLiteral)))
                    {
                        //SourceError of some kind.
                        if (numberOp[0].TokenType == OilexerGrammarTokenType.NumberLiteral)
                            ExpectOperator(numberOp[1], OilexerGrammarTokens.OperatorType.Minus | OilexerGrammarTokens.OperatorType.Comma, true);
                        else
                            Expect("Number or ','", numberOp[0].Position);
                    }
                    if (numberOp[0].TokenType == OilexerGrammarTokenType.Operator)
                    {
                        OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)numberOp[0];
                        if (op.Type == OilexerGrammarTokens.OperatorType.Comma)
                            item.RepeatOptions = new ScannableEntryItemRepeatInfo(null, ((OilexerGrammarTokens.NumberLiteral)(numberOp[1])).GetCleanValue()) | ScannableEntryItemRepeatInfo.MaxReduce;
                        else
                            Expect("Number or ','", numberOp[0].Position);
                        ExpectOperator(this.PopAhead(), OilexerGrammarTokens.OperatorType.RightCurlyBrace, true);
                    }
                    else
                    {
                        OilexerGrammarTokens.NumberLiteral num = (OilexerGrammarTokens.NumberLiteral)numberOp[0];
                        OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)numberOp[1];
                        if (op.Type == OilexerGrammarTokens.OperatorType.Comma)
                        {
                            var max = this.PopAhead();
                            if (max.TokenType == OilexerGrammarTokenType.NumberLiteral)
                            {
                                var maxNumber = max as OilexerGrammarTokens.NumberLiteral;
                                var maxTail = this.PopAhead();
                                if (maxTail.TokenType == OilexerGrammarTokenType.Operator &&
                                    ((OilexerGrammarTokens.OperatorToken)(maxTail)).Type == OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                                    item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), maxNumber.GetCleanValue()) | ScannableEntryItemRepeatInfo.MaxReduce;
                                else
                                    ExpectOperator(maxTail, OilexerGrammarTokens.OperatorType.RightCurlyBrace, true);
                            }
                            else if (max.TokenType == OilexerGrammarTokenType.Operator)
                            {
                                var maxTail = max as OilexerGrammarTokens.OperatorToken;
                                if (maxTail.Type != OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                                    Expect("Number or }", max.Position);
                                else
                                    item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), null) | ScannableEntryItemRepeatInfo.MaxReduce;
                            }
                            else
                                Expect("Number or }", max.Position);
                        }
                        else if (op.Type == OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                        {
                            int cl = num.GetCleanValue();
                            item.RepeatOptions = new ScannableEntryItemRepeatInfo(cl, cl) | ScannableEntryItemRepeatInfo.MaxReduce;
                        }
                        else
                            ExpectOperator(op, OilexerGrammarTokens.OperatorType.RightCurlyBrace | OilexerGrammarTokens.OperatorType.Comma, true);
                    }
                }
                else
                    item.RepeatOptions = ScannableEntryItemRepeatInfo.MaxReduce;
                if (itemIsRuleItem)
                {
                    if (TokenizerLookAhead(0) == '@')
                    {
                        item.RepeatOptions |= ScannableEntryItemRepeatInfo.AnyOrder | ScannableEntryItemRepeatInfo.MaxReduce;
                        this.PopAhead();
                    }
                }
            }
            if (TokenizerLookAhead(0) == '?')
            {
                item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrOne;
                this.PopAhead();
                if (TokenizerLookAhead(0) == '$')
                {
                    this.PopAhead();
                    item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                }
            }
            else if (TokenizerLookAhead(0) == '+')
            {
                item.RepeatOptions = ScannableEntryItemRepeatInfo.OneOrMore;
                this.PopAhead();
                if (TokenizerLookAhead(0) == '$')
                {
                    this.PopAhead();
                    item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                }
            }
            else if (TokenizerLookAhead(0) == '*')
            {
                if (TokenizerLookAhead(1) == '*' && item is TokenItem)
                {
                    this.PopAhead();
                    ((TokenItem)(item)).SiblingAmbiguity = true;
                    if (TokenizerLookAhead(0) == '*')
                    {
                        item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore;
                        this.PopAhead();
                    }
                }
                else if (TokenizerLookAhead(1) == '*' && item is TokenGroupItem)
                {
                    this.PopAhead();
                    ((TokenGroupItem)(item)).SiblingAmbiguity = true;
                    if (TokenizerLookAhead(0) == '*')
                    {
                        item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore;
                        this.PopAhead();
                    }
                }
                else
                {
                    item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore;
                    this.PopAhead();
                    if (TokenizerLookAhead(0) == '$')
                    {
                        this.PopAhead();
                        item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                    }
                }
            }
            else if (TokenizerLookAhead(0) == '{')
            {
                this.PopAhead();
                var numberOp = this.GetAhead(2);
                //Either min unbounded or error.
                if (!(SeriesMatch(numberOp, OilexerGrammarTokenType.NumberLiteral, OilexerGrammarTokenType.Operator) ||
                      SeriesMatch(numberOp, OilexerGrammarTokenType.Operator, OilexerGrammarTokenType.NumberLiteral)))
                {
                    //SourceError of some kind.
                    if (numberOp[0].TokenType == OilexerGrammarTokenType.NumberLiteral)
                        ExpectOperator(numberOp[1], OilexerGrammarTokens.OperatorType.Minus | OilexerGrammarTokens.OperatorType.Comma, true);
                    else
                        Expect("Number or ','", numberOp[0].Position);
                }
                else if (numberOp[0].TokenType == OilexerGrammarTokenType.Operator)
                {
                    OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)numberOp[0];
                    if (op.Type == OilexerGrammarTokens.OperatorType.Comma)
                        item.RepeatOptions = new ScannableEntryItemRepeatInfo(null, ((OilexerGrammarTokens.NumberLiteral)(numberOp[1])).GetCleanValue());
                    else
                        Expect("Number or ','", numberOp[0].Position);
                    ExpectOperator(this.PopAhead(), OilexerGrammarTokens.OperatorType.RightCurlyBrace, true);
                    if (TokenizerLookAhead(0) == '$')
                    {
                        this.PopAhead();
                        item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                    }
                }
                else
                {
                    OilexerGrammarTokens.NumberLiteral num = (OilexerGrammarTokens.NumberLiteral)numberOp[0];
                    OilexerGrammarTokens.OperatorToken op = (OilexerGrammarTokens.OperatorToken)numberOp[1];
                    if (op.Type == OilexerGrammarTokens.OperatorType.Comma)
                    {
                        var max = this.PopAhead();
                        if (max.TokenType == OilexerGrammarTokenType.NumberLiteral)
                        {
                            var maxNumber = (OilexerGrammarTokens.NumberLiteral)max;
                            var maxTail = this.PopAhead();
                            if (maxTail.TokenType == OilexerGrammarTokenType.Operator &&
                                ((OilexerGrammarTokens.OperatorToken)(maxTail)).Type == OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                                item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), maxNumber.GetCleanValue());
                            else
                                ExpectOperator(maxTail, OilexerGrammarTokens.OperatorType.RightCurlyBrace, true);
                            if (TokenizerLookAhead(0) == '$')
                            {
                                this.PopAhead();
                                item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                            }
                        }
                        else if (max.TokenType == OilexerGrammarTokenType.Operator)
                        {
                            var maxTail = (OilexerGrammarTokens.OperatorToken)max;
                            if (maxTail.Type != OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                                Expect("Number or }", max.Position);
                            else
                                item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), null);
                            if (TokenizerLookAhead(0) == '$')
                            {
                                this.PopAhead();
                                item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                            }
                        }
                        else
                            Expect("Number or }", max.Position);
                    }
                    else if (op.Type == OilexerGrammarTokens.OperatorType.RightCurlyBrace)
                    {
                        int cl = num.GetCleanValue();
                        item.RepeatOptions = new ScannableEntryItemRepeatInfo(cl, cl);
                        if (TokenizerLookAhead(0) == '$')
                        {
                            this.PopAhead();
                            item.RepeatOptions |= ScannableEntryItemRepeatInfo.MaxReduce;
                        }
                    }
                    else
                        ExpectOperator(op, OilexerGrammarTokens.OperatorType.RightCurlyBrace | OilexerGrammarTokens.OperatorType.Comma, true);
                }
            }
            if (itemIsRuleItem)
                if (TokenizerLookAhead(0) == '@')
                {
                    item.RepeatOptions |= ScannableEntryItemRepeatInfo.AnyOrder;
                    this.PopAhead();
                }

        }

        private ICharRangeTokenItem ParseTokenCharacterRange()
        {
            OilexerGrammarTokens.CharacterRangeToken crt = LookAhead(0) as OilexerGrammarTokens.CharacterRangeToken;
            if (crt != null)
            {
                PopAhead();
                return new CharRangeTokenItem(crt.Inverted, crt.Ranges, crt.Line, crt.Column, crt.Position);
            }
            else
                Expect("Character range");
            return null;
        }

        private ILiteralStringTokenItem ParseTokenStringLiteral()
        {
            OilexerGrammarTokens.StringLiteralToken slt = LookAhead(0) as OilexerGrammarTokens.StringLiteralToken;
            if (slt == null)
            {
                Expect("\"string\"");
                return null;
            }
            PopAhead();
            return new LiteralStringTokenItem(slt.GetCleanValue(), slt.CaseInsensitive, slt.Column, slt.Line, slt.Position, false);
        }

        private ITokenItem ParseTokenCharLiteral()
        {
            OilexerGrammarTokens.CharLiteralToken slt = LookAhead(0) as OilexerGrammarTokens.CharLiteralToken;
            if (slt == null)
            {
                PopAhead();
                Expect("'char'");
                return null;
            }
            PopAhead();
            return new LiteralCharTokenItem(slt.GetCleanValue(), slt.CaseInsensitive, slt.Column, slt.Line, slt.Position);
        }
        private ITokenItem ParseTokenReferenceItem()
        {
            ISoftReferenceTokenItem isrti = null;
            OilexerGrammarTokens.IdentifierToken id = this.LookAhead(0) as OilexerGrammarTokens.IdentifierToken;
            if (id != null)
            {
                OilexerGrammarTokens.IdentifierToken id2 = null;
                PopAhead();
                if (IdIsTokenCommand(id.Name))
                    return ParseCommandTokenItem(id);
                if (LookAhead(0).TokenType == OilexerGrammarTokenType.Operator &&
                    ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.Period, false))
                {
                    id2 = (OilexerGrammarTokens.IdentifierToken)LookAhead(1);
                    GetAhead(2);
                    isrti = new SoftReferenceTokenItem(id.Name, id2.Name, id.Column, id.Line, id.Position)
                    {
                        PrimaryToken = id,
                        SecondaryToken = id2
                    };
                }
                else
                    isrti = new SoftReferenceTokenItem(id.Name, null, id.Column, id.Line, id.Position)
                    {
                        PrimaryToken = id
                    };
            }
            else
                Expect("identifier");
            return isrti;
        }

        private ICommandTokenItem ParseCommandTokenItem(OilexerGrammarTokens.IdentifierToken id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (!IdIsTokenCommand(id.Name))
                throw new ArgumentException("id");
            if (!ExpectOperator(PopAhead(), OilexerGrammarTokens.OperatorType.LeftParenthesis, true))
                return null;
            int myDepth = ++parameterDepth;
            commandDepths.Add(myDepth);
            OilexerGrammarTokens.NumberLiteral numChars = null;
            OilexerGrammarTokens.NumberLiteral numericBase = null;
            OilexerGrammarTokens.StringLiteralToken stringBase = null;
            OilexerGrammarTokens.OperatorToken lastOp = null;
            IOilexerGrammarToken baseTok = null;
            List<ITokenExpressionSeries> commandExpressionSets = new List<ITokenExpressionSeries>();
            try
            {
            nextSet:
                commandExpressionSets.Add(ParseTokenExpressionSeries());
                if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.Comma, false))
                {
                    lastOp = (OilexerGrammarTokens.OperatorToken)PopAhead();
                    if (id.Name.ToLower() != "baseencode")
                        goto nextSet;
                    if (LookAhead(0).TokenType == OilexerGrammarTokenType.NumberLiteral)
                        baseTok = numericBase = (OilexerGrammarTokens.NumberLiteral)PopAhead();
                    else if (LookAhead(0).TokenType == OilexerGrammarTokenType.StringLiteral)
                        baseTok = stringBase = (OilexerGrammarTokens.StringLiteralToken)PopAhead();
                    if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.Comma, true))
                    {
                        lastOp = (OilexerGrammarTokens.OperatorToken)PopAhead();
                        if (LookAhead(0).TokenType == OilexerGrammarTokenType.NumberLiteral)
                            numChars = (OilexerGrammarTokens.NumberLiteral)PopAhead();
                    }
                }
                if (ExpectOperator(LookAhead(0), OilexerGrammarTokens.OperatorType.RightParenthesis, true))
                    lastOp = (OilexerGrammarTokens.OperatorToken)PopAhead();
                switch (id.Name.ToLower())
                {
                    case "baseencode":
                        DefineCommandIdentifier(id);
                        if (commandExpressionSets.Count != 1)
                        {
                            LogError(OilexerGrammarParserErrors.FixedArgumentCountError, "BaseEncode command requires exactly one lexical expression, the base, and the number of characters per encoding.", id.Position);
                            return null;
                        }
                        else if (numericBase == null && stringBase == null)
                        {
                            Expect("Number or String", lastOp == null ? baseTok == null ? id.Position : baseTok.Position : lastOp.Position);
                            return null;
                        }
                        else if (numChars == null || numChars.GetCleanValue() < 2)
                        {
                            Expect("Number of 2 or greater", numChars == null ? lastOp == null ? baseTok == null ? id.Position : baseTok.Position : lastOp.Position : numChars.Position);
                            return null;
                        }
                        else if (numChars.GetCleanValue() < 2)
                        {
                            Expect("Number of 2 or greater", numChars.Position);
                            return null;
                        }
                        else if (numericBase == null)
                            return new BaseEncodeGraphCommand(commandExpressionSets[0], stringBase, numChars, id.Column, id.Line, id.Position);
                        var cleanDigits = numericBase.GetCleanValue();
                        switch (cleanDigits)
                        {
                            case 8:
                            case 16:
                            case 18:
                            case 27:
                            case 36:
                            case 60:
                                break;
                            default:
                                Expect("8, 16, 18, 27, 36, or 60", numChars.Position);
                                LogError(OilexerGrammarParserErrors.Unexpected, cleanDigits.ToString(), numChars.Position);
                                return null;
                        }
                        return new BaseEncodeGraphCommand(commandExpressionSets[0], numericBase, numChars, id.Column, id.Line, id.Position);
                    case "scan":
                        DefineCommandIdentifier(id);
                        if (commandExpressionSets.Count != 2)
                        {

                            LogError(OilexerGrammarParserErrors.FixedArgumentCountError, "Scan command requires exactly two parameters.", id.Position);
                            if (commandExpressionSets.Count <= 0)
                                return null;
                            else
                                return new ScanCommandTokenItem(commandExpressionSets[0], false, id.Column, id.Line, id.Position);
                        }
                        else
                        {
                            if (commandExpressionSets[1].Count == 1 &&
                                commandExpressionSets[1][0].Count == 1)
                            {
                                var softRef = commandExpressionSets[1][0][0] as ISoftReferenceTokenItem;
                                if (softRef != null)
                                {
                                    bool value = false;
                                    if (softRef.PrimaryName.ToLower() == "true")
                                        value = true;
                                    else if (softRef.PrimaryName.ToLower() != "false")
                                        Expect("true or false", softRef.Position);
                                    if (softRef.PrimaryToken != null)
                                        this.DefineKeywordIdentifier(softRef.PrimaryToken);
                                    return new ScanCommandTokenItem(commandExpressionSets[0], value, id.Column, id.Line, id.Position);
                                }
                            }
                            else
                                Expect("true or false", commandExpressionSets[0].Position);
                        }
                        break;
                    case "subtract":
                        DefineCommandIdentifier(id);
                        if (commandExpressionSets.Count != 2)
                        {
                            LogError(OilexerGrammarParserErrors.FixedArgumentCountError, "Subtraction command requires exactly two parameters.", id.Position);
                            if (commandExpressionSets.Count <= 0)
                                return new SubtractionCommandTokenItem(null, null, id.Column, id.Line, id.Position);
                            else if (commandExpressionSets.Count == 1)
                                return new SubtractionCommandTokenItem(commandExpressionSets[0], null, id.Column, id.Line, id.Position);
                            else
                                return new SubtractionCommandTokenItem(commandExpressionSets[0], commandExpressionSets[1], id.Column, id.Line, id.Position);
                        }
                        else
                            return new SubtractionCommandTokenItem(commandExpressionSets[0], commandExpressionSets[1], id.Column, id.Line, id.Position);
                }
                return null;
            }
            finally
            {
                commandDepths.Remove(myDepth--);
                parameterDepth = myDepth;
            }
        }

        private bool IdIsProductionRuleCommand(string idName)
        {
            return idName.ToLower() == "subtract";
        }

        private bool IdIsTokenCommand(string idName)
        {
            switch (idName.ToLower())
            {
                case "baseencode":
                case "scan":
                case "subtract":
                    return true;
            }
            return false;
        }

        private bool ExpectBoolean(IOilexerGrammarToken boolIDToken, bool error)
        {
            var boolID = boolIDToken as OilexerGrammarTokens.IdentifierToken;
            if (boolID != null &&
                (ExpectIdentifier(boolIDToken, "true", StringComparison.InvariantCultureIgnoreCase, false) ||
                 ExpectIdentifier(boolIDToken, "false", StringComparison.InvariantCultureIgnoreCase, false)))
            {
                this.DefineKeywordIdentifier(boolID);
                return true;
            }
            else if (error)
                Expect("true or false", boolIDToken.Position);
            return false;
        }

        private bool SeriesMatch(ITokenStream<IOilexerGrammarToken> its, params OilexerGrammarTokenType[] gdtt)
        {
            for (int i = 0; i < its.Count; i++)
                if (i >= gdtt.Length)
                    break;
                else
                    if (its[i].TokenType != gdtt[i])
                        return false;

            return its.Count >= gdtt.Length;
        }

        private void Expect(string s)
        {
            LogError(OilexerGrammarParserErrors.Expected, s);
        }

        private void LogError(OilexerGrammarParserErrors error)
        {
            this.LogError(error, string.Empty);
        }

        private void LogError(OilexerGrammarParserErrors error, string text)
        {
            currentTarget.SyntaxErrors.SyntaxError(OilexerGrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetLineIndex(this.StreamPosition), this.CurrentTokenizer.GetColumnIndex(this.StreamPosition), error, text));
        }
        private void Expect(string s, long position)
        {
            LogError(OilexerGrammarParserErrors.Expected, s, position);
        }

        private void LogError(OilexerGrammarParserErrors error, long position)
        {
            this.LogError(error, string.Empty, position);
        }

        private void LogError(OilexerGrammarParserErrors error, string text, long position)
        {
            currentTarget.SyntaxErrors.SyntaxError(OilexerGrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetLineIndex(position), this.CurrentTokenizer.GetColumnIndex(position), error, text));
        }

        public T NextAhead<T>()
            where T :
                IOilexerGrammarToken
        {
            return (T)this.LookAhead(this.AheadLength);
        }


    }
}
