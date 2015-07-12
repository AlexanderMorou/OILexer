using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal class CaptureTokenCharRangeStructuralItem :
        CaptureTokenStructuralItem,
        ICaptureTokenCharRangeStructuralItem
    {
        private ControlledCollection<ICharRangeTokenItem> sources;
        private CovariantReadOnlyCollection<ITokenSource, ICharRangeTokenItem> _sources;

        public CaptureTokenCharRangeStructuralItem(ICharRangeTokenItem source)
        {
            this.sources = new ControlledCollection<ICharRangeTokenItem>();
            this.sources.baseList.Add(source);
            this.ResultType = ResultedDataType.Character;
        }

        public CaptureTokenCharRangeStructuralItem(IEnumerable<ICharRangeTokenItem> sources)
        {
            this.sources = new ControlledCollection<ICharRangeTokenItem>(sources.ToArray());
            this.ResultType = ResultedDataType.Character;
        }
        protected override IControlledCollection<ITokenSource> OnGetSources()
        {
            if (this._sources == null)
                this._sources = new CovariantReadOnlyCollection<ITokenSource, ICharRangeTokenItem>(this.sources);
            return this._sources;
        }

        #region ICaptureTokenCharRangeStructuralItem Members

        public bool HasSiblings { get; set; }

        #endregion

        #region ICaptureTokenCharRangeStructuralItem Members

        public new IControlledCollection<ICharRangeTokenItem> Sources
        {
            get {
                return this.sources;
            }
        }

        #endregion

        protected override ICaptureTokenStructuralItem PerformUnion(ICaptureTokenStructuralItem rightElement)
        {
            if (rightElement is ICaptureTokenCharRangeStructuralItem)
            {
                var rightRange = (ICaptureTokenCharRangeStructuralItem)rightElement;
                var result = new CaptureTokenCharRangeStructuralItem(this.sources.Concat(rightRange.Sources).Distinct());
                result.ResultType = this.ResultType;
                if (result.ResultType == ResultedDataType.Character && rightElement.ResultType == ResultedDataType.String)
                    result.ResultType = ResultedDataType.String;
                if (this.GroupOptional || rightElement.GroupOptional)
                    result.GroupOptional = true;
                //result.Rank = Math.Max(this.Rank, rightElement.Rank);
                return result;
            }
            else
                return new CaptureTokenGeneralStructuralItem(this.sources.Concat(rightElement.Sources));
        }

    }
}
