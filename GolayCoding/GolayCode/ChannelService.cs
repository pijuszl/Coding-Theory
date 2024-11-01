using System.Collections;

namespace GolayCode
{
    /// <summary>
    /// Simulates a noisy communication channel and applies noise to a BitArray based on a specified probability.
    /// </summary>
    public class ChannelService
    {
        private readonly double _probability;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the ChannelService with a specified error probability.
        /// </summary>
        /// <param name="probability">The probability of a bit error occurring in the channel.</param>
        public ChannelService(double probability)
        {
            _probability = probability;
            _random = new Random();
        }

        /// <summary>
        /// Simulates transmission of a BitArray through a noisy channel, flipping bits based on the specified error probability.
        /// </summary>
        /// <param name="vector">The BitArray to be transmitted through the channel.</param>
        /// <returns>A new BitArray representing the input after passing through the noisy channel.</returns>
        public BitArray NoisyChannel(BitArray vector)
        {
            BitArray result = new(vector.Length);

            // Loop through each bit in the vector
            for (int i = 0; i < vector.Length; i++)
            {
                // Randomly flip the bit based on the specified probability
                if (_random.NextDouble() <= _probability)
                {
                    result[i] = !vector[i]; // Flip the bit
                }
                else
                {
                    result[i] = vector[i]; // Keep the bit unchanged
                }
            }

            return result;
        }
    }
}
