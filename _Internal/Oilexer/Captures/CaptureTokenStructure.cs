using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal class CaptureTokenStructure :
        ControlledDictionary<string, ICaptureTokenStructuralItem>,
        ICaptureTokenStructure
    {
        private string name;
        private string resultedTypeName;
        private bool groupOptional;
        internal HashList<HashList<string>> structures = new HashList<HashList<string>>();

        public CaptureTokenStructure()
        {
            this.Sources = new ControlledCollection<ITokenSource>();
        }
        #region ICaptureTokenStructure Members

        public ICaptureTokenStructure Union(ICaptureTokenStructure second)
        {
            if (this.Count == 0)
                return second;
            else if (second.Count == 0)
                return this;
            var tokenGrpSource =
                (ITokenGroupItem)second.Sources.Where(k => k is ITokenGroupItem).FirstOrDefault();
            var result = new CaptureTokenStructure();
            var left = this.Keys.Except(second.Keys).ToArray();
            var middle = this.Keys.Intersect(second.Keys).ToArray();
            var right = second.Keys.Except(this.Keys).ToArray();
            foreach (var element in left)
                result._Add(element, this[element]);
            foreach (var element in middle)
            {
                var leftElement = this[element];
                var rightElement = second[element];
                var middleElement = leftElement.Union(rightElement);
                result._Add(element, middleElement);
            }
            foreach (var element in right)
            {
                var rStructure = second[element];
                result._Add(element, rStructure);
            }

            if (this.ResultType == second.ResultType)
                result.ResultType = this.ResultType;
            else
                result.ResultType = ResultedDataType.ComplexType;
            result.ResultedTypeName = this.ResultedTypeName ?? second.ResultedTypeName;
            ((ControlledCollection<ITokenSource>)(result.Sources)).AddRange(this.Sources.ToArray());
            ((ControlledCollection<ITokenSource>)(result.Sources)).AddRange(second.Sources.ToArray());
            result.Optional = this.Optional || second.Optional;

            return result;
        }

        public ICaptureTokenStructure Concat(ICaptureTokenStructuralItem item)
        {
            var k = (from s in item.Sources
                     let sN = s as INamedFiniteAutomataSource
                     where sN != null && !string.IsNullOrEmpty(sN.Name)
                     select sN).FirstOrDefault();

            if (item.ResultType == ResultedDataType.PassThrough)
                return this.Union((ICaptureTokenStructure)item);
            else if (k == null)
                return this;

            var result = new CaptureTokenStructure();
            int? offsetIndex = null;
            foreach (var element in this)
            {
                if (k.Name == element.Value.Name)
                {
                    if (element.Value.BucketIndex == null)
                        element.Value.BucketIndex = offsetIndex = 1;
                    else
                        offsetIndex = element.Value.BucketIndex;
                    result._Add(element.Value.BucketName, element.Value);
                }
                else
                    result._Add(element.Key, element.Value);
            }
            if (!result.ContainsKey(k.Name))
            {
                if (offsetIndex == null)
                    result._Add(k.Name, item);
                else
                {
                    item.BucketIndex = offsetIndex + 1;
                    result._Add(item.BucketName, item);
                }
            }
            var first = this.Values.FirstOrDefault();
            if (this.Count > 0 && first != null && (first.ResultType == ResultedDataType.FlagEnumerationItem || first.ResultType == ResultedDataType.EnumerationItem))
            {
                if (first.ResultType == ResultedDataType.EnumerationItem)
                    first.ResultType = ResultedDataType.FlagEnumerationItem;
                if (item.ResultType == ResultedDataType.EnumerationItem)
                    item.ResultType = ResultedDataType.FlagEnumerationItem;
            }
            ResultedDataType resultDataType = ResultedDataType.Enumeration;
            foreach (var value in result.Values)
                if (resultDataType == ResultedDataType.Enumeration &&
                  !(value.ResultType == ResultedDataType.EnumerationItem ||
                    value.ResultType == ResultedDataType.FlagEnumerationItem))
                    resultDataType = ResultedDataType.ComplexType;
            result.ResultedTypeName = this.ResultedTypeName;
            result.ResultType = resultDataType;
            return result;
        }

        #endregion

        #region ICaptureTokenStructuralItem Members

        public int Rank { get; set; }

        public IControlledCollection<ITokenSource> Sources { get; private set; }

        public ResultedDataType ResultType { get; set; }

        public ICaptureTokenStructuralItem Union(ICaptureTokenStructuralItem rightElement)
        {
            if (rightElement is CaptureTokenStructure)
            {
                return this.Union((CaptureTokenStructure)rightElement);
            }
            else
            {
                var rightElementStructure = new CaptureTokenStructure();
                rightElementStructure.Concat(rightElement);
                return this.Union(rightElementStructure);
            }
        }

        #endregion

        #region ICaptureTokenStructuralItem Members


        public string Name
        {
            get
            {
                return (from s in this.Sources
                        let iti = s as ITokenItem
                        where iti != null
                        select iti.Name).FirstOrDefault();
            }
        }

        #endregion


        public string BucketName
        {
            get
            {
                return this.BucketIndex == null ? this.Name : string.Format("{0}{1}", this.Name, this.BucketIndex);
            }
        }
        public int? BucketIndex { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.ResultType, this.BucketName);
        }

        public string ResultedTypeName
        {
            get
            {
                if (this.resultedTypeName == null)
                    return this.Name;
                return this.resultedTypeName;
            }
            set
            {
                this.resultedTypeName = value;
            }
        }
        public bool Optional { get; set; }


        public IIntermediateEnumType AggregateSetEnum { get; set; }

        public IIntermediateEnumType[] ResultEnumSet { get; set; }

        public IIntermediateClassType ResultClass { get; set; }

        public IIntermediateInterfaceType ResultInterface { get; set; }

        public HashList<HashList<string>> Structures { get { return this.structures; } }

        public IIntermediateFieldMember AssociatedField { get; set; }

        public int StateIndex { get; set; }

        public bool GroupOptional
        {
            get
            {
                return this.groupOptional;
            }
            set
            {
                this.groupOptional = value;
                foreach (var element in this.Values)
                    element.GroupOptional = value;
            }
        }
    }
}
