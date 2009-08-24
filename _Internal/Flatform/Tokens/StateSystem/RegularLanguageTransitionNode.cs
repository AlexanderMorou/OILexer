using System;
using System.Collections.Generic;
using System.Text;
//using Oilexer._Internal.UI.Visualization;
using Oilexer.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    internal partial class RegularLanguageTransitionNode //:
       // IVisualTransitionTableNode<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>
    {
        internal RegularLanguageBitArray check;
        #region RegularLanguageTransitionNode Properties
        public RegularLanguageBitArray Check { get { return this.check; } internal set { this.check = value; } }
        public List<RegularLanguageState> Targets { get; private set; }
        public List<SourceElement> Sources { get; private set; }
        #endregion

        #region RegularLanguageTransitionNode Constructors
        public RegularLanguageTransitionNode(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets, IEnumerable<SourceElement> sources)
        {
            this.check = check;
            this.Targets = new List<RegularLanguageState>(targets);
            this.Sources = new List<SourceElement>(sources);
        }

        public RegularLanguageTransitionNode(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets, SourceElement source)
        {
            this.check = check;
            this.Targets = new List<RegularLanguageState>(targets);
            this.Sources = new List<SourceElement>();
            this.Sources.Add(source);
        }

        public RegularLanguageTransitionNode(RegularLanguageBitArray check, RegularLanguageState target, SourceElement source)
        {
            this.check = check;
            this.Targets = new List<RegularLanguageState>();
            this.Sources = new List<SourceElement>();
            this.Targets.Add(target);
            this.Sources.Add(source);
        }

        public RegularLanguageTransitionNode(RegularLanguageBitArray check, RegularLanguageState target)
        {
            this.check = check;
            this.Targets = new List<RegularLanguageState>();
            this.Sources = new List<SourceElement>();
            this.Targets.Add(target);
        }

        public RegularLanguageTransitionNode(RegularLanguageBitArray check, IEnumerable<RegularLanguageState> targets)
        {
            this.check = check;
            this.Targets = new List<RegularLanguageState>(targets);
            this.Sources = new List<SourceElement>();
        }
        #endregion

/*
        #region IVisualTransitionTableNode<RegularLanguageState,RegularLanguageTransitionNode,RegularLanguageBitArray> Members

        IControlledStateCollection<RegularLanguageState> IVisualTransitionTableNode<RegularLanguageState, RegularLanguageTransitionNode, RegularLanguageBitArray>.Targets
        {
            get {
                if (this._targets == null)
                    this._targets = new ControlledStateCollection<RegularLanguageState>(this.Targets);
                return this._targets;
            }
        }

        #endregion
*/
    }
}
