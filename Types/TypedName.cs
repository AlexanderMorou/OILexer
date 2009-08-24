using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// A typed name used to simplify generation of common elements.
    /// </summary>
    public struct TypedName
    {
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="TypeReference"/>.
        /// </summary>
        private ITypeReference typeReference;

        /// <summary>
        /// Creates a new instance of <see cref="TypedName"/> with the <paramref name="name"/> and
        /// <paramref name="typedReference"/> provided.
        /// </summary>
        /// <param name="name">The name of <see cref="TypedName"/></param>
        /// <param name="typeReference">The <see cref="ITypeReference"/> that the 
        /// <paramref name="name"/> relates to.</param>
        public TypedName(string name, ITypeReference typeReference)
        {
            this.name = name;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypedName"/> with the <paramref name="name"/> and
        /// <paramref name="typedReference"/> provided.
        /// </summary>
        /// <param name="name">The name of <see cref="TypedName"/></param>
        /// <param name="type"><para>The <see cref="IType"/> containing the reference that the 
        /// <paramref name="name"/> relates to.</para>
        /// <seealso cref="IType.GetTypeReference()"/>
        /// </param>
        public TypedName(string name, IType type)
            : this(name, type.GetTypeReference())
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="TypedName"/> with the <paramref name="name"/> and
        /// <paramref name="typedReference"/> provided.
        /// </summary>
        /// <param name="name">The name of <see cref="TypedName"/></param>
        /// <param name="type"><para>The <see cref="IType"/> containing the reference that the 
        /// <paramref name="name"/> relates to.</para>
        /// <seealso cref="IType.GetTypeReference()"/>
        /// </param>
        public TypedName(string name, IType type, ITypeReferenceCollection typeParameters)
            : this(name, type.GetTypeReference(typeParameters))
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="TypedName"/> with the <paramref name="name"/> and
        /// <paramref name="typedReference"/> provided.
        /// </summary>
        /// <param name="name">The name of <see cref="TypedName"/></param>
        /// <param name="type"><para>The <see cref="System.Type"/> which is used to obtain
        /// the type reference that <paramref name="name"/> relates to.</para>
        /// <seealso cref="IType.GetTypeReference()"/>
        /// </param>
        public TypedName(string name, Type type)
            : this(name, type.GetTypeReference())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypedName"/> with the <paramref name="name"/> and
        /// <paramref name="typedReference"/> provided.
        /// </summary>
        /// <param name="name">The name of <see cref="TypedName"/></param>
        /// <param name="type"><para>The <see cref="System.Type"/> which is used to obtain
        /// the type reference that <paramref name="name"/> relates to.</para>
        /// <seealso cref="IType.GetTypeReference()"/>
        /// </param>
        public TypedName(string name, Type type, ITypeReferenceCollection typeParameters)
            : this(name, type.GetTypeReference(typeParameters))
        {
        }

        /// <summary>
        /// Returns the name of the <see cref="TypedName"/>.
        /// </summary>
        public string Name 
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Returns the <see cref="ITypeReference"/> of the <see cref="Name"/>.
        /// </summary>
        public ITypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }
    }
}
