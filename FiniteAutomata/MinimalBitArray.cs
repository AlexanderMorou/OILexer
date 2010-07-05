using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Oilexer.FiniteAutomata
{
    public class MinimalBitArray :
        FiniteAutomataBitSet<MinimalBitArray>
    {
        public MinimalBitArray()
        {
        }
        public MinimalBitArray(BitArray target)
            : base()
        {
            if (target == null)
                throw new ArgumentNullException("target");

#if x86
            int[] values = new int[(int)Math.Ceiling(((double)(target.Length)) / ((double)(SlotBitCount)))];
            target.CopyTo(values, 0);
            uint[] values2 = new uint[values.Length];
            for (int i = 0; i < values2.Length; i++)
                values2[i] = unchecked((uint)values[i]);
            base.Set(values2, 0, (uint)target.Length, (uint)target.Length);
#elif x64
            int[] values = new int[(int)Math.Ceiling(((double)(target.Length)) / 32D)];
            target.CopyTo(values, 0);
            ulong[] values2 = new ulong[(int)Math.Ceiling(((double)(values.Length)) / 2)];
            for (int i = 0; i < values2.Length; i++)
                if ((i * 2 + 1) >= values.Length)
                    values2[i] = ((uint)(values[i * 2]));
                else
                    values2[i] = ((uint)(values[i * 2])) | (((ulong)values[(i * 2) + 1]) << 32);
            base.Set(values2, 0, (uint)target.Length, (uint)target.Length);
#endif
        }

        public static implicit operator BitArray(MinimalBitArray source)
        {
            BitArray result = new BitArray((int)source.FullLength);
            for (uint i = source.IsNegativeSet ? 0 : source.Offset; i < (source.IsNegativeSet ? source.FullLength : source.Offset + source.Length); i++)
            {
                if (source[i])
                    result[(int)i] = true;
            }
            return result;
        }

    }
}
