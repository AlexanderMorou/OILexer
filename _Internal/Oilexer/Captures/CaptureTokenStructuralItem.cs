using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal abstract class CaptureTokenStructuralItem :
        ICaptureTokenStructuralItem
    {
        private string name;
        #region ICaptureTokenStructuralItem Members

        public IControlledCollection<ITokenSource> Sources { get { return this.OnGetSources(); } }

        protected abstract IControlledCollection<ITokenSource> OnGetSources();

        public virtual ResultedDataType ResultType { get; set; }

        //public int Rank { get; set; }

        public ICaptureTokenStructuralItem Union(ICaptureTokenStructuralItem rightElement)
        {
            return this.PerformUnion(rightElement);
        }

        protected abstract ICaptureTokenStructuralItem PerformUnion(ICaptureTokenStructuralItem rightElement);

        public string Name
        {
            get
            {
                return (from s in this.Sources
                        let iti = s as ITokenItem
                        where iti != null
                        select iti.Name).FirstOrDefault();
            }
        }

        public string BucketName
        {
            get
            {
                return this.BucketIndex == null ? this.Name : string.Format("{0}{1}", this.Name, this.BucketIndex);
            }
        }

        public int? BucketIndex { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.ResultType, this.BucketName);
        }
        #endregion

        public IIntermediateFieldMember AssociatedField { get; set; }

        public int StateIndex { get; set; }

        public bool Optional { get; set; }
        public bool GroupOptional { get; set; }
    }
}
