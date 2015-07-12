using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a set of event arguments for the phase
    /// of a parser builder when it changes.
    /// </summary>
    public class ParserBuilderPhaseChangeEventArgs :
        EventArgs
    {

        /// <summary>
        /// Creates a new <see cref="ParserBuilderPhaseChangeEventArgs"/>
        /// with the <paramref name="phase"/> provided.
        /// </summary>
        /// <param name="phase">The <see cref="ParserCompilerPhase"/>
        /// which has been entered.</param>
        public ParserBuilderPhaseChangeEventArgs(ParserCompilerPhase phase)
        {
            this.Phase = phase;
        }

        public ParserCompilerPhase Phase { get; private set; }
    }
}
