
namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>Outlines how a given destination was reached.</summary>
    public class PredictionTreeDestinationPoint
    {
        /// <summary>Returns/sets the <see cref="PredictionTreeBranch"/> which is the current element in the sequence.</summary>
        public PredictionTreeBranch CurrentPath { get; set; }
        /// <summary>Returns/sets the <see cref="PredictionTreeBranch"/> which leads out of the <see cref="CurrentPath"/> in the sequence.</summary><remarks>Null the <see cref="CurrentPath"/> terminates the decision tree.</remarks>
        public PredictionTreeBranch PreviousPath { get; set; }
    }
}
