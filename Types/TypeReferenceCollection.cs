using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    [Serializable]
    public class TypeReferenceCollection :
        List<ITypeReference>,
        ITypeReferenceCollection
    {
        public TypeReferenceCollection()
        {

        }

        public TypeReferenceCollection(params ITypeReference[] typeReferences)
        {
            this.AddRange(typeReferences);
        }
        public TypeReferenceCollection(params IType[] types)
        {
            this.AddRange(types);
        }
        public TypeReferenceCollection(params object[] types)
        {
            foreach (object t in types)
            {
                if (t is Type)
                    this.Add((Type)t);
                else if (t is IType)
                    this.Add((IType)t);
                else if (t is ITypeReference)
                    this.Add((ITypeReference)t);
            }
        }
        public TypeReferenceCollection(params Type[] types)
        {
            this.AddRange(types);
        }

        #region ITypeReferenceCollection Members

        public ITypeReference[] AddRange(Type[] types)
        {
            ITypeReference[] result = new ITypeReference[types.Length];
            for (int i = 0; i < types.Length; i++)
                result[i] = types[i].GetTypeReference();
            this.AddRange(result);
            return result;
        }

        public ITypeReference[] AddRange(IType[] types)
        {
            ITypeReference[] result = new ITypeReference[types.Length];
            for (int i = 0; i < types.Length; i++)
                result[i] = types[i].GetTypeReference();
            this.AddRange(result);
            return result;
        }

        public ITypeReference Add(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            ITypeReference result = type.GetTypeReference();
            this.Add(result);
            return result;
        }

        public ITypeReference Add(IType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            ITypeReference result = type.GetTypeReference();
            this.Add(result);
            return result;
        }

        public void AddRange(ITypeReference[] types)
        {
            foreach (ITypeReference tr in types)
                if (tr == null)
                    throw new ArgumentException("One of the entries in types was null.");
            base.AddRange(types);
        }
        #endregion
    }
}
