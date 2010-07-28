using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser;
using System.IO;

namespace Oilexer.VSIntegration
{
    internal sealed class GDFileStoredHandler :
        GDFileHandlerBase
    {
        private GDFileBufferedHandler rootHandler;
        public IDictionary<string, GDFileStoredHandler> RelativeScopeFiles { get; private set; }

        public GDFileStoredHandler(string filename, GDFileBufferedHandler rootHandler, IDictionary<string, GDFileStoredHandler> relativeScopeFiles)
        {
            
            this.rootHandler = rootHandler;
            this.RelativeScopeFiles = relativeScopeFiles;
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            StreamReader fileReader = new StreamReader(fileStream);
            base.BufferText = fileReader.ReadToEnd();
            fileReader.Dispose();
            fileStream.Dispose();
            this.SetBuffer();
            ((GDParser.Lexer)this.Parser.CurrentTokenizer).FileName = filename;
        }


        internal void Reparse()
        {
            if (this.CurrentlyParsing)
                return;
            this.CurrentlyParsing = true;
            CurrentParseResults = this.Parser.BeginParse();
            this.CurrentlyParsing = false;
        }
        public bool CurrentlyParsing { get; private set; }

        protected override void PerformReclassification(IGDToken token, Parser.GDTokenType newClassification)
        {
            /* *
             * Reclassifications aren't necessary on the files used to 
             * assist in identity resolution.
             * */
        }

        internal void Dispose()
        {
            this.rootHandler = null;
            this.parser = null;
        }

        protected override void SetBuffer()
        {
            var currentBuffer = new StringStream(this.BufferText);
            Lexer.SetStream(currentBuffer);
        }
    }
}
