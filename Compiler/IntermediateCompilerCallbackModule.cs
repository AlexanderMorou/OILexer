using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.FileModel;
using Oilexer.Utilities.Collections;

namespace Oilexer.Compiler
{
    public abstract class IntermediateCompilerCallbackModule :
        IntermediateCompilerModuleBase,
        IIntermediateCompilerCallbackModule
    {
        #region IIntermediateCompilerModuleCallback Members

        public abstract IIntermediateCompilerModuleActionResult PrepareResources(IReadOnlyCollection<TemporaryFile> resources);

        public abstract IIntermediateCompilerModuleActionResult PrepareSource(IReadOnlyCollection<TemporaryFile> files);

        public abstract IIntermediateCompilerModuleActionResult PrepareKey(TemporaryFile pfxFile);

        public abstract IIntermediateCompilerResults Compile();

        #endregion


        public override CompilerModuleType Type
        {
            get { return CompilerModuleType.Callback; }
        }
    }
}
