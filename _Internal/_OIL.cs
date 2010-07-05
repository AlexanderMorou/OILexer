using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using Oilexer.Utilities.Collections;
using System.Collections;
using System.CodeDom;
using System.Runtime.InteropServices;
using Oilexer;

#if x64
using SlotType = System.UInt64;
#elif x86
using SlotType = System.UInt32;
#endif


namespace Oilexer._Internal
{
    internal static partial class _OIL
    {
        public static string Repeat(this char c, int length)
        {
            char[] result = new char[length];
            for (int i = 0; i < result.Length; i++)
                result[i] = c;
            return new string(result);
        }

        internal unsafe static SlotType[] ObtainFiniteSeries(this BitArray characters, int FullSetLength)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");
            else if (characters.Length > FullSetLength + 1)
                throw new ArgumentOutOfRangeException("characters");

            int[] values = new int[(int)Math.Ceiling(((double)(characters.Length)) / 32D)];
            characters.CopyTo(values, 0);
#if x86
            uint[] values2 = new uint[values.Length];
            for (int i = 0; i < values2.Length; i++)
                values2[i] = unchecked((uint)values[i]);
#elif x64
            ulong[] values2 = new ulong[(int)Math.Ceiling(((double)(values.Length)) / 2)];
            fixed (SlotType* values2ptr = values2)
            {
                uint* v2p = (uint*)values2ptr;
                fixed (int* valuesPtr = values)
                {
                    uint* vp = (uint*)valuesPtr;
                    for (int i = 0; i < values.Length; i++)
                    {
                        *v2p = *vp;
                        v2p++;
                        vp++;
                    }
                }
            }
#endif
            return values2;
        }

    }
}
