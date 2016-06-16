using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
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
        /// <summary>
        /// The state machine resulted in a transducer which needs to
        /// be contextually aware of which elements are valid at a given
        /// point, essentially making each end-point a language symbol of its
        /// own.
        /// </summary>
        ContextfulTransducer,
        /// <summary>
        /// The state of the regular capture type hasn't been decided yet.
        /// </summary>
        Undecided,
    }
}
