using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    [Serializable]
    public class DeclarationChangeArgs :
        EventArgs
    {
        /// <summary>
        /// Data member for <see cref="Cancel"/>.
        /// </summary>
        private bool cancel = false;
        /// <summary>
        /// Data member for <see cref="Declaration"/>.
        /// </summary>
        private IDeclaration declaration;
        /// <summary>
        /// Creates a new instance of <see cref="DeclarationChangeArgs"/> with the
        /// <paramref name="declaration"/> provided.
        /// </summary>
        /// <param name="declaration">The declaration the args refer to.</param>
        public DeclarationChangeArgs(IDeclaration declaration)
        {
            this.declaration = declaration;
        }
        /// <summary>
        /// Returns the <see cref="IDeclaration"/> implementation that the 
        /// <see cref="DeclarationChangeArgs"/> indicate a change was made on.
        /// </summary>
        public IDeclaration Declaration
        {
            get
            {
                return this.declaration;
            }
        }

        /// <summary>
        /// Returns/sets whether to cancel the change operation.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }
    }
}
