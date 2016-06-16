using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class PredictionTreeDestinationAvenues :
        ControlledCollection<PredictionTreeDestinationAvenue>
    {
        public PredictionTreeDestinationAvenues(IEnumerable<PredictionTreeDestinationAvenue> set)
            : base(set.ToList())
        {

        }
    }
}
