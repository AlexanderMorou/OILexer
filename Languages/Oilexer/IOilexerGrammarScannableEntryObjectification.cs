using AllenCopeland.Abstraction.Slf.Ast;
using System;
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public interface IOilexerGrammarScannableEntryObjectification
    {
        IIntermediateClassType Class { get; }
        IOilexerGrammarScannableEntry Entry { get; }
        IIntermediateInterfaceType RelativeInterface { get; }
    }
}
