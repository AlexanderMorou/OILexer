using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AllenCopeland.Abstraction.Numerics;
using AllenCopeland.Abstraction.Globalization;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Languages.CSharp;
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using System.Text;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using System.CodeDom.Compiler;
using AllenCopeland.Abstraction.Slf.Translation;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Translation;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml;

#if x86
using SlotType = System.UInt32;
#elif x64
using SlotType = System.UInt64;
#endif
/* *
 * Old Release Post-build command:
 * "$(ProjectDir)PostBuild.bat" "$(ConfigurationName)" "$(TargetPath)"
 * */
namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides an entrypoint and initial decision logic
    /// for the application.
    /// </summary>
    public static class OilexerProgram
    {
        private static int longestLineLength = 0;
        private const string Syntax = "-s";
        private const string SingleFileOutput = "-single";
        private const string NoSyntax = "-ns";
        private const string NoLogo = "-nl";
        private const string Quiet = "-q";
        private const string Verbose = "-v";
        private const string NoObjectModel = "-nom";

        private const string StreamAnalysis = "-a:";
        private const string CycleDepth = "-cycledepth:";
        private const string ExportKind_TraversalHTML = "t-html";
        private const string ExportKind_OilexerTraversalHTML = "ot-html";
        private const string ExportKind_DLL = "dll";
        private const string ExportKind_EXE = "exe";
        private const string ExportKind_CSharp = "cs";
        private const string StreamAnalysisExtension = "-ae:";
        private const string Export = "-ex:";
        private const string Define = "-d:";
        private const string TranslationOrder = "-tlorder:";
        private const string DefineAlt = "-define:";
        private const string ExportKind_DotFormat = "dot";
        private const string ExportKind_DotFormatFor = "dotsfor:";
        private const string ExportKind_DotFormatSingle = "dots";
        private const string Export_TraversalHTML = Export + ExportKind_TraversalHTML;
        private const string Export_OilexerTraversalHTML = Export + ExportKind_OilexerTraversalHTML;
        private const string Export_DLL = Export + ExportKind_DLL;
        private const string Export_EXE = Export + ExportKind_EXE;
        private const string Export_CSharp = Export + ExportKind_CSharp;
        private const string Export_Dot = Export + ExportKind_DotFormat;
        private const string Export_DotSingle = Export + ExportKind_DotFormatSingle;
        private const string Export_DotSingleFor = Export + ExportKind_DotFormatFor;
        private const string TitleSequence_CharacterSetCache = "Character set cache size";
        private const string TitleSequence_CharacterSetComputations = "Character set computations";
        private const string TitleSequence_VocabularyCache = "Vocabulary cache size";
        private const string TitleSequence_VocabularyComputations = "Vocabulary set computations";
        private const string TitleSequence_NumberOfRules = "Number of rules";
        private const string TitleSequence_NumberOfTokens = "Number of tokens";
        private const string TitleSequence_NumberOfFiles = "Number of files";
        private const string TitleSequence_NumberOfSymbols = "Number of symbols";
        private const string PhaseName_Linking = "Linking";
        private const string PhaseName_ExpandingTemplates = "Expanding templates";
        private const string PhaseName_Deliteralization = "Deliteralization";
        private const string PhaseName_InliningTokens = "Inlining tokens";
        private const string PhaseName_TokenNFAConstruction = "Token NFA Construction";
        private const string PhaseName_TokenDFAConstruction = "Token DFA Construction";
        private const string PhaseName_TokenDFAReduction = "Token DFA Reduction";
        private const string PhaseName_RuleDuplicationCheck = "Rule Duplication Check";
        private const string PhaseName_RuleNFAConstruction = "Rule NFA Construction";
        private const string PhaseName_RuleDFAConstruction = "Rule DFA Construction";
        private const string PhaseName_RuleDFAReduction = "Rule DFA Reduction";
        private const string PhaseName_ConstructProjectionNodes = "Construct Projection Nodes";
        private const string PhaseName_ProjectLookaheadInitial = "Look-Ahead Analysis - LL(1) Determination";
        private const string PhaseName_ProjectLookaheadExpanded = "Look-Ahead Analysis - Unbound Look-ahead";
        //private const string PhaseName_CreatingDynamicStateMachine = "Creating Dynamic Lexer";
        private const string PhaseName_CreateCoreLexerAndEstablishFullAmbiguitySet = "Lexer NFA/DFA and Lexical Ambiguities";
        private const string PhaseName_ObjectModelParser = "Object Model - Building Parser";
        private const string PhaseName_LiftAmbiguitiesAndGenerateLexerCore = "Reducing Ambiguities and Generating Lexer Automata";
        private const string PhaseName_BuildExtensions = "Building Extension Methods";
        private const string PhaseName_ProjectionStateMachines = "Prediction State Machines";
        private const string PhaseName_FollowProjectionsAndMachines = "Follow Predictions and State Machines";
        
        private const string PhaseName_ObjectModelScaffolding = "Object Model Scaffolding";
        private const string PhaseName_TokenCaptureConstruction = "Token Capture Construction";
        private const string PhaseName_TokenEnumConstruction = "Token Enum Construction";
        private const string PhaseName_RuleStructureConstruction = "Rule Structure Construction";
        private const string PhaseName_Parsing = "Parsing";
        private static int MaxDescriptionLength = new int[] { TitleSequence_CharacterSetCache.Length, TitleSequence_CharacterSetComputations.Length, TitleSequence_VocabularyCache.Length, TitleSequence_VocabularyComputations.Length, TitleSequence_NumberOfRules.Length, TitleSequence_NumberOfTokens.Length, TitleSequence_NumberOfFiles.Length, PhaseName_Linking.Length, PhaseName_ExpandingTemplates.Length, PhaseName_Deliteralization.Length, PhaseName_InliningTokens.Length, PhaseName_TokenNFAConstruction.Length, PhaseName_LiftAmbiguitiesAndGenerateLexerCore.Length, PhaseName_TokenDFAConstruction.Length, PhaseName_TokenDFAReduction.Length, PhaseName_RuleNFAConstruction.Length, PhaseName_RuleDFAConstruction.Length, PhaseName_ConstructProjectionNodes.Length, PhaseName_ProjectLookaheadInitial.Length, PhaseName_ProjectLookaheadExpanded.Length, PhaseName_ObjectModelParser.Length, PhaseName_TokenCaptureConstruction.Length, PhaseName_TokenEnumConstruction.Length, PhaseName_RuleStructureConstruction.Length, PhaseName_Parsing.Length, PhaseName_CreateCoreLexerAndEstablishFullAmbiguitySet.Length }.Max();
        private static int LengthOfTimeColumn = 19;
        /// <summary>
        /// Defines the valid options for the <see cref="OilexerProgram"/>.
        /// </summary>
        [Flags]
        public enum ValidOptions
        {
            /// <summary>
            /// No options were specified.
            /// </summary>
            None,
            /// <summary>
            /// Displays the language's sytnax at the end.
            /// </summary>
            ShowSyntax = 0x0001,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to not emit 
            /// the syntax at the end.
            /// </summary>
            DoNotEmitSyntax = 0x0002,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to not
            /// display a logo to the console.
            /// </summary>
            NoLogo = 0x0014,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit as little
            /// as possible to the console.
            /// </summary>
            QuietMode = 0x0018,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to 
            /// display extra information to the console.
            /// </summary>
            VerboseMode = 0x0030,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a series of hypertext mark-up language (HTML)
            /// files associated to parsing the current
            /// grammar.
            /// </summary>
            ExportTraversalHTML = 0x0240,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a series of hypertext mark-up language (HTML)
            /// files associated to parsing the current grammar.
            /// </summary>
            ExportOilexerTraversalHTML = 0x1200,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a dynamic link library (DLL) which can parse
            /// strings of the described language.
            /// </summary>
            ExportDLL = 0x0280,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a simple executable (EXE) which can parse
            /// strings of the described language by accepting
            /// a series of strings which represent file(s) to 
            /// parse.
            /// </summary>
            ExportEXE = 0x0300,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a graph viz dot format (dot) which represents the
            /// state machines for the lexer/parser.
            /// </summary>
            ExportDot = 0x8000,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a graph viz dot format (dot) which represents the
            /// state machines for the lexer/parser items that
            /// are specified.
            /// </summary>
            ExportDotSpecific = 0x20000,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a graph viz dot format (dot) which represents the
            /// state machines for the lexer/parser into a single file.
            /// </summary>
            ExportDotSingle = 0x10000,
            ExportMask = ExportTraversalHTML | ExportDLL | ExportEXE | ExportDot | ExportDotSingle | ExportOilexerTraversalHTML,
            /// <summary>
            /// Instructs the <see cref="OilexerProgram"/> to emit
            /// a series of C&#9839; files.
            /// </summary>
            ExportCSharp = 0x0600,
            /// <summary>
            /// Denotes there is no object model processed.
            /// </summary>
            NoObjectModel = 0x800,
            /// <summary>
            /// Denotes the code generation results should be exported as a single file.
            /// </summary>
            SingleFileOutput = 0x100000,
        }

        public static List<string> StreamAnalysisFiles = new List<string>();
        public static Dictionary<string, string> ConsoleDefines = new Dictionary<string, string>();
        public static ValidOptions options = ValidOptions.DoNotEmitSyntax;
        public static string baseTitle;
        private static MemoryStream traceStream;
        /// <summary>
        /// The entrypoint.
        /// </summary>
        /// <param name="args">The string parameters sent in by the
        /// call site.</param>
        private static void Main(string[] args)
        {
            var traceListenerToDispose = SetupTraceListener();
            var resultsOfCompile = ProcessArgumentSet(args);
            if (resultsOfCompile != null && traceStream != null && traceStream.CanRead)
            {
                var fnames = (from e in resultsOfCompile.Builder.Source.Files
                              orderby e.Length descending
                              select e).ToArray();
                long size = GetTotalFileSize(resultsOfCompile.Builder.Source.Files);
                string relativeRoot = resultsOfCompile.Builder.Source.Files.GetRelativeRoot();
                //GetRelativeRoot(resultsOfCompile.Builder.Source, fnames, fnames.First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries), out size, out relativeRoot);
                using (traceStream)
                using (var traceFile = GetTraceStream(relativeRoot))
                {
                    traceListenerToDispose.Dispose();
                    traceFile.Write(traceStream.ToArray());
                }
            }
        }

        private static HtmlTextWriterTraceListener SetupTraceListener()
        {
            Trace.Listeners.Clear();
            traceStream = new MemoryStream();
            var textStreamer = new HtmlTextWriterTraceListener(traceStream);
            textStreamer.Name = "TextLogger";
            textStreamer.TraceOutputOptions = TraceOptions.None;

            ConsoleTraceListener consoleListener = new ConsoleTraceListener(false);
            consoleListener.TraceOutputOptions = TraceOptions.None;

            Trace.Listeners.Add(textStreamer);
            Trace.Listeners.Add(consoleListener);
            Trace.AutoFlush = true;
            return textStreamer;
        }

        internal static ParserCompilerResults ProcessArgumentSet(string[] args)
        {
            var consoleTitle = Console.Title;
            try
            {
                Console.Title = string.Format("{0}", Path.GetFileNameWithoutExtension(typeof(OilexerProgram).Assembly.Location));
                if (args.Length <= 0)
                {
                    if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                        DisplayLogo();
                    OilexerProgram.DisplayUsage();
                    return null;
                }
                bool exists = false;
                string file = null;
                string extension = null;
                int cycleDepth = 3;
                bool isAlt = false;
                List<string> dotsFor = null;
                HashSet<DeclarationTranslationOrder> toKinds = new HashSet<DeclarationTranslationOrder>();
                foreach (string s in args)
                    if (s.ToLower() == NoSyntax)                    /* -ns        */
                        options = (options & ~ValidOptions.ShowSyntax) | ValidOptions.DoNotEmitSyntax;
                    else if (s.ToLower() == Syntax)                 /* -s         */
                        options = (options & ~ValidOptions.DoNotEmitSyntax) | ValidOptions.ShowSyntax;
                    else if (s.ToLower() == SingleFileOutput)       /* -single    */
                        options |= ValidOptions.SingleFileOutput;
                    else if (s.ToLower() == NoObjectModel)          /* -nom       */
                        options |= ValidOptions.NoObjectModel;
                    else if (s.ToLower() == NoLogo)                 /* -nl        */
                        options = options | ValidOptions.NoLogo;
                    else if ((s.Length > TranslationOrder.Length && s.ToLower().Substring(0, TranslationOrder.Length) == TranslationOrder)   /* -tlorder: */)
                    {
                        string postTLOrder = s.Substring(TranslationOrder.Length);
                        ObtainDeclarationTranslationOrders(toKinds, postTLOrder);
                    }
                    else if ((s.Length > Define.Length && s.ToLower().Substring(0, Define.Length) == Define)   /* -d: */ ||
                        (s.Length > DefineAlt.Length && (isAlt = s.ToLower().Substring(0, DefineAlt.Length) == DefineAlt))) /* -define: */
                    {
                        var defPart = isAlt ? s.Substring(DefineAlt.Length) : s.Substring(Define.Length);
                        if (defPart.Contains("="))
                        {
                            var left = defPart.Substring(0, defPart.IndexOf('='));
                            var right = defPart.Substring(left.Length + 1);
                            ConsoleDefines.Add(left, right);
                        }
                        else

                            ConsoleDefines.Add(defPart, null);
                    }
                    else if (s.ToLower() == Export_TraversalHTML)   /* -ex:t-html */
                    {
                        options &= ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportCSharp | ValidOptions.ExportOilexerTraversalHTML);
                        options |= ValidOptions.ExportTraversalHTML;
                    }
                    else if (s.ToLower() == Export_OilexerTraversalHTML)   /* -ex:ot-html */
                        options = (options & ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportCSharp | ValidOptions.ExportTraversalHTML)) | ValidOptions.ExportOilexerTraversalHTML;
                    else if (s.ToLower() == Export_Dot)             /* -ex:dot    */
                        options = (options & ~(ValidOptions.ExportMask | ValidOptions.NoObjectModel)) | (ValidOptions.ExportDot | ValidOptions.NoObjectModel);
                    else if (s.ToLower() == Export_DotSingle)       /* -ex:dots   */
                        options = (options & ~(ValidOptions.ExportMask | ValidOptions.NoObjectModel)) | (ValidOptions.ExportDotSingle | ValidOptions.NoObjectModel);
                    else if (s.ToLower() == Export_DLL)             /* -ex:dll    */
                        options = (options & ~(ValidOptions.ExportMask)) | ValidOptions.ExportDLL;
                    else if (s.ToLower() == Export_EXE)             /* -ex:exe    */
                        options = (options & ~(ValidOptions.ExportMask)) | ValidOptions.ExportEXE;
                    else if (s.ToLower() == Export_CSharp)          /* -ex:cs     */
                        options = (options & ~(ValidOptions.ExportMask)) | ValidOptions.ExportCSharp;
                    else if (s.ToLower() == Quiet)                  /* -q         */
                        options = (options & ~ValidOptions.VerboseMode) | ValidOptions.QuietMode;
                    else if (s.ToLower() == Verbose)                /* -v         */
                        options = (options & ~ValidOptions.QuietMode) | ValidOptions.VerboseMode;
                    else if (s.Length > StreamAnalysis.Length && s.ToLower().Substring(0, StreamAnalysis.Length) == StreamAnalysis) /* -a:FILE */
                    {
                        var streamFile = s.ToLower().Substring(StreamAnalysis.Length);
                        if (File.Exists(streamFile))
                        {
                            FileInfo fi = new FileInfo(streamFile);
                            StreamAnalysisFiles.Add(fi.FullName);
                        }
                        else if (Directory.Exists(streamFile))
                        {
                            DirectoryInfo di = new DirectoryInfo(streamFile);
                            FillInFolder(di, StreamAnalysisFiles);
                            di = null;
                        }
                    }
                    else if (s.Length > Export_DotSingleFor.Length && s.ToLower().Substring(0, Export_DotSingleFor.Length) == Export_DotSingleFor) /* -ex:dotfor: */
                    {
                        options = (options & ~(ValidOptions.ExportMask | ValidOptions.NoObjectModel)) | (ValidOptions.ExportDotSpecific | ValidOptions.NoObjectModel);
                        var defPart = s.Substring(Export_DotSingleFor.Length);
                        bool containsComma;
                        if (dotsFor == null)
                            dotsFor = new List<string>();
                        while (containsComma = defPart.Contains(','))
                        {
                            var left = defPart.Substring(0, defPart.IndexOf(','));
                            defPart = defPart.Substring(left.Length + 1);
                            dotsFor.Add(left);
                        }
                        dotsFor.Add(defPart);
                    }
                    else if (s.Length > StreamAnalysisExtension.Length && s.ToLower().Substring(0, StreamAnalysisExtension.Length) == StreamAnalysisExtension) /* -ae:EXTENSION */
                        extension = s.ToLower().Substring(StreamAnalysisExtension.Length);
                    else if (s.Length > CycleDepth.Length && s.ToLower().Substring(0, CycleDepth.Length) == CycleDepth) /* -cycledepth: */
                    {
                        string cycleDepthRight = s.ToLower().Substring(CycleDepth.Length);
                        if (!int.TryParse(cycleDepthRight, out cycleDepth))
                        {
                            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                                DisplayLogo();
                            OilexerProgram.DisplayUsage();
                            return null;
                        }
                    }
                    else if (!File.Exists(s)) /* FILENAME */
                        Trace.WriteLine(string.Format("File {0} does not exist.", s));
                    else if (file == null)
                    {
                        exists = true;
                        file = s;
                    }
                if (!string.IsNullOrEmpty(extension))
                    StreamAnalysisFiles = (from analysisFile in StreamAnalysisFiles
                                           where analysisFile.EndsWith(extension)
                                           select analysisFile).ToList();
                if (!exists)
                {
                    if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                        DisplayLogo();
                    OilexerProgram.DisplayUsage();
                    return null;
                }
                return OilexerProgram.ProcessFile(file, toKinds, dotsFor, cycleDepth, (options & ValidOptions.SingleFileOutput) == ValidOptions.SingleFileOutput);
            }
            finally
            {
                Console.Title = consoleTitle;
            }
        }

        private static void ObtainDeclarationTranslationOrders(HashSet<DeclarationTranslationOrder> toKinds, string postTLOrder)
        {
            string[] tlOrderParts = (from s in postTLOrder.Split(',')
                                     select s.Trim().ToLower()).ToArray();
            foreach (var part in tlOrderParts)
            {
                switch (part)
                {
                    case "fld":
                    case "field":
                    case "fields":
                        toKinds.Add(DeclarationTranslationOrder.Fields);
                        break;
                    case "evt":
                    case "event":
                    case "events":
                        toKinds.Add(DeclarationTranslationOrder.Events);
                        break;
                    case "ctor":
                    case ".ctor":
                    case ".ctors":
                    case "ctors":
                    case "constructor":
                    case "constructors":
                        toKinds.Add(DeclarationTranslationOrder.Constructors);
                        break;
                    case "unop":
                    case "unops":
                    case "unaryop":
                    case "unaryops":
                    case "unaryoperator":
                    case "unaryoperators":
                        toKinds.Add(DeclarationTranslationOrder.UnaryOperatorCoercions);
                        break;
                    case "biop":
                    case "binop":
                    case "biops":
                    case "binops":
                    case "binaryop":
                    case "binaryops":
                    case "binaryoperator":
                    case "binaryoperators":
                        toKinds.Add(DeclarationTranslationOrder.BinaryOperatorCoercions);
                        break;
                    case "tpop":
                    case "tpops":
                    case "typeop":
                    case "typeops":
                    case "typeoperator":
                    case "typeoperators":
                        toKinds.Add(DeclarationTranslationOrder.TypeCoercions);
                        break;
                    case "ind":
                    case "inds":
                    case "indexer":
                    case "indexers":
                        toKinds.Add(DeclarationTranslationOrder.Indexers);
                        break;
                    case "prop":
                    case "props":
                    case "property":
                    case "properties":
                        toKinds.Add(DeclarationTranslationOrder.Properties);
                        break;
                    case "method":
                    case "methods":
                    case "fnc":
                    case "fncs":
                    case "function":
                    case "functions":
                        toKinds.Add(DeclarationTranslationOrder.Methods);
                        break;
                    case "cls":
                    case "class":
                    case "classes":
                        toKinds.Add(DeclarationTranslationOrder.Classes);
                        break;
                    case "dlg":
                    case "dlgs":
                    case "delegate":
                    case "delegates":
                        toKinds.Add(DeclarationTranslationOrder.Delegates);
                        break;
                    case "enm":
                    case "enms":
                    case "enum":
                    case "enums":
                        toKinds.Add(DeclarationTranslationOrder.Enums);
                        break;
                    case "int":
                    case "ints":
                    case "interface":
                    case "interfaces":
                        toKinds.Add(DeclarationTranslationOrder.Interfaces);
                        break;
                    case "str":
                    case "struct":
                    case "structs":
                        toKinds.Add(DeclarationTranslationOrder.Structs);
                        break;
                    case "oth":
                    case "oths":
                    case "other":
                    case "others":
                    case "remaining":
                        toKinds.Add(DeclarationTranslationOrder.Remaining);
                        break;
                }
            }
        }

        private static void FillInFolder(DirectoryInfo di, List<string> files)
        {
            foreach (var fileInfo in di.EnumerateFiles())
                files.Add(fileInfo.FullName);
            foreach (var directoryInfo in di.EnumerateDirectories())
                FillInFolder(directoryInfo, files);
        }
