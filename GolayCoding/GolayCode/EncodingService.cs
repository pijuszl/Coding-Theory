using System.Collections;

namespace GolayCode
{
    /// <summary>
    /// Provides functionality to encode a BitArray using the Golay encoding scheme.
    /// </summary>
    public class EncodingService
    {
        private readonly int[,] _G;

        /// <summary>
        /// Initializes a new instance of the EncodingService with a specified generator matrix.
        /// </summary>
        /// <param name="G">The generator matrix used for encoding.</param>
        public EncodingService(int[,] G)
        {
            _G = G;
        }

        /// <summary>
        /// Encodes a given BitArray using the Golay encoding scheme.
        /// </summary>
        /// <param name="vector">The BitArray to encode. Must be of length 12.</param>
        /// <returns>The encoded BitArray of length 23.</returns>
        /// <exception cref="GolayException">Thrown when the input vector is not of length 12.</exception>
        public BitArray Encode(BitArray vector)
        {
            // Validate input vector length
            if (vector == null || vector.Length != 12)
            {
                throw new GolayException("Vector must be a length of 12");
            }

            // Create an empty BitArray for the encoded vector
            BitArray encodedVector = new(23);

            // Multiply vector with generator matrix G (optimized for binary operations)
            for (int i = 0; i < 23; i++)
            {
                bool bitValue = false;
                for (int j = 0; j < 12; j++)
                {
                    // Perform XOR operation equivalent for binary matrix multiplication
                    if (vector[j] && _G[j, i] == 1)
                    {
                        bitValue = !bitValue;
                    }
                }
                encodedVector[i] = bitValue;
            }

            return encodedVector;
        }

    }
}
