using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Types.Members;
using System.CodeDom.Compiler;

namespace Oilexer.Compiler
{
    public interface IIntermediateCompilerCommandLineModule :
        IIntermediateCompilerModule
    {
        /// <summary>
        /// Obtains the command for referencing a source file.
        /// </summary>
        /// <param name="file">The path to the file.</param>
        /// <returns>A string properly formatted for the command line.</returns>
        string GetSourceCommand(string file, IIntermediateCompilerOptions options);
        string GetOptimizationCommand(bool optimize, IIntermediateCompilerOptions options);
        string GetDebugCommand(DebugSupport support, IIntermediateCompilerOptions options);
        string GetResourceCommand(string resourceFile, IIntermediateCompilerOptions options);
        string GetReferenceCommand(string reference, IIntermediateCompilerOptions options);
        string[] GetSourcesCommand(string[] files, IIntermediateCompilerOptions options);
        string[] GetResourcesCommand(string[] resourceFiles, IIntermediateCompilerOptions options);
        string[] GetReferencesCommand(string[] references, IIntermediateCompilerOptions options);
        string GetTypeCommand(ProjectOutputType outputType, IIntermediateCompilerOptions options);
        string[] GetModuleCommand(IIntermediateModule module, string[] sourceFiles, string[] resourceFiles, IIntermediateCompilerOptions options);
        string GetEntryPointCommand(IMethodMember entrypointMethod, IIntermediateCompilerOptions options);
        string GetSetWarningLevel(CompilerWarnLevel warnLevel, IIntermediateCompilerOptions options);
        string GetBaseAddressCommand(uint baseAddress, IIntermediateCompilerOptions options);
        string GetOutputCommand(string target, IIntermediateCompilerOptions options);
        IIntermediateCompilerResults Compile(string[] commandSequences, string responseFile, TempFileCollection tempFiles, IIntermediateCompilerOptions options);

        string GetXMLDocumentationCommand(bool generateDocs, IIntermediateCompilerOptions options);
    }
}
