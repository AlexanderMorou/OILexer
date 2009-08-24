using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oilexer.FileModel
{
    public partial class TemporaryDirectory : 
        IDisposable
    {
        /// <summary>
        /// Data member for <see cref="disposed"/>.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Data member for <see cref="Path"/>.
        /// </summary>
        private string path;

        /// <summary>
        /// Data member for <see cref="Keep"/>.
        /// </summary>
        private bool keep;


        /// <summary>
        /// Creates a new instance of <see cref="TemporaryDirectory"/> with the path and 
        /// whether to keep it afterwards.
        /// </summary>
        /// <param name="path">The location the <see cref="TemporaryPath"/> is set to.</param>
        /// <param name="keep">Whether to keep the <see cref="TemporaryPath"/> upon disposal.</param>
        internal TemporaryDirectory(string path, bool keep)
        {
            if (path.Substring(path.Length - 1) != @"\")
                path += @"\";
            this.path = path;
            this.keep = keep;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        ~TemporaryDirectory()
        {
            this.Dispose();
        }

        /// <summary>
        /// Returns whether the <see cref="TemporaryDirectory"/> is to be kept upon disposal.
        /// </summary>
        public bool Keep
        {
            get
            {
                return this.keep;
            }
        }

        /// <summary>
        /// Returns the path that the <see cref="TemporaryDirectory"/> exists at.
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public DirFiles Files
        {
            get
            {
                return new DirFiles(this);
            }
        }

        public SubDirectories Directories
        {
            get
            {
                return new SubDirectories(this);
            }
        }

        public TemporaryDirectory Parent
        {
            get
            {

                return TemporaryFileHelper.GetDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(this.path))));
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.  Deletes the temporary directory, and all its contents 
        /// if it exists, and <see cref="Keep"/> is false.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                if (!this.Keep && Directory.Exists(this.path))
                    try
                    {
                        Directory.Delete(this.path, true);
                    }
                    catch
                    {
                    }
                TemporaryFileHelper.TemporaryPaths.Remove(this.path);
                this.path = null;
            }
        }

        #endregion

    }
}
