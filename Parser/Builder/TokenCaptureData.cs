using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.Builder
{
    internal class TokenCaptureData
    {
        private CaptureStateMachineKind captureKind = CaptureStateMachineKind.Undecided;
        /// <summary>
        /// Returns the <see cref="CaptureStateMachineKind"/> of the current 
        /// capturing token.
        /// </summary>
        public CaptureStateMachineKind CaptureKind
        {
            get
            {
                return this.captureKind;
            }
        }
    }

    /// <summary>
    /// The kind of state-machine utilized by the capture.
    /// </summary>
    internal enum CaptureStateMachineKind
    {
        /// <summary>
        /// The capture's state machine kind isn't decided yet.
        /// </summary>
        Undecided,
        /// <summary>
        /// The capture contains named captures, and cannot be 
        /// reduced maximally, but can be reduced on the 
        /// left-hand side.
        /// </summary>
        Transducer,
        /// <summary>
        /// The capture doesn't contain any captureable elements
        /// and can be reduced on the left-hand and right-hand
        /// side.
        /// </summary>
        Recognizer,
    }
}