#if false
        private static void TestMethod()
        {
            var reflectionTime = Time(BrowseCSCMessagesReflection);
            var abstractionTime = Time(BrowseCSCMessages);
            Trace.WriteLine(string.Format("Abstraction took: {0}\nReflection Took: {1}", abstractionTime, reflectionTime));
        }
#endif
        private static void BrowseCSCMessagesReflection()
        {
            var cscms = typeof(CSharpCompilerMessages);
            var properties = cscms.GetProperties(BindingFlags.Public | BindingFlags.Static);
#pragma warning disable 429
            //Intentional unreachable expression, used for data typing.
            var warnErrors = false ? (new[] { new { IsError = false, MessageId = 0, WarningLevel = 0, Message = String.Empty, Name = String.Empty } }) : null;
#pragma warning restore 429
            Array.Resize(ref warnErrors, properties.Length);
            //Tuple<bool, int, int, string, string>[] warnErrors = new Tuple<bool, int, int, string, string>[properties.Length];
            int index = 0;

            foreach (var prop in properties)
            {
                var returnType = prop.PropertyType;
                var getMethod = prop.GetGetMethod(true);
                if (returnType == typeof(ICompilerReferenceWarning))
                {
                    ICompilerReferenceWarning warning = (ICompilerReferenceWarning)getMethod.Invoke(null, null);
                    warnErrors[index++] = new { IsError = false, MessageId = warning.MessageIdentifier, WarningLevel = warning.WarningLevel, Message = warning.MessageBase, Name = prop.Name };
                    //warnErrors[index++] = new Tuple<bool, int, int, string, string>(false, warning.MessageIdentifier, warning.WarningLevel, warning.MessageBase, prop.Name);
                }
                else if (returnType == typeof(ICompilerReferenceError))
                {
                    ICompilerReferenceError error = (ICompilerReferenceError)getMethod.Invoke(null, null);
                    warnErrors[index++] = new { IsError = true, MessageId = error.MessageIdentifier, WarningLevel = 0, Message = error.MessageBase, Name = prop.Name };
                    //warnErrors[index++] = new Tuple<bool, int, int, string, string>(true, error.MessageIdentifier, 0, error.MessageBase, prop.Name);
                }
            }
            var messages = (from we in warnErrors
                            where we != null
                            orderby we.IsError,
                                    we.WarningLevel,
                                    we.MessageId,
                                    we.Name
                            select we).ToArray();
        }
#if false
        private static void BrowseCSCMessages()
        {
            var cscms = typeof(CSharpCompilerMessages).GetTypeReference<IGeneralGenericTypeUniqueIdentifier, IClassType>();
            //var warnErrors = (new { IsError = false, MessageId = 0, WarningLevel = 0, Message = String.Empty, Name = String.Empty }).GetAnonymousTypeArray(cscms.Properties.Count);
            ////Tuple<bool, int, int, string, string>[] warnErrors = new Tuple<bool, int, int, string, string>[cscms.Properties.Count];
            //int index = 0;
            
            //foreach (ICompiledPropertyMember prop in cscms.Properties.Values)
            //{
            //    var returnType = prop.PropertyType;
            //    if (returnType == typeof(ICompilerReferenceWarning).GetTypeReference())
            //    {
            //        var warning = prop.GetValue<ICompilerReferenceWarning>();
            //        warnErrors[index++] = new { IsError = false, MessageId = warning.MessageIdentifier, WarningLevel = warning.WarningLevel, Message = warning.MessageBase, Name = prop.Name };
            //    }
            //    else if (returnType == typeof(ICompilerReferenceError).GetTypeReference())
            //    {
            //        var error = prop.GetValue<ICompilerReferenceError>();
            //        warnErrors[index++] = new { IsError = true, MessageId = error.MessageIdentifier, WarningLevel = 0, Message = error.MessageBase, Name = prop.Name };
            //    }
            //}
            var errorRef = typeof(ICompilerReferenceError).GetTypeReference();
            var warningRef = typeof(ICompilerReferenceWarning).GetTypeReference();
            var timedFunc = MiscHelperMethods.TimeResult(() =>
                (from ICompiledPropertyMember prop in cscms.Properties.Values
                 let propertyType = prop.PropertyType
                 let isError = propertyType == errorRef
                 let isWarning = propertyType == warningRef
                 where isError || isWarning
                 let warning = isError ? null : prop.GetValue<ICompilerReferenceWarning>()
                 let error = isError ? prop.GetValue<ICompilerReferenceError>() : null
                 let MessageId = isError ?
                     error.MessageIdentifier :
                     warning.MessageIdentifier
                 let WarningLevel = isError ?
                     0 :
                     warning.WarningLevel
                 let Message = isError ?
                     error.MessageBase :
                     warning.MessageBase
                 let Name = prop.Name
                 orderby isError,
                         WarningLevel,
                         MessageId,
                         Name
                 select new
                 {
                     IsError = isError,
                     MessageId = isError ?
                         error.MessageIdentifier :
                         warning.MessageIdentifier,
                     WarningLevel = isError ?
                         0 :
                         warning.WarningLevel,
                     Message = isError ?
                         error.MessageBase :
                         warning.MessageBase,
                     Name = prop.Name
                 }).ToArray());

        }
