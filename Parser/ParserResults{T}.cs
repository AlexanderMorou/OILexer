using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Parser
{
    public class ParserResults<T> :
        IParserResults<T>
    {
        private T result;
        private CompilerErrorCollection errors;
        protected ParserResults()
        {
        }

        #region IParserResults<T> Members

        public T Result
        {
            get { return this.result; }
            protected set { this.result = value; }
        }

        #endregion

        #region IParserResults Members

        public CompilerErrorCollection Errors
        {
            get {
                if (this.errors == null)
                    this.errors = new CompilerErrorCollection();
                return this.errors; }
        }

        object IParserResults.Result
        {
            get { return this.result; }
        }

        public bool Successful
        {
            get { return !this.Errors.HasErrors; }
        }

        #endregion
    }
}
