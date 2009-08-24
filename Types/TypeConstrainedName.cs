using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Utilities.Arrays;

namespace Oilexer.Types
{
    /// <summary>
    /// A type constrained name used to simplify generation of common elements.
    /// </summary>
    public struct TypeConstrainedName
    {
        #region TypeConstrainedName Data members

        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="TypeReferences"/>.
        /// </summary>
        private ITypeReference[] typeReferences;
        /// <summary>
        /// Data member for <see cref="RequiresConstructor"/>.
        /// </summary>
        private bool requiresConstructor;

        TypeParameterSpecialCondition specialCondition;
        #endregion

        #region TypeConstrainedName Constructors
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name) :
            this(name, false, TypeParameterSpecialCondition.None, new ITypeReference[0])
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor)
            : this(name, requiresConstructor, TypeParameterSpecialCondition.None, new ITypeReference[0])
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="ITypeReference"/>s 
        /// which denote the constraints set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, params ITypeReference[] typeReferences)
            : this(name, false, TypeParameterSpecialCondition.None, typeReferences)
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="IType"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, params  IType[] typeReferences)
            : this(name, false, TypeParameterSpecialCondition.None, typeReferences)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="Type"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, params Type[] typeReferences)
            : this(name, false, TypeParameterSpecialCondition.None, typeReferences)
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="requiresConstructor"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="Type"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, params Type[] typeReferences)
            : this(name, requiresConstructor, TypeParameterSpecialCondition.None, typeReferences)
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>,
        /// <paramref name="typeReferences"/> and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="IType"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, params IType[] typeReferences)
            : this(name, requiresConstructor, TypeParameterSpecialCondition.None, typeReferences)
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>,
        /// <paramref name="typeReferences"/> and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="ITypeReference"/>s 
        /// which denote the constraints set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, params ITypeReference[] typeReferences)
            : this(name, requiresConstructor, TypeParameterSpecialCondition.None, typeReferences)
        {
        }



        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, TypeParameterSpecialCondition specialCondition)
            : this(name, false, specialCondition, new ITypeReference[0])
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition)
            : this(name, requiresConstructor, specialCondition, new ITypeReference[0])
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="ITypeReference"/>s 
        /// which denote the constraints set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, TypeParameterSpecialCondition specialCondition, params ITypeReference[] typeReferences)
            : this(name, false, specialCondition, typeReferences)
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="IType"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, TypeParameterSpecialCondition specialCondition, params  IType[] typeReferences)
            : this(name, false, specialCondition, typeReferences)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="typeReferences"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="Type"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        public TypeConstrainedName(string name, TypeParameterSpecialCondition specialCondition, params Type[] typeReferences)
            : this(name, false, specialCondition, typeReferences)
        {

        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>
        /// and <paramref name="requiresConstructor"/>
        /// provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="Type"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition, params Type[] typeReferences)
            : this(name, requiresConstructor, specialCondition, typeReferences.GetTypeReferences())
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>,
        /// <paramref name="typeReferences"/> and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="IType"/>s that are strung
        /// together into <see cref="ITypeReference"/>s which denote the constraints
        /// set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition, params IType[] typeReferences)
            : this(name, requiresConstructor, specialCondition, CodeGeneratorHelper.GetTypeReferences(typeReferences))
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="TypeConstrainedName"/> with the <paramref name="name"/>,
        /// <paramref name="typeReferences"/> and <paramref name="requiresConstructor"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="typeReferences">The series of <see cref="ITypeReference"/>s 
        /// which denote the constraints set forth on the <see cref="TypeConstrainedName"/>.</param>
        /// <param name="requiresConstructor">Whether the <see cref="TypeConstrainedName"/>
        /// requires an empty-parameter constructor.</param>
        public TypeConstrainedName(string name, bool requiresConstructor, TypeParameterSpecialCondition specialCondition, params ITypeReference[] typeReferences)
        {
            this.name = name;
            bool valueTypeConstraint = false;
            this.typeReferences = Tweaks.FilterArray<ITypeReference>(typeReferences, delegate(ITypeReference d)
            {
                bool isValueType = false;
                if (d is IExternTypeReference)
                    if (((IExternTypeReference)d).TypeInstance.Type == typeof(ValueType))
                        valueTypeConstraint = isValueType = true;
                    else
                        return true;
                else
                    return true;
                return (!(isValueType));
            });

            this.requiresConstructor = requiresConstructor;
            if (specialCondition == TypeParameterSpecialCondition.None && valueTypeConstraint)
                this.specialCondition = TypeParameterSpecialCondition.ValueType;
            else
                this.specialCondition = specialCondition;
        }

        #endregion

        /// <summary>
        /// Returns the name of the <see cref="TypeConstrainedName"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Returns the constraints set forth on the <see cref="TypeConstrainedName"/>.
        /// </summary>
        public ITypeReference[] TypeReferences
        {
            get
            {
                return this.typeReferences;
            }
        }

        /// <summary>
        /// Returns whether the <see cref="TypeConstrainedName"/> has a null-parameter
        /// constructor as a condition.
        /// </summary>
        public bool RequiresConstructor
        {
            get
            {
                return this.requiresConstructor;
            }
        }

        public TypeParameterSpecialCondition SpecialCondition
        {
            get
            {
                return this.specialCondition;
            }
            set
            {
                this.specialCondition = value;
            }
        }
    }
}