#endif
        private static TimeSpan Time(Action a)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            a();
            sw.Stop();
            return sw.Elapsed;
        }

        private static ParserCompilerResults ProcessFile(string file, HashSet<DeclarationTranslationOrder> toKinds, List<string> dotsFor = null, int cycleDepth = 3, bool singleFileOutput = false)
        {
            ParserCompilerResults resultsOfBuild = null;
            baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(OilexerProgram).Assembly.Location), Path.GetFileName(file.GetFilenameProperCasing()));
            Console.Title = baseTitle;
            Stopwatch sw = new Stopwatch();
            IParserResults<IOilexerGrammarFile> resultsOfParse = null;
            sw.Start();
            /* *
             * Obtain a parser instance and define the command-line directives.
             * */
            var parser = (options & ValidOptions.ExportOilexerTraversalHTML) == ValidOptions.ExportOilexerTraversalHTML ? new OilexerProvider.AltParser(true, true) : new OilexerParser();
            foreach (var directive in ConsoleDefines)
                parser.Define(directive.Key, directive.Value);
            resultsOfParse = parser.Parse(file);
            var parseTimes = parser.GetParseTimes().ToArray();
            //foreach (var fName in resultsOfParse.Result.Files)
            //    Console.WriteLine(fName);
            sw.Stop();
            var parseTime = sw.Elapsed;
            /* *
             * After parsing, obtain the maximum name length of
             * the entries within the file.  This is for determining the result
             * size of the textual border.
             * */
            var maxQuery = (from e in resultsOfParse.Result
                            let scannableEntry = e as IOilexerGrammarScannableEntry
                            where scannableEntry != null
                            select scannableEntry.Name.Length).ToArray();

            var tLenMax = maxQuery.Length == 0 ? 0 : maxQuery.Max();

            if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
                MaxDescriptionLength = Math.Max(MaxDescriptionLength, tLenMax);
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                DisplayLogo();
            int oldestLongest = longestLineLength;
            longestLineLength = Math.Max(longestLineLength, MaxDescriptionLength + LengthOfTimeColumn);

            if ((options & ValidOptions.NoLogo) == ValidOptions.NoLogo && (options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Trace.WriteLine(string.Format("╒═{0}═╕", '═'.Repeat(longestLineLength)));
            if (resultsOfParse.Successful)
                baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(OilexerProgram).Assembly.Location), resultsOfParse.Result.Options.GrammarName);//string.IsNullOrEmpty(resultsOfParse.Result.Options.AssemblyName) ? resultsOfParse.Result.Options.GrammarName : Path.GetFileName(resultsOfParse.Result.Options.AssemblyName));
            try
            {
                Console.Title = baseTitle;
            }
            catch (IOException)
            {

            }

            /* *
             * If the parser succeeds, build the project and check 
             * potential errors again.
             * */
            GrammarVocabulary fullVocabHit = null;
            var dotsCopy = dotsFor == null ? null : new List<string>(dotsFor);
            if (resultsOfParse.Successful)
            {
                try
                {
                    Console.Title = string.Format("{0} Linking project...", baseTitle);
                }
                catch (IOException) { }
                resultsOfBuild = Build(resultsOfParse, cycleDepth);
                resultsOfBuild.PhaseTimes._Add(ParserCompilerPhase.Parsing, parseTime);
                if (resultsOfBuild == null)
                    goto errorChecker;

                DisplayStateMachineDetails(resultsOfBuild, resultsOfParse);

                if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                    DisplayBuildBreakdown(resultsOfBuild, MaxDescriptionLength);
            errorChecker:

                if (resultsOfBuild == null || resultsOfBuild.Project == null || resultsOfBuild.CompilationErrors.HasErrors)
                {
                    /* *
                     * The builder encountered an error not immediately obvious by linking.
                     * */
                    if ((options & ValidOptions.NoObjectModel) != ValidOptions.NoObjectModel)
                        try
                        {
                            Console.Title = string.Format("{0} could not build project...", baseTitle);
                        }
                        catch (IOException)
                        {
                        }
                    goto ShowParseTime;
                }

                if ((options & ValidOptions.ExportTraversalHTML) == ValidOptions.ExportTraversalHTML)
                {
                    SetAttributes(resultsOfParse, resultsOfBuild);
                    WriteProject(resultsOfBuild, resultsOfParse, toKinds, HtmlCodeFormatterProvider.Singleton, "html", singleFileOutput);
                }
                else if ((options & ValidOptions.ExportCSharp) == ValidOptions.ExportCSharp && !resultsOfBuild.CompilationErrors.HasErrors &&
                    resultsOfBuild.Project != null)
                {
                    SetAttributes(resultsOfParse, resultsOfBuild);
                    var files = WriteProject(resultsOfBuild, resultsOfParse, toKinds, extension: "cs", singleFileOutput: singleFileOutput);
                    WriteCsharpProject(files, resultsOfBuild, resultsOfParse, toKinds);
                }
                else if ((options & ValidOptions.ExportDLL) == ValidOptions.ExportDLL ||
                    (options & ValidOptions.ExportEXE) == ValidOptions.ExportEXE)
                    Compile(resultsOfBuild, resultsOfParse);
                else if ((options & ValidOptions.ExportOilexerTraversalHTML) == ValidOptions.ExportOilexerTraversalHTML)
                    ExportTraversibleHTML(resultsOfParse);
                else if ((options & ValidOptions.ExportDot) == ValidOptions.ExportDot)
                    ExportDotFiles(resultsOfBuild, resultsOfParse);
                else if ((options & ValidOptions.ExportDotSpecific) == ValidOptions.ExportDotSpecific && dotsFor != null)
                    fullVocabHit = ExportSingleDotFile(dotsFor, resultsOfBuild, resultsOfParse, fullVocabHit);
                else if ((options & ValidOptions.ExportDotSingle) == ValidOptions.ExportDotSingle)
                    ExportLargeDotFile(resultsOfBuild, resultsOfParse);
                goto ShowParseTime;
            }
            else
                goto ShowParseTime;

        ShowParseTime:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
            {
                Trace.WriteLine(string.Format("├─{0}─┤", '─'.Repeat(longestLineLength)));
                DisplayTailInformation(MaxDescriptionLength, resultsOfParse, resultsOfBuild);
                Trace.WriteLine(string.Format("╘═{0}═╛", '═'.Repeat(longestLineLength)));
            }
            if ((options & ValidOptions.ShowSyntax) == ValidOptions.ShowSyntax &&
                !(resultsOfParse.SyntaxErrors.HasErrors || resultsOfBuild.CompilationErrors.HasErrors))
            {
                try
                {
                    Console.Title = string.Format("{0} - Expanded grammar.", baseTitle);
                }
                catch (IOException) { }
                ShowSyntax(resultsOfParse);
            }
            try
            {
                Console.Title = string.Format("{0} - {1}", baseTitle, "Finished");
            }
            catch (IOException) { }
            if (resultsOfParse.SyntaxErrors.HasErrors)
                OilexerProgram.ShowErrors(resultsOfParse);
            else if (resultsOfBuild != null && resultsOfBuild.CompilationErrors.HasErrors)
                OilexerProgram.ShowErrors(resultsOfParse, resultsOfBuild.CompilationErrors);
            if (resultsOfBuild != null && resultsOfBuild.CompilationErrors.Count > 0 && !resultsOfBuild.CompilationErrors.HasErrors)
            {
                var consoleListener = Trace.Listeners.Cast<TraceListener>().First(k => k is ConsoleTraceListener);
                Trace.Listeners.Remove(consoleListener.Name);
                ShowErrors(resultsOfParse, resultsOfBuild.CompilationErrors);
                Trace.Listeners.Add(consoleListener);

            }
            foreach (var fName in resultsOfParse.Result.Files.ToArray())
                Trace.WriteLine(fName);
            if (dotsFor != null && dotsFor.Count > 0)
            {
                Trace.WriteLine(string.Format("Could not find an entry with the name{0}:", dotsFor.Count == 1 ? string.Empty : "s"));
                foreach (var dot in dotsFor)
                    Trace.WriteLine(string.Format("\t{0}", dot));
            }

            if ((options & ValidOptions.ExportDotSpecific) == ValidOptions.ExportDotSpecific && dotsFor != null && fullVocabHit != null)
            {
                var graphNames = dotsCopy.Except(dotsFor);
                var rulesHit = new GrammarVocabulary(fullVocabHit.symbols, fullVocabHit.Breakdown.Rules.ToArray()) | new GrammarVocabulary(fullVocabHit.symbols, (from entryName in dotsCopy
                                                                                                                                                                  from symbol in fullVocabHit.symbols
                                                                                                                                                                  let grs = symbol as IGrammarRuleSymbol
                                                                                                                                                                  where grs != null && grs.Source.Name == entryName
                                                                                                                                                                  select grs).Distinct().ToArray());
                var unusedRules = new GrammarVocabulary(fullVocabHit.symbols, (new GrammarVocabulary(fullVocabHit.symbols, fullVocabHit.symbols.ToArray()) & ~rulesHit).Breakdown.Rules.ToArray());
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Format("Rule{1} used by graphs: {0}", rulesHit.IsEmpty ? "none" : rulesHit.ToString(), rulesHit.TrueCount == 1 ? string.Empty : "s"));
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Format("Rule{1} not used by graphs: {0}", unusedRules.IsEmpty ? "none" : unusedRules.ToString(), unusedRules.TrueCount == 1 ? string.Empty : "s"));
            }

            return resultsOfBuild;
        }

        private static void WriteCsharpProject(string[] files, ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse, HashSet<DeclarationTranslationOrder> toKinds)
        {
            var fileName = Path.Combine(Path.Combine(resultsOfParse.Result.RelativeRoot, "out"), string.Format("{0}.csproj", resultsOfParse.Result.Options.AssemblyName));
            var transformationSchema = typeof(OilexerProgram).Assembly.GetManifestResourceStream("AllenCopeland.Abstraction.Slf.Compilers.Oilexer.Transformation.CSharpProjectTransformer.xslt");
            var xmlDocument =
                new XDocument(
                    new XElement("Build", (new object[]
                                              {
                                                  GetProjectGuidAttribute(resultsOfParse),
                                                  GetRootNamespaceAttribute(resultsOfParse),
                                                  GetAssemblyNameAttribute(resultsOfParse)
                                              })
                                          .Concat(
                                              GetFiles(files, resultsOfBuild, resultsOfParse))
                                          .ToArray()));
            var transformer = new XslCompiledTransform();
            using (var xmlReader = XmlReader.Create(transformationSchema))
                transformer.Load(xmlReader);
            using (var streamReader = new StringReader(xmlDocument.ToString()))
            using (var output = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var xmlReader = XmlReader.Create(streamReader))
            {
                transformer.Transform(xmlReader, new XsltArgumentList(), output);
                output.Flush();
            }
        }

        private static XAttribute GetAssemblyNameAttribute(IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            return new XAttribute(
                                                                  "AssemblyName", resultsOfParse.Result.Options.AssemblyName);
        }

        private static XAttribute GetRootNamespaceAttribute(IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            return new XAttribute(
                "RootNamespace", resultsOfParse.Result.Options.Namespace ?? "OILexer.DefaultNamespace");
        }

        private static IEnumerable<XElement> GetFiles(string[] files, ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            return from file in files
                   let fName = GetFileName(file, resultsOfBuild, resultsOfParse)
                   orderby fName
                   select new XElement(
                       "Include",
                       new XAttribute(
                           "Include",
                           fName));
        }

        private static XAttribute GetProjectGuidAttribute(IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            return new XAttribute(
                "ProjectGuid",
                string.Format("{{{0}}}",
                resultsOfParse.Result.DefinedSymbols.ContainsKey("ProjectGuid")
                ? resultsOfParse.Result.DefinedSymbols["ProjectGuid"]
                : Guid.NewGuid().ToString()));
        }

        private static string GetFileName(string file, ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            file = file.Contains(@"\.\")
                   ? file.Replace(@"\.\", @"\")
                   : file;
            var rr = Path.Combine(resultsOfParse.Result.RelativeRoot, "out");
            if (file.ToLower().StartsWith(rr))
                file = 
                    file.Substring(rr.EndsWith(@"\")
                        ? rr.Length
                        : file[rr.Length].ToString() == @"\" 
                          ? rr.Length + 1
                          : 0);
            return file;
        }

        private static void DisplayStateMachineDetails(ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode &&
                resultsOfParse.Successful && resultsOfBuild != null && resultsOfBuild.Project != null &&
                !resultsOfBuild.CompilationErrors.HasErrors && ((options & ValidOptions.NoObjectModel) != ValidOptions.NoObjectModel))
            {
                const string stateMachineCounts = "State machine state counts:";
                /* *
                 * Display the number of state machine states for both rules and tokens.
                 * */
                Trace.WriteLine(string.Format("│ {0}{1} │", stateMachineCounts, ' '.Repeat(longestLineLength - stateMachineCounts.Length)));
                /* *
                 * Obtain a series of elements which indicate the name, state count, and token status of 
                 * the entries in the parsed file.
                 * */
                var toks = (from t in resultsOfParse.Result.GetInlinedTokens()
                            let state = t.DFAState
                            where state != null
                            let stateCount = t.DFAState.CountStates()
                            select new { Name = t.Name, StateCount = stateCount, IsToken = true }).ToArray();
                var rules = (from rule in resultsOfParse.Result.GetRules()
                             let state = resultsOfBuild.RuleStateMachines.ContainsKey(rule) ? resultsOfBuild.RuleStateMachines[rule] : null
                             where state != null
                             let stateCount = state.CountStates()
                             select new { Name = rule.Name, StateCount = stateCount, IsToken = false }).ToArray();
                //Combine the tokens and rules into one array.
                var combined = toks.Add(rules);
                //Group the elements by the number of states.
                var grouped = (from entry in combined
                               orderby entry.StateCount descending,
                                       entry.Name ascending
                               group entry by entry.StateCount).ToDictionary(key => key.Key, value => value.ToList());

                int longestMinusComma = longestLineLength - 2;
                var consoleForeColor = Console.ForegroundColor;
                /* *
                 * Iterate through the elements and print the names
                 * of each entry being cautious to not fill past the edge
                 * of the allotted space.
                 * *
                 * Utilities.Common.StringHandling.FixedJoin could work here;
                 * however, color-coding elements would be out.
                 * */

                foreach (var count in grouped.Keys)
                {
                    string countStr = string.Format(" {0} ", count);
                    Trace.WriteLine(string.Format("├─{1}{0}─┤", '─'.Repeat(longestLineLength - countStr.Length), countStr));
                    int currentLength = 0;
                    bool first = true;
                    Trace.Write("│ ");
                    foreach (var element in grouped[count])
                    {
                        if (first)
                            first = false;
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Trace.Write(", ");
                            Console.ForegroundColor = consoleForeColor;
                            currentLength += 2;
                        }
                        int newLength = currentLength + element.Name.Length;
                        /* *
                         * In the event of spill-over, cap the edge of the
                         * current line and start the next.
                         * */
                        if (newLength >= longestMinusComma)
                        {
                            Trace.WriteLine(string.Format("{0} │", ' '.Repeat(longestLineLength - currentLength)));
                            currentLength = element.Name.Length;
                            Trace.Write("│ ");
                        }
                        else
                            currentLength = newLength;

                        if (element.IsToken)
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        else
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        Trace.Write(element.Name);
                        Console.ForegroundColor = consoleForeColor;
                    }
                    //Cap the last element.
                    Trace.WriteLine(string.Format("{0} │", ' '.Repeat(longestLineLength - currentLength)));

                }
                Console.ForegroundColor = consoleForeColor;
                /* *
                 * Separate the state machine counts from the next section.
                 * */
                Trace.WriteLine(string.Format("├─{0}─┤", '─'.Repeat(longestLineLength)));

            }
        }

        private static void Compile(ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            try
            {
                Console.Title = string.Format("{0} Compiling project...", baseTitle);
            }
            catch (IOException) { }
            string rootPath = string.Empty;
            foreach (var cFile in resultsOfParse.Result.Files)
            {
                string bPath = Path.GetDirectoryName(cFile);
                if (bPath.Length < rootPath.Length || rootPath == string.Empty)
                    rootPath = bPath;
            }
            SetAttributes(resultsOfParse, resultsOfBuild);
            rootPath += string.Format("\\{0}.dll", resultsOfParse.Result.Options.AssemblyName);
            Trace.WriteLine(rootPath);
            /*
            IIntermediateCompiler intermediateCompiler = new CSharpIntermediateCompiler(resultsOfBuild.Project, new IntermediateCompilerOptions(rootPath, true, generateXMLDocs: true, debugSupport: DebugSupport.None));
            intermediateCompiler.Translator.Options.AutoResolveReferences = true;
            intermediateCompiler.Translator.Options.AllowPartials = true;
            intermediateCompiler.Translator.Options.AutoComments = true;
            Stopwatch compileTimer = new Stopwatch();
            compileTimer.Start();
            IIntermediateCompilerResults compileResults = intermediateCompiler.Compile();
            compileTimer.Stop();

            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
            {
                const string compile = "Compile";
                const string compileTime = compile + " time";
                const string compileSuccessful = "Successful";
                const string compileFailure = "Failed";
                Trace.WriteLine(string.Format("│ {1}{2} : {0} │", compileTimer.Elapsed, ' '.Repeat(maxLength - compileTime.Length), compileTime));
                string compileSuccess = string.Format("{0}{1}", ' '.Repeat(maxLength - compile.Length), compile);
                if (compileResults.NativeReturnValue == 0)
                    compileSuccess = string.Format("{0} : {1}", compileSuccess, compileSuccessful);
                else
                    compileSuccess = string.Format("{0} : {1}", compileSuccess, compileFailure);
                compileSuccess = string.Format("{0}{1}", compileSuccess, ' '.Repeat(longestLineLength - compileSuccess.Length));
                Trace.WriteLine(string.Format("│ {0} │", compileSuccess));
            }
            compileResults.TemporaryFiles.KeepFiles = true;
            */
        }

        private static void ExportTraversibleHTML(IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            try
            {
                Console.Title = string.Format("{0} traversible html files...", baseTitle);
                var files = resultsOfParse.Result.Files.ToArray();
                Dictionary<string, IOilexerGrammarToken[]> fileTokens = new Dictionary<string, IOilexerGrammarToken[]>();
                foreach (var oilexerFile in files)
                {
                    List<IOilexerGrammarToken> currentSet = new List<IOilexerGrammarToken>();
                    FileStream fs = new FileStream(oilexerFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var currentLexer = new OilexerParser.Lexer(fs, oilexerFile);
                    do
                    {
                        currentLexer.NextToken();
                        if (currentLexer.CurrentError == null)
                        {
                            var currentToken = currentLexer.CurrentToken;
                            if (currentToken != null)
                                currentSet.Add(currentToken);
                        }
                    } while (currentLexer.CurrentError == null && currentLexer.CurrentToken != null);
                    fileTokens.Add(oilexerFile, currentSet.ToArray());
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (IOException)
            {

            }
        }

        private static void ExportDotFiles(ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            try
            {
                Console.Title = string.Format("{0} Generating dot files...", baseTitle);
            }
            catch (IOException) { }
            SetAttributes(resultsOfParse, resultsOfBuild);

            var fnames = (from e in resultsOfParse.Result.Files
                          orderby e.Length descending
                          select e).ToArray();
            long size = GetTotalFileSize(fnames);
            string relativeRoot = fnames.GetRelativeRoot();

            //GetRelativeRoot(resultsOfParse.Result, fnames, fnames.First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries), out size, out relativeRoot);

            foreach (var tknSet in (from t in resultsOfParse.Result
                                    where t is InlinedTokenEntry && !(t is OilexerGrammarInlinedTokenEofEntry)
                                    group (InlinedTokenEntry)t by t.FileName))
                foreach (var tkn in tknSet)
                    WriteDotFile(tkn, relativeRoot, resultsOfParse.Result);

            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> pre = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState>();
            var grammar = new GrammarSymbolSet(resultsOfParse.Result);
            foreach (var pr in resultsOfParse.Result.GetRules())
            {
                var nfa = (SyntacticalNFARootState)pr.BuildNFA(new SyntacticalNFARootState(pr, pre, grammar), grammar, pre, new Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem>());//new SyntacticalNFARootState(pr, pre, grammar);
                var dfa = nfa.DeterminateAutomata();
                dfa.ReduceDFA();
                dfa.Enumerate();
                pre._Add(pr, dfa);
            }
            foreach (var prSet in (from p in resultsOfParse.Result
                                   where p is IOilexerGrammarProductionRuleEntry
                                   group (IOilexerGrammarProductionRuleEntry)p by p.FileName))
                foreach (var rule in prSet)
                    WriteDotFile(rule, pre[rule], relativeRoot);
        }

        private static GrammarVocabulary ExportSingleDotFile(List<string> dotsFor, ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse, GrammarVocabulary fullVocabHit)
        {
            baseTitle = string.Format("{0} Generating dot files{{0}}...", baseTitle);
            try
            {
                Console.Title = string.Format(baseTitle, " - Building rule and token automations");
            }
            catch (IOException) { }
            SetAttributes(resultsOfParse, resultsOfBuild);

            long size = GetTotalFileSize(resultsOfParse.Result.Files);
            string relativeRoot = resultsOfParse.Result.Files.GetRelativeRoot();

            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> pre = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState>();
            var grammar = new GrammarSymbolSet(resultsOfParse.Result);
            List<IOilexerGrammarProductionRuleEntry> forwardReferences = new List<IOilexerGrammarProductionRuleEntry>();
            List<InlinedTokenEntry> forwardTokens = new List<InlinedTokenEntry>();
        retryRules:
            foreach (var pr in resultsOfParse.Result.GetRules())
                if ((dotsFor.Contains(pr.Name) || forwardReferences.Contains(pr)) && !pre.ContainsKey(pr))
                {
                    try
                    {
                        Console.Title = string.Format(string.Format(baseTitle, " - Building NFA for {0}"), pr.Name);
                    }
                    catch (IOException) { }

                    var nfa = (SyntacticalNFARootState)pr.BuildNFA(new SyntacticalNFARootState(pr, pre, grammar), grammar, pre, new Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem>());
                    try
                    {
                        Console.Title = string.Format(string.Format(baseTitle, " - Building DFA for {0}"), pr.Name);
                    }
                    catch (IOException) { }
                    var dfa = nfa.DeterminateAutomata();
                    try
                    {
                        Console.Title = string.Format(string.Format(baseTitle, " - Reducing DFA for {0}"), pr.Name);
                    }
                    catch (IOException) { }
                    dfa.ReduceDFA();
                    var flatform = new List<SyntacticalDFAState>();
                    SyntacticalDFAState.FlatlineState(dfa, flatform);
                    GrammarVocabulary flatVocab = null;
                    if (!flatform.Contains(dfa))
                        flatform.Add(dfa);
                    foreach (var state in flatform)
                    {
                        var current = state.OutTransitions.FullCheck;
                        if (flatVocab == null)
                            flatVocab = current;
                        else
                            flatVocab |= current;
                    }
                    if (flatVocab != null)
                    {
                        var refRules = (from r in flatVocab.Breakdown.Rules
                                        let rule = r.Source
                                        where !pre.ContainsKey(rule)
                                        select rule).ToArray();
                        var refTkns = (from t in flatVocab.Breakdown.Tokens
                                       let iT = (InlinedTokenEntry)t
                                       select iT);
                        if (refRules.Length == 0)
                            flatVocab = null;
                        foreach (var refTkn in refTkns)
                            if (!forwardTokens.Contains(refTkn))
                                forwardTokens.Add(refTkn);
                        foreach (var refRule in refRules)
                            if (!forwardReferences.Contains(refRule))
                                forwardReferences.Add(refRule);
                    }
                    if (fullVocabHit == null)
                        fullVocabHit = flatVocab;
                    else
                        fullVocabHit |= flatVocab;
                    pre._Add(pr, dfa);
                    if (flatVocab != null)
                        goto retryRules;
                }

            foreach (var tkn in forwardTokens)
            {
                try
                {
                    Console.Title = string.Format(string.Format(baseTitle, " - Building NFA for {0}"), tkn.Name);
                }
                catch (IOException) { }
                tkn.BuildNFA(resultsOfParse.Result);
                try
                {
                    Console.Title = string.Format(string.Format(baseTitle, " - Building DFA for {0}"), tkn.Name);
                }
                catch (IOException) { }
                tkn.BuildDFA();
                try
                {
                    Console.Title = string.Format(string.Format(baseTitle, " - Reducing DFA for {0}"), tkn.Name);
                }
                catch (IOException) { }
                tkn.ReduceDFA();
            }
            try
            {
                Console.Title = string.Format(baseTitle, " - Constructing dot files");
            }
            catch (IOException) { }

            foreach (var tknSet in (from t in resultsOfParse.Result
                                    where t is InlinedTokenEntry && !(t is OilexerGrammarInlinedTokenEofEntry)
                                    group (InlinedTokenEntry)t by t.FileName))
                foreach (var tkn in tknSet)
                    if (dotsFor.Contains(tkn.Name))
                    {
                        dotsFor.Remove(tkn.Name);
                        WriteDotFile(tkn, relativeRoot, resultsOfParse.Result);
                    }
            foreach (var prSet in (from p in resultsOfParse.Result
                                   where p is IOilexerGrammarProductionRuleEntry
                                   group (IOilexerGrammarProductionRuleEntry)p by p.FileName))
                foreach (var rule in prSet)
                    if (dotsFor.Contains(rule.Name))
                    {
                        dotsFor.Remove(rule.Name);
                        WriteDotFile(rule, pre[rule], relativeRoot);
                    }
            return fullVocabHit;
        }

        private static void ExportLargeDotFile(ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse)
        {
            try
            {
                Console.Title = string.Format("{0} Generating dot file with subgraph...", baseTitle);
            }
            catch (IOException) { }
            SetAttributes(resultsOfParse, resultsOfBuild);

            long size = GetTotalFileSize(resultsOfParse.Result.Files);
            string relativeRoot = resultsOfParse.Result.Files.GetRelativeRoot();

            var startEntry = resultsOfBuild.StartEntry;
            var seFile = Path.Combine(relativeRoot, startEntry.Name) + ".gv";
            FileStream gvStream = new FileStream(seFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            IndentedTextWriter itw = new IndentedTextWriter(new StreamWriter(gvStream));
            itw.WriteLine("/* Built from '{0}' */", startEntry.FileName.Substring(relativeRoot.Length == 0 ? 0 : relativeRoot.Length + 1));
            ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> pre = new ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState>();
            var grammar = new GrammarSymbolSet(resultsOfParse.Result);
            foreach (var pr in resultsOfParse.Result.GetRules())
            {
                var nfa = (SyntacticalNFARootState)pr.BuildNFA(new SyntacticalNFARootState(pr, pre, grammar), grammar, pre, new Dictionary<IProductionRuleSource, IProductionRuleCaptureStructuralItem>());
                var dfa = nfa.DeterminateAutomata();
                dfa.ReduceDFA();
                pre._Add(pr, dfa);
            }
            
            bool first = true;
            if (first)
                first = false;
            else
                itw.WriteLine();
            resultsOfBuild.RuleStateMachines = pre;
            WriteDirectedGraph<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>(itw, pre[startEntry], startEntry.Name, false, true, resultsOfBuild.RuleStateMachines, resultsOfParse.Result.GetRules().ToArray(), resultsOfParse.Result.GetInlinedTokens().Cast<IOilexerGrammarTokenEntry>().ToArray());
            itw.Flush();
            gvStream.Close();
            itw.Dispose();
            gvStream.Dispose();
        }

        private static string[] WriteProject(ParserCompilerResults resultsOfBuild, IParserResults<IOilexerGrammarFile> resultsOfParse, HashSet<DeclarationTranslationOrder> toKinds, IIntermediateCodeTranslatorFormatterProvider formatterProvider = null, string extension = null, bool singleFileOutput = false)
        {
            SetAttributes(resultsOfParse, resultsOfBuild);
            long size = GetTotalFileSize(resultsOfParse.Result.Files);
            string relativeRoot = resultsOfParse.Result.Files.GetRelativeRoot();

            var options = new IntermediateCodeTranslatorOptions(formatterProvider ?? new DefaultCodeTranslatorFormatterProvider());
            options.AllowPartials = !singleFileOutput;
            if (singleFileOutput)
            {
                resultsOfBuild.Project.ScopeCoercions.Add(string.Format("{0}.Cst", resultsOfBuild.Project.DefaultNamespace.FullName));
            }
            options.ElementOrderingMethod = TranslationOrderKind.Specific | TranslationOrderKind.Alphabetic;
            if (!toKinds.Contains(DeclarationTranslationOrder.Remaining))
            {
                toKinds.Add(DeclarationTranslationOrder.Fields);
                toKinds.Add(DeclarationTranslationOrder.Constructors);
                toKinds.Add(DeclarationTranslationOrder.UnaryOperatorCoercions);
                toKinds.Add(DeclarationTranslationOrder.BinaryOperatorCoercions);
                toKinds.Add(DeclarationTranslationOrder.TypeCoercions);
                toKinds.Add(DeclarationTranslationOrder.Properties);
                toKinds.Add(DeclarationTranslationOrder.Methods);
                toKinds.Add(DeclarationTranslationOrder.Classes);
                toKinds.Add(DeclarationTranslationOrder.Delegates);
                toKinds.Add(DeclarationTranslationOrder.Enums);
                toKinds.Add(DeclarationTranslationOrder.Interfaces);
                toKinds.Add(DeclarationTranslationOrder.Structs);
            }
            foreach (var kind in toKinds)
                options.TranslationOrder.Add(kind);
            var projectTranslator = new CSharpProjectTranslator(options);
            options.ShortenFilenames = true;
            //AddSerializationDetail(resultsOfBuild);
            return projectTranslator.WriteProject(resultsOfBuild.Project, Path.Combine(relativeRoot, "out"), "." + extension);
            /*
                        var fileOut = string.Format("{0}{1}{2}.{3}", relativeRoot, Path.DirectorySeparatorChar.ToString(), "results", extension);
                        var fileStream = new FileStream(fileOut, FileMode.Create, FileAccess.Write, FileShare.Read);
                        var streamWriter = new StreamWriter(fileStream);
                        streamWriter.AutoFlush = true;
                        cstr.Target = new IndentedTextWriter(streamWriter);
                        cstr.Translate(resultsOfBuild.Project);
                        streamWriter.Flush();
                        streamWriter.Dispose();
                        fileStream.Close();
                        fileStream.Dispose();
                         * */
        }

        private static void AddSerializationDetail(ParserCompilerResults resultsOfBuild)
        {
            var exceptionType = typeof(ArgumentException).GetTypeReference(resultsOfBuild.Project.IdentityManager as ICliManager);

            var targetTypes = resultsOfBuild.Project.GetTypes().Where(k => k.Type == TypeKind.Class && k.BaseType != exceptionType && !k.Name.EndsWith("DebuggerProxy", StringComparison.InvariantCultureIgnoreCase)).Cast<IIntermediateClassType>().Where(k => (k.SpecialModifier & SpecialClassModifier.Static) != SpecialClassModifier.Static);

            foreach (var type in targetTypes)
            {
                if (!(type.IsGenericConstruct && type.IsGenericDefinition))
                    resultsOfBuild.Builder.RootRuleBuilder.LanguageRuleRoot.Metadata.Add(new MetadatumDefinitionParameterValueCollection(typeof(KnownTypeAttribute).GetTypeReference((ICliManager)resultsOfBuild.Project.IdentityManager)) { type });

                type.Metadata.Add(new MetadatumDefinitionParameterValueCollection(typeof(DataContractAttribute).GetTypeReference((ICliManager)resultsOfBuild.Project.IdentityManager)) { { "IsReference", true } });
                var fields = type.Fields.Values.Where(k => !k.IsStatic);
                if (type == resultsOfBuild.Builder.ParserBuilder.ParserClass)
                {
                    var pb = resultsOfBuild.Builder.ParserBuilder;
                    fields = fields.Except(new[] { pb._CurrentContextImpl, pb._LookAheadDepthsImpl, pb._StateImpl });
                }
                else if (type == resultsOfBuild.Builder.CharStreamBuilder.CharStream)
                {
                    var csb = resultsOfBuild.Builder.CharStreamBuilder;
                    fields = fields.Except(new[] { csb.BufferArrayField, csb._StreamReaderImpl, csb.ActualBufferLength, csb.StringCacheImpl });
                }
                else if (type == resultsOfBuild.Builder.RootRuleBuilder.LanguageRuleRoot)
                    type.Metadata.Add(new MetadatumDefinitionParameterValueCollection(typeof(KnownTypeAttribute).GetTypeReference((ICliManager)resultsOfBuild.Project.IdentityManager)) { typeof(List<>).GetTypeReference<IClassType>((ICliManager)resultsOfBuild.Project.IdentityManager).MakeGenericClosure(resultsOfBuild.Builder.CommonSymbolBuilder.ILanguageSymbol) });
                foreach (var field in fields)
                    field.Metadata.Add(new MetadatumDefinitionParameterValueCollection(typeof(DataMemberAttribute).GetTypeReference((ICliManager)resultsOfBuild.Project.IdentityManager)));
            }
        }

        private static void WriteDotFile(InlinedTokenEntry tkn, string rootPath, IOilexerGrammarFile source)
        {
            string fName = null;
            var tknName = tkn.FileName;
            if (tkn.DFAState == null)
            {
                tkn.BuildNFA(source);
                tkn.BuildDFA();
                tkn.ReduceDFA();
                tkn.DFAState.Enumerate();
            }
            fName = Path.Combine(Path.GetDirectoryName(tknName), tkn.Name) + ".gv";
            FileStream gvStream = new FileStream(fName, FileMode.Create, FileAccess.Write, FileShare.Read);
            IndentedTextWriter itw = new IndentedTextWriter(new StreamWriter(gvStream));
            itw.WriteLine("/* Built from '{0}' */", fName.Substring(rootPath.Length == 0 ? 0 : rootPath.Length + 1));
            WriteDirectedGraph<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource>(itw, tkn.DFAState, tkn.Name, false);
            itw.Flush();
            gvStream.Close();
            itw.Dispose();
            gvStream.Dispose();
        }

        private static void WriteDotFile(IOilexerGrammarProductionRuleEntry productionRule, SyntacticalDFARootState state, string rootPath)
        {
            string fName = null;
            var prName = productionRule.FileName;
            fName = Path.Combine(Path.GetDirectoryName(prName), productionRule.Name) + ".gv";
            FileStream gvStream = new FileStream(fName, FileMode.Create, FileAccess.Write, FileShare.Read);
            IndentedTextWriter itw = new IndentedTextWriter(new StreamWriter(gvStream));
            itw.WriteLine("/* Built from '{0}' */", productionRule.FileName.Substring(rootPath.Length == 0 ? 0 : rootPath.Length + 1));
            bool first = true;
            if (first)
                first = false;
            else
                itw.WriteLine();
            WriteDirectedGraph<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>(itw, state, productionRule.Name, false);
            itw.Flush();
            gvStream.Close();
            itw.Dispose();
            gvStream.Dispose();
        }

        private static void WriteDirectedGraph<TCheck, TNFAState, TDFAState, TSourceElement>(IndentedTextWriter itw, TDFAState state, string graphName, bool subgraph, bool recursive = false, ControlledDictionary<IOilexerGrammarProductionRuleEntry, SyntacticalDFARootState> lookup = null, IOilexerGrammarProductionRuleEntry[] ruleEntries = null, IOilexerGrammarTokenEntry[] tokenEntries = null, bool first = true)
            where TCheck :
                class, IFiniteAutomataSet<TCheck>, new()
            where TNFAState :
                NFAState<TCheck, TNFAState, TDFAState, TSourceElement>
            where TDFAState :
                DFAState<TCheck, TNFAState, TDFAState, TSourceElement>,
                new()
            where TSourceElement :
                IFiniteAutomataSource
        {
            if (subgraph)
                itw.Write("subgraph ");
            else
                itw.Write("digraph ");
            itw.WriteLine("\"{0}\"", graphName);
            itw.WriteLine("{");
            itw.Indent++;
            itw.WriteLine("rankdir=LR;");
            List<TDFAState> flatform = new List<TDFAState>();
            DFAState<TCheck, TNFAState, TDFAState, TSourceElement>.FlatlineState(state, flatform);
            if (!flatform.Contains(state))
                flatform.Add(state);
            List<TDFAState> orderedStates = (from s in flatform
                                             orderby s.InTransitions.Count ascending,
                                                     s.StateValue descending
                                             select s).ToList();
            if (first)
            {
                if (ruleEntries != null)
                {
                    foreach (var pr in ruleEntries)
                    {
                        var currentSubstate = lookup[pr];
                        if (currentSubstate == (object)state)
                            continue;
                        WriteDirectedGraph<GrammarVocabulary, SyntacticalNFAState, SyntacticalDFAState, IProductionRuleSource>(itw, currentSubstate, pr.Name, true, true, lookup, ruleEntries, tokenEntries, false);
                    }
                }
                if (tokenEntries != null)
                    foreach (InlinedTokenEntry tkn in tokenEntries)
                    {
                        var currentSubstate = tkn.DFAState;

                        if (currentSubstate == (object)state ||
                            currentSubstate == null ||
                            tkn.DetermineKind() == RegularCaptureType.ContextfulTransducer)
                            continue;
                        WriteDirectedGraph<RegularLanguageSet, RegularLanguageNFAState, RegularLanguageDFAState, ITokenSource>(itw, currentSubstate, tkn.Name, true, true, lookup, ruleEntries, tokenEntries, false);
                    }
            }
            WriteDirectedGraphShapes<TCheck, TNFAState, TDFAState, TSourceElement>(itw, orderedStates, graphName, ruleEntries, tokenEntries);
            WriteDirectedGraphTransitions<TCheck, TNFAState, TDFAState, TSourceElement>(itw, orderedStates, graphName, ruleEntries, tokenEntries);
            itw.Indent--;
            itw.WriteLine("}");
        }

        private static string GetStateValueName(int stateValue, string graphName, IOilexerGrammarProductionRuleEntry[] ruleEntries, IOilexerGrammarTokenEntry[] tokenEntries, bool encaseZero = false)
        {
            if (stateValue == 0 && encaseZero)
                return string.Format("{0} ({1})", graphName, stateValue);
            if (ruleEntries == null)
                return stateValue.ToString();
            return string.Format(@"{0}::{1}", graphName, stateValue);
        }

        private static void WriteDirectedGraphShapes<TCheck, TNFAState, TDFAState, TSourceElement>(IndentedTextWriter itw, List<TDFAState> stateSet, string graphName, IOilexerGrammarProductionRuleEntry[] ruleEntries, IOilexerGrammarTokenEntry[] tokenEntries)
            where TCheck :
                class, IFiniteAutomataSet<TCheck>, new()
            where TDFAState :
                DFAState<TCheck, TNFAState, TDFAState, TSourceElement>,
                new()
            where TNFAState :
                NFAState<TCheck, TNFAState, TDFAState, TSourceElement>
            where TSourceElement :
                IFiniteAutomataSource
        {
            string stateSource = null;
            if (graphName == "CaptureGroup")
            {
            }
            foreach (var state in stateSet)
            {
                if (state.IsEdge)
                {
                    if (state.StateValue == 0)
                    {
                        stateSource = graphName;
                        itw.WriteLine(@"""{0} ({1})"" [shape=rect,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                    }
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                    {
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                        if (stateSource != null)
                            itw.WriteLine(@"""{0} ({1})"" [shape=rect,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        else
                            itw.WriteLine(@"""{0}"" [shape={1}];", state.StateValue, ruleEntries == null ? "doublecircle" : "rect,peripheries=2");
                    }
                    else
                        itw.WriteLine(@"""{0}"" [shape={1}];", state.StateValue, ruleEntries == null ? "doublecircle" : "rect,peripheries=2");
                }
                else if (state is SyntacticalDFAState && ((SyntacticalDFAState)(object)state).ContainsEmptyTarget)
                {
                    if (state.StateValue == 0)
                    {
                        stateSource = graphName;
                        itw.WriteLine(@"""{0} ({1})"" [shape=cds,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                    }
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                    {
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                        if (stateSource != null)
                            itw.WriteLine(@"""{0} ({1})"" [shape=cds,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        else
                            itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "Mcircle" : "cds");
                    }
                    else
                        itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "Mcircle" : "cds");
                }
                else if (state is SyntacticalDFAState && ((SyntacticalDFAState)(object)state).CanBeEmpty)
                {
                    if (state.StateValue == 0)
                    {
                        stateSource = graphName;
                        foreach (var source in stateSource)
                            itw.WriteLine(@"""{0} ({1})"" [shape=cds,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                    }
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                    {
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                        if (stateSource != null)
                            itw.WriteLine(@"""{0} ({1})"" [shape=cds,peripheries=2];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        else
                            itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "Mcircle,peripheries=2" : "cds,peripheries=2");
                    }
                    else
                        itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "Mcircle,peripheries=2" : "cds,peripheries=2");
                }
                else
                {
                    if (state.StateValue == 0)
                    {
                        stateSource = graphName;
                        foreach (var source in stateSource)
                            itw.WriteLine(@"""{0} ({1})"" [shape=rect];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                    }
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                    {
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                        if (stateSource != null)
                            itw.WriteLine(@"""{0} ({1})"" [shape=rect];", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        else
                            itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "circle" : "rect");
                    }
                    else
                        itw.WriteLine(@"""{0}"" [shape={1}];", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries), ruleEntries == null ? "circle" : "rect");
                }
            }
        }

        private static IScannableEntryItem[] GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(TDFAState state)
            where TCheck : class, IFiniteAutomataSet<TCheck>, new()
            where TDFAState : DFAState<TCheck, TNFAState, TDFAState, TSourceElement>,
                new()
            where TNFAState : NFAState<TCheck, TNFAState, TDFAState, TSourceElement>
            where TSourceElement : IFiniteAutomataSource
        {
            var initSet = (from s in state.Sources
                           let sei = s.Item1 as IScannableEntryItem
                           where sei != null
                           where !sei.Name.IsEmptyOrNull() &&
                                 (((s.Item2 & FiniteAutomationSourceKind.Final) == FiniteAutomationSourceKind.Final || (s.Item2 & FiniteAutomationSourceKind.Intermediate) == FiniteAutomationSourceKind.Intermediate && sei.RepeatOptions != ScannableEntryItemRepeatInfo.None && !((s.Item1 is IProductionRuleGroupItem || s.Item1 is ITokenGroupItem))) &&
                                  (s.Item2 & FiniteAutomationSourceKind.Initial) != FiniteAutomationSourceKind.Initial && !(s.Item1 is InlinedTokenReferenceTokenItem) ||
                                  ((s.Item1 is IProductionRuleGroupItem || s.Item1 is ITokenGroupItem) && (((s.Item2 & FiniteAutomationSourceKind.Intermediate) == FiniteAutomationSourceKind.Intermediate && !(s.Item1 is InlinedTokenReferenceTokenItem)) || ((s.Item2 & FiniteAutomationSourceKind.Final) == FiniteAutomationSourceKind.Final && (s.Item1 is InlinedTokenReferenceTokenItem)))))
                           select sei).ToArray();
            return initSet.Concat(from s in state.Sources
                                  let sei = s.Item1 as IScannableEntryItem
                                  where sei != null && sei.Name.IsEmptyOrNull()
                                  from s2 in initSet
                                  let rGroup = s2 as IProductionRuleGroupItem
                                  let tGroup = s2 as ITokenGroupItem
                                  where (tGroup != null || rGroup != null)
                                  where tGroup == null ? rGroup.Contains((IProductionRuleItem)sei) : tGroup.Contains((ITokenItem)sei)
                                  select sei).ToArray();
        }

        private static void WriteDirectedGraphTransitions<TCheck, TNFAState, TDFAState, TSourceElement>(IndentedTextWriter itw, List<TDFAState> stateSet, string graphName, IOilexerGrammarProductionRuleEntry[] ruleEntries, IOilexerGrammarTokenEntry[] tokenEntries)
            where TCheck :
                class, IFiniteAutomataSet<TCheck>, new()
            where TDFAState :
                DFAState<TCheck, TNFAState, TDFAState, TSourceElement>,
                new()
            where TNFAState :
                NFAState<TCheck, TNFAState, TDFAState, TSourceElement>
            where TSourceElement :
                IFiniteAutomataSource
        {
            foreach (var state in stateSet)
            {
                string stateSource = null;
                if (state.IsEdge)
                {
                    if (state.StateValue == 0)
                        stateSource = graphName;
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                }
                else
                {
                    if (state.StateValue == 0)
                        stateSource = graphName;
                    else if (state.Sources.FirstOrDefault(k => k.Item1 is IScannableEntryItem) != null)
                        stateSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(state));
                }
                foreach (var transition in state.OutTransitions)
                {
                    var key = transition.Key;
                    var target = state.OutTransitions[key];
                    var targetSource = GetNamesLimited(GetSourcesFor<TCheck, TNFAState, TDFAState, TSourceElement>(target).ToArray());

                    string sKey = LimitLabel(key.ToString()).Encode(false).Replace("\"", "\\\"").Replace(@"\\\""", "\\\"");

                    if (targetSource != null)
                    {
                        if (stateSource != null)
                        {
                            itw.Write(@"""{0} ({1})""", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                            itw.Write("->");
                            itw.Write(@"""{0} ({1})""", targetSource.Encode(false), GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries));
                            itw.Write(" [label=");
                            itw.Write(@"""{0}""", sKey);
                            itw.WriteLine("];");
                        }
                        else
                        {
                            itw.Write(@"""{0}""", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                            itw.Write("->");
                            itw.Write(@"""{0} ({1})""", targetSource.Encode(false), GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries));
                            itw.Write(" [label=");
                            itw.Write(@"""{0}""", sKey);
                            itw.WriteLine("];");
                        }
                    }
                    else
                    {
                        if (stateSource != null)
                            itw.Write(@"""{0} ({1})""", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        else
                            itw.Write(@"""{0}""", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                        itw.Write("->");
                        itw.Write(@"""{0}""", GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries, true));
                        itw.Write(" [label=");
                        itw.Write(@"""{0}""", sKey);
                        itw.WriteLine("];");
                    }
                    if (state is SyntacticalDFAState)
                    {
                        var synState = (SyntacticalDFAState)(object)state;
                        var check = (GrammarVocabulary)(object)key;
                        var breakdown = check.Breakdown;
                        foreach (var ruleSymbol in breakdown.Rules)
                        {
                            if (synState[ruleSymbol.Source].CanBeEmpty)
                            {
                                if (targetSource != null)
                                {
                                    if (stateSource != null)
                                    {
                                        itw.Write(@"""{0} ({1})""", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                        itw.Write("->");
                                        itw.Write(@"""{0} ({1})""", targetSource.Encode(false), GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries));
                                        itw.Write(" [label=");
                                        itw.Write(@"""{0} was null""", ruleSymbol.Source.Name);
                                        itw.WriteLine("];");
                                    }
                                    else
                                    {
                                        itw.Write(@"""{0}""", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                        itw.Write("->");
                                        itw.Write(@"""{0} ({1})""", targetSource.Encode(false), GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries));
                                        itw.Write(" [label=");
                                        itw.Write(@"""{0} was null""", ruleSymbol.Source.Name);
                                        itw.WriteLine("];");
                                    }
                                }
                                else
                                {
                                    if (stateSource != null)
                                        itw.Write(@"""{0} ({1})""", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                    else
                                        itw.Write(@"""{0}""", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                    itw.Write("->");
                                    itw.Write(@"""{0}""", GetStateValueName(target.StateValue, graphName, ruleEntries, tokenEntries, true));
                                    itw.Write(" [label=");
                                    itw.Write(@"""{0} was null""", ruleSymbol.Source.Name);
                                    itw.WriteLine("];");
                                }
                            }
                        }
                    }
                    if (ruleEntries != null)
                        if (state is SyntacticalDFAState)
                        {
                            var check = (GrammarVocabulary)(object)key;
                            var breakdown = check.Breakdown;
                            foreach (var element in breakdown.CaptureTokens.Concat<IGrammarSymbol>(breakdown.Rules))
                            {
                                if (stateSource != null)
                                {
                                    itw.Write(@"""{0} ({1})""", stateSource.Encode(false), GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                    itw.Write("->");
                                    if (element is IGrammarRuleSymbol)
                                    {
                                        IGrammarRuleSymbol ruleSymbol = (IGrammarRuleSymbol)element;
                                        itw.WriteLine(@"""{0} (0)""", ruleSymbol.ElementName.Encode(false));
                                    }
                                    else if (element is IGrammarVariableSymbol)
                                    {
                                        IGrammarVariableSymbol variableSymbol = (IGrammarVariableSymbol)element;
                                        itw.WriteLine(@"""{0} (0)""", variableSymbol.ElementName.Encode(false));
                                    }
                                }
                                else
                                {
                                    itw.Write(@"""{0}""", GetStateValueName(state.StateValue, graphName, ruleEntries, tokenEntries));
                                    if (element is IGrammarRuleSymbol)
                                    {
                                        IGrammarRuleSymbol ruleSymbol = (IGrammarRuleSymbol)element;
                                        itw.WriteLine(@"""{0} (0)""", ruleSymbol.ElementName.Encode(false));
                                    }
                                    else if (element is IGrammarVariableSymbol)
                                    {
                                        IGrammarVariableSymbol variableSymbol = (IGrammarVariableSymbol)element;
                                        itw.WriteLine(@"""{0} (0)""", variableSymbol.ElementName.Encode(false));
                                    }
                                }
                            }
                        }
                }
            }
        }

        private static string LimitLabel(string p)
        {
            if (p.Length > 45)
            {
                var left = p.Substring(0, 21);
                var right = p.Substring(p.Length - 21);
                return string.Format("{0}...{1}", left, right);
            }
            return p;
        }


        private static string GetNamesLimited(IScannableEntryItem[] set)
        {
            //set = (from s in set
            //       select s).ToArray();
            set = set.Distinct().ToArray();
            Dictionary<IScannableEntryItem, List<IScannableEntryItem>> supers = new Dictionary<IScannableEntryItem, List<IScannableEntryItem>>();

            List<string> result = new List<string>();
            if (set.Any(k => k is IProductionRuleItem))
            {
                var groupSupers = (from k in set
                                   where k is IProductionRuleGroupItem
                                   select (IProductionRuleGroupItem)k).ToArray();
                supers = (from k in set
                          from g in groupSupers
                          where g.Contains((IProductionRuleItem)k)
                          group k by g).ToDictionary(k => (IScannableEntryItem)k.Key, v => v.Cast<IScannableEntryItem>().ToList());
            }
            else
            {
                var groupSupers = (from k in set
                                   where k is ITokenGroupItem
                                   select (ITokenGroupItem)k).ToArray();
                supers = (from k in set
                          from g in groupSupers
                          where g.Contains((ITokenItem)k)
                          group k by g).ToDictionary(k => (IScannableEntryItem)k.Key, v => v.Cast<IScannableEntryItem>().ToList());

            }
            if (supers.Count > 0)
            {
                foreach (var super in supers.Keys)
                {
                    if (super.Name == null)
                        continue;
                    if (supers.Values.Any(k => k.Contains(super)))
                        continue;
                    Stack<IScannableEntryItem[]> superStack = new Stack<IScannableEntryItem[]>();
                    foreach (var element in supers[super])
                        superStack.Push(new[] { super, element });
                    while (superStack.Count > 0)
                    {
                        var currentSuperSet = superStack.Pop();
                        var superPart = currentSuperSet.Last();
                        if (supers.ContainsKey(superPart))
                        {
                            foreach (var element in supers[superPart])
                                if (element.Name != null)
                                    superStack.Push(currentSuperSet.ToArray().AddInline(element).ToArray());
                        }
                        else
                            result.Add(string.Format("{0} ({1})", superPart.Name, string.Join(" on ", from k in currentSuperSet.Reverse().Skip(1)
                                                                                                      where !k.Name.IsEmptyOrNull()
                                                                                                      select k.Name)));
                    }
                }
            }
            else if (set.Length > 0)
                foreach (var item in set)
                    if (!((item is IProductionRuleGroupItem) || (item is ITokenGroupItem)) || item is InlinedTokenReferenceTokenItem && !string.IsNullOrEmpty(item.Name))
                        result.Add(item.Name);
                    else if (item is InlinedTokenReferenceTokenItem && item.Name.IsEmptyOrNull())
                        result.Add(((InlinedTokenReferenceTokenItem)item).Source.Reference.Name);
            return result.Count == 0 ? null : string.Join("\n", result.Distinct());
        }

        internal static IEnumerable<RegularLanguageDFAState> Flatline(RegularLanguageDFARootState state)
        {
            var flatline = new List<RegularLanguageDFAState>();
            RegularLanguageDFAState.FlatlineState(state, flatline);
            if (!flatline.Contains(state))
                flatline.Add(state);
            return from f in flatline
                   orderby f.StateValue
                   select f;
        }

        internal static Action<T1, T2> GetImplicitlyTypedAction<T1, T2>(Action<T1, T2> action)
        {
            return action;
        }

        private class TokenInformationSet :
            FiniteAutomataTransitionTable<RegularLanguageSet, RegularLanguageNFAState, List<IOilexerGrammarTokenEntry>>
        {
            private bool autoSegment;

            internal TokenInformationSet(bool autoSegment)
            {
                this.autoSegment = autoSegment;
            }

            /// <summary>
            /// Adds a <typeparamref name="TCheck"/> condition with the series
            /// of states provided as the non-deterministic target.
            /// </summary>
            /// <param name="check">The <typeparamref name="TCheck"/>
            /// that determines the transitionary condition.</param>
            /// <param name="target">The series of <typeparamref name="IOilexerGrammarTokenEntry"/>
            /// instances which denote the non-deterministic target.</param>
            public override void Add(RegularLanguageSet check, List<IOilexerGrammarTokenEntry> target)
            {
                if (autoSegment)
                {
                    IDictionary<RegularLanguageSet, IFiniteAutomataTransitionNode<RegularLanguageSet, List<IOilexerGrammarTokenEntry>>> colliders;
                    var remainder = base.GetColliders(check, out colliders);
                    /* *
                     * If the intersection is the full condition, check to see
                     * if there's a node that exactly matches, if so, just add the
                     * state to the node target.
                     * */
                    if (colliders.Count == 1 && remainder.IsEmpty)
                    {
                        var first = colliders.First();
                        if (!base.ContainsKey(first.Key))
                            goto alternate;
                        var currentNode = base.GetNode(check);
                        foreach (var state in target)
                        {
                            if (!currentNode.Target.Contains(state))
                                currentNode.Target.Add(state);
                        }
                        goto altSkip;
                    }
                alternate:
                    /* *
                     * Otherwise, iterate the collision sections
                     * and break apart the current entry with
                     * the intersection, repeat until all
                     * colliding nodes are finished.
                     * */
                    foreach (var intersection in colliders.Keys)
                    {
                        var currentNode = colliders[intersection];
                        base.Remove(currentNode.Check);
                        var nodeRemainder = currentNode.Check.SymmetricDifference(intersection);
                        if (!nodeRemainder.IsEmpty)
                            base.AddInternal(nodeRemainder, currentNode.Target);
                        List<IOilexerGrammarTokenEntry> targetSet = new List<IOilexerGrammarTokenEntry>(currentNode.Target);
                        foreach (var subTarget in target)
                            if (!targetSet.Contains(subTarget))
                                targetSet.Add(subTarget);
                        base.AddInternal(intersection, targetSet);
                    }
                    if (!remainder.IsEmpty)
                        base.AddInternal(remainder, new List<IOilexerGrammarTokenEntry>(target));
                altSkip:
                    ;
                }
                else if (base.ContainsKey(check))
                {
                    /* *
                     * Obtain the current node, and simply add the target
                     * elements to the node.
                     * */
                    var currentNode = base.GetNode(check);
                    foreach (var state in target)
                        if (!currentNode.Target.Contains(state))
                            currentNode.Target.Add(state);
                }
                else
                    /* *
                     * If auto-segmentation isn't active and the node
                     * isn't exactly present in the current setup...
                     * */
                    base.AddInternal(check, target);
            }

            protected override List<IOilexerGrammarTokenEntry> GetStateTarget(RegularLanguageNFAState state)
            {
                throw new NotImplementedException();
            }


            public override IEnumerable<RegularLanguageNFAState> Targets { get { yield break; } }
        }

        private static void ShowErrors(IParserResults<IOilexerGrammarFile> parserResults, ICompilerErrorCollection errors)
        {
            //error color used for denoting the specific error.
            const ConsoleColor errorColor = ConsoleColor.DarkRed;
            //Used for the string noting there were errors.
            const ConsoleColor errorMColor = ConsoleColor.Red;
            //warning color.
            const ConsoleColor warnColor = ConsoleColor.DarkYellow;
            //Position Color.
            const ConsoleColor posColor = ConsoleColor.Gray;

            var modelMessages = errors.Where(k => k is ICompilerModelMessage).Cast<ICompilerModelMessage>().ToList();
            var sourceMessages = errors.Where(k => k is ICompilerSourceMessage).Cast<ICompilerSourceMessage>().ToList();
            StreamWriter compilerErrors = null;
            var sortedModelMessages =
                (from ICompilerModelMessage ce in modelMessages
                 orderby ce.MessageIdentifier
                 select ce).ToArray();
            var sortedMessages =
                (from ICompilerSourceMessage ce in sourceMessages
                 orderby ce.Start.Line,
                         ce.Message
                 select ce).ToArray();
            var filenames = (from m in sortedMessages
                             select m.Source.LocalPath).ToArray();
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Ambiguities come late in the game, if we were to list the           *
             * nodes that were touched to define the ambiguity we might have       *
             * to list most of the files.                                          *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Therefore: they aren't 'source' errors, but rather model errors.    *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            int largestColumn = sourceMessages.Count == 0 ? 0 : sortedMessages.Max(p => Math.Max(p.Start.Column, p.End.Column)).ToString().Length;
            int furthestLine = sourceMessages.Count == 0 ? 0 : sortedMessages.Max(p => Math.Max(p.Start.Line, p.End.Line)).ToString().Length;
            var partsRoot = (from ICompilerSourceMessage e in sourceMessages
                             let ePath = Path.GetDirectoryName(e.Source.LocalPath)
                             orderby ePath.Length descending
                             select ePath).FirstOrDefault();
            string[] parts = partsRoot == null ? new string[0] : partsRoot.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            long size = GetTotalFileSize(filenames);
            string relativeRoot = filenames.GetRelativeRoot();
            //GetRelativeRoot(parserResults.Result, filenames, parts, out size, out relativeRoot);
            if (parts.Length > 0)
                compilerErrors = GetErrorStream(relativeRoot);
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Change was made to the sequence of LINQ expressions due to their    *
             * readability versus traditional methods of a myriad of dictionaries. *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            var fileQuery =
                (from error in sortedMessages
                 select error.Source.LocalPath).Distinct();

            var folderQuery =
                (from file in fileQuery
                 select Path.GetDirectoryName(file)).Distinct();

            var fileErrorQuery =
                (from file in fileQuery
                 join error in sortedMessages on file equals error.Source.LocalPath into fileErrors
                 let fileErrorsArray = fileErrors.ToArray()
                 orderby fileErrorsArray.Length descending,
                         file ascending
                 select new { File = Path.GetFileName(file), Path = Path.GetDirectoryName(file), Errors = fileErrorsArray }).ToArray();

            var folderErrors =
                (from folder in folderQuery
                 join file in fileErrorQuery on folder equals file.Path into folderFileErrors
                 let folderFileArray = folderFileErrors.ToArray()
                 orderby folderFileArray.Length descending,
                         folder ascending
                 select new { Path = folder, ErroredFiles = folderFileArray, FileCount = folderFileArray.Length, ErrorCount = folderFileArray.Sum(fileData => fileData.Errors.Length) }).ToArray();


            GC.Collect();
            GC.WaitForPendingFinalizers();
            List<string> erroredFiles = new List<string>();
            var color = Console.ForegroundColor;
            if (relativeRoot != null)
                Trace.WriteLine(string.Format("All folders relative to: {0}", relativeRoot));
            foreach (var folder in folderErrors)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;

                string flError = (folder.ErrorCount > 1) ? "errors" : "error";
                string rPath = null;
                if (relativeRoot != null)
                {
                    if (folder.Path == relativeRoot)
                        rPath = @".\";
                    else
                        rPath = folder.Path.Substring(relativeRoot.Length);// Trace.WriteLine(string.Format("{0} {2} in folder .{1}", folder.ErrorCount, folder.Path.Substring(relativeRoot.Length), flError));
                }
                else
                    rPath = folder.Path;// Trace.WriteLine(string.Format("{0} {2} in folder {1}", folder.ErrorCount, folder.Path, flError));
                Trace.WriteLine(string.Format("{0} {2} in folder {1}", folder.ErrorCount, rPath, flError));

                foreach (var fileErrorSet in folder.ErroredFiles)
                {
                    Console.ForegroundColor = errorMColor;
                    string fError = (fileErrorSet.Errors.Length > 1) ? "errors" : "error";
                    Trace.WriteLine(string.Format("\t{0} {2} in file {1}:", fileErrorSet.Errors.Length, fileErrorSet.File, fError));
                    foreach (var ce in fileErrorSet.Errors)
                    {
                        bool isWarning = ce is ICompilerSourceWarning;
                        if (!isWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Trace.Write(string.Format("\t\t{0} - {{", ce.MessageIdentifier));
                        Console.ForegroundColor = posColor;
                        int l = ce.Start.Line, c = ce.Start.Column;
                        l = furthestLine - l.ToString().Length;
                        c = largestColumn - c.ToString().Length;
                        if (ce.End == LineColumnPair.Zero)
                        {
                            Trace.Write(String.Format("{2}{0}:{1}{3}", ce.Start.Line, ce.Start.Column, ' '.Repeat(l), ' '.Repeat(c)));
                        }
                        else
                        {
                            Trace.Write(string.Format("{2}{0}:{1}", ce.Start.Line, ce.Start.Column, ' '.Repeat(l)));
                            Trace.Write("-");
                            if (ce.Start.Line == ce.End.Line)
                                Trace.Write(string.Format(string.Format("{0}{1}", ce.End.Column, ' '.Repeat(c))));
                            else
                                Trace.Write(string.Format("{0}:{1}{2}", ce.End.Line, ce.End.Column, ' '.Repeat(c)));
                        }

                        if (!isWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Trace.Write(string.Format("}} - {0}", ce.Message));
                        Trace.WriteLine(string.Empty);
                    }
                    Trace.WriteLine(string.Empty);
                }
                Trace.WriteLine(string.Empty);
            }
            if (sortedModelMessages.Length > 0)
            {
                if (sortedMessages.Length == 0)
                {
                    size = GetTotalFileSize(parserResults.Result.Files);
                    relativeRoot = parserResults.Result.Files.GetRelativeRoot();

                    //GetRelativeRoot(parserResults.Result, parserResults.Result.Files.ToArray(), (from string f in parserResults.Result.Files
                    //                                                                             let ePath = Path.GetDirectoryName(f)
                    //                                                                             orderby ePath.Length descending
                    //                                                                             select ePath).First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries), out size, out relativeRoot);
                    compilerErrors = GetErrorStream(relativeRoot);
                }
                foreach (var modelError in sortedModelMessages)
                {
                    compilerErrors.WriteLine("{0} - {1}", modelError.MessageIdentifier, modelError.Message);
                    switch (modelError.MessageIdentifier)
                    {
                        case (int)OilexerGrammarLogicErrors.AmbiguousParsePath:
                            var ambiguousPathMessage = (ICompilerModelError<string>)modelError;
                            compilerErrors.WriteLine(ambiguousPathMessage.Item1);
                            break;
                    }
                }
                compilerErrors.Flush();
                compilerErrors.Close();
            }
            Console.ForegroundColor = color;
            int totalErrorCount = errors.Count,
                totalFileCount = folderErrors.Sum(folderData => folderData.FileCount);
            Trace.WriteLine(string.Format("There were {0} {2} in {1} {3}.", totalErrorCount, totalFileCount, totalErrorCount == 1 ? "error" : "errors", totalFileCount == 1 ? "file" : "files"));
            Trace.WriteLine(string.Format("A total of {0:#,#} bytes were parsed from {1} files.", size, parserResults.Result.Files.Count));
        }

        private static StreamWriter GetErrorStream(string relativeRoot)
        {
            string compilerErrorsFileName = Path.Combine(relativeRoot, "compiler.errors");
            Trace.WriteLine(string.Format("Errors written to {0}", compilerErrorsFileName));
            var compilerErrors = File.Open(compilerErrorsFileName, FileMode.Create, FileAccess.Write);
            return new StreamWriter(compilerErrors);
        }

        private static BinaryWriter GetTraceStream(string relativeRoot)
        {
            string compilerTraceFileName = Path.Combine(relativeRoot, "compiler-trace.html");
            var compilerTrace = File.Open(compilerTraceFileName, FileMode.Create, FileAccess.Write);
            return new BinaryWriter(compilerTrace);
        }
        private static void GetRelativeRoot(IOilexerGrammarFile grammarFile, string[] filenames, string[] parts, out long size, out string relativeRoot)
        {
            size = GetTotalFileSize(filenames);

            if (size == 0)
            {
                relativeRoot = string.Empty;
                return;
            }
            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Breakdown the longest filename and rebuild it part by part,         *
             * where all elements from the error set contain the current path,     *
             * select that path, then select the longest common path.              *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * If the longest file doesn't contain anything in common, then there  *
             * is no group relative path and the paths will be shown in full.      *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            relativeRoot = null;
            for (int i = 1; i < parts.Length; i++)
            {
                string currentRoot = string.Join(@"\", parts, 0, parts.Length - i);
                if (filenames.All(p => p.Contains(currentRoot)))
                {
                    relativeRoot = currentRoot;
                    break;
                }
            }
        }

        private static long GetTotalFileSize(IEnumerable<string> fileNames)
        {
            long size;
            size = (from s in fileNames
                    select new FileInfo(s).Length).Sum();
            return size;
        }

        private static void ShowErrors(IParserResults<IOilexerGrammarFile> iprs)
        {
            long size = (from s in iprs.Result.Files
                         select new FileInfo(s).Length).Sum();

            var iprsSorted =
                (from IParserSyntaxError ce in iprs.SyntaxErrors
                 orderby ce.Start.Line,
                         ce.Message
                 select ce).ToArray();
            int largestColumn = iprsSorted.Max(p => p.Start.Column).ToString().Length;
            int furthestLine = iprsSorted.Max(p => p.Start.Line).ToString().Length;

            string[] parts = (from e in iprs.SyntaxErrors
                              let ePath = Path.GetDirectoryName(e.Source.LocalPath)
                              orderby ePath.Length descending
                              select ePath).First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Breakdown the longest filename and rebuild it part by part,         *
             * where all elements from the error set contain the current path,     *
             * select that path, then select the longest common path.              *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * If the longest file doesn't contain anything in common, then there  *
             * is no group relative path and the paths will be shown in full.      *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            string relativeRoot = null;
            for (int i = 0; i < parts.Length; i++)
            {
                string currentRoot = string.Join(@"\", parts, 0, parts.Length - i);
                if (iprsSorted.All(p => p.Source.LocalPath.Contains(currentRoot)))
                {
                    relativeRoot = currentRoot;
                    break;
                }
            }

            /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
             * Change was made to the sequence of LINQ expressions due to their    *
             * readability versus traditional methods of a myriad of dictionaries. *
             * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
            var fileQuery =
                (from IParserSyntaxError error in iprsSorted
                 select error.Source.LocalPath).Distinct();

            var folderQuery =
                (from file in fileQuery
                 select Path.GetDirectoryName(file)).Distinct();

            var fileErrorQuery =
                (from file in fileQuery
                 join IParserSyntaxError error in iprsSorted on file equals error.Source.LocalPath into fileErrors
                 let fileErrorsArray = fileErrors.ToArray()
                 orderby fileErrorsArray.Length descending,
                         file ascending
                 select new { File = Path.GetFileName(file), Path = Path.GetDirectoryName(file), Errors = fileErrorsArray }).ToArray();

            var folderErrors =
                (from folder in folderQuery
                 join file in fileErrorQuery on folder equals file.Path into folderFileErrors
                 let folderFileArray = folderFileErrors.ToArray()
                 orderby folderFileArray.Length descending,
                         folder ascending
                 select new { Path = folder, ErroredFiles = folderFileArray, FileCount = folderFileArray.Length, ErrorCount = folderFileArray.Sum(fileData => fileData.Errors.Length) }).ToArray();


            GC.Collect();
            GC.WaitForPendingFinalizers();
            List<string> erroredFiles = new List<string>();
            var color = Console.ForegroundColor;
            if (relativeRoot != null)
                Trace.WriteLine(string.Format("All folders relative to: {0}", relativeRoot));
            foreach (var folder in folderErrors)
            {
                //error color used for denoting the specific error.
                const ConsoleColor errorColor = ConsoleColor.DarkRed;
                //Used for the string noting there were errors.
                const ConsoleColor errorMColor = ConsoleColor.Red;
                //Position Color.
                const ConsoleColor posColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.DarkGray;

                string flError = (folder.ErrorCount > 1) ? "errors" : "error";
                string rPath = null;
                if (relativeRoot != null)
                {
                    if (folder.Path == relativeRoot)
                        rPath = @".\";
                    else
                        rPath = folder.Path.Substring(relativeRoot.Length);// Trace.WriteLine(string.Format("{0} {2} in folder .{1}", folder.ErrorCount, folder.Path.Substring(relativeRoot.Length), flError));
                }
                else
                    rPath = folder.Path;// Trace.WriteLine(string.Format("{0} {2} in folder {1}", folder.ErrorCount, folder.Path, flError));
                Trace.WriteLine(string.Format("{0} {2} in folder {1}", folder.ErrorCount, rPath, flError));
                foreach (var fileErrorSet in folder.ErroredFiles)
                {
                    Console.ForegroundColor = errorMColor;
                    string fError = (fileErrorSet.Errors.Length > 1) ? "errors" : "error";
                    Trace.WriteLine(string.Format("\t{0} {2} in file {1}:", fileErrorSet.Errors.Length, fileErrorSet.File, fError));
                    foreach (var ce in fileErrorSet.Errors)
                    {
                        Console.ForegroundColor = errorColor;
                        Trace.Write("\t\t{");
                        Console.ForegroundColor = posColor;
                        int l = ce.Start.Line, c = ce.Start.Column;
                        l = furthestLine - l.ToString().Length;
                        c = largestColumn - c.ToString().Length;
                        Trace.Write(string.Format("{2}{0}:{1}{3}", ce.Start.Line, ce.Start.Column, ' '.Repeat(l), ' '.Repeat(c)));
                        Console.ForegroundColor = errorColor;
                        Trace.Write(string.Format("}} - {0}", ce.Message));
                        Trace.WriteLine(string.Empty);
                    }
                    Trace.WriteLine(string.Empty);
                }
                Trace.WriteLine(string.Empty);
            }
            Console.ForegroundColor = color;
            int totalErrorCount = iprs.SyntaxErrors.Count,
                totalFileCount = folderErrors.Sum(folderData => folderData.FileCount);
            Trace.WriteLine(string.Format("There were {0} {2} in {1} {3}.", totalErrorCount, totalFileCount, totalErrorCount == 1 ? "error" : "errors", totalFileCount == 1 ? "file" : "files"));
            Trace.WriteLine(string.Format("A total of {0:#,#} bytes were parsed from {1} files.", size, iprs.Result.Files.Count));
        }

        private static void SetAttributes(IParserResults<IOilexerGrammarFile> iprs, ParserCompilerResults resultsOfBuild)
        {
            var prj = resultsOfBuild.Project;
            if (prj != null)
            {
                var im = (IIntermediateCliManager)prj.IdentityManager;
                prj.AssemblyInformation.Company = "None";
                prj.AssemblyInformation.AssemblyName = iprs.Result.Options.AssemblyName;
                /* *
                 * Culture specifier here.
                 * */
                prj.AssemblyInformation.Culture = CultureIdentifiers.English_UnitedStates;
                prj.AssemblyInformation.Description = string.Format("Language parser for {0}.", iprs.Result.Options.GrammarName);
                prj.AssemblyInformation.AssemblyVersion = new IntermediateVersion(1, 0) { AutoIncrementRevision = true };
            }
            //resultsOfBuild.Project.Attributes.AddNew(typeof(AssemblyVersionAttribute).GetTypeReference(), new AttributeConstructorParameter(new PrimitiveExpression("1.0.0.*")));
        }

        private static void DisplayTailInformation(int maxLength, IParserResults<IOilexerGrammarFile> iprs, ParserCompilerResults pbrs)
        {
            var sequence = new[] {
                new
                {
                    Title = TitleSequence_NumberOfRules,
                    Value = iprs.Result.GetRules().Count()
                }, new
                {
                    Title = TitleSequence_NumberOfTokens,
                    Value = iprs.Result.GetTokens().Count()
                }, new
                {
                    Title = TitleSequence_NumberOfFiles,
                    Value = iprs.Result.Files.Count
                }, new
                {
                    Title = TitleSequence_NumberOfSymbols,
                    Value = (pbrs == null || pbrs.FullGrammar == null) ? 0 : (int)pbrs.FullGrammar.Length
                },
                null,
                new
                {
                    Title = TitleSequence_CharacterSetCache,
                    Value = RegularLanguageSet.CountComputationCaches()
                },
                new
                {
                    Title = TitleSequence_CharacterSetComputations,
                    Value = RegularLanguageSet.CountComputations()
                } ,
                new
                {
                    Title = TitleSequence_VocabularyCache,
                    Value = GrammarVocabulary.CountComputationCaches()
                },
                new
                {
                    Title = TitleSequence_VocabularyComputations,
                    Value = GrammarVocabulary.CountComputations()
                }};
            foreach (var element in sequence)
                if (element == null)
                    Trace.WriteLine(string.Format("├─{0}─┤", '─'.Repeat(longestLineLength)));
                else
                {
                    var s = string.Format("{0}{1} : {2}", ' '.Repeat(maxLength - element.Title.Length), element.Title, element.Value);
                    Trace.WriteLine(string.Format("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length)));
                }
        }

        private static void DisplayLogo()
        {
            var assembly = typeof(OilexerProgram).Assembly;
            var name = assembly.GetName();
            var copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).Cast<AssemblyCopyrightAttribute>().First();
            string cpr = copyright.Copyright.Replace("©", "(C)");
            string ProjectOpenLine = string.Format("OILexer Parser Compiler version {0}", name.Version);
            longestLineLength = Math.Max(MaxDescriptionLength, ProjectOpenLine.Length) + LengthOfTimeColumn;
            if (cpr.Length > longestLineLength)
                longestLineLength = cpr.Length;
            Trace.WriteLine(String.Format("╒═{0}═╕", '═'.Repeat(longestLineLength)));
            Trace.WriteLine(string.Format("│ {0}{1} │", ProjectOpenLine, ' '.Repeat(longestLineLength-ProjectOpenLine.Length)));
            Trace.WriteLine(string.Format("│ {0}{1} │", cpr, ' '.Repeat(longestLineLength-cpr.Length)));
            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Trace.WriteLine(string.Format("├─{0}─┤", '─'.Repeat(longestLineLength)));
            else
                Trace.WriteLine(string.Format("╘═{0}═╛", '═'.Repeat(longestLineLength)));
        }
        //private static void FinishLogo(int oldestLongest, int cy, int cx)
        //{
        //    bool canAdjustConsoleLocale = true;
        //    try
        //    {
        //        Console.CursorTop = cy - 3;
        //    }
        //    catch (ArgumentOutOfRangeException) { canAdjustConsoleLocale = false; }
        //    catch (IOException) { canAdjustConsoleLocale = false; }
        //    if (canAdjustConsoleLocale)
        //    {
        //        Console.CursorLeft = cx;
        //        Trace.WriteLine(string.Format("{0}═╕", '═'.Repeat(longestLineLength - oldestLongest)));
        //        Console.CursorTop = cy - 2;
        //        Console.CursorLeft = cx;
        //        Trace.WriteLine(string.Format("{0} │", ' '.Repeat(longestLineLength - oldestLongest)));
        //        Console.CursorTop = cy - 1;
        //        Console.CursorLeft = cx;
        //        Trace.WriteLine(string.Format("{0} │", ' '.Repeat(longestLineLength - oldestLongest)));
        //        if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
        //        {
        //            Console.CursorTop = cy;
        //            Console.CursorLeft = cx;
        //            Trace.WriteLine(string.Format("{0}─┤", '─'.Repeat(longestLineLength - oldestLongest)));
        //        }
        //        else
        //        {
        //            Console.CursorTop = cy;
        //            Console.CursorLeft = cx;
        //            Trace.WriteLine(string.Format("{0}═╛", '═'.Repeat(longestLineLength - oldestLongest)));
        //        }
        //    }
        //}

        private static void DisplayBuildBreakdown(ParserCompilerResults resultsOfBuild, int maxLength)
        {
            var phaseIdentifiers = resultsOfBuild.PhaseTimes.Keys.ToDictionary(key => key, value => new { Title = GetPhaseSubString(value), Time = resultsOfBuild.PhaseTimes[value] });
            foreach (ParserCompilerPhase phase in from phase in phaseIdentifiers.Keys
                                                 orderby phaseIdentifiers[phase].Time descending
                                                 select phase)
            {
                var currentPhaseData = phaseIdentifiers[phase];
                if (currentPhaseData.Title == null)
                    continue;
                var s = string.Format("{2}{0} : {1}", currentPhaseData.Title, currentPhaseData.Time, ' '.Repeat(maxLength - currentPhaseData.Title.Length));
                Trace.WriteLine(string.Format("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length)));
            }
        }

        private static void ShowSyntax(IParserResults<IOilexerGrammarFile> iprs)
        {
            var files = iprs.Result.Files.ToArray();
            string relativeRoot = files.GetRelativeRoot();

            var folderQuery = (from file in files
                               select Path.GetDirectoryName(file)).Distinct();
            var folderEntryQuery =
                (from file in files
                 join IOilexerGrammarEntry entry in iprs.Result on file equals entry.FileName into fileEntries
                 let fileEntriesArray = (from entry in fileEntries
                                         orderby entry.Line
                                         select entry).ToArray()
                 select new { File = Path.GetFileName(file), Path = Path.GetDirectoryName(file), Entries = fileEntriesArray }).ToArray();
            var folderEntries =
                (from folder in folderQuery
                 join file in folderEntryQuery on folder equals file.Path into folderFileEntries
                 let folderFiles = folderFileEntries.ToArray()
                 orderby folder ascending
                 select new { Path = folder, Files = folderFiles, FileCount = folderFiles.Length, EntryCount = folderFiles.Sum(fileData => fileData.Entries.Length) }).ToArray();

            var consoleOriginal = Console.ForegroundColor;
            int longestName = ((from entry in iprs.Result
                                let tokenEntry = entry as IOilexerGrammarTokenEntry
                                let ruleEntry = entry as IOilexerGrammarProductionRuleEntry
                                where tokenEntry != null || ruleEntry != null
                                let nameLength = tokenEntry == null ? ruleEntry.Name.Length : tokenEntry.Name.Length
                                orderby nameLength descending
                                select nameLength).FirstOrDefault());
            foreach (var folder in folderEntries)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                string rPath = StringHandling.GetExtensionFromRelativeRoot(folder.Path, relativeRoot);
                var entryTerm = folder.EntryCount == 1 ? "entry" : "entries";
                var fileTerm = folder.FileCount == 1 ? "file" : "files";
                Trace.WriteLine(string.Format("//{2} {3} within {1} {4} in {0}", rPath, folder.FileCount, folder.EntryCount, entryTerm, fileTerm));
                Console.ForegroundColor = consoleOriginal;
                foreach (var file in folder.Files)
                {
                    if (file.Entries.Length == 0)
                        continue;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    if (file.Entries.Length == 1)
                        Trace.WriteLine(string.Format("//1 entry in {0}", file.File));
                    else
                        Trace.WriteLine(string.Format("//{1} entries in {0}", file.File, file.Entries.Length));
                    Console.ForegroundColor = consoleOriginal;
                    foreach (var entry in file.Entries)
                        DisplaySyntax(entry, longestName);
                }
            }
        }

        private static void DisplaySyntax(IOilexerGrammarEntry entry, int longestName)
        {
            if (entry is IOilexerGrammarProductionRuleEntry)
            {
                var pdEntry = (IOilexerGrammarProductionRuleEntry)entry;
                DisplaySyntax((IOilexerGrammarProductionRuleEntry)entry, longestName);
            }
            else if (entry is IOilexerGrammarTokenEntry)
                DisplaySyntax((IOilexerGrammarTokenEntry)entry, longestName);
            Trace.WriteLine(string.Empty);
        }

        private static void DisplaySyntax(IOilexerGrammarProductionRuleEntry nonTerminal, int longestName)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Trace.Write(nonTerminal.Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            //Show all of the rule start symbols aligned to the same point.
            Trace.WriteLine(string.Format("{1}::={0}", nonTerminal.IsRuleCollapsePoint ? ">" : string.Empty, ' '.Repeat((longestName + (nonTerminal.IsRuleCollapsePoint ? 0 : 1)) - nonTerminal.Name.Length)));
            Console.ForegroundColor = consoleColor;
            bool startingLine = true;
            DisplaySeriesSyntax<IProductionRuleItem, IProductionRule, IProductionRuleSeries>(nonTerminal, ref startingLine);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            //If the cursor position is changeable, move it back one.
            if (!startingLine)
            {
                try
                {
                    Console.CursorLeft--;
                }
                catch (ArgumentOutOfRangeException) { }
            }
            //Insert an ending ';'
            Trace.WriteLine(";");
            Console.ForegroundColor = consoleColor;
        }


        private static void DisplaySyntax<T>(T item, ref bool startingLine, int depth)
            where T :
                IScannableEntryItem
        {
            if (item is IProductionRuleGroupItem)
                DisplayGroupSyntax<IProductionRuleItem, IProductionRule, IProductionRuleSeries>((IProductionRuleGroupItem)item, ref startingLine, depth);
            else if (item is ILiteralCharReferenceProductionRuleItem)
                DisplaySyntax((ILiteralCharReferenceProductionRuleItem)item, ref startingLine, depth);
            else if (item is ILiteralStringReferenceProductionRuleItem)
                DisplaySyntax((ILiteralStringReferenceProductionRuleItem)item, ref startingLine, depth);
            else if (item is IRuleReferenceProductionRuleItem)
                DisplaySyntax((IRuleReferenceProductionRuleItem)item, ref startingLine, depth);
            else if (item is ITokenReferenceProductionRuleItem)
                DisplaySyntax((ITokenReferenceProductionRuleItem)item, ref startingLine, depth);
            else if (item is InlinedTokenReferenceTokenItem)
                DisplaySyntax((InlinedTokenReferenceTokenItem)(object)item, ref startingLine, depth);
            else if (item is ILiteralCharTokenItem)
                DisplaySyntax((ILiteralCharTokenItem)item, ref startingLine, depth);
            else if (item is ILiteralStringTokenItem)
                DisplaySyntax((ILiteralStringTokenItem)item, ref startingLine, depth);
            else if (item is ITokenGroupItem)
                DisplayGroupSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>((ITokenGroupItem)item, ref startingLine, depth);
            else if (item is ICharRangeTokenItem)
                DisplaySyntax((ICharRangeTokenItem)item, ref startingLine, depth);
            else if (item is ICommandTokenItem)
                DisplaySyntax((ICommandTokenItem)item, ref startingLine, depth);
            DisplayItemInfo(item, ref startingLine, depth);
        }

        private static void DisplayLiteralOptions(ILiteralTokenItem literalTokenItem)
        {
            var cc = Console.ForegroundColor;
            if (literalTokenItem.IsFlag.HasValue)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Trace.Write("Flag");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Trace.Write(":");
                Console.ForegroundColor = ConsoleColor.Blue;
                Trace.Write(literalTokenItem.IsFlag.Value);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Trace.Write(";");
            }
            Console.ForegroundColor = cc;
        }

        private static void DisplayItemInfo(IScannableEntryItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            bool maxReduce = (item.RepeatOptions.Options & ScannableEntryItemRepeatOptions.MaxReduce) == ScannableEntryItemRepeatOptions.MaxReduce;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!string.IsNullOrEmpty(item.Name))
            {
                Trace.Write(':');
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Trace.Write(item.Name);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Trace.Write(';');
            }

            ScannableEntryItemRepeatOptions optionsNoReduce = (item.RepeatOptions.Options & ~ScannableEntryItemRepeatOptions.MaxReduce);
            if (item is ILiteralTokenItem)
                DisplayLiteralOptions((ILiteralTokenItem)item);
            if (item.RepeatOptions.Options == ScannableEntryItemRepeatOptions.OneOrMore)
                Trace.Write("+");
            else if (optionsNoReduce == ScannableEntryItemRepeatOptions.ZeroOrMore)
                Trace.Write("*");
            else if (optionsNoReduce == ScannableEntryItemRepeatOptions.ZeroOrOne)
                Trace.Write("?");
            else if (optionsNoReduce != ScannableEntryItemRepeatOptions.None)
            {
                Trace.Write("{");
                if (item.RepeatOptions.Min != null && item.RepeatOptions.Max != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.Write(item.RepeatOptions.Min.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Trace.Write(", ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.Write(item.RepeatOptions.Max.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (item.RepeatOptions.Max != null)
                {
                    Trace.Write(" ,");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.Write(item.RepeatOptions.Max.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (item.RepeatOptions.Min != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Trace.Write(item.RepeatOptions.Min.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Trace.Write("}");
            }
            if (maxReduce)
                Trace.Write("$");
            if (item is ILiteralStringTokenItem)
            {
                var stringItem = (ILiteralStringTokenItem)item;
                if (stringItem.SiblingAmbiguity)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Trace.Write("**");
                }
            }
            Console.ForegroundColor = consoleColor;

            Trace.Write(" ");
            if (startingLine)
                startingLine = false;
        }

        private static void DisplaySyntax(IRuleReferenceProductionRuleItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Trace.Write(item.Reference.Name);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ITokenReferenceProductionRuleItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Trace.Write(item.Reference.Name);
            Console.ForegroundColor = consoleColor;
        }

        /**
         *  <summary>
         *  Displays the syntax of a group, from either a token
         *  or a production rule.
         *  </summary>
         *  <typeparam name="TItem">The kind of scannable entry item.</typeparam>
         *  <typeparam name="TExpression">The kind of expression.</typeparam>
         *  <typeparam name="TSeries">The kind of expression series.</typeparam>
         *  <param name="series">The <typeparamref name="TSeries"/> instance which represents the group.</param>
         *  <param name="startingLine">Whether the next element written starts the line.</param>
         *  <param name="depth">The tabbing depth threshold.</param>
         **/
        private static void DisplayGroupSyntax<TItem, TExpression, TSeries>(TSeries series, ref bool startingLine, int depth)
            where TItem :
                IScannableEntryItem
            where TExpression :
                IControlledCollection<TItem>
            where TSeries :
                IControlledCollection<TExpression>
        {
            if (series.Count == 0)
                return;
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            bool needLine = series.Count > 1;
            if (!startingLine && needLine)
            {
                Trace.WriteLine(string.Empty);
                startingLine = true;
            }
            if (needLine)
                Trace.WriteLine(string.Format("{0}(", ' '.Repeat((depth + 1) * 4)));
            else
                if (startingLine)
                    Trace.Write(string.Format("{0}(", ' '.Repeat((depth + 1) * 4)));
                else
                    Trace.Write("(");
            if (!needLine && startingLine)
                startingLine = false;
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<TItem, TExpression, TSeries>(series, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!startingLine && needLine)
                Trace.WriteLine(string.Empty);
            else if (startingLine)
                startingLine = false;
            if (needLine)
                Trace.Write(string.Format("{0})", ' '.Repeat((depth + 1) * 4)));
            else
            {
                try
                {
                    if (!startingLine)
                        Console.CursorLeft--;
                }
                catch (System.ArgumentOutOfRangeException) { }
                Trace.Write(")");
            }
            Console.ForegroundColor = consoleColor;
        }

        /**
         *  <summary>
         *  Displays the syntax of a series of unified expressions.
         *  </summary>
         *  <typeparam name="TItem">The kind of scannable entry item.</typeparam>
         *  <typeparam name="TExpression">The kind of expression.</typeparam>
         *  <typeparam name="TSeries">The kind of expression series.</typeparam>
         *  <param name="series">The <typeparamref name="TSeries"/> instance which represents the series.</param>
         *  <param name="startingLine">Whether the next element written starts the line.</param>
         *  <param name="depth">The tabbing depth threshold.</param>
         * */
        private static void DisplaySeriesSyntax<TItem, TExpression, TSeries>(TSeries series, ref bool startingLine, int depth = 0)
            where TItem :
                IScannableEntryItem
            where TExpression :
                IControlledCollection<TItem>
            where TSeries :
                IControlledCollection<TExpression>
        {
            bool first = true;
            foreach (var expression in series)
            {
                if (first)
                    first = false;
                else
                {
                    var consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    if (!startingLine)
                    {
                        Trace.WriteLine("|");
                        startingLine = true;
                    }
                    else
                        Trace.WriteLine(string.Format("{0}|", ' '.Repeat((depth + 1) * 4)));

                    Console.ForegroundColor = consoleColor;
                }

                DisplaySyntax<TItem, TExpression>(expression, ref startingLine, depth);
            }
        }

        private static void DisplaySyntax<T, U>(U expression, ref bool startingLine, int depth)
            where T :
                IScannableEntryItem
            where U :
                IControlledCollection<T>
        {
            foreach (var item in expression)
                DisplaySyntax<T>(item, ref startingLine, depth);
        }

        private static void DisplaySyntax(ILiteralCharReferenceProductionRuleItem charItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            if (charItem.Literal.CaseInsensitive)
                Trace.Write("@");
            Trace.Write(charItem.Literal.Value.ToString().Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralStringReferenceProductionRuleItem stringItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            if (stringItem.Literal.CaseInsensitive)
                Trace.Write("@");
            Trace.Write(stringItem.Literal.Value.Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(IOilexerGrammarTokenEntry terminal, int longestName)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Trace.Write(terminal.Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Trace.WriteLine(string.Format("{0}:=", ' '.Repeat((longestName + 2) - terminal.Name.Length)));
            Console.ForegroundColor = consoleColor;
            bool startingLine = true;
            DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(terminal.Branches, ref startingLine);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!startingLine)
            {
                try
                {
                    Console.CursorLeft--;
                }
                catch (ArgumentOutOfRangeException) { }
            }
            Trace.WriteLine(";");
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ICommandTokenItem commandItem, ref bool startingLine, int depth)
        {
            if (commandItem is IScanCommandTokenItem)
                DisplaySyntax((IScanCommandTokenItem)commandItem, ref startingLine, depth);
            else if (commandItem is ISubtractionCommandTokenItem)
                DisplaySyntax((ISubtractionCommandTokenItem)commandItem, ref startingLine, depth);
        }

        private static void DisplaySyntax(ISubtractionCommandTokenItem subtractCommand, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Trace.Write("Subtract");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Trace.Write("(");
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(subtractCommand.Left, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Trace.Write(",");
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(subtractCommand.Right, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Trace.Write(")");
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(IScanCommandTokenItem scanCommand, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Trace.Write("Scan");
            DisplayCommandArguments(scanCommand, ref startingLine, depth, consoleColor);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Trace.Write(", ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Trace.Write(scanCommand.SeekPast);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Trace.Write(")");
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplayCommandArguments(IScanCommandTokenItem scanCommand, ref bool startingLine, int depth, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Trace.Write("(");
            bool first = true;
            foreach (var arg in scanCommand.Arguments)
            {
                if (first)
                    first = false;
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Trace.Write(", ");
                    Console.ForegroundColor = consoleColor;
                }
                DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(arg, ref startingLine, depth + 1);
            }
        }

        private static void DisplaySyntax(ICharRangeTokenItem charRange, ref bool startingLine, int depth)
        {
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Trace.Write(charRange.Range);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(InlinedTokenReferenceTokenItem item, ref bool startingLine, int depth)
        {
            //DisplayGroupSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(item, ref startingLine, depth);
            //return;
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Trace.Write(item.Source.Reference.Name);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralStringTokenItem stringItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            if (stringItem.CaseInsensitive)
                Trace.Write("@");
            Trace.Write(stringItem.Value.Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralCharTokenItem charItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Trace.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            if (charItem.CaseInsensitive)
                Trace.Write("@");
            Trace.Write(charItem.Value.ToString().Encode());
            Console.ForegroundColor = consoleColor;
        }


        private static string GetPhaseSubString(ParserCompilerPhase phase)
        {
            string op = null;

            switch (phase)
            {
                case ParserCompilerPhase.Linking:
                    op = PhaseName_Linking;
                    break;
                case ParserCompilerPhase.ExpandingTemplates:
                    op = PhaseName_ExpandingTemplates;
                    break;
                case ParserCompilerPhase.LiteralLookup:
                    op = PhaseName_Deliteralization;
                    break;
                case ParserCompilerPhase.InliningTokens:
                    op = PhaseName_InliningTokens;
                    break;
                case ParserCompilerPhase.TokenNFAConstruction:
                    op = PhaseName_TokenNFAConstruction;
                    break;
                case ParserCompilerPhase.TokenDFAConstruction:
                    op = PhaseName_TokenDFAConstruction;
                    break;
                case ParserCompilerPhase.TokenDFAReduction:
                    op = PhaseName_TokenDFAReduction;
                    break;
                case ParserCompilerPhase.RuleNFAConstruction:
                    op = PhaseName_RuleNFAConstruction;
                    break;
                case ParserCompilerPhase.RuleDuplicationCheck:
                    op = PhaseName_RuleDuplicationCheck;
                    break;
                case ParserCompilerPhase.RuleDFAConstruction:
                    op = PhaseName_RuleDFAConstruction;
                    break;
                case ParserCompilerPhase.RuleDFAReduction:
                    op = PhaseName_RuleDFAReduction;
                    break;
                //case ParserCompilerPhase.CreatingDynamicStateMachine:
                //    op = PhaseName_CreatingDynamicStateMachine;
                //    break;
                case ParserCompilerPhase.CreateCoreLexerAndEstablishFullAmbiguitySet:
                    op = PhaseName_CreateCoreLexerAndEstablishFullAmbiguitySet;
                    break;
                case ParserCompilerPhase.ConstructProjectionNodes:
                    op = PhaseName_ConstructProjectionNodes;
                    break;
                case ParserCompilerPhase.ProjectLookaheadInitial:
                    op = PhaseName_ProjectLookaheadInitial;
                    break;
                case ParserCompilerPhase.ProjectLookaheadExpanded:
                    op = PhaseName_ProjectLookaheadExpanded;
                    break;
                case ParserCompilerPhase.ObjectModelParser:
                    op = PhaseName_ObjectModelParser;
                    break;
                case ParserCompilerPhase.LiftAmbiguitiesAndGenerateLexerCore:
                    op = PhaseName_LiftAmbiguitiesAndGenerateLexerCore;
                    break;
                case ParserCompilerPhase.BuildExtensions:
                    op = PhaseName_BuildExtensions;
                    break;
                case ParserCompilerPhase.ObjectModelScaffolding:
                    op = PhaseName_ObjectModelScaffolding;
                    break;
                case ParserCompilerPhase.ProjectionStateMachines:
                    op = PhaseName_ProjectionStateMachines;
                    break;
                case ParserCompilerPhase.FollowProjectionsAndMachines:
                    op = PhaseName_FollowProjectionsAndMachines;
                    break;
                case ParserCompilerPhase.ObjectModelTokenCaptureConstruction:
                    op = PhaseName_TokenCaptureConstruction;
                    break;
                case ParserCompilerPhase.ObjectModelTokenEnumConstruction:
                    op = PhaseName_TokenEnumConstruction;
                    break;
                case ParserCompilerPhase.ObjectModelRuleStructureConstruction:
                    op = PhaseName_RuleStructureConstruction;
                    break;
                case ParserCompilerPhase.Parsing:
                    op = PhaseName_Parsing;
                    break;
            }
            return op;
        }

        private static ParserCompilerResults Build(IParserResults<IOilexerGrammarFile> file, int cycleDepth)
        {
            ParserCompiler compiler = new ParserCompiler(file.Result, StreamAnalysisFiles, (options & ValidOptions.NoObjectModel) != ValidOptions.NoObjectModel, (options & ValidOptions.ExportOilexerTraversalHTML) != ValidOptions.ExportOilexerTraversalHTML) { CycleDepth = cycleDepth };
            EventHandler<ParserBuilderPhaseChangeEventArgs> changeEvent = (source, phaseArgs) =>
                {
                    if (phaseArgs.Phase == ParserCompilerPhase.None)
                        return;
                    Console.Title = string.Format("{0} - {1}...", OilexerProgram.baseTitle, GetPhaseSubString(phaseArgs.Phase));
                };
            compiler.PhaseChange += changeEvent;
            compiler.BuildProject();
            compiler.PhaseChange -= changeEvent;
            return new ParserCompilerResults() { Project = compiler.ResultAssembly, CompilationErrors = compiler.CompilationErrors, PhaseTimes = new ControlledDictionary<ParserCompilerPhase, TimeSpan>(compiler.PhaseTimes), RuleStateMachines = compiler.RuleDFAStates, FullGrammar = compiler.GrammarSymbols == null ? null : new GrammarVocabulary(compiler.GrammarSymbols, compiler.GrammarSymbols.ToArray()), Symbols = compiler.GrammarSymbols, Builder = compiler, StartEntry = compiler.StartEntry };

        }

        private static void DisplayUsage()
        {
            const string Usage_Options = "options:";
            const string Usage_Export = "       " + Export + "kind; Export, where kind is:";
            const string Usage_Export_Exe = "              " + ExportKind_EXE + " │ Executable";
            const string Usage_Export_Dll = "              " + ExportKind_DLL + " │ Dynamic link library";
            const string Usage_Export_CSharp = "               " + ExportKind_CSharp + " │ CSharp Code";
            const string Usage_Export_THTML = "           " + ExportKind_TraversalHTML + " │ Traversable HTML";
            const string Usage_Syntax = "               " + Syntax + " │ Show syntax.";
            const string Usage_NoSyntax = "             " + NoSyntax + "* │ Don't show syntax";
            const string Usage_NoLogo = "              " + NoLogo + " │ Do not show logo";
            const string Usage_Verbose = "               " + Verbose + " │ Verbose mode";
            const string Usage_QuietMode = "               " + Quiet + " │ Quiet mode";
            const string Usage_Default = "        *         │ default";
            const string Usage_Usage = "Usage:";
            const string Usage_Define = "      " + Define + "CONSTANT │ Define a constant 'CONSTANT'";
            const string Usage_DefineAlt = " " + DefineAlt + "CONSTANT │ ";
            const string Usage_LineCenter = "──────────────────┼";
            const string Usage_LineDown = "──────────────────┬";
            const string Usage_LineUp = "──────────────────┴";
            const string Usage_End = "══════════════════╧";

            string Usage_TagLine = string.Format("    {0} [options] File [options]", Path.GetFileNameWithoutExtension(typeof(OilexerProgram).Assembly.Location));
            string[] usageLines = new string[] {
                Usage_Usage,
                Usage_TagLine,
                "-",
                Usage_Options,
                Usage_Export,
                Usage_LineDown,
                Usage_Export_Exe,
                Usage_Export_Dll,
                Usage_Export_CSharp,
                Usage_Export_THTML,
                Usage_LineCenter,
                Usage_Verbose,
                Usage_NoLogo,
                Usage_QuietMode,
                Usage_LineCenter,
                Usage_Define,
                Usage_DefineAlt,
                Usage_LineCenter,
                Usage_Syntax,
                Usage_NoSyntax,
                Usage_LineCenter,
                Usage_Default,
            };
            int oldMaxLongestLength = longestLineLength;
            longestLineLength = Math.Max(usageLines.Max(p => p.Length), oldMaxLongestLength);
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo && (options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Trace.WriteLine(string.Format("╒═{0}═╕", '═'.Repeat(longestLineLength)));
            foreach (string s in usageLines)
            {
                if (s == "-")
                    Trace.WriteLine(string.Format("├─{0}─┤", '─'.Repeat(longestLineLength)));
                else if (s == Usage_LineDown ||
                         s == Usage_LineUp ||
                         s == Usage_LineCenter)
                    Trace.WriteLine(string.Format("├─{0}{1}─┤", s, '─'.Repeat(longestLineLength - s.Length)));
                else
                    Trace.WriteLine(string.Format("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length)));
            }
            Trace.WriteLine(string.Format("╘═{1}{0}═╛", '═'.Repeat(longestLineLength - Usage_End.Length), Usage_End));
        }

        public static ParserCompilerResults CallMainMethod(params string[] args)
        {
            return ProcessArgumentSet(args);
        }
    }

}