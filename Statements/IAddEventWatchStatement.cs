using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;

namespace Oilexer.Statements
{
    /// <summary>
    /// Defines properties and methods for working with a statement which
    /// adds a watch to a event using a delegate reference.
    /// </summary>
    public interface IAddEventWatchStatement :
        IStatement<CodeAttachEventStatement>
    {
        /// <summary>
        /// The event to watch.
        /// </summary>
        IEventReferenceExpression Reference { get; }
        /// <summary>
        /// The target, or invoked element, of the event watch.
        /// </summary>
        IExpression Listener { get; }
    }
}
