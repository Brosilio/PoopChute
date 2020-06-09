using System;
using System.Collections.Generic;
using System.Text;

namespace PoopChuteLib
{
    public static class ShittyExtensions
    {
        /// <summary>
        /// Is this <see cref="int"/> within the specified range? Inclusive minimum, exclusive maximum.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="min">The inclusive lower bound.</param>
        /// <param name="max">The exclusive upper bound.</param>
        /// <returns></returns>
        public static bool IsInRange(this int i, int min, int max)
        {
            return i >= min && i < max;
        }
    }
}
