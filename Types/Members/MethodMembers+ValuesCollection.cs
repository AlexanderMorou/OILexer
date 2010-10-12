using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
            private Members<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>, IMemberParentType, CodeMemberMethod>.ValuesCollection baseList;
            /// <summary>
            /// Creates a new instance of the values collection.
            /// </summary>
            /// <param name="baseList">The base collection which contains the original typed entries.</param>
            internal ValuesCollection(Members<IMethodSignatureMember<IMethodParameterMember, IMethodTypeParameterMember, CodeMemberMethod, IMemberParentType>, IMemberParentType, CodeMemberMethod>.ValuesCollection baseList)
            {
                this.baseList = baseList;
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
                return baseList.Contains(item);
            }

            /// <summary>
            /// Copies the entries contained within the <see cref="ValuesCollection"/> to
            /// a <see cref="System.Array"/> of <see cref="IMethodMember"/>.
            /// </summary>
            /// <param name="array">The array to copy the instances to.</param>
            /// <param name="arrayIndex">The point at which to start the copy.</param>
            public void CopyTo(IMethodMember[] array, int arrayIndex)
            {
                IMethodMember[] resultItems = this.baseList.Cast<IMethodMember>().ToArray();
                resultItems.CopyTo(array, arrayIndex);
                resultItems = null;
            }

            /// <summary>
            /// Returns the number of <see cref="IMethodMember"/> instances there are in the
            /// <see cref="ValuesCollection"/>.
            /// </summary>
            public int Count
            {
                get { return this.baseList.Count; }
            }

            /// <summary>
            /// Flattens the <see cref="ValuesCollection"/> into a standard <see cref="System.Array"/>
            /// of <see cref="IMethodMember"/> instance implementations.
            /// </summary>
            /// <returns>A series of <see cref="IMethodMember"/> instance implementations as a <see cref="System.Array"/>.</returns>
            public IMethodMember[] ToArray()
            {
                return baseList.Cast<IMethodMember>().ToArray();
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
                    return this.baseList[index] as IMethodMember;
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
                foreach (IMethodMember imm in this.baseList)
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
                return this.baseList.IndexOf(element);
            }

            #endregion
        }
    }
}
