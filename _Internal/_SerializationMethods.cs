using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Oilexer.Expression;
using Oilexer.Types;

namespace Oilexer._Internal
{
    /// <summary>
    /// Provides a series of serialization methods for the OIL Architecture.
    /// </summary>
    internal static class _SerializationMethods
    {
        internal static void SerializeMultiFaceValue<T>(string baseName, SerializationInfo info, T value)
            where T :
                class
        {
            string type = string.Format("{0}.Type", baseName);
            if (value == null)
                info.AddValue(type, typeof(void), typeof(Type));
            else
            {
                info.AddValue(type, value.GetType(), typeof(Type));
                info.AddValue(baseName, value, value.GetType());
            }
        }
        internal static T DeserializeMultiFaceValue<T>(string baseName, SerializationInfo info)
            where T :
                class
        {
            string type = string.Format("{0}.Type", baseName);
            System.Type valueType = (Type)info.GetValue(type, typeof(Type));
            if (valueType != typeof(void))
                return (T)info.GetValue(baseName, valueType);
            else
                return null;
        }

        /// <summary>
        /// Serializes an expression with the option to omit the actual expression if it is null.
        /// </summary>
        /// <param name="baseName">The base name to serialize the expression under.</param>
        /// <param name="info">The <see cref="SerializationInfo"/> to use to serialize 
        /// the expression.</param>
        /// <param name="expression">The expression to serialize.</param>
        internal static void SerializeExpression(string baseName, SerializationInfo info, IExpression expression)
        {
            SerializeMultiFaceValue<IExpression>(baseName, info, expression);
        }

        internal static IExpression DeserializeExpression(string baseName, SerializationInfo info)
        {
            return DeserializeMultiFaceValue<IExpression>(baseName, info);
        }

        internal static void SerializeExpressionCollection(string baseName, SerializationInfo info, IExpressionCollection collection)
        {
            if (collection == null)
            {
                info.AddValue(baseName, -1);
                return;
            }
            info.AddValue(baseName, collection.Count);
            for (int i = 0; i < collection.Count; i++)
            {
                string itemBaseName = string.Format("{0}.[{1}]", baseName, i);
                SerializeExpression(itemBaseName, info, collection[i]);
            }
        }

        internal static IExpressionCollection DeserializeExpressionCollection(string baseName, SerializationInfo info)
        {
            int count = info.GetInt32(baseName);
            if (count == -1)
                return null;
            IExpressionCollection result = new ExpressionCollection();
            for (int i = 0; i < count; i++)
            {
                string itemBaseName = string.Format("{0}.[{1}]", baseName, i);
                IExpression current = DeserializeExpression(itemBaseName, info);
                if (current != null)
                    result.Add(current);
            }
            return result;
        }

        internal static void SerializeTypeReference(string baseName, SerializationInfo info, ITypeReference typeReference)
        {
            SerializeMultiFaceValue<ITypeReference>(baseName, info, typeReference);
        }

        internal static ITypeReference DeserializeTypeReference(string baseName, SerializationInfo info)
        {
            return DeserializeMultiFaceValue<ITypeReference>(baseName, info);
        }
    }
}
