using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Types;
using System.Collections.ObjectModel;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class PropertyBuilderMember
    {
        public IFieldMember DataMember { get; private set; }
        public IPropertyMember PropertyMember { get; private set; }
        public IMemberParentType Parent { get; private set; }
        private string name;
        public PropertyBuilderMember(string name, IMemberParentType parent, ITypeReference type)
        {
            this.name = name;
            this.Parent = parent;
            this.DataMember = parent.Fields.AddNew(new TypedName(parent.Fields.GetUniqueName(GetFieldName(name)), type));
            this.PropertyMember = parent.Properties.AddNew(new TypedName(parent.Properties.GetUniqueName(name), type));
        }

        private static string GetFieldName(string baseName)
        {
            return string.Format("{0}{1}{2}", '_', char.ToLower(baseName[0]), baseName.Substring(1));
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (name == value ||
                    name == PropertyMember.Name)
                    return;
                this.name = value;
                this.DataMember.Name = this.Parent.Fields.GetUniqueName(GetFieldName(value));
                this.PropertyMember.Name = this.Parent.Properties.GetUniqueName(value);
            }
        }
    }

    internal class PropertyBuilder :
        Collection<PropertyBuilderMember>
    {
        public IMemberParentType Parent { get; private set; }
        public PropertyBuilder(IMemberParentType parent)
        {
            this.Parent = parent;
        }

        public PropertyBuilderMember Add(string name, ITypeReference type)
        {
            var result = new PropertyBuilderMember(name, this.Parent, type);
            this.Add(result);
            return result;
        }
    }
}
