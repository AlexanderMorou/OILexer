using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public enum EntryScanMode
    {
        /// <summary>
        /// The rule/token is allowed to span the same as the calling rule.
        /// </summary>
        Inherited,
        /// <summary>
        /// The rule/token is allowed to span multiple lines.
        /// </summary>
        Multiline,
        /// <summary>
        /// The rule/token is allowed to span a single line.
        /// </summary>
        SingleLine
    }
}
