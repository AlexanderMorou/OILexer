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
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers
{
    public partial class OILexerParser :
        Parser<IGDToken, IGDTokenizer, IGDFile>,
        IGDParser
    {
        private List<string> includeDirectives = new List<string>();
        private List<string> parsedIncludeDirectives = new List<string>();
        private List<int> commandDepths = new List<int>();
        private GDParserResults currentTarget;
        private int templateDepth = 0;
        private int parameterDepth = 0;
        private IDictionary<string, string> definedSymbols = new Dictionary<string,string>();
        private delegate bool SimpleParseDelegate<T>(ref T target);
        private bool parseIncludes;
        private bool captureRegions;
        public OILexerParser()
            : this(true)
        {

        }
        public OILexerParser(bool parseIncludes, bool captureRegions = false, IList<IToken> originalFormTokens = null)
            : base(originalFormTokens)
        {
            this.parseIncludes = parseIncludes;
            this.captureRegions = captureRegions;
        }

        /// <summary>
        /// Parses the <paramref name="fileName"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="fileName">The file to parse.</param>
        /// <returns>An instance of an implementation of <see cref="IParserResults{T}"/> 
        /// that indicates the success of the operation, with an instance of 
        /// <see cref="IGDFile"/>, if successful.</returns>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown when 
        /// <paramref name="fileName"/> is not found.</exception>
        public override IParserResults<IGDFile> Parse(string fileName)
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
            fileName = fileName.ToLower();
            #endif
            IParserResults<IGDFile> result = null;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            result = this.Parse(fs, fileName);
            fs.Close();
            return result;
        }

        /// <summary>
        /// Parses the <paramref name="stream"/> and returns the appropriate <see cref="IParserResults{T}"/>
        /// </summary>
        /// <param name="s">The stream to parse.</param>
        /// <param name="fileName">The file name used provided an error is encountered in the <see cref="Stream"/>, <paramref name="s"/>.</param>
        /// <returns>An instance of an implementation of <see cref="IParserResults{T}"/> 
        /// that indicates the success of the operation with an instance of 
        /// <see cref="IGDFile"/>, if successful.</returns>
        public override IParserResults<IGDFile> Parse(Stream s, string fileName)
        {
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
            IParserResults<IGDFile> gf = this.BeginParse();
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
            return gf;
        }

        /* *
         * Initializes the parse process.
         * */
        protected IParserResults<IGDFile> BeginParse()
        {
            //Instantiate the parser results and give it a blank file to work with.
            currentTarget = new GDParserResults();
            currentTarget.SetResult(new GDFile(this.CurrentTokenizer.FileName));
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
                        continue;
                    }
                    IGDToken it = its[0];
                    if (it == null || it.TokenType != GDTokenType.Comment)
                        Expect("comment");
                    var commentToken = ((GDTokens.CommentToken)(it));
                    if (captureRegions && commentToken.MultiLine)
                        (this.currentTarget.Result as GDFile).AddCommentRegion(commentToken);
                    this.currentTarget.Result.Add(new CommentEntry(commentToken.Comment, CurrentTokenizer.FileName, it.Column, it.Line, it.Position));
                }
                //Expected preprocessor.
                else if (c == '#')
                {
                    GDTokens.PreprocessorDirective preprocessorToken = this.LookAhead(0) as GDTokens.PreprocessorDirective;
                    if (preprocessorToken != null)
                    {
                        switch (preprocessorToken.Type)
                        {
                            case GDTokens.PreprocessorType.IfDirective:
                            case GDTokens.PreprocessorType.IfDefinedDirective:
                            case GDTokens.PreprocessorType.IfNotDefinedDirective:
                                PopAhead();
                                IPreprocessorCLogicalOrConditionExp condition = null;
                                if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                                {
                                    var ifEntry = ParseIfDirective(condition, preprocessorToken, PreprocessorContainer.File);
                                    ProcessIfEntry(ifEntry);
                                }
                                break;
                            case GDTokens.PreprocessorType.DefineDirective:
                                IPreprocessorDirective ipd = ParsePreprocessor(PreprocessorContainer.File);
                                var defineDirective = ipd as IPreprocessorDefineSymbolDirective;
                                if (defineDirective != null)
                                    if (!definedSymbols.ContainsKey(defineDirective.SymbolName))
                                        definedSymbols.Add(defineDirective.SymbolName, defineDirective.Value);
                                break;
                            case GDTokens.PreprocessorType.ThrowDirective:
                                break;
                            default:
                                goto parseSimple;
                        }
                        goto postSimple;;
                    parseSimple: 
                        ProcessStringTerminal(ParseStringTerminalDirective());
                    postSimple: ;

                    }
                }
                else if (Lexer.IsIdentifierChar(c))
                {
                    /* *
                     * All declarations must begin at the first column 
                     * of the line.
                     * */
                    if (this.CurrentTokenizer.GetColumnIndex(this.StreamPosition) != 1)
                    {
                        GetAhead(1);
                        LogError(GDParserErrors.ExpectedEndOfLine);
                        continue;
                    }
                    SetMultiLineMode(true);
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
                    if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
                    {
                        List<string> lowerPrecedences = new List<string>();
                        GDTokens.IdentifierToken id = ((GDTokens.IdentifierToken)(its[0]));
                        GDTokens.OperatorToken ot = ((GDTokens.OperatorToken)(its[1]));
                        bool elementsAreChildren = false;
                        bool forcedRecognizer = false;
                    checkOp:
                        switch (ot.Type)
                        {
                            //ID$
                            case GDTokens.OperatorType.ForcedStringForm:
                                forcedRecognizer = true;
                                if (LookAhead(0).TokenType == GDTokenType.Operator)
                                {
                                    ot = (GDTokens.OperatorToken)PopAhead();
                                    goto checkOp;
                                }
                                else
                                {
                                    ExpectOperator(LookAhead(0), GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.GreaterThan, true);
                                    PopAhead();
                                }
                                break;
                            //ID ::=
                            case GDTokens.OperatorType.ColonColonEquals:
                                if ((ot = LookAhead(0) as GDTokens.OperatorToken) != null &&
                                    ot.Type == GDTokens.OperatorType.GreaterThan)
                                {
                                    elementsAreChildren = true;
                                    PopAhead();
                                }
                                SetMultiLineMode(true);
                                ParseProductionRule(id, EntryScanMode.Inherited, id.Position, elementsAreChildren);
                                break;
                            //ID :=
                            case GDTokens.OperatorType.ColonEquals:
                                {
                                    SetMultiLineMode(true);
                                    bool unhinged = false;
                                    if (TokenizerLookAhead(0) == '*')
                                    {
                                        //ID :=*
                                        /* *
                                         * Token is unbounded by grammar
                                         * and can appear anywhere.
                                         * */
                                        this.PopAhead();
                                        unhinged = true;
                                    }
                                    ParseToken(id, EntryScanMode.Inherited, id.Position, unhinged, lowerPrecedences, forcedRecognizer);
                                }
                                break;
                            //ID =
                            case GDTokens.OperatorType.Equals:
                                ParseError(id.Name, id.Position);
                                break;
                            //ID<
                            case GDTokens.OperatorType.LessThan:
                                SetMultiLineMode(true);
                                IList<IProductionRuleTemplatePart> parts = this.ParseProductionRuleTemplateParts(ot);
                                if (parts == null)
                                    continue;
                                else
                                {
                                    IGDToken g = null;
                                    GDTokens.OperatorToken gdt = (g = PopAhead()) as GDTokens.OperatorToken;
                                    if (gdt == null)
                                        ExpectOperator(g, GDTokens.OperatorType.ColonColonEquals | GDTokens.OperatorType.Plus | GDTokens.OperatorType.Minus, true);
                                        //Expect("+, -, or ::=");
                                    if (gdt.Type == GDTokens.OperatorType.Minus)
                                    {
                                        gdt = (g = PopAhead()) as GDTokens.OperatorToken;
                                        esm = EntryScanMode.SingleLine;
                                    }
                                    else if (gdt.Type == GDTokens.OperatorType.Plus)
                                    {
                                        gdt = (g = PopAhead()) as GDTokens.OperatorToken;
                                        esm = EntryScanMode.Multiline;
                                    }
                                    if (gdt == null || !ExpectOperator(gdt, GDTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        ExpectOperator(g, GDTokens.OperatorType.ColonColonEquals, true);
                                        //Expect("::=");
                                        break;
                                    }
                                    this.ParseProductionRuleTemplate(id, parts, esm, id.Position);
                                }
                                break;
                            //ID>
                            case GDTokens.OperatorType.GreaterThan:
                                //Case of a token defining precedence.
                                while (LookAhead(0).TokenType == GDTokenType.Identifier)
                                {
                                    var lowerPrecedenceToken = ((GDTokens.IdentifierToken)LookAhead(0)).Name;
                                    lowerPrecedences.Add(lowerPrecedenceToken);
                                    PopAhead();
                                    GDTokens.OperatorToken cOp = null;
                                    //','  * More precedences exist.
                                    if ((cOp = (LookAhead(0) as GDTokens.OperatorToken)) != null &&
                                        cOp.Type == GDTokens.OperatorType.Comma)
                                        PopAhead();
                                    //":=" * Final precedence reached.
                                    else if (cOp != null &&
                                        cOp.Type == GDTokens.OperatorType.ColonEquals)
                                    {
                                        PopAhead();
                                        goto case GDTokens.OperatorType.ColonEquals;
                                    }
                                    else
                                    {
                                        //Syntax error.
                                        ExpectOperator(LookAhead(0), GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.Comma, true);
                                        break;
                                    }
                                }
                                break;
                            //ID-
                            case GDTokens.OperatorType.Minus:
                            //ID+
                            case GDTokens.OperatorType.Plus:
                                esm = ot.Type == GDTokens.OperatorType.Minus ? EntryScanMode.SingleLine : EntryScanMode.Multiline;
                                IGDToken ahead = PopAhead();
                                if (ahead.TokenType == GDTokenType.Operator)
                                {
                                    ot = ((GDTokens.OperatorToken)(ahead));
                                    switch (ot.Type)
                                    {
                                        case GDTokens.OperatorType.ColonColonEquals:
                                            SetMultiLineMode(true);
                                            if ((ot = LookAhead(0) as GDTokens.OperatorToken) != null &&
                                                ot.Type == GDTokens.OperatorType.GreaterThan)
                                            {
                                                elementsAreChildren = true;
                                                PopAhead();
                                            }
                                            ParseProductionRule(id, esm, id.Position, elementsAreChildren);
                                            break;
                                        case GDTokens.OperatorType.ColonEquals:
                                            {
                                                SetMultiLineMode(true);
                                                bool unhinged = false;
                                                if (TokenizerLookAhead(0) == '*')
                                                {
                                                    //ID[+-] :=*
                                                    /* *
                                                     * Token is unbounded by grammar
                                                     * and can appear anywhere.
                                                     * */
                                                    this.PopAhead();
                                                    unhinged = true;
                                                }
                                                ParseToken(id, esm, id.Position, unhinged, lowerPrecedences, forcedRecognizer);
                                            }
                                            break;
                                        case GDTokens.OperatorType.Equals:
                                            break;
                                    }
                                }
                                else
                                    ExpectOperator(ahead, GDTokens.OperatorType.ColonColonEquals | GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.Equals, true);
                                break;
                            default:
                                ExpectOperator(ot, GDTokens.OperatorType.ColonColonEquals | GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.Equals |  GDTokens.OperatorType.LessThan | GDTokens.OperatorType.Minus | GDTokens.OperatorType.Plus, true);
                                break;
                        }
                    }
                    else
                    {
                        ExpectOperator(its[1], GDTokens.OperatorType.ColonColonEquals | GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.Equals | GDTokens.OperatorType.LessThan | GDTokens.OperatorType.Minus | GDTokens.OperatorType.Plus, true);
                        continue;
                    }
                }
                else if (Lexer.IsWhitespaceChar(c))
                    //Skip
                    SkipWhitespace();
                else
                {
                    LogError(GDParserErrors.ExpectedEndOfLine);
                    GetAhead(1);
                }
            }
            GDParserResults gdpr = currentTarget;
            currentTarget = null;
            if (this.parseIncludes)
                ParseIncludes(gdpr);
            return gdpr;
        }

        internal virtual void SkipWhitespace()
        {
            ((Lexer)(this.CurrentTokenizer)).ParseWhitespaceInternal();
        }

        private void ProcessIfEntry(IPreprocessorIfDirective ifEntry)
        {
            ProcessIfEntry(ifEntry, PreprocessorContainer.File, (IEntry p) => this.currentTarget.Result.Add(p));
        }

        private void ProcessIfEntry<T>(IPreprocessorIfDirective ifEntry, PreprocessorContainer container, Action<T> adder)
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
                        ProcessIfEntry(ifEntry.Next);
                    break;
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIfDefined:
                    bool check = ProcessDefinedCondition(ifEntry.Condition, ifEntry.Type != EntryPreprocessorType.IfNotDefined);
                    if (check)
                        ProcessIfBody(ifEntry.Body, container, adder);
                    else if (ifEntry.Next != null)
                        ProcessIfEntry(ifEntry.Next);
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
                            LogError(GDParserErrors.UnknownSymbol, left, eqExp.PreCEqualityExp.PreCPrimary.Identifier.Position);
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
                            LogError(GDParserErrors.UnknownSymbol, right, eqExp.PreCPrimary.Identifier.Position);
                            return false;
                        }
                    }
                    if (eqExp.Rule == 1)
                        return left == right;
                    else
                        return left != right;
                case 3:
                    ExpectOperator(eqExp.PreCPrimary.Token, GDTokens.OperatorType.EqualEqual | GDTokens.OperatorType.ExclaimEqual, true);
                    return false;
                default:
                    LogError(GDParserErrors.Unexpected, "Unexpected kind of equality expression", eqExp.Position);
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
                        ProcessIfEntry((IPreprocessorIfDirective)item);
                        break;
                    case EntryPreprocessorType.DefineRule:
                        LogError(GDParserErrors.Unexpected, "Unexpected rule declaration command.", item.Position);
                        break;
                    case EntryPreprocessorType.AddRule:
                        LogError(GDParserErrors.Unexpected, "Unexpected add rule command.", item.Position);
                        break;
                    case EntryPreprocessorType.Throw:
                        throw new NotImplementedException();
                    case EntryPreprocessorType.Return:
                        LogError(GDParserErrors.Unexpected, "Unexpected return command.", item.Position);
                        break;
                    case EntryPreprocessorType.StringTerminal:
                        if (container == PreprocessorContainer.File)
                            ProcessStringTerminal((IPreprocessorStringTerminalDirective)(item));
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
                                LogError(GDParserErrors.Unexpected, "define directive: symbol already exists", defineDirective.Position);
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
                    LogError(GDParserErrors.UnknownSymbol, stringTerminal.Position);
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
            var its = GetAhead(4);
            if (SeriesMatch(its, GDTokenType.StringLiteral, GDTokenType.Operator, GDTokenType.NumberLiteral, GDTokenType.Operator))
            {
                if (ExpectOperator(its[1], GDTokens.OperatorType.Comma, true) && 
                    ExpectOperator(its[3], GDTokens.OperatorType.SemiColon, true))
                {
                    IErrorEntry iee = new ErrorEntry(name, ((GDTokens.StringLiteralToken)(its[0])).GetCleanValue(), ((GDTokens.NumberLiteral)(its[2])).GetCleanValue(), this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetColumnIndex(position), this.CurrentTokenizer.GetLineIndex(position), position);
                    this.currentTarget.Result.Add(iee);
                }
            }
            else if (its[0].TokenType == GDTokenType.StringLiteral)
            {
                var second = its[1];
                if (second.TokenType == GDTokenType.Operator && (((GDTokens.OperatorToken)(second)).Type == GDTokens.OperatorType.Comma))
                {
                    if (its[2].TokenType == GDTokenType.NumberLiteral)
                        ExpectOperator(its[3], GDTokens.OperatorType.SemiColon, true);
                    else
                        Expect("number", its[2].Position);
                }
                else
                    ExpectOperator(second, GDTokens.OperatorType.Comma, true);
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
                include = GrammarCore.CombinePaths(Path.GetDirectoryName(CurrentTokenizer.FileName), include);
            #if WINDOWS
            /* *
             * Win32 only because it's not case-sensitive on filenames.
             * This ensures that the same file isn't included twice due to case
             * inconsistencies.
             * */
            include = include.ToLower();
            #endif

            if (File.Exists(include))
            {
                if (!(currentTarget.Result.Includes.Contains(include)))
                    currentTarget.Result.Includes.Add(include);
                //if (!(includeDirectives.Contains(include)))
                //    this.includeDirectives.Add(include);
            }
            else
                LogError(GDParserErrors.IncludeFileNotFound, include, str.Position);
        }

        private void ParseIncludes(GDParserResults currentTarget)
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
                    IParserResults<IGDFile> includedFile = this.Parse(s);
                    currentTarget.SetResult(currentTarget.Result + (GDFile)includedFile.Result);
                    if (!includedFile.Successful)
                        foreach (var ce in includedFile.SyntaxErrors)
                            currentTarget.SyntaxErrors.SyntaxError(ce.Message, ce.Location.Line, ce.Location.Column, ce.FileName);
                }
            }
        }

        private IList<IProductionRuleTemplatePart> ParseProductionRuleTemplateParts(GDTokens.OperatorToken currentOperator)
        {
            var currentToken = currentOperator;
            if (!(currentOperator != null &&
                currentOperator.Type == GDTokens.OperatorType.LessThan))
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
                if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
                {
                    GDTokens.OperatorToken ot = its[1] as GDTokens.OperatorToken;
                    GDTokens.IdentifierToken id = its[0] as GDTokens.IdentifierToken;
                    switch (ot.Type)
                    {
                        case GDTokens.OperatorType.OptionsSeparator:
                            its = GetAhead(5);
                            if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator, GDTokenType.Identifier, GDTokenType.Operator, GDTokenType.Operator))
                                if (ExpectIdentifier(its[0], "expect", StringComparison.InvariantCultureIgnoreCase, true))
                                {
                                    special = TemplatePartExpectedSpecial.None;
                                    if (ExpectOperator(its[1], GDTokens.OperatorType.Equals, true))
                                        if (ExpectIdentifier(its[2], "token", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.Token;
                                        else if (ExpectIdentifier(its[2], "tokenorrule", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.TokenOrRule;
                                        else if (ExpectIdentifier(its[2], "rule", StringComparison.InvariantCultureIgnoreCase, false))
                                            special = TemplatePartExpectedSpecial.Rule;
                                        else
                                        {
                                            special =  null;
                                            expectTarget = ((GDTokens.IdentifierToken)(its[2])).Name;
                                        }
                                    if (ExpectOperator(its[3], GDTokens.OperatorType.SemiColon, true))
                                    {
                                        ot = its[4] as GDTokens.OperatorToken;
                                        if (ot.Type == GDTokens.OperatorType.Comma)
                                            goto case GDTokens.OperatorType.Comma;
                                        else if (ot.Type == GDTokens.OperatorType.GreaterThan)
                                            goto case GDTokens.OperatorType.GreaterThan;
                                        else if (ot.Type == GDTokens.OperatorType.Plus)
                                            goto case GDTokens.OperatorType.Plus;
                                        else
                                            Expect("'+', ',' or '>'", ot.Position);
                                    }
                                }
                                else
                                    return null;
                            else
                                return null;
                            break;
                        case GDTokens.OperatorType.Comma:
                            if (special == null)
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, expectTarget, id.Line, id.Column, id.Position);
                            else
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, special.Value, id.Line, id.Column, id.Position);
                            this.DefineRuleTemplateParameterIdentifier(id, currentPart);
                            result.Add(currentPart);
                            break;
                        case GDTokens.OperatorType.Plus:
                            currentIsSeries = true;
                            IGDToken gdt = PopAhead();
                            if (ExpectOperator(gdt, GDTokens.OperatorType.Comma, false))
                                goto case GDTokens.OperatorType.Comma;
                            else if (ExpectOperator(gdt, GDTokens.OperatorType.GreaterThan, false))
                                goto case GDTokens.OperatorType.GreaterThan;
                            else
                                Expect("',' or '>'");
                            break;
                        case GDTokens.OperatorType.GreaterThan:
                            if (special == null)
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, expectTarget, id.Line, id.Column, id.Position);
                            else
                                currentPart = new ProductionRuleTemplatePart(id.Name, currentIsSeries, special.Value, id.Line, id.Column, id.Position);
                            this.DefineRuleTemplateParameterIdentifier(id, currentPart);
                            result.Add(currentPart);
                            goto _ter;
                        default:
                            Expect("'+', '(', ',' or '>'");
                            return null;
                    }
                }
                else if ((its[0]).TokenType == GDTokenType.Identifier)
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

        private bool ExpectIdentifier(IGDToken cur, string idValue, StringComparison comparisonType, bool error)
        {
            if (cur.TokenType == GDTokenType.Identifier && (string.Compare(((GDTokens.IdentifierToken)(cur)).Name, idValue, comparisonType) == 0))
                return true;
            else if (error)
                Expect(idValue);
            return false;
        }

        private bool ExpectOperator(IGDToken gdt, GDTokens.OperatorType operatorType, bool error)
        {
            if (gdt != null && gdt.TokenType == GDTokenType.Operator && (((GDTokens.OperatorToken)(gdt)).Type & operatorType) != GDTokens.OperatorType.None)
                return true;
            else if (error)
                Expect(operatorType.ToString(), gdt.Position);
            return false;
        }

        private void ParseProductionRuleTemplate(GDTokens.IdentifierToken identifier, IList<IProductionRuleTemplatePart> parts, EntryScanMode scanMode, long position)
        {
            ParseProductionRuleTemplate(identifier, parts, scanMode, position, null);
        }

        private void ParseProductionRuleTemplate(GDTokens.IdentifierToken identifier, IList<IProductionRuleTemplatePart> parts, EntryScanMode scanMode, long position, PreprocessorIfDirective.DirectiveBody target)
        {
            long bodyStart = this.StreamPosition;
            IProductionRuleTemplateEntry iprte = new ProductionRuleTemplateEntry(identifier.Name, scanMode, parts, CurrentTokenizer.FileName, CurrentTokenizer.GetColumnIndex(position), CurrentTokenizer.GetLineIndex(position), position);
            ParseProductionRule(((ProductionRuleTemplateEntry)iprte).BaseCollection, PreprocessorContainer.Template);
            long bodyEnd = this.StreamPosition;
            this.DefineRuleTemplateIdentifier(identifier, iprte);
            if (captureRegions)
            {
                int startLine = this.CurrentTokenizer.GetLineIndex(bodyStart);
                int endLine = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (endLine > startLine + 1)
                {
                    var gdResult = this.currentTarget.Result as GDFile;
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
                ClearAhead();
                char c = LookPastAndSkip();
                if (c == char.MinValue)
                    break;
                else if (c == ';')
                {
                    ExpectOperator(PopAhead(), GDTokens.OperatorType.SemiColon, true);
                    break;
                }
                else if (c == '|')
                {
                    if (!(ExpectOperator(PopAhead(), GDTokens.OperatorType.Pipe, true)))
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
                        var preprocessorAhead =(GDTokens.PreprocessorDirective) LookAhead(0);
                        if (preprocessorAhead.Type == GDTokens.PreprocessorType.EndIfDirective ||
                            preprocessorAhead.Type == GDTokens.PreprocessorType.ElseIfDirective ||
                            preprocessorAhead.Type == GDTokens.PreprocessorType.ElseDirective ||
                            preprocessorAhead.Type == GDTokens.PreprocessorType.ElseIfDefinedDirective)
                            break;
                    }
                    ParseProductionRuleBody(iprte, container);
                }
                else if (Lexer.IsWhitespaceChar(c))
                    //Skip
                    SkipWhitespace();
                else
                {
                    Expect("string, char, identifier, preprocessor, or ';'");
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
                IGDToken igdt = LookAhead(0);
                if (igdt == null && CurrentTokenizer.CurrentError != null)
                {
                    var ce = CurrentTokenizer.CurrentError;
                    this.currentTarget.SyntaxErrors.SyntaxError(ce.Message, ce.Location.Line, ce.Location.Column, ce.FileName);
                    return;
                }
                else if (igdt == null)
                    goto yield;
                switch (igdt.TokenType)
                {
                    case GDTokenType.Comment:
                        var commentToken = igdt as GDTokens.CommentToken;
                        if (captureRegions && commentToken.MultiLine)
                        {
                            var gdResult = currentTarget.Result as GDFile;
                            gdResult.AddCommentRegion(commentToken);
                        }
                        PopAhead();
                        continue;
                    case GDTokenType.CharacterLiteral:
                        item = ParseProductionRuleCharLiteral();
                        break;
                    case GDTokenType.StringLiteral:
                        item = ParseProductionRuleStringLiteral();
                        break;
                    case GDTokenType.Identifier:
                        item = ParseProductionRuleReferenceItem(container);
                        break;
                    case GDTokenType.CharacterRange:
                        LogError(GDParserErrors.Unexpected, "character range", igdt.Position);
                        PopAhead();
                        break;
                    //case GDTokenType.CharacterRangeCommand:
                    //    LogError(GDParserErrors.Unexpected, "CharRange(*", igdt.Position);
                    //    break;
                    case GDTokenType.NumberLiteral:
                        LogError(GDParserErrors.Unexpected, "number", igdt.Position);
                        PopAhead();
                        break;
                    case GDTokenType.PreprocessorDirective:
                        GDTokens.PreprocessorDirective ppDirective = ((GDTokens.PreprocessorDirective)(igdt));
                        if (ppDirective.Type == GDTokens.PreprocessorType.EndIfDirective ||
                            ppDirective.Type == GDTokens.PreprocessorType.ElseIfDirective ||
                            ppDirective.Type == GDTokens.PreprocessorType.ElseDirective ||
                            ppDirective.Type == GDTokens.PreprocessorType.ElseIfDefinedDirective)
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
                                case GDTokens.PreprocessorType.IfDirective:
                                case GDTokens.PreprocessorType.IfDefinedDirective:
                                case GDTokens.PreprocessorType.IfNotDefinedDirective:
                                    PopAhead();
                                    IPreprocessorCLogicalOrConditionExp condition = null;
                                    if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                                    {
                                        var ifEntry = ParseIfDirective(condition, ppDirective, PreprocessorContainer.Rule);
                                        List<IProductionRule> set = new List<IProductionRule>();
                                        bool first = true, yieldFast = false;
                                        ProcessIfEntry(ifEntry, PreprocessorContainer.Rule, (IProductionRule current) => 
                                            {
                                                if (first)
                                                {
                                                    first = false;
                                                    foreach (var subElement in current)
                                                        seriesItemMembers.Add(subElement);
                                                }
                                                else
                                                {
                                                    if (!yieldFast)
                                                        yieldFast = true;
                                                    set.Add(current);
                                                }
                                            });
                                        if (yieldFast)
                                            goto yield;
                                    }
                                    break;
                                case GDTokens.PreprocessorType.DefineDirective:
                                    LogError(GDParserErrors.Unexpected, "Define directive inside production rule.", ppDirective.Position);
                                    break;
                                case GDTokens.PreprocessorType.AddRuleDirective:
                                    LogError(GDParserErrors.Unexpected, "Add rule directive inside production rule.", ppDirective.Position);
                                    break;
                                default:
                                    Expect("#if, #ifdef, #ifndef,identifier, '(' or ';'", ppDirective.Position);
                                    break;
                            }
                        }
                        break;
                    case GDTokenType.Operator:
                        switch (((GDTokens.OperatorToken)(igdt)).Type)
                        {
                            case GDTokens.OperatorType.Pipe:
                                ClearAhead();
                                goto yield;
                            case GDTokens.OperatorType.LeftParenthesis:
                                ClearAhead();
                                parameterDepth++;
                                item = (IProductionRuleGroupItem)ParseProductionRuleGroup(container);
                                parameterDepth--;
                                break;
                            case GDTokens.OperatorType.RightParenthesis:
                                goto yield;
                            case GDTokens.OperatorType.GreaterThan:
                            case GDTokens.OperatorType.Comma:
                                if (templateDepth <= 0)
                                    LogError(GDParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(', or ';'");
                                goto yield;
                            case GDTokens.OperatorType.SemiColon:
                                ClearAhead();
                                if (templateDepth != 0 || parameterDepth != 0)
                                {
                                    Expect((templateDepth == 0 ? ">" : string.Empty) + (parameterDepth > 0 ? ")" : string.Empty), this.StreamPosition);
                                }
                                goto yield;
                            default:
                                goto outerDefault;
                        }
                        break;
                    default:
                    outerDefault: { }
                    {
                        PopAhead();
                        LogError(GDParserErrors.Expected, "#if, #ifdef, #ifndef,identifier, '(' or ';'");
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
            series.Add(new ProductionRule(seriesItemMembers, CurrentTokenizer.FileName, l, ci, p));
        }

        private IProductionRuleGroupItem ParseProductionRuleGroup(PreprocessorContainer container)
        {
            
            GDTokens.OperatorToken ot = LookAhead(0) as GDTokens.OperatorToken;
            if (ot != null && ExpectOperator(PopAhead(), GDTokens.OperatorType.LeftParenthesis, true))
            {
                List<IProductionRule> items = new List<IProductionRule>();
                ParseProductionRule(items, container);
                IProductionRuleGroupItem result = new ProductionRuleGroupItem(items.ToArray(), ot.Column, ot.Line, ot.Position);
                var endTok = this.PopAhead();
                long groupEnd = endTok.Position;
                if (captureRegions)
                {
                    var gdResult = this.currentTarget.Result as GDFile;
                    int lineStart = ot.Line;
                    int lineEnd = endTok.Line;
                    if (lineEnd > lineStart + 1)
                        gdResult.AddRuleGroupRegion(result, ot.Position, groupEnd);
                }
                ExpectOperator(endTok, GDTokens.OperatorType.RightParenthesis, true);
                return result;
            }
            return null;
        }

        private void ParseItemOptions(IScannableEntryItem item)
        {
            GDTokens.OperatorToken ot = LookAhead(0) as GDTokens.OperatorToken;
            if (ot != null && ExpectOperator(ot, GDTokens.OperatorType.OptionsSeparator, true))
            {
                PopAhead();
                var its = GetAhead(2);
                if (its.Count != 2)
                {
                    Expect("Name and ';'");
                    return;
                }
                if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
                {
                    GDTokens.IdentifierToken idt = its[0] as GDTokens.IdentifierToken;
                    GDTokens.OperatorToken op = its[1] as GDTokens.OperatorToken;
                    if (ExpectOperator(op, GDTokens.OperatorType.SemiColon, true))
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
                if (LookAhead(0).TokenType == GDTokenType.Identifier &&
                    LookAhead(1).TokenType == GDTokenType.Operator &&
                    LookAhead(2).TokenType == GDTokenType.Identifier &&
                    LookAhead(3).TokenType == GDTokenType.Operator)
                { 
                    if (ExpectIdentifier(LookAhead(0), "default", StringComparison.InvariantCultureIgnoreCase, false) &&
                        ExpectOperator(LookAhead(1), GDTokens.OperatorType.Equals, false) &&
                        //At this point, we can expect beyond a doubt, so error if there is no
                        //';' operator.
                        ExpectOperator(LookAhead(3), GDTokens.OperatorType.SemiColon, true))
                    {
                        string v = ((GDTokens.IdentifierToken)LookAhead(2)).Name;
                        if (item is TokenGroupItem)
                            ((TokenGroupItem)item).DefaultSoftRefOrValue = v;
                        else if (item is TokenItem)
                            ((TokenItem)(item)).DefaultSoftRefOrValue = v;
                        GetAhead(4);
                    }
                }
            }
        }

        private void ParseFlagOption(IScannableEntryItem item)
        {

            bool isCharItem;
            if ((isCharItem = (item is LiteralCharTokenItem)) ||
                item is LiteralStringTokenItem)
            {
                if (LookAhead(0).TokenType == GDTokenType.Identifier &&
                    LookAhead(1).TokenType == GDTokenType.Operator &&
                    LookAhead(2).TokenType == GDTokenType.Identifier &&
                    LookAhead(3).TokenType == GDTokenType.Operator)
                {
                    if (ExpectIdentifier(LookAhead(0), "flag", StringComparison.InvariantCultureIgnoreCase, false) &&
                        ExpectOperator(LookAhead(1), GDTokens.OperatorType.Equals, false) &&
                        //At this point, we can expect beyond a doubt, so error if there is no
                        //boolean or ';' operator.
                        ExpectBoolean(LookAhead(2), true) &&
                        ExpectOperator(LookAhead(3), GDTokens.OperatorType.SemiColon, true))
                    {
                        var aheadFour = GetAhead(4);
                        var thirdAhead = (GDTokens.IdentifierToken)aheadFour[2];
                        if (isCharItem)
                        {
                            var charItem = item as LiteralCharTokenItem;
                            charItem.IsFlag = bool.Parse(thirdAhead.Name);
                            charItem.IsFlagToken = thirdAhead;
                        }
                        else//LiteralStringTokenItem
                        {
                            var stringItem = item as LiteralStringTokenItem;
                            stringItem.IsFlag = bool.Parse(thirdAhead.Name);
                            stringItem.IsFlagToken = thirdAhead;
                        }
                        DefineKeywordIdentifier((GDTokens.IdentifierToken)aheadFour[0]);
                        DefineKeywordIdentifier(thirdAhead);
                    }
                }
            }
        }

        private IProductionRulePreprocessorDirective ParseProductionRulePreprocessor(PreprocessorContainer container)
        {
            GDTokens.PreprocessorDirective ppd = LookAhead(0) as GDTokens.PreprocessorDirective;
            if (ppd != null)
            {
                IPreprocessorDirective ippd = ParsePreprocessor(container);
                if (ippd != null)
                    return new ProductionRulePreprocessorDirective(ippd, ppd.Column, ppd.Line, ppd.Position);
            }
            Expect("Preprocessor Directive");
            return null;
        }

        private IPreprocessorDirective ParsePreprocessor(PreprocessorContainer container)
        {
            IPreprocessorDirective result = null;
            bool ml = ((Lexer)this.CurrentTokenizer).MultiLineMode;
            this.SetMultiLineMode(false);
            GDTokens.PreprocessorDirective ppd = LookAhead(0) as GDTokens.PreprocessorDirective;
            if (ppd != null)
            {
                switch (ppd.Type)
                {
                    case GDTokens.PreprocessorType.IncludeDirective:
                    case GDTokens.PreprocessorType.RootDirective:
                    case GDTokens.PreprocessorType.AssemblyNameDirective:
                    case GDTokens.PreprocessorType.LexerNameDirective:
                    case GDTokens.PreprocessorType.GrammarNameDirective:
                    case GDTokens.PreprocessorType.ParserNameDirective:
                    case GDTokens.PreprocessorType.TokenPrefixDirective:
                    case GDTokens.PreprocessorType.TokenSuffixDirective:
                    case GDTokens.PreprocessorType.RulePrefixDirective:
                    case GDTokens.PreprocessorType.RuleSuffixDirective:
                        if (container == PreprocessorContainer.File)
                            result = ParseStringTerminalDirective();
                        else
                        {
                            string ppdS = ppd.Type.ToString();
                            ppdS = ppdS.Substring(0, ppdS.Length - 9);
                            LogError(GDParserErrors.Unexpected, string.Format("{0} directive.", ppdS), ppd.Position);
                        }
                        break;
                    case GDTokens.PreprocessorType.IfDefinedDirective:
                    case GDTokens.PreprocessorType.IfNotDefinedDirective:
                    case GDTokens.PreprocessorType.IfDirective:
                        PopAhead();
                        IPreprocessorCLogicalOrConditionExp condition = null;
                        if (ParsePreprocessorCLogicalOrConditionExp(ref condition))
                        {
                            result = ParseIfDirective(condition, ppd, container);
                        }
                        break;
                    case GDTokens.PreprocessorType.AddRuleDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseAddRuleDirective();
                        break;
                    case GDTokens.PreprocessorType.DefineDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseDefineRuleDirective();
                        else if (container == PreprocessorContainer.File)
                            result = ParseDefineSymbolDirective(ppd);
                        else
                            LogError(GDParserErrors.Unexpected, "Define directive.", ppd.Position);
                        break;
                    case GDTokens.PreprocessorType.ReturnDirective:
                        if (container == PreprocessorContainer.Template)
                            result = ParseReturnDirective();
                        else
                            LogError(GDParserErrors.Unexpected, "Return directive.", ppd.Position);
                        break;
                    case GDTokens.PreprocessorType.ThrowDirective:
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
                this.ParseProductionRuleBody(containedSet, container);
                this.SetMultiLineMode(multiLine);
                return new PreprocessorProductionRuleSeries(containedSet, column, line, position);
            }
            return result;
        }

        private IPreprocessorDirective ParseDefineSymbolDirective(GDTokens.PreprocessorDirective preprocessor)
        {
            var stream = GetAhead(3);
            if (SeriesMatch(stream, GDTokenType.PreprocessorDirective, GDTokenType.Identifier, GDTokenType.Operator))
            {
                GDTokens.PreprocessorDirective def = (GDTokens.PreprocessorDirective)stream[0];
                GDTokens.IdentifierToken id = (GDTokens.IdentifierToken)stream[1];
                GDTokens.OperatorToken op = (GDTokens.OperatorToken)stream[2];
                string name = id.Name;
                string value = null;
                if (op.Type == GDTokens.OperatorType.Equals)
                {
                    var tok = LookAhead(0);
                    if (tok.TokenType == GDTokenType.Identifier)
                        value = ((GDTokens.IdentifierToken)(tok)).Name;
                    else if (tok.TokenType == GDTokenType.StringLiteral)
                        value = ((GDTokens.StringLiteralToken)(tok)).GetCleanValue();
                    else
                    {
                        Expect("string or identifier", tok.Position);
                        return null;
                    }
                    PopAhead();
                    tok = PopAhead();
                    if (!ExpectOperator(tok, GDTokens.OperatorType.SemiColon, true))
                        return null;

                }
                else if (!ExpectOperator(op, GDTokens.OperatorType.SemiColon, true))
                    return null;
                return new PreprocessorDefineSymbolDirective(name, value, preprocessor.Column, preprocessor.Line, preprocessor.Position);
            }
            else if (stream.Count == 3 && (stream[1]).TokenType == GDTokenType.Identifier)
                ExpectOperator(stream[2], GDTokens.OperatorType.Comma | GDTokens.OperatorType.SemiColon, true);
            else if (stream.Count > 1)
                Expect("identifier", stream[1].Position);
            else if (stream.Count == 1)
                Expect("identifier", stream[0].Position + stream[0].Length);

            return null;
        }

        private IPreprocessorStringTerminalDirective ParseStringTerminalDirective()
        {
            GDTokens.PreprocessorDirective pp = LookAhead(0) as GDTokens.PreprocessorDirective;
            
            if (pp == null)
            {
                Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                return null;
            }

            IGDToken one = LookAhead(1);
            if (one == null)
            {
                switch (pp.Type)
                {
                    case GDTokens.PreprocessorType.IncludeDirective:
                    case GDTokens.PreprocessorType.LexerNameDirective:
                    case GDTokens.PreprocessorType.ParserNameDirective:
                    case GDTokens.PreprocessorType.GrammarNameDirective:
                    case GDTokens.PreprocessorType.AssemblyNameDirective:
                    case GDTokens.PreprocessorType.RootDirective:
                    case GDTokens.PreprocessorType.RulePrefixDirective:
                    case GDTokens.PreprocessorType.RuleSuffixDirective:
                    case GDTokens.PreprocessorType.TokenPrefixDirective:
                    case GDTokens.PreprocessorType.TokenSuffixDirective:
                    case GDTokens.PreprocessorType.NamespaceDirective:
                        break;
                    default:
                        this.PopAhead();
                        Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                        return null;
                }
                currentTarget.SyntaxErrors.SyntaxError(GrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.CurrentError.Location.Line, this.CurrentTokenizer.CurrentError.Location.Column, GDParserErrors.Expected, "string"));
                this.PopAhead();
                return null;
            }
            else if (this.LookAhead(2) == null)
            {
                switch (pp.Type)
                {
                    case GDTokens.PreprocessorType.IncludeDirective:
                    case GDTokens.PreprocessorType.LexerNameDirective:
                    case GDTokens.PreprocessorType.ParserNameDirective:
                    case GDTokens.PreprocessorType.GrammarNameDirective:
                    case GDTokens.PreprocessorType.AssemblyNameDirective:
                    case GDTokens.PreprocessorType.RootDirective:
                    case GDTokens.PreprocessorType.RulePrefixDirective:
                    case GDTokens.PreprocessorType.RuleSuffixDirective:
                    case GDTokens.PreprocessorType.TokenPrefixDirective:
                    case GDTokens.PreprocessorType.TokenSuffixDirective:
                    case GDTokens.PreprocessorType.NamespaceDirective:
                        if (one.TokenType != GDTokenType.StringLiteral)
                        {
                            Expect("string", one.Position);
                            this.PopAhead();
                            this.PopAhead();
                            return null;
                        }
                        break;
                    default:
                        Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                        this.PopAhead();
                        this.PopAhead();
                        return null;
                }
                currentTarget.SyntaxErrors.SyntaxError(GrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.CurrentError.Location.Line, this.CurrentTokenizer.CurrentError.Location.Column, GDParserErrors.Expected, ";"));
                this.PopAhead();
                this.PopAhead();
                return null;
            }
            var its = GetAhead(3);

            if (its.Count == 3 && SeriesMatch(its, GDTokenType.PreprocessorDirective, GDTokenType.StringLiteral, GDTokenType.Operator))
            {
                StringTerminalKind kind = StringTerminalKind.Unknown;
                GDTokens.StringLiteralToken str = (GDTokens.StringLiteralToken)its[1];
                GDTokens.OperatorToken op = (GDTokens.OperatorToken)its[2];
                if (op.Type == GDTokens.OperatorType.SemiColon)
                {
                    string literal = str.GetCleanValue();
                    switch (pp.Type)
                    {
                        case GDTokens.PreprocessorType.IncludeDirective:
                            kind = StringTerminalKind.Include;
                            break;
                        case GDTokens.PreprocessorType.RootDirective:
                            kind = StringTerminalKind.Root;
                            break;
                        case GDTokens.PreprocessorType.AssemblyNameDirective:
                            kind = StringTerminalKind.AssemblyName;
                            break;
                        case GDTokens.PreprocessorType.LexerNameDirective:
                            kind = StringTerminalKind.LexerName;
                            break;
                        case GDTokens.PreprocessorType.GrammarNameDirective:
                            kind = StringTerminalKind.GrammarName;
                            break;
                        case GDTokens.PreprocessorType.ParserNameDirective:
                            kind = StringTerminalKind.ParserName;
                            break;
                        case GDTokens.PreprocessorType.TokenPrefixDirective:
                            kind = StringTerminalKind.TokenPrefix;
                            break;
                        case GDTokens.PreprocessorType.TokenSuffixDirective:
                            kind = StringTerminalKind.TokenSuffix;
                            break;
                        case GDTokens.PreprocessorType.RulePrefixDirective:
                            kind = StringTerminalKind.RulePrefix;
                            break;
                        case GDTokens.PreprocessorType.RuleSuffixDirective:
                            kind = StringTerminalKind.RuleSuffix;
                            break;
                        case GDTokens.PreprocessorType.NamespaceDirective:
                             kind = StringTerminalKind.Namespace;
                            break;
                        default:
                            Expect("#include, #AssemblyName, #LexerName, #ParserName, #GrammarName, #TokenPrefix, #TokenSuffix, #RulePrefix, or #RuleSuffix directive", pp.Position);
                            break;
                    }
                    return new PreprocessorStringTerminalDirective(kind, literal, pp.Column, pp.Line, pp.Position);
                }
            }
            else
            {
                switch (pp.Type)
                {
                    case GDTokens.PreprocessorType.IncludeDirective:
                    case GDTokens.PreprocessorType.LexerNameDirective:
                    case GDTokens.PreprocessorType.ParserNameDirective:
                    case GDTokens.PreprocessorType.GrammarNameDirective:
                    case GDTokens.PreprocessorType.AssemblyNameDirective:
                    case GDTokens.PreprocessorType.RootDirective:
                    case GDTokens.PreprocessorType.RulePrefixDirective:
                    case GDTokens.PreprocessorType.RuleSuffixDirective:
                    case GDTokens.PreprocessorType.TokenPrefixDirective:
                    case GDTokens.PreprocessorType.TokenSuffixDirective:
                    case GDTokens.PreprocessorType.NamespaceDirective:
                        if (its.Count > 1)
                        {
                            IGDToken second = its[1];
                            if (second.TokenType == GDTokenType.StringLiteral)
                                ExpectOperator(its[2], GDTokens.OperatorType.SemiColon, true);
                            else
                            {
                                Expect("string", second.Position);
                            }
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
            if (LookAhead(0).TokenType != GDTokenType.PreprocessorDirective &&
                ((GDTokens.PreprocessorDirective)(LookAhead(0))).Type == GDTokens.PreprocessorType.ReturnDirective)
            {
                Expect("#return");
                return null;
            }
            IGDToken igdt = PopAhead();
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
            if (LookAhead(0).TokenType != GDTokenType.PreprocessorDirective &&
                ((GDTokens.PreprocessorDirective)(LookAhead(0))).Type == GDTokens.PreprocessorType.AddRuleDirective)
            {
                Expect("#addrule");
                return null;
            }
            IGDToken igdt = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
            {
                GDTokens.IdentifierToken target = its[0] as GDTokens.IdentifierToken;
                GDTokens.OperatorToken op = its[1] as GDTokens.OperatorToken;
                if (!ExpectOperator(op, GDTokens.OperatorType.Comma, true))
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
            if (LookAhead(0).TokenType != GDTokenType.PreprocessorDirective &&
                ((GDTokens.PreprocessorDirective)(LookAhead(0))).Type == GDTokens.PreprocessorType.DefineDirective)
            {
                Expect("#define");
                return null;
            }
            IGDToken igdt = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
            {
                GDTokens.IdentifierToken target = its[0] as GDTokens.IdentifierToken;
                GDTokens.OperatorToken op = its[1] as GDTokens.OperatorToken;
                if (!ExpectOperator(op, GDTokens.OperatorType.Equals, true))
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
            if (LookAhead(0).TokenType != GDTokenType.PreprocessorDirective &&
                ((GDTokens.PreprocessorDirective)(LookAhead(0))).Type == GDTokens.PreprocessorType.ThrowDirective)
            {
                Expect("#throw");
                return null;
            }
            IGDToken preproc = PopAhead();
            var its = GetAhead(2);
            if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
            {
                GDTokens.IdentifierToken id = (GDTokens.IdentifierToken)its[0];
                if (ExpectOperator(its[1], GDTokens.OperatorType.SemiColon, false))
                    result = new PreprocessorThrowDirective(this.currentTarget.Result, id.Name, new IGDToken[0], preproc.Column, preproc.Line, preproc.Position);
                else if (ExpectOperator(its[1], GDTokens.OperatorType.Comma, true))
                {
                    List<IGDToken> args = new List<IGDToken>();
                    while (true)
                    {
                        its = GetAhead(2);
                        IGDToken arg = its[0];
                        if (its.Count != 2)
                            break;
                        if (arg.TokenType == GDTokenType.Identifier || arg.TokenType == GDTokenType.StringLiteral || arg.TokenType == GDTokenType.CharacterLiteral)
                            args.Add(arg);
                        else
                        {
                            Expect("string, char or identifier");
                            break;
                        }
                        if (ExpectOperator(its[1], GDTokens.OperatorType.SemiColon, false))
                        {
                            result = new PreprocessorThrowDirective(this.currentTarget.Result, id.Name, args.ToArray(), preproc.Column, preproc.Line, preproc.Position);
                            break;
                        }
                        else if (ExpectOperator(its[1], GDTokens.OperatorType.Comma, true))
                            continue;
                        else
                            break;
                    }
                }
            }
            else if (its.Count > 0 && its[0].TokenType == GDTokenType.Identifier)
            {
                ExpectOperator(its[0], GDTokens.OperatorType.SemiColon, true);
            }
            else
                Expect("identifier");
            
            this.SetMultiLineMode(mlm);
            return result;
        }

        private IPreprocessorIfDirective ParseIfDirective(IPreprocessorCLogicalOrConditionExp condition, GDTokens.PreprocessorDirective preprocessor, PreprocessorContainer container)
        {
            IPreprocessorIfDirective result = null;
            bool secondHalf = true;
            switch (preprocessor.Type)
            {
                case GDTokens.PreprocessorType.IfDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.If, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case GDTokens.PreprocessorType.IfDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.IfDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case GDTokens.PreprocessorType.IfNotDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.IfNotDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    break;
                case GDTokens.PreprocessorType.ElseIfDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.ElseIf, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                case GDTokens.PreprocessorType.ElseIfDefinedDirective:
                    ParseIfDirectiveBody(result = new PreprocessorIfDirective(EntryPreprocessorType.ElseIfDefined, condition, this.CurrentTokenizer.FileName, preprocessor.Column, preprocessor.Line, preprocessor.Position), container);
                    secondHalf = false;
                    break;
                case GDTokens.PreprocessorType.ElseDirective:
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
            IGDToken igdt = null;
            this.SetMultiLineMode(true);
            while (
                (igdt = LookAhead(0)).TokenType == GDTokenType.PreprocessorDirective ||
                igdt.TokenType == GDTokenType.Comment)
            {
                GDTokens.PreprocessorDirective ppd = igdt as GDTokens.PreprocessorDirective;
                IPreprocessorCLogicalOrConditionExp subCondition = null;
                if (ppd != null)
                {
                    PopAhead();
                    switch (ppd.Type)
                    {
                        case GDTokens.PreprocessorType.ElseIfDirective:
                        case GDTokens.PreprocessorType.ElseIfDefinedDirective:
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
                        case GDTokens.PreprocessorType.ElseDirective:
                            final = true;
                            current = ParseIfDirective(null, ppd, container);
                            break;
                        case GDTokens.PreprocessorType.EndIfDirective:
                            //There's no break² operation.
                            goto endWhile;
                        default:
                            break;
                    }
                }
                else if (igdt.TokenType == GDTokenType.Comment)
                {
                    var commentToken = igdt as GDTokens.CommentToken;
                    if (captureRegions && commentToken.MultiLine)
                    {
                        var gdResult = currentTarget.Result as GDFile;
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
            Action<IEntry> preprocessorInserter = p => ppidb.Add(new PreprocessorEntryContainer(p, p.Column, p.Line, p.Position));
            long bodyStart = ipid.Position;
            while (true)
            {
                LookPastAndSkip();
                this.SetMultiLineMode(true);

                var la0 = this.LookAhead(0);
                GDTokens.PreprocessorDirective ppd = la0 as GDTokens.PreprocessorDirective;
                if (ppd != null)
                {
                    switch (ppd.Type)
                    {
                        case GDTokens.PreprocessorType.IfDirective:
                        case GDTokens.PreprocessorType.IfDefinedDirective:
                        case GDTokens.PreprocessorType.IfNotDefinedDirective:
                        case GDTokens.PreprocessorType.DefineDirective:
                        case GDTokens.PreprocessorType.ThrowDirective:
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                                break;
                            }
                        case GDTokens.PreprocessorType.AddRuleDirective:
                        case GDTokens.PreprocessorType.ReturnDirective:
                            if (container == PreprocessorContainer.Template)
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                            }
                            else
                                LogError(GDParserErrors.Unexpected, string.Format("{0} directive.", ppd.Type == GDTokens.PreprocessorType.AddRuleDirective ? "Add rule" : "Return"), ppd.Position);
                            break;
                        case GDTokens.PreprocessorType.ElseIfDirective:
                        case GDTokens.PreprocessorType.ElseIfDefinedDirective:
                        case GDTokens.PreprocessorType.ElseDirective:
                        case GDTokens.PreprocessorType.EndIfDirective:
                            goto endBody;
                        case GDTokens.PreprocessorType.IncludeDirective:
                        case GDTokens.PreprocessorType.RootDirective:
                        case GDTokens.PreprocessorType.AssemblyNameDirective:
                        case GDTokens.PreprocessorType.LexerNameDirective:
                        case GDTokens.PreprocessorType.GrammarNameDirective:
                        case GDTokens.PreprocessorType.ParserNameDirective:
                        case GDTokens.PreprocessorType.TokenPrefixDirective:
                        case GDTokens.PreprocessorType.TokenSuffixDirective:
                        case GDTokens.PreprocessorType.RulePrefixDirective:
                        case GDTokens.PreprocessorType.RuleSuffixDirective:
                            if (container == PreprocessorContainer.File)
                            {
                                IPreprocessorDirective ipd = ParsePreprocessor(container);
                                ppidb.Add(ipd);
                            }
                            else
                                LogError(GDParserErrors.Unexpected, "StringTerminal directive.", ppd.Position);
                            break;
                        default:
                            break;
                    }
                }
                else if (container == PreprocessorContainer.File && la0.TokenType == GDTokenType.Identifier)
                {
                    /* *
                     * All declarations must begin at the first column 
                     * of the line.
                     * */
                    if (this.CurrentTokenizer.GetColumnIndex(this.StreamPosition) != 1)
                    {
                        GetAhead(1);
                        LogError(GDParserErrors.ExpectedEndOfLine);
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
                    if (SeriesMatch(its, GDTokenType.Identifier, GDTokenType.Operator))
                    {
                        List<string> lowerPrecedences = new List<string>();
                        GDTokens.IdentifierToken id = ((GDTokens.IdentifierToken)(its[0]));
                        GDTokens.OperatorToken ot = ((GDTokens.OperatorToken)(its[1]));
                        bool elementsAreChildren = false;
                        bool forcedRecognizer = false;
                    checkOp:
                        switch (ot.Type)
                        {
                            //ID$
                            case GDTokens.OperatorType.ForcedStringForm:
                                forcedRecognizer = true;
                                if (LookAhead(0).TokenType == GDTokenType.Operator)
                                {
                                    ot = (GDTokens.OperatorToken)PopAhead();
                                    goto checkOp;
                                }
                                else
                                {
                                    ExpectOperator(LookAhead(0), GDTokens.OperatorType.ColonEquals | GDTokens.OperatorType.GreaterThan, true);
                                    PopAhead();
                                } 
                                break;
                            //ID ::=
                            case GDTokens.OperatorType.ColonColonEquals:
                                if ((ot = LookAhead(0) as GDTokens.OperatorToken) != null &&
                                    ot.Type == GDTokens.OperatorType.GreaterThan)
                                {
                                    elementsAreChildren = true;
                                    PopAhead();
                                }
                                SetMultiLineMode(true);
                                ParseProductionRule(id, EntryScanMode.Inherited, id.Position, elementsAreChildren, preprocessorInserter, container);
                                break;
                            //ID :=
                            case GDTokens.OperatorType.ColonEquals:
                                {
                                    SetMultiLineMode(true);
                                    bool unhinged = false;
                                    if (TokenizerLookAhead(0) == '*')
                                    {
                                        //ID :=*
                                        /* *
                                         * Token is unbounded by grammar
                                         * and can appear anywhere.
                                         * */
                                        this.PopAhead();
                                        unhinged = true;
                                    }
                                    ParseToken(id, EntryScanMode.Inherited, id.Position, unhinged, lowerPrecedences, forcedRecognizer, ppidb);
                                }
                                break;
                            //ID =
                            case GDTokens.OperatorType.Equals:
                                ParseError(id.Name, id.Position);
                                break;
                            //ID<
                            case GDTokens.OperatorType.LessThan:
                                SetMultiLineMode(true);
                                IList<IProductionRuleTemplatePart> parts = this.ParseProductionRuleTemplateParts(ot);
                                if (parts == null)
                                    continue;
                                else
                                {
                                    IGDToken g = null;
                                    GDTokens.OperatorToken gdt = (g = PopAhead()) as GDTokens.OperatorToken;
                                    if (gdt == null)
                                        Expect("+, -, or ::=");
                                    if (gdt.Type == GDTokens.OperatorType.Minus)
                                    {
                                        gdt = PopAhead() as GDTokens.OperatorToken;
                                        esm = EntryScanMode.SingleLine;
                                    }
                                    else if (gdt.Type == GDTokens.OperatorType.Plus)
                                    {
                                        gdt = PopAhead() as GDTokens.OperatorToken;
                                        esm = EntryScanMode.Multiline;
                                    }
                                    if (gdt == null || !ExpectOperator(gdt, GDTokens.OperatorType.ColonColonEquals, true))
                                    {
                                        Expect("::=");
                                        break;
                                    }
                                    this.ParseProductionRuleTemplate(id, parts, esm, id.Position, ppidb);
                                }
                                break;
                            //ID>
                            case GDTokens.OperatorType.GreaterThan:
                                //Case of a token defining precedence.
                                while (LookAhead(0).TokenType == GDTokenType.Identifier)
                                {
                                    var lowerPrecedenceToken = ((GDTokens.IdentifierToken)LookAhead(0)).Name;
                                    lowerPrecedences.Add(lowerPrecedenceToken);
                                    PopAhead();
                                    GDTokens.OperatorToken cOp = null;
                                    //','  * More precedences exist.
                                    if ((cOp = (LookAhead(0) as GDTokens.OperatorToken)) != null &&
                                        cOp.Type == GDTokens.OperatorType.Comma)
                                        PopAhead();
                                    //":=" * Final precedence reached.
                                    else if (cOp != null &&
                                        cOp.Type == GDTokens.OperatorType.ColonEquals)
                                    {
                                        PopAhead();
                                        goto case GDTokens.OperatorType.ColonEquals;
                                    }
                                    else
                                    {
                                        //Syntax error.
                                        ExpectOperator(LookAhead(0), GDTokens.OperatorType.ColonEquals, true);
                                        break;
                                    }
                                }
                                break;
                            //ID-
                            case GDTokens.OperatorType.Minus:
                            //ID+
                            case GDTokens.OperatorType.Plus:
                                esm = ot.Type == GDTokens.OperatorType.Minus ? EntryScanMode.SingleLine : EntryScanMode.Multiline;
                                IGDToken ahead = PopAhead();
                                if (ahead.TokenType == GDTokenType.Operator)
                                {
                                    ot = ((GDTokens.OperatorToken)(ahead));
                                    switch (ot.Type)
                                    {
                                        case GDTokens.OperatorType.ColonColonEquals:
                                            SetMultiLineMode(true);
                                            if ((ot = LookAhead(0) as GDTokens.OperatorToken) != null &&
                                                ot.Type == GDTokens.OperatorType.GreaterThan)
                                            {
                                                elementsAreChildren = true;
                                                PopAhead();
                                            }
                                            ParseProductionRule(id, esm, id.Position, elementsAreChildren, preprocessorInserter, container);
                                            break;
                                        case GDTokens.OperatorType.ColonEquals:
                                            {
                                                SetMultiLineMode(true);
                                                bool unhinged = false;
                                                if (TokenizerLookAhead(0) == '*')
                                                {
                                                    //ID[+-] :=*
                                                    /* *
                                                     * Token is unbounded by grammar
                                                     * and can appear anywhere.
                                                     * */
                                                    this.PopAhead();
                                                    unhinged = true;
                                                }
                                                ParseToken(id, esm, id.Position, unhinged, lowerPrecedences, forcedRecognizer, ppidb);
                                            }
                                            break;
                                        case GDTokens.OperatorType.Equals:
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
                else if (la0.TokenType == GDTokenType.Comment)
                {
                    var commentToken = la0 as GDTokens.CommentToken;
                    if (captureRegions && commentToken.MultiLine)
                    {
                        var gdResult = currentTarget.Result as GDFile;
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
                var gdResult = this.currentTarget.Result as GDFile;
                long bodyEnd = this.StreamPosition;
                int startLine = this.CurrentTokenizer.GetLineIndex(bodyStart) + 1;
                int endLine = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (endLine > startLine + 1)
                {
                    var nlLen = Environment.NewLine.Length;
                    bodyStart = this.CurrentTokenizer.GetPositionFromLine(startLine);
                    bodyEnd = this.CurrentTokenizer.GetPositionFromLine(endLine) - nlLen;

                    gdResult.AddIfRegion(ipid, bodyStart, bodyEnd);
                }
            }
            ;
        }

        internal virtual void SetMultiLineMode(bool value)
        {
            ((Lexer)(this.CurrentTokenizer)).MultiLineMode = value;
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
            PushAhead(new GDTokens.ReferenceToken(current, current.Column, current.Line, current.Position));
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
                    LogError(GDParserErrors.Unexpected, "SourceError");
                else
                    PushAhead(new GDTokens.ReferenceToken(result, result.Column, result.Line, result.Position));
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
                    this.PopAhead();
                else if (rResult == null)
                {
                    Expect("||", CurrentTokenizer.Position);
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
                    this.PopAhead();
                else if (rResult == null)
                {
                    Expect("&&", CurrentTokenizer.Position);
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
                        return false;
                    }
                    this.PopAhead();
                }
                else if (rResult == null)
                {
                    Expect("== or !=", CurrentTokenizer.Position);
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
                var leftParenthesis = this.PopAhead() as GDTokens.OperatorToken;
                IPreprocessorCLogicalOrConditionExp orExp = null;
                if (ParsePreprocessorCLogicalOrConditionExp(ref orExp))
                {
                    if (LookPastAndSkip() == ')')
                        //Skip it, not a token but a literal for CPrimary.
                        this.PopAhead();
                    else
                    {
                        Expect(")", CurrentTokenizer.Position);
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
                IGDToken ahead = this.LookAhead(0);
                if (ahead.TokenType == GDTokenType.Identifier ||
                    ahead.TokenType == GDTokenType.StringLiteral ||
                    ahead.TokenType == GDTokenType.CharacterLiteral || 
                    ahead.TokenType == GDTokenType.NumberLiteral)
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
                if (ahead.TokenType == GDTokenType.Identifier)
                    cPrimary = new PreprocessorCPrimary(((GDTokens.IdentifierToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == GDTokenType.StringLiteral)
                    cPrimary = new PreprocessorCPrimary(((GDTokens.StringLiteralToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == GDTokenType.CharacterLiteral)
                    cPrimary = new PreprocessorCPrimary(((GDTokens.CharLiteralToken)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                else if (ahead.TokenType == GDTokenType.NumberLiteral)
                    cPrimary = new PreprocessorCPrimary(((GDTokens.NumberLiteral)(ahead)), ahead.Column, ahead.Line, ahead.Position);
                return true;
            }
        }

        private T GetReference<T>()
            where T :
                class
        {
            if (LookAhead(0).TokenType == GDTokenType.ReferenceToken && ((GDTokens.ReferenceToken)(LookAhead(0))).Reference is T)
            {
                return (T)((GDTokens.ReferenceToken)(PopAhead(false))).Reference;
            }
            return null;
        }

        private bool ExpectReference<T>()
        {
            if (LookAhead(0).TokenType == GDTokenType.ReferenceToken && ((GDTokens.ReferenceToken)(LookAhead(0))).Reference is T)
            {
                return true;
            }
            PopAhead(false);
            return false;
        }

        private IProductionRuleItem ParseProductionRuleReferenceItem(PreprocessorContainer container)
        {
            ISoftReferenceProductionRuleItem isrpri = null;
            GDTokens.IdentifierToken id = this.LookAhead(0) as GDTokens.IdentifierToken;
            if (id != null)
            {
                GDTokens.IdentifierToken id2 = null;
                PopAhead();
                if (LookAhead(0) == null && CurrentTokenizer.CurrentError != null)
                {
                    if (!(currentTarget.SyntaxErrors.Contains(CurrentTokenizer.CurrentError)))
                        currentTarget.SyntaxErrors.SyntaxError(CurrentTokenizer.CurrentError);
                    return null;
                }
                if (LookAhead(0).TokenType == GDTokenType.Operator &&
                    ExpectOperator(LookAhead(0), GDTokens.OperatorType.Period, false))
                {
                    id2 = LookAhead(1) as GDTokens.IdentifierToken;
                    if (id2 != null)
                        GetAhead(2);
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
                        isrpri = ParseTemplateReference(id, container);
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
            return isrpri;
        }

        protected virtual void DefineRuleIdentifier(GDTokens.IdentifierToken identifier, IProductionRuleEntry ruleEntry)
        {
        }

        protected virtual void DefineRuleTemplateIdentifier(GDTokens.IdentifierToken ruleTemplateIdentifier, IProductionRuleTemplateEntry ruleTemplateEntry)
        {
        }

        protected virtual void DefineCommandIdentifier(GDTokens.IdentifierToken commandIdentifier)
        {
        }
        protected virtual void DefineKeywordIdentifier(GDTokens.IdentifierToken keywordIdentifier)
        {
        }

        protected virtual void DefineTokenIdentifier(GDTokens.IdentifierToken identifier, ITokenEntry tokenEntry)
        {
            
        }

        protected virtual void DefineRuleSoftReference(GDTokens.IdentifierToken primary, GDTokens.IdentifierToken secondary = null)
        {
        }

        protected virtual void DefineRuleTemplateParameterIdentifier(GDTokens.IdentifierToken id, IProductionRuleTemplatePart currentPart)
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
                if (LookAhead(0).TokenType == GDTokenType.Operator)
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

        private ISoftTemplateReferenceProductionRuleItem ParseTemplateReference(GDTokens.IdentifierToken id, PreprocessorContainer container)
        {
            if (ExpectOperator(LookAhead(0), GDTokens.OperatorType.LessThan, true))
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
                    if (ExpectOperator(LookAhead(0), GDTokens.OperatorType.Comma, false))
                    {
                        PopAhead();
                        continue;
                    }
                    else if (ExpectOperator(LookAhead(0), GDTokens.OperatorType.GreaterThan, true))
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
            GDTokens.StringLiteralToken slt = LookAhead(0) as GDTokens.StringLiteralToken;
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
            GDTokens.CharLiteralToken slt = LookAhead(0) as GDTokens.CharLiteralToken;
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

        private void ParseProductionRule(GDTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool elementsAreChildren)
        {
            ParseProductionRule(identifier, scanMode, position, elementsAreChildren, p => this.currentTarget.Result.Add(p));
        }
        private void ParseProductionRule(GDTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool elementsAreChildren, Action<IEntry> adder, PreprocessorContainer container = PreprocessorContainer.File)
        {
            long bodyStart = this.StreamPosition;
            IProductionRuleEntry ipre = new ProductionRuleEntry(identifier.Name, scanMode, CurrentTokenizer.FileName, CurrentTokenizer.GetColumnIndex(position), CurrentTokenizer.GetLineIndex(position), position);
            ipre.ElementsAreChildren = elementsAreChildren;
            ParseProductionRule(((ProductionRuleEntry)ipre).BaseCollection, container);
            long bodyEnd = this.StreamPosition;
            adder(ipre);
            DefineRuleIdentifier(identifier, ipre);
            if (captureRegions)
            {
                int lineStart = this.CurrentTokenizer.GetLineIndex(bodyStart);
                int lineEnd = this.CurrentTokenizer.GetLineIndex(bodyEnd);
                if (lineEnd > lineStart + 1)
                {
                    var gdResult = currentTarget.Result as GDFile;
                    gdResult.AddRuleRegion(ipre, bodyStart, bodyEnd);
                }
            }
        }

        private void ParseToken(GDTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool unhinged, List<string> lowerPrecedences, bool forcedRecognizer)
        {
            ParseToken(identifier, scanMode, position, unhinged, lowerPrecedences, forcedRecognizer, null);
        }

        private void ParseToken(GDTokens.IdentifierToken identifier, EntryScanMode scanMode, long position, bool unhinged, List<string> lowerPrecedences, bool forcedRecognizer, PreprocessorIfDirective.DirectiveBody target)
        {
            long bodyStart = this.StreamPosition;
            ITokenEntry ite = new TokenEntry(identifier.Name, ParseTokenExpressionSeries(), scanMode, this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetColumnIndex(position), this.CurrentTokenizer.GetLineIndex(position), position, unhinged, lowerPrecedences, forcedRecognizer);
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
                    var gdResult = currentTarget.Result as GDFile;
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
                    ExpectOperator(PopAhead(), GDTokens.OperatorType.SemiColon, true);
                    break;
                }
                else if (c == '|')
                {
                    if (!(ExpectOperator(PopAhead(), GDTokens.OperatorType.Pipe, true)))
                        break;
                    ParseTokenBody(expressions);
                    continue;
                }
                else if (c == '/' && (TokenizerLookAhead(1) == '*' || TokenizerLookAhead(1) == '/'))
                {
                    var igdt = LookAhead(0);
                    if (igdt.TokenType == GDTokenType.Comment)
                    {
                        var commentToken = igdt as GDTokens.CommentToken;
                        if (captureRegions && commentToken.MultiLine)
                        {
                            var gdResult = currentTarget.Result as GDFile;
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
                {
                    if (commandDepths.Contains(parameterDepth))
                        break;
                    else
                        goto Expect;
                }
                else if (Lexer.IsIdentifierChar(c) || c == '(' || c == '\'' || c == '"' || c == '@' || c == '[')
                {
                    ParseTokenBody(expressions);
                }
                else
                {
                    goto Expect;
                }
                continue;
            Expect:
                Expect("string, char, identifier, preprocessor, or ';'");
                break;
            }
        }

        private ITokenGroupItem ParseTokenGroup()
        {
            GDTokens.OperatorToken ot = LookAhead(0) as GDTokens.OperatorToken;
            
            if (ot != null && ExpectOperator(PopAhead(), GDTokens.OperatorType.LeftParenthesis, true))
            {
                List<ITokenExpression> items = new List<ITokenExpression>();
                parameterDepth++;
                ParseTokenExpressions(items);
                parameterDepth--;
                ITokenGroupItem result = new TokenGroupItem(this.CurrentTokenizer.FileName, items.ToArray(), ot.Column, ot.Line, ot.Position);
                var endTok = PopAhead();
                if (captureRegions)
                {
                    var gdResult = this.currentTarget.Result as GDFile;
                    int lineStart = ot.Line;
                    int lineEnd = endTok.Line;
                    if (lineEnd > lineStart + 1)
                        gdResult.AddTokenGroupRegion(result, ot.Position, endTok.Position);
                }
                ExpectOperator(endTok, GDTokens.OperatorType.RightParenthesis, true);
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
                IGDToken igdt = LookAhead(0);
                switch (igdt.TokenType)
                {
                    case GDTokenType.Comment:
                        var commentToken = igdt as GDTokens.CommentToken;
                        if (captureRegions && commentToken.MultiLine)
                        {
                            var gdResult = currentTarget.Result as GDFile;
                            gdResult.AddCommentRegion(commentToken);
                        }

                        PopAhead();
                        continue;
                    case GDTokenType.CharacterLiteral:
                        item = ParseTokenCharLiteral();
                        break;
                    case GDTokenType.StringLiteral:
                        item = ParseTokenStringLiteral();
                        break;
                    case GDTokenType.Identifier:
                        item = ParseTokenReferenceItem();
                        break;
                    case GDTokenType.CharacterRange:
                        item = ParseTokenCharacterRange();
                        break;
                    case GDTokenType.NumberLiteral:
                        LogError(GDParserErrors.Unexpected, "number", igdt.Position);
                        goto yield;
                    case GDTokenType.PreprocessorDirective:
                        LogError(GDParserErrors.Unexpected, "preprocessor", igdt.Position);
                        goto yield;
                    case GDTokenType.Operator:
                        switch (((GDTokens.OperatorToken)(igdt)).Type)
                        {
                            case GDTokens.OperatorType.Pipe:
                                ClearAhead();
                                goto yield;
                            case GDTokens.OperatorType.LeftParenthesis:
                                ClearAhead();
                                item = (ITokenGroupItem)ParseTokenGroup();
                                break;
                            case GDTokens.OperatorType.RightParenthesis:
                                if (parameterDepth <= 0)
                                    LogError(GDParserErrors.Expected, ";", igdt.Position);
                                goto yield;
                            case GDTokens.OperatorType.GreaterThan:
                            case GDTokens.OperatorType.Comma:
                                if (commandDepths.Contains(parameterDepth))
                                {
                                    ClearAhead();
                                    goto yield;
                                }
                                LogError(GDParserErrors.Expected, "identifier, '(', or ';'");
                                goto yield;
                            case GDTokens.OperatorType.SemiColon:
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
                        PopAhead();
                    LogError(GDParserErrors.Expected, "identifier, '(' or ';'");
                    break;
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
            if (TokenizerLookAhead(0) == '?')
            {
                item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrOne;
                this.PopAhead();
            }
            else if (TokenizerLookAhead(0) == '+')
            {
                item.RepeatOptions = ScannableEntryItemRepeatInfo.OneOrMore;
                this.PopAhead();
            }
            else if (TokenizerLookAhead(0) == '*')
            {
                if (TokenizerLookAhead(1) == '*' &&
                    item is LiteralStringTokenItem)
                {
                    this.PopAhead();
                    ((LiteralStringTokenItem)(item)).SiblingAmbiguity = true;
                }
                else
                {
                    item.RepeatOptions = ScannableEntryItemRepeatInfo.ZeroOrMore;
                    this.PopAhead();
                }
            }
            else if (TokenizerLookAhead(0) == '{')
            {
                this.PopAhead();
                var numberOp = this.GetAhead(2);
                //Either min unbounded or error.
                if (!(SeriesMatch(numberOp, GDTokenType.NumberLiteral, GDTokenType.Operator) ||
                      SeriesMatch(numberOp, GDTokenType.Operator, GDTokenType.NumberLiteral)))
                {
                    //SourceError of some kind.
                    if (numberOp[0].TokenType == GDTokenType.NumberLiteral)
                        ExpectOperator(numberOp[1], GDTokens.OperatorType.Minus | GDTokens.OperatorType.Comma, true);
                    else
                        Expect("Number or ','", numberOp[0].Position);
                }
                if (numberOp[0].TokenType == GDTokenType.Operator)
                {
                    GDTokens.OperatorToken op = numberOp[0] as GDTokens.OperatorToken;
                    if (op.Type == GDTokens.OperatorType.Comma)
                        item.RepeatOptions = new ScannableEntryItemRepeatInfo(null, ((GDTokens.NumberLiteral)(numberOp[1])).GetCleanValue());
                    else
                        Expect("Number or ','", numberOp[0].Position);
                    ExpectOperator(this.PopAhead(), GDTokens.OperatorType.RightCurlyBrace, true);
                }
                else
                {
                    GDTokens.NumberLiteral num = numberOp[0] as GDTokens.NumberLiteral;
                    GDTokens.OperatorToken op = numberOp[1] as GDTokens.OperatorToken;
                    if (op.Type == GDTokens.OperatorType.Comma)
                    {
                        var max = this.PopAhead();
                        if (max.TokenType == GDTokenType.NumberLiteral)
                        {
                            var maxNumber = max as GDTokens.NumberLiteral;
                            var maxTail = this.PopAhead();
                            if (maxTail.TokenType == GDTokenType.Operator &&
                                ((GDTokens.OperatorToken)(maxTail)).Type == GDTokens.OperatorType.RightCurlyBrace)
                                item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), maxNumber.GetCleanValue());
                            else
                                ExpectOperator(maxTail, GDTokens.OperatorType.RightCurlyBrace, true);
                        }
                        else if (max.TokenType == GDTokenType.Operator)
                        {
                            var maxTail = max as GDTokens.OperatorToken;
                            if (maxTail.Type != GDTokens.OperatorType.RightCurlyBrace)
                                Expect("Number or }", max.Position);
                            else
                                item.RepeatOptions = new ScannableEntryItemRepeatInfo(num.GetCleanValue(), null);
                        }
                        else
                            Expect("Number or }", max.Position);
                    }
                    else if (op.Type == GDTokens.OperatorType.RightCurlyBrace)
                    {
                        int cl = num.GetCleanValue();
                        item.RepeatOptions = new ScannableEntryItemRepeatInfo(cl, cl);
                    }
                    else
                        ExpectOperator(op, GDTokens.OperatorType.RightCurlyBrace | GDTokens.OperatorType.Comma, true);
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
            GDTokens.CharacterRangeToken crt = LookAhead(0) as GDTokens.CharacterRangeToken;
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
            GDTokens.StringLiteralToken slt = LookAhead(0) as GDTokens.StringLiteralToken;
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
            GDTokens.CharLiteralToken slt = LookAhead(0) as GDTokens.CharLiteralToken;
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
            ISoftReferenceTokenItem isrpri = null;
            GDTokens.IdentifierToken id = this.LookAhead(0) as GDTokens.IdentifierToken;
            if (id != null)
            {
                GDTokens.IdentifierToken id2 = null;
                PopAhead();
                if (IdIsCommand(id.Name))
                    return ParseCommandTokenItem(id);
                if (LookAhead(0).TokenType == GDTokenType.Operator &&
                    ExpectOperator(LookAhead(0), GDTokens.OperatorType.Period, false))
                {
                    id2 = LookAhead(1) as GDTokens.IdentifierToken;
                    if (id2 != null)
                        GetAhead(2);
                    isrpri = new SoftReferenceTokenItem(id.Name, id2.Name, id.Column, id.Line, id.Position)
                    {
                        PrimaryToken = id,
                        SecondaryToken = id2
                    };
                }
                else
                    isrpri = new SoftReferenceTokenItem(id.Name, null, id.Column, id.Line, id.Position)
                    {
                        PrimaryToken = id
                    };
            }
            else
                Expect("identifier");
            return isrpri;
        }

        private ICommandTokenItem ParseCommandTokenItem(GDTokens.IdentifierToken id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (!IdIsCommand(id.Name))
                throw new ArgumentException("id");
            if (!ExpectOperator(PopAhead(), GDTokens.OperatorType.LeftParenthesis, true))
                return null;
            int myDepth = ++parameterDepth;
            commandDepths.Add(myDepth);
            List<ITokenExpressionSeries> commandExpressionSets = new List<ITokenExpressionSeries>();
            try
            {
            nextSet:
                commandExpressionSets.Add(ParseTokenExpressionSeries());
                if (ExpectOperator(LookAhead(0), GDTokens.OperatorType.Comma, false))
                {
                    PopAhead();
                    goto nextSet;
                }
                if (ExpectOperator(LookAhead(0), GDTokens.OperatorType.RightParenthesis, true))
                    PopAhead();
                switch (id.Name.ToLower())
                {
                    case "scan":
                        DefineCommandIdentifier(id);
                        if (commandExpressionSets.Count != 2)
                        {

                            LogError(GDParserErrors.FixedArgumentCountError, "Scan command requires exactly two parameters.", id.Position);
                            if (commandExpressionSets.Count <= 0)
                                return new ScanCommandTokenItem(null, false, id.Column, id.Line, id.Position);
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
                            LogError(GDParserErrors.FixedArgumentCountError, "Subtraction command requires exactly two parameters.", id.Position);
                            if (commandExpressionSets.Count <= 0)
                                return new SubtractionCommandTokenItem(null, null, id.Column, id.Line, id.Position);
                            else if (commandExpressionSets.Count ==1)
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

        private bool IdIsCommand(string idName)
        {
            switch (idName.ToLower())
            {
                case "scan":
                case "subtract":
                    return true;
            }
            return false;
        }

        private bool ExpectBoolean(IGDToken boolIDToken, bool error)
        {
            var boolID = boolIDToken as GDTokens.IdentifierToken;
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

        private bool SeriesMatch(ITokenStream<IGDToken> its, params GDTokenType[] gdtt)
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
            LogError(GDParserErrors.Expected, s);
        }

        private void LogError(GDParserErrors error)
        {
            this.LogError(error, string.Empty);
        }

        private void LogError(GDParserErrors error, string text)
        {
            currentTarget.SyntaxErrors.SyntaxError(GrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetLineIndex(this.StreamPosition), this.CurrentTokenizer.GetColumnIndex(this.StreamPosition), error, text));
        }
        private void Expect(string s, long position)
        {
            LogError(GDParserErrors.Expected, s, position);
        }

        private void LogError(GDParserErrors error, long position)
        {
            this.LogError(error, string.Empty, position);
        }

        private void LogError(GDParserErrors error, string text, long position)
        {
            currentTarget.SyntaxErrors.SyntaxError(GrammarCore.GetSyntaxError(this.CurrentTokenizer.FileName, this.CurrentTokenizer.GetLineIndex(position), this.CurrentTokenizer.GetColumnIndex(position), error, text));
        }

        public T NextAhead<T>()
            where T :
                IGDToken
        {
            return (T)this.LookAhead(this.AheadLength);
        }

        
    }
}
