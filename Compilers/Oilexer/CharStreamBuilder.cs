using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract.Members;
namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class CharStreamBuilder
    {
        private ParserCompiler _compiler;
        private IIntermediateAssembly _assembly;
        private IIntermediateCliManager _identityManager;

        public void Build(ParserCompiler compiler, IIntermediateAssembly assembly)
        {
            this._compiler = compiler;
            this._assembly = assembly;
            this._identityManager = ((IIntermediateCliManager)(assembly.IdentityManager));
            this.BuildInterface();
            this.BuildClass();

        }

        private void CreateStreamReaderImpl()
        {
            var streamReaderImpl = CharStream.Properties.Add(new TypedName("StreamReader", this.StreamReader.PropertyType), true, false);
            streamReaderImpl.AccessLevel = AccessLevelModifiers.Public;
            var _streamReaderImpl = CharStream.Fields.Add(new TypedName("_streamReader", this.StreamReader.PropertyType));
            _streamReaderImpl.AccessLevel = AccessLevelModifiers.Private;
            this._StreamReaderImpl = _streamReaderImpl;

            this.StreamReaderImpl = streamReaderImpl;
            var streamReaderImplCheck = streamReaderImpl.GetMethod.If(_streamReaderImpl.EqualTo(IntermediateGateway.NullValue).LogicalAnd(_BaseStreamImpl.InequalTo(IntermediateGateway.NullValue)));
            streamReaderImplCheck.Assign(_streamReaderImpl, _streamReaderImpl.FieldType.GetNewExpression(_BaseStreamImpl.GetReference()));
            streamReaderImpl.GetMethod.Return(_streamReaderImpl);
        }

        private void CreateStreamReader()
        {
            var streamReader = ICharStream.Properties.Add(new TypedName("StreamReader", this._identityManager.ObtainTypeReference(typeof(StreamReader))), true, false);
            this.StreamReader = streamReader;
        }

        private void CreateBaseStream()
        {
            this.BaseStream = ICharStream.Properties.Add(new TypedName("BaseStream", this._identityManager.ObtainTypeReference(typeof(Stream))), true, false);
        }

        private void CreateBaseStreamImpl()
        {
            var _baseStreamImpl = CharStream.Fields.Add(new TypedName("_baseStream", BaseStream.PropertyType));
            var baseStreamImpl = CharStream.Properties.Add(new TypedName("BaseStream", BaseStream.PropertyType), true, false);
            baseStreamImpl.AccessLevel = AccessLevelModifiers.Public;
            baseStreamImpl.GetMethod.Return(_baseStreamImpl.GetReference());
            this.BaseStreamImpl = baseStreamImpl;
            this._BaseStreamImpl = _baseStreamImpl;
        }

        private void CreateLength()
        {
            this.Length = ICharStream.Properties.Add(new TypedName("Length", RuntimeCoreType.Int64, this._identityManager), true, false);
        }

        private void Create_LengthImpl()
        {
            this._LengthImpl = this.CharStream.Fields.Add(new TypedName("_length", RuntimeCoreType.Int64, this._identityManager));
            this._LengthImpl.AccessLevel = AccessLevelModifiers.Private;
            this._LengthImpl.SummaryText = string.Format("Denotes the length of the @s:BaseStream; retrieved on creation of the @s:{0};", this.CharStream.Name);
        }

        private void CreateLengthImpl()
        {
            var lengthImpl = CharStream.Properties.Add(new TypedName("Length", this.Length.PropertyType), true, false);
            lengthImpl.GetMethod.Return(this._LengthImpl);
            lengthImpl.AccessLevel = AccessLevelModifiers.Public;
            this.LengthImpl = lengthImpl;
        }

        private void BuildClass()
        {
            this.CharStream = this._assembly.DefaultNamespace.Parts.Add().Classes.Add("{0}CharStream", this._compiler.Source.Options.AssemblyName);
            this.CharStream.AccessLevel = AccessLevelModifiers.Public;

            this.CreateEofEncountered();
            this.CreateBaseStreamImpl();
            this.Create_LengthImpl();
            this.CreateLengthImpl();
            this.CreateCharStreamCtor();
            this.CreateStreamReaderImpl();
            this.CreateCheckBuffer();
            this.CreateLookAheadMethodImpl();
            this.CreateCharIndexerImpl();
            this.CreateStringCacheImpl();
            this.CreateStringIndexerImpl();
            this.CreateDisposeMethodImpl();

            this.CharStream.ImplementedInterfaces.ImplementInterfaceQuick(this.ICharStream);
        }

        private void CreateDisposeMethodImpl()
        {
            var disposeMethod = this.CharStream.Methods.Add(new TypedName("Dispose", RuntimeCoreType.VoidType, this._identityManager));
            disposeMethod.If(this._BaseStreamImpl.InequalTo(IntermediateGateway.NullValue))
                .Using(this._BaseStreamImpl.GetReference())
                    .Call(this._BaseStreamImpl.GetReference().GetMethod("Close")).FollowedBy()
                    .Assign(this._BaseStreamImpl, IntermediateGateway.NullValue);
            var streamReaderCheck = disposeMethod.If(this._StreamReaderImpl.InequalTo(IntermediateGateway.NullValue))
                .Using(this._StreamReaderImpl.GetReference())
                    .Call(this._StreamReaderImpl.GetReference().Call("Close")).FollowedBy()
                    .Assign(this._StreamReaderImpl, IntermediateGateway.NullValue);
            this.DisposeMethodImpl = disposeMethod;
            this.DisposeMethodImpl.AccessLevel = AccessLevelModifiers.Public;
        }

        private void CreateStringCacheImpl()
        {
            var intRef = this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32);
            var stringRef = this._identityManager.ObtainTypeReference(RuntimeCoreType.String);
            var stringCache = this.CharStream.Fields.Add(new TypedName("stringCache", this._compiler.IMultikeyedDictionary.MakeGenericClosure(intRef, intRef, stringRef)));
            stringCache.AccessLevel = AccessLevelModifiers.Private;
            stringCache.InitializationExpression = this._compiler.MultikeyedDictionary.MakeGenericClosure(intRef, intRef, stringRef).GetNewExpression();
            this.StringCacheImpl = stringCache;
        }

        private void CreateCharIndexerImpl()
        {
            var charIndexer = this.CharStream.Indexers.Add(this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32), new TypedNameSeries(new TypedName("offset", RuntimeCoreType.Int32, this._identityManager)), true, false);
            charIndexer.GetMethod.Return(this.LookAheadImpl.GetReference().Invoke(charIndexer.Parameters["offset"].GetReference()));
            charIndexer.AccessLevel = AccessLevelModifiers.Public;
            this.CharIndexerImpl = charIndexer;
        }

        private void CreateStringIndexerImpl()
        {
            //Create a string from the start/end provided, use a bit of cache to ensure we're not chugging memory, the memory we use here speeds things up by a factor of about 18.
            var stringIndexer = this.CharStream.Indexers.Add(_identityManager.ObtainTypeReference(RuntimeCoreType.String), new TypedNameSeries(new TypedName("start", RuntimeCoreType.Int32, _identityManager), new TypedName("end", RuntimeCoreType.Int32, _identityManager)), true, false);
            var startParam = stringIndexer.Parameters["start"];
            var endParam = stringIndexer.Parameters["end"];
            stringIndexer.SummaryText = string.Format("Obtains the @s:String; value inclusively between the @p:{0}; @p:{1}; provided.", startParam.Name, endParam.Name);
            startParam.SummaryText = string.Format("The @s:Int32; value denoting the start character offset within the @s:{0}; to retrieve the string of.", CharStream.Name);
            endParam.SummaryText = string.Format("The @s:Int32; value denoting the end character offset within the @s:{0}; to retrieve the string of.", CharStream.Name);
            var stringTypeRef = this._identityManager.ObtainTypeReference(RuntimeCoreType.String);
            stringIndexer.GetMethod.Comment(@"Create a string from the start/end provided, use a bit of cache to ensure we're not chugging memory, the memory we use here speeds things up by a factor of about 18.");
            var resultLocal = stringIndexer.GetMethod.Locals.Add(new TypedName("result", RuntimeCoreType.String, this._identityManager));
            // if (!cache.TryGetValue(start, end, out result)) { ...
            var cacheCheck = stringIndexer.GetMethod.If(this.StringCacheImpl.GetReference().GetMethod("TryGetValue").Invoke(startParam.GetReference(), endParam.GetReference(), resultLocal.GetReference().Direct(ParameterCoercionDirection.Out)).Not());
            cacheCheck.Comment("Index patch-up.  If we receive a value beyond the end of the stream, fix-up the end value, we make note of the original index because it might result in two cache entries, but only one string.");
            var endValue = cacheCheck.Locals.Add(new TypedName("endValue", RuntimeCoreType.Int32, this._identityManager), CharIndexerImpl.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), endParam.GetReference()));
            var originalEnd = cacheCheck.Locals.Add(new TypedName("originalEnd", RuntimeCoreType.Int32, this._identityManager), endParam.GetReference());
            var endCheck = cacheCheck.If(endValue.EqualTo(-1));
            endCheck.Assign(endParam.GetReference(), ActualBufferLength.GetReference().Cast(endParam.ParameterType));
            endCheck.Comment("If we went past the end, make one final check with the new end to make sure the data is read, if you seek past the end it skips the read because it logically knows there's nothing there.");
            // if (this.buffer[end] == char.MinValue)
            var readSkipCheck = endCheck.If(endValue.EqualTo(this._identityManager.ObtainTypeReference(RuntimeCoreType.Char).GetTypeExpression().GetField("MinValue")));
            // endValue = this[end];
            readSkipCheck.Assign(endValue.GetReference(), CharIndexerImpl.GetReference(new SpecialReferenceExpression(SpecialReferenceKind.This), endParam.GetReference()));
            // if (end <= start)
            var rangeCheck = cacheCheck.If(endParam.LessThanOrEqualTo(startParam));
            //     return string.Empty;
            rangeCheck.Return(stringTypeRef.GetTypeExpression().GetField("Empty"));

            // else if (originalEnd == end)
            rangeCheck.CreateNext(originalEnd.EqualTo(endParam));
            var rangeNext = (IConditionBlockStatement)rangeCheck.Next;
            //cache.Try(start, end, result = new string(this.buffer, start, end - start));
            rangeNext.Call(StringCacheImpl.GetReference().GetMethod("Add").Invoke(startParam.GetReference(), endParam.GetReference(), resultLocal.GetReference().Assign(stringTypeRef.GetNewExpression(this.BufferArrayField.GetReference(), startParam.GetReference(), endParam.Subtract(startParam)))));
            
            rangeNext.CreateNext(this.StringCacheImpl.GetReference().GetMethod("TryGetValue").Invoke(startParam.GetReference(), endParam.GetReference(), resultLocal.GetReference().Direct(ParameterCoercionDirection.Out)).Not());
            rangeNext = (IConditionBlockStatement)rangeNext.Next;
            //cache.Add(start, end, result = new string(this.buffer, start, end - start));
            rangeNext.Call(StringCacheImpl.GetReference().GetMethod("Add").Invoke(startParam.GetReference(), endParam.GetReference(), resultLocal.GetReference().Assign(stringTypeRef.GetNewExpression(this.BufferArrayField.GetReference(), startParam.GetReference(), endParam.Subtract(startParam)))));
            //cache.Add(start, originalEnd, result);
            rangeNext.Call(StringCacheImpl.GetReference().GetMethod("Add").Invoke(startParam.GetReference(), originalEnd.GetReference(), resultLocal.GetReference()));
            rangeNext.CreateNext();
            var finalNext = rangeNext.Next;
            //cache.Add(start, originalEnd, result);
            finalNext.Call(StringCacheImpl.GetReference().GetMethod("Add").Invoke(startParam.GetReference(), originalEnd.GetReference(), resultLocal.GetReference()));
            //return result;
            stringIndexer.GetMethod.Return(resultLocal.GetReference());
            stringIndexer.AccessLevel = AccessLevelModifiers.Public;
            this.StringIndexerImpl = stringIndexer;
        }

        private void CreateEofEncountered()
        {
            this.EofEncountered = this.CharStream.Fields.Add(new TypedName("eofEncountered", RuntimeCoreType.Boolean, this._identityManager));
            this.EofEncountered.AccessLevel = AccessLevelModifiers.Private;
            this.EofEncountered.SummaryText = "Tracks whether the stream's end has been reached.";
            this.EofEncountered.RemarksText = "Used when encoding options, at the file's start, for the file slightly adjust the actual length.";
        }

        private void BuildInterface()
        {
            this.ICharStream = _assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}CharStream", this._compiler.Source.Options.AssemblyName);
            ICharStream.AccessLevel = AccessLevelModifiers.Public;
            this.CreateBaseStream();
            this.CreateStreamReader();
            this.CreateLength();
            this.CreateCharIndexer();
            this.CreateStringIndexer();
        }

        private void CreateStringIndexer()
        {
            this.StringIndexer = this.ICharStream.Indexers.Add(_identityManager.ObtainTypeReference(RuntimeCoreType.String), new TypedNameSeries(new TypedName("start", RuntimeCoreType.Int32, _identityManager), new TypedName("end", RuntimeCoreType.Int32, _identityManager)), true, false);
        }

        private void CreateCharIndexer()
        {
            this.CharIndexer = this.ICharStream.Indexers.Add(_identityManager.ObtainTypeReference(RuntimeCoreType.Int32), new TypedNameSeries(new TypedName("offset", RuntimeCoreType.Int32, _identityManager)), true, false);
        }

        public void CreateCharStreamCtor()
        {
            var ctor = CharStream.Constructors.Add(new TypedName("baseStream", typeof(Stream).ObtainCILibraryType<IClassType>(this._identityManager)));
            ctor.AccessLevel = AccessLevelModifiers.Public;
            var baseStreamParam = ctor.Parameters["baseStream"];
            //#1 rule in code generation, only use the string of a term once, everything else is a reference!
            ctor.SummaryText = string.Format("Creates a new @s:{0}; with the @p:{1}; provided.", CharStream.Name, baseStreamParam.Name);
            baseStreamParam.SummaryText = string.Format("The seekable @s:Stream; from which the {0} reads from.", this.CharStream.Name);
            ctor.RemarksText = string.Format("If @p:{0}; is not seekable, this type will fail.", baseStreamParam.Name);
            var nullCheck = ctor.If(baseStreamParam.EqualTo(IntermediateGateway.NullValue));
            nullCheck.Throw(this._identityManager.ObtainTypeReference(typeof(ArgumentNullException)).GetNewExpression(baseStreamParam.Name.ToPrimitive()));

            ctor.Assign(_BaseStreamImpl, baseStreamParam);
            ctor.Assign(_LengthImpl, baseStreamParam.GetReference().GetProperty("Length"));
        }

        private void CreateLookAheadMethodImpl()
        {
            
            var mathTypeExp = typeof(Math).ObtainCILibraryType<IClassType>(this._identityManager).GetTypeExpression();
            var maxMethod = mathTypeExp.GetMethod("Max");
            var lookAheadImpl = this.CharStream.Methods.Add(new TypedName("LookAhead", this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32)), new TypedNameSeries(new TypedName("distance", this._identityManager.ObtainTypeReference(RuntimeCoreType.Int32))));
            this.LookAheadImpl = lookAheadImpl;
            var dist = lookAheadImpl.Parameters[TypeSystemIdentifiers.GetMemberIdentifier("distance")];

            lookAheadImpl.AccessLevel = AccessLevelModifiers.Private;
            lookAheadImpl.SummaryText = "Returns the @s:Char; value at the @p:distance;.";
            dist.SummaryText = "The distance from the the start of the @s:BaseStream; to read a character.";

            IConditionBlockStatement posCheck;
            { 
                /* *
                 * if (distance >= this.Length ||
                 *     /* End of file correction: sometimes, things like a Unicode header, take a few bytes. *//*
                 *     this.eofEncountered && distance >= this.actualBufferLength)
                 *     return -1;
                 * */
                posCheck =
                    LookAheadImpl
                        .If(
                            dist
                                .GreaterThanOrEqualTo(
                            this.LengthImpl)
                            .LogicalOr(
                            this.EofEncountered.GetReference()
                            .LeftNewLine()
                            .LeftComment("End of file correction: sometimes, things like a Unicode header, take a few bytes.")
                            .LeftNewLine()
                            .LogicalAnd(
                            dist
                                .GreaterThanOrEqualTo(
                            ActualBufferLength))));
                posCheck.Return(-1);
            }
            //if (this.actualBufferLength <= distance) { ...
            var bufferDataAvailCheck = LookAheadImpl.If(ActualBufferLength.LessThanOrEqualTo(dist));
            //long bytesRead = Math.Max(distance + 1L - this.actualBufferLength, 4096);
            var availCheckReadCount = bufferDataAvailCheck.Locals.Add(new TypedName("bytesRead", dist.ParameterType), maxMethod.Invoke(dist.Add(1).Subtract(ActualBufferLength), 4096.ToPrimitive()).Cast(lookAheadImpl.ReturnType));
            availCheckReadCount.AutoDeclare = false;
            bufferDataAvailCheck.DefineLocal(availCheckReadCount);

            var checkBufferCheck = bufferDataAvailCheck.If(BufferArrayField.EqualTo(IntermediateGateway.NullValue).LogicalOr(ActualBufferLength.Add(availCheckReadCount).GreaterThan(BufferArrayField.GetReference().GetProperty("LongLength"))));
            checkBufferCheck.Call(this.CheckBufferImpl.GetReference().Invoke(ActualBufferLength.Add(availCheckReadCount)));
            
            //if (bytesRead > 0) { ...
            var readCheck = bufferDataAvailCheck.If(availCheckReadCount.GreaterThan(IntermediateGateway.NumberZero));

            /* Make sure the stream is in place for the read. */
            /* *
             *     if (this._baseStream.Position != this.actualBufferLength)
             *         this._baseStream.Seek(this.actualBufferLength, SeekOrigin.Begin);
             *         
             * ** REMOVED ** 
             * Causes synchronization issues, unknown cause.
             * */
            //var readSeekCheck = readCheck.If(this._BaseStreamImpl.GetReference().GetProperty("Position").InequalTo(this.ActualBufferLength));
            //readSeekCheck.Call(this._BaseStreamImpl.GetReference().GetMethod("Seek").Invoke(this.ActualBufferLength.GetReference(), this._identityManager.ObtainTypeReference(typeof(SeekOrigin)).GetTypeExpression().GetField("Begin")));
            var actualBytesRead = readCheck.Locals.Add(new TypedName("actualBytesRead", RuntimeCoreType.Int32, this._identityManager));
            actualBytesRead.AutoDeclare = false;
            readCheck.DefineLocal(actualBytesRead);

            //     int actualBytesRead = this.StreamReader.ReadBlock(this.buffer, (int)this.actualBufferLength, (int)bytesRead);
            actualBytesRead.InitializationExpression = this.StreamReaderImpl.GetReference().GetMethod("ReadBlock").Invoke(BufferArrayField.GetReference(), ActualBufferLength.GetReference().Cast(lookAheadImpl.ReturnType), availCheckReadCount.GetReference().Cast(lookAheadImpl.ReturnType));
            //     this.actualBufferLength += actualBytesRead;
            readCheck.Assign(ActualBufferLength.GetReference(), AssignmentOperation.AddAssign, actualBytesRead.GetReference());

            /* *
             *     if (actualBytesRead != bytesRead)
             *         this.eofEncountered = true;
             * */
            var eofCheck = readCheck.If(actualBytesRead.InequalTo(availCheckReadCount));
            eofCheck.Assign(this.EofEncountered.GetReference(), IntermediateGateway.TrueValue);
            /* *
             *     } // //if (bytesRead > 0)
             * } // if (this.actualBufferLength <= distance)
             * return ((int)(this.buffer[distance]));
             * */
            LookAheadImpl.Return(BufferArrayField.GetReference().GetIndexer(dist.GetReference()).Cast(lookAheadImpl.ReturnType));

        }

        private void CreateCheckBuffer()
        {
            var mathTypeExp = typeof(Math).ObtainCILibraryType<IClassType>((ICliManager)this._identityManager).GetTypeExpression();
            var maxMethod = mathTypeExp.GetMethod("Max");

            var bufferCharArray = this.CharStream.Fields.Add(new TypedName("buffer", RuntimeCoreType.Char, this._identityManager));
            bufferCharArray.AccessLevel = AccessLevelModifiers.Private;
            var actualBufferLength = this.CharStream.Fields.Add(new TypedName("actualBufferLength", RuntimeCoreType.Int64, this._identityManager));
            actualBufferLength.AccessLevel = AccessLevelModifiers.Private;

            var checkBufferMethod = this.CharStream.Methods.Add(new TypedName("CheckBuffer", RuntimeCoreType.VoidType, this._identityManager), new TypedNameSeries(new TypedName("requiredLength", RuntimeCoreType.Int64, this._identityManager)));
            var cbRequiredLength = checkBufferMethod.Parameters["requiredLength"];
            var charTypeRef = bufferCharArray.FieldType;
            bufferCharArray.FieldType = charTypeRef.MakeArray();
            bufferCharArray.SummaryText = "Denotes the @s:Char; array which acts as a buffer to the file.";
            checkBufferMethod.SummaryText = string.Format("Checks the state of the internal @s:Char; @s:{0}; on whether it's at least as large as @p:{1};.", bufferCharArray.Name, cbRequiredLength.Name);
            cbRequiredLength.SummaryText = string.Format("The @s:Int64; value which denotes how large the @s:{0}; needs to be in order to succeed.", bufferCharArray.Name);

            // if (this.buffer == null)
            var bufferNullCheck = checkBufferMethod.If(bufferCharArray.EqualTo(IntermediateGateway.NullValue));
            //     this.buffer = new char[Math.Max(requiredLength, 4096)];
            bufferNullCheck.Assign(bufferCharArray, new MalleableCreateArrayExpression(charTypeRef, maxMethod.Invoke(cbRequiredLength.GetReference(), 4096.ToPrimitive())));
            // else if (requiredLength >= this.buffer.LongLength)
            bufferNullCheck.CreateNext(cbRequiredLength.GreaterThanOrEqualTo(bufferCharArray.GetReference().GetProperty("LongLength")));

            // ... new char[...;
            var tempCreate = new MalleableCreateArrayDetailExpression(charTypeRef);
            // ... Math.Max(requiredLength, this.buffer.LongLength * 2)]
            tempCreate.Sizes.Add(maxMethod.Invoke(cbRequiredLength.GetReference(), bufferCharArray.GetReference().GetProperty("LongLength").Multiply(2)));

            //     char[] tempBuffer = new char[Math.Max(requiredLength, this.buffer.LongLength * 2)];
            var tempBuffer = bufferNullCheck.Next.Locals.Add(new TypedName("tempBuffer", bufferCharArray.FieldType), tempCreate);

            var arrayType = typeof(Array).ObtainCILibraryType<IClassType>(this._identityManager);
            //     Array.Copy(this.buffer, 0, tempBuffer, 0, this.actualBufferLength);
            var copy = arrayType.Methods.Values.First(k => k.Name == "Copy" && k.Parameters.Count == 5 && k.Parameters.Values.Last().ParameterType == cbRequiredLength.ParameterType);
            bufferNullCheck.Next.Call(copy.GetReference().Invoke(bufferCharArray.GetReference(), IntermediateGateway.NumberZero, tempBuffer.GetReference(), IntermediateGateway.NumberZero, actualBufferLength.GetReference()));
            // this.buffer = tempBuffer;
            bufferNullCheck.Next.Assign(bufferCharArray, tempBuffer);

            this.BufferArrayField = bufferCharArray;
            this.ActualBufferLength = actualBufferLength;
            checkBufferMethod.AccessLevel = AccessLevelModifiers.Private;
            this.CheckBufferImpl = checkBufferMethod;
        }

        public IIntermediateInterfaceType ICharStream { get; private set; }

        public IIntermediateClassType CharStream { get; private set; }

        public IIntermediateClassPropertyMember LengthImpl { get; private set; }//

        public IIntermediateInterfacePropertyMember Length { get; private set; }

        public IIntermediateClassPropertyMember BaseStreamImpl { get; private set; }//

        public IIntermediateClassFieldMember _BaseStreamImpl { get; private set; }

        public IIntermediateInterfacePropertyMember BaseStream { get; private set; }

        public IIntermediateInterfacePropertyMember StreamReader { get; private set; }//

        public IIntermediateClassPropertyMember StreamReaderImpl { get; private set; }

        public IIntermediateClassFieldMember _StreamReaderImpl { get; private set; }//

        public IIntermediateClassIndexerMember CharIndexerImpl { get; private set; }

        public IIntermediateInterfaceIndexerMember CharIndexer { get; private set; }

        public IIntermediateClassIndexerMember SegmentIndexerImpl { get; private set; }

        public IIntermediateInterfaceIndexerMember SegmentIndexer { get; private set; }

        public IIntermediateClassMethodMember GetLineIndexImpl { get; private set; }

        public IIntermediateInterfaceMethodMember GetLineIndex { get; private set; }

        public IIntermediateClassMethodMember GetColumnIndexImpl { get; private set; }

        public IIntermediateInterfaceMethodMember GetColumnIndex { get; private set; }

        public IIntermediateClassMethodMember LookAheadImpl { get; private set; }

        public IIntermediateClassFieldMember BufferArrayField { get; private set; }

        public IIntermediateClassFieldMember ActualBufferLength { get; private set; }

        public IIntermediateClassMethodMember CheckBufferImpl { get; private set; }

        public IIntermediateClassFieldMember EofEncountered { get; private set; }

        public IIntermediateClassFieldMember BufferLocker { get; private set; }

        public IIntermediateInterfaceIndexerMember StringIndexer { get; private set; }

        public IIntermediateClassFieldMember StringCacheImpl { get; private set; }

        public IIntermediateClassFieldMember _LengthImpl { get; private set; }

        public IIntermediateClassIndexerMember StringIndexerImpl { get; private set; }

        public IIntermediateClassMethodMember DisposeMethodImpl { get; set; }
    }
}
