using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Translation;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Oil;
using System.ComponentModel;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class IntermediateCodeTranslatorOptions :
        IIntermediateCodeTranslatorOptions
    {
        private IControlledStateCollection<IIntermediateDeclaration> _buildTrail;
        private IReadOnlyCollection<IIntermediateDeclaration> buildTrail;

        #region IIntermediateCodeTranslatorOptions Members

        /// <summary>
        /// Returns/sets whether the code translator allows partial instances to be written
        /// to file, or whether the full code is written in one file.
        /// </summary>
        public bool AllowPartials { get; set; }

        /// <summary>
        /// Returns/sets whether the code translator will extend the parent instance's
        /// scope to include the namespace of the type involved based off of the explicit
        /// types involved.
        /// </summary>
        public bool AutoScope { get; set; }

        /// <summary>
        /// Returns the <see cref="IReadOnlyCollection{T}"/> of intermediate
        /// declarations presently being built.
        /// </summary>
        public IReadOnlyCollection<IIntermediateDeclaration> BuildTrail
        {
            get {
                if (this.buildTrail == null)
                    this.buildTrail = new ReadOnlyWrapper<IIntermediateDeclaration>(this._buildTrail);
                return this.buildTrail;
            }
        }

        #endregion


    }
}
