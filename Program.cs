using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Oilexer._Internal;
using Oilexer.Parser.Builder;
using Oilexer.Compiler;
using Oilexer.Expression;
using Oilexer.FiniteAutomata;
using Oilexer.FiniteAutomata.Tokens;
using Oilexer.Parser;
using Oilexer.Parser.GDFileData;
using Oilexer.Types;
using Oilexer.Utilities.Arrays;
using Oilexer.Utilities.Collections;
using Oilexer.FiniteAutomata.Rules;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal.Inlining;
using Oilexer.Translation;
/* *
 * Old Release Post-build command:
 * "$(ProjectDir)PostBuild.bat" "$(ConfigurationName)" "$(TargetPath)"
 * */
namespace Oilexer
{
    /// <summary>
    /// Provides an entrypoint and initial decision logic
    /// for the application.
    /// </summary>
    internal static class Program
    {
        private static int longestLineLength = 0;
        private const string Syntax = "-s";
        private const string NoSyntax = "-ns";
        private const string NoLogo = "-nl";
        private const string Quiet = "-q";
        private const string Verbose = "-v";
        private const string StreamAnalysis = "-a:";
        private const string StreamAnalysisExtension = "-ae:";
        private const string Export = "-ex:";
        private const string ExportKind_TraversalHTML = "t-html";
        private const string ExportKind_DLL = "dll";
        private const string ExportKind_EXE = "exe";
        private const string ExportKind_CSharp = "cs";
        private const string Export_TraversalHTML = Export + ExportKind_TraversalHTML;
        private const string Export_DLL = Export + ExportKind_DLL;
        private const string Export_EXE = Export + ExportKind_EXE;
        private const string Export_CSharp = Export + ExportKind_CSharp;
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
            /// files associated to the current grammar.
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
        public static string baseTitle;
        public static ValidOptions options = ValidOptions.DoNotEmitSyntax;
        /// <summary>
        /// The entrypoint.
        /// </summary>
        /// <param name="args">The string parameters sent in by the
        /// call site.</param>
        private static void Main(string[] args)
        {
            //Console.BackgroundColor = ConsoleColor.DarkBlue;
            //Console.ForegroundColor = ConsoleColor.White;
            var consoleTitle = Console.Title;
            try
            {
                try
                {
                    Console.Clear();
                    Console.Title = string.Format("{0}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
                }
                catch (IOException)
                {
                } 
                if (args.Length <= 0)
                {
                    if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                        DisplayLogo();
                    Program.DisplayUsage();
                    return;
                }
                bool exists = false;
                string file = null;
                string extension = null;
                foreach (string s in args)
                    if (s.ToLower() == NoSyntax)
                        options = (options & ~ValidOptions.ShowSyntax) | ValidOptions.DoNotEmitSyntax;
                    else if (s.ToLower() == Syntax)
                        options = (options & ~ValidOptions.DoNotEmitSyntax) | ValidOptions.ShowSyntax;
                    else if (s.ToLower() == NoLogo)
                        options = options | ValidOptions.NoLogo;
                    else if (s.ToLower() == Export_TraversalHTML)
                    {
                        options &= ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportCSharp);
                        options |= ValidOptions.ExportTraversalHTML;
                    }
                    else if (s.ToLower() == Export_DLL)
                    {
                        options &= ~(ValidOptions.ExportEXE | ValidOptions.ExportTraversalHTML | ValidOptions.ExportCSharp);
                        options |= ValidOptions.ExportDLL;
                    }
                    else if (s.ToLower() == Export_EXE)
                    {
                        options &= ~(ValidOptions.ExportTraversalHTML | ValidOptions.ExportDLL | ValidOptions.ExportCSharp);
                        options |= ValidOptions.ExportEXE;
                    }
                    else if (s.ToLower() == Export_CSharp)
                    {
                        options &= ~(ValidOptions.ExportEXE | ValidOptions.ExportDLL | ValidOptions.ExportTraversalHTML);
                        options |= ValidOptions.ExportCSharp;
                    }
                    else if (s.ToLower() == Quiet)
                        options = (options & ~ValidOptions.VerboseMode) | ValidOptions.QuietMode;
                    else if (s.ToLower() == Verbose)
                        options = (options & ~ValidOptions.QuietMode) | ValidOptions.VerboseMode;
                    //Ignored rule, redefined default.
                    else if (s.ToLower().Substring(0, StreamAnalysis.Length) == StreamAnalysis)
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
                    else if (s.ToLower().Substring(0, StreamAnalysisExtension.Length) == StreamAnalysisExtension)
                        extension = s.ToLower().Substring(StreamAnalysisExtension.Length);
                    else if (!File.Exists(s))
                        Console.WriteLine("File {0} does not exist.", s);
                    else if (file == null)
                    {
                        exists = true;
                        file = s;
                    }
                if (!string.IsNullOrEmpty(extension))
                {
                    List<string> streamAnalysisFiles = new List<string>();
                    foreach (var analysisFile in StreamAnalysisFiles)
                        if (analysisFile.EndsWith(extension))
                            streamAnalysisFiles.Add(analysisFile);
                    StreamAnalysisFiles = streamAnalysisFiles;
                }
                if (!exists)
                {
                    if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                        DisplayLogo();
                    Program.DisplayUsage();
                    return;
                }
                Program.ParseFile(file);
            }
            finally
            {
                try
                {
                    Console.Title = consoleTitle;
                }
                catch (IOException) {
                }
            }
        }

