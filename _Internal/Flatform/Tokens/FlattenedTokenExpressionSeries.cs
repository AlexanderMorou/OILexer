using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Utilities.Collections;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedTokenExpressionSeries :
        ControlledStateDictionary<ITokenExpression, FlattenedTokenExpression>
    {
        private bool initialized = false;
        private ITokenExpressionSeries source;
        internal RegularLanguageState startState;
        private IControlledStateCollection<RegularLanguageState> exitStates;
        protected FlattenedTokenExpressionSeries(ITokenExpressionSeries source)
        {
            this.source = source;
        }
        internal void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
            for (int i = 0; i < this.source.Count; i++)
            {
                var item = new FlattenedTokenExpression(this.source[i]);
                this.Add(this.source[i], item);
                item.Initialize();
            }
        }

        public RegularLanguageState GetState()
        {
            if (this.startState == null)
                this.InitializeState();
            return this.startState;
        }

        public IControlledStateCollection<RegularLanguageState> GetExitStates()
        {
            if (this.exitStates == null)
                this.InitializeExitStates();
            return this.exitStates;
        }

        private void InitializeExitStates()
        {
            if (this.startState == null)
                this.InitializeState();
            this.exitStates = new ControlledStateCollection<RegularLanguageState>(this.startState.ObtainEdges().ToArray());
        }

        private void InitializeState()
        {
            if (!initialized)
                this.Initialize();
            RegularLanguageState state = null;
            foreach (var expression in this.Values)
            {
                if (state == null)
                    state = expression.GetState();
                else
                    state |= expression.GetState();
            }
            this.startState = state;
        }
    }
}
