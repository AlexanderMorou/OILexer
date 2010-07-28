using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.Parser
{
    public class GDPreprocessorIfDirectiveRegion :
        IGDRegion
    {
        private IPreprocessorIfDirective directive;
        public GDPreprocessorIfDirectiveRegion(IPreprocessorIfDirective directive, long bodyStart, long bodyEnd)
        {
            this.directive = directive;
            this.Start = (int)bodyStart;
            this.End = (int)bodyEnd;
        }


        #region IGDRegion Members

        public int Start { get; private set; }

        public int End { get; private set; }

        public string Description
        {
            get
            {
                return directive.Body.ToString();
            }
        }

        public string CollapseForm
        {
            get
            {
                return "...";
            }
        }

        #endregion
    }
}
