using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AllenCopeland.Abstraction.Slf._Internal.Ast;

namespace OilexerTests
{
    [TestClass]
    public class ContextTracking
    {
        [TestMethod]
        public void TestObjectModelContext()
        {
            var dictionary = ObjectModelContext.GetFluxContentDictionary<string, string>(typeof(ContextTracking), "Test");
            var dictionary2 = ObjectModelContext.GetFluxContentDictionary<string, string, string>(typeof(ContextTracking), "Test");
            ObjectModelContext.ObjectDisposed(typeof(ContextTracking));
        }
    }
}
