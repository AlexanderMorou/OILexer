using System;
using System.Collections.Generic;
using System.Text;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// The kind of special item represented.
    /// </summary>
    public enum SpecialItemType
    {
        /// <summary>
        /// Represents the start of the file.
        /// </summary>
        /// <remarks>Actual value: ~</remarks>
        BeginningOfFile,
        /// <summary>
        /// Represents the beginning of a line.
        /// </summary>
        /// <remarks>Actual value: $</remarks>
        BeginningOfLine,
        /// <summary>
        /// Represents the end of a line.
        /// </summary>
        /// <remarks>Actual value: ^</remarks>
        EndOfLine,
        /// <summary>
        /// Represents the end of the file.
        /// </summary>
        /// <remarks>Actual Value %</remarks>
        EndOfFile,
    }
    public interface IProductionRuleSpecialItem :
        IProductionRuleItem
    {
        SpecialItemType Type { get; }

        /// <summary>
        /// Creates a copy of the current <see cref="IProductionRuleSpecialItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IProductionRuleSpecialItem"/> with the data
        /// members of the current <see cref="IProductionRuleSpecialItem"/>.</returns>
        new IProductionRuleSpecialItem Clone();
    }
}
