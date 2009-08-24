using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.Reflection;
using Oilexer.Translation;

namespace Oilexer
{
    public struct ProjectDependencyReport
    {
        private static NameSpaceSorter nsSorter = new NameSpaceSorter();

        private ITypeReferenceCollection sourceData;
        private List<string> nameSpaceDependencies;
        private List<Assembly> compiledAssemblyReferences;
        private List<IIntermediateProject> intermediateAssemblyReferences;
        private bool initialized;

        private ITypeReferenceable target;
        private ICodeTranslationOptions options;
        public ProjectDependencyReport(ITypeReferenceCollection sourceData)
            : this()
        {
            this.sourceData = sourceData;
        }

        public ProjectDependencyReport(ITypeReferenceable target, ICodeTranslationOptions options) 
            : this()
        {
            this.initialized = false;
            if (target == null)
                throw new ArgumentNullException("target");
            this.target = target;
            this.options = options;
        }

        public void Begin()
        {
            if (this.initialized)
                return;
            this.initialized = true;
            if (this.sourceData != null)
                this.sourceData = DelveFurther(sourceData);
            else if (this.target != null && this.options != null)
            {
                ITypeReferenceCollection itc = new TypeReferenceCollection();
                target.GatherTypeReferences(ref itc, options);
                this.sourceData = DelveFurther(itc);
            }
        }

        public void Reduce()
        {
            if (!this.initialized)
                this.Begin();
            List<ITypeReference> result = new List<ITypeReference>();
            foreach (ITypeReference itr in this.sourceData)
                if (!result.Contains(itr))
                    result.Add(itr);
            this.sourceData = new TypeReferenceCollection(result.ToArray());
        }

        private static ITypeReferenceCollection DelveFurther(ITypeReferenceCollection itc)
        {
            ITypeReferenceCollection result = new TypeReferenceCollection();
            foreach (ITypeReference itr in itc)
                result.AddRange(DelveFurther(itr));
            return result;
        }

        private static ITypeReference[] DelveFurther(ITypeReference itr)
        {
            List<ITypeReference> result = new List<ITypeReference>();
            result.Add(itr);
            result.AddRange(DelveFurther(itr.TypeParameters));
            return result.ToArray();
        }

        public List<string> NameSpaceDependencies
        {
            get
            {
                if (this.nameSpaceDependencies == null)
                    this.InitializeNameSpaceDependencies();
                return this.nameSpaceDependencies;
            }
        }

        private void InitializeNameSpaceDependencies()
        {
            if (!this.initialized)
                Begin();
            List<string> result = new List<string>();
            foreach (ITypeReference itr in this.sourceData)
            {
                IType it = itr.TypeInstance;
                if (it is IDeclaredType)
                {
                    IDeclarationTarget idt = (IDeclarationTarget)it;
                    while (idt != null && (!(idt is INameSpaceDeclaration)))
                        idt = idt.ParentTarget;
                    if (idt is INameSpaceDeclaration)
                    {
                        if (!result.Contains(((INameSpaceDeclaration)(idt)).FullName))
                            result.Add(((INameSpaceDeclaration)(idt)).FullName);
                    }
                    else
                        continue;
                }
                else if (it is IExternType)
                {
                    if (((IExternType)(it)).Type.IsGenericParameter)
                        continue;
                    if ((!(result.Contains(((IExternType)(it)).Type.Namespace))))
                        result.Add(((IExternType)(it)).Type.Namespace);
                }
                else
                    continue;
            }
            result.Sort(nsSorter);
            this.nameSpaceDependencies = result;
        }


        public List<Assembly> CompiledAssemblyReferences
        {
            get
            {
                if (this.compiledAssemblyReferences == null)
                    this.InitializeCompiledAssemblyRefrences();
                return this.compiledAssemblyReferences;
            }
        }

        private void InitializeCompiledAssemblyRefrences()
        {
            if (!this.initialized)
                Begin();
            List<Assembly> result = new List<Assembly>();
            foreach (ITypeReference itr in this.sourceData)
            {
                IType it = itr.TypeInstance;
                if (it is IExternType)
                {
                    if (((IExternType)(it)).Type.IsGenericParameter)
                        continue;
                    if ((!(result.Contains(((IExternType)(it)).Type.Assembly))))
                        InsertAssemblyWithReferences(result, ((IExternType)(it)).Type.Assembly);
                }
                else
                    continue;
            }
            this.compiledAssemblyReferences = result;
        }

        private static void InsertAssemblyWithReferences(List<Assembly> result, Assembly a)
        {
            AssemblyName[] dependencies = a.GetReferencedAssemblies();
            if (!result.Contains(a))
                result.Add(a);
            else
                return;
            foreach (AssemblyName reference in dependencies)
            {
                Assembly referencedAssembly = Assembly.Load(reference);
                if (!result.Contains(referencedAssembly))
                {
                    InsertAssemblyWithReferences(result, referencedAssembly);
                    result.Add(referencedAssembly);
                }
            }
        }

        public List<IIntermediateProject> IntermediateAssemblyReferences
        {
            get
            {
                if (this.intermediateAssemblyReferences == null)
                    this.InitializeIntermediateAssemblyReferences();
                return this.intermediateAssemblyReferences;
            }
        }

        private void InitializeIntermediateAssemblyReferences()
        {
            if (!this.initialized)
                Begin();
            List<IIntermediateProject> result = new List<IIntermediateProject>();
            foreach (ITypeReference itr in this.sourceData)
            {
                IType it = itr.TypeInstance;
                if (it is IDeclaredType)
                {
                    if ((!(result.Contains(((IDeclaredType)it).Project))))
                        result.Add(((IDeclaredType)it).Project);
                }
                else
                    continue;
            }
            this.intermediateAssemblyReferences = result;
        }
        public ITypeReferenceCollection SourceData
        {
            get
            {
                return this.sourceData;
            }
        }

        private class NameSpaceSorter : IComparer<string>
        {
            #region IComparer<string> Members

            public int Compare(string x, string y)
            {
                bool xSystem = x.Length >= 6;
                if (xSystem)
                    xSystem = (x.Substring(0, 6) == "System");
                bool ySystem = y.Length >= 6;
                if (ySystem)
                    ySystem = (y.Substring(0, 6) == "System");
                if (!xSystem)
                {
                    xSystem = xSystem = x.Length >= 9;
                    if (xSystem)
                        xSystem = x.Substring(0, 9) == "Microsoft";
                }
                if (!ySystem)
                {
                    ySystem = ySystem = y.Length >= 9;
                    if (ySystem)
                        ySystem = y.Substring(0, 9) == "Microsoft";
                }
                if (xSystem && !ySystem)
                    return -1;
                else if (ySystem && !xSystem)
                    return 1;
                return x.CompareTo(y);
            }

            #endregion
        }

    }
}
