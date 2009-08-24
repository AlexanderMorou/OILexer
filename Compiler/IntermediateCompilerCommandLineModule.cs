using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Utilities.Arrays;
using System.CodeDom.Compiler;

namespace Oilexer.Compiler
{
    public abstract class IntermediateCompilerCommandLineModule :
        IntermediateCompilerModuleBase,
        IIntermediateCompilerCommandLineModule
    {
        public override CompilerModuleType Type
        {
            get { return CompilerModuleType.CommandLine; }
        }

        #region IIntermediateCompilerModuleCommandLine Members

        public abstract string GetSourceCommand(string file, IIntermediateCompilerOptions options);

        public abstract string GetOptimizationCommand(bool optimize, IIntermediateCompilerOptions options);

        public abstract string GetDebugCommand(DebugSupport support, IIntermediateCompilerOptions options);

        public abstract string GetResourceCommand(string resourceFile, IIntermediateCompilerOptions options);

        public abstract string GetReferenceCommand(string reference, IIntermediateCompilerOptions options);

        public virtual string[] GetSourcesCommand(string[] files, IIntermediateCompilerOptions options)
        {
            return Tweaks.ProcessArray(files, delegate(string f)
            {
                return GetSourceCommand(f, options);
            });
        }

        public virtual string[] GetResourcesCommand(string[] resourceFiles, IIntermediateCompilerOptions options)
        {
            return Tweaks.ProcessArray(resourceFiles, delegate(string rF)
            {
                return GetReferenceCommand(rF, options);
            });
        }

        public virtual string[] GetReferencesCommand(string[] references, IIntermediateCompilerOptions options)
        {
            return Tweaks.ProcessArray(references, delegate(string r)
            {
                return GetReferenceCommand(r, options);
            });
        }

        public abstract string GetTypeCommand(ProjectOutputType outputType, IIntermediateCompilerOptions options);

        public abstract string[] GetModuleCommand(IIntermediateModule module, string[] sourceFiles, string[] resourceFiles, IIntermediateCompilerOptions options);

        public abstract string GetOutputCommand(string target, IIntermediateCompilerOptions options);

        public abstract IIntermediateCompilerResults Compile(string[] commandSequences, string responseFile, TempFileCollection tempFiles, IIntermediateCompilerOptions options);

        public abstract string GetEntryPointCommand(IMethodMember entrypointMethod, IIntermediateCompilerOptions options);

        public abstract string GetSetWarningLevel(CompilerWarnLevel warnLevel, IIntermediateCompilerOptions options);

        public abstract string GetBaseAddressCommand(uint baseAddress, IIntermediateCompilerOptions options);

        public abstract string GetXMLDocumentationCommand(bool generateDocs, IIntermediateCompilerOptions options);

        #endregion
    }
}
