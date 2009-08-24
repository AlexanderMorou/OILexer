using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer.Expression
{
    public interface ITypeOfExpression :
        IMemberParentExpression<CodeTypeOfExpression>
    {
        ITypeReference TypeReference
        {
            get;
            set;
        }

    }
}
