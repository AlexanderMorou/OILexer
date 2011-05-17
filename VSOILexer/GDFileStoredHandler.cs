using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal sealed class GDFileStoredHandler :
        GDFileHandlerBase
    {
        private GDFileBufferedHandler rootHandler;
        public IDictionary<string, GDFileHandlerBase> RelativeScopeFiles { get; private set; }

        public GDFileStoredHandler(string filename, GDFileBufferedHandler rootHandler, IDictionary<string, GDFileHandlerBase> relativeScopeFiles)
        {
            
            this.rootHandler = rootHandler;
            this.RelativeScopeFiles = relativeScopeFiles;
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader fileReader = new StreamReader(fileStream);
            base.BufferText = fileReader.ReadToEnd();
            fileReader.Dispose();
            fileStream.Dispose();
            this.SetBuffer();
            ((OILexerParser.Lexer)this.Parser.CurrentTokenizer).FileName = filename;
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

        protected override void PerformReclassification(IGDToken token, GDTokenType newClassification)
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
