using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Statements;

namespace Oilexer.Parser.Builder
{
    public interface IGDBuilder
    {
        IIntermediateProject Build(IParserResults<IGDFile> parserResults);
    }
}
