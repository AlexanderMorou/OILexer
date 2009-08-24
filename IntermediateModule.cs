using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;

namespace Oilexer
{
    public class IntermediateModule :
        IIntermediateModule
    {
        /// <summary>
        /// Data member for <see cref="Name"/>
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="Project"/>.
        /// </summary>
        private IIntermediateProject project;

        public IntermediateModule(IIntermediateProject project, string name)
        {
            this.name = name;
            this.project = project;
        }

        #region IIntermediateModule Members

        /// <summary>
        /// Returns/sets the name of the <see cref="IntermediateModule"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Returns an enumerable instance which contains the <see cref="IDeclaredType"/> instance
        /// implementations which relate to the current <see cref="IntermediateModule"/>.
        /// </summary>
        public IEnumerable<IDeclaredType> DeclaredTypes
        {
            get
            {
                if (this.Project == null)
                    return new IDeclaredType[0];
                List<IDeclaredType> result = new List<IDeclaredType>();
                List<IIntermediateProject> iip = new List<IIntermediateProject>();
                GatherTypes(result, iip, this.Project);
                GatherTypes(result, iip, this.Project.NameSpaces);
                return result.ToArray();
            }
        }

        private void GatherTypes(List<IDeclaredType> target, List<IIntermediateProject> projectPartials, INameSpaceDeclarations nameSpaces)
        {
            foreach (INameSpaceDeclaration insd in nameSpaces.Values)
            {
                GatherTypes(target, projectPartials, (ITypeParent)insd);
                GatherTypes(target, projectPartials, insd.ChildSpaces);
            }
        }

        private void GatherTypes(List<IDeclaredType> target, List<IIntermediateProject> projectPartials, ITypeParent typeParent)
        {
            GatherTypes(target, projectPartials, typeParent.Classes);
            GatherTypes(target, projectPartials, typeParent.Delegates);
            GatherTypes(target, projectPartials, typeParent.Enumerators);
            GatherTypes(target, projectPartials, typeParent.Interfaces);
            GatherTypes(target, projectPartials, typeParent.Structures);
        }

        private void GatherTypes<TItem, TDom>(List<IDeclaredType> target, List<IIntermediateProject> projectPartials, IDeclaredTypes<TItem, TDom> declaredTypes)
            where TItem :
                IDeclaredType<TDom>
            where TDom :
                CodeTypeDeclaration
        {
            /* *
             * Doesn't go beyond the top-level types, because most .NET Compilers 
             * flatten object hierarchies (nested types) that span across multiple modules.
             * That is, if Type A is in Module 1 and Type A+B is in Module 2
             * A+B end up both in Module 1.
             * */
            if (declaredTypes == null)
                return;
            foreach (IDeclaredType<TDom> type in declaredTypes.Values)
                if (type.Module == this && (!projectPartials.Contains(type.Project)))
                {
                    target.Add(type);
                    projectPartials.Add(type.Project);
                    if (type is ITypeParent)
                        GatherTypes(target, projectPartials, (ITypeParent)type);
                }
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the 
        /// <see cref="IntermediateModule"/> is assocaited to.
        /// </summary>
        public IIntermediateProject Project
        {
            get
            {
                return this.project;
            }
        }

        #endregion

    }
}