        private static void ParseFile(string file)
        {
            int maxLength = new int[] { TitleSequence_CharacterSetCache.Length, TitleSequence_CharacterSetComputations.Length, TitleSequence_VocabularyCache.Length, TitleSequence_VocabularyComputations.Length, TitleSequence_NumberOfRules.Length, TitleSequence_NumberOfTokens.Length, PhaseName_Linking.Length, PhaseName_ExpandingTemplates.Length, PhaseName_Deliteralization.Length, PhaseName_InliningTokens.Length, PhaseName_TokenNFAConstruction.Length, PhaseName_TokenDFAConstruction.Length, PhaseName_TokenDFAReduction.Length, PhaseName_RuleNFAConstruction.Length, PhaseName_RuleDFAConstruction.Length, PhaseName_CallTreeAnalysis.Length, PhaseName_ObjectModelConstruction.Length, PhaseName_TokenCaptureConstruction.Length, PhaseName_TokenEnumConstruction.Length, PhaseName_RuleStructureConstruction.Length }.Max();
            baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(file));
            Console.Title = baseTitle;
            GDParser gp = new GDParser();
            //GDBuilder igdb = new GDBuilder();
            Stopwatch sw = new Stopwatch();
            IParserResults<IGDFile> iprs = null;
            try
            {
                Console.Clear();
                Console.Title = string.Format("{0} Parsing...", baseTitle);
            }
            catch (IOException) 
            {
                /* *
                 * Clear not available.
                 * Cases: Application type changed.
                 *        Console output redirected to something else, eg. file.
                 * */
            }
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                DisplayLogo();
            sw.Start();
            iprs = gp.Parse(file);
            var tLenMax = (from e in iprs.Result
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
            sw.Stop();
            if (iprs.Successful)
                baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(string.IsNullOrEmpty(iprs.Result.Options.AssemblyName) ? iprs.Result.Options.ParserName : iprs.Result.Options.AssemblyName));
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
            if (iprs.Successful)
            {
                try
                {
                    Console.Title = string.Format("{0} Linking project...", baseTitle);
                }
                catch (IOException) { }
                ParserBuilderResults resultsOfBuild = Build(iprs);
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
                    var toks = (from t in iprs.Result.GetTokens()
                                let state = t.DFAState
                                where state != null
                                let stateCount = t.DFAState.CountStates()
                                select new { Name = t.Name, StateCount = stateCount, IsToken = true}).ToArray();
                    var rules = (from rule in iprs.Result.GetRules()
                                 let state = resultsOfBuild.RuleStateMachines.ContainsKey(rule) ? resultsOfBuild.RuleStateMachines[rule] : null
                                 where state != null
                                 let stateCount = state.CountStates()
                                 select new { Name = rule.Name, StateCount = stateCount, IsToken=false}).ToArray();
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
                if (resultsOfBuild == null || resultsOfBuild.Project == null)
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
                    SetAttributes(iprs, resultsOfBuild);
                    ProjectTranslator.WriteProject(resultsOfBuild.Project, ProjectTranslator.GetRelativeRoot(iprs.Result.Files), true, true, IntermediateCodeTranslator.HTMLFormatter, ".html", "&nbsp;".Repeat(4));
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
                    foreach (var cFile in iprs.Result.Files)
                    {
                        string bPath = Path.GetDirectoryName(cFile);
                        if (bPath.Length < rootPath.Length)
                            rootPath = bPath;
                    }
                    SetAttributes(iprs, resultsOfBuild);
                    rootPath += string.Format("{0}.dll", iprs.Result.Options.AssemblyName);
                    IIntermediateCompiler intermediateCompiler = new CSharpIntermediateCompiler(resultsOfBuild.Project, new IntermediateCompilerOptions(rootPath, true, generateXMLDocs: true, debugSupport: DebugSupport.None));
                    intermediateCompiler.Translator.Options.AutoResolveReferences = true;
                    intermediateCompiler.Translator.Options.AllowPartials = false;
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
                }
                goto ShowParseTime;
            __CheckErrorAgain:
                if (iprs.Errors.HasErrors)
                    Program.ShowErrors(iprs);
            }
            else
                Program.ShowErrors(iprs);
        ShowParseTime:
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
            {
                Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));
                DisplayTailInformation(maxLength, iprs);
                Console.WriteLine("╘═{0}═╛", '═'.Repeat(longestLineLength));
            }
            if ((options & ValidOptions.ShowSyntax) == ValidOptions.ShowSyntax)
            {
                try
                {
                    Console.Title = string.Format("{0} - Expanded grammar.", baseTitle);
                }
                catch (IOException) { }
                ShowSyntax(iprs);
            
            }
            try
            {
                Console.Title = string.Format("{0} - {1}", baseTitle, "Finished");
            }
            catch (IOException) { }
            Console.ReadKey(true);
        }

        private static void SetAttributes(IParserResults<IGDFile> iprs, ParserBuilderResults resultsOfBuild)
        {
            resultsOfBuild.Project.AssemblyInformation.Company = "None";
            resultsOfBuild.Project.AssemblyInformation.AssemblyName = iprs.Result.Options.AssemblyName;
            /* *
             * Culture specifier here.
             * */
            resultsOfBuild.Project.AssemblyInformation.Culture = CultureIdentifiers.English_UnitedStates;
            resultsOfBuild.Project.AssemblyInformation.Description = string.Format("Language parser for {0}.", iprs.Result.Options.GrammarName);
            resultsOfBuild.Project.Attributes.AddNew(typeof(AssemblyVersionAttribute).GetTypeReference(), new AttributeConstructorParameter(new PrimitiveExpression("1.0.0.*")));
        }

        private static void DisplayTailInformation(int maxLength, IParserResults<IGDFile> iprs)
        {
            var sequence = (
                new
                {
                    Title = TitleSequence_CharacterSetComputations,
                    Value = RegularLanguageSet.CountComputations()
                }).GetArray(
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
            if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
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
            string relativeRoot = ProjectTranslator.GetRelativeRoot(files);

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
                string rPath = ProjectTranslator.GetExtensionFromRelativeRoot(folder.Path, relativeRoot);
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
         * */
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
            if (!startingLine)
            {
                if (needLine)
                    Console.WriteLine();
            }
            else
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
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(charItem.Literal.Value.ToString().Encode());
            Console.ForegroundColor = consoleColor;
        }

        private static void DisplaySyntax(ILiteralStringReferenceProductionRuleItem charItem, ref bool startingLine, int depth)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            if (startingLine)
            {
                Console.Write(' '.Repeat((depth + 1) * 4));
                startingLine = false;
            }
            Console.Write(charItem.Literal.Value.Encode());
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
            }
            return op;
        }

        private static ParserBuilderResults Build(IParserResults<IGDFile> iprs)
        {
            ParserBuilderResults resultsOfBuild = iprs.Result.Build(StreamAnalysisFiles, iprs.Errors, phase =>
            {
                try
                {
                    Console.Title = string.Format("{0} - {1}...", Program.baseTitle, GetPhaseSubString(phase));
                }
                catch (IOException) { }
            });
            return resultsOfBuild;
        }

        private static void ShowErrors(IParserResults<IGDFile> iprs)
        {
            long size = (from s in iprs.Result.Files
                         select new FileInfo(s).Length).Sum();

            var iprsSorted =
                (from CompilerError ce in iprs.Errors
                 orderby ce.Line,
                         ce.ErrorText
                 select ce).ToArray();
            int largestColumn = iprsSorted.Max(p => p.Column).ToString().Length;
            int furthestLine = iprsSorted.Max(p => p.Line).ToString().Length;

            string[] parts = (from CompilerError e in iprs.Errors
                              let ePath = Path.GetDirectoryName(e.FileName)
                              orderby ePath.Length descending
                              select ePath).First().Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            /* *
             * Breakdown the longest filename and rebuild it part by part,
             * where all elements from the error set contain the current path,
             * select that path, then select the longest common path.
             * *
             * If the longest file doesn't contain anything in common, then there
             * is no group relative path and the paths will be shown in full.
             * */
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
            /* *
             * Change was made to the sequence of LINQ expressions due to their
             * readability versus traditional methods of a myriad of dictionaries.
             * */
            var fileQuery =
                (from CompilerError error in iprsSorted
                 select error.FileName).Distinct();

            var folderQuery =
                (from file in fileQuery
                 select Path.GetDirectoryName(file)).Distinct();

            var fileErrorQuery =
                (from file in fileQuery
                 join CompilerError error in iprsSorted on file equals error.FileName into fileErrors
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
                //Error color used for denoting the specific error.
                const ConsoleColor errorColor = ConsoleColor.DarkRed;
                //Used for the string noting there were errors.
                const ConsoleColor errorMColor = ConsoleColor.Red;
                //Warning color.
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
                        if (!ce.IsWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("\t\t{0} - {{", ce.ErrorNumber);
                        Console.ForegroundColor = posColor;
                        int l = ce.Line, c = ce.Column;
                        l = furthestLine - l.ToString().Length;
                        c = largestColumn - c.ToString().Length;
                        Console.Write("{2}{0}:{1}{3}", ce.Line, ce.Column, ' '.Repeat(l), ' '.Repeat(c));
                        if (!ce.IsWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("}} - {0}", ce.ErrorText);
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = color;
            int totalErrorCount = iprs.Errors.Count,
                totalFileCount  = folderErrors.Sum(folderData => folderData.FileCount);
            Console.WriteLine("There were {0} {2} in {1} {3}.", totalErrorCount, totalFileCount, totalErrorCount == 1 ? "error" : "errors", totalFileCount == 1 ? "file" : "files");
            Console.WriteLine("A total of {0:#,#} bytes were parsed from {1} files.", size, iprs.Result.Files.Count);
        }

        private static void DisplayUsage()
        {
            const string Usage_Options = "options:";
            const string Usage_Export = "    " + Export + "kind; Export, where kind is:";
            //const string Usage_ExportKindIs = "      Kind is: │       ";
            const string Usage_Export_Exe = "           " + ExportKind_EXE + " │ Executable";
            const string Usage_Export_Dll = "           " + ExportKind_DLL + " │ Dynamic link library";
            const string Usage_Export_CSharp = "            " + ExportKind_CSharp + " │ CSharp Code";
            const string Usage_Export_TraversalHTML = "        " + ExportKind_TraversalHTML + " │ Traversable HTML";
            const string Usage_Syntax = "    " + Syntax + "         │ Show syntax.";
            const string Usage_NoSyntax = "    " + NoSyntax + "*       │ Don't show syntax";
            const string Usage_NoLogo = "    " + NoLogo + "        │ Do not show logo";
            const string Usage_Verbose = "    " + Verbose + "         │ Verbose mode";
            const string Usage_QuietMode = "    " + Quiet + "         │ Quiet mode";
            const string Usage_LineCenter = "───────────────┼";
            const string Usage_LineDown = "───────────────┬";
            const string Usage_LineUp = "───────────────┴";
            const string Usage_End = "═══════════════╧";
            const string Usage_Default = "     *         │ default";
            const string Usage_Usage = "Usage:";
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
                Usage_Export_TraversalHTML,
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
            Console.ReadKey(true);
        }
    }
}

//FARP: Failed Assertion Return Point.