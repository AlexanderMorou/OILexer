using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
    public interface ILabelStatement :
        IStatement<CodeLabeledStatement>
    {
        /// <summary>
        /// Returns/sets the name of the label.
        /// </summary>
        string Name { get; set; }
        CodeGotoStatement GetCodeDomGoTo();
        /// <summary>
        /// Obtains a <see cref="IGoToLabelStatement"/> associated to the current 
        /// <see cref="ILabelStatement"/>, contained within the 
        /// <paramref name="sourceBlock"/> provided.</summary>
        /// <param name="sourceBlock">The <see cref="IStatementBlock"/> in which the
        /// resulted <see cref="IGoToLabelStatement"/> will be contained.</param>
        /// <returns>A new <see cref="IGoToLabelStatement"/> associated to the current 
        /// <see cref="ILabelStatement"/>, contained within
        /// the <paramref name="sourceBlock"/> provided.</returns>
        IGoToLabelStatement GetGoTo(IStatementBlock sourceBlock);
    }
}
