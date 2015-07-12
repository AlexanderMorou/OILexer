using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface ICaptureTokenStructuralItem :
        ITokenSource
    {
        ///// <summary>Returns the <see cref="Int32"/> value which denotes the
        ///// frequency at which the element is matched.
        ///// </summary>
        ///// <remarks>If matched exactly once, the rank is zero.</remarks>
        //int Rank { get; set; }
        /// <summary>
        /// Returns the <see cref="ITokenSource"/> set which is responsible
        /// for the structural item within the capture.
        /// </summary>
        IControlledCollection<ITokenSource> Sources { get; }
        /// <summary>
        /// Returns/sets the <see cref="ResultedDataType"/> for a given
        /// capture token structural item.
        /// </summary>
        ResultedDataType ResultType { get; set; }

        bool GroupOptional { get; set; }

        ICaptureTokenStructuralItem Union(ICaptureTokenStructuralItem rightElement);

        /// <summary>
        /// Returns the name of the <see cref="ICaptureTokenStructuralItem"/>.
        /// </summary>
        string Name { get; }
        string BucketName { get; }
        int? BucketIndex { get; set; }
        IIntermediateFieldMember AssociatedField { get; set; }

        bool Optional { get; set; }

        int StateIndex { get; set; }
    }
}
