using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a builder
    /// construct which takes a given <typeparamref name="TInput"/>
    /// and yields a <typeparamref name="TOutput"/>.
    /// </summary>
    /// <typeparam name="TInput">The type of input used by the builder.</typeparam>
    /// <typeparam name="TOutput">The type of output constructed by the builder.</typeparam>
    public interface IConstructBuilder<TInput, TOutput>
    {
        TOutput Build(TInput input);
    }
}
