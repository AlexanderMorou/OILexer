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
    /// <summary>
    /// Defines properties and methods for working with an iterator
    /// break statement for breaking out of an iterator state machine.
    /// </summary>
    public interface IYieldBreakStatement :
        IStatement<CodeSnippetStatement>
    {
    }
}
