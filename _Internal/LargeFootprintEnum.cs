using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Types;
using System.IO;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Statements;
using System.Linq;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal partial class 
#if SERIALIZE
        LargeFootprintEnum : 
            Table<LargeFootprintEnumGroup>
    {
        public LargeFootprintEnum()
            : this(true)
        {
        }
        public LargeFootprintEnum(bool condense)
        {
            this.Condense = condense;
        }
        public string FileName { get; set; }
#else
        LargeFootprintEnum<T> :
            ControlledStateCollection<LargeFootprintEnumGroup<T>>
    {
        private bool condense;
        private string name;

        public LargeFootprintEnum(string name, bool condense)
        {
            this.name = name;
            this.condense = condense;
        }

        public string Name 
        { 
            get
            {
                return this.name;
            }
        }
#endif
        public bool Condense
        {
#if SERIALIZE
            get;
            set;
#else
            get
            {
                return this.condense;
            }
#endif
        }

        public BuildResults Build<V>(V parent)
            where V :
                ITypeParent,
                ISegmentableDeclarationTarget<V>
        {
            int count = this.CountAll();
#if !SERIALIZE //Easier to read
            Dictionary<LargeFootprintEnumGroupItem<T>, FieldParentMapping> memberMapping = new Dictionary<LargeFootprintEnumGroupItem<T>, FieldParentMapping>();
            Dictionary<LargeFootprintEnumGroupItem<T>, PropertyParentMapping> targetMapping = new Dictionary<LargeFootprintEnumGroupItem<T>, PropertyParentMapping>();
#else
            Dictionary<LargeFootprintEnumGroupItem   , FieldParentMapping> memberMapping = new Dictionary<LargeFootprintEnumGroupItem   , FieldParentMapping>();
            Dictionary<LargeFootprintEnumGroupItem   , PropertyParentMapping> targetMapping = new Dictionary<LargeFootprintEnumGroupItem   , PropertyParentMapping>();
#endif
#if SERIALIZE
            var kResult = parent.Structures.AddNew(Path.GetFileNameWithoutExtension(this.FileName));
            Dictionary<LargeFootprintEnumGroup, IEnumeratorType> groupMapping = new Dictionary<LargeFootprintEnumGroup, IEnumeratorType>();
            Dictionary<LargeFootprintEnumGroup, IClassType> groupMapping2 = new Dictionary<LargeFootprintEnumGroup, IClassType>();

#else
            var kResult = parent.Partials.AddNew().Structures.AddNew(this.Name);
            Dictionary<LargeFootprintEnumGroup<T>, IEnumeratorType> groupMapping = new Dictionary<LargeFootprintEnumGroup<T>, IEnumeratorType>();
            Dictionary<LargeFootprintEnumGroup<T>, IClassType> groupMapping2 = new Dictionary<LargeFootprintEnumGroup<T>, IClassType>();
            Dictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> fieldMapping = new Dictionary<LargeFootprintEnumGroup<T>, LargeFootprintEnum<T>.FieldParentMapping>();
            Dictionary<LargeFootprintEnumGroup<T>, PropertyParentMapping> propertyMapping = new Dictionary<LargeFootprintEnumGroup<T>, LargeFootprintEnum<T>.PropertyParentMapping>();
            Dictionary<LargeFootprintEnumGroup<T>, FieldParentMapping> noneMapping = new Dictionary<LargeFootprintEnumGroup<T>, LargeFootprintEnum<T>.FieldParentMapping>();
#endif
            IClassType kElementsRoot = parent.Partials.AddNew().Classes.AddNew(kResult.Name + "s");
            kElementsRoot.IsStatic = true;
            kElementsRoot.AccessLevel = DeclarationAccessLevel.Internal;
            List<IConstructorMember> lastConstructors = new List<IConstructorMember>();
            Dictionary<IEnumeratorType, IFieldMember> NoneMembers = new Dictionary<IEnumeratorType, IFieldMember>();
            Dictionary<IEnumeratorType, IFieldMember> AllMembers = new Dictionary<IEnumeratorType, IFieldMember>();
            foreach (var group in this)
            {
                IClassType currentContainer = kElementsRoot.Partials.AddNew().Classes.AddNew(group.Name);
                currentContainer.IsStatic = true;
                currentContainer.AccessLevel = DeclarationAccessLevel.Internal;
                groupMapping2.Add(group, currentContainer);
                ulong value = 1;
                int currentCount = group.Count;
                IEnumeratorType currentEnum = null;
                if (group.ValuesType != null)
                    currentEnum = group.ValuesType;
                else
                {
                    currentEnum = kResult.Partials.AddNew().Enumerators.AddNew(group.Name + "Sector");
                    currentEnum.Attributes.AddNew(typeof(FlagsAttribute));
                }
                currentEnum.AccessLevel = DeclarationAccessLevel.Public;
                var paramInfo = new TypedName(group.Name[0].ToString().ToLower() + group.Name.Substring(1), currentEnum.GetTypeReference());
                bool needsCtor = this.Count > 1;

                IConstructorMember currentCtor = needsCtor ? kResult.Constructors.AddNew() : null;
                IConstructorParameterMember currentCtorParam = null;
                if (needsCtor)
                {
                    currentCtorParam = currentCtor.Parameters.AddNew(paramInfo);
                    currentCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.This;
                    foreach (var et in groupMapping.Values)
                        currentCtor.CascadeMembers.Add(NoneMembers[et].GetReference());
                    currentCtor.CascadeMembers.Add(currentCtorParam.GetReference());
                    currentCtor.AccessLevel = DeclarationAccessLevel.Public;
                }
                if (group.ValuesType == null)
                {
                    NoneMembers.Add(currentEnum, currentEnum.Fields.AddNew("\uC3C3", 0));
                }
                else if (currentEnum.Fields.Values.Any(noneFinder))
                    NoneMembers.Add(currentEnum, currentEnum.Fields.Values.First(noneFinder));
                else
                    NoneMembers.Add(currentEnum, currentEnum.Fields.AddNew("\uC3C3", 0));
                noneMapping.Add(group, new FieldParentMapping(currentEnum, NoneMembers[currentEnum]));
                if (needsCtor)
                {
                    foreach (var lc in lastConstructors)
                        lc.CascadeMembers.Add(NoneMembers[currentEnum].GetReference());
                    lastConstructors.Add(currentCtor);
                }
                if (group.ValuesType == null)
                {
                    if (currentCount == 64)
                        currentEnum.BaseType = EnumeratorBaseType.ULong;
                    else if (currentCount > 32)
                        currentEnum.BaseType = EnumeratorBaseType.SLong;
                    else if (currentCount == 32)
                        currentEnum.BaseType = EnumeratorBaseType.UInt;
                    else
                        currentEnum.BaseType = EnumeratorBaseType.SInt;
                }
                foreach (var item in group)
                {
                    IFieldMember currentItem = null;
                    if (item.ValueMember == null)
                    {
                        if (currentCount == 64)
                            currentItem = currentEnum.Fields.AddNew(item.Name, value);
                        else if (currentCount > 32)
                            currentItem = currentEnum.Fields.AddNew(item.Name, (long)value);
                        else if (currentCount == 32)
                            currentItem = currentEnum.Fields.AddNew(item.Name, (uint)value);
                        else
                            currentItem = currentEnum.Fields.AddNew(item.Name, (int)value);
                        currentItem.Remarks = item.Remarks;
                        currentItem.Summary = item.Summary;
                    }
                    else
                        currentItem = item.ValueMember;
                    memberMapping.Add(item, new FieldParentMapping(currentEnum, currentItem));
                    value *= 2;
                }
                if (group.ValuesType == null)
                {
                    IFieldMember allMember = null;
                    if (currentCount == 64)
                        allMember = currentEnum.Fields.AddNew(currentEnum.Fields.GetUniqueName("All"), (value - 1));
                    else if (currentCount > 32)
                        allMember = currentEnum.Fields.AddNew(currentEnum.Fields.GetUniqueName("All"), (long)(value - 1));
                    else if (currentCount == 32)
                        allMember = currentEnum.Fields.AddNew(currentEnum.Fields.GetUniqueName("All"), (uint)(value - 1));
                    else
                        allMember = currentEnum.Fields.AddNew(currentEnum.Fields.GetUniqueName("All"), (int)(value - 1));
                    AllMembers.Add(currentEnum, allMember);
                    NoneMembers[currentEnum].Name = currentEnum.Fields.GetUniqueName("None");
                }
                groupMapping.Add(group, currentEnum);
                var currentCoercion = kResult.Coercions.AddNewImplicitTo(currentEnum.GetTypeReference());
                currentCoercion.Return(new CreateNewObjectExpression(kResult.GetTypeReference(), currentCoercion.Source));
                currentCoercion.AccessLevel = DeclarationAccessLevel.Public;
            }
            IConstructorMember mainCtor = kResult.Constructors.AddNew();
            mainCtor.AccessLevel = DeclarationAccessLevel.Public;
            mainCtor.CascadeExpressionsTarget = ConstructorCascadeTarget.This;
            IBinaryOperatorOverloadMember iboomOr = kResult.Coercions.AddNew(OverloadableBinaryOperators.LogicalOr);
            IMethodMember exclusiveOrMethod = kResult.Methods.AddNew(new TypedName("ExclusiveOr", kResult));
            exclusiveOrMethod.AccessLevel = DeclarationAccessLevel.Public;
            exclusiveOrMethod.IsStatic = true;
            var xOrleft = exclusiveOrMethod.Parameters.AddNew(new TypedName("left", kResult));
            var xOrright = exclusiveOrMethod.Parameters.AddNew(new TypedName("right", kResult));

            ICreateNewObjectExpression iboomOrCnoe = new CreateNewObjectExpression(kResult.GetTypeReference());
            IBinaryOperatorOverloadMember iboomAnd = kResult.Coercions.AddNew(OverloadableBinaryOperators.LogicalAnd);
            ICreateNewObjectExpression iboomAndCnoe = new CreateNewObjectExpression(kResult.GetTypeReference());
            ICreateNewObjectExpression iboomExOrCnoe = new CreateNewObjectExpression(kResult.GetTypeReference());
            iboomOr.AccessLevel = DeclarationAccessLevel.Public;
            iboomAnd.AccessLevel = DeclarationAccessLevel.Public;
            exclusiveOrMethod.AccessLevel = DeclarationAccessLevel.Public;
            exclusiveOrMethod.Statements.Add(new CommentStatement("Because some goofball relied on CodeDOM early on, no BitwiseExOr operator is available."));

            var toStringMethod = kResult.Methods.AddNew(new TypedName("ToString", typeof(string)));
            //public override string ToString()
            toStringMethod.Overrides = true;
            toStringMethod.IsFinal = false;
            toStringMethod.AccessLevel = DeclarationAccessLevel.Public;
            //{
            //    StringBuilder result = new StringBuilder();
            var toStringResult = toStringMethod.Locals.AddNew(new TypedName("result", typeof(StringBuilder)), new CreateNewObjectExpression(typeof(StringBuilder).GetTypeReference()));

            bool first = true;
            var emptyProperty = kResult.Properties.AddNew(new TypedName("Empty", typeof(bool)), true, false);
            emptyProperty.AccessLevel = DeclarationAccessLevel.Public;
            IExpression emptyReturnExpression = null;
            foreach (var groupPair in groupMapping)
            {
                var name = groupPair.Key.Name[0].ToString().ToLower() + groupPair.Key.Name.Substring(1);
                if (name == groupPair.Key.Name)
                    name = "_" + name;
                var type = groupPair.Value.GetTypeReference();
                var currentParam = mainCtor.Parameters.AddNew(new TypedName(name, type));
                var currentField = kResult.Fields.AddNew(new TypedName(name, type));
                var currentProperty = kResult.Properties.AddNew(new TypedName(groupPair.Key.Name, type), true, false);
                currentProperty.AccessLevel = DeclarationAccessLevel.Public;
                currentField.Summary = string.Format("Data member for <see cref=\"{0}\"/>", currentProperty.Name);
                currentProperty.GetPart.Return(currentField.GetReference());
                currentProperty.Summary = groupPair.Key.Summary;
                currentProperty.Remarks = groupPair.Key.Remarks;
                fieldMapping.Add(groupPair.Key, new FieldParentMapping(kResult, currentField));
                propertyMapping.Add(groupPair.Key, new PropertyParentMapping(kResult, currentProperty));
                var xOrMaskLocal = exclusiveOrMethod.Locals.AddNew(new TypedName(string.Format(name, "xOrMask"), type), new UnaryOperationExpression(UnaryOperations.Compliment, new BinaryOperationExpression(xOrleft.GetReference().GetField(name), CodeBinaryOperatorType.BitwiseAnd, xOrright.GetReference().GetField(name))));
                iboomOrCnoe.Arguments.Add(new BinaryOperationExpression(iboomOr.LeftParameter.GetField(name), CodeBinaryOperatorType.BitwiseOr, iboomOr.RightParameter.GetField(name)));
                iboomAndCnoe.Arguments.Add(new BinaryOperationExpression(iboomAnd.LeftParameter.GetField(name), CodeBinaryOperatorType.BitwiseAnd, iboomAnd.RightParameter.GetField(name)));
                iboomExOrCnoe.Arguments.Add(new BinaryOperationExpression(xOrMaskLocal.GetReference(), CodeBinaryOperatorType.BitwiseAnd, new BinaryOperationExpression(xOrleft.GetReference().GetField(name), CodeBinaryOperatorType.BitwiseOr, xOrright.GetReference().GetField(name))));
                mainCtor.Statements.Assign(currentField.GetReference(), currentParam.GetReference());
                var currentContainer = groupMapping2[groupPair.Key];
                currentContainer.AccessLevel = DeclarationAccessLevel.Internal;
                foreach (var item in groupPair.Key)
                {
                    string lName = currentContainer.Fields.GetUniqueName("_" + item.Name);
                    string pName = currentContainer.Properties.GetUniqueName(item.Name);
                    var krRef = kResult.GetTypeReference();
                    var kRef = currentContainer.GetTypeReference().GetTypeExpression();
                    krRef.Nullable=true;
                    var currentItemField = currentContainer.Fields.AddNew(new TypedName(lName, krRef));
                    var currentItemProperty = currentContainer.Properties.AddNew(new TypedName(pName, kResult.GetTypeReference()), true, false);
                    currentItemField.AccessLevel = DeclarationAccessLevel.Private;
                    currentItemField.IsStatic = true;
                    currentItemProperty.IsStatic = true;
                    currentItemProperty.AccessLevel = DeclarationAccessLevel.Public;
                    var condition = currentItemProperty.GetPart.IfThen(new BinaryOperationExpression(currentItemField.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
                    condition.Assign(kRef.GetField(currentItemField.Name), memberMapping[item].Member.GetReference());
                    currentItemProperty.GetPart.Return(kRef.GetField(currentItemField.Name).GetProperty("Value"));
                    targetMapping.Add(item, new PropertyParentMapping(currentContainer, currentItemProperty));
                }

                var currentNone = groupPair.Value.Fields.Values.First(noneFinder);
                var currentEmptyCheck = new BinaryOperationExpression(currentField.GetReference(), CodeBinaryOperatorType.IdentityEquality, currentNone.GetReference());
                if (emptyReturnExpression == null)
                    emptyReturnExpression = currentEmptyCheck;
                else
                    emptyReturnExpression = new BinaryOperationExpression(emptyReturnExpression, CodeBinaryOperatorType.BooleanAnd, currentEmptyCheck);
                var toStringCheck = toStringMethod.IfThen(new BinaryOperationExpression(currentField.GetReference(), CodeBinaryOperatorType.IdentityInequality, currentNone.GetReference()));
                if (first)
                    first = false;
                else
                {
                    var lengthCheck = toStringCheck.IfThen(new BinaryOperationExpression(toStringResult.GetReference().GetProperty("Length"), CodeBinaryOperatorType.GreaterThan, PrimitiveExpression.NumberZero));
                    lengthCheck.CallMethod(toStringResult.GetReference().GetMethod("Append").Invoke(new PrimitiveExpression(", ")));
                }
                toStringCheck.CallMethod(toStringResult.GetReference().GetMethod("AppendFormat").Invoke(new PrimitiveExpression(string.Format("{0} ({{0}})", groupPair.Key.Name)), currentField.GetReference()));
            }
            emptyProperty.GetPart.Return(emptyReturnExpression);
            iboomOr.Return(iboomOrCnoe);
            iboomAnd.Return(iboomAndCnoe);
            exclusiveOrMethod.Return(iboomExOrCnoe);
            //(toString)
            //    return result.ToString();
            toStringMethod.Return(toStringResult.GetReference().GetMethod("ToString").Invoke());
            //}
            return new BuildResults(memberMapping, targetMapping, kResult, kElementsRoot, fieldMapping, propertyMapping, noneMapping);
        }

        private static bool noneFinder(IFieldMember element)
        {
            return element.Name == "None";
        }

        private int CountAll()
        {
            int r = 0;
            foreach (var group in this)
                r += group.Count;
            return r;
        }
        
        internal LargeFootprintEnumGroup<T> Add(string name, string summary, string remarks, IEnumeratorType valuesType)
        {
            var result = new LargeFootprintEnumGroup<T>(name, summary, remarks, valuesType);
            base.baseCollection.Add(result);
            return result;
        }

        internal LargeFootprintEnumGroup<T> Add(string name, string summary, string remarks)
        {
            var result = new LargeFootprintEnumGroup<T>(name, summary, remarks, null);
            base.baseCollection.Add(result);
            return result;
        }
    }
}
