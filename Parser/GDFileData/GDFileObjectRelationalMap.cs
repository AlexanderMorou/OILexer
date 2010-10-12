using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Oilexer.Types;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser;
using Oilexer.Utilities.Collections;
using Oilexer.FiniteAutomata.Rules;
using Oilexer;

namespace Oilexer.Parser.GDFileData
{
    using RuleTreeNode = KeyedTreeNode<IProductionRuleEntry, GDFileObjectRelationalMap.RuleObjectification>;
    using RuleTree = KeyedTree<IProductionRuleEntry, GDFileObjectRelationalMap.RuleObjectification>;

    public class GDFileObjectRelationalMap :
        ControlledStateDictionary<IScannableEntry, IEntryObjectRelationalMap>,
        IGDFileObjectRelationalMap
    {
        internal class RuleObjectification
        {
            
            public RuleObjectification(IInterfaceType @interface, IClassType @class, IProductionRuleEntry entry)
            {
                this.Class = @class;
                this.RelativeInterface = @interface;
                this.Entry = entry;
            }

            public IInterfaceType RelativeInterface { get; private set; }
            public IClassType Class { get; private set; }
            public IProductionRuleEntry Entry { get; private set; }
        }
        
        public GDFileObjectRelationalMap(IGDFile source, IControlledStateDictionary<IProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateProject project)
        {
            this.Source = source;
            this.Process(ruleStates, project.Partials.AddNew());
        }

        private void Process(IControlledStateDictionary<IProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateProject project)
        {
            var tokens = (from t in Source
                          let token = t as ITokenEntry
                          where token != null
                          orderby token.Name
                          select token).ToArray();
            var rules = (from r in Source
                         let rule = r as IProductionRuleEntry
                         where rule != null
                         orderby rule.Name
                         select rule).ToArray();
            var ruleDependencyGraph = (from rulePrimary in rules
                                       from ruleSecondary in rules
                                       where ruleSecondary.ElementsAreChildren
                                       let secondaryState = ruleStates[ruleSecondary]
                                       where secondaryState.OutTransitions.FullCheck.Breakdown.Rules.Any(p => p.Source == rulePrimary)
                                       group ruleSecondary by rulePrimary).ToArray();

            RuleTree ruleVariants = new RuleTree();
            
            IProductionRuleEntry[] emptySet = new IProductionRuleEntry[0];
            foreach (var rule in from rule in rules
                                 where !ruleDependencyGraph.Any(dependency => dependency.Key == rule)
                                 select rule)
            {
                IRuleEntryObjectRelationalMap ruleORM = new RuleEntryObjectRelationalMap(emptySet, this, rule);
                this._Add(rule, ruleORM);
            }
            foreach (var ruleDependency in ruleDependencyGraph)
            {
                IRuleEntryObjectRelationalMap ruleORM = new RuleEntryObjectRelationalMap(ruleDependency.ToArray(), this, ruleDependency.Key);
                this._Add(ruleDependency.Key, ruleORM);
            }
            foreach (var rule in rules)
            {
                RuleTreeNode startingNode = CheckRootVariant(project, ruleVariants, rule);
                if (rule.ElementsAreChildren)
                    continue;
                foreach (var variation in (this[rule] as IRuleEntryObjectRelationalMap).Variations)
                {
                    var currentNode = startingNode;
                    foreach (var variationElement in variation.Reverse().Skip(1) /* First is always the current. */)
                    {
                        var currentRootVariant = CheckRootVariant(project, ruleVariants, variationElement);
                        currentNode = BuildVariation(currentNode, currentRootVariant, project);
                    }
                }
            }
            
        }

        private RuleTreeNode BuildVariation(RuleTreeNode currentNode, RuleTreeNode secondaryRoot, IIntermediateProject project)
        {
            if (currentNode.ContainsKey(secondaryRoot.Value.Entry))
                return currentNode[secondaryRoot.Value.Entry];
            var currentSubVariant = currentNode.Value.Class.Classes.AddNew(string.Format("_{0}", secondaryRoot.Value.Entry.Name));
            currentSubVariant.BaseType = currentNode.Value.Class.GetTypeReference();
            currentSubVariant.ImplementsList.Add(secondaryRoot.Value.RelativeInterface);
            return currentNode.Add(secondaryRoot.Value.Entry, new RuleObjectification(null, currentSubVariant, secondaryRoot.Value.Entry));
        }

        private RuleTreeNode CheckRootVariant(IIntermediateProject project, RuleTree ruleVariants, IProductionRuleEntry rule)
        {
            if (ruleVariants.ContainsKey(rule))
                return ruleVariants[rule];
            else
                return ruleVariants.Add(rule, BuildRootObjectification(rule, project));
        }

        private RuleObjectification BuildRootObjectification(IProductionRuleEntry entry, IIntermediateProject project)
        {
            const string cstModule = "Cst";
            IIntermediateModule module;
            if (project.Modules.ContainsKey(cstModule))
                module = project.Modules[cstModule];
            else
                module = project.Modules.AddNew(cstModule);
            var dNamespace = project.DefaultNameSpace;
            const string nameFormat = "{0}{1}{2}";
            const string iNameFormat = "I" + nameFormat;
            //dNamespace = dNamespace.Partials.AddNew();
            var iFace = dNamespace.Interfaces.AddNew(string.Format(iNameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
            iFace.Module = module;
            iFace.AccessLevel = DeclarationAccessLevel.Public;
            IClassType bClass = null;
            if (!entry.ElementsAreChildren)
            {
                bClass = dNamespace.Classes.AddNew(string.Format(nameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
                bClass.Module = module;
                bClass.AccessLevel = DeclarationAccessLevel.Public;
                bClass.ImplementsList.Add(iFace);
            }
            return new RuleObjectification(iFace, bClass, entry);
        }

        #region IGDFileObjectRelationalMap Members

        public IGDFile Source { get; private set; }

        #endregion

    }
}
