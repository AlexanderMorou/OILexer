using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Oil;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    using RuleTree = KeyedTree<IProductionRuleEntry, GDFileObjectRelationalMap.RuleObjectification>;
    using RuleTreeNode = KeyedTreeNode<IProductionRuleEntry, GDFileObjectRelationalMap.RuleObjectification>;

    public class GDFileObjectRelationalMap :
        ControlledStateDictionary<IScannableEntry, IEntryObjectRelationalMap>,
        IGDFileObjectRelationalMap
    {
        internal class RuleObjectification
        {
            
            public RuleObjectification(IIntermediateInterfaceType @interface, IIntermediateClassType @class, IProductionRuleEntry entry)
            {
                this.Class = @class;
                this.RelativeInterface = @interface;
                this.Entry = entry;
            }

            public IIntermediateInterfaceType RelativeInterface { get; private set; }
            public IIntermediateClassType Class { get; private set; }
            public IProductionRuleEntry Entry { get; private set; }
        }
        
        public GDFileObjectRelationalMap(IGDFile source, IControlledStateDictionary<IProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateAssembly project)
        {
            this.Source = source;
            this.Process(ruleStates, project);
        }

        private IEnumerable<T> FilterScannable<T>(IGDFile target)
            where T :
                class,
                IScannableEntry
        {
            return (from t in target
                    let tItem = t as T
                    where tItem != null
                    orderby tItem.Name
                    select tItem);
        }

        private void Process(IControlledStateDictionary<IProductionRuleEntry, SyntacticalDFARootState> ruleStates, IIntermediateAssembly project)
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

        private RuleTreeNode BuildVariation(RuleTreeNode currentNode, RuleTreeNode secondaryRoot, IIntermediateAssembly project)
        {
            if (currentNode.ContainsKey(secondaryRoot.Value.Entry))
                return currentNode[secondaryRoot.Value.Entry];
            var currentSubVariant = currentNode.Value.Class.Classes.Add(string.Format("_{0}", secondaryRoot.Value.Entry.Name));
            currentSubVariant.BaseType = currentNode.Value.Class;
            currentSubVariant.ImplementedInterfaces.ImplementInterfaceQuick(secondaryRoot.Value.RelativeInterface);
            return currentNode.Add(secondaryRoot.Value.Entry, new RuleObjectification(null, currentSubVariant, secondaryRoot.Value.Entry));
        }

        private RuleTreeNode CheckRootVariant(IIntermediateAssembly project, RuleTree ruleVariants, IProductionRuleEntry rule)
        {
            if (ruleVariants.ContainsKey(rule))
                return ruleVariants[rule];
            else
                return ruleVariants.Add(rule, BuildRootObjectification(rule, project));
        }

        private RuleObjectification BuildRootObjectification(IProductionRuleEntry entry, IIntermediateAssembly project)
        {
            const string defaultNamespaceSubspace = "Cst";
            var dNamespace = project.DefaultNamespace;
            if (dNamespace.Namespaces.ContainsKey(defaultNamespaceSubspace))
                dNamespace = dNamespace.Namespaces[defaultNamespaceSubspace];
            else
                dNamespace = dNamespace.Namespaces.Add(defaultNamespaceSubspace);
            const string nameFormat = "{0}{1}{2}";
            const string iNameFormat = "I" + nameFormat;
            var iFace = dNamespace.Interfaces.Add(string.Format(iNameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
            var bClass = dNamespace.Classes.Add(string.Format(nameFormat, this.Source.Options.RulePrefix, entry.Name, this.Source.Options.RuleSuffix));
            bClass.ImplementedInterfaces.ImplementInterfaceQuick(iFace);
            return new RuleObjectification(iFace, bClass, entry);
        }

        #region IGDFileObjectRelationalMap Members

        public IGDFile Source { get; private set; }

        #endregion

    }
}
