using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer
{
    public interface IIntermediateModules :
        IControlledStateDictionary<string, IIntermediateModule>
    {
        /// <summary>
        /// Determines whether the <see cref="IIntermediateModule"/> is registered.
        /// </summary>
        /// <param name="module">The module to check registration status of.</param>
        /// <returns>true, if the module has been registered or was created
        /// with the current <see cref="IIntermediateModules"/>; false, otherwise.</returns>
        bool IsModuleRegistered(IIntermediateModule module);
        /// <summary>
        /// Checks an unregistered module into the modules listing.
        /// </summary>
        /// <param name="module">The unregistered module that needs checked-in.</param>
        void UnregisteredCheckIn(IIntermediateModule module);
        /// <summary>
        /// Inserts a new <see cref="IIntermediateModule"/> instance into the 
        /// <see cref="IIntermediateModules"/> dictionary with the name specified.
        /// </summary>
        /// <param name="name">The name of the new <see cref="IIntermediateModule"/>.</param>
        /// <returns>A new <see cref="IIntermediateModule"/> implementation instance.</returns>
        IIntermediateModule AddNew(string name);
        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the 
        /// <see cref="IIntermediateModules"/> are assocaited to.
        /// </summary>
        IIntermediateProject Project { get; }
    }
}
