using System.Collections;

namespace GolayCode
{
    /// <summary>
    /// Provides functionality to decode a BitArray using the Golay decoding scheme.
    /// </summary>
    public class DecodingService
    {
        private readonly int[,] _H;
        private readonly int[,] _B;

        /// <summary>
        /// Initializes a new instance of the DecodingService with specified parity-check and B matrices.
        /// </summary>
        /// <param name="H">The parity-check matrix used for decoding.</param>
        /// <param name="B">An additional matrix used in the decoding process.</param>
        public DecodingService(int[,] H, int[,] B)
        {
            _H = H;
            _B = B;
        }

        /// <summary>
        /// Decodes a given BitArray using the Golay decoding scheme.
        /// </summary>
        /// <param name="vector">The BitArray to decode. Must be of length 23.</param>
        /// <returns>The decoded BitArray of length 12.</returns>
        /// <exception cref="GolayException">Thrown when the input vector is not of length 23.</exception>
        public BitArray Decode(BitArray vector)
        {
            // Validate input vector length
            if (vector == null || vector.Length != 23)
            {
                throw new GolayException("Vector must be a length of 23");
            }

            // Extend the vector by one bit based on the weight of the vector
            bool lastBit = GetWeight(vector) % 2 == 0;
            BitArray extendedVector = new(24);
            CopyBitArray(vector, extendedVector, 0);
            extendedVector[23] = lastBit; // Append the calculated bit

            // Perform the decoding process on the extended vector
            extendedVector = DecodeExtended(extendedVector);

            // Extract the first 12 bits as the final decoded vector
            BitArray decodedVector = new(12);
            for (int i = 0; i < 12; i++)
            {
                decodedVector[i] = extendedVector[i];
            }

            return decodedVector;
        }

        private BitArray DecodeExtended(BitArray w)
        {
            // Compute the syndrome of the vector
            BitArray syndrome = ComputeSyndrome(w);

            // Handle cases where the syndrome weight is less than or equal to 3
            if (GetWeight(syndrome) <= 3)
            {
                BitArray u = new(24);
                CopyBitArray(syndrome, u, 0); // Copy syndrome to u
                return CodewordC(w, u); // Correct the codeword based on syndrome
            }
            else
            {
                // Process for finding a low-weight error pattern in matrix B
                for (int row = 0; row < 12; row++)
                {
                    BitArray u = new(24);

                    for (int i = 0; i < 12; i++)
                    {
                        u[i] = syndrome[i] != (_B[row, i] == 1); // XOR with _B matrix
                    }

                    if (GetWeight(u) <= 2)
                    {
                        u[row + 12] = true; // Mark error location
                        return CodewordC(w, u); // Correct the codeword
                    }
                }

                // Compute the second syndrome
                BitArray secondSyndrome = ComputeSecondSyndrome(syndrome);

                if (GetWeight(secondSyndrome) <= 3)
                {
                    BitArray u = new(24);
                    CopyBitArray(secondSyndrome, u, 12); // Copy second syndrome to the second part of u
                    return CodewordC(w, u); // Correct the codeword based on second syndrome
                }
                else
                {
                    // Further process for complex error patterns
                    for (int row = 0; row < 12; row++)
                    {
                        BitArray u = new(24);

                        for (int i = 0; i < 12; i++)
                        {
                            u[i + 12] = secondSyndrome[i] != (_B[row, i] == 1); // XOR with _B matrix
                        }

                        if (GetWeight(u) <= 2)
                        {
                            u[row] = true; // Mark error location
                            return CodewordC(w, u); // Correct the codeword
                        }
                    }
                }
            }

            // If no valid correction is found, indicate a failure to decode
            throw new GolayException("Failed to decode. Retransmission is required.");
        }

        private static int GetWeight(BitArray bitArray)
        {
            // Calculate the Hamming weight (number of 1s) of the bit array
            int weight = 0;
            foreach (bool bit in bitArray)
            {
                if (bit) weight++;
            }
            return weight;
        }

        private static void CopyBitArray(BitArray source, BitArray destination, int startIndex)
        {
            // Copy the contents of one BitArray to another, starting at a given index
            for (int i = 0; i < source.Length; i++)
            {
                destination[i + startIndex] = source[i];
            }
        }

        private BitArray ComputeSyndrome(BitArray vector)
        {
            // Validate the vector length
            if (vector == null || vector.Length != 24)
            {
                throw new GolayException("Vector must be a length of 24");
            }

            // Compute the syndrome of the vector using matrix H
            BitArray syndrome = new(12);
            for (int i = 0; i < 12; i++)
            {
                bool bitValue = false;
                for (int j = 0; j < 24; j++)
                {
                    // Perform XOR between bitValue and the logical AND of vector[j] and H[j, i]
                    if (vector[j] && _H[j, i] == 1)
                    {
                        bitValue = !bitValue;
                    }
                }
                syndrome[i] = bitValue;
            }

            return syndrome;
        }

        private BitArray ComputeSecondSyndrome(BitArray vector)
        {
            // Validate the vector length
            if (vector == null || vector.Length != 12)
            {
                throw new GolayException("Vector must be a length of 12");
            }

            // Compute the second syndrome using matrix B
            BitArray syndrome = new(12);
            for (int i = 0; i < 12; i++)
            {
                bool bitValue = false;
                for (int j = 0; j < 12; j++)
                {
                    // XOR operation for each bit
                    if (vector[j] && _B[j, i] == 1)
                    {
                        bitValue = !bitValue;
                    }
                }
                syndrome[i] = bitValue;
            }

            return syndrome; // result is a BitArray of length 12
        }

        private static BitArray CodewordC(BitArray w, BitArray u)
        {
            // Correct the codeword w based on the error pattern u
            BitArray v = new(24);
            for (int i = 0; i < 24; i++)
            {
                v[i] = w[i] != u[i]; // XOR operation
            }

            return v;
        }

    }
}
