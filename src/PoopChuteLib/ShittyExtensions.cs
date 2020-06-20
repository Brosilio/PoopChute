using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Try to read the specified amount of data (potentially using multiple reads) from this stream.
        /// <para>Returns true if the count read into the buffer is equal to the requested count.</para>
        /// <para>Returns false if the stream failed to read the requested byte count.</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="buffer">The buffer to write into.</param>
        /// <param name="offset">The offset in the buffer to start at.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns></returns>
        public static async Task<bool> TryFillBufferAsync(this Stream s, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            do
            {
                int r = await s.ReadAsync(buffer, offset, count - totalRead);
                if (r == 0)
                    return false;
                totalRead += r;
                offset += r;

            } while (totalRead < count);

            return true;
        }
    }
}
