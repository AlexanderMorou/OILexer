using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    public interface IIntermediateCompilerOptions 
    {
        /// <summary>
        /// Returns/sets whether the code should be optimized.
        /// </summary>
        bool Optimize { get; set; }
        /// <summary>
        /// Returns/sets whether the resulted assembly is com visible.
        /// </summary>
        bool COMVisible { get; set; }
        /// <summary>
        /// Returns/sets whether the assembly can have unsafe code.
        /// </summary>
        bool AllowUnsafeCode { get; set; }
        /// <summary>
        /// Returns/sets whether the compiler should generate an accompanying XML documentation set.
        /// </summary>
        bool GenerateXMLDocs { get; set; }
        /// <summary>
        /// Returns/sets the assembly target file name.
        /// </summary>
        /// <remarks>If <see cref="InMemory"/> is true, returns null; otherwise the target file.</remarks>
        string Target { get; set; }
        /// <summary>
        /// Returns/sets whether the target assembly is in-memory only.
        /// </summary>
        bool InMemory { get; set; }
        /// <summary>
        /// Returns/sets the level of support given to debug output.
        /// </summary>
        DebugSupport DebugSupport { get; set; }
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of the 
        /// extra files to include in the compile process.
        /// </summary>
        IEnumerable<string> ExtraFiles { get; }
        void AddFile(string file);
        void RemoveFile(string file);
        bool KeepTempFiles { get; set; }
    }
}
