using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Types;
using Oilexer.Types.Members;

namespace Oilexer.Parser.Builder
{
    public class TokenDefinition :
        ITokenDefinition
    {
        /// <summary>
        /// Data member for <see cref="RelativeToken"/>.
        /// </summary>
        private ITokenEntry relativeToken;
        private IEnumeratorType cases;
        private IMethodMember parseMethod;
        private IStructType dataForm;

        public TokenDefinition(ITokenEntry relativeToken)
        {
            this.relativeToken = relativeToken;
        }
        #region ITokenDefinition Members

        public IEnumeratorType Cases
        {
            get { return this.cases; }
            internal set { this.cases = value; }
        }

        public IStructType DataForm
        {
            get { return this.dataForm; }
            internal set { this.dataForm = value; }
        }

        public IMethodMember ParseMethod
        {
            get
            {
                return this.parseMethod;
            }
            internal set
            {
                this.parseMethod = value;
            }
        }

        public ITokenEntry RelativeToken
        {
            get { return this.relativeToken; }
        }

        #endregion
    }
}
