using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.FileModel;
using System.Reflection;
using Oilexer.Utilities.Collections;
using System.CodeDom.Compiler;

namespace Oilexer.Compiler
{
    /// <summary>
    /// The results of a compile operation.
    /// </summary>
    public sealed class IntermediateCompilerResults :
        IIntermediateCompilerResults
    {
        private string commandLine;
        /// <summary>
        /// Data member for <see cref="Assembly"/>.
        /// </summary>
        private Assembly assembly;
        /// <summary>
        /// Data member for <see cref="nativeReturnValue"/>.
        /// </summary>
        private int nativeReturnValue;
        /// <summary>
        /// Data member for <see cref="TemporaryFiles"/>.
        /// </summary>
        private TempFileCollection temporaryFiles;
        /// <summary>
        /// Data member for <see cref="Errors"/>.
        /// </summary>
        private IReadOnlyCollection<IIntermediateCompilerError> errors;
        /// <summary>
        /// Data member for <see cref="Warnings"/>.
        /// </summary>
        private IReadOnlyCollection<IIntermediateCompilerError> warnings;

        internal IntermediateCompilerResults(Assembly assembly, int nativeReturnValue, TempFileCollection temporaryFiles, IIntermediateCompilerError[] errors)
        {
            ProcessErrorArray(errors);
            this.assembly = assembly;
            this.nativeReturnValue = nativeReturnValue;
            this.temporaryFiles = temporaryFiles;
        }

        internal IntermediateCompilerResults()
        {
        }

        internal void ProcessErrorArray(IIntermediateCompilerError[] errors)
        {
            if (errors != null)
            {
                List<IIntermediateCompilerError> warningsCopy = new List<IIntermediateCompilerError>();
                List<IIntermediateCompilerError> errorsCopy = new List<IIntermediateCompilerError>();
                foreach (IIntermediateCompilerError error in errors)
                    if (error.IsWarning)
                        warningsCopy.Add(error);
                    else
                        errorsCopy.Add(error);
                this.errors = new ReadOnlyCollection<IIntermediateCompilerError>(errorsCopy);
                this.warnings = new ReadOnlyCollection<IIntermediateCompilerError>(warningsCopy);
            }
        }

        #region IIntermediateCompilerResults Members

        /// <summary>
        /// Returns the resulted assembly from the compile operation.
        /// </summary>
        public Assembly ResultedAssembly
        {
            get {
                return this.assembly;
            }
        }

        /// <summary>
        /// Returns the native return value from the compiler.
        /// </summary>
        public int NativeReturnValue
        {
            get { return this.nativeReturnValue; }
            internal set { this.nativeReturnValue = value; }
        }

        /// <summary>
        /// Returns the <see cref="TemporaryDirectory"/> that the files were stored in
        /// during the compile process.
        /// </summary>
        public TempFileCollection TemporaryFiles
        {
            get { return this.temporaryFiles; }
            internal set { this.temporaryFiles = value; }
        }

        /// <summary>
        /// Returns the series of <see cref="IIntermediateCompilerErrors"/> that occurred.
        /// </summary>
        public IReadOnlyCollection<IIntermediateCompilerError> Errors
        {
            get { return this.errors; }
        }

        /// <summary>
        /// Returns the series of <see cref="IIntermediateCompilerErrors"/>, as warnings, that occurred.
        /// </summary>
        public IReadOnlyCollection<IIntermediateCompilerError> Warnings
        {
            get { return this.warnings; }

        }

        /// <summary>
        /// Returns whether there was an error during the compilation process.
        /// </summary>
        public bool HasErrors
        {
            get {
                if (this.Errors == null)
                    return false;
                return this.Errors.Count > 0;
            }
        }

        /// <summary>
        /// Returns whether there was any warnings during the compilation process.
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                if (this.Warnings == null)
                    return false;
                return this.Warnings.Count > 0;
            }
        }


        public string CommandLine
        {
            get { return this.commandLine; }
            internal set
            {
                this.commandLine = value;
            }
        }

        #endregion
    }
}
