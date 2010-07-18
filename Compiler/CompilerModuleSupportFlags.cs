using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    /// <summary>
    /// The various *support flags for intermediate -> code -> *intermediateCompiler module -> MSIL.
    /// </summary>
    [Flags]
    public enum CompilerModuleSupportFlags :
        short
    {
        /// <summary>
        /// Compiler module supports generation of a documentation file 
        /// from documentation comments.
        /// </summary>
        XMLDocumentation = 1,
        /// <summary>
        /// Compiler module supports optimization.
        /// </summary>
        Optimization = 2,
        /// <summary>
        /// Compiler module supports unsafe code which is not CLSCompliant.
        /// </summary>
        Unsafe = 4,
        /// <summary>
        /// Compiler module supports COM Interop.
        /// </summary>
        COMInterop = 8,
        /// <summary>
        /// Compiler module supports the define directive.
        /// </summary>
        Define = 16,
        /// <summary>
        /// Compiler module supports .NET Resources.
        /// </summary>
        Resources = 32,
        /// <summary>
        /// Compiler module supports win32 resources.
        /// </summary>
        Win32Resources = 64,
        /// <summary>
        /// Compiler module supports signing the assembly.
        /// </summary>
        Signing = 128,
        /// <summary>
        /// Compiler module supports creation of multi-file assemblies.
        /// </summary>
        MultiFileAssemblies=256,
        /// <summary>
        /// Compiler module supports libraries with full debugger support.
        /// </summary>
        DebuggerSupport = 512,
        /// <summary>
        /// Compiler supports using a response file for longer command
        /// sequences.
        /// </summary>
        ResponseFile = 1024,
        FullSupport = XMLDocumentation 
                    | Optimization 
                    | DebuggerSupport 
                    | Unsafe 
                    | COMInterop 
                    | Define
                    | Resources | Win32Resources 
                    | Signing 
                    | MultiFileAssemblies
                    | ResponseFile
    }
}
