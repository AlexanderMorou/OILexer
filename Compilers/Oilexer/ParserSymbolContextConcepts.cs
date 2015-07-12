using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class ParserSymbolContextConcepts
    {
        /// <summary>
        /// Returns the <see cref="ParserCompiler"/> which created the 
        /// <see cref="ParserSymbolContextConcepts"/>.
        /// </summary>
        public ParserCompiler Builder { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/>
        /// which unifies all rules under one concept.
        /// </summary>
        public IIntermediateInterfaceType RootRuleInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/>
        /// which unifies all tokens under one concept.
        /// </summary>
        public IIntermediateInterfaceType RootTokenInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/>
        /// which unifies all rules and tokens under one concept.
        /// </summary>
        public IIntermediateInterfaceType RootSymbolInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/>
        /// which specifies the basic concept behind a symbol
        /// stream.
        /// </summary>
        public IIntermediateInterfaceType SymbolStreamInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/> which 
        /// specifies the basic concept behind a token stream.
        /// </summary>
        public IIntermediateInterfaceType TokenStreamInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateInterfaceType"/>
        /// which denotes the identity ambiguities represent within
        /// the context stack.
        /// </summary>
        public IIntermediateInterfaceType RootProjectionInterface { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassType"/> which
        /// denotes the identity of symbols which make up portions
        /// of the parse stack.
        /// </summary>
        public IIntermediateInterfaceType IRootContextSymbolInterface { get; private set; }
    }
}
