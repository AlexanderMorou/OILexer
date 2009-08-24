using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.FileModel;

namespace Oilexer.Compiler
{
    public interface IIntermediateCompilerCallbackModule :
        IIntermediateCompilerModule
    {
        IIntermediateCompilerModuleActionResult PrepareResources(IReadOnlyCollection<TemporaryFile> resources);
        IIntermediateCompilerModuleActionResult PrepareSource(IReadOnlyCollection<TemporaryFile> files);
        IIntermediateCompilerModuleActionResult PrepareKey(TemporaryFile pfxFile);
        IIntermediateCompilerResults Compile();
    }
}
