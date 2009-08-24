using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    partial class StatementBlockLocalMember
    {
        [Serializable]
        public class ReferenceExpression :
            VariableReferenceExpression,
            ILocalReferenceExpression
        {
            internal StatementBlockLocalMember referencePoint;
            public ReferenceExpression(StatementBlockLocalMember referencePoint)
                : base(referencePoint.Name, null)
            {
                this.referencePoint = referencePoint;
            }
            public override string Name
            {
                get
                {
                    return this.referencePoint.Name;
                }
            }

            public override CodeVariableReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
            {
                if (options.NameHandler.HandlesName(this.referencePoint))
                    return new CodeVariableReferenceExpression(options.NameHandler.HandleName(this.referencePoint));
                else
                    return new CodeVariableReferenceExpression(this.Name);
            }

            /// <summary>
            /// Performs a look-up on the <see cref="StatementBlockLocalMember.ReferenceExpression"/> to determine its 
            /// dependencies.
            /// </summary>
            /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
            /// relates to the <see cref="ITypeReference"/> instance implementations
            /// that the <see cref="StatementBlockLocalMember.ReferenceExpression"/> relies on.</param>
            /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
            /// guide the gathering process.</param>
            public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
            {
                
            }
        }
    }
}