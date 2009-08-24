using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using System.Runtime.Serialization;

namespace Oilexer.Types
{
    /// <summary>
    /// A partials collection of a partial-able type.
    /// </summary>
    /// <typeparam name="TItem">The type of members in the partials collection.</typeparam>
    /// <typeparam name="TDom">The <see cref="CodeTypeDeclaration"/> which 
    /// <typeparamref name="TItem"/> instances yield.</typeparam>
    [Serializable]
    public abstract class SegmentableDeclaredTypePartials<TItem, TDom> :
        ControlledStateCollection<TItem>,
        ISegmentableDeclaredTypePartials<TItem, TDom>,
        ISegmentableDeclaredTypePartials
        where TItem :
            ISegmentableDeclaredType<TItem, TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        /// <summary>
        /// Data member for <see cref="RootDeclaration"/>.
        /// </summary>
        private TItem rootDeclaration;

        /// <summary>
        /// Creates a new instance of <see cref="SegmentableDeclaredTypePartials{TItem, TDom}"/>
        /// with the <see cref="RootDeclaration"/> provided.
        /// </summary>
        public SegmentableDeclaredTypePartials(TItem rootDeclaration)
        {
            this.rootDeclaration = rootDeclaration;
        }

        #region ISegmentableDeclaredTypePartials Members

        ISegmentableDeclaredType ISegmentableDeclaredTypePartials.RootDeclaration
        {
            get { return this.RootDeclaration; }
        }

        ISegmentableDeclaredType ISegmentableDeclaredTypePartials.AddNew()
        {
            return this.AddNew();
        }


        ISegmentableDeclaredType ISegmentableDeclaredTypePartials.AddNew(ITypeParent parentTarget)
        {
            return this.AddNew(parentTarget);
        }


        #endregion

        #region ISegmentableDeclaredTypePartials<TItem,TDom> Members

        public TItem RootDeclaration
        {
            get { return this.rootDeclaration; }
        }
        public TItem AddNew(ITypeParent parentTarget)
        {
            if ((parentTarget is ISegmentableDeclarationTarget && this.RootDeclaration.ParentTarget is ISegmentableDeclarationTarget && ((ISegmentableDeclarationTarget)parentTarget).GetRootDeclaration().Equals(((ISegmentableDeclarationTarget)(this.RootDeclaration.ParentTarget)).GetRootDeclaration())) || parentTarget == this.RootDeclaration.ParentTarget)
            {
                TItem partial = this.GetNewPartial(parentTarget);
                base.baseCollection.Add(partial);
                return partial;
            }
            throw new InvalidOperationException("Type-hierarchy must be identical to the root declaration");
        }

        public TItem AddNew()
        {
            TItem newItem = default(TItem);
            if (this.RootDeclaration.ParentTarget is INameSpaceDeclaration)
                newItem = this.GetNewPartial((ITypeParent)((INameSpaceDeclaration)this.RootDeclaration.ParentTarget).Partials.AddNew());
            else if (this.RootDeclaration.ParentTarget is IIntermediateProject)
                newItem = this.GetNewPartial((ITypeParent)((IIntermediateProject)(this.RootDeclaration.ParentTarget)).Partials.AddNew());
            else if ((this.RootDeclaration.ParentTarget is ISegmentableDeclaredType) && (this.RootDeclaration.ParentTarget is ITypeParent))
                newItem = this.GetNewPartial((ITypeParent)((ISegmentableDeclaredType)this.RootDeclaration.ParentTarget).Partials.AddNew());
            else
                newItem = this.GetNewPartial(this.RootDeclaration.ParentTarget);
            base.baseCollection.Add(newItem);
            return newItem;
        }

        #endregion
        /// <summary>
        /// Obtains a new partial of the <typeparamref name="TItem"/> relative to 
        /// <see cref="RootDeclaration"/>.
        /// </summary>
        /// <returns>A new instance of <typeparamref name="TItem"/> if successful.</returns>
        /// <exception cref="System.InvalidOperationException"><see cref="SegmentableDeclaredTypePartials{TItem, TDom}"/> is in an
        /// invalid state.</exception>
        protected abstract TItem GetNewPartial(ITypeParent parent);


        #region ISegmentableDeclarationTargetPartials Members

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.RootDeclaration
        {
            get { return this.rootDeclaration; }
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.AddNew()
        {
            return this.AddNew();
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.AddNew(IDeclarationTarget parentTarget)
        {
            return this.AddNew((ITypeParent)parentTarget);
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials<TItem> Members


        TItem ISegmentableDeclarationTargetPartials<TItem>.AddNew(IDeclarationTarget parentTarget)
        {
            return this.AddNew((ITypeParent)parentTarget);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.rootDeclaration != null && this.rootDeclaration.IsRoot)
            {
                foreach (TItem item in this)
                    item.Dispose();
                this.baseCollection.Clear();
            }
            this.rootDeclaration = default(TItem);
            this.baseCollection = null;
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials<TItem> Members


        public void Remove(TItem partial)
        {
            if (base.baseCollection.Contains(partial))
            {
                partial.Dispose();
                this.baseCollection.Remove(partial);
            }
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials Members

        void ISegmentableDeclarationTargetPartials.Remove(ISegmentableDeclarationTarget partial)
        {
            if (!(partial is TItem))
                throw new ArgumentException("partial");
            this.Remove((TItem)partial);
        }

        #endregion
    }
}
