using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer._Internal.Inlining;
using Oilexer.Parser.GDFileData;
using Oilexer.FiniteAutomata.Rules;

namespace Oilexer.FiniteAutomata.Tokens
{
    partial class RegularLanguageTokenTable
    {
        internal class Target :
            List<InlinedTokenEntry>
        {
            public Target(Target baseTarget)
                : base(baseTarget)
            {

            }

            public Target()
            {
            }

            public T ObtainWorkingStateMachineData<T>(GrammarVocabulary workingSet, Dictionary<InlinedTokenEntry, IRegularLanguageStateMachine> stateMachines, Func<Tuple<bool[], List<IRegularLanguageStateMachine>>, T> yielder)
            {
                var breakdown = workingSet.Breakdown;
                var resultMachineData = new List<IRegularLanguageStateMachine>();
                foreach (var tokenEntry in breakdown.ConstantTokens)
                {
                    var entry = (InlinedTokenEntry)tokenEntry.Source;
                    if (this.Contains(entry) && stateMachines.ContainsKey(entry))
                        resultMachineData.Add(stateMachines[entry]);
                }
                foreach (var tokenEntry in breakdown.CaptureTokens)
                {
                    var entry = (InlinedTokenEntry)tokenEntry.Source;
                    if (this.Contains(entry) && stateMachines.ContainsKey(entry))
                        resultMachineData.Add(stateMachines[entry]);
                }
                foreach (var tokenEntry in breakdown.LiteralSeriesTokens)
                {
                    var entry = (InlinedTokenEntry)tokenEntry.Key;
                    if (this.Contains(entry) && stateMachines.ContainsKey(entry))
                        resultMachineData.Add(stateMachines[entry]);
                }
                bool[] machineEnabledState = new bool[resultMachineData.Count];
                for (int i = 0; i < machineEnabledState.Length; i++)
                    machineEnabledState[i] = true;
                return yielder(new Tuple<bool[], List<IRegularLanguageStateMachine>>(machineEnabledState, resultMachineData));
            }
        }
    }
}
