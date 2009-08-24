using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;

namespace Oilexer
{
    public class StringFormWriter :
        IndentedTextWriter
    {
        private MemoryStream msSelf;
        private StreamReader msReader;
        public StringFormWriter():
            base(InitializeWriter())
        {
            msSelf = ((MemoryStream)((StreamWriter)base.InnerWriter).BaseStream);
            msReader = new StreamReader(msSelf);
        }

        private static TextWriter InitializeWriter()
        {
            StreamWriter sw = new StreamWriter(new MemoryStream());
            sw.AutoFlush = true;
            return sw;
        }

        public override string ToString()
        {
            msSelf.Seek(0, SeekOrigin.Begin);
            string result  = msReader.ReadToEnd();
            msSelf.Seek(msSelf.Length, SeekOrigin.End);
            return result;
        }

        public void Clear()
        {
            msSelf.SetLength(0);
            msSelf.Seek(0, SeekOrigin.Begin);
        }
    }
}
