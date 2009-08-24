using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules.StateSystem
{
    partial class SimpleLanguageTransitionNode
    {
        public enum SourceElementType
        {
            /// <summary>
            /// The source element is a rule entry.
            /// </summary>
            RuleEntry,
            /// <summary>
            /// The source element is a series of rule expressions
            /// </summary>
            RuleSeries,
            /// <summary>
            /// The source element is an expression.
            /// </summary>
            RuleExpression,
            /// <summary>
            /// The source element is a rule group.
            /// </summary>
            RuleGroup,
            /// <summary>
            /// The source element is a rule item.
            /// </summary>
            RuleItem,
        }
        public class SourceElement :
            IEquatable<SourceElement>
        {
            private SourceElementType elementType;
            private IFlattenedRuleItem sourceItem;
            private FlattenedRuleExpression sourceExpression;
            private FlattenedRuleExpressionSeries sourceSeries;
            private FlattenedRuleEntry sourceEntry;
            private FlattenedRuleGroupItem sourceGroup;

            public SourceElement(IFlattenedRuleItem item)
            {
                this.sourceItem = item;
                this.elementType = SourceElementType.RuleItem;
            }

            public SourceElement(FlattenedRuleExpression sourceExpression)
            {
                this.sourceExpression = sourceExpression;
                this.elementType = SourceElementType.RuleExpression;
            }

            public SourceElement(FlattenedRuleGroupItem sourceGroup)
            {
                this.sourceGroup = sourceGroup;
                this.elementType = SourceElementType.RuleGroup;
            }

            public SourceElement(FlattenedRuleExpressionSeries sourceSeries)
            {
                this.sourceSeries = sourceSeries;
                this.elementType = SourceElementType.RuleSeries;
            }

            public SourceElement(FlattenedRuleEntry sourceEntry)
            {
                this.sourceEntry = sourceEntry;
                this.elementType = SourceElementType.RuleEntry;
            }

            public static implicit operator SourceElement(FlattenedRuleItem item)
            {
                return new SourceElement(item);
            }

            public static implicit operator SourceElement(FlattenedRuleExpression expression)
            {
                return new SourceElement(expression);
            }

            public static implicit operator SourceElement(FlattenedRuleEntry entry)
            {
                return new SourceElement(entry);
            }

            public static implicit operator SourceElement(FlattenedRuleExpressionSeries series)
            {
                return new SourceElement(series);
            }

            public static implicit operator SourceElement(FlattenedRuleGroupItem group)
            {
                return new SourceElement(group);
            }

            public static explicit operator FlattenedRuleGroupItem(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.RuleGroup)
                    throw new ArgumentException("element");
                return element.sourceGroup;
            }

            public static explicit operator FlattenedRuleExpression(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.RuleExpression)
                    throw new ArgumentException("element");
                return element.sourceExpression;
            }

            public static explicit operator FlattenedRuleExpressionSeries(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.RuleSeries)
                    throw new ArgumentException("element");
                return element.sourceSeries;
            }

            /// <summary>
            /// Explicitly converts the <paramref name="element"/> to a
            /// <see cref="FlattenedRuleEntry"/>.
            /// </summary>
            /// <param name="element">The <see cref="SourceElement"/> to convert.</param>
            /// <returns>The <see cref="FlattenedRuleEntry"/> instance that the
            /// <see cref="SourceElement"/> was created with.</returns>
            public static explicit operator FlattenedRuleEntry(SourceElement element)
            {
                if (element == null)
                    throw new ArgumentNullException("element");
                if (element.elementType != SourceElementType.RuleEntry)
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
                    case SourceElementType.RuleEntry:
                        return left.sourceEntry == right.sourceEntry;
                    case SourceElementType.RuleSeries:
                        return left.sourceSeries == right.sourceSeries;
                    case SourceElementType.RuleExpression:
                        return left.sourceExpression == right.sourceExpression;
                    case SourceElementType.RuleGroup:
                        return left.sourceGroup == right.sourceGroup;
                    case SourceElementType.RuleItem:
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
                    case SourceElementType.RuleEntry:
                        return left.sourceEntry != right.sourceEntry;
                    case SourceElementType.RuleSeries:
                        return left.sourceSeries != right.sourceSeries;
                    case SourceElementType.RuleExpression:
                        return left.sourceExpression != right.sourceExpression;
                    case SourceElementType.RuleGroup:
                        return left.sourceGroup != right.sourceGroup;
                    case SourceElementType.RuleItem:
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
                    case SourceElementType.RuleEntry:
                        return this.sourceEntry.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.RuleSeries:
                        return this.sourceSeries.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.RuleExpression:
                        return this.sourceExpression.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.RuleGroup:
                        return this.sourceGroup.GetHashCode() ^ elementTypeHash;
                    case SourceElementType.RuleItem:
                        return this.sourceItem.GetHashCode() ^ elementTypeHash;
                    default:
                        throw new InvalidOperationException("invalid state");
                }
            }
        }
    }
}
