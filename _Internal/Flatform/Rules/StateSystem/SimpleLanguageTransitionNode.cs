using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
//using Oilexer._Internal.UI.Visualization;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    internal partial class SimpleLanguageTransitionNode // :
        //IVisualTransitionTableNode<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>
    {
        public class SourceSet :
            List<SourceElement>
        {
            public SourceSet()
            {
            }
            public SourceSet(SourceSet original)
                : base(original)
            {
            }
        }

        public SimpleLanguageBitArray Check { get; internal set; }
        public List<SimpleLanguageState> Targets { get; private set; }
        public SourceSet Sources { get; private set; }
        public SimpleLanguageTransitionNode(SimpleLanguageBitArray check, SimpleLanguageState target)
        {
            this.Check = check;
            this.Targets = new List<SimpleLanguageState>();
            this.Targets.Add(target);
            this.Sources = new SourceSet();
        }

        public SimpleLanguageTransitionNode(SimpleLanguageBitArray check, IEnumerable<SimpleLanguageState> targets)
        {
            if (check == null)
                throw new ArgumentNullException("check");
            this.Check = check;
            this.Targets = new List<SimpleLanguageState>();
            this.Targets.AddRange(targets);
            this.Sources = new SourceSet();
        }

        //#region IVisualTransitionTableNode<SimpleLanguageState,Node,SimpleLanguageBitArray> Members

        //IControlledStateCollection<SimpleLanguageState> IVisualTransitionTableNode<SimpleLanguageState, SimpleLanguageTransitionNode, SimpleLanguageBitArray>.Targets
        //{
        //    get
        //    {
        //        if (this._targets == null)
        //            this._targets = new ControlledStateCollection<SimpleLanguageState>(this.Targets);
        //        return this._targets;
        //    }
        //}

        //#endregion
    }
}
