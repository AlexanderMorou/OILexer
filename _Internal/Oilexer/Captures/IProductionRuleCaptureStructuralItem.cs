using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface IProductionRuleCaptureStructuralItem :
        IProductionRuleSource
    {
        /// <summary>
        /// Returns the <see cref="ITokenSource"/> set which is responsible
        /// for the structural item within the capture.
        /// </summary>
        IControlledCollection<IProductionRuleSource> Sources { get; }
        /// <summary>
        /// Returns/sets the <see cref="ResultedDataType"/> for a given
        /// capture token structural item.
        /// </summary>
        ResultedDataType ResultType { get; set; }

        IProductionRuleCaptureStructuralItem Union(IProductionRuleCaptureStructuralItem rightElement);

        /// <summary>
        /// Returns the name of the <see cref="IProductionRuleCaptureStructuralItem"/>.
        /// </summary>
        string Name { get; }
        string BucketName { get; }
        int? BucketIndex { get; set; }
        IIntermediateFieldMember AssociatedField { get; set; }

        bool Optional { get; set; }
        bool GroupOptional { get; set; }

        int StateIndex { get; set; }
    }
}
