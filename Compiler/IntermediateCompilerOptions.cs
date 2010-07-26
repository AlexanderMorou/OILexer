using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Compiler
{
    public class IntermediateCompilerOptions :
        IIntermediateCompilerOptions
    {
        private IList<string> extraFiles;
        /// <summary>
        /// Data member for <see cref="Optimize"/>.
        /// </summary>
        bool optimize;
        /// <summary>
        /// Data member for <see cref="COMVisible"/>.
        /// </summary>
        bool comVisible;
        /// <summary>
        /// Data member for <see cref="AllowUnsafeCode"/>.
        /// </summary>
        bool allowUnsafeCode;
        /// <summary>
        /// Data member for <see cref="GenerateXMLDocs"/>.
        /// </summary>
        bool generateXMLDocs;
        /// <summary>
        /// Data member for <see cref="Target"/>.
        /// </summary>
        string target;
        /// <summary>
        /// Data member for <see cref="InMemory"/>.
        /// </summary>
        bool inMemory;
        /// <summary>
        /// Data member for <see cref="DebugSupport"/>.
        /// </summary>
        DebugSupport debugSupport;

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="target"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="target">The file name for the resulted <see cref="System.Reflection.Assembly"/>.</param>
        public IntermediateCompilerOptions(bool optimize, bool generateXMLDocs, string target)
            : this(optimize, false, false, generateXMLDocs, target, DebugSupport.Full)
        {

        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="target"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="target">The file name for the resulted <see cref="System.Reflection.Assembly"/>.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs, string target)
            : this(optimize, comVisible, allowUnsafeCode, generateXMLDocs, false, DebugSupport.Full)
        {
            this.target = target;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="inMemory"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="inMemory">Whether the resulted assembly is strictly limited to a build in memory.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs, bool inMemory)
            : this(optimize, comVisible, allowUnsafeCode, generateXMLDocs, inMemory, DebugSupport.Full)
        {
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>, and
        /// <paramref name="generateXMLDocs"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs)
            : this(optimize, comVisible, allowUnsafeCode, generateXMLDocs, DebugSupport.Full)
        {
        }
        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="target"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="target">The file name for the resulted <see cref="System.Reflection.Assembly"/>.</param>
        /// <param name="debugSupport">The level of support to give to a debugger.</param>
        public IntermediateCompilerOptions(bool optimize, bool generateXMLDocs, string target, DebugSupport debugSupport)
            : this(optimize, false, false, generateXMLDocs, target, debugSupport)
        {

        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="target"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="target">The file name for the resulted <see cref="System.Reflection.Assembly"/>.</param>
        /// <param name="debugSupport">The level of support to give to a debugger.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs, string target, DebugSupport debugSupport)
            : this(optimize, comVisible, allowUnsafeCode, generateXMLDocs, false, debugSupport)
        {
            this.target = target;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>,
        /// <paramref name="generateXMLDocs"/>, and <paramref name="inMemory"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="inMemory">Whether the resulted assembly is strictly limited to a build in memory.</param>
        /// <param name="debugSupport">The level of support to give to a debugger.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs, bool inMemory, DebugSupport debugSupport)
        {
            this.optimize = optimize;
            this.comVisible = COMVisible;
            this.allowUnsafeCode = allowUnsafeCode;
            this.generateXMLDocs = generateXMLDocs;
            this.inMemory = inMemory;
            this.debugSupport = debugSupport;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCompilerOptions"/> instance with the <paramref name="optimize"/>,
        /// <paramref name="comVisible"/>, <paramref name="allowUnsafeCode"/>, 
        /// <paramref name="generateXMLDocs"/>, and <paramref name="debugSupport"/> provided.
        /// </summary>
        /// <param name="optimize">Whether to optimize the code in the resulted assembly.</param>
        /// <param name="comVisible">Whether the assembly is visible to COM targets.</param>
        /// <param name="allowUnsafeCode">Whether the assembly resulted will allow unsafe code to be
        /// generated.</param>
        /// <param name="generateXMLDocs">Whether to generate XML documentation with the
        /// resulted assembl.</param>
        /// <param name="debugSupport">The level of support to give to a debugger.</param>
        public IntermediateCompilerOptions(bool optimize, bool comVisible, bool allowUnsafeCode,
            bool generateXMLDocs, DebugSupport debugSupport)
            : this(optimize, comVisible, allowUnsafeCode, generateXMLDocs, true, debugSupport)
        {
        }

        #region IIntermediateCompilerOptions Members

        /// <summary>
        /// Returns/sets whether the code should be optimized.
        /// </summary>
        public bool Optimize
        {
            get
            {
                return this.optimize;
            }
            set
            {
                this.optimize = value;
            }
        }

        /// <summary>
        /// Returns/sets whether the resulted assembly is com visible.
        /// </summary>
        public bool COMVisible
        {
            get
            {
                return this.comVisible;
            }
            set
            {
                this.comVisible = value;
            }
        }

        /// <summary>
        /// Returns/sets whether the assembly can have unsafe code.
        /// </summary>
        public bool AllowUnsafeCode
        {
            get
            {
                return this.allowUnsafeCode;
            }
            set
            {
                this.allowUnsafeCode = value;
            }
        }

        /// <summary>
        /// Returns/sets whether the compiler should generate an accompanying XML documentation set.
        /// </summary>
        public bool GenerateXMLDocs
        {
            get
            {
                return this.generateXMLDocs;
            }
            set
            {
                this.generateXMLDocs = value;
            }
        }

        /// <summary>
        /// Returns/sets the assembly target file name.
        /// </summary>
        /// <remarks>If <see cref="InMemory"/> is true, returns null; otherwise the target file.</remarks>
        public string Target
        {
            get
            {
                return this.target;
            }
            set
            {
                if (value == null && !inMemory)
                    inMemory = true;
                else if (inMemory && value != null)
                    inMemory = false;
                this.target = value;
            }
        }

        /// <summary>
        /// Returns/sets whether the target assembly is in-memory only.
        /// </summary>
        public bool InMemory
        {
            get
            {
                return this.inMemory;
            }
            set
            {
                if (this.target != null && value)
                    this.target = null;
                this.inMemory = value;
            }
        }
        /// <summary>
        /// Returns/sets the level of support given to debug output and debuggers in general.
        /// </summary>
        public DebugSupport DebugSupport
        {
            get
            {
                return this.debugSupport;
            }
            set
            {
                this.debugSupport = value;
            }
        }

        public IEnumerable<string> ExtraFiles
        {
            get {
                if (this.extraFiles == null)
                    yield break;
                foreach (var file in extraFiles)
                    yield return file;
                yield break;
            }
        }

        public void AddFile(string file)
        {
            if (this.extraFiles == null)
                this.extraFiles = new List<string>();
            if (extraFiles.Contains(file))
                return;
            extraFiles.Add(file);
        }

        public void RemoveFile(string file)
        {
            if (this.extraFiles == null)
                throw new ArgumentException("file");
            if (this.extraFiles.Contains(file))
                this.extraFiles.Remove(file);
            else
                throw new ArgumentException("file");
        }

        #endregion
    }
}
