using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public class EventDeclarationArgs<T> :
        EventArgs
        where T :
            IDeclaration
    {
        /// <summary>
        /// Data member for <see cref="Declaration"/>.
        /// </summary>
        private T declaration;
        /// <summary>
        /// Creates a new instance of <see cref="EventDeclarationArgs{T}"/> with the
        /// <see cref="Declaration"/> provided.
        /// </summary>
        /// <param name="declaration">The <see cref="IDeclaration"/> implementation
        /// instance which is represented by the <see cref="EventDeclarationArgs{T}"/>.</param>
        public EventDeclarationArgs(T declaration)
        {
            this.declaration = declaration;
        }

        /// <summary>
        /// Returns the <see cref="IDeclaration"/> implementation
        /// instance which is represented by the <see cref="EventDeclarationArgs{T}"/>.
        /// </summary>
        public T Declaration
        {
            get
            {
                return this.declaration;
            }
        }

    }
}
