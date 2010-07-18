using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    public abstract class IntermediateCompilerModuleBase :
        IIntermediateCompilerModule
    {

        #region IIntermediateCompilerModule Members

        /// <summary>
        /// Returns the parts of compilation that the <see cref="IIntermediateCompilerModule"/> 
        /// supports.
        /// </summary>
        public abstract CompilerModuleSupportFlags Support { get; }

        /// <summary>
        /// Returns the type of intermediateCompiler module the <see cref="IIntermediateCompilerModule"/> is.
        /// </summary>
        public abstract CompilerModuleType Type { get; }

        public bool Supports(CompilerModuleSupportFlags supportRequest)
        {
            return ((this.Support & supportRequest) == supportRequest);
        }

        #endregion
    }
}
