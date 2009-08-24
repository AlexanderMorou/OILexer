using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Diagnostics;
using System.Reflection;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class IndexerParameterMember :
        ParameteredParameterMember<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>,
        IIndexerParameterMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="IndexerParameterMember"/>
        /// with the parameter type, name and target provided.
        /// </summary>
        /// <param name="nameAndType">The type and name of the parameter.</param>
        /// <param name="parentTarget">The place the parameter exists on.</param>
        public IndexerParameterMember(TypedName nameAndType, IIndexerMember parentTarget)
            : base(nameAndType, parentTarget)
        {
        }
        public override FieldDirection Direction
        {
            get
            {
                return base.Direction;
            }
            [DebuggerHidden()]
            set
            {
                if (value != FieldDirection.In)
                    throw new InvalidOperationException("'ref' and 'out' are invalid on indexer parameters.");
                base.Direction = value;
            }
        }
#if false
        #region IFauxableReliant<ParameterInfo,Type> Members

        public ParameterInfo GetFauxCast(Type parentFaux)
        {
            
        }

        #endregion
#endif
    }
}
