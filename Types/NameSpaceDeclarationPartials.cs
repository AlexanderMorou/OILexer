using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    public class NameSpaceDeclarationPartials :
        ControlledStateCollection<INameSpaceDeclaration>,
        INameSpaceDeclarationPartials,
        ISegmentableDeclarationTargetPartials
    {
        private INameSpaceDeclaration rootDeclaration;

        public NameSpaceDeclarationPartials(INameSpaceDeclaration rootDeclaration)
        {
            this.rootDeclaration = rootDeclaration;
        }

        #region ISegmentableDeclarationTargetPartials<INameSpaceDeclaration> Members

        public INameSpaceDeclaration RootDeclaration
        {
            get { return this.rootDeclaration; }
        }

        public INameSpaceDeclaration AddNew()
        {
            INameSpaceDeclaration newNameSpace = null;
            if (this.RootDeclaration.ParentTarget is INameSpaceDeclaration)
                newNameSpace = new NameSpaceDeclaration(this.RootDeclaration, ((INameSpaceDeclaration)this.RootDeclaration.ParentTarget).Partials.AddNew());
            else if (this.RootDeclaration.ParentTarget is IIntermediateProject)
                newNameSpace = new NameSpaceDeclaration(this.RootDeclaration, ((IIntermediateProject)this.RootDeclaration.ParentTarget).Partials.AddNew());
            else
                newNameSpace = new NameSpaceDeclaration(this.RootDeclaration, this.RootDeclaration.ParentTarget);
            this.baseList.Add(newNameSpace);
            return newNameSpace;
        }

        public INameSpaceDeclaration AddNew(INameSpaceParent parentTarget)
        {
            if ((parentTarget is ISegmentableDeclarationTarget && this.RootDeclaration.ParentTarget is ISegmentableDeclarationTarget && ((ISegmentableDeclarationTarget)parentTarget).GetRootDeclaration().Equals(((ISegmentableDeclarationTarget)(this.RootDeclaration.ParentTarget)).GetRootDeclaration())) || parentTarget == this.RootDeclaration.ParentTarget)
            {
                INameSpaceDeclaration partial = new NameSpaceDeclaration(this.RootDeclaration, parentTarget);
                base.baseList.Add(partial);
                return partial;
            }
            throw new InvalidOperationException("Type-hierarchy must be identical to the root declaration");
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials<INameSpaceDeclaration> Members

        INameSpaceDeclaration ISegmentableDeclarationTargetPartials<INameSpaceDeclaration>.AddNew(IDeclarationTarget parentTarget)
        {
            return this.AddNew((INameSpaceParent)parentTarget);
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials Members

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.RootDeclaration
        {
            get { return this.RootDeclaration; }
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.AddNew()
        {
            return this.AddNew();
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTargetPartials.AddNew(IDeclarationTarget parentTarget)
        {
            return this.AddNew((INameSpaceParent)parentTarget);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.rootDeclaration != null && this.rootDeclaration.IsRoot)
            {
                foreach (INameSpaceDeclaration insd in this)
                    insd.Dispose();
                this.baseList.Clear();
            }
            this.rootDeclaration = null;
            this.baseList = null;
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials<INameSpaceDeclaration> Members


        public void Remove(INameSpaceDeclaration partial)
        {
            if (!base.baseList.Contains(partial))
                return;
            partial.Dispose();
            base.baseList.Remove(partial);
        }

        #endregion

        #region ISegmentableDeclarationTargetPartials Members


        public void Remove(ISegmentableDeclarationTarget partial)
        {
            if (!(partial is INameSpaceDeclaration))
                throw new ArgumentException("partial");
        }

        #endregion
    }
}
