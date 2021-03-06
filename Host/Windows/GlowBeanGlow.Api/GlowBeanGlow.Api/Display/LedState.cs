﻿using System;

namespace GlowBeanGlow.Api.Display
{
    public class LedState
    {
        public int Length
        {
            get
            {
                // Number of LEDs in the hardware implementation.
                return 12;
            }
        }

        public ushort LedRawBits { get; set; }

        public void RotateClockwise()
        {
            var last = this[Length - 1];
            LedRawBits <<= 1;
            this[0] = last;
        }

        public void RotateCounterClockwise()
        {
            var first = this[0];
            LedRawBits >>= 1;
            this[Length - 1] = first;
        }

        public bool this[int index]
        {
            get
            {
                AssertValidLedIndex(index);
                return (LedRawBits & (1 << index)) > 0;
            }

            set
            {
                AssertValidLedIndex(index);
                if (value)
                {
                    // turn on the relevant bit
                    LedRawBits |= (ushort)(1 << index);
                }
                else
                {
                    // turn off the relevant bit
                    LedRawBits &= (ushort)~(1 << index);
                }
            }
        }

        private void AssertValidLedIndex(int index)
        {
            if (index > Length - 1)
            {
                throw new IndexOutOfRangeException(
                    string.Format("There are only {0} LEDs available.  You attempted to turn on LED {1}.", Length, index + 1));
            }
        }
    }
}
