using AllenCopeland.Abstraction.Slf.FiniteAutomata;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface INamedFiniteAutomataSource :
        IFiniteAutomataSource
    {
        /// <summary>
        /// Returns the name of the <see cref="INamedFiniteAutomataSource"/>, if it was defined.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        string Name { get; }
    }
    public interface INamedTokenSource :
        INamedFiniteAutomataSource,
        ITokenSource
    {

    }
    public interface INamedProductionRuleSource :
        INamedFiniteAutomataSource,
        IProductionRuleSource
    {

    }
}
