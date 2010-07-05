using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.FiniteAutomata.Tokens
{
    /// <summary>
    /// The kind of capture type used by the regular language
    /// state machine.
    /// </summary>
    public enum RegularCaptureType
    {
        /// <summary>
        /// The state machine resulted is a recognizer, and is reduced
        /// as low as possible.
        /// </summary>
        Recognizer,
        /// <summary>
        /// The state machine resulted is capturing state-machine
        /// and is reduced on the left-side only to facilitate
        /// capture group identification.
        /// </summary>
        Capturer,
        /// <summary>
        /// The state machine resulted is a transducer, and is reduced
        /// on the left-side only, creating an optimal state-machine 
        /// whose term can be identified.
        /// </summary>
        Transducer,
        Undecided,
    }
}
