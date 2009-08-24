using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
namespace Oilexer.Types.Members
{
    public interface IPropertySetBodyMember :
        IPropertyBodyMember
    {
        IPropertySetValueReferenceExpression ValueLocal { get; }
    }
}
