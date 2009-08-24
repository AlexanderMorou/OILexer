using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    internal class NameSpaceDeclarations :
        ControlledStateDictionary<string, INameSpaceDeclaration>,
        INameSpaceDeclarations,
        IDeclarations
    {
        /// <summary>
        /// Data member for <see cref="TargetDeclaration"/>.
        /// </summary>
        private INameSpaceParent targetDeclaration;

        #region IDeclarations<INameSpaceDeclaration> Events

        public event EventHandler<EventDeclarationArgs<INameSpaceDeclaration>> ItemAdded;

        public event EventHandler<EventDeclarationArgs<INameSpaceDeclaration>> ItemRemoved;

        #endregion

        #region IDeclarations Events

        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemAdded;
        private event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations_ItemRemoved;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="ClassTypes"/> denoting the <see cref="ITypeParent"/>
        /// that members are children of initially.
        /// </summary>
        internal NameSpaceDeclarations(INameSpaceParent targetDeclaration)
        {
            this.targetDeclaration = targetDeclaration;
        }

        internal NameSpaceDeclarations(INameSpaceParent targetDeclaration, IDictionary<string, INameSpaceDeclaration> basePartialMembers)
            : base(basePartialMembers)
        {
            this.targetDeclaration = targetDeclaration;
        }
        #region IDeclarations<INameSpaceDeclaration> Members

        IDeclarationTarget IDeclarations<INameSpaceDeclaration>.TargetDeclaration
        {
            get { return this.TargetDeclaration; }
        }

        #endregion

        #region INameSpaceDeclarations Members

        /// <summary>
        /// Returns the number of types in the <see cref="NameSpaceDeclarations"/> that 
        /// <paramref name="target"/> what's provided.
        /// </summary>
        /// <param name="target">The namespace parent to check which against the namespaces.</param>
        /// <returns>An integer containing the number of <see cref="INameSpaceDeclaration"/> instances that 
        /// are children of the <paramref name="target"/>.</returns>
        public int GetCountForTarget(INameSpaceParent target)
        {
            int result = 0;
            foreach (INameSpaceDeclaration ti in this.Values)
                if (ti.ParentTarget == target)
                    result++;
                else
                    foreach (INameSpaceDeclaration nsp in ti.Partials)
                        if (nsp.ParentTarget == target)
                            result++;
            return result;
        }

        /// <summary>
        /// Returns the <see cref="INameSpaceDeclaration"/> the <see cref="INameSpaceDeclarations"/> belongs
        /// to.
        /// </summary>
        /// <returns>
        /// The <see cref="INameSpaceDeclaration"/> that contains the <see cref="INameSpaceDeclarations"/>.
        /// </returns>
        public INameSpaceParent TargetDeclaration
        {
            get { return this.targetDeclaration; }
        }


        public INameSpaceDeclaration AddNew(string name)
        {
            INameSpaceDeclaration result = new NameSpaceDeclaration(name, this.targetDeclaration);
            this.Add(name, result);
            return result;
        }

        protected override void Add(string key, INameSpaceDeclaration value)
        {
            base.Add(key, value);
            this.OnItemAdded(value);
        }

        private void OnItemAdded(INameSpaceDeclaration itemAdded)
        {
            if (this.IDeclarations_ItemAdded != null)
                this.IDeclarations_ItemAdded(this, new EventDeclarationArgs<IDeclaration>(itemAdded));
            if (this.ItemAdded != null)
                this.ItemAdded(this, new EventDeclarationArgs<INameSpaceDeclaration>(itemAdded));
        }

        public void Add(INameSpaceDeclaration nameSpace)
        {
            if (this.ContainsKey(nameSpace.Name))
                throw new InvalidOperationException("namespace exists");
            else
                this.Add(nameSpace.Name, nameSpace);
        }

        #endregion

        #region IDeclarations<INameSpaceDeclaration> Members


        public new void Remove(string name)
        {
            INameSpaceDeclaration removedItem = this[name];
            base.Remove(name);
            this.OnItemRemoved(removedItem);
        }

        protected virtual void OnItemRemoved(INameSpaceDeclaration removedItem)
        {
            if (this.IDeclarations_ItemRemoved != null)
                this.IDeclarations_ItemRemoved(this, new EventDeclarationArgs<IDeclaration>(removedItem));
            if (this.ItemRemoved != null)
                this.ItemRemoved(this, new EventDeclarationArgs<INameSpaceDeclaration>(removedItem));
        }

        public new void Clear()
        {
            base.Clear();
        }
        
        public new INameSpaceDeclaration this[int index]
        {
            get
            {
                return ((KeyValuePair<string, INameSpaceDeclaration>)base[index]).Value;
            }
        }

        public CodeNamespace[] GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeNamespace> result = new List<CodeNamespace>();
            foreach (INameSpaceDeclaration nameSpace in this.Values)
                result.AddRange(nameSpace.GenerateGroupCodeDom(options));
            return result.ToArray();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.targetDeclaration = null;
            foreach (INameSpaceDeclaration nsd in this.Values)
                try
                {
                    this.Remove(nsd.Name);
                }
                catch { }
        }

        #endregion

        #region INameSpaceDeclarations Members

        public string GetUniqueName(string baseName)
        {
            int indexOffset = 0;
            string currentName = string.Format("{0}{1}", baseName, ++indexOffset);
            while (ContainsName(currentName))
                currentName = string.Format("{0}{1}", baseName, ++indexOffset);
            return currentName;
        }

        private bool ContainsName(string name)
        {
            if (this.TargetDeclaration.Name == name)
                return true;
            foreach (INameSpaceDeclaration ti in this.Values)
                if (ti.Name == name)
                    return true;
            return false;
        }

        public INameSpaceDeclarations GetPartialClone(INameSpaceParent basePartial)
        {
            return new NameSpaceDeclarations(basePartial,this.dictionaryCopy);
        }

        #endregion
        #region IDeclarations Event Placeholders

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemAdded
        {
            add { this.IDeclarations_ItemAdded += value; }
            remove { this.IDeclarations_ItemAdded -= value; }
        }

        event EventHandler<EventDeclarationArgs<IDeclaration>> IDeclarations.ItemRemoved
        {
            add { this.IDeclarations_ItemRemoved += value; }
            remove { this.IDeclarations_ItemRemoved -= value; }
        }

        #endregion

        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="NameSpaceDeclarations"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="NameSpaceDeclarations"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            foreach (INameSpaceDeclaration ti in this.Values)
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
                    foreach (INameSpaceDeclaration insd in ti.Partials)
                    {
                        if (this.targetDeclaration is ISegmentableDeclarationTarget)
                        {
                            if (insd.ParentTarget == this.targetDeclaration)
                                insd.GatherTypeReferences(ref result, options);
                        }
                        else
                            insd.GatherTypeReferences(ref result, options);
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


    }
}
