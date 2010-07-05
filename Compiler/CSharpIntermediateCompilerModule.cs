using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Utilities.Arrays;
using Oilexer.FileModel;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using Oilexer.Types;
using System.IO;
using Oilexer.Utilities.Collections;
using Oilexer._Internal;

namespace Oilexer.Compiler
{
    public class CSharpIntermediateCompilerModule :
        IntermediateCompilerCommandLineModule
    {
        public static readonly string OptimizeCommand = "/optimize{0}";
        public static readonly string DebugCommand = "/debug{0}";
        public static readonly string DebugNone = string.Format(DebugCommand, "-");
        public static readonly string DebugFull = string.Format(DebugCommand, ":full");
        public static readonly string DebugPDBOnly = string.Format(DebugCommand, ":pdbonly");
        public static readonly string EntryPointCommand = "/main:{0}";
        public static readonly string OutputCommand = "/out:\"{0}\"";
        public static readonly string OutputType = "/target:{0}";
        public static readonly string OutputClassLibrary = string.Format(OutputType, "library");
        public static readonly string OutputConsoleApp = string.Format(OutputType, "exe");
        public static readonly string OutputModule = string.Format(OutputType, "module");
        public static readonly string OutputWindowsApp = string.Format(OutputType, "winexe");
        public static readonly string ReferenceCommand = "/reference:\"{0}\"";
        public static readonly string WarnLevelCommand = "/warn:{0}";
        public static readonly string XMLDocCommand = "/doc:\"{0}\"";
        private static readonly Regex CSCResultRegex = new Regex(@"(^([^)]+)(\([0-9]+,[0-9]+\))?: )?(warning|error) ([A-Z]+[0-9]+)?: (.*)");
        public override string GetSourceCommand(string file, IIntermediateCompilerOptions options)
        {
            return string.Format("\"{0}\"", file);
        }

        public override string GetOptimizationCommand(bool optimize, IIntermediateCompilerOptions options)
        {
            if (optimize)
                return string.Format(OptimizeCommand, "+");
            else
                return string.Format(OptimizeCommand, "-");
        }

        public override string GetDebugCommand(DebugSupport support, IIntermediateCompilerOptions options)
        {
            switch (support)
            {
                case DebugSupport.None:
                    return DebugNone;
                case DebugSupport.PDB:
                    return DebugPDBOnly;
                case DebugSupport.Full:
                    return DebugFull;
            }
            return string.Empty;
        }

        public override string GetResourceCommand(string resourceFile, IIntermediateCompilerOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetReferenceCommand(string reference, IIntermediateCompilerOptions options)
        {
            return string.Format(ReferenceCommand, reference);
        }

        public override string GetTypeCommand(ProjectOutputType outputType, IIntermediateCompilerOptions options)
        {
            switch (outputType)
            {
                case ProjectOutputType.ClassLibrary:
                    return OutputClassLibrary;
                case ProjectOutputType.ConsoleApplication:
                    return OutputConsoleApp;
                case ProjectOutputType.Module:
                    return OutputModule;
                case ProjectOutputType.WindowsApplication:
                    return OutputWindowsApp;
            }
            return string.Empty;
        }

        public override string[] GetModuleCommand(IIntermediateModule module, string[] sourceFiles, string[] resourceFiles, IIntermediateCompilerOptions options)
        {
            string target = options.Target;
            if (module != module.Project.RootModule)
                target = string.Format("{0}.{1}.part", target.Substring(0, target.LastIndexOf('.')), module.Name);
            return Tweaks.MergeArrays<string>(new string[] { GetTypeCommand(module != module.Project.RootModule ? ProjectOutputType.Module : module.Project.OutputType, options), GetOutputCommand(target, options) }, GetSourcesCommand(sourceFiles, options));
            //return string.Format("{0} {1} {2}", , string.Join(" ", ));
        }

        public override string GetOutputCommand(string target, IIntermediateCompilerOptions options)
        {
            return string.Format(OutputCommand, target);
        }

        public override IIntermediateCompilerResults Compile(string[] commandSequences, string responseFile, TempFileCollection temporaryFiles, IIntermediateCompilerOptions options)
        {
            IntermediateCompilerResults results = new IntermediateCompilerResults();
            results.TemporaryFiles = temporaryFiles;
            results.CommandLine = string.Join(" ", commandSequences);
            string outputText = string.Empty;
            string errorText = string.Empty;
            results.NativeReturnValue = Executor.ExecWaitWithCapture(string.Format("{0}csc.exe /noconfig @\"{1}\"", _OIL._Core.GetRuntimeDirectory(), responseFile), temporaryFiles, ref outputText, ref errorText);
            return results;
        }

        public override CompilerModuleSupportFlags Support
        {
            get { return CompilerModuleSupportFlags.FullSupport; }
        }

        public override string GetEntryPointCommand(IMethodMember entrypointMethod, IIntermediateCompilerOptions options)
        {
            if (entrypointMethod == null)
                throw new ArgumentNullException("entrypointMethod");
            return string.Format(EntryPointCommand,((IDeclaredType)entrypointMethod.ParentTarget).GetTypeName(CodeGeneratorHelper.DefaultTranslatorOptions));
        }

        public override string GetSetWarningLevel(CompilerWarnLevel warnLevel, IIntermediateCompilerOptions options)
        {
            byte warnID = 0;
            switch (warnLevel)
            {
                case CompilerWarnLevel.Level1:
                    warnID = 1;
                    break;
                case CompilerWarnLevel.Level2:
                    warnID = 2;
                    break;
                case CompilerWarnLevel.Level3:
                    warnID = 3;
                    break;
                case CompilerWarnLevel.Level4:
                    warnID = 4;
                    break;
            }
            return string.Format(WarnLevelCommand, warnID.ToString());
        }

        public override string GetBaseAddressCommand(uint baseAddress, IIntermediateCompilerOptions options)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetXMLDocumentationCommand(bool generateDocs, IIntermediateCompilerOptions options)
        {
            if (!generateDocs)
                return string.Empty;
            string output = options.Target;

            if (output.Contains("."))
            {
                output = string.Format("{0}.xml", output.Substring(0, output.LastIndexOf('.')));
            }
            else
            {
                output = string.Format("{0}.xml", output);
            }
            return string.Format(XMLDocCommand, output);
        }
    }
}