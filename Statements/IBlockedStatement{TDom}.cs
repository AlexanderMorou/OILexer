using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Statements
{
	public interface IBlockedStatement<TDom> :
        IStatement<TDom>,
        IBlockedStatement
        where TDom :
            CodeStatement
    {
    }
}
