﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    
    internal sealed class OilexerGrammarTokenGroupRegion :
        IOilexerGrammarRegion
    {
        private ITokenGroupItem groupItem;
        public OilexerGrammarTokenGroupRegion(ITokenGroupItem groupItem, long start, long end)
        {
            this.Start = (int)start;
            this.End = (int)end;
            this.groupItem = groupItem;
        }
        #region IOilexerGrammarRegion Members

        public int Start { get; private set; }

        public int End { get; private set; }

        public string Description { get { return this.groupItem.GetBodyString(); } }

        public string CollapseForm
        {
            get { return "..."; }
        }

        #endregion
    }
}
