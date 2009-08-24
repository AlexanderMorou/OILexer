using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// A generic declaration that yields instances of <typeparamref name="TDom"/> and is
    /// child to <typeparamref name="TParent"/> instances.
    /// </summary>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="IDeclaration{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which the
    /// <see cref="Declaration{TParent, TDom}"/> yields.</typeparam>
    [Serializable]
    public abstract class Declaration<TParent, TDom> :
        IDeclaration<TParent, TDom>
        where TParent :
            IDeclarationTarget
        where TDom :
            CodeObject
    {

        /// <summary>
        /// Data member for <see cref="ParentTarget"/>
        /// </summary>
        protected TParent parentTarget;
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="AccessLevel"/>.
        /// </summary>
        private DeclarationAccessLevel accessLevel;

        /// <summary>
        /// Creates a new instance of <see cref="Declaration{TParent, TDom}"/>
        /// </summary>
        /// <param name="name">The name of the declaration.</param>
        /// <param name="parentTarget">The <see cref="IDeclarationTarget"/> the 
        /// <see cref="Declaration{TParent, TDom}"/> exists as a sub-member of.</param>
        protected Declaration(string name, TParent parentTarget)
        {
            this.name = name;
            this.parentTarget = parentTarget;
        }

        protected Declaration(TParent parentTarget)
        {
            this.parentTarget = parentTarget;
        }

        #region IDeclaration Members

        /// <summary>
        /// Returns/sets the accessability of the <see cref="Declaration{TParent, TDom}"/>.
        /// </summary>
        public virtual DeclarationAccessLevel AccessLevel
        {
            get
            {
                return this.accessLevel;
            }
            set
            {
                this.accessLevel = value;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            if (this is INameSpaceDeclaration && (this.ParentTarget == null || this.ParentTarget is IIntermediateProject))
                return (INameSpaceDeclaration)this;
            if (this.ParentTarget == null)
                return null;
            return this.ParentTarget.GetRootNameSpace();
        }


        /// <summary>
        /// Indicates the declaration has changed.
        /// </summary>
        
        public event EventHandler<DeclarationChangeArgs> DeclarationChange;

        protected void InvokeDeclarationChange(object sender, IDeclaration declaration)
        {
            if (this.DeclarationChange != null)
                this.DeclarationChange(sender, new DeclarationChangeArgs(declaration));
        }

        /// <summary>
        /// Returns a unique identifier that relates to the <see cref="Declaration{TParent, TDom}"/> relative
        /// to the data components that comprise the <see cref="Declaration{TParent, TDom}"/>.
        /// </summary>
        /// <returns>A string representing the uniqueness of the <see cref="Declaration{TParent, TDom}"/>.</returns>
        public virtual string GetUniqueIdentifier()
        {
            return this.Name;
        }

        /// <summary>
        /// Returns the name of the <see cref="Declaration{TParent, TDom}"/>.
        /// </summary>
        public virtual string Name
        {
            get 
            {
                return this.name;
            }
            set
            {
                string oldName = this.name;
                this.name = value;
                DeclarationChangeArgs callArgs = new DeclarationChangeArgs(this);
                OnDeclarationChanged(callArgs);
                if (callArgs.Cancel)
                    this.name = oldName;
            }
        }

        protected virtual void OnDeclarationChanged(DeclarationChangeArgs callArgs)
        {
            if (this.DeclarationChange != null)
                this.DeclarationChange(this, callArgs);
        }

        CodeObject IDeclaration.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options);
        }

        #endregion

        #region IDeclarationTarget Members

        /// <summary>
        /// Returns the <see cref="IDeclarationTarget"/> the 
        /// <see cref="Declaration{TParent, TDom}"/> exists as a sub-member of.
        /// </summary>
        public virtual TParent ParentTarget
        {
            get { return parentTarget; }
            set { this.parentTarget = value; }
        }

        IDeclarationTarget IDeclarationTarget.ParentTarget
        {
            get { return this.ParentTarget; }
            set
            {
                if (!(value is TParent))
                    throw new ArgumentException("Must be a TParent");
                this.ParentTarget = (TParent)value; }
        }

        #endregion

        #region IDeclaration<TParent,TDom> Members

        /// <summary>
        /// Generates the <typeparamref name="TDom"/> that represents the <see cref="Declaration{TParent, TDom}"/>.
        /// </summary>
        /// <returns>A new instance of a <typeparamref name="TDom"/> if successful.-null- otherwise.</returns>
        public abstract TDom GenerateCodeDom(ICodeDOMTranslationOptions options);

        #endregion

        #region System.Object Members

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="Declaration{TParent, TDom}.GetHashCode()"/>
        /// is suitable for use in hashing algorithms and data structures like a hash
        /// table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Declaration{TParent, TDom}"/>.</returns>
        public override int GetHashCode()
        {
            if (this.parentTarget != null)
                if (this.Name != null)
                    return this.Name.GetHashCode() ^ this.parentTarget.GetHashCode() ^ this.AccessLevel.GetHashCode();
                else
                    return this.parentTarget.GetHashCode() ^ this.AccessLevel.GetHashCode();
            else
                return this.Name.GetHashCode() ^ this.AccessLevel.GetHashCode();
        }

        //
        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Declaration{TParent, TDom}"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="Declaration{TParent, TDom}"/>.</returns>
        public override string ToString()
        {
            if (this is IType)
                return ((IType)this).GetTypeName(CodeGeneratorHelper.DefaultDomOptions);

            List<string> members = new List<string>();
            List<string> types = new List<string>();
            List<string> nameSpaces = new List<string>();
            IDeclaration current = this;
            while (current != null)
            {
                if (current is INameSpaceDeclaration)
                    nameSpaces.Add(current.GetUniqueIdentifier());
                else if (current is IType)
                    types.Add(current.GetUniqueIdentifier());
                else if (current is IDeclaration)
                    members.Add(current.GetUniqueIdentifier());
                if (current.ParentTarget is IIntermediateProject)
                    break;
                current = (IDeclaration)current.ParentTarget;
            }
            members.Reverse();
            types.Reverse();
            nameSpaces.Reverse();
            if (members.Count > 0)
                return String.Format("{0}.{1}::{2}", String.Join(".", nameSpaces.ToArray()), String.Join("+", types.ToArray()), string.Join("]]", members.ToArray()));
            else if (types.Count >0 )
                return String.Format("{0}.{1}", String.Join(".", nameSpaces.ToArray()), String.Join("+", types.ToArray()));
            else
                return String.Format("{0}", String.Join(".", nameSpaces.ToArray()));
        }

        #endregion



        #region IDisposable Members

        /// <summary>
        /// Releases the resources associated with the <see cref="Declaration{TParent, TDom}"/>.
        /// And places the <see cref="Declaration{TParent, TDom}"/> in a disposed state.
        /// </summary>
        public virtual void Dispose()
        {
            this.name = null;
            this.parentTarget = default(TParent);
        }

        #endregion
        protected static MemberAttributes AccessLevelAttributes(DeclarationAccessLevel accessLevel)
        {
            switch (accessLevel)
            {
                case DeclarationAccessLevel.Internal:
                    return MemberAttributes.Assembly;
                case DeclarationAccessLevel.Private:
                    return MemberAttributes.Private;
                case DeclarationAccessLevel.Public:
                    return MemberAttributes.Public;
                case DeclarationAccessLevel.Protected:
                    return MemberAttributes.Family;
                case DeclarationAccessLevel.ProtectedInternal:
                    return MemberAttributes.FamilyOrAssembly;
                default:
                    break;
            }
            return MemberAttributes.Private;
        }


        #region IDeclaration Members

        /// <summary>
        /// Performs a look-up on the <see cref="Declaration{TParent, TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="Declaration{TParent, TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public abstract void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options);

        #endregion
    }
}
