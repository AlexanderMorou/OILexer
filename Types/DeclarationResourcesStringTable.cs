using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;

namespace Oilexer.Types
{
    public class DeclarationResourcesStringTable :
        ControlledStateDictionary<string, IDeclarationResourcesStringTableEntry>,
        IDeclarationResourcesStringTable
    {
        private IDictionary<string, string> originalKeys = new Dictionary<string, string>();
        /// <summary>
        /// Data member for <see cref="Resources"/>.
        /// </summary>
        private IDeclarationResources resources = null;
        internal DeclarationResourcesStringTable(IDeclarationResources resources)
        {
            this.resources = resources;
        }

        internal DeclarationResourcesStringTable(DeclarationResourcesStringTable sibling, IDictionary<string, string> originalKeys, IDeclarationResources resources)
            : base(sibling)
        {
            this.originalKeys = originalKeys;
            this.resources = resources;
        }

        #region IDeclarationResourcesStringTable Members

        public IDeclarationResourcesStringTableEntry Add(string name, string value)
        {
            IDeclarationResourcesStringTableEntry entry = new DeclarationResourcesStringTableEntry(this.resources, name, value);
            string newKey = name.ToLower();
            if (!(this.originalKeys.ContainsKey(name) || base.ContainsKey(name.ToLower())))
            {
                originalKeys.Add(name, newKey);
                base._Add(newKey, entry);
            }
            else if (!this.originalKeys.ContainsKey(name))
            {
                int i = 0;
                while (base.ContainsKey(newKey))
                    newKey = string.Format("{0}{1}", name.ToLower(), i);
                originalKeys.Add(name, newKey);
                base._Add(newKey, entry);
                entry.Name = newKey;
            }
            return entry;
        }

        protected override IDeclarationResourcesStringTableEntry OnGetThis(string key)
        {
            return base[originalKeys[key]];
        }

        public override sealed bool ContainsKey(string key)
        {
            return this.originalKeys.ContainsKey(key);
        }

        public new void Remove(string name)
        {
            this[name].Dispose();
            base._Remove(name);
        }

        public new void Clear()
        {
            base._Clear();
        }

        public void Rebuild()
        {
            foreach (IDeclarationResourcesStringTableEntry idrste in this.Values)
                idrste.Rebuild();
        }

        #endregion


        #region IDeclarationResourcesStringTable Members


        public IDeclarationResourcesStringTable GetPartialClone(IDeclarationResources targetDeclaration)
        {
            return new DeclarationResourcesStringTable(this, this.originalKeys, targetDeclaration);
        }

        #endregion

    }
}
