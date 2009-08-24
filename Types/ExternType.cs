using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types
{
    public partial class ExternType :
        IExternType
    {
        /// <summary>
        /// Data member for <see cref="Type"/>.
        /// </summary>
        private System.Type type;

        public ExternType(System.Type type)
        {
            this.type = type;
        }

        #region IExternType Members

        public Type Type
        {
            get { return this.type; }
        }

        #endregion

        #region IType Members

        public bool IsGeneric
        {
            get
            {
                return type.IsGenericType;
            }
        }

        public bool IsDelegate
        {
            get
            {
                if (!this.type.IsClass)
                    return false;
                return this.type.IsSubclassOf(typeof(Delegate));
            }
        }

        public bool IsGenericTypeDefinition
        {
            get { return type.IsGenericTypeDefinition; }
        }

        internal static Type GetFinalArrayElement(System.Type arrayType)
        {
            System.Type currentType = arrayType;
            while (currentType.IsArray)
                currentType = currentType.GetElementType();
            return currentType;
        }

        public string GetTypeName(
                        ICodeTranslationOptions 

                options)
        {
            return GetTypeName(options, false, new ITypeReference[0]);
        }

        public string GetTypeName(
                        ICodeTranslationOptions 

                options, bool commentStyle)
        {
            return GetTypeName(options, commentStyle, new ITypeReference[0]);
        }

        public string GetTypeName(
                        ICodeTranslationOptions 

                options, ITypeReference[] typeParameters)
        {
            return this.GetTypeName(options, false, typeParameters);
        }

        public string GetTypeName(
                        ICodeTranslationOptions 

                options, bool commentStyle, ITypeReference[] typeParameterValues)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }
            if (_OIL._Core.AutoFormTypes.Contains(GetFinalArrayElement(this.type)))
                return this.type.FullName;
            List<Type> hierarchy = new List<Type>();
            for (Type t = this.Type; t != null; t = t.DeclaringType)
                hierarchy.Add(t);
            hierarchy.Reverse();
            Stack<Type> lastHierarchyTypeArgs = null;
            string[] names = new string[hierarchy.Count];
            int nameIndex = 0;
            int tpChainArgs = 0;
            string tArgNames = null;
            string[] typeArgNames;
            for (IEnumerator<Type> _enum = hierarchy.GetEnumerator(); _enum.MoveNext(); )
            {
                System.Type t = _enum.Current;
                if (t.IsGenericType)
                {
                    /* *
                     * If commentStyle is the intent, then we need to extract the  
                     * type-arguments from the hierarchy members.
                     * */
                    if (commentStyle)
                    {
                        List<Type> tArgs = new List<Type>();
                        if (!t.IsGenericTypeDefinition)
                            t = t.GetGenericTypeDefinition();
                        tArgs.AddRange(t.GetGenericArguments());

                        /* *
                         * Every member contains a full set of the type-arguments including 
                         * those of their parents, so the children will have their list 
                         * truncated to their set only.
                         * */
                        List<Type> tArgsFull = new List<Type>(tArgs.ToArray());
                        if (lastHierarchyTypeArgs != null)
                            while (lastHierarchyTypeArgs.Count > 0)
                            {
                                lastHierarchyTypeArgs.Pop();
                                tArgs.RemoveAt(0);
                            }

                        //Setup for the next child in the list, if applicable.
                        lastHierarchyTypeArgs = new Stack<Type>(tArgsFull);
                        string name = t.Name;
                        if (name.Contains("`"))
                            name = name.Substring(0, name.IndexOf('`'));
                        typeArgNames = new string[tArgs.Count];
                        for (int i = 0; i < tArgs.Count; i++)
                            typeArgNames[i] = tArgs[i].Name;
                        if (tArgs.Count > 0)
                            name += string.Format("{{{0}}}", string.Join(", ", typeArgNames));
                        names[nameIndex++] = name;
                    }
                    else
                        names[nameIndex++] = t.Name;
                }
                else
                    names[nameIndex++] = t.Name;
            }
            if (this.Type.IsGenericType)
                tpChainArgs = this.Type.GetGenericArguments().Length;
            if ((!(commentStyle)) && (tpChainArgs == typeParameterValues.Length && tpChainArgs > 0))
            {
                typeArgNames = new string[tpChainArgs];
                for (int tpArgIndex = 0; tpArgIndex < typeParameterValues.Length; tpArgIndex++)
                {
                    if (((typeParameterValues[tpArgIndex].ResolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType) || ((typeParameterValues[tpArgIndex].ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                    {
                        bool autoResolve = options.AutoResolveReferences;
                        if (autoResolve)
                            options.AutoResolveReferences = false;
                        typeArgNames[tpArgIndex] = typeParameterValues[tpArgIndex].TypeInstance.GetTypeName(options, typeParameterValues[tpArgIndex].TypeParameters.ToArray());
                        if (autoResolve)
                            options.AutoResolveReferences = autoResolve;

                    }
                    else
                        typeArgNames[tpArgIndex] = typeParameterValues[tpArgIndex].TypeInstance.GetTypeName(options, typeParameterValues[tpArgIndex].TypeParameters.ToArray());
                    typeArgNames[tpArgIndex] = string.Format("[{0}]", typeArgNames[tpArgIndex]);
                }
                tArgNames = String.Format("[{0}]", String.Join(",", typeArgNames));
            }

            if (options.AutoResolveReferences && options.ImportList.Contains(this.type.Namespace))
                if (commentStyle)
                    return String.Join(".", names);
                else
                    return String.Format("{0}{1}", String.Join("+", names), tArgNames);
            else if (options.AutoResolveReferences)
            {
                options.ImportList.Add(this.type.Namespace);
                if (commentStyle)
                    return String.Join(".", names);
                else
                    return String.Format("{0}{1}", String.Join("+", names), tArgNames);
            }
            if (commentStyle)
                return string.Format("{0}.{1}", this.Type.Namespace, String.Join(".", names));
            return string.Format("{0}.{1}{2}", this.Type.Namespace, String.Join("+", names), tArgNames);
        }

#if false
        private string GetPartialTypeName(ICodeDOMTranslationOptions options)
        {
            System.Type[] nestingChain = GetNestChain();
            string[] nestNames = new string[nestingChain.Length];
            for (int i = 0; i < nestingChain.Length; i++)
            {
                nestNames[i] = nestingChain[i].Name;
            }
            return string.Join("+", nestNames);
        }
        private Type[] GetNestChain()
        {
            List<Type> result = new List<Type>();
            System.Type current = this.type;
            while (current != null)
            {
                result.Add(current);
                current = current.DeclaringType;
            }
            result.Reverse();
            return result.ToArray();
        }
#endif

        #endregion
        #region IType Members

        public ITypeReference GetTypeReference()
        {
            return this.type.GetTypeReference();
        }

        public ITypeReference GetTypeReference(ITypeReferenceCollection typeParameters)
        {
            return this.Type.GetTypeReference(typeParameters);
        }

        public ITypeReference GetTypeReference(params ITypeReference[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        public ITypeReference GetTypeReference(params IType[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        public ITypeReference GetTypeReference(params object[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        public bool IsClass
        {
            get {return this.Type.IsClass; }
        }

        public bool IsInterface
        {
            get { return this.Type.IsInterface; }
        }

        public bool IsEnumerator
        {
            get { return this.Type.IsEnum; }
        }

        public bool IsStructure
        {
            get { return this.Type.IsValueType; }
        }

        #endregion

        public static explicit operator ExternType(Type type)
        {
            return (ExternType)type.GetExternType();
        }

        #region IEquatable<IType> Members

        public bool Equals(IType other)
        {
            if (!(other is IExternType))
                return false;
            return (((IExternType)other).Type == this.Type);
        }

        #endregion

    }
}
