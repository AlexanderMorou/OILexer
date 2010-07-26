using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Utilities.Arrays;
using System.Collections;

namespace Oilexer.Types.Members
{
	partial class MethodMembers
	{
        new public class ValuesCollection :
            IReadOnlyCollection<IMethodMember>
        {
            /// <summary>
            /// Private reference of the <see cref="ValuesCollection"/> data source.
            /// </summary>
            private ICollection<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>> baseCollection;
            /// <summary>
            /// Creates a new instance of the values collection.
            /// </summary>
            /// <param name="baseCollection">The base collection which contains the original typed entries.</param>
            internal ValuesCollection(ICollection<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>> baseCollection)
            {
                this.baseCollection = baseCollection;
            }

            #region IReadOnlyCollection<IMethodMember> Members

            /// <summary>
            /// Returns whether or not the <see cref="ValuesCollection"/> contains the <paramref name="item"/>
            /// provided.
            /// </summary>
            /// <param name="item">The <see cref="IMethodMember"/> to look for.</param>
            /// <returns>True if the <paramref name="item"/> exists within the <see cref="ValuesCollection"/>.</returns>
            public bool Contains(IMethodMember item)
            {
                return baseCollection.Contains(item);
            }

            /// <summary>
            /// Copies the entries contained within the <see cref="ValuesCollection"/> to
            /// a <see cref="System.Array"/> of <see cref="IMethodMember"/>.
            /// </summary>
            /// <param name="array">The array to copy the instances to.</param>
            /// <param name="arrayIndex">The point at which to start the copy.</param>
            public void CopyTo(IMethodMember[] array, int arrayIndex)
            {
                IMethodMember[] resultItems = Tweaks.CastArray<IMethodMember, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>>(Special.GetCollectionItems(this.baseCollection));
                resultItems.CopyTo(array, arrayIndex);
                resultItems = null;
            }

            /// <summary>
            /// Returns the number of <see cref="IMethodMember"/> instances there are in the
            /// <see cref="ValuesCollection"/>.
            /// </summary>
            public int Count
            {
                get { return this.baseCollection.Count; }
            }

            /// <summary>
            /// Flattens the <see cref="ValuesCollection"/> into a standard <see cref="System.Array"/>
            /// of <see cref="IMethodMember"/> instance implementations.
            /// </summary>
            /// <returns>A series of <see cref="IMethodMember"/> instance implementations as a <see cref="System.Array"/>.</returns>
            public IMethodMember[] ToArray()
            {
                return Tweaks.CastArray<IMethodMember, IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>>(Special.GetCollectionItems(this.baseCollection));
            }

            /// <summary>
            /// Returns a member at the index provided.
            /// </summary>
            /// <param name="index">The index of the item to return.</param>
            /// <returns>The instance at the index provided.</returns>
            public IMethodMember this[int index]
            {
                get 
                {
                    if (index < 0)
                        throw new ArgumentOutOfRangeException("index", "below zero");
                    int i = 0;
                    foreach (IMethodMember imm in this.baseCollection)
                        if (i++ == index)
                            return imm;
                    throw new ArgumentOutOfRangeException("index", "beyond last item");
                }
            }

            #endregion

            #region IEnumerable<IMethodMember> Members

            /// <summary>
            /// Returns an enumerator that can iterate through the entries in the 
            /// <see cref="ValuesCollection"/>.
            /// </summary>
            /// <returns>A new <see cref="IEnumerator{T}"/> implementation which
            /// can iterate the <see cref="ValuesCollection"/>.</returns>
            public IEnumerator<IMethodMember> GetEnumerator()
            {
                foreach (IMethodMember imm in this.baseCollection)
                    yield return imm;
                yield break;
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region IControlledStateCollection<IMethodMember> Members


            public int IndexOf(IMethodMember element)
            {
                int index = -1;
                int i = 0;
                foreach (var currentElement in this.baseCollection)
                    if (currentElement.Equals(element))
                    {
                        index = i;
                        break;
                    }
                    else
                        i++;
                return index;
            }

            #endregion
        }
	}
}
