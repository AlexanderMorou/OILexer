using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Oilexer.FileModel
{
    public class TemporaryFile :
        IDisposable
    {
        /// <summary>
        /// Data member for <see cref="FileName"/>.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Data member for <see cref="Keep"/>.
        /// </summary>
        private bool keep;

        /// <summary>
        /// Data member indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Data member for <see cref="TemporaryFile.FileStream"/>.
        /// </summary>
        private FileStream fileStream;


        /// <summary>
        /// Creates a new instance of <see cref="TemporaryFile"/> given the path,
        /// pattern, and whether to keep the file on disposal..
        /// </summary>
        /// <param name="path">The path the file is contained within.</param>
        /// <param name="pattern"></param>
        /// <param name="keep"></param>
        internal TemporaryFile(string path, string pattern, bool keep) 
            : this(TemporaryFileHelper.GetTemporaryName(path, pattern), keep)
        {

        }
        internal TemporaryFile(string fileName, bool keep)
            : this(fileName, false, keep)
        {
        }
        internal TemporaryFile(string fileName, bool openStream, bool keep)
        {
            this.keep = keep;
            this.fileName = fileName;
            if (openStream)
                if (File.Exists(fileName))
                    this.fileStream = File.Open(fileName, FileMode.Open);
                else
                    this.fileStream = File.Create(fileName);
            else if (!File.Exists(fileName))
                File.Create(fileName).Close();
        }
        ~TemporaryFile()
        {
            this.Dispose();
        }
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.  Deletes the temporary file if it exists,
        /// and <see cref="Keep"/> is false.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                if (this.fileStream != null)
                    this.fileStream.Close();
                disposed = true;
                if (!this.Keep && File.Exists(this.fileName))
                    File.Delete(this.fileName);
                TemporaryFileHelper.TemporaryFiles.Remove(this.fileName);
                this.fileName = null;
            }
        }

        #endregion
        /// <summary>
        /// Returns the file name associated with the <see cref="TemporaryFile"/>.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }
        /// <summary>
        /// Returns/sets whether the file is to be kept upon the <see cref="TemporaryFile"/>'s disposal.
        /// </summary>
        public bool Keep
        {
            get
            {
                return this.keep;
            }
            set
            {
                this.keep = value;
            }
        }
        /// <summary>
        /// Returns <see cref="FileStream"/> created upon the <see cref="TemporaryFile"/>'s creation.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        public FileStream FileStream
        {
            get
            {
                return this.fileStream;
            }
        }

        public TemporaryDirectory Parent
        {
            get
            {
                return TemporaryFileHelper.GetDirectory(Path.GetFullPath(Path.GetDirectoryName(this.fileName)));
            }
        }

        public void OpenStream(FileMode mode)
        {
            if (this.fileStream == null)
                this.fileStream = new FileStream(this.FileName, mode);
        }

        public void CloseStream()
        {
            if (this.fileStream != null)
            {
                this.fileStream.Close();
                this.fileStream = null;
            }
        }
    }
}
