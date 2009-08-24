using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class TokenFinalDataSet : 
        Dictionary<ITokenEntry, TokenFinalData>
    {
        public IEnumeratorType[] TokenSelectorSets { get; internal set; }
        //public IStructType TokenValidData { get; private set; }
        public IInterfaceType BaseInterface { get; internal set; }
        public IDictionary<RegularLanguageBitArray, List<ITokenEntry>> Relationships { get; internal set; }
        /// <summary>
        /// The constructor which contains a series of parameters for every possible
        /// token in the series that contains subset information, as well as a parameter
        /// for the capture parameters to be expressed.
        /// </summary>
        public IConstructorMember FullCaseConstructor { get; internal set; }
        internal TokenFinalDataSet()//IStructType validData)
        {
            //this.TokenValidData = validData;
        }
    }

    internal enum TokenFinalType
    {
        RecognizerCapture,
        Enumeration,
    }
    internal class TokenFinalData
    {
        public RegularLanguageState FinalState { get; private set; }
        public IClassType StateMachine { get; private set; }
        public TokenFinalType Type { get; private set; }
        public ITokenEntry Entry { get; private set; }
        public IInterfaceType TokenInterface { get; internal set; }
        public IEnumerable<ITokenItem> Elements { get; private set; }
        //public IFieldMember ValidDataField { get; internal set; }
        public IFieldMember ValidCaseField { get; internal set; }
        public IClassType TokenBaseType { get; internal set; }
        public IMethodMember MachineInjectMethod { get; internal set; }
        public IClassType EntryClass { get; internal set; }
        public TokenFinalData(ITokenEntry entry, ITokenItem[] elements, TokenFinalType type, IClassType stateMachine, RegularLanguageState finalState, IClassType entryClass)
        {
            this.Type = type;
            this.Entry = entry;
            this.Elements = elements;
            this.StateMachine = stateMachine;
            this.FinalState = finalState;
            this.EntryClass = entryClass;
        }
    }

    internal class TokenEofFinalData :
        TokenFinalData
    {
        public TokenEofFinalData(ITokenEofEntry entry, IClassType entryClass)
            : base(entry, new ITokenItem[0], TokenFinalType.RecognizerCapture, null, null, entryClass)
        {
        }
        public IMethodMember DataSetAddMethod { get; internal set; }
    }

    internal class TokenCaptureFinalData :
        TokenFinalData
    {
        public TokenCaptureFinalData(ITokenEntry entry, IClassType stateMachine, RegularLanguageState finalState, IClassType entryClass)
            : base(entry, null, TokenFinalType.RecognizerCapture, stateMachine, finalState, entryClass)
        {
        }
        public IMethodMember DataSetAddMethod { get; internal set; }
    }
    internal class TokenEnumFinalData :
        TokenFinalData
    {
        /// <summary>
        /// Returns the <see cref="IEnumeratorType"/> which
        /// defines the cases associated to the token.
        /// </summary>
        public IEnumeratorType CaseEnumeration { get; private set; }
        public IFieldMember ValidDataFieldMember { get; internal set; }
        public Dictionary<IEnumeratorType, IMethodMember> DataSetAddMethods { get; internal set; }
        public IDictionary<IEnumeratorType, IFieldMember> ScannerSetEnumFields { get; private set; }
        /// <summary>
        /// Returns the relational dictionary between the 
        /// elements of the token entry and the 
        /// </summary>
        public ProjectConstructor.EnumStateMachineDataSet FinalItemLookup { get; private set; }
        public IDictionary<ITokenItem, ITokenItemData> TokenRelationshipInfo { get; private set; }
        public IEnumeratorType FinalStateEnum { get; private set; }
        public IFieldMember ExitStateField { get; private set; }
        public TokenEnumFinalData(ITokenEntry entry, ITokenItem[] elements, 
            IEnumeratorType caseEnumeration, IClassType stateMachine, RegularLanguageState finalState,
            ProjectConstructor.EnumStateMachineDataSet finalItemLookup,
            IDictionary<IEnumeratorType, IFieldMember> scannerSetEnumFields, IClassType entryClass,
            IDictionary<ITokenItem, ITokenItemData> tokenRelationshipInfo,
            IEnumeratorType finalStateEnum, IFieldMember exitStateField)
            : base(entry, elements, TokenFinalType.Enumeration, stateMachine, finalState, entryClass)
        {
            this.FinalStateEnum = finalStateEnum;
            this.TokenRelationshipInfo = tokenRelationshipInfo;
            this.ScannerSetEnumFields = scannerSetEnumFields;
            this.FinalItemLookup = finalItemLookup;
            this.CaseEnumeration = caseEnumeration;
            this.ExitStateField = exitStateField;
        }
    }

    internal class TokenEnumSetFinalData :
        TokenEnumFinalData
    {
        public IClassType[] TokenBaseTypes { get; internal set; }
        /// <summary>
        /// Returns the relational dictionary between the 
        /// <see cref="TokenEnumFinalData.CaseEnumeration"/>
        /// fields and the sub-set enumerators that resulted 
        /// due to a total set greater than 32 elements.
        /// </summary>
        public Dictionary<IFieldMember, IEnumeratorType> SubsetEnumerations { get; private set; }
        public IFieldMember[] ValidDataFieldMembers { get; internal set; }
        public TokenEnumSetFinalData(ITokenEntry entry, ITokenItem[] elements, 
            IEnumeratorType caseEnumeration, IClassType stateMachine, RegularLanguageState finalState,
            ProjectConstructor.EnumStateMachineDataSet finalItemLookup,
            Dictionary<IFieldMember, IEnumeratorType> subsetEnumerations,
            IDictionary<IEnumeratorType, IFieldMember> scannerSetEnumFields, IClassType entryClass,
            IDictionary<ITokenItem, ITokenItemData> tokenRelationshipInfo,
            IEnumeratorType finalStateEnum, IFieldMember exitStateField)
            : base(entry, elements, caseEnumeration, stateMachine, finalState, 
                   finalItemLookup, scannerSetEnumFields, entryClass, tokenRelationshipInfo,
                   finalStateEnum, exitStateField)
        {
            SubsetEnumerations = new Dictionary<IFieldMember, IEnumeratorType>(subsetEnumerations);
        }
    }

}
