using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Utilities.Arrays;
using Oilexer.Translation;
using System.Linq;
namespace Oilexer.Types
{
    [Serializable]
    public abstract class DeclaredTypes<TItem, TDom> :
        ControlledStateDictionary<string, TItem>,
        IDeclaredTypes<TItem, TDom>,
        IDeclarations
        where TItem :
            IDeclaredType<TDom>
        where TDom :
            CodeTypeDeclaration
    {
        /// <summary>
        /// Data member for <see cref="TargetDeclaration"/>.
        /// </summary>
        private ITypeParent targetDeclaration;

        #region IDeclarations Events

        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemAdded;

        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemRemoved;

        #endregion

        #region IDeclarations<TItem> Events

        public event EventHandler<EventDeclarationArgs<TItem>> ItemAdded;

        public event EventHandler<EventDeclarationArgs<TItem>> ItemRemoved;

        #endregion


        /// <summary>
        /// Creates a new instance of <see cref="ClassTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        public DeclaredTypes(ITypeParent targetDeclaration)
        {
            Initialize(targetDeclaration);
        }
        public DeclaredTypes(ITypeParent targetDeclaration, DeclaredTypes<TItem, TDom> sibling) :
            base(sibling)
        {
            Initialize(targetDeclaration);
        }

        private void Initialize(ITypeParent targetDeclaration)
        {
            this.targetDeclaration = targetDeclaration;
        }

        #region IDeclaredTypes Members
        /// <summary>
        /// Returns the <see cref="ITypeParent"/> which new elements are children to.
        /// </summary>
        public ITypeParent TargetDeclaration
        {
            get { return this.targetDeclaration; }
        }

        #endregion

        #region IDeclaredTypes<TItem,TDom> Members



        public abstract TItem AddNew(string name);
        public abstract TItem AddNew(string name, params TypeConstrainedName[] typeParameters);
        public TDom[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TItem[] items = new TItem[this.Count];
            this.Values.CopyTo(items, 0);
            TDom[] result = new TDom[this.Count];
            for (int i = 0; i < this.Count; i++)
                result[i] = items[i].GenerateCodeDom(options);
            return result;
        }
        IDeclaredTypes<TItem, TDom> IDeclaredTypes<TItem, TDom>.GetPartialClone(ITypeParent partialTarget)
        {
            return this.OnGetPartialClone(partialTarget);
        }

        public string GetUniqueName(string name)
        {
            int indexOffset = 0;
            string currentName = name;
            while (ContainsName(currentName))
                currentName = string.Format("{0}{1}", name, ++indexOffset);
            return currentName;
        }

        private bool ContainsName(string name)
        {
            if (this.TargetDeclaration.Name == name)
                return true;
            foreach (TItem ti in this.Values)
                if (ti.Name == name)
                    return true;
            return false;
        }


        public void Add(TItem declaredType)
        {
            this._Add(declaredType.GetUniqueIdentifier(), declaredType);
        }

        #endregion


        #region IDeclarations<TItem> Members

        IDeclarationTarget IDeclarations<TItem>.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        protected internal override void _Add(string uniqueIdentifier, TItem value)
        {
            value.DeclarationChange += new EventHandler<DeclarationChangeArgs>(OnItemChanged);
            base._Add(uniqueIdentifier, value);
        }

        void OnItemChanged(object sender, DeclarationChangeArgs e)
        {
            if (!(e.Declaration is TItem))
                return;
            int index = this.Values.IndexOf((TItem)e.Declaration);
            this.Keys[index] = e.Declaration.GetUniqueIdentifier();
        }

        /// <summary>
        /// Removes the <typeparamref name="TItem"/> from the <see cref="DeclaredTypes{TItem, TDom}"/>
        /// </summary>
        /// <param name="name"></param>
        public new virtual void Remove(string name)
        {
            this[name].DeclarationChange -= new EventHandler<DeclarationChangeArgs>(OnItemChanged);
            this._Remove(name);
        }

        /// <summary>
        /// Removes every item from the <see cref="DeclaredTypes{TItem, TDom}"/>.
        /// </summary>
        public new void Clear()
        {
            foreach (TItem ti in this.Values)
                ti.DeclarationChange -= new EventHandler<DeclarationChangeArgs>(OnItemChanged);
            this._Clear();
        }

        public new TItem this[int index]
        {
            get
            {
                return ((KeyValuePair<string, TItem>)base[index]).Value;
            }
        }

        #endregion

        protected abstract IDeclaredTypes<TItem, TDom> OnGetPartialClone(ITypeParent partialTarget);

        #region IDisposable Members

        public void Dispose()
        {
            if (this.Count == 0)
            {
                this.targetDeclaration = null;
                return;
            }
            /* *
             * Iterate through a filtered copy of the elements in the DeclaredTypes dictionary.
             * This enables elements of the shared partial dictionary to be removed without 
             * an exception of 'collection changed' occurring.
             * */
            foreach (TItem t in from value in this.Values
                                where (!(targetDeclaration is ISegmentableDeclarationTarget)) ||
                                      value.ParentTarget == this.targetDeclaration
                                select value)
            {
                this.Remove(t.GetUniqueIdentifier());
                t.Dispose();
            }
            this.targetDeclaration = null;
        }

        #endregion

        #region IDeclarations Members

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemAdded
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemRemoved
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region On* Event Invokers

        protected virtual void OnItemRemoved(TItem removedItem)
        {
            if (this.IDeclarations_ItemRemoved != null)
                this.IDeclarations_ItemRemoved(this, new EventDeclarationArgs<IDeclaration>(removedItem));
            if (this.ItemRemoved != null)
                this.ItemRemoved(this, new EventDeclarationArgs<TItem>(removedItem));
        }

        protected virtual void OnItemAdded(TItem itemAdded)
        {
            if (this.IDeclarations_ItemAdded != null)
                this.IDeclarations_ItemAdded(this, new EventDeclarationArgs<IDeclaration>(itemAdded));
            if (this.ItemAdded != null)
                this.ItemAdded(this, new EventDeclarationArgs<TItem>(itemAdded));
        }

        #endregion 

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="DeclaredTypes{TItem, TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="DeclaredTypes{TItem, TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            foreach (TItem ti in this.Values)
            {
                if (options.AllowPartials)
                {
                    if (this.targetDeclaration is ISegmentableDeclarationTarget)
                    {
                        if (ti.ParentTarget == this.targetDeclaration)
                            ti.GatherTypeReferences(ref result, options);
                    }
                    else
                        ti.GatherTypeReferences(ref result, options);
                    if (ti is ISegmentableDeclaredType)
                    {
                        ISegmentableDeclaredType isdt = ((ISegmentableDeclaredType)(ti));
                        foreach (TItem ti2 in isdt.Partials)
                        {
                            if (this.targetDeclaration is ISegmentableDeclarationTarget)
                            {
                                if (ti2.ParentTarget == this.targetDeclaration)
                                    ti2.GatherTypeReferences(ref result, options);
                            }
                            else
                                ti2.GatherTypeReferences(ref result, options);
                        }
                    }
                }
                else
                    if (this.targetDeclaration is ISegmentableDeclarationTarget)
                    {
                        if (((ISegmentableDeclarationTarget)this.targetDeclaration).IsRoot)
                            ti.GatherTypeReferences(ref result, options);
                    }
                    else
                        ti.GatherTypeReferences(ref result, options);
            }

        }

        #endregion

        #region IDeclaredTypes<TItem, TDom> Members

        public int GetCountForTarget(ITypeParent target)
        {
            int result = 0;
            foreach (TItem ti in this.Values)
                if (ti.ParentTarget == target)
                    result++;
                else if (ti is ISegmentableDeclaredType)
                    foreach (TItem tiPartial in ((ISegmentableDeclaredType)(ti)).Partials)
                        if (tiPartial.ParentTarget == target)
                            result++;
            return result;
        }

        #endregion

    }
}
