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
        private const string Syntax = "-s";
        private const string NoSyntax = "-ns";
        private const string DoNotCompile = "-cmp:no";
        private const string Compile = "-cmp:yes";
        private const string NoLogo = "-nl";
        private const string Quiet = "-q";
        private const string Verbose = "-v";
        private const string StreamAnalysis = "-a:";
        private const string StreamAnalysisExtension = "-ae:";
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

        private static void ParseFile(string file)
        {
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
                DisplayBuildBreakdown(resultsOfBuild);
                if ((options & ValidOptions.VerboseMode) == ValidOptions.VerboseMode)
                {
                    Console.WriteLine("Number of rules: {0}", iprs.Result.GetRules().Count());
                    Console.WriteLine("Number of tokens: {0}", iprs.Result.GetTokens().Count());
                }
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
                if ((options & ValidOptions.ShowSyntax) == ValidOptions.ShowSyntax)
                {
                    Console.Title = string.Format("{0} - Expanded grammar.", baseTitle);
                    foreach (IEntry ie in iprs.Result)
                    {
                        Console.WriteLine(ie);
                        Console.WriteLine();
                    }
                }
                Console.WriteLine("Character set cache size: {0}", RegularLanguageSet.CountComputationCaches());
                Console.WriteLine("Character set computations: {0}", RegularLanguageSet.CountComputations());
                Console.WriteLine("Vocabulary cache size: {0}", GrammarVocabulary.CountComputationCaches());
                Console.WriteLine("Vocabulary set computations: {0}", GrammarVocabulary.CountComputations());
                Console.WriteLine("Parse time: {0}ms", sw.Elapsed);
            }
            Console.Title = string.Format("{0} - {1}", baseTitle, "Finished");
            Console.ReadKey(true);
        }

        private static void DisplayLogo()
        {
            var assembly = typeof(Program).Assembly;
            var name = assembly.GetName();
            var copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).Cast<AssemblyCopyrightAttribute>().First();
            string p = string.Format("OILexer Parser Compiler version {0}", name.Version);
            int pL = p.Length;
            Console.WriteLine(p);
            if (copyright.Copyright.Length > pL)
                pL = copyright.Copyright.Length;
            Console.WriteLine(copyright.Copyright);
            Console.WriteLine('-'.Repeat(pL));
            Console.WriteLine();
        }

        private static void DisplayBuildBreakdown(ParserBuilderResults resultsOfBuild)
        {
            foreach (ParserBuilderPhase phase in resultsOfBuild.PhaseTimes.Keys)
            {
                string op = GetPhaseSubString(phase);
                Console.WriteLine("{0} took: {1}", op, resultsOfBuild.PhaseTimes[phase]);
            }
        }

        private static string GetPhaseSubString(ParserBuilderPhase phase)
        {
            string op = null;

            switch (phase)
            {
                case ParserBuilderPhase.Linking:
                    op = "Linking";
                    break;
                case ParserBuilderPhase.ExpandingTemplates:
                    op = "Expanding templates";
                    break;
                case ParserBuilderPhase.LiteralLookup:
                    op = "Deliteralization";
                    break;
                case ParserBuilderPhase.InliningTokens:
                    op = "Inlining tokens";
                    break;
                case ParserBuilderPhase.TokenNFAConstruction:
                    op = "Token NFA Construction";
                    break;
                case ParserBuilderPhase.TokenDFAConstruction:
                    op = "Token DFA Construction";
                    break;
                case ParserBuilderPhase.TokenDFAReduction:
                    op = "Token DFA Reduction";
                    break;
                case ParserBuilderPhase.RuleNFAConstruction:
                    op = "Rule NFA Construction";
                    break;
                case ParserBuilderPhase.RuleDFAConstruction:
                    op = "Rule DFA Construction";
                    break;
                case ParserBuilderPhase.CallTreeAnalysis:
                    op = "Call Tree Analysis";
                    break;
                case ParserBuilderPhase.ObjectModelRootTypesConstruction:
                    op = "Object Model Construction";
                    break;
                case ParserBuilderPhase.ObjectModelTokenCaptureConstruction:
                    op = "Token Capture Construction";
                    break;
                case ParserBuilderPhase.ObjectModelTokenEnumConstruction:
                    op = "Token Enum Construction";
                    break;
                case ParserBuilderPhase.ObjectModelRuleStructureConstruction:
                    op = "Rule structure construction";
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
                if (relativeRoot != null)
                {
                    if (folder.Path == relativeRoot)
                        Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, @".\", flError);
                    else
                        Console.WriteLine("{0} {2} in folder .{1}", folder.ErrorCount, folder.Path.Substring(relativeRoot.Length), flError);
                }
                else
                    Console.WriteLine("{0} {2} in folder {1}", folder.ErrorCount, folder.Path, flError);
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
            Console.WriteLine("Usage:\r\n\t{0} [options] File [options]", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("\t{0}  - Do not compile", DoNotCompile);
            Console.WriteLine("\t{0} - Compile (default)", Compile);
            Console.WriteLine("");
            Console.WriteLine("\t{0}       - Show syntax.", Syntax);
            Console.WriteLine("\t{0}      - Don't show syntax (default).", NoSyntax);
            Console.ReadKey(true);
        }
    }
}

//FARP: Failed Assertion Return Point.