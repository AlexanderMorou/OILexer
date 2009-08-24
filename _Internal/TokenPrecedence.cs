using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal enum TokenPrecedence
    {
        Lower   = -1,
        Equal   =  0,
        Higher  =  1,
    }
}
