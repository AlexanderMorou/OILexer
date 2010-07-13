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
        private const string DoNotCompile = "-cmp:no";
        private const string Compile = "-cmp:yes";
        private const string NoLogo = "-nl";
        private const string Quiet = "-q";
        private const string Verbose = "-v";
        private const string StreamAnalysis = "-a:";
        private const string StreamAnalysisExtension = "-ae:";
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
            /// Compiles the langauge grammar into a library.
            /// </summary>
            Compile                 = 0x0001,
            /// <summary>
            /// Displays the language's sytnax at the end.
            /// </summary>
            ShowSyntax              = 0x0002,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not emit 
            /// the syntax at the end.
            /// </summary>
            DoNotEmitSyntax         = 0x0004,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not compile 
            /// at the end.
            /// </summary>
            DoNotCompile            = 0x0008,
            NoLogo                  = 0x0020,
            /// <summary>
            /// Instructs the <see cref="Program"/> to emit as little
            /// as possible to the console.
            /// </summary>
            QuietMode               = 0x0030,
            VerboseMode             = 0x0040,
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
                Console.Clear();
                Console.Title = string.Format("{0}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
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
                    else if (s.ToLower() == DoNotCompile)
                        options = (options & ~ValidOptions.Compile) | ValidOptions.DoNotCompile;
                    else if (s.ToLower() == Compile)
                        options = (options & ~ValidOptions.DoNotCompile) | ValidOptions.Compile;
                    else if (s.ToLower() == Syntax)
                        options = (options & ~ValidOptions.DoNotEmitSyntax) | ValidOptions.ShowSyntax;
                    else if (s.ToLower() == NoLogo)
                        options = options | ValidOptions.NoLogo;
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
                Console.Title = consoleTitle;
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
            }
            catch (IOException) 
            {
                /* *
                 * Clear not available.
                 * Cases: Application type changed.
                 *        Console output redirected to something else, eg. file.
                 * */
            }
            Console.Title = string.Format("{0} Parsing...", baseTitle);
            if ((options & ValidOptions.NoLogo) != ValidOptions.NoLogo)
                DisplayLogo();
            sw.Start();
            iprs = gp.Parse(file);
            var tLenMax = (from e in iprs.Result
                           let token = e as ITokenEntry
                           where token != null
                           select token.Name.Length).Max();

            maxLength = Math.Max(maxLength, tLenMax);
            int oldestLongest = longestLineLength;
            longestLineLength = Math.Max(longestLineLength, maxLength + 19);
            if ((options & ValidOptions.NoLogo) == ValidOptions.None)
                FinishLogo(oldestLongest, Console.CursorTop, Console.CursorLeft);
            else
                Console.WriteLine("╒═{0}═╕", '═'.Repeat(longestLineLength));
            sw.Stop();
            if (iprs.Successful)
                baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(string.IsNullOrEmpty(iprs.Result.Options.AssemblyName) ? iprs.Result.Options.ParserName : iprs.Result.Options.AssemblyName));
            Console.Title = baseTitle;

            /* *
             * If the parser succeeds, build the project and check 
             * potential errors again.
             * */
            if (iprs.Successful)
            {
                Console.Title = string.Format("{0} Linking project...", baseTitle);
                //igdb.BuildPhaseChange += new EventHandler<BuildUpdateEventArgs>(igdb_BuildPhaseChange);
                ParserBuilderResults resultsOfBuild = Build(iprs);
                if (resultsOfBuild == null)
                    goto errorChecker;
                if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
                {
                    const string stateMachineCounts = "State machine state counts:";
                    Console.WriteLine("│ {0}{1} │", stateMachineCounts, ' '.Repeat(longestLineLength - stateMachineCounts.Length));
                    var toks = (from t in iprs.Result.GetTokens()
                                let state = t.DFAState
                                where state != null
                                let stateCount = t.DFAState.CountStates()
                                orderby stateCount
                                select new { Name = t.Name, StateCount = stateCount, NameLength = t.Name.Length}).ToArray();
                    foreach (var token in toks)
                    {
                        var s = string.Format("{2}{0} : {1}", token.Name, token.StateCount, ' '.Repeat(maxLength - token.NameLength));
                        Console.WriteLine("│ {0}{1} │", s, ' '.Repeat(longestLineLength-s.Length));
                    }
                    Console.WriteLine("├─{0}─┤", '─'.Repeat(longestLineLength));

                }
                DisplayBuildBreakdown(resultsOfBuild, maxLength);
            errorChecker:
                if (resultsOfBuild == null || resultsOfBuild.Project == null)
                {
                    /* *
                     * The builder encountered an error not immediately obvious by linking.
                     * */
                    Console.Title = string.Format("{0} could not build project...", baseTitle);
                    goto __CheckErrorAgain;
                }

                if ((options & ValidOptions.DoNotCompile) == ValidOptions.None)
                {
                    Console.Title = string.Format("{0} Compiling project...", baseTitle);
                    string rootPath = string.Empty;
                    foreach (var cFile in iprs.Result.Files)
                    {
                        string bPath = Path.GetDirectoryName(cFile);
                        if (bPath.Length < rootPath.Length)
                            rootPath = bPath;
                    }
                    resultsOfBuild.Project.AssemblyInformation.Company = "None";
                    resultsOfBuild.Project.AssemblyInformation.AssemblyName = iprs.Result.Options.AssemblyName;
                    /* *
                     * Culture specifier here.
                     * */
                    resultsOfBuild.Project.AssemblyInformation.Culture = CultureIdentifiers.English_UnitedStates;
                    resultsOfBuild.Project.AssemblyInformation.Description = string.Format("Language parser for {0}.", iprs.Result.Options.GrammarName);
                    resultsOfBuild.Project.Attributes.AddNew(typeof(AssemblyVersionAttribute).GetTypeReference(), new AttributeConstructorParameter(new PrimitiveExpression("1.0.0.*")));
                    rootPath += string.Format("{0}.dll", iprs.Result.Options.AssemblyName);
                    IIntermediateCompiler iic = new CSharpIntermediateCompiler(resultsOfBuild.Project, new IntermediateCompilerOptions(true, true, rootPath, DebugSupport.None));
                    iic.Translator.Options.AutoResolveReferences = true;
                    iic.Translator.Options.AllowPartials = false;
                    iic.Translator.Options.AutoComments = true;
                    Stopwatch compileTimer = new Stopwatch();
                    compileTimer.Start();
                    IIntermediateCompilerResults iicrs = iic.Compile();
                    compileTimer.Stop();
                    Console.WriteLine("Compile time: {0}", compileTimer.Elapsed);
                    iicrs.TemporaryFiles.KeepFiles = true;
                    if ((options & ValidOptions.QuietMode) != ValidOptions.QuietMode)
                    {
                        if (iicrs.NativeReturnValue == 0)
                            Console.WriteLine("Compile Successful");
                        else
                            Console.WriteLine("Compile Failed.");
                    }
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
                Console.WriteLine("╘═{0}═╛", '═'.Repeat(longestLineLength));
                if ((options & ValidOptions.ShowSyntax) == ValidOptions.ShowSyntax)
                {
                    Console.Title = string.Format("{0} - Expanded grammar.", baseTitle);
                    ShowSyntax(iprs);
                }

            }
            Console.Title = string.Format("{0} - {1}", baseTitle, "Finished");
            Console.ReadKey(true);
        }

        private static void FinishLogo(int oldestLongest, int cy, int cx)
        {
            Console.CursorTop = cy - 3;
            Console.CursorLeft = cx;
            Console.WriteLine("{0}═╕", '═'.Repeat(longestLineLength - oldestLongest));
            Console.CursorTop = cy - 2;
            Console.CursorLeft = cx;
            Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - oldestLongest));
            Console.CursorTop = cy - 1;
            Console.CursorLeft = cx;
            Console.WriteLine("{0} │", ' '.Repeat(longestLineLength - oldestLongest));
            Console.CursorTop = cy;
            Console.CursorLeft = cx;
            Console.WriteLine("{0}─┤", '─'.Repeat(longestLineLength - oldestLongest));
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
            var parts = (from f in files
                         let ePath = Path.GetDirectoryName(f)
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
                if (iprs.Result.Files.All(p => p.Contains(currentRoot)))
                {
                    relativeRoot = currentRoot;
                    break;
                }
            }

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
            foreach (var folder in folderEntries)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                string rPath = null;
                if (relativeRoot != null)
                {
                    if (folder.Path == relativeRoot)
                        rPath = @".\";
                    else
                        rPath = string.Format(".{0}", folder.Path.Substring(relativeRoot.Length));
                }
                else
                    rPath = folder.Path;
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
                    {
                        //Console.ForegroundColor = ConsoleColor.DarkGreen;
                        //Console.WriteLine("/*Line: {0}*/", entry.Line);
                        //Console.ForegroundColor = consoleOriginal;
                        Console.WriteLine(entry);
                        Console.WriteLine();
                    }
                }
            }
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
            Console.Write("├─{0}", '─'.Repeat(longestLineLength));
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
            ParserBuilderResults resultsOfBuild = iprs.Result.Build(iprs.Errors, phase =>
            {
                Console.Title = string.Format("{0} - {1}...", Program.baseTitle, GetPhaseSubString(phase));
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
            const string Usage_DoNotCompile = "    " + DoNotCompile + "  - Do not compile.";
            const string Usage_Compile = "    " + Compile + " - Compile (default)";
            const string Usage_Syntax = "    " + Syntax + "       - Show syntax.";
            const string Usage_NoSyntax = "    " + NoSyntax + "      - Don't show syntax (default).";
            const string Usage_Usage = "Usage:";
            string Usage_TagLine = string.Format("    {0} [options] File [options]", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
            string[] usageLines = new string[] {
                Usage_Usage,
                Usage_TagLine,
                string.Empty,
                Usage_Options,
                Usage_DoNotCompile,
                Usage_Compile,
                string.Empty,
                Usage_Syntax,
                Usage_NoSyntax
            };
            int oldMaxLongestLength = longestLineLength;
            longestLineLength = usageLines.Max(p => p.Length);
            if ((options & ValidOptions.NoLogo) == ValidOptions.None)
                FinishLogo(oldMaxLongestLength, Console.CursorTop, Console.CursorLeft);
            else
                Console.WriteLine("╒═{0}═╕", '═'.Repeat(longestLineLength));
            foreach (string s in usageLines)
                Console.WriteLine("│ {0}{1} │", s, ' '.Repeat(longestLineLength - s.Length));
            Console.WriteLine("╘═{0}═╛", '═'.Repeat(longestLineLength));
            Console.ReadKey(true);
        }
    }
}

//FARP: Failed Assertion Return Point.