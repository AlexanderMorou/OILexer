using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class GenericSymbolStreamBuilder :
        IConstructBuilder<Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly>, Tuple<IIntermediateInterfaceType, IIntermediateClassType>>
    {
        private IIntermediateAssembly assembly;
        private ParserCompiler compiler;
        private CommonSymbolBuilder commonSymbolBuilder;

        public Tuple<IIntermediateInterfaceType, IIntermediateClassType> Build(Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly> input)
        {
            this.assembly = input.Item3;
            this.compiler = input.Item1;
            this.commonSymbolBuilder = input.Item2;
            var resultInterface = assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}SymbolStream", this.compiler.Source.Options.AssemblyName);
            var iFaceTParam = resultInterface.TypeParameters.Add(new GenericParameterData("TSymbol", new IType[] { input.Item2.ILanguageSymbol }));
            var resultClass = assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}SymbolStream", this.compiler.Source.Options.AssemblyName);
            var classTParam = resultClass.TypeParameters.Add(new GenericParameterData("TSymbol", new IType[] { input.Item2.ILanguageSymbol }));
            resultClass.ImplementedInterfaces.ImplementInterfaceQuick(resultInterface.MakeGenericClosure(classTParam));
            var readOnlyList = typeof(IReadOnlyList<>).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager).MakeGenericClosure(iFaceTParam);
            resultInterface.ImplementedInterfaces.Add(readOnlyList);
            var initialIndexer = readOnlyList.Indexers[0].Value;
            var readOnlyCollection = (IInterfaceType)readOnlyList.ImplementedInterfaces[0];
            EndOFilePresent = resultInterface.Properties.Add(new TypedName("EndOFilePresent", RuntimeCoreType.Boolean, assembly.IdentityManager), true, false);
            EndOFilePresent.SummaryText = string.Format(@"Returns whether the @s:{0}Symbols.{1}; is present within the @s:{2}{{TSymbol}};", this.compiler.Source.Options.AssemblyName, this.compiler.LexicalSymbolModel.GetEofIdentityField().Name, resultInterface.Name);
            var countProp = readOnlyCollection.Properties[TypeSystemIdentifiers.GetMemberIdentifier("Count")];
            CreateInternalStream(resultClass, classTParam);
            CreateInternalStreamImpl(resultClass, classTParam);
            this.CreateCountImpl(resultClass, countProp);
            CreateEndOfFilePresentImpl(resultClass, initialIndexer, readOnlyCollection, this.CountImpl);
            CreateIndexerImpl(resultClass, initialIndexer);

            var enumeratorTypeOfT = typeof(IEnumerator<>).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager).MakeGenericClosure(classTParam);
            var enumeratorType = typeof(IEnumerator).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager);
            var enumerableType = typeof(IEnumerable).ObtainCILibraryType<IInterfaceType>(assembly.IdentityManager);

            this.CreateGetEnumerator(resultClass, enumeratorTypeOfT, enumeratorType, enumerableType);
            this.ResultInterface = resultInterface;
            this.ResultClass = resultClass;
            this.InternalStream.AccessLevel = AccessLevelModifiers.Private;
            this.EndOFilePresentImpl.AccessLevel = AccessLevelModifiers.Public;
            this.ResultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.ResultClass.AccessLevel = AccessLevelModifiers.Internal;
            return Tuple.Create(this.ResultInterface, this.ResultClass);
        }

        private void CreateGetEnumerator(IIntermediateClassType resultClass, IInterfaceType enumeratorTypeOfT, IInterfaceType enumeratorType, IInterfaceType enumerableType)
        {
            var enumeratorMethod = resultClass.Methods.Add(new TypedName("GetEnumerator", enumeratorTypeOfT));
            enumeratorMethod.SummaryText = string.Format("Returns an @s:{0}{{T}}; that iterates through the @s:{1};.", enumeratorTypeOfT.ElementType.Name, resultClass.Name);
            enumeratorMethod.Return(this.InternalStreamImpl.GetReference().GetMethod(enumeratorMethod.Name).Invoke());
            enumeratorMethod.AccessLevel = AccessLevelModifiers.Public;
            this.GetEnumeratorImpl = enumeratorMethod;
            /* *
             * Necessary evil to help disambiguate between the two
             * GetEnumerator variants, C# only allows exactly
             * TypeName.MethodName when explicitly implementing
             * an interface.
             * *
             * ToDo: Make abstraction provide 'LanguageSpecificQuantifier' callouts on the 
             * declaration site of the method.
             * */
            var enumerationMethodImpl = resultClass.Methods.Add(new TypedName("_GetEnumerator", enumeratorType));
            enumerationMethodImpl.LanguageSpecificQualifier = enumerableType.FullName;
            enumerationMethodImpl.Name = enumeratorMethod.Name;
            enumerationMethodImpl.Implementations.Add(enumerableType);
            enumerationMethodImpl.Return(enumeratorMethod.GetReference().Invoke());
        }

        private void CreateCountImpl(IIntermediateClassType resultClass, IInterfacePropertyMember countProp)
        {
            var countProperty = resultClass.Properties.Add(new TypedName("Count", RuntimeCoreType.Int32, this.assembly.IdentityManager), true, false);
            countProperty.SummaryText = string.Format("Returns the @s:Int32; value which denotes the number of elements within the @s:{0};", resultClass.Name);
            countProperty.GetMethod.Return(countProp.GetReference(this.InternalStreamImpl.GetReference()));
            countProperty.AccessLevel = AccessLevelModifiers.Public;
            this.CountImpl = countProperty;
        }

        private void CreateIndexerImpl(IIntermediateClassType resultClass, IInterfaceIndexerMember initialIndexer)
        {
            var indexerFirstParam = initialIndexer.Parameters[0].Value;
            var indexerImpl = resultClass.Indexers.Add(new TypedName(initialIndexer.Name, initialIndexer.PropertyType), new TypedNameSeries(new TypedName(indexerFirstParam.Name, indexerFirstParam.ParameterType)), true, false);
            indexerImpl.SummaryText = string.Format("Returns the @t:{0}; at the @p:{1}; provided.", indexerImpl.PropertyType.Name, indexerFirstParam.Name);
            var indexerImplFirstParam = indexerImpl.Parameters[indexerFirstParam.UniqueIdentifier];
            indexerImplFirstParam.SummaryText = string.Format("The @s:Int32; value denoting the index of the @s:{0}; to retrieve.", indexerImpl.PropertyType.Name);
            indexerImpl.AccessLevel = AccessLevelModifiers.Public;
            var indexerImplMainBlock = indexerImpl.GetMethod;
            indexerImplMainBlock.Return(this.InternalStreamImpl.GetReference().GetIndexer(indexerImpl.Parameters[indexerFirstParam.UniqueIdentifier].GetReference()));
            this.IndexerImpl = indexerImpl;
        }

        private void CreateInternalStreamImpl(IIntermediateClassType resultClass, IIntermediateGenericTypeParameter classTParam)
        {
            var internalStream = resultClass.Properties.Add(new TypedName("InternalStream", this.InternalStream.FieldType), true, false);
            internalStream.AccessLevel = AccessLevelModifiers.Internal;
            internalStream.GetMethod.Return(this.InternalStream.GetReference());
            this.InternalStreamImpl = internalStream;
        }

        private void CreateInternalStream(IIntermediateClassType resultClass, IIntermediateGenericTypeParameter classTParam)
        {
            var listOfT = typeof(List<>).ObtainCILibraryType<IClassType>(resultClass.IdentityManager).MakeGenericClosure(classTParam);
            var internalStream = resultClass.Fields.Add(new TypedName("internalStream", listOfT), listOfT.GetNewExpression());
            internalStream.SummaryText = "Data member for @s:InternalStream;.";
            this.InternalStream = internalStream;
        }

        private void CreateEndOfFilePresentImpl(IIntermediateClassType resultClass, IInterfaceIndexerMember initialIndexer, IInterfaceType readOnlyCollection, IClassPropertyMember countProp)
        {
            EndOFilePresentImpl = resultClass.Properties.Add(new TypedName("EndOFilePresent", RuntimeCoreType.Boolean, assembly.IdentityManager), true, false);
            EndOFilePresentImpl.SummaryText = string.Format(@"Returns whether the @s:{0}Symbols.{1}; is present within the @s:{2};", this.compiler.Source.Options.AssemblyName, this.compiler.LexicalSymbolModel.GetEofIdentityField().Name, resultClass.Name);
            var mainBlock = EndOFilePresentImpl.GetMethod;
            var countPropRef = countProp.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This));
            var countCheck = mainBlock.If(countPropRef.GreaterThan(0));
            countCheck.Return(this.commonSymbolBuilder.Identity.GetReference(initialIndexer.GetIndexerSignatureReference<IInterfaceIndexerMember, IInterfaceType>(new SpecialReferenceExpression(SpecialReferenceKind.This), new IExpression[] { countPropRef.Subtract(1) })).EqualTo(compiler.LexicalSymbolModel.GetEofIdentityField().GetReference()));
            mainBlock.Return(IntermediateGateway.FalseValue);
        }

        public IIntermediateInterfacePropertyMember EndOFilePresent { get; private set; }
        public IIntermediateClassPropertyMember EndOFilePresentImpl { get; private set; }
        public IIntermediateInterfaceType ResultInterface { get; private set; }
        public IIntermediateClassType ResultClass { get; private set; }

        /// <summary>
        /// Returns the <see cref="IIntermediateClassFieldMember"/> 
        /// which stores the local copy of the elements exposed through
        /// the <see cref="IReadOnlyList{T}"/> interface.
        /// </summary>
        public IIntermediateClassFieldMember InternalStream { get; set; }

        public IIntermediateClassPropertyMember InternalStreamImpl { get; set; }

        public IIntermediateClassPropertyMember CountImpl { get; set; }

        public IIntermediateClassMethodMember GetEnumeratorImpl { get; set; }

        public IIntermediateClassIndexerMember IndexerImpl { get; set; }
    }
}
