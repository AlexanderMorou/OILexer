using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Expression;

namespace Oilexer.Translation
{
    /// <summary>
    /// Defines properties/methods for working with a name handler for the generation process.
    /// </summary>
    public interface ICodeGeneratorNameHandler

    {
        /// <summary>
        /// Returns whether the <see cref="ICodeDOMGeneratorNameHandler"/> will process the name
        /// based upon the type of declaration and/or its active name value.
        /// </summary>
        /// <param name="declaredMember">The member that needs its name/type/etc checked.</param>
        /// <returns>true if the name is to be handled, false otherwise.</returns>
        bool HandlesName(IDeclaration declaredMember);
        /// <summary>
        /// Returns whether the <see cref="ICodeDOMGeneratorNameHandler"/> will process the name
        /// based upon the name alone.
        /// </summary>
        /// <param name="name">The declaration-less name to determine if it's handled.</param>
        /// <returns>true if the name is to be handled, false otherwise.</returns>
        bool HandlesName(string name);
        /// <summary>
        /// Returns the new name of the <paramref name="declaredMember"/>.
        /// </summary>
        /// <param name="declaredMember">The member that needs its name translated.</param>
        /// <returns>A string containing the translated form of the 
        /// <paramref name="declaredMember"/>'s name</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="declaredMember"/> is null.
        /// </exception>
        string HandleName(IDeclaration declaredMember);
        /// <summary>
        /// Returns the new value of <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name that needs translated.</param>
        /// <returns>A string containing the translated form of <paramref name="name"/>.</returns>
        string HandleName(string name);

    }
}
