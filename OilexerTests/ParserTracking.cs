using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OilexerTests
{
    [TestClass]
    public class ParserTracking
    {
        [TestMethod]
        public void TestParser()
        {
#if DEBUG
            OILexerProgram.CallMainMethod(@"-v", @"C:\Projects\Code\C#\Abstraction\Build\x86\Debug\Samples\ToySharp\ToySharp.oilexer");
#else
            OILexerProgram.CallMainMethod(@"-v ""C:\Projects\Code\C#\Abstraction\Build\x86\Release\Samples\ToySharp\ToySharp.oilexer""");
#endif
        }
    }
}
