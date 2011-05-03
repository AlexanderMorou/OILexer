using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.CodeDom;
using Oilexer.Utilities.Collections;
using Oilexer.Utilities.Arrays;
using System.Runtime.Serialization;
using System.Diagnostics;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of <typeparamref name="TItem"/> instances
    /// which make up all, or part of, the members of <typeparamref name="TParent"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IMember{TParent, TDom}"/> contained
    /// within the <see cref="Members{TItem, TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TParent">The type that is valid as a parent of the <see cref="IMember{TParent, TDom}"/>.</typeparam>
    /// <typeparam name="TDom">The type of <see cref="CodeObject"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    [Serializable]
    public abstract class Members<TItem, TParent, TDom> :
        ControlledStateDictionary<string, TItem>,
        IMembers<TItem, TParent, TDom>,
        IMembers
        where TItem :
            IMember<TParent, TDom>
        where TParent :
            class,
            IDeclarationTarget
        where TDom :
            CodeObject
    {

        public event EventHandler MembersChanged;

        #region IDeclarations Events

        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemAdded;

        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemRemoved;

        #endregion

        #region IDeclarations<TItem> Events

        public event EventHandler<EventDeclarationArgs<TItem>> ItemAdded;

        public event EventHandler<EventDeclarationArgs<TItem>> ItemRemoved;

        #endregion

        private bool needsReindexed = false;
        private bool reindexing = false;
        /// <summary>
        /// Data member for <see cref="TargetDeclaration"/>.
        /// </summary>
        private TParent targetDeclaration;

        /// <summary>
        /// Creates a new instance of <see cref="Members{TItem, TParent, TDom}"/> with the
        /// <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <typeparamref name="TParent"/> that contains
        /// the <see cref="Members{TItem, TParent, TDom}"/>.</param>
        public Members(TParent targetDeclaration)
        {
            this.targetDeclaration = targetDeclaration;
        }

        public Members(TParent targetDeclaration, Members<TItem, TParent, TDom> sibling)
            : base(sibling)
        {
            this.targetDeclaration = targetDeclaration;
        }

        #region IMembers<TItem,TParent,TDom> Members

        /// <summary>
        /// Returns the <typeparamref name="TParent"/> the <see cref="Members{TItem, TParent, TDom}"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <typeparamref name="TParent"/> that contains the <see cref="Members{TItem, TParent, TDom}"/>.
        /// </returns>
        public TParent TargetDeclaration
        {
            get { return this.targetDeclaration; }
        }

        public virtual TDom[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TDom[] result = new TDom[this.Count];
            TItem[] items = new TItem[this.Count];
            this.Values.CopyTo(items, 0);
            for (int i = 0; i < this.Count; i++)
                result[i] = items[i].GenerateCodeDom(options);
            return result;
        }

        #endregion

        #region IDeclarations<TItem> Members

        IDeclarationTarget IDeclarations<TItem>.TargetDeclaration
        {
            get
            {
                return this.targetDeclaration;
            }
        }

        #endregion


        #region IDeclarations<TItem> Members

        public new void Clear()
        {
            foreach (TItem item in this.Values)
                item.DeclarationChange -= new EventHandler<DeclarationChangeArgs>(Item_DeclarationChange);
            this._Clear();
        }

        #endregion

        #region IMembers Members


        public string GetUniqueName(string baseName)
        {
            if (this.needsReindexed)
                this.Reindex();
            string currentName = baseName;
            int i = 0;
            while (this.ContainsName(currentName))
                currentName = string.Format("{0}_{1}", baseName, i++);
            return currentName;
        }

        private bool ContainsName(string name)
        {
            foreach (TItem item in this.Values)
                if (item.Name == name)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns the number of members in the declarations that target what's provided.
        /// </summary>
        /// <param name="target">The member to check which target.</param>
        /// <returns>An integer containing the number of members that are children of the <paramref name="target"/>.</returns>
        public int GetCountForTarget(IDeclarationTarget target)
        {
            int count = 0;
            foreach (TItem member in this.Values)
                if (member.ParentTarget.Equals(target))
                    count++;
            return count;
        }

        IDeclarationTarget IMembers.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        CodeObject[] IMembers.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options).Cast<CodeObject>().ToArray();
        }

        /// <summary>
        /// Returns the element at the index provided
        /// </summary>
        /// <param name="index">The index of the element to find.</param>
        /// <returns>The instance of <typeparam name="TItem"/> at the index provided.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is  beyond the range of the 
        /// <see cref="Members{TItem, TParent, TDom}"/>.
        /// </exception>
        public new TItem this[int index]
        {
            get
            {
                return ((KeyValuePair<string, TItem>)base[index]).Value;
            }
        }

        IMembers IMembers.GetPartialClone(IDeclaredType parent)
        {
            if (!(parent is TParent))
                throw new ArgumentException("must be of type TParent", "parent");
            return (IMembers)this.OnGetPartialClone((TParent)parent);
        }


        #endregion


        protected abstract IMembers<TItem, TParent, TDom> OnGetPartialClone(TParent parent);


        #region IMembers<TItem,TParent,TDom> Members


        public IMembers<TItem, TParent, TDom> GetPartialClone(TParent parent)
        {
            return this.OnGetPartialClone(parent);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[0].Dispose();
                this._Remove(0);
            }
            this.targetDeclaration = default(TParent);
        }

        #endregion
        
        protected internal override void _Add(string key, TItem value)
        {
            if (reindexing)
                return;
            if (needsReindexed)
                Reindex();
            value.DeclarationChange += new EventHandler<DeclarationChangeArgs>(Item_DeclarationChange);
            base._Add(key, value);
            if (this.MembersChanged != null)
                this.MembersChanged(this, null);
            OnItemAdded(value);
        }

        protected internal override bool _Remove(int index)
        {
            bool result;
            TItem removedItem = this[index];
            result = base._Remove(index);
            OnItemRemoved(removedItem);
            return result;
        }
        
        public new virtual void Remove(string uniqueIdentifier)
        {
            if (reindexing)
                return;
            TItem removedItem = this[uniqueIdentifier];
            removedItem.DeclarationChange -= new EventHandler<DeclarationChangeArgs>(Item_DeclarationChange);
            base._Remove(uniqueIdentifier);
            this.OnItemRemoved(removedItem);
        }


        public void Remove(TItem member)
        {

            for (int i = 0; i < this.Count; i++)
            {
                if (this.Values[i].Equals(member))
                    this.Remove(this.Keys[i]);
            }
        }
        void IMembers.Remove(IMember member)
        {
            if (member is TItem)
                this.Remove((TItem)member);
        }
        private void Reindex()
        {
            reindexing = true;
            this.needsReindexed = false;
            TItem[] copy = new TItem[this.Count];
            this.Values.CopyTo(copy, 0);
            base._Clear();
            foreach (TItem item in copy)
            {
                if (base.ContainsKey(item.GetUniqueIdentifier()))
                    item.Dispose();
                else
                    base._Add(item.GetUniqueIdentifier(), item);
            }
            if (this.MembersChanged != null)
                MembersChanged(this, EventArgs.Empty);
            reindexing = false;
        }

        void Item_DeclarationChange(object sender, DeclarationChangeArgs e)
        {
            foreach (TItem imm in this.Values)
            {
                if (((IDeclaration)imm) == e.Declaration)
                {
                    if (!this.ContainsKey(imm.GetUniqueIdentifier()))
                    {
                        this.needsReindexed = true; 
                        break;
                    }
                }
            }
        }

        protected void CheckIndexingStatus()
        {
            if (this.needsReindexed)
                this.Reindex();
        }

        #region IDeclarations Members

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemAdded
        {
            add
            {
                this.IDeclarations_ItemAdded += value;
            }
            remove
            {
                this.IDeclarations_ItemAdded -= value;
            }
        }

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemRemoved
        {
            add
            {
                this.IDeclarations_ItemRemoved += value;
            }
            remove
            {
                this.IDeclarations_ItemRemoved -= value;
            }
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
                IDeclarations_ItemAdded(this, new EventDeclarationArgs<IDeclaration>(itemAdded));
            if (this.ItemAdded != null)
                ItemAdded(this, new EventDeclarationArgs<TItem>(itemAdded));
        }

        #endregion 

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="Members{TItem, TParent, TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="Members{TItem, TParent, TDom}"/> relies on.</param>
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

        void IMembers.Add(IMember item)
        {
            if (!(item is TItem))
                throw new ArgumentException("series not of valid type.");
            this.Add((TItem)item);
        }
        [DebuggerHidden()]
        public void Add(TItem item)
        {
            if (this.ContainsKey(item.GetUniqueIdentifier()))
                throw new InvalidOperationException("item containing the same identifier exists.");
            base._Add(item.GetUniqueIdentifier(), item);
        }
    }
}
