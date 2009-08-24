using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a type-parameter member.
    /// </summary>
    public interface ITypeParameterMember :
        IType,
        IMember,
        IAutoCommentFragmentMember//,
        //IFauxableReliant<Type, Type>
    {
        /// <summary>
        /// Returns the constraints set forth on the <see cref="ITypeParameterMember{TDom, TParent}"/>.
        /// </summary>
        /// <remarks>Due to limitations in the CodeDOM framework, <see cref="ValueType"/> and <see cref="Object"/> as 
        /// 'struct' and 'class' are not valid constraints.</remarks>
        ITypeReferenceCollection Constraints { get; }
        /// <summary>
        /// Returns/sets whether the <see cref="ITypeParameterMember{TDom, TParent}"/> contains the
        /// empty constructor constraint.
        /// </summary>
        /// <remarks><code>new()</code></remarks>
        bool RequiresConstructor { get; set; }
        /// <summary>
        /// Returns/sets the special condition of whether the type-parameter needs to be a 
        /// value type, a class type, or neither.
        /// </summary>
        TypeParameterSpecialCondition SpecialCondition { get; set; }
    }
}