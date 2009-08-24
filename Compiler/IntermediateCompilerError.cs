using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Compiler
{
    public class IntermediateCompilerError :
        CompilerError,
        IIntermediateCompilerError
    {
        
        public IntermediateCompilerError()
            : base()
        {
        }
        public IntermediateCompilerError(string fileName, int line, int column, string errorNumber, string errorText)
            : base(fileName, line, column, errorNumber, errorText)
        {
        }

    }
}
