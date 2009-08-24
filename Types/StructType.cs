using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types.Members;
using System.Collections;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Provides a structure type.
    /// </summary>
    [Serializable]
    public class StructType :
        SegmentableParameteredMemberTypeParentType<IStructType, CodeTypeDeclaration, IStructPartials>,
        IStructType
    {
        ITypeReferenceCollection implementsList;

        /// <summary>
        /// Creates a new instance of <see cref="StructType"/>with the <paramref name="name"/>
        /// and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="StructType"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="StructType"/>.</param>
        protected internal StructType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
        }

        protected internal StructType(IStructType basePartial, ITypeParent parentTarget)
            : base(basePartial,parentTarget)
        {
        }

        /// <summary>
        /// Generates the <see cref="CodeTypeDeclaration"/> that represents the <see cref="StructType"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeTypeDeclaration"/> if successful.-null- otherwise.</returns>
        public override CodeTypeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeTypeDeclaration result = base.GenerateCodeDom(options);
            ITypeReference[] impls = this.ImplementsList.ToArray();
            bool[] duplicate = new bool[impls.Length];
            List<string> names = new List<string>();
            for (int i = 0; i < impls.Length; i++)
            {
                string currentName = impls[i].TypeInstance.GetTypeName(options);
                if (impls[i].TypeParameters.Count > 0 && impls[i].TypeInstance.IsGeneric)
                    currentName += string.Format("`{0}", impls[i].TypeParameters.Count);
                duplicate[i] = names.Contains(currentName);
                if (duplicate[i])
                {
                    duplicate[names.IndexOf(currentName)] = true;
                }
                names.Add(currentName);
            }
            bool autoResolve = options.AutoResolveReferences;
            for (int i = 0; i < impls.Length; i++)
            {
                ITypeReference typeRef = impls[i];
                if (duplicate[i] && autoResolve)
                    options.AutoResolveReferences = false;
                result.BaseTypes.Add(typeRef.GenerateCodeDom(options));
                if (duplicate[i] && autoResolve)
                    options.AutoResolveReferences = autoResolve;
            }
            result.IsStruct = true;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region ISegmentableDeclaredType<IStructType,CodeTypeDeclaration> Members

        ISegmentableDeclaredTypePartials<IStructType, CodeTypeDeclaration> ISegmentableDeclaredType<IStructType,CodeTypeDeclaration>.Partials
        {
            get { return this.Partials; }
        }

        public new IStructType GetRootDeclaration()
        {
            return (IStructType)base.GetRootDeclaration();
        }

        #endregion

        public ITypeReferenceCollection ImplementsList
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().ImplementsList;
                if (this.implementsList == null)
                    this.implementsList = new TypeReferenceCollection();
                return this.implementsList;
            }
        }

        protected override IStructPartials InitializePartials()
        {
            return new StructPartials(this);
        }

        /// <summary>
        /// Performs a look-up on the <see cref="StructType"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="StructType"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.implementsList != null)
                foreach (ITypeReference itr in this.ImplementsList)
                        result.Add(itr);
        }
    }
}
