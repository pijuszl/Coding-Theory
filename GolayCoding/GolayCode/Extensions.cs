using System.Collections;
using System.Text;
using System.Drawing;

namespace GolayCode
{
    /// <summary>
    /// Provides extension methods for converting between BitArrays and other data types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a BitArray to its string representation.
        /// </summary>
        /// <param name="vector">The BitArray to convert.</param>
        /// <returns>A string representation of the BitArray.</returns>
        public static string VectorToString(this BitArray vector)
        {
            StringBuilder sb = new();

            // Convert each bit in the BitArray to a character ('1' or '0')
            for (int i = 0; i < vector.Count; i++)
            {
                char c = vector[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a byte to a BitArray of length 12, padding with 0s.
        /// </summary>
        /// <param name="inputByte">The byte to convert.</param>
        /// <returns>A 12-bit BitArray representing the input byte.</returns>
        public static BitArray ByteToBitArray(this byte inputByte)
        {
            // Extend it to a 12-bit BitArray, filling the last 4 bits with 0s
            BitArray bitArray = new(new byte[] { inputByte });

            // Extend to a 12-bit BitArray
            BitArray extendedBitArray = new(12);
            for (int i = 0; i < 8; i++)
            {
                extendedBitArray[i] = bitArray[i];
            }

            return extendedBitArray;
        }

        /// <summary>
        /// Converts a BitArray to a single byte. Assumes the BitArray contains 8 or fewer bits.
        /// </summary>
        /// <param name="bits">The BitArray to convert. Must contain 8 or fewer bits.</param>
        /// <returns>A byte representing the given BitArray.</returns>
        /// <exception cref="GolayException">Thrown when the BitArray contains more than 8 bits.</exception>
        public static byte BitArrayToByte(this BitArray bits)
        {
            // Ensure the BitArray does not exceed the size of a byte
            if (bits.Count > 8)
            {
                throw new GolayException("BitArray contains more bits than can fit into a single byte.");
            }

            // Convert the BitArray back to a byte
            byte[] byteArray = new byte[1];
            bits.CopyTo(byteArray, 0);
            return byteArray[0];
        }
    }
}
