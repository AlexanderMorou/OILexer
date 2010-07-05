using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Utilities.Collections;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.IO;
using System.CodeDom.Compiler;
using Oilexer.Expression;
using System.Collections;
using Oilexer.Types.Members;
using Oilexer._Internal;
using System.CodeDom;
using Oilexer.Translation;
using Oilexer.Statements;

namespace Oilexer
{
    public enum TranslationLanguage
    {
        CSharp,
        /* *
         * Visual Basic translator removed due to issues with it not 
         * supporting iterators.
         * */
    }
    public static class CodeGeneratorHelper
    {
        private static readonly IDictionary<Type, IExternType> ExternTypeInstances = new Dictionary<System.Type, IExternType>();

        public static readonly ICodeDOMTranslationOptions DefaultDomOptions;
        public static readonly IIntermediateCodeTranslatorOptions DefaultTranslatorOptions;

        static CodeGeneratorHelper()
        {
            ICodeDOMTranslationOptions icdgo = new CodeDOMTranslationOptions(false);
            IIntermediateCodeTranslatorOptions icto = new IntermediateCodeTranslatorOptions(false);
            DefaultTranslatorOptions = icto;
            icdgo.LanguageProvider = _OIL._Core.DefaultCSharpCodeProvider;
            ((CodeDOMTranslationOptions)icdgo).locked = true;
            DefaultDomOptions = icdgo;
        }

        public static IMethodReferenceExpression GetGetTypeReferenceExpression()
        {
            return typeof(CodeGeneratorHelper).GetTypeReference().GetTypeExpression().GetMethod("GetTypeReference");
        }
        public static IMethodInvokeExpression GetGetTypeReferenceExpression(IExpression type)
        {
            return typeof(CodeGeneratorHelper).GetTypeReference().GetTypeExpression().GetMethod("GetTypeReference").Invoke(type);
        }

        public static ITypeReference[] GetTypeReferences(IType[] types)
        {
            ITypeReference[] result = new ITypeReference[types.Length];
            for (int i = 0; i < types.Length; i++)
                result[i] = types[i].GetTypeReference();
            return result;
        }
        public static ITypeReference[] GetTypeReferences(this Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException("types");
            ITypeReference[] result = new ITypeReference[types.Length];
            for (int i = 0; i < types.Length; i++)
                result[i] = types[i].GetTypeReference();
            return result;
        }

        public static IExpressionCollection GetExpressionCollection(this IExpression[] array)
        {
            return new ExpressionCollection(array);
        }

        public static ICastExpression Cast(this IExpression e, ITypeReference type)
        {
            return new CastExpression(e, type);
        }

        public static ICastExpression Cast(this IExpression e, Type t)
        {
            return e.Cast(t.GetTypeReference());
        }

        public static string ConvertToString(this IExpression expression, TranslationLanguage language)
        {
            StringFormWriter sfw = new StringFormWriter();
            IIntermediateCodeTranslator iict = null;
            switch (language)
            {
                case TranslationLanguage.CSharp:
                    iict = _OIL._Core.DefaultCSharpCodeTranslator;
                    break;
                //case TranslationLanguage.VisualBasic:
                //    iict = _OIL._Core.DefaultVBCodeTranslator;
                //    break;
                default:
                    throw new ArgumentOutOfRangeException("language");
            }
            iict.Target = sfw;
            iict.TranslateExpression(expression);
            string result = sfw.ToString();
            sfw.Close();
            sfw.Dispose();
            return result;
        }

        public static string ConvertToString(this IStatement statement, TranslationLanguage language)
        {
            return ConvertToString((iict, target)=>iict.TranslateStatement(target), statement, language);
        }

        public static string ConvertToString<TParent, TDom>(this IMember<TParent, TDom> member, TranslationLanguage language)
            where TParent :
                IDeclarationTarget
            where TDom :
                CodeObject
        {
            return ConvertToString((iict, target) => iict.TranslateMember(target), member, language);
        }

        /// <summary>
        /// Obtains the logical not of the <paramref name="target"/> <see cref="IExpression"/>.
        /// </summary>
        /// <param name="target">The <see cref="IExpression"/> to logically not.</param>
        /// <returns>A new <see cref="IUnaryOperationExpression"/> which is the logical not of the <paramref name="target"/>.</returns>
        public static IUnaryOperationExpression LogicalNot(this IExpression target)
        {
            return new UnaryOperationExpression(UnaryOperations.LogicalNot, target);
        }

        /// <summary>
        /// Obtains the compliement of the <paramref name="target"/> <see cref="IExpression"/>.
        /// </summary>
        /// <param name="target">The <see cref="IExpression"/> to obtain the compliement of.</param>
        /// <returns>A new <see cref="IUnaryOperationExpression"/> which is the compliement of the <paramref name="target"/>.</returns>
        public static IUnaryOperationExpression Compliment(this IExpression target)
        {
            return new UnaryOperationExpression(UnaryOperations.Compliment, target);
        }

