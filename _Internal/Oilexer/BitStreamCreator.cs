using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Languages.CSharp;
using AllenCopeland.Abstraction.Slf.Languages.CSharp.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Expressions;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Ast.Statements;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal class CharStreamClass
    {
        public IIntermediateClassType BitStream { get; private set; }
        public IIntermediateClassFieldMember Buffer { get; private set; }
        public IIntermediateClassFieldMember ActualSize { get; private set; }
        public IIntermediateClassMethodMember PurgeMethod { get; private set; }
        public IIntermediateClassMethodMember PushStringMethod { get; private set; }
        public IIntermediateClassMethodMember PushCharMethod { get; private set; }
        public IIntermediateClassMethodMember ToStringMethod { get; private set; }
        public IIntermediateClassMethodMember GrowBufferMethod { get; private set; }

        public CharStreamClass(IIntermediateClassType bitStream, IIntermediateClassFieldMember buffer, IIntermediateClassFieldMember actualSize,
            IIntermediateClassMethodMember purgeMethod, IIntermediateClassMethodMember pushStringMethod, IIntermediateClassMethodMember pushCharMethod,
            IIntermediateClassMethodMember toStringMethod, IIntermediateClassMethodMember growBufferMethod)
            : base()
        {
            this.BitStream = bitStream;
            this.Buffer = buffer;
            this.ActualSize = actualSize;
            this.PurgeMethod = purgeMethod;
            this.PushStringMethod = pushStringMethod;
            this.PushCharMethod = pushCharMethod;
            this.ToStringMethod = toStringMethod;
            this.GrowBufferMethod = growBufferMethod;
        }
    }
    internal static class BitStreamCreator
    {
        public static CharStreamClass CreateBitStream(IIntermediateTypeParent parent, IIntermediateCliManager identityManager)
        {
            IIntermediateClassType result = parent.Classes.Add("CharStream");

            result.AccessLevel = AccessLevelModifiers.Internal;
            IIntermediateClassFieldMember charBuffer = result.Fields.Add(new TypedName("buffer", identityManager.ObtainTypeReference(RuntimeCoreType.Char).MakeArray()));
            charBuffer.AccessLevel = AccessLevelModifiers.Internal;
            IIntermediateClassFieldMember charBufferSize = result.Fields.Add(new TypedName("actualSize", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            charBufferSize.AccessLevel = AccessLevelModifiers.Internal;
            charBufferSize.InitializationExpression = IntermediateGateway.NumberZero;
            var purgeMethod = AddPurgeMethod(result, charBufferSize, identityManager);
            var growBufferMethod = AddGrowBufferMethod(result, charBufferSize, charBuffer, identityManager);
            var pushStringMethod = AddPushStringMethod(result, charBufferSize, charBuffer, growBufferMethod, identityManager);
            var toStringMethod = AddToStringMethod(result, charBufferSize, charBuffer, identityManager);
            var pushCharMethod = AddPushMethod(result, charBufferSize, charBuffer, growBufferMethod, identityManager);
            return new CharStreamClass(result, charBuffer, charBufferSize, purgeMethod, pushStringMethod, pushCharMethod, toStringMethod, growBufferMethod);
        }

        private static IIntermediateClassMethodMember AddToStringMethod(IIntermediateClassType result, IIntermediateClassFieldMember charBufferSize, IIntermediateClassFieldMember charBuffer, IIntermediateCliManager identityManager)
        {
            /* *
             * Full method:
             * char[] result = new char[this.actualSize];
             * for (int i = 0; i < this.actualSize; i++)
             *     result[i] = buffer[i];
             * return new string(result);
             * */
            IIntermediateClassMethodMember toStringOverride = result.Methods.Add(new TypedName("ToString", identityManager.ObtainTypeReference(RuntimeCoreType.String)));
            toStringOverride.AccessLevel = AccessLevelModifiers.Public;
            toStringOverride.IsOverride = true;
            //char[] result = new char[this.actualSize];
            var resultCharsInitExp = new MalleableCreateArrayDetailExpression(identityManager.ObtainTypeReference(RuntimeCoreType.Char));
            resultCharsInitExp.Sizes.Add(charBufferSize.GetReference());
            var resultChars = toStringOverride.Locals.Add(new TypedName("result", identityManager.ObtainTypeReference(RuntimeCoreType.Char).MakeArray()), resultCharsInitExp);
            
            var iLocal = toStringOverride.Locals.Add(new TypedName("i", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            //int i = 0;
            iLocal.InitializationExpression = IntermediateGateway.NumberZero;
            //So it isn't declared in the main body.
            iLocal.AutoDeclare = false;
            //i++

            var increment = iLocal.Increment();
            //for (int i = 0; i < this.actualSize; i++)

            var loop = toStringOverride.Iterate(iLocal.GetDeclarationStatement(), iLocal.LessThan(charBufferSize), new IStatementExpression[] { increment });
            //    result[i] = this.buffer[i];
            loop.Assign(resultChars.GetReference().GetIndexer(iLocal.GetReference()), charBuffer.GetReference().GetIndexer(iLocal.GetReference()));
            //return new string(result);
            toStringOverride.Return(identityManager.ObtainTypeReference(RuntimeCoreType.String).GetNewExpression(resultChars.GetReference()));
            return toStringOverride;
        }

        private static IIntermediateClassMethodMember AddGrowBufferMethod(IIntermediateClassType result, IIntermediateClassFieldMember charBufferSize, IIntermediateClassFieldMember charBuffer, IIntermediateCliManager identityManager)
        {
            /* *
             * Full Method:
             * if (this.buffer == null)
             * {
             *     this.buffer = new char[totalSize];
             *     return;
             * }
             * if (this.buffer.Length >= totalSize)
             *     return;
             * int pNew = this.buffer.Length * 2;
             * if (totalSize > pNew)
             *     pNew = totalSize;
             * char[] newBuffer = new char[pNew];
             * this.buffer.CopyTo(newBuffer, 0);
             * this.buffer = newBuffer;
             * */
            var growBufferMethod = result.Methods.Add(new TypedName("GrowBuffer", identityManager.ObtainTypeReference(RuntimeCoreType.VoidType)));
            var totalSizeParameter = growBufferMethod.Parameters.Add(new TypedName("totalSize", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
//          if (this.buffer == null)
//          {
            var nullCheck = growBufferMethod.If(charBuffer.GetReference().EqualTo(IntermediateGateway.NullValue));
//              this.buffer = new char[totalSize];
            nullCheck.Assign(charBuffer.GetReference(), new MalleableCreateArrayExpression(identityManager.ObtainTypeReference(RuntimeCoreType.Char), totalSizeParameter.GetReference()));
//              return;
//          }
            nullCheck.Return();
//          if (this.buffer.Length >= totalSize)
            var needCheck = growBufferMethod.If(charBuffer.GetReference().GetProperty("Length").GreaterThanOrEqualTo(totalSizeParameter));
//              return;
            needCheck.Return();

//          int pNew = this.actualSize * 2;
            var pNewVar = growBufferMethod.Locals.Add(new TypedName("pNew", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)), charBufferSize.Multiply(2));
            //So it isn't declared automatically, thus causing a potential null reference exception.
            pNewVar.AutoDeclare = false;
            growBufferMethod.DefineLocal(pNewVar);
//          if (totalSize > pNew)
            var rangeCheck = growBufferMethod.If(totalSizeParameter.GreaterThan(pNewVar));
//              pNew = totalSize;
            rangeCheck.Assign(pNewVar.GetReference(), totalSizeParameter.GetReference());

//          char[] newBuffer = new char[pNew];
            var newBufferVar = growBufferMethod.Locals.Add(new TypedName("newBuffer", identityManager.ObtainTypeReference(RuntimeCoreType.Char).MakeArray()), new MalleableCreateArrayExpression(identityManager.ObtainTypeReference(RuntimeCoreType.Char), pNewVar.GetReference()));
            //So newBuffer doesn't refer to pNew before it is declared.
            newBufferVar.AutoDeclare = false;
            growBufferMethod.DefineLocal(newBufferVar);

//          this.buffer.CopyTo(newBuffer, 0);
            growBufferMethod.Call(charBuffer.GetReference().GetMethod("CopyTo").
                Invoke(newBufferVar.GetReference(), IntermediateGateway.NumberZero));

//          this.buffer = newBuffer;
            growBufferMethod.Assign(charBuffer.GetReference(), newBufferVar.GetReference());
            return growBufferMethod;
        }

        private static IIntermediateClassMethodMember AddPushStringMethod(IIntermediateClassType result, IIntermediateClassFieldMember charBufferSize, IIntermediateClassFieldMember charBuffer, IIntermediateClassMethodMember growBufferMethod, IIntermediateCliManager identityManager)
        {
            /* *
             * Full Method:
             * if (buffer == null)
             *     GrowBuffer(s.Length);
             * else if (buffer.Length < actualSize + s.Length)
             *     GrowBuffer(actualSize + s.Length);
             * for (int i = 0; i < s.Length; i++)
             * {
             *     buffer[actualSize] = s[i];
             *     actualSize++;
             * }
             * */
            IIntermediateClassMethodMember pushStringMethod = result.Methods.Add(new TypedName("Push", identityManager.ObtainTypeReference(RuntimeCoreType.VoidType)));
            pushStringMethod.AccessLevel = AccessLevelModifiers.Public;
            var sParameter = pushStringMethod.Parameters.Add(new TypedName("s", identityManager.ObtainTypeReference(RuntimeCoreType.String)));
//          if (buffer == null)

            var nullCheck = pushStringMethod.If(charBuffer.GetReference().EqualTo(IntermediateGateway.NullValue));
//              GrowBuffer(s.Length);
            nullCheck.Call(growBufferMethod.GetReference().Invoke(sParameter.GetReference().GetProperty("Length")));
//          else if (buffer.Length < actualSize + s.Length)
            nullCheck.CreateNext(charBuffer.GetReference().GetProperty("Length").LessThan(charBufferSize.GetReference().Add(sParameter.GetReference().GetProperty("Length"))));
            var rangeCheck = (IConditionBlockStatement)nullCheck.Next;
//              GrowBuffer(actualSize + s.Length);
            rangeCheck.Call(growBufferMethod.GetReference().Invoke(charBufferSize.GetReference().Add(sParameter.GetReference().GetProperty("Length"))));

            //int i = 0;
            var iLocal = pushStringMethod.Locals.Add(new TypedName("i", identityManager.ObtainTypeReference(RuntimeCoreType.Int32)));
            //So it isn't declared in the main body.
            iLocal.InitializationExpression = IntermediateGateway.NumberZero;
            iLocal.AutoDeclare = false;
            //i++
//          for (int i = 0; i < s.Length; i++)
//          {
            var sToBufferIterate = pushStringMethod.Iterate(iLocal.GetDeclarationStatement(), iLocal.LessThan(sParameter.GetReference().GetProperty("Length")), new IStatementExpression[] { iLocal.Increment() });
            //var sToBufferIterate = pushStringMethod.Iterate(iLocal.GetDeclarationStatement(), IntermediateGateway.NumberZero, sParameter.GetReference().GetProperty("Length"));
//              buffer[actualSize++] = s[i];
            sToBufferIterate.Assign(charBuffer.GetReference().GetIndexer(charBufferSize.Increment()), sParameter.GetReference().GetIndexer(iLocal.GetReference()));
//          }
            return pushStringMethod;
        }

        private static IIntermediateClassMethodMember AddPurgeMethod(IIntermediateClassType result, IIntermediateClassFieldMember charBufferSize, IIntermediateCliManager identityManager)
        {
            var purgeMethod = result.Methods.Add(new TypedName("Purge", identityManager.ObtainTypeReference(RuntimeCoreType.VoidType)));
            purgeMethod.AccessLevel = AccessLevelModifiers.Public;
            purgeMethod.Assign(charBufferSize.GetReference(), IntermediateGateway.NumberZero);
            return purgeMethod;
        }

        private static IIntermediateClassMethodMember AddPushMethod(IIntermediateClassType result, IIntermediateClassFieldMember charBufferSize, IIntermediateClassFieldMember charBuffer, IIntermediateClassMethodMember growBufferMethod, IIntermediateCliManager identityManager)
        {
            /* *
             * Full Method:
             * if (buffer == null)
             *     GrowBuffer(2);
             * else if (buffer.Length < actualSize + 1)
             *     GrowBuffer(actualSize + 1);
             * buffer[actualSize] = c;
             * actualSize++;
             * */
            IIntermediateClassMethodMember pushMethod = result.Methods.Add(new TypedName("Push", identityManager.ObtainTypeReference(RuntimeCoreType.VoidType)));
            pushMethod.AccessLevel = AccessLevelModifiers.Public;
            var cParameter = pushMethod.Parameters.Add(new TypedName("c", identityManager.ObtainTypeReference(RuntimeCoreType.Char)));
//          if (buffer == null)
            var nullCheck = pushMethod.If(charBuffer.EqualTo(IntermediateGateway.NullValue));
//              GrowBuffer(2);
            nullCheck.Call(growBufferMethod.GetReference().Invoke(2.ToPrimitive()));
//          else if (buffer.Length < actualSize + 1)
            nullCheck.CreateNext(charBuffer.GetReference().GetProperty("Length").LessThan(charBufferSize.GetReference().Add(1.ToPrimitive())));
            var rangeCheck = (IConditionBlockStatement)nullCheck.Next;
//              GrowBuffer(actualSize + 1);
            rangeCheck.Call(growBufferMethod.GetReference().Invoke(charBufferSize.GetReference().Add(1.ToPrimitive())));
//          buffer[actualSize++] = c;
            pushMethod.Assign(charBuffer.GetReference().GetIndexer(charBufferSize.Increment()), cParameter.GetReference());
            return pushMethod;
        }

    }
}
