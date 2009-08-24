using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Types;
using Oilexer.Types.Members;

namespace Oilexer.Parser.Builder
{
    public interface ITokenDefinition
    {
        /// <summary>
        /// Returns the <see cref="IEnumeratorType"/> which defines the possible cases for a 
        /// <see cref="ITokenDefinition"/>.
        /// </summary>
        IEnumeratorType Cases { get; }
        /// <summary>
        /// Returns the <see cref="IStructType"/> of the <see cref="ITokenDefinition"/> which
        /// contains the data for the token.
        /// </summary>
        IStructType DataForm { get; }
        /// <summary>
        /// Returns the parse method for the <see cref="ITokenDefinition"/>.
        /// </summary>
        /// <returns>A <see cref="IMethodMember"/> relative to the <see cref="ITokenDefinition"/>
        /// which is responsible for resolving the rule.</returns>
        IMethodMember ParseMethod { get; }
        /// <summary>
        /// Returns the rule that declared the <see cref="IProductionRuleRootDefinition"/>.
        /// </summary>
        ITokenEntry RelativeToken { get; }
    }
}
