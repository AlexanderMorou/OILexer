using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
#if SERIALIZE
using System.IO;
#endif
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class LargeFootprintEnumGroupItem
#if SERIALIZE
        : IEntry
#else
        <T>
#endif
    {
#if SERIALIZE
        public LargeFootprintEnumGroupItem()
        {
        }
#else
        private string name;
        private string summary;
        private string remarks;
        private IFieldMember valueMember;
#endif

#if SERIALIZE
        public LargeFootprintEnumGroupItem(string name, string summary, string remarks)
        {
            this.Name = name;
            this.Remarks = remarks;
            this.Summary = summary;
        }
#else
        public LargeFootprintEnumGroupItem(string name, string summary, string remarks, T element)
        {
            this.name = name;
            this.remarks = remarks;
            this.summary = summary;
            this.Element = element;
        }

        public LargeFootprintEnumGroupItem(string name, string summary, string remarks, T element, IFieldMember valueMember)
        {
            this.name = name;
            this.remarks = remarks;
            this.summary = summary;
            this.Element = element;
            this.valueMember = valueMember;
        }
#endif

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

        #region IEntry Members

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
            writer.Write(this.Summary);
            writer.Write(this.Remarks);
        }

        public void Deserialize(BinaryReader reader)
        {
            this.Name = reader.ReadString();
            this.Summary = reader.ReadString();
            this.Remarks = reader.ReadString();
        }

        #endregion
#else
        public T Element { get; private set; }
        public IFieldMember ValueMember { get { return this.valueMember; } }
#endif

    }
}
