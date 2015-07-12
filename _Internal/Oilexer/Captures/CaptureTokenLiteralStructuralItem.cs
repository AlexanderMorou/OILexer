using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal class CaptureTokenLiteralStructuralItem :
        CaptureTokenStructuralItem,
        ICaptureTokenLiteralStructuralItem
    {
        private ControlledCollection<ILiteralTokenItem> sources;
        private CovariantReadOnlyCollection<ITokenSource, ILiteralTokenItem> _sources;

        public CaptureTokenLiteralStructuralItem(ILiteralTokenItem source)
        {
            this.sources = new ControlledCollection<ILiteralTokenItem>();
            this.Add(source);
            if (source.IsFlag.HasValue && source.IsFlag.Value)
                this.ResultType = ResultedDataType.FlagEnumerationItem;
            else
                this.ResultType = ResultedDataType.EnumerationItem;
        }

        public override ResultedDataType ResultType
        {
            get
            {
                return base.ResultType;
            }
            set
            {
                base.ResultType = value;
            }
        }

        public void Add(ILiteralTokenItem item)
        {
            this.sources.baseList.Add(item);
        }

        #region ICaptureTokenLiteralStructuralItem Members

        /// <summary>
        /// Returns the <see cref="String"/> name of the <see cref="CaptureTokenLiteralStructuralItem"/>.
        /// </summary>
        public string Name
        {
            get { return this.Sources.First().Name; }
        }

        public object Value
        {
            get 
            {
                return this.Sources.First().Value;
            }
        }

        public new IControlledCollection<ILiteralTokenItem> Sources
        {
            get
            {
                return this.sources;
            }
        }

        #endregion

        protected override IControlledCollection<ITokenSource> OnGetSources()
        {
            if (this._sources == null) 
                this._sources = new CovariantReadOnlyCollection<ITokenSource, ILiteralTokenItem>(this.Sources);
            return this._sources;
        }

        protected override ICaptureTokenStructuralItem PerformUnion(ICaptureTokenStructuralItem rightElement)
        {
            if (rightElement is ICaptureTokenLiteralStructuralItem)
            {
                var rightLiteral = (ICaptureTokenLiteralStructuralItem)rightElement;
                CaptureTokenLiteralStructuralItem result = new CaptureTokenLiteralStructuralItem(this.sources.First());
                result.sources.AddRange(this.sources.Skip(1).ToArray());
                result.sources.AddRange(rightLiteral.Sources.ToArray());
                result.ResultType = this.ResultType;
                if (result.ResultType == ResultedDataType.Character && rightElement.ResultType == ResultedDataType.String)
                    result.ResultType = ResultedDataType.String;
                result.Optional = this.Optional || rightElement.Optional;
                return result;
            }
            else
                return new CaptureTokenGeneralStructuralItem(this.sources.Concat(rightElement.Sources).ToArray());
        }
    }
}
