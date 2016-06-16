using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class ProductionRuleLookAheadBucket
    {

        public string BucketName
        {
            get
            {
                return string.Format("_la{0}", this.BucketID);
            }
        }
        public int BucketID { get; set; }
        public PredictionTreeDFAContext Owner { get; set; }
    }
}
