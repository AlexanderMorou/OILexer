using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer.Statements
{
    public interface IBlockStatement :
        IBlockedStatement<CodeSnippetStatement>
    {
    }
}
