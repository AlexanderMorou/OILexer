using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeDestinationAvenue :
        ControlledCollection<PredictionTreeDestinationPoint>
    {
        public PredictionTreeDestinationAvenue(IEnumerable<PredictionTreeDestinationPoint> set)
            : base(set.ToList())
        {
        }

        public PredictionTree Tree { get; internal set; }
    }

}
