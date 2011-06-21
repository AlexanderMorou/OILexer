using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Cli.Members;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Oil;
using AllenCopeland.Abstraction.Slf.Oil.Expressions;
using AllenCopeland.Abstraction.Slf.Oil.Members;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Arrays;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Utilities.Common;
using AllenCopeland.Abstraction.Globalization;
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
    internal static class Program
    {
        private static int longestLineLength = 0;
        private const string Syntax                                 = "-s";
        private const string NoSyntax                               = "-ns";
        private const string NoLogo                                 = "-nl";
        private const string Quiet                                  = "-q";
        private const string Verbose                                = "-v";
        private const string StreamAnalysis                         = "-a:";
        private const string StreamAnalysisExtension                = "-ae:";
        private const string Export                                 = "-ex:";
        private const string ExportKind_TraversalHTML               = "t-html";
        private const string ExportKind_DLL                         = "dll";
        private const string ExportKind_EXE                         = "exe";
        private const string ExportKind_CSharp                      = "cs";
        private const string Export_TraversalHTML                   = Export + ExportKind_TraversalHTML;
        private const string Export_DLL                             = Export + ExportKind_DLL;
        private const string Export_EXE                             = Export + ExportKind_EXE;
        private const string Export_CSharp                          = Export + ExportKind_CSharp;
        private const string TitleSequence_CharacterSetCache        = "Character set cache size";
        private const string TitleSequence_CharacterSetComputations = "Character set computations";
        private const string TitleSequence_VocabularyCache          = "Vocabulary set computations";
        private const string TitleSequence_VocabularyComputations   = "Vocabulary cache size";
        private const string TitleSequence_NumberOfRules            = "Number of rules";
        private const string TitleSequence_NumberOfTokens           = "Number of tokens";
        private const string PhaseName_Linking                      = "Linking";
        private const string PhaseName_ExpandingTemplates           = "Expanding templates";
        private const string PhaseName_Deliteralization             = "Deliteralization";
        private const string PhaseName_InliningTokens               = "Inlining tokens";
        private const string PhaseName_TokenNFAConstruction         = "Token NFA Construction";
        private const string PhaseName_TokenDFAConstruction         = "Token DFA Construction";
        private const string PhaseName_TokenDFAReduction            = "Token DFA Reduction";
        private const string PhaseName_RuleNFAConstruction          = "Rule NFA Construction";
        private const string PhaseName_RuleDFAConstruction          = "Rule DFA Construction";
        private const string PhaseName_CallTreeAnalysis             = "Call Tree Analysis";
        private const string PhaseName_ObjectModelConstruction      = "Object Model Construction";
        private const string PhaseName_TokenCaptureConstruction     = "Token Capture Construction";
        private const string PhaseName_TokenEnumConstruction        = "Token Enum Construction";
        private const string PhaseName_RuleStructureConstruction    = "Rule Structure Construction";
        private const string PhaseName_Parsing                      = "Parsing";
        //TitleSequence_CharacterSetCache.Length, TitleSequence_CharacterSetComputations.Length, TitleSequence_VocabularyCache.Length, TitleSequence_VocabularyComputations.Length, TitleSequence_NumberOfRules.Length, TitleSequence_NumberOfTokens.Length, PhaseName_Linking.Length, PhaseName_ExpandingTemplates.Length, PhaseName_Deliteralization.Length, PhaseName_InliningTokens.Length, PhaseName_TokenNFAConstruction.Length , PhaseName_TokenDFAConstruction.Length , PhaseName_TokenDFAReduction.Length, PhaseName_RuleNFAConstruction.Length  , PhaseName_RuleDFAConstruction.Length  , PhaseName_CallTreeAnalysis.Length , PhaseName_ObjectModelConstruction.Length  , PhaseName_TokenCaptureConstruction.Length , PhaseName_TokenEnumConstruction.Length, PhaseName_RuleStructureConstruction.Length
        /// <summary>
        /// Defines the valid options for the <see cref="Program"/>.
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
            ShowSyntax              = 0x0001,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not emit 
            /// the syntax at the end.
            /// </summary>
            DoNotEmitSyntax         = 0x0002,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not
            /// display a logo to the console.
            /// </summary>
            NoLogo                  = 0x0014,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit as little
            /// as possible to the console.
            /// </summary>
            QuietMode               = 0x0018,
            /// <summary>
            /// Instructs the <see cref="Program"/> to 
            /// display extra information to the console.
            /// </summary>
            VerboseMode             = 0x0030,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit
            /// a series of hypertext mark-up language (HTML)
            /// files associated to parsing the current
            /// grammar.
            /// </summary>
            ExportTraversalHTML     = 0x0240,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit
            /// a dynamic link library (DLL) which can parse
            /// strings of the described language.
            /// </summary>
            ExportDLL               = 0x0280,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit
            /// a simple executable (EXE) which can parse
            /// strings of the described language by accepting
            /// a series of strings which represent file(s) to 
            /// parse.
            /// </summary>
            ExportEXE               = 0x0300,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit
            /// a series of C&#9839; files.
            /// </summary>
            ExportCSharp            = 0x0600,
        }

        public static List<string> StreamAnalysisFiles = new List<string>();
        public static ValidOptions options             = ValidOptions.DoNotEmitSyntax;
        public static string baseTitle;
        /// <summary>
        /// The entrypoint.
        /// </summary>
        /// <param name="args">The string parameters sent in by the
        /// call site.</param>
        private static void Main(string[] args)
        {
            //TestMethod();
            ProcessArgumentSet(args);
        }

        internal static ParserBuilderResults ProcessArgumentSet(string[] args)
        {
            var consoleTitle = Console.Title;
            try
            {
                Console.Title = string.Format("{0}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
                if (args.Length <= 0)
                {
                    if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                        DisplayLogo();
                    Program.DisplayUsage();
                    return null;
                }
                bool exists = false;
                string file = null;
                string extension = null;
                foreach (string s in args)
                    if (s.ToLower() == NoSyntax) /* -ns */
                        options = (options & ~ValidOptions.ShowSyntax) | ValidOptions.DoNotEmitSyntax;
                    else if (s.ToLower() == Syntax)/* -s */
                        options = (options & ~ValidOptions.DoNotEmitSyntax) | ValidOptions.ShowSyntax;
                    else if (s.ToLower() == NoLogo)/* -nl */
                        options = options | ValidOptions.NoLogo;
                    else if (s.ToLower() == Export_TraversalHTML)/* -ex:t-html */
                    {
                        options &= ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportCSharp);
                        options |= ValidOptions.ExportTraversalHTML;
                    }
                    else if (s.ToLower() == Export_DLL) /* -ex:dll */
                        options = (options & ~(ValidOptions.ExportEXE | ValidOptions.ExportTraversalHTML | ValidOptions.ExportCSharp)) | ValidOptions.ExportDLL;
                    else if (s.ToLower() == Export_EXE) /* -ex:exe */
                        options = (options & ~(ValidOptions.ExportTraversalHTML | ValidOptions.ExportDLL | ValidOptions.ExportCSharp)) | ValidOptions.ExportEXE;
                    else if (s.ToLower() == Export_CSharp) /* -ex:cs */
                        options = (options & ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportTraversalHTML)) | ValidOptions.ExportCSharp;
                    else if (s.ToLower() == Quiet) /* -q */
                        options = (options & ~ValidOptions.VerboseMode) | ValidOptions.QuietMode;
                    else if (s.ToLower() == Verbose) /* -v */
                        options = (options & ~ValidOptions.QuietMode) | ValidOptions.VerboseMode;
                    else if (s.ToLower().Substring(0, StreamAnalysis.Length) == StreamAnalysis) /* -a:FILE */
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
                            foreach (var fileInfo in di.EnumerateFiles())
                                StreamAnalysisFiles.Add(fileInfo.FullName);
                            di = null;
                        }
                    }
                    else if (s.ToLower().Substring(0, StreamAnalysisExtension.Length) == StreamAnalysisExtension) /* -ae:EXTENSION */
                        extension = s.ToLower().Substring(StreamAnalysisExtension.Length);
                    else if (!File.Exists(s)) /* FILENAME */
                        Console.WriteLine("File {0} does not exist.", s);
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
                    Program.DisplayUsage();
                    return null;
                }
                return Program.ProcessFile(file);
            }
            finally
            {
                Console.Title = consoleTitle;
            }
        }

        private static void TestMethod()
        {
            var reflectionTime = Time(BrowseCSCMessagesReflection);
            var abstractionTime = Time(BrowseCSCMessages);
            Console.WriteLine("Abstraction took: {0}\nReflection Took: {1}", abstractionTime, reflectionTime);
        }

        private static void BrowseCSCMessagesReflection() {
            var cscms = typeof(CSharpCompilerMessages);
            var properties = cscms.GetProperties(BindingFlags.Public | BindingFlags.Static);
            var warnErrors = (new { IsError = false, MessageId = 0, WarningLevel = 0, Message = String.Empty, Name = String.Empty }).GetAnonymousTypeArray(properties.Length);
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

        private static void BrowseCSCMessages()
        {
            var cscms = typeof(CSharpCompilerMessages).GetTypeReference<IClassType>();
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
            var timedAction = Utilities.Common.MiscHelperMethods.TimeResult(() =>
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

        private static TimeSpan Time(Action a)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            a();
            sw.Stop();
            return sw.Elapsed;
        }

        private static void T<T1>()
        {
            Console.WriteLine(typeof(T1));
        }

        private static ParserBuilderResults ProcessFile(string file)
        {
            ParserBuilderResults resultsOfBuild = null;
            int maxLength = new int[] { TitleSequence_CharacterSetCache.Length, TitleSequence_CharacterSetComputations.Length, TitleSequence_VocabularyCache.Length, TitleSequence_VocabularyComputations.Length, TitleSequence_NumberOfRules.Length, TitleSequence_NumberOfTokens.Length, PhaseName_Linking.Length, PhaseName_ExpandingTemplates.Length, PhaseName_Deliteralization.Length, PhaseName_InliningTokens.Length, PhaseName_TokenNFAConstruction.Length, PhaseName_TokenDFAConstruction.Length, PhaseName_TokenDFAReduction.Length, PhaseName_RuleNFAConstruction.Length, PhaseName_RuleDFAConstruction.Length, PhaseName_CallTreeAnalysis.Length, PhaseName_ObjectModelConstruction.Length, PhaseName_TokenCaptureConstruction.Length, PhaseName_TokenEnumConstruction.Length, PhaseName_RuleStructureConstruction.Length, PhaseName_Parsing.Length }.Max();
            baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(file));
            Console.Title = baseTitle;
            Stopwatch sw = new Stopwatch();
            IParserResults<IGDFile> resultsOfParse = null;
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                DisplayLogo();
            sw.Start();
            resultsOfParse = file.Parse<IGDToken, IGDTokenizer, IGDFile, OILexerParser>();
            sw.Stop();
            var parseTime = sw.Elapsed;
            var tLenMax = (from e in resultsOfParse.Result
                           let scannableEntry = e as IScannableEntry
                           where scannableEntry != null
                           select scannableEntry.Name.Length).Max();

            if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
                maxLength = Math.Max(maxLength, tLenMax);
            int oldestLongest = longestLineLength;
            longestLineLength = Math.Max(longestLineLength, maxLength + 19);
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                FinishLogo(oldestLongest, Console.CursorTop, Console.CursorLeft);
            else if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Console.WriteLine("╒═{0}═╕", '═'.Repeat(longestLineLength));
            if (resultsOfParse.Successful)
                baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(string.IsNullOrEmpty(resultsOfParse.Result.Options.AssemblyName) ? resultsOfParse.Result.Options.ParserName : resultsOfParse.Result.Options.AssemblyName));
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
            if (resultsOfParse.Successful)
            {
                try
                {
                    Console.Title = string.Format("{0} Linking project...", baseTitle);
                }
                catch (IOException) { }
                resultsOfBuild = Build(resultsOfParse);

                resultsOfBuild.PhaseTimes._AddInternal(ParserBuilderPhase.Parsing, parseTime);
                if (resultsOfBuild == null)
                    goto errorChecker;
                
                if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
                {
                    const string stateMachineCounts = "State machine state counts:";
                    /* *
                     * Display the number of state machine states for both rules and tokens.
                     * */
                    Console.WriteLine("│ {0}{1} │", stateMachineCounts, ' '.Repeat(longestLineLength - stateMachineCounts.Length));
                    /* *
                     * Obtain a series of elements which indicate the name, state count, and token status of 
                     * the entries in the parsed file.
                     * */
                    var toks = (from t in resultsOfParse.Result.GetTokens()
                                let state = t.DFAState
                                where state != null
                                let stateCount = t.DFAState.CountStates()
                                select new { Name = t.Name, StateCount = stateCount, IsToken = true}).ToArray();
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
                        Console.WriteLine("├─{1}{0}─┤", '─'.Repeat(longestLineLength - countStr.Length), countStr);
                        int currentLength = 0;
                        bool first = true;
                        Console.Write("│ ");
                        foreach (var element in grouped[count])
                        {
                            if (first)
                                first = false;
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(", ");
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
                                Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - currentLength));
                                currentLength = element.Name.Length;
                                Console.Write("│ ");
                            }
                            else
                                currentLength = newLength;

                            if (element.IsToken)
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            else
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(element.Name);
                            Console.ForegroundColor = consoleForeColor;
                        }
                        //Cap the last element.
                        Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - currentLength));
                        
                    }
                    Console.ForegroundColor = consoleForeColor;
                    /* *
                     * Separate the state machine counts from the next section.
                     * */
                    Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));

                }
                if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                    DisplayBuildBreakdown(resultsOfBuild, maxLength);
            errorChecker:
                if (resultsOfBuild == null || resultsOfBuild.Project == null || resultsOfBuild.CompilationErrors.HasErrors)
                {
                    /* *
                     * The builder encountered an error not immediately obvious by linking.
                     * */
                    try
                    {
                        Console.Title = string.Format("{0} could not build project...", baseTitle);
                    }
                    catch (IOException)
                    {
                    }
                    goto __CheckErrorAgain;
                }
                
                if ((options & ValidOptions.ExportTraversalHTML) == ValidOptions.ExportTraversalHTML)
                {
                    SetAttributes(resultsOfParse, resultsOfBuild);
                    //WriteProject(resultsOfBuild.Project, ProjectTranslator.GetRelativeRoot(resultsOfParse.Result.Files), ".html", "&nbsp;".Repeat(4), true);
                }
                else if ((options & ValidOptions.ExportCSharp) == ValidOptions.ExportCSharp)
                {
                    SetAttributes(resultsOfParse, resultsOfBuild);
                    //WriteProject(resultsOfBuild.Project, ProjectTranslator.GetRelativeRoot(resultsOfParse.Result.Files));
                }
                else if ((options & ValidOptions.ExportDLL) == ValidOptions.ExportDLL ||
                    (options & ValidOptions.ExportEXE) == ValidOptions.ExportEXE)
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
                        if (bPath.Length < rootPath.Length)
                            rootPath = bPath;
                    }
                    SetAttributes(resultsOfParse, resultsOfBuild);
                    rootPath += string.Format("{0}.dll", resultsOfParse.Result.Options.AssemblyName);
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
                        Console.WriteLine("│ {1}{2} : {0} │", compileTimer.Elapsed, ' '.Repeat(maxLength - compileTime.Length), compileTime);
                        string compileSuccess = string.Format("{0}{1}", ' '.Repeat(maxLength - compile.Length), compile);
                        if (compileResults.NativeReturnValue == 0)
                            compileSuccess = string.Format("{0} : {1}", compileSuccess, compileSuccessful);
                        else
                            compileSuccess = string.Format("{0} : {1}", compileSuccess, compileFailure);
                        compileSuccess = string.Format("{0}{1}", compileSuccess, ' '.Repeat(longestLineLength - compileSuccess.Length));
                        Console.WriteLine("│ {0} │", compileSuccess);
                    }
                    compileResults.TemporaryFiles.KeepFiles = true;
                    */
                }
                goto ShowParseTime;
                
            __CheckErrorAgain:
                if (resultsOfParse.SyntaxErrors.HasErrors)
                    Program.ShowErrors(resultsOfParse);
                else if (resultsOfBuild.CompilationErrors.HasErrors)
                    Program.ShowErrors(resultsOfParse, resultsOfBuild.CompilationErrors);

            }
            else
                Program.ShowErrors(resultsOfParse);
            
        ShowParseTime:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
            {
                Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));
                DisplayTailInformation(maxLength, resultsOfParse);
                Console.WriteLine("╘═{0}═╛", '═'.Repeat(longestLineLength));
            }
            if ((options & ValidOptions.ShowSyntax) == ValidOptions.ShowSyntax)
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
            return resultsOfBuild;
        }

        private static void ShowErrors(IParserResults<IGDFile> parserResults, ICompilerErrorCollection errors)
        {
            long size = (from s in parserResults.Result.Files
                         select new FileInfo(s).Length).Sum();

            var sortedMessages =
                (from ICompilerSourceMessage ce in errors
                 orderby ce.Location.Line,
                         ce.Message
                 select ce).ToArray();
            int largestColumn = sortedMessages.Max(p => p.Location.Column).ToString().Length;
            int furthestLine = sortedMessages.Max(p => p.Location.Line).ToString().Length;

            string[] parts = (from ICompilerSourceMessage e in errors
                              let ePath = Path.GetDirectoryName(e.FileName)
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
                if (sortedMessages.All(p => p.FileName.Contains(currentRoot)))
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
                (from error in sortedMessages
                 select error.FileName).Distinct();

            var folderQuery =
                (from file in fileQuery
                 select Path.GetDirectoryName(file)).Distinct();

            var fileErrorQuery =
                (from file in fileQuery
                 join error in sortedMessages on file equals error.FileName into fileErrors
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
                Console.WriteLine("All folders relative to: {0}", relativeRoot);
            foreach (var folder in folderErrors)
            {
                //error color used for denoting the specific error.
                const ConsoleColor errorColor = ConsoleColor.DarkRed;
                //Used for the string noting there were errors.
                const ConsoleColor errorMColor = ConsoleColor.Red;
                //warning color.
                const ConsoleColor warnColor = ConsoleColor.DarkBlue;
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
                        rPath = folder.Path.Substring(relativeRoot.Length);// Console.WriteLine("{0} {2} in folder .{1}", folder.ErrorCount, folder.Path.Substring(relativeRoot.Length), flError);
                }
                else
                    rPath = folder.Path;// Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, folder.Path, flError);
                Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, rPath, flError);
                foreach (var fileErrorSet in folder.ErroredFiles)
                {
                    Console.ForegroundColor = errorMColor;
                    string fError = (fileErrorSet.Errors.Length > 1) ? "errors" : "error";
                    Console.WriteLine("\t{0} {2} in file {1}:", fileErrorSet.Errors.Length, fileErrorSet.File, fError);
                    foreach (var ce in fileErrorSet.Errors)
                    {
                        bool isWarning = ce is ICompilerSourceWarning;
                        if (!isWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("\t\t{0} - {{", ce.MessageIdentifier);
                        Console.ForegroundColor = posColor;
                        int l = ce.Location.Line, c = ce.Location.Column;
                        l = furthestLine - l.ToString().Length;
                        c = largestColumn - c.ToString().Length;
                        Console.Write("{2}{0}:{1}{3}", ce.Location.Line, ce.Location.Column, ' '.Repeat(l), ' '.Repeat(c));
                        if (!isWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("}} - {0}", ce.Message);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = color;
            int totalErrorCount = errors.Count,
                totalFileCount = folderErrors.Sum(folderData => folderData.FileCount);
            Console.WriteLine("There were {0} {2} in {1} {3}.", totalErrorCount, totalFileCount, totalErrorCount == 1 ? "error" : "errors", totalFileCount == 1 ? "file" : "files");
            Console.WriteLine("A total of {0:#,#} bytes were parsed from {1} files.", size, parserResults.Result.Files.Count);
        }

        private static void ShowErrors(IParserResults<IGDFile> iprs)
        {
            long size = (from s in iprs.Result.Files
                         select new FileInfo(s).Length).Sum();

            var iprsSorted =
                (from IParserSyntaxError ce in iprs.SyntaxErrors
                 orderby ce.Location.Line,
                         ce.Message
                 select ce).ToArray();
            int largestColumn = iprsSorted.Max(p => p.Location.Column).ToString().Length;
            int furthestLine = iprsSorted.Max(p => p.Location.Line).ToString().Length;

            string[] parts = (from e in iprs.SyntaxErrors
                              let ePath = Path.GetDirectoryName(e.FileName)
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
                if (iprsSorted.All(p => p.FileName.Contains(currentRoot)))
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
                 select error.FileName).Distinct();

            var folderQuery =
                (from file in fileQuery
                 select Path.GetDirectoryName(file)).Distinct();

            var fileErrorQuery =
                (from file in fileQuery
                 join IParserSyntaxError error in iprsSorted on file equals error.FileName into fileErrors
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
                Console.WriteLine("All folders relative to: {0}", relativeRoot);
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
                        rPath = folder.Path.Substring(relativeRoot.Length);// Console.WriteLine("{0} {2} in folder .{1}", folder.ErrorCount, folder.Path.Substring(relativeRoot.Length), flError);
                }
                else
                    rPath = folder.Path;// Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, folder.Path, flError);
                Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, rPath, flError);
                foreach (var fileErrorSet in folder.ErroredFiles)
                {
                    Console.ForegroundColor = errorMColor;
                    string fError = (fileErrorSet.Errors.Length > 1) ? "errors" : "error";
                    Console.WriteLine("\t{0} {2} in file {1}:", fileErrorSet.Errors.Length, fileErrorSet.File, fError);
                    foreach (var ce in fileErrorSet.Errors)
                    {
                        Console.ForegroundColor = errorColor;
                        Console.Write("\t\t{");
                        Console.ForegroundColor = posColor;
                        int l = ce.Location.Line, c = ce.Location.Column;
                        l = furthestLine - l.ToString().Length;
                        c = largestColumn - c.ToString().Length;
                        Console.Write("{2}{0}:{1}{3}", ce.Location.Line, ce.Location.Column, ' '.Repeat(l), ' '.Repeat(c));
                        Console.ForegroundColor = errorColor;
                        Console.Write("}} - {0}", ce.Message);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = color;
            int totalErrorCount = iprs.SyntaxErrors.Count,
                totalFileCount  = folderErrors.Sum(folderData => folderData.FileCount);
            Console.WriteLine("There were {0} {2} in {1} {3}.", totalErrorCount, totalFileCount, totalErrorCount == 1 ? "error" : "errors", totalFileCount == 1 ? "file" : "files");
            Console.WriteLine("A total of {0:#,#} bytes were parsed from {1} files.", size, iprs.Result.Files.Count);
        }
        
        /*
        public static void WriteProject(IIntermediateAssembly project, string targetDirectory, string fileExtension = ".cs", string tabString = "    ", bool htmlExportMode = false)
        {
            //Create the translator to use.
            CSharpCodeTranslator translator = new CSharpCodeTranslator();

            if (htmlExportMode)
            {
                var projectLineCounts = new Dictionary<IIntermediateAssembly, int>();
                translator.Options = new IntermediateCodeTranslatorOptions(true, IntermediateCodeTranslator.HTMLFormatter)
                {
                    GetFileNameOf =
                        (type) => ProjectTranslator.GetFileNameFor((IDeclaredType)type, targetDirectory, project, fileExtension, translator.Options, true),
                    GetLineNumber = p =>{
                        if (!projectLineCounts.ContainsKey(p))
                            projectLineCounts.Add(p, 0);
                        return ++projectLineCounts[p];
                    }
                };
            }
            else
                translator.Options = new IntermediateCodeTranslatorOptions(true);
            translator.Options.AutoComments = true;
            TemporaryDirectory td;
            TempFileCollection tfc;
            List<string> files;
            Stack<IIntermediateAssembly> partialCompletions;
            Dictionary<IIntermediateModule, List<string>> moduleFiles;
            ProjectTranslator.WriteProject(project, translator, targetDirectory, out td, out tfc, out files, out partialCompletions, out moduleFiles, true, true, fileExtension, true, tabString);
        }
        //*/

        private static void SetAttributes(IParserResults<IGDFile> iprs, ParserBuilderResults resultsOfBuild)
        {
            resultsOfBuild.Project.AssemblyInformation.Company = "None";
            resultsOfBuild.Project.AssemblyInformation.AssemblyName = iprs.Result.Options.AssemblyName;
            /* *
             * Culture specifier here.
             * */
            resultsOfBuild.Project.AssemblyInformation.Culture = CultureIdentifiers.English_UnitedStates;
            resultsOfBuild.Project.AssemblyInformation.Description = string.Format("Language parser for {0}.", iprs.Result.Options.GrammarName);
            resultsOfBuild.Project.CustomAttributes.Add(new CustomAttributeDefinition.ParameterValueCollection(typeof(AssemblyVersionAttribute).GetTypeReference()) { "1.0.0.*" });
            //resultsOfBuild.Project.Attributes.AddNew(typeof(AssemblyVersionAttribute).GetTypeReference(), new AttributeConstructorParameter(new PrimitiveExpression("1.0.0.*")));
        }

        private static void DisplayTailInformation(int maxLength, IParserResults<IGDFile> iprs)
        {
            var sequence = (
                new
                {
                    Title = TitleSequence_CharacterSetComputations,
                    Value = RegularLanguageSet.CountComputations()
                }).GetAnonymousTypeArray(
                new
                {
                    Title = TitleSequence_CharacterSetCache,
                    Value = RegularLanguageSet.CountComputationCaches()
                },
                new
                {
                    Title = TitleSequence_VocabularyCache,
                    Value = GrammarVocabulary.CountComputations()
                },
                new
                {
                    Title = TitleSequence_VocabularyComputations,
                    Value = GrammarVocabulary.CountComputationCaches()
                });
            //if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
            {
                sequence = sequence.AddBefore(
                new
                {
                    Title = TitleSequence_NumberOfRules,
                    Value = iprs.Result.GetRules().Count()
                }, new
                {
                    Title = TitleSequence_NumberOfTokens,
                    Value = iprs.Result.GetTokens().Count()
                },
                null);
            }
            foreach (var element in sequence)
                if (element == null)
                    Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));
                else
                {
                    var s = string.Format("{0}{1} : {2}", ' '.Repeat(maxLength - element.Title.Length), element.Title, element.Value);
                    Console.WriteLine("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length));
                }
        }

        private static void FinishLogo(int oldestLongest, int cy, int cx)
        {
            bool canAdjustConsoleLocale = true;
            try
            {
                Console.CursorTop = cy - 3;
            }
            catch (ArgumentOutOfRangeException) { canAdjustConsoleLocale = false; }
            if (canAdjustConsoleLocale)
            {
                Console.CursorLeft = cx;
                Console.WriteLine("{0}═╕", '═'.Repeat(longestLineLength - oldestLongest));
                Console.CursorTop = cy - 2;
                Console.CursorLeft = cx;
                Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - oldestLongest));
                Console.CursorTop = cy - 1;
                Console.CursorLeft = cx;
                Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - oldestLongest));
                if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                {
                    Console.CursorTop = cy;
                    Console.CursorLeft = cx;
                    Console.WriteLine("{0}─┤", '─'.Repeat(longestLineLength - oldestLongest));
                }
                else
                {
                    Console.CursorTop = cy;
                    Console.CursorLeft = cx;
                    Console.WriteLine("{0}═╛", '═'.Repeat(longestLineLength - oldestLongest));
                }
            }
        }

        private static void DisplayBuildBreakdown(ParserBuilderResults resultsOfBuild, int maxLength)
        {
            var phaseIdentifiers = resultsOfBuild.PhaseTimes.Keys.ToDictionary(key => key, value => new { Title = GetPhaseSubString(value), Time = resultsOfBuild.PhaseTimes[value] });
            foreach (ParserBuilderPhase phase in from phase in phaseIdentifiers.Keys
                                                 orderby phaseIdentifiers[phase].Time descending
                                                 select phase)
            {
                var currentPhaseData = phaseIdentifiers[phase];
                var s = string.Format("{2}{0} : {1}", currentPhaseData.Title, currentPhaseData.Time, ' '.Repeat(maxLength - currentPhaseData.Title.Length));
                Console.WriteLine("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length));
            }
        }

        private static void ShowSyntax(IParserResults<IGDFile> iprs)
        {
            var files = iprs.Result.Files.ToArray();
            string relativeRoot = files.GetRelativeRoot();

            var folderQuery = (from file in files
                               select Path.GetDirectoryName(file)).Distinct();
            var folderEntryQuery =
                (from file in files
                 join IEntry entry in iprs.Result on file equals entry.FileName into fileEntries
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
                                let tokenEntry = entry as ITokenEntry
                                let ruleEntry = entry as IProductionRuleEntry
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
                Console.WriteLine("//{2} {3} within {1} {4} in {0}", rPath, folder.FileCount, folder.EntryCount, entryTerm, fileTerm);
                Console.ForegroundColor = consoleOriginal;
                foreach (var file in folder.Files)
                {
                    if (file.Entries.Length == 0)
                        continue;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    if (file.Entries.Length == 1)
                        Console.WriteLine("//1 entry in {0}", file.File);
                    else
                        Console.WriteLine("//{1} entries in {0}", file.File, file.Entries.Length);
                    Console.ForegroundColor = consoleOriginal;
                    foreach (var entry in file.Entries)
                        DisplaySyntax(entry, longestName);
                }
            }
        }

        private static void DisplaySyntax(IEntry entry, int longestName)
        {
            if (entry is IProductionRuleEntry)
                DisplaySyntax((IProductionRuleEntry)entry, longestName);
            else if (entry is ITokenEntry)
                DisplaySyntax((ITokenEntry)entry, longestName);
            Console.WriteLine();
        }

        private static void DisplaySyntax(IProductionRuleEntry nonTerminal, int longestName)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("{0} ", nonTerminal.Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            //Show all of the rule start symbols aligned to the same point.
            Console.WriteLine("{1}::={0}", nonTerminal.ElementsAreChildren ? ">" : string.Empty, ' '.Repeat((longestName + (nonTerminal.ElementsAreChildren ? 0 : 1)) - nonTerminal.Name.Length));
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
            Console.WriteLine(";");
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

        private static void DisplayItemInfo(IScannableEntryItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!string.IsNullOrEmpty(item.Name))
            {
                Console.Write(':');
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(item.Name);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(';');
            }
            if (item.RepeatOptions == ScannableEntryItemRepeatInfo.OneOrMore)
                Console.Write("+");
            else if (item.RepeatOptions == ScannableEntryItemRepeatInfo.ZeroOrMore)
                Console.Write("*");
            else if (item.RepeatOptions == ScannableEntryItemRepeatInfo.ZeroOrOne)
                Console.Write("?");
            else if (item.RepeatOptions != ScannableEntryItemRepeatInfo.None)
            {
                Console.Write("{");
                if (item.RepeatOptions.Min != null && item.RepeatOptions.Max != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(item.RepeatOptions.Min.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(", ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(item.RepeatOptions.Max.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (item.RepeatOptions.Max != null)
                {
                    Console.Write(" ,");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(item.RepeatOptions.Max.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (item.RepeatOptions.Min != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(item.RepeatOptions.Min.Value);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write("}");
            }
            if (item is ILiteralStringTokenItem)
            {
                var stringItem = (ILiteralStringTokenItem)item;
                if (stringItem.SiblingAmbiguity)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("**");
                }
            }
            Console.ForegroundColor = consoleColor;

            Console.Write(" ");
            if (startingLine)
                startingLine = false;
        }

        private static void DisplaySyntax(IRuleReferenceProductionRuleItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(item.Reference.Name);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ITokenReferenceProductionRuleItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(item.Reference.Name);
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
                IReadOnlyCollection<TItem>
            where TSeries :
                IReadOnlyCollection<TExpression>
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            bool needLine = series.Count > 1;
            if (!startingLine && needLine)
            {
                Console.WriteLine();
                startingLine = true;
            }
            if (needLine)
                Console.WriteLine("{0}(", ' '.Repeat((depth + 1) * 4));
            else
                if (startingLine)
                    Console.Write("{0}(", ' '.Repeat((depth + 1) * 4));
                else
                    Console.Write("(");
            if (!needLine && startingLine)
                startingLine = false;
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<TItem, TExpression, TSeries>(series, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (!startingLine && needLine)
                Console.WriteLine();
            else if (startingLine)
                startingLine = false;
            if (needLine)
                Console.Write("{0})", ' '.Repeat((depth + 1) * 4));
            else
            {
                try
                {
                    if (!startingLine)
                        Console.CursorLeft--;
                }
                catch (System.ArgumentOutOfRangeException) { }
                Console.Write(")");
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
                IReadOnlyCollection<TItem>
            where TSeries :
                IReadOnlyCollection<TExpression>
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
                        Console.WriteLine("|");
                        startingLine = true;
                    }
                    else
                        Console.WriteLine("{0}|", ' '.Repeat((depth + 1) * 4));

                    Console.ForegroundColor = consoleColor;
                }

                DisplaySyntax<TItem, TExpression>(expression, ref startingLine, depth);
            }
        }

        private static void DisplaySyntax<T, U>(U expression, ref bool startingLine, int depth)
            where T :
                IScannableEntryItem
            where U :
                IReadOnlyCollection<T>
        {
            foreach (var item in expression)
                DisplaySyntax<T>(item, ref startingLine, depth);
        }

        private static void DisplaySyntax(ILiteralCharReferenceProductionRuleItem charItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(charItem.Literal.Value);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralStringReferenceProductionRuleItem stringItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(stringItem.Literal.Value);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ITokenEntry terminal, int longestName)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("{0} ", terminal.Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("{0}:=", ' '.Repeat((longestName + 2) - terminal.Name.Length));
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
            Console.WriteLine(";");
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
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Subtract");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("(");
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(subtractCommand.Left, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(",");
            Console.ForegroundColor = consoleColor;
            DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(subtractCommand.Right, ref startingLine, depth + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(")");
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(IScanCommandTokenItem scanCommand, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Scan");
            DisplayCommandArguments(scanCommand, ref startingLine, depth, consoleColor);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(scanCommand.SeekPast);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(")");
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplayCommandArguments(IScanCommandTokenItem scanCommand, ref bool startingLine, int depth, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("(");
            bool first = true;
            foreach (var arg in scanCommand.Arguments)
            {
                if (first)
                    first = false;
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(", ");
                    Console.ForegroundColor = consoleColor;
                }
                DisplaySeriesSyntax<ITokenItem, ITokenExpression, ITokenExpressionSeries>(arg, ref startingLine, depth + 1);
            }
        }

        private static void DisplaySyntax(ICharRangeTokenItem charRange, ref bool startingLine, int depth)
        {
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(charRange.Range);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(InlinedTokenReferenceTokenItem item, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(item.Source.Reference.Name);
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralStringTokenItem stringItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(stringItem.Value.Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralCharTokenItem charItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(charItem.Value.ToString().Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplayLogo()
        {
            var assembly = typeof(Program).Assembly;
            var name = assembly.GetName();
            var copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).Cast<AssemblyCopyrightAttribute>().First();
            string ProjectOpenLine = string.Format("OILexer Parser Compiler version {0}", name.Version);
            longestLineLength = ProjectOpenLine.Length;
            if (copyright.Copyright.Length > longestLineLength)
                longestLineLength = copyright.Copyright.Length;
            Console.WriteLine("╒═{0}", '═'.Repeat(longestLineLength));
            Console.WriteLine("│ {0}", ProjectOpenLine);
            Console.WriteLine("│ {0}", copyright.Copyright);
            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Console.Write("├─{0}", '─'.Repeat(longestLineLength));
            else
                Console.Write("╘═{0}", '═'.Repeat(longestLineLength));
        }

        private static string GetPhaseSubString(ParserBuilderPhase phase)
        {
            string op = null;

            switch (phase)
            {
                case ParserBuilderPhase.Linking:
                    op = PhaseName_Linking;
                    break;
                case ParserBuilderPhase.ExpandingTemplates:
                    op = PhaseName_ExpandingTemplates;
                    break;
                case ParserBuilderPhase.LiteralLookup:
                    op = PhaseName_Deliteralization;
                    break;
                case ParserBuilderPhase.InliningTokens:
                    op = PhaseName_InliningTokens;
                    break;
                case ParserBuilderPhase.TokenNFAConstruction:
                    op = PhaseName_TokenNFAConstruction;
                    break;
                case ParserBuilderPhase.TokenDFAConstruction:
                    op = PhaseName_TokenDFAConstruction;
                    break;
                case ParserBuilderPhase.TokenDFAReduction:
                    op = PhaseName_TokenDFAReduction;
                    break;
                case ParserBuilderPhase.RuleNFAConstruction:
                    op = PhaseName_RuleNFAConstruction;
                    break;
                case ParserBuilderPhase.RuleDFAConstruction:
                    op = PhaseName_RuleDFAConstruction;
                    break;
                case ParserBuilderPhase.CallTreeAnalysis:
                    op = PhaseName_CallTreeAnalysis;
                    break;
                case ParserBuilderPhase.ObjectModelRootTypesConstruction:
                    op = PhaseName_ObjectModelConstruction;
                    break;
                case ParserBuilderPhase.ObjectModelTokenCaptureConstruction:
                    op = PhaseName_TokenCaptureConstruction;
                    break;
                case ParserBuilderPhase.ObjectModelTokenEnumConstruction:
                    op = PhaseName_TokenEnumConstruction;
                    break;
                case ParserBuilderPhase.ObjectModelRuleStructureConstruction:
                    op = PhaseName_RuleStructureConstruction;
                    break;
                case ParserBuilderPhase.Parsing:
                    op = PhaseName_Parsing;
                    break;
            }
            return op;
        }

        private static ParserBuilderResults Build(IParserResults<IGDFile> file)
        {
            ParserBuilder builder = new ParserBuilder(file.Result, StreamAnalysisFiles);
            EventHandler<ParserBuilderPhaseChangeEventArgs> changeEvent = (source, phaseArgs) =>
                {
                    if (phaseArgs.Phase == ParserBuilderPhase.None)
                        return;
                    Console.Title = string.Format("{0} - {1}...", Program.baseTitle, GetPhaseSubString(phaseArgs.Phase));
                };
            builder.PhaseChange += changeEvent;
            builder.BuildProject();
            builder.PhaseChange -= changeEvent;
            return new ParserBuilderResults() { Project = builder.Project, CompilationErrors = builder.CompilationErrors, PhaseTimes = new ReadOnlyDictionary<ParserBuilderPhase, TimeSpan>(builder.PhaseTimes), RuleStateMachines = builder.RuleDFAStates };
        }

        private static void DisplayUsage()
        {
            const string Usage_Options       = "options:";
            const string Usage_Export        = "    " + Export + "kind; Export, where kind is:";
            const string Usage_Export_Exe    = "           " + ExportKind_EXE + " │ Executable";
            const string Usage_Export_Dll    = "           " + ExportKind_DLL + " │ Dynamic link library";
            const string Usage_Export_CSharp = "            " + ExportKind_CSharp + " │ CSharp Code";
            const string Usage_Export_THTML  = "        " + ExportKind_TraversalHTML + " │ Traversable HTML";
            const string Usage_Syntax        = "    " + Syntax + "         │ Show syntax.";
            const string Usage_NoSyntax      = "    " + NoSyntax + "*       │ Don't show syntax";
            const string Usage_NoLogo        = "    " + NoLogo + "        │ Do not show logo";
            const string Usage_Verbose       = "    " + Verbose + "         │ Verbose mode";
            const string Usage_QuietMode     = "    " + Quiet + "         │ Quiet mode";
            const string Usage_Default       = "     *         │ default";
            const string Usage_Usage         = "Usage:";

            const string Usage_LineCenter = "───────────────┼";
            const string Usage_LineDown   = "───────────────┬";
            const string Usage_LineUp     = "───────────────┴";
            const string Usage_End        = "═══════════════╧";

            string Usage_TagLine = string.Format("    {0} [options] File [options]", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
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
                Usage_Syntax,
                Usage_NoSyntax,
                Usage_LineCenter,
                Usage_Default
            };
            int oldMaxLongestLength = longestLineLength;
            longestLineLength = Math.Max(usageLines.Max(p => p.Length), oldMaxLongestLength);
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                FinishLogo(oldMaxLongestLength, Console.CursorTop, Console.CursorLeft);
            else if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                Console.WriteLine("╒═{0}═╕", '═'.Repeat(longestLineLength));
            foreach (string s in usageLines)
            {
                if (s == "-")
                    Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));
                else if (s == Usage_LineDown || 
                         s == Usage_LineUp || 
                         s == Usage_LineCenter)
                    Console.WriteLine("├─{0}{1}─┤", s, '─'.Repeat(longestLineLength - s.Length));
                else
                    Console.WriteLine("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length));

            }
            Console.WriteLine("╘═{1}{0}═╛", '═'.Repeat(longestLineLength - Usage_End.Length), Usage_End);
        }
    }

    public static class OILexerProgram
    {
        public static ParserBuilderResults CallMainMethod(params string[] args)
        {
            return Program.ProcessArgumentSet(args);
        }
    }
}