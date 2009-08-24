using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens.StateSystem
{
    partial class RegularLanguageTransitionNode
    {
        public enum SourceElementType
        {
            /// <summary>
            /// The source element is a token entry.
            /// </summary>
            TokenEntry,
            /// <summary>
            /// The source element is a series of token expressions
            /// </summary>
            TokenSeries,
            /// <summary>
            /// The source element is an expression.
            /// </summary>
            TokenExpression,
            /// <summary>
            /// The source element is a token group.
            /// </summary>
            TokenGroup,
            /// <summary>
            /// The source element is a token item.
            /// </summary>
            TokenItem,
        }
        public class SourceElement :
            IEquatable<SourceElement>
        {
            private SourceElementType elementType;
            private IFlattenedTokenItem sourceItem;
            private FlattenedTokenExpression sourceExpression;
            private FlattenedTokenExpressionSeries sourceSeries;
            private FlattenedTokenEntry sourceEntry;
            private FlattenedTokenGroupItem sourceGroup;

            public SourceElement(IFlattenedTokenItem item)
            {
                this.sourceItem = item;
                this.elementType = SourceElementType.TokenItem;
            }

            public SourceElement(FlattenedTokenExpression sourceExpression)
            {
                this.sourceExpression = sourceExpression;
                this.elementType = SourceElementType.TokenExpression;
            }

            public SourceElement(FlattenedTokenGroupItem sourceGroup)
            {
                this.sourceGroup = sourceGroup;
                this.elementType = SourceElementType.TokenGroup;
            }

            public SourceElement(FlattenedTokenExpressionSeries sourceSeries)
            {
                this.sourceSeries = sourceSeries;
                this.elementType = SourceElementType.TokenSeries;
            }

            public SourceElement(FlattenedTokenEntry sourceEntry)
            {
                this.sourceEntry = sourceEntry;
                this.elementType = SourceElementType.TokenEntry;
            }

            public static implicit operator SourceElement(FlattenedTokenCharRangeItem item)
            {
                return new SourceElement(item);
            }

            public static implicit operator SourceElement(FlattenedTokenLiteralItem item)
            {
                return new SourceElement(item);
            }

            public static implicit operator SourceElement(FlattenedScanTokenCommandItem item)
            {
                return new SourceElement(item);
            }

            public static implicit operator SourceElement(FlattenedTokenExpression expression)
            {
                return new SourceElement(expression);
            }

            public static implicit operator SourceElement(FlattenedTokenEntry entry)
            {
                return new SourceElement(entry);
            }

            public static implicit operator SourceElement(FlattenedTokenExpressionSeries series)
            {
                return new SourceElement(series);
            }

            public static implicit operator SourceElement(FlattenedTokenGroupItem group)
            {
                return new SourceElement(group);
            }

            public static explicit operator FlattenedTokenGroupItem(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.TokenGroup)
                    throw new ArgumentException("element");
                return element.sourceGroup;
            }

            public static explicit operator FlattenedTokenExpression(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.TokenExpression)
                    throw new ArgumentException("element");
                return element.sourceExpression;
            }

            public static explicit operator FlattenedTokenExpressionSeries(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.TokenSeries)
                    throw new ArgumentException("element");
                return element.sourceSeries;
            }

            /// <summary>
            /// Explicitly converts the <paramref name="element"/> to a
            /// <see cref="FlattenedTokenEntry"/>.
            /// </summary>
            /// <param name="element">The <see cref="SourceElement"/> to convert.</param>
            /// <returns>The <see cref="FlattenedTokenEntry"/> instance that the
            /// <see cref="SourceElement"/> was created with.</returns>
            public static explicit operator FlattenedTokenEntry(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.TokenEntry)
                    throw new ArgumentException("element");
                return element.sourceEntry;
            }

            public static bool operator ==(SourceElement left, SourceElement right)
            {
                if (object.ReferenceEquals(left, null))
                    return object.ReferenceEquals(right, null);
                else if (object.ReferenceEquals(right, null))
                    return false;
                if (left.elementType != right.elementType)
                    return false;
                switch (left.elementType)
                {
                    case SourceElementType.TokenEntry:
                        return left.sourceEntry == right.sourceEntry;
                    case SourceElementType.TokenSeries:
                        return left.sourceSeries == right.sourceSeries;
                    case SourceElementType.TokenExpression:
                        return left.sourceExpression == right.sourceExpression;
                    case SourceElementType.TokenGroup:
                        return left.sourceGroup == right.sourceGroup;
                    case SourceElementType.TokenItem:
                        return left.sourceItem == right.sourceItem;
                    default:
                        //The element's type was invalid.
                        throw new ArgumentOutOfRangeException("left");
                }
            }

            public static bool operator !=(SourceElement left, SourceElement right)
            {
                if (object.ReferenceEquals(left, null))
                    return !object.ReferenceEquals(right, null);
                else if (object.ReferenceEquals(right, null))
                    return true;
                if (left.elementType != right.elementType)
                    return true;
                switch (left.elementType)
                {
                    case SourceElementType.TokenEntry:
                        return left.sourceEntry != right.sourceEntry;
                    case SourceElementType.TokenSeries:
                        return left.sourceSeries != right.sourceSeries;
                    case SourceElementType.TokenExpression:
                        return left.sourceExpression != right.sourceExpression;
                    case SourceElementType.TokenGroup:
                        return left.sourceGroup != right.sourceGroup;
                    case SourceElementType.TokenItem:
                        return left.sourceItem != right.sourceItem;
                    default:
                        //The element's type was invalid.
                        throw new ArgumentOutOfRangeException("left");
                }

            }

            #region IEquatable<SourceElement> Members

            public bool Equals(SourceElement other)
            {
                return other == this;
            }

            #endregion

            public override bool Equals(object obj)
            {
                var other = obj as SourceElement;
                if (obj == null)
                    return false;
                return other == this;
            }

            public override int GetHashCode()
            {
                var elementTypeHash = this.elementType.GetHashCode();
                switch (this.elementType)
                {
                    case SourceElementType.TokenEntry:
                        return this.sourceEntry.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.TokenSeries:
                        return this.sourceSeries.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.TokenExpression:
                        return this.sourceExpression.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.TokenGroup:
                        return this.sourceGroup.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.TokenItem:
                        return this.sourceItem.GetHashCode() ^ elementTypeHash;
                    default:
                        throw new InvalidOperationException("invalid state");
                }
            }
        }
    }
}
