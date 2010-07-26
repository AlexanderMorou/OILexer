using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace Oilexer.FileModel
{
    partial class TemporaryDirectory
    {
        public class DirFiles : ICollection<TemporaryFile>
        {
            /// <summary>
            /// Data member which determines the path to iterate.
            /// </summary>
            private string path;
            private bool keep;

            internal DirFiles(TemporaryDirectory dirInst)
            {
                this.path = dirInst.path;
                this.keep = dirInst.keep;
            }


            #region IEnumerable<TemporaryFile> Members

            public IEnumerator<TemporaryFile> GetEnumerator()
            {
                string[] files = Directory.GetFiles(this.path);
                foreach (string file in files)
                    yield return TemporaryFileHelper.GetFile(System.IO.Path.GetFullPath(file), this.keep);
            }

            #endregion

            public TemporaryFile GetTemporaryFile(string name)
            {
                if (name == "." || name.Contains(".."))
                    throw new ArgumentException("Cannot refer to temp dir or parent.", "name");
                if (name.Contains(@"\") || name.Contains("/"))
                    throw new ArgumentException("Cannot be more than one level deep.", "name");
                return TemporaryFileHelper.GetFile(TemporaryFileHelper.GetTemporaryName(path, name), keep);
            }

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region ICollection<TemporaryFile> Members

            void ICollection<TemporaryFile>.Add(TemporaryFile item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TemporaryFile>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TemporaryFile>.Contains(TemporaryFile item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");
                return ((System.IO.Path.GetFullPath(item.Parent.Path)) == (System.IO.Path.GetFullPath(path)));
            }

            void ICollection<TemporaryFile>.CopyTo(TemporaryFile[] array, int arrayIndex)
            {

                string[] files = Directory.GetFiles(this.path);
                int arrayOffset = arrayIndex;
                foreach (string file in files)
                    array[arrayOffset++] = TemporaryFileHelper.GetFile(System.IO.Path.GetFullPath(file));
            }

            int ICollection<TemporaryFile>.Count
            {
                get { return Directory.GetFiles(System.IO.Path.GetFullPath(this.path)).Length; }
            }

            bool ICollection<TemporaryFile>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TemporaryFile>.Remove(TemporaryFile item)
            {
                throw new NotSupportedException();
            }

            #endregion

        }
    }
}
