using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Translation
{
    partial class CodeTranslationOptions
    {
        /// <summary>
        /// A passive name handler that yields what it's given.
        /// </summary>
        protected class PassiveNameHandler :
            ICodeGeneratorNameHandler
        {
            #region ICodeDOMGeneratorNameHandler Members

            /// <summary>
            /// Returns whether the <see cref="PassiveNameHandler"/> will process the name
            /// based upon the type of declaration and/or its active name value.
            /// </summary>
            /// <param name="declaredMember">The member that needs its name/type/etc checked.</param>
            /// <returns>true if the name is to be handled, false otherwise.</returns>
            public bool HandlesName(IDeclaration declaredMember)
            {
                return false;
            }
            /// <summary>
            /// Returns the new name of the <paramref name="declaredMember"/>.
            /// </summary>
            /// <param name="declaredMember">The member that needs its name translated.</param>
            /// <returns>A string containing the translated form of  
            /// <paramref name="declaredMember"/>'s name</returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="declaredMember"/> is null.
            /// </exception>
            public string HandleName(IDeclaration declaredMember)
            {
                if (declaredMember == null)
                    throw new ArgumentNullException("declaredMember");
                return declaredMember.Name;
            }

            /// <summary>
            /// Returns whether the <see cref="PassiveNameHandler"/> will process the name
            /// based upon the name alone.
            /// </summary>
            /// <param name="name">The declaration-less name to determine if it's handled.</param>
            /// <returns>false.</returns>
            public bool HandlesName(string name)
            {
                return false;
            }

            /// <summary>
            /// Returns whether the <see cref="PassiveNameHandler"/> will process the name
            /// based upon the type of declaration and/or its active name value.
            /// </summary>
            /// <param name="declaredMember">The member that needs its name/type/etc checked.</param>
            /// <returns>false.</returns>
            public string HandleName(string name)
            {
                return name;
            }

            #endregion
        }
    }
}
