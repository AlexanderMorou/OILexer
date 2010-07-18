using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace Oilexer.FileModel
{
    partial class TemporaryDirectory
    {
        public class SubDirectories :
            ICollection<TemporaryDirectory>
        {
            /// <summary>
            /// Data member which determines the path to iterate.
            /// </summary>
            private string path;

            internal SubDirectories(TemporaryDirectory dirInst)
            {
                this.path = dirInst.path;
            }


            public TemporaryDirectory GetTemporaryDirectory(string name)
            {
                if (name == "." || name.Contains(".."))
                    throw new ArgumentException("Cannot refer to temp dir or parent.", "name");
                if (name.Contains(@"\") || name.Contains("/"))
                    throw new ArgumentException("Cannot be more than one level deep.", "name");
                return TemporaryFileHelper.GetDirectory(this.path +  name);
            }

            #region IEnumerable<TemporaryDirectory> Members

            public IEnumerator<TemporaryDirectory> GetEnumerator()
            {
                string[] files = Directory.GetDirectories(this.path);
                foreach (string file in files)
                    yield return TemporaryFileHelper.GetDirectory(System.IO.Path.GetFullPath(file));
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region ICollection<TemporaryDirectory> Members

            void ICollection<TemporaryDirectory>.Add(TemporaryDirectory item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TemporaryDirectory>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TemporaryDirectory>.Contains(TemporaryDirectory item)
            {
                if (item == null)
                    throw new ArgumentNullException("series");
                return ((System.IO.Path.GetFullPath(item.Parent.Path)) == (System.IO.Path.GetFullPath(path)));
            }

            void ICollection<TemporaryDirectory>.CopyTo(TemporaryDirectory[] array, int arrayIndex)
            {

                string[] files = Directory.GetDirectories(this.path);
                int arrayOffset = arrayIndex;
                foreach (string file in files)
                    array[arrayOffset++] = TemporaryFileHelper.GetDirectory(System.IO.Path.GetFullPath(file));
            }

            int ICollection<TemporaryDirectory>.Count
            {
                get { return Directory.GetDirectories(System.IO.Path.GetFullPath(this.path)).Length; }
            }

            bool ICollection<TemporaryDirectory>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TemporaryDirectory>.Remove(TemporaryDirectory item)
            {
                throw new NotSupportedException();
            }

            #endregion

        }
    }
}