        public static IUnaryOperationExpression Indirect(this IExpression target)
        {
            return new UnaryOperationExpression(UnaryOperations.Indirection, target);
        }

        public static IUnaryOperationExpression AddressOf(this IExpression target)
        {
            return new UnaryOperationExpression(UnaryOperations.AddressOf, target);
        }

        public static IUnaryOperationExpression Negate(this IExpression target)
        {
            return new UnaryOperationExpression(UnaryOperations.Negate, target);
        }

        public static IUnaryOperationExpression SizeOf(this ITypeReference target)
        {
            return new UnaryOperationExpression(UnaryOperations.SizeOf, target.GetTypeExpression());
        }

        public static IUnaryOperationExpression SizeOf(this Type target)
        {
            return new UnaryOperationExpression(UnaryOperations.SizeOf, target.GetTypeReferenceExpression());
        }

        public static string ConvertToString<T, TDom>(this T declType, TranslationLanguage language) 
            where T :
                IDeclaredType<TDom>
            where TDom :
                CodeTypeDeclaration
        {
            return ConvertToString((iict, target) => iict.TranslateType(target), declType, language);
        }

        private static string ConvertToString<T>(Action<IIntermediateCodeTranslator, T> targetTranslator, T targetEntity, TranslationLanguage language)
        {
            StringFormWriter sfw = new StringFormWriter();
            IIntermediateCodeTranslator iict = null;
            switch (language)
            {
                case TranslationLanguage.CSharp:
                    iict = _OIL._Core.DefaultCSharpCodeTranslator;
                    break;
                //case TranslationLanguage.VisualBasic:
                //    iict = _OIL._Core.DefaultVBCodeTranslator;
                //    break;
                default:
                    throw new ArgumentOutOfRangeException("language");
            }
            iict.Target = sfw;
            targetTranslator(iict, targetEntity);
            string result = sfw.ToString();
            sfw.Close();
            sfw.Dispose();
            return result;
        }

        public static ITypeReferenceExpression GetTypeReferenceExpression(this Type type)
        {
            return type.GetTypeReference().GetTypeExpression();
        }

        /// <summary>
        /// Obtains a <see cref="ITypeReference"/> for the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> to obtain the <see cref="ITypeReference"/>
        /// of.</param>
        /// <returns>A verifiable <see cref="ITypeReference"/> encapsulating <paramref name="type"/>.</returns>
        public static ITypeReference GetTypeReference(this Type type)
        {
            /* *
             * For whatever reason, types as arrays are reversed in their rank order.
             * */
            List<int> arrayRanks = new List<int>();
            System.Type currentType = type;
            bool nullable = false;
            if (currentType.IsByRef)
                currentType = currentType.GetElementType();
            while (currentType != null && currentType.IsArray)
            {
                arrayRanks.Add(currentType.GetArrayRank());
                currentType = currentType.GetElementType();
            }
            arrayRanks.Reverse();
            if (currentType.IsByRef)
                currentType = currentType.GetElementType();
            //Handle the special ValueType Nullable<T>.
            //There is only one Nullable<T> depth.
            if (currentType.IsGenericType && (!(currentType.IsGenericTypeDefinition)))
            {
                if (currentType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type[] gParams = currentType.GetGenericArguments();
                    if (gParams.Length == 1)
                    {
                        currentType = gParams[0];
                        nullable = true;
                    }
                }
            }
            ITypeReferenceCollection typeParameters = new TypeReferenceCollection();
            if (currentType.IsGenericType && !(currentType.IsGenericTypeDefinition))
            {
                Type[] typeParams;
                typeParams = currentType.GetGenericArguments();
                currentType = currentType.GetGenericTypeDefinition();
                typeParameters.AddRange(typeParams);
            }
            ITypeReference typeRef = new ExternType.TypeReference(currentType.GetExternType(), typeParameters);
            if (nullable)
                typeRef.Nullable = true;
            while (arrayRanks.Count > 0)
            {
                typeRef = typeRef.MakeArray(arrayRanks[0]);
                arrayRanks.RemoveAt(0);
            }
            return typeRef;
        }

        public static IExternTypeReference GetTypeReference(this Type type, ITypeReferenceCollection typeParameters)
        {
            return new ExternType.TypeReference(type.GetExternType(), typeParameters);
        }

        public static IExternType GetExternType(this Type type)
        {
            lock (ExternTypeInstances)
            {
                if (!(ExternTypeInstances.ContainsKey(type)))
                {
                    IExternType externType = null;
                    externType = new ExternType(type);
                    ExternTypeInstances.Add(type, externType);
                }
                return ExternTypeInstances[type];
            }
        }

        public static ICreateNewObjectExpression CreateNew(this Type t, params IExpression[] exprs)
        {
            return new CreateNewObjectExpression(t.GetTypeReference(), exprs);
        }

        public static ICreateNewObjectExpression CreateNew(this ITypeReference t, params IExpression[] exprs)
        {
            return new CreateNewObjectExpression(t, exprs);
        }

        public static ITypeReference GetTypeReference<T>()
        {
            return typeof(T).GetTypeReference();
        }

    }
}
