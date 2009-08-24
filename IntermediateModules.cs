using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer
{
    /// <summary>
    /// Provides a series of <see cref="IIntermediateModule"/> instance implementations
    /// which associate with a single root <see cref="IIntermediateProject"/>.
    /// </summary>
    public class IntermediateModules :
        ControlledStateDictionary<string, IIntermediateModule>,
        IIntermediateModules
    {
        /// <summary>
        /// Data member for <see cref="Project"/>.
        /// </summary>
        private IIntermediateProject project;

        /// <summary>
        /// Creates a new <see cref="IntermediateModules"/> instance
        /// with the <see cref="Project"/> defined.
        /// </summary>
        /// <param name="project">The project the <see cref="IntermediateModules"/> belong to.</param>
        public IntermediateModules(IIntermediateProject project)
        {
            this.project = project;
            this.AddNew("RootModule");
        }

        #region IIntermediateModules Members

        /// <summary>
        /// Determines whether the <see cref="IIntermediateModule"/> is registered.
        /// </summary>
        /// <param name="module">The module to check registration status of.</param>
        /// <returns>true, if the module has been registered or was created
        /// with the current <see cref="IIntermediateModules"/>; false, otherwise.</returns>
        public bool IsModuleRegistered(IIntermediateModule module)
        {
            return this.Values.Contains(module);
        }

        /// <summary>
        /// Checks an unregistered module into the modules listing.
        /// </summary>
        /// <param name="module">The unregistered module that needs checked-in.</param>
        public void UnregisteredCheckIn(IIntermediateModule module)
        {
            if ((module.Project == null) || (module.Project != this.Project))
                throw new ArgumentException("Module does not belong to the same project.", "module");
            if (this.IsModuleRegistered(module))
                return;
            this.Add(module.Name, module);
        }

        /// <summary>
        /// Inserts a new <see cref="IIntermediateModule"/> instance into the 
        /// <see cref="IntermediateModules"/> dictionary with the name specified.
        /// </summary>
        /// <param name="name">The name of the new <see cref="IIntermediateModule"/>.</param>
        /// <returns>A new <see cref="IIntermediateModule"/> implementation instance.</returns>
        public IIntermediateModule AddNew(string name)
        {
            IIntermediateModule result = new IntermediateModule(this.Project, name);
            this.Add(result.Name, result);
            return result;
        }

        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the 
        /// <see cref="IntermediateModules"/> are assocaited to.
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
