using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser
{
    public interface ITokenStream<T> :
        IControlledStateCollection<T>
        where T :
            IToken
    {
        
    }
}
