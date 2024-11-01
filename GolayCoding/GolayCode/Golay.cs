using System.Collections;
using System.Drawing;
using System.Text;

namespace GolayCode
{
    /// <summary>
    /// Represents a Golay code encoder and decoder. This class includes methods for encoding and decoding vectors, strings, and bitmap images using Golay codes.
    /// </summary>
    public class Golay
    {
        // Matrices used in the Golay encoding and decoding process

        private readonly int[,] _I = new int[,] // Identity matrix
        {
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        };

        private readonly int[,] _B = new int[,] // Specific matrix required for Golay code
        {
            { 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1 },
            { 1, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1 },
            { 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1 },
            { 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1 },
            { 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1 },
            { 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1 },
            { 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1 },
            { 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1 },
            { 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1 },
            { 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1 },
            { 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
        };

        private readonly int[,] _G; // Generator matrix, derived from _I and _B
        private readonly int[,] _H; // Parity-check matrix, derived from _I and _B

        // Failure probability used in simulating a noisy channel
        private readonly double _probability;

        // Services for encoding, decoding, and simulating a noisy channel
        private readonly EncodingService _encodingService;
        private readonly DecodingService _decodingService;
        private readonly ChannelService _channelService;

        /// <summary>
        /// Initializes a new instance of the Golay class with a specified failure probability for the simulated channel.
        /// </summary>
        /// <param name="failureProbability">The probability of a bit error in the simulated noisy channel.</param>
        public Golay(double failureProbability)
        {
            _probability = failureProbability;
            _G = InitialiseMatrixG();
            _H = InitialiseMatrixH();

            _encodingService = new EncodingService(_G);
            _decodingService = new DecodingService(_H, _B);
            _channelService = new ChannelService(_probability);
        }

        /// <summary>
        /// Initializes the generator matrix G used in Golay encoding. G is constructed by concatenating the identity matrix I and matrix B horizontally.
        /// </summary>
        private int[,] InitialiseMatrixG()
        {
            int[,] matrix = new int[12, 23];

            for (int row = 0; row < 12; row++)
            {
                // Copying the identity matrix _I to the first 12 columns of matrix G
                for (int collumn = 0; collumn < 12; collumn++)
                {
                    matrix[row, collumn] = _I[row, collumn];
                }

                // Copying the matrix _B to the remaining 11 columns of matrix G
                for (int collumn = 0; collumn < 11; collumn++)
                {
                    matrix[row, collumn + 12] = _B[row, collumn];
                }
            }

            return matrix;
        }

        /// <summary>
        /// Initializes the parity-check matrix H used in Golay decoding. H is constructed by concatenating the identity matrix I and matrix B vertically.
        /// </summary>
        private int[,] InitialiseMatrixH()
        {
            int[,] matrix = new int[24, 12];

            for (int row = 0; row < 24; row++)
            {
                // Concatenating the identity matrix _I and matrix _B vertically to form matrix H
                for (int collumn = 0; collumn < 12; collumn++)
                {
                    if (row < 12)
                    {
                        matrix[row, collumn] = _I[row, collumn];
                    }
                    else
                    {
                        matrix[row, collumn] = _B[row - 12, collumn];
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Encodes and decodes a given BitArray vector using Golay coding, simulates transmission through a noisy channel, 
        /// and allows for optional manual alteration of the transmitted vector.
        /// </summary>
        /// <param name="vector">The BitArray vector to be encoded and decoded.</param>
        public void CodingVector(BitArray vector)
        {
            // Display the original vector
            Console.WriteLine("Vector is: " + vector.VectorToString());
            Console.WriteLine("Failure probability is: " + _probability.ToString());
            Console.WriteLine();

            // Encoding the vector
            vector = _encodingService.Encode(vector);
            Console.WriteLine("Encoded vector is: " + vector.VectorToString());

            // Simulating the transmission through a noisy channel
            BitArray noisyVector = _channelService.NoisyChannel(vector);

            // Counting the number of errors introduced during transmission
            int errors = 0;
            for (int i = 0; i < noisyVector.Length; i++)
            {
                if (noisyVector[i] != vector[i])
                {
                    errors++;
                }
            }

            // Displaying the noisy vector with errors highlighted in red
            Console.Write($"After sending through noisy channel ({errors} errors in red): ");
            for (int i = 0; i < noisyVector.Length; i++)
            {
                if (noisyVector[i] != vector[i])
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(noisyVector[i] ? '1' : '0');
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(noisyVector[i] ? '1' : '0'); // Print the same bit
                }
            }
            Console.WriteLine();

            // Allowing user to modify the vector before decoding
            vector = new BitArray(noisyVector);
            Console.Write("Do you want to change vector before decoding it? [y/n] ");
            while (true)
            {
                string? input = Console.ReadLine();

                if (input != null)
                {
                    if (string.Equals(input, "y"))
                    {
                        Console.Write("Write a new 23 length vector: ");
                        while (true)
                        {
                            input = Console.ReadLine();

                            if (input != null && input.Length == 23)
                            {
                                bool correct = true;

                                for (int i = 0; i < input.Length; i++)
                                {
                                    if (input[i] != '1' && input[i] != '0')
                                    {
                                        correct = false;
                                    }

                                }

                                if (correct)
                                {
                                    for (int i = 0; i < input.Length; i++)
                                    {
                                        vector[i] = input[i] == '1';
                                    }

                                    break;
                                }
                            }

                            Console.WriteLine("Incorrect vector. Try again.");
                        }

                        break;
                    }
                    else if (string.Equals(input, "n"))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect answer. Try again.");
                    }
                }
                else { Console.WriteLine("Incorrect answer. Try again."); }
            }

            // Decoding the potentially modified or same vector
            vector = _decodingService.Decode(vector);
            Console.WriteLine("Decoded vector is: " + vector.VectorToString());
        }

        /// <summary>
        /// Encodes and decodes a given string using Golay coding and simulates transmission through a noisy channel.
        /// </summary>
        /// <param name="input">The string to be encoded and decoded.</param>
        public void CodingString(string input)
        {
            // Display the original string and the failure probability
            Console.WriteLine("String is: " + input);
            Console.WriteLine("Failure probability is: " + _probability.ToString());
            Console.WriteLine();

            // Convert the string to bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Process the string without using Golay coding
            string withoutCodingResult = ProcessStringWithoutCoding(inputBytes);

            // Process the string with Golay coding
            string withCodingResult = ProcessStringWithCoding(inputBytes);

            Console.WriteLine("After sending string through noisy channel:");
            Console.WriteLine("String without coding:" + withoutCodingResult);
            Console.WriteLine("String with Golay coding: " + withCodingResult);
        }

        private string ProcessStringWithoutCoding(byte[] inputBytes)
        {
            List<byte> processedBytes = [];

            // Loop through each byte in the input
            foreach (byte inputByte in inputBytes)
            {
                // Convert the byte to a BitArray
                BitArray bits = new(new byte[] { inputByte });

                // Simulate the transmission of the BitArray through a noisy channel
                BitArray noisyBits = _channelService.NoisyChannel(bits);

                // Convert the noisy BitArray back to a byte
                byte[] noisyByte = new byte[1];
                noisyBits.CopyTo(noisyByte, 0);
                processedBytes.Add(noisyByte[0]);
            }

            // Convert the processed bytes back to a string and return it
            return Encoding.UTF8.GetString(processedBytes.ToArray());
        }

        private string ProcessStringWithCoding(byte[] inputBytes)
        {
            List<byte> processedBytes = [];

            // Loop through each byte in the input
            foreach (byte inputByte in inputBytes)
            {
                // Convert the byte to a 12-bit BitArray
                BitArray bits = inputByte.ByteToBitArray();

                // Encode, simulate transmission, and decode the BitArray
                BitArray processedBits = CodingBitArray(bits);

                // Truncate the processed BitArray to the first 8 bits
                BitArray truncatedBits = new(8);
                for (int i = 0; i < 8; i++)
                {
                    truncatedBits[i] = processedBits[i];
                }

                // Convert the truncated BitArray back to a byte
                byte processedByte = truncatedBits.BitArrayToByte();
                processedBytes.Add(processedByte);
            }

            // Convert the processed bytes back to a string and return it
            return Encoding.UTF8.GetString(processedBytes.ToArray());
        }

        /// <summary>
        /// Encodes and decodes the pixel data of a BMP image file using Golay coding and simulates transmission through a noisy channel.
        /// </summary>
        /// <param name="filePath">The file path of the BMP image to be processed.</param>
        public void CodingBmpImage(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            // Read all bytes from the BMP image file
            byte[] bmpBytes = File.ReadAllBytes(filePath);

            // Extract the header and pixel data from the BMP file
            int headerSize = 54; // BMP Header is 54 bytes
            byte[] header = bmpBytes.Take(headerSize).ToArray();
            byte[] pixelData = bmpBytes.Skip(headerSize).ToArray();

            // Process the pixel data without Golay coding
            Console.WriteLine("Processing image without coding...");
            byte[] processedPixelsWithoutCoding = ProcessImageWithoutCoding(pixelData);

            Console.WriteLine("Processing image with coding...");
            byte[] processedPixelsWithCoding = ProcessImageWithCoding(pixelData);

            // Reassemble and save the BMP files with and without Golay coding
            byte[] reassembledBmpWithoutCoding = [.. header, .. processedPixelsWithoutCoding];
            byte[] reassembledBmpWithCoding = [.. header, .. processedPixelsWithCoding];

            File.WriteAllBytes("image_without_coding.bmp", reassembledBmpWithoutCoding);
            File.WriteAllBytes("image_with_coding.bmp", reassembledBmpWithCoding);

            Console.WriteLine("New files are created");
        }

        private byte[] ProcessImageWithoutCoding(byte[] pixelData)
        {
            return pixelData.SelectMany(b =>
            {
                // Convert each byte of pixel data to a BitArray
                BitArray bitArray = new(new byte[] { b });

                // Simulate the transmission of the BitArray through a noisy channel
                BitArray noisyBits = _channelService.NoisyChannel(bitArray);

                // Convert the noisy BitArray back to a byte and return it
                byte[] noisyByte = new byte[1];
                noisyBits.CopyTo(noisyByte, 0);
                return noisyByte;
            }).ToArray();
        }

        private byte[] ProcessImageWithCoding(byte[] pixelData)
        {
            return pixelData.SelectMany(b =>
            {
                // Convert each byte of pixel data to a 12-bit BitArray
                BitArray bits = b.ByteToBitArray();

                // Encode, simulate transmission, and decode the BitArray
                BitArray processedBits = CodingBitArray(bits);

                // Truncate the processed BitArray to the first 8 bits
                BitArray truncatedBits = new(8);
                for (int i = 0; i < 8; i++)
                {
                    truncatedBits[i] = processedBits[i];
                }

                // Convert the truncated BitArray back to a byte and return it
                byte processedByte = truncatedBits.BitArrayToByte();
                return new[] { processedByte };
            }).ToArray();
        }


        private BitArray CodingBitArray(BitArray bits)
        {
            // Check if the BitArray has the correct length
            if (bits.Length != 12)
            {
                throw new GolayException("BitArray must be a length of 12");
            }

            // Encode the BitArray
            BitArray encodedBits = _encodingService.Encode(bits);

            // Simulate the transmission of the encoded BitArray through a noisy channel
            BitArray noisyBits = _channelService.NoisyChannel(encodedBits);

            // Decode the noisy BitArray and return it
            return _decodingService.Decode(noisyBits);
        }
    }
}