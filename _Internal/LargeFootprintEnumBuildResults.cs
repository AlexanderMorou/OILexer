using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    partial class 
#if !SERIALIZE
        LargeFootprintEnum<T>
#else
        LargeFootprintEnum
#endif
    {
        public struct FieldParentMapping
        {
            public IType Parent { get; private set; }
            public IFieldMember Member { get; private set; }

            internal FieldParentMapping(IType parent, IFieldMember member)
                : this()
            {
                this.Parent = parent;
                this.Member = member;
            }
        }
        public struct PropertyParentMapping
        {
            public IType Parent { get; private set; }
            public IPropertyMember Member { get; private set; }

            internal PropertyParentMapping(IType parent, IPropertyMember member)
                : this()
            {
                this.Parent = parent;
                this.Member = member;
            }
        }
        public struct BuildResults
        {
            public IStructType ResultantType { get; private set; }
            public IClassType  ResultantSetsType { get; private set; }
#if !SERIALIZE //Easier to read
            public IDictionary<LargeFootprintEnumGroupItem<T>, FieldParentMapping> MemberMapping { get; private set; }
            public IDictionary<LargeFootprintEnumGroupItem<T>, PropertyParentMapping> TargetMapping { get; private set; }

            public IDictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> FieldMapping { get; private set; }
            public IDictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> NoneMapping { get; private set; }
            public IDictionary<LargeFootprintEnumGroup<T>, PropertyParentMapping> PropertyMapping { get; private set; }

            public BuildResults(IDictionary<LargeFootprintEnumGroupItem<T>, FieldParentMapping> memberMapping, IDictionary<LargeFootprintEnumGroupItem<T>, PropertyParentMapping> targetMapping, IStructType resultantType, IClassType resultantSetsType, IDictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> fieldMapping, IDictionary<LargeFootprintEnumGroup<T>, PropertyParentMapping> propertyMapping, IDictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> noneMapping)
                : this()
            {
                this.MemberMapping = memberMapping;
                this.TargetMapping = targetMapping;
                this.ResultantType = resultantType;
                this.ResultantSetsType = resultantSetsType;
                this.FieldMapping = fieldMapping;
                this.PropertyMapping = propertyMapping;
                this.NoneMapping = noneMapping;
            }
#else
            public IDictionary<LargeFootprintEnumGroupItem   , FieldParentMapping> MemberMapping { get; private set; }
            public IDictionary<LargeFootprintEnumGroupItem   , PropertyParentMapping> TargetMapping { get; private set; }
            public BuildResults(IDictionary<LargeFootprintEnumGroupItem, FieldParentMapping> memberMapping, IDictionary<LargeFootprintEnumGroupItem, PropertyParentMapping> targetMapping, IStructType resultantType, IClassType resultantSetsType)
                : this()
            {
                this.MemberMapping = memberMapping;
                this.TargetMapping = targetMapping;
                this.ResultantType = resultantType;
                this.ResultantSetsType = resultantSetsType;
            }
#endif
        }
    }
}
