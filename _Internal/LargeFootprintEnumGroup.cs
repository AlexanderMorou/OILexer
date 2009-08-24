using System;
using System.Collections.Generic;
using System.Text;
#if SERIALIZE
using System.IO;
#endif
using Oilexer.Utilities.Collections;
using Oilexer.Types.Members;
using Oilexer.Types;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class LargeFootprintEnumGroup
#if SERIALIZE
        : Table<LargeFootprintEnumGroupItem>
#else
        <T> :
        ControlledStateCollection<LargeFootprintEnumGroupItem<T>>
#endif
    {

#if SERIALIZE

        public LargeFootprintEnumGroup()
        {
        }
#else
        private string name;
        private string summary;
        private string remarks;
        private IEnumeratorType valuesType;
#endif
        public LargeFootprintEnumGroup(string name, string summary, string remarks
#if !SERIALIZE
            , IEnumeratorType valuesType
#endif
            )
        {
#if SERIALIZE
            this.Name = name;
            this.Remarks = remarks;
            this.Summary = summary;
#else
            this.name = name;
            this.remarks = remarks;
            this.summary = summary;
            this.valuesType = valuesType;
#endif
        }


        public string Name
        {
#if SERIALIZE
            get;
            set; 
#else
            get
            {
                return this.name;
            }
#endif
        }
        public string Summary
        {
#if SERIALIZE
            get;
            set;
#else
            get
            {
                return this.summary;
            }
#endif
        }
        public string Remarks
        {
#if SERIALIZE
            get;
            set;
#else
            get
            {
                return this.remarks;
            }
#endif
        }

#if SERIALIZE

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
            writer.Write(this.Summary);
            writer.Write(this.Remarks);
            base.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            this.Name = reader.ReadString();
            this.Summary = reader.ReadString();
            this.Remarks = reader.ReadString();
            base.Deserialize(reader);
        }

#else
        public LargeFootprintEnumGroupItem<T> Add(T element, string name, string summary, string remarks)
        {
            var result = new LargeFootprintEnumGroupItem<T>(name, summary, remarks, element);
            base.baseCollection.Add(result);
            return result;
        }

        public LargeFootprintEnumGroupItem<T> Add(T element, string name, string summary, string remarks, IFieldMember valueMember)
        {
            var result = new LargeFootprintEnumGroupItem<T>(name, summary, remarks, element, valueMember);
            base.baseCollection.Add(result);
            return result;
        }

        public IEnumeratorType ValuesType { get { return this.valuesType; } }
#endif
    }
}
