using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Oilexer.Compiler;
using Oilexer.Expression;
using Oilexer.Parser;
using Oilexer.Parser.Builder;
using Oilexer.Parser.GDFileData;
using Oilexer.Types;
namespace Oilexer
{
    internal static class Program
    {
        private const string NoSyntax = "-ns";
        private const string DoNotCompile = "-cmp:no";
        private const string Compile = "-cmp:yes";
        /// <summary>
        /// Defines the valid options for the <see cref="Program"/>.
        /// </summary>
        [Flags]
        private enum ValidOptions
        {
            /// <summary>
            /// No options were specified.
            /// </summary>
            None,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not emit 
            /// the syntax at the end.
            /// </summary>
            DoNotEmitSyntax = 1,
            /// <summary>
            /// Instructs the <see cref="Program"/> to not compile 
            /// at the end.
            /// </summary>
            DoNotCompile = 2,
        }
        public static string baseTitle;
        [STAThread]
        static void Main(string[] args)
        {
            ValidOptions options = ValidOptions.None;
            Console.Title = string.Format("{0}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
            if (args.Length <= 0)
            {
                Program.DisplayUsage();
                return;
            }
            bool exists = false;
            string file = null;
            foreach (string s in args)
                if (s.ToLower() == NoSyntax)
                    options |= ValidOptions.DoNotEmitSyntax;
                else if (s.ToLower() == DoNotCompile)
                    options |= ValidOptions.DoNotCompile;
                else if (s.ToLower() == Compile)
                    options &= ~ValidOptions.DoNotCompile;
                //Ignored rule, redefined default.
                else if (!File.Exists(s))
                    Console.WriteLine("File {0} does not exist.", s);
                else if (file == null)
                {
                    exists = true;
                    file = s;
                }

            if (!exists)
            {
                Program.DisplayUsage();
                return;
            }
            Program.ParseFile(file, options);
        }

        private static void ParseFile(string file, ValidOptions options)
        {
            baseTitle = string.Format("{0}: {1}", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), Path.GetFileName(file));
            Console.Title = baseTitle;
            GDParser gp = new GDParser();
            IGDBuilder igdb = new GDBuilder();
            Stopwatch sw = new Stopwatch();
            IParserResults<IGDFile> iprs = null;
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }
            Console.Title = string.Format("{0} Parsing...", baseTitle);
            Console.WriteLine("Beginning parse.");
            sw.Start();
            iprs = gp.Parse(file);
            sw.Stop();
            /* *
             * If the parser succeeds, build the project and check 
             * potential errors again.
             * */
            if (iprs.Successful)
            {
                Stopwatch buildTime = new Stopwatch();
                buildTime.Start();
                Console.Title = string.Format("{0} Building project...", baseTitle);
                IIntermediateProject iip = igdb.Build(iprs);
                buildTime.Stop();
                Console.WriteLine("Structure build time: {0}", buildTime.Elapsed);
                if (iip == null)
                {
                    /* *
                     * The builder encountered an error not immediately obvious by linking.
                     * */
                    Console.Title = string.Format("{0} could not build project...", baseTitle);
                    goto __CheckErrorAgain;
                }
                if ((options & ValidOptions.DoNotCompile) == ValidOptions.None)
                {
                    //
                    Console.Title = string.Format("{0} Compiling project...", baseTitle);
                    string rootPath = string.Empty;
                    foreach (var cFile in iprs.Result.Files)
                    {
                        string bPath = Path.GetDirectoryName(cFile);
                        if (bPath.Length < rootPath.Length)
                            rootPath = bPath;
                    }
                    iip.AssemblyInformation.Company = "None";
                    iip.AssemblyInformation.AssemblyName = iprs.Result.Options.AssemblyName;
                    /* *
                     * Culture specifier here.
                     * */
                    iip.AssemblyInformation.Culture = CultureIdentifiers.English_UnitedStates;
                    iip.AssemblyInformation.Description = string.Format("Language parser for {0}.", iprs.Result.Options.GrammarName);
                    iip.Attributes.AddNew(typeof(AssemblyVersionAttribute).GetTypeReference(), new AttributeConstructorParameter(new PrimitiveExpression("1.0.0.*")));
                    rootPath += string.Format("{0}.dll", iprs.Result.Options.AssemblyName);
                    IIntermediateCompiler iic = new CSharpIntermediateCompiler(iip, new IntermediateCompilerOptions(true, true, rootPath, DebugSupport.None));
                    iic.Translator.Options.AutoResolveReferences = true;
                    iic.Translator.Options.AutoComments = true;
                    IIntermediateCompilerResults iicrs = iic.Compile();
                    iicrs.TemporaryFiles.KeepFiles = true;
                    if (iicrs.NativeReturnValue == 0)
                        Console.WriteLine("Compile Successful");
                    else
                        Console.WriteLine("Compile Failed.");
                }
                goto ShowParseTime;
            __CheckErrorAgain:
                if (iprs.Errors.HasErrors)
                    Program.ShowErrors(iprs);
            }
            else
                Program.ShowErrors(iprs);
        ShowParseTime:
            if ((options & ValidOptions.DoNotEmitSyntax) == ValidOptions.None)
            {
                Console.Title = string.Format("{0} - Expanded grammar.", baseTitle);
                foreach (IEntry ie in iprs.Result)
                {
                    Console.WriteLine(ie);
                    Console.WriteLine();
                }
            }
            Console.WriteLine("Parse time: {0}ms", sw.Elapsed);
            Console.Title = string.Format("{0} - {1}", baseTitle, "Finished");
            Console.ReadKey(true);
        }

        private static void ShowErrors(IParserResults<IGDFile> iprs)
        {
            long size = 0;
            foreach (string s in iprs.Result.Files)
                size += new FileInfo(s).Length;
            var errors2 = from CompilerError e in iprs.Errors
                          group e by Path.GetDirectoryName(e.FileName);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            List<string> erroredFiles = new List<string>();
            var color = Console.ForegroundColor;
            foreach (var ceSet in errors2)
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
                var setSubQuery = from ce in ceSet
                                  group ce by Path.GetFileName(ce.FileName);
                Console.WriteLine("{0} Errors in folder {1}", ceSet.Count(), ceSet.Key);
                foreach (var ceSubSet in setSubQuery)
                {
                    Console.ForegroundColor = errorMColor;
                    Console.WriteLine("\t{0} Errors in file {1}:", ceSubSet.Count(), ceSubSet.Key);
                    foreach (var ce in ceSubSet)
                    {
                        if (!ce.IsWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("\t\t{0} - {{", ce.ErrorNumber);
                        Console.ForegroundColor = posColor;
                        Console.Write("{0}:{1}", ce.Line, ce.Column);
                        if (!ce.IsWarning)
                            Console.ForegroundColor = errorColor;
                        else
                            Console.ForegroundColor = warnColor;
                        Console.Write("}} - {0}", ce.ErrorText);
                        Console.WriteLine();
                        if (!erroredFiles.Contains(ce.FileName))
                            erroredFiles.Add(ce.FileName);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = color;
            Console.WriteLine("There were {0} errors in {1} files.", iprs.Errors.Count, erroredFiles.Count);
            Console.WriteLine("Total of {0} bytes parsed from {1} files.", size, iprs.Result.Files.Count);
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Usage:\r\n\t{0} [options] File [options]", Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location));
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("\t{0}  - Do not compile", DoNotCompile);
            Console.WriteLine("\t{0} - Compile (default)", Compile);
            Console.WriteLine("");
            Console.WriteLine("\t{0}      - Don't emit syntax.", NoSyntax);
            Console.ReadKey(true);
        }
    }
}
