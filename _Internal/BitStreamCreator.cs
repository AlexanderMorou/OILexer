using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Expression;
using Oilexer.Statements;
using System.CodeDom;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal class CharStreamClass
    {
        public IClassType BitStream { get; private set; }
        public IFieldMember Buffer { get; private set; }
        public IFieldMember ActualSize { get; private set; }
        public IMethodMember PurgeMethod { get; private set; }
        public IMethodMember PushStringMethod { get; private set; }
        public IMethodMember PushCharMethod { get; private set; }
        public IMethodMember ToStringMethod { get; private set; }
        public IMethodMember GrowBufferMethod { get; private set; }

        public CharStreamClass(IClassType bitStream, IFieldMember buffer, IFieldMember actualSize,
            IMethodMember purgeMethod, IMethodMember pushStringMethod, IMethodMember pushCharMethod,
            IMethodMember toStringMethod, IMethodMember growBufferMethod)
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
        public static CharStreamClass CreateBitStream(ITypeParent parent)
        {
            IClassType result = parent.Classes.AddNew("CharStream");
            
            result.AccessLevel = DeclarationAccessLevel.Internal;
            IFieldMember charBuffer = result.Fields.AddNew(new TypedName("buffer", typeof(char[])));
            charBuffer.AccessLevel = DeclarationAccessLevel.Internal;
            IFieldMember charBufferSize = result.Fields.AddNew(new TypedName("actualSize", typeof(int)));
            charBufferSize.AccessLevel = DeclarationAccessLevel.Internal;
            charBufferSize.InitializationExpression = PrimitiveExpression.NumberZero;
            var purgeMethod = AddPurgeMethod(result, charBufferSize);
            var growBufferMethod = AddGrowBufferMethod(result, charBufferSize, charBuffer);
            var pushStringMethod = AddPushStringMethod(result, charBufferSize, charBuffer, growBufferMethod);
            var toStringMethod = AddToStringMethod(result, charBufferSize, charBuffer);
            var pushCharMethod = AddPushMethod(result, charBufferSize, charBuffer, growBufferMethod);
            return new CharStreamClass(result, charBuffer, charBufferSize, purgeMethod, pushStringMethod, pushCharMethod, toStringMethod, growBufferMethod);
        }

        private static IMethodMember AddToStringMethod(IClassType result, IFieldMember charBufferSize, IFieldMember charBuffer)
        {
            /* *
             * Full method:
             * char[] result = new char[this.actualSize];
             * for (int i = 0; i < this.actualSize; i++)
             *     result[i] = buffer[i];
             * return new string(result);
             * */
            IMethodMember toStringOverride = result.Methods.AddNew(new TypedName("ToString", typeof(string)));
            toStringOverride.AccessLevel = DeclarationAccessLevel.Public;
            toStringOverride.Overrides = true;
            //char[] result = new char[this.actualSize];
            var resultChars = toStringOverride.Locals.AddNew(new TypedName("result", typeof(char[])), new CreateArrayExpression(typeof(char).GetTypeReference(), charBufferSize.GetReference()));
            var iLocal = toStringOverride.Locals.AddNew(new TypedName("i", typeof(int)));
            //int i = 0;
            iLocal.InitializationExpression = PrimitiveExpression.NumberZero;
            //So it isn't declared in the main body.
            iLocal.AutoDeclare = false;
            //i++
            ICrementStatement icrement = new CrementStatement(CrementType.Postfix, CrementOperation.Increment, iLocal.GetReference());
            //for (int i = 0; i < this.actualSize; i++)
            var loop = toStringOverride.Iterate(iLocal.GetDeclarationStatement(), icrement, new BinaryOperationExpression(iLocal.GetReference(), CodeBinaryOperatorType.LessThan, charBufferSize.GetReference()));
            //    result[i] = this.buffer[i];
            loop.Assign(resultChars.GetReference().GetIndex(iLocal.GetReference()), charBuffer.GetReference().GetIndex(iLocal.GetReference()));
            //return new string(result);
            toStringOverride.Return(new CreateNewObjectExpression(typeof(string).GetTypeReference(), resultChars.GetReference()));
            return toStringOverride;
        }

        private static IMethodMember AddGrowBufferMethod(IClassType result, IFieldMember charBufferSize, IFieldMember charBuffer)
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
            var growBufferMethod = result.Methods.AddNew(new TypedName("GrowBuffer", typeof(void)));
            var totalSizeParameter = growBufferMethod.Parameters.AddNew(new TypedName("totalSize", typeof(int)));
//          if (this.buffer == null)
//          {
            var nullCheck = growBufferMethod.IfThen(new BinaryOperationExpression(charBuffer.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
//              this.buffer = new char[totalSize];
            nullCheck.Assign(charBuffer.GetReference(), new CreateArrayExpression(typeof(char).GetTypeReference(), totalSizeParameter.GetReference()));
//              return;
//          }
            nullCheck.Return();
//          if (this.buffer.Length >= totalSize)
            var needCheck = growBufferMethod.IfThen(new BinaryOperationExpression(charBuffer.GetReference().GetProperty("Length"), CodeBinaryOperatorType.GreaterThanOrEqual, totalSizeParameter.GetReference()));
//              return;
            needCheck.Return();

//          int pNew = this.buffer.Length * 2;
            var pNewVar = growBufferMethod.Locals.AddNew(new TypedName("pNew", typeof(int)), new BinaryOperationExpression(charBufferSize.GetReference(), CodeBinaryOperatorType.Multiply, new PrimitiveExpression(2)));
            //So it isn't declared automatically, thus causing a potential null reference exception.
            pNewVar.AutoDeclare = false;
            growBufferMethod.DefineLocal(pNewVar);
//          if (totalSize > pNew)
            var rangeCheck = growBufferMethod.IfThen(new BinaryOperationExpression(totalSizeParameter.GetReference(), CodeBinaryOperatorType.GreaterThan, pNewVar.GetReference()));
//              pNew = totalSize;
            rangeCheck.Assign(pNewVar.GetReference(), totalSizeParameter.GetReference());

//          char[] newBuffer = new char[pNew];
            var newBufferVar = growBufferMethod.Locals.AddNew(new TypedName("newBuffer", typeof(char[])), new CreateArrayExpression(typeof(char).GetTypeReference(), pNewVar.GetReference()));
            //So newBuffer doesn't refer to pNew before it is declared.
            newBufferVar.AutoDeclare = false;
            growBufferMethod.DefineLocal(newBufferVar);

//          this.buffer.CopyTo(newBuffer, 0);
            growBufferMethod.CallMethod(charBuffer.GetReference().GetMethod("CopyTo").
                Invoke(newBufferVar.GetReference(), PrimitiveExpression.NumberZero));

//          this.buffer = newBuffer;
            growBufferMethod.Assign(charBuffer.GetReference(), newBufferVar.GetReference());
            return growBufferMethod;
        }

        private static IMethodMember AddPushStringMethod(IClassType result, IFieldMember charBufferSize, IFieldMember charBuffer, IMethodMember growBufferMethod)
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
            IMethodMember pushStringMethod = result.Methods.AddNew(new TypedName("Push", typeof(void)));
            pushStringMethod.AccessLevel = DeclarationAccessLevel.Public;
            var sParameter = pushStringMethod.Parameters.AddNew(new TypedName("s", typeof(string)));
//          if (buffer == null)
            var nullCheck = pushStringMethod.IfThen(new BinaryOperationExpression(charBuffer.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
//              GrowBuffer(s.Length);
            nullCheck.CallMethod(growBufferMethod.GetReference().Invoke(sParameter.GetReference().GetProperty("Length")));
//          else if (buffer.Length < actualSize + s.Length)
            var rangeCheck = nullCheck.FalseBlock.IfThen(new BinaryOperationExpression(charBuffer.GetReference().GetProperty("Length"), CodeBinaryOperatorType.LessThan, new BinaryOperationExpression(charBufferSize.GetReference(), CodeBinaryOperatorType.Add, sParameter.GetReference().GetProperty("Length"))));
//              GrowBuffer(actualSize + s.Length);
            rangeCheck.CallMethod(growBufferMethod.GetReference().Invoke(new BinaryOperationExpression(charBufferSize.GetReference(), CodeBinaryOperatorType.Add, sParameter.GetReference().GetProperty("Length"))));

            //int i = 0;
            var iLocal = pushStringMethod.Locals.AddNew(new TypedName("i", typeof(int)));
            iLocal.InitializationExpression = PrimitiveExpression.NumberZero;
            //So it isn't declared in the main body.
            iLocal.AutoDeclare = false;
            //i++
            ICrementStatement icrement = new CrementStatement(CrementType.Postfix, CrementOperation.Increment, iLocal.GetReference());
//          for (int i = 0; i < s.Length; i++)
//          {
            var sToBufferIterate = pushStringMethod.Iterate(iLocal.GetDeclarationStatement(), icrement, new BinaryOperationExpression(iLocal.GetReference(), CodeBinaryOperatorType.LessThan, sParameter.GetReference().GetProperty("Length")));
//              buffer[actualSize] = s[i];
            sToBufferIterate.Assign(charBuffer.GetReference().GetIndex(charBufferSize.GetReference()), sParameter.GetReference().GetIndex(iLocal.GetReference()));
//              actualSize++;
//          }
            //Necessary because crement expressions don't exist in this version of OIL.
            sToBufferIterate.Crement(charBufferSize.GetReference(), CrementType.Postfix, CrementOperation.Increment);
            return pushStringMethod;
        }

        private static IMethodMember AddPurgeMethod(IClassType result, IFieldMember charBufferSize)
        {
            IMethodMember purgeMethod = result.Methods.AddNew(new TypedName("Purge", typeof(void)));
            purgeMethod.AccessLevel = DeclarationAccessLevel.Public;
//          this.actualSize = 0;
            purgeMethod.Assign(charBufferSize.GetReference(), PrimitiveExpression.NumberZero);
            return purgeMethod;
        }

        private static IMethodMember AddPushMethod(IClassType result, IFieldMember charBufferSize, IFieldMember charBuffer, IMethodMember growBufferMethod)
        {
            /* *
             * Full Method:
             * if (this.buffer == null)
             *     GrowBuffer(2);
             * else if (this.buffer.Length < this.actualSize + 1)
             *     GrowBuffer(this.actualSize + 1);
             * buffer[this.actualSize] = c;
             * this.actualSize++;
             * */
            IMethodMember pushMethod = result.Methods.AddNew(new TypedName("Push", typeof(void)));
            pushMethod.AccessLevel = DeclarationAccessLevel.Public;
            var cParameter = pushMethod.Parameters.AddNew(new TypedName("c", typeof(char)));
//          if (buffer == null)
            var nullCheck = pushMethod.IfThen(new BinaryOperationExpression(charBuffer.GetReference(), CodeBinaryOperatorType.IdentityEquality, PrimitiveExpression.NullValue));
//              GrowBuffer(2);
            nullCheck.CallMethod(growBufferMethod.GetReference().Invoke(new PrimitiveExpression(2)));
//          else if (buffer.Length < actualSize + 1)
            var rangeCheck = nullCheck.FalseBlock.IfThen(new BinaryOperationExpression(charBuffer.GetReference().GetProperty("Length"), CodeBinaryOperatorType.LessThan, new BinaryOperationExpression(charBufferSize.GetReference(), CodeBinaryOperatorType.Add, new PrimitiveExpression(1))));
//              GrowBuffer(actualSize + 1);
            rangeCheck.CallMethod(growBufferMethod.GetReference().Invoke(new BinaryOperationExpression(charBufferSize.GetReference(), CodeBinaryOperatorType.Add, new PrimitiveExpression(1))));
//          buffer[actualSize] = c;
            pushMethod.Assign(charBuffer.GetReference().GetIndex(charBufferSize.GetReference()), cParameter.GetReference());
//          actualSize++;
            pushMethod.Crement(charBufferSize.GetReference(), CrementType.Postfix, CrementOperation.Increment);
            return pushMethod;
        }

    }
}
