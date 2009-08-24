using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;

namespace Oilexer.Comments
{
    /// <summary>
    /// Defines properties and methods for working with a reference comment that refers to a 
    /// property.
    /// </summary>
    public interface IPropertyReferenceComment :
        IMemberReferenceComment<IPropertyReferenceExpression>
    {
    }
}
