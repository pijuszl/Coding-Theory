using GolayCode;
using System.Collections;

string? input;
int option;
double probability;

while(true)
{
    Console.WriteLine("Golay Code program");
    Console.WriteLine();
    Console.WriteLine("Coding scenarios:");
    Console.WriteLine("1. Vector:");
    Console.WriteLine("2. Text");
    Console.WriteLine("3. BMP Image");
    Console.WriteLine("Choose scenario by typing a number [1-3]:");

    while (true)
    {
        input = Console.ReadLine();

        try
        {
            option = int.Parse(input);

            if (option >= 1 && option <= 3)
            {
                break;
            }

            Console.WriteLine("Incorrect option. Try again.");
        }
        catch (FormatException)
        {
            Console.WriteLine("Incorrect option. Try again.");
        }
    }

    Console.Clear();
    Console.WriteLine("Golay Code program");

    if (option == 1)
    {
        Console.WriteLine("Coding vector");
    }
    else if (option == 2)
    {
        Console.WriteLine("Coding text");
    }
    else if (option == 3)
    {
        Console.WriteLine("Coding BMP image");
    }

    Console.WriteLine();
    Console.WriteLine("Write a failure probality from 0 to 1:");

    while (true)
    {
        input = Console.ReadLine();

        try
        {
            probability = double.Parse(input);

            if (probability > 0 && probability < 1)
            {
                break;
            }

            Console.WriteLine("Failure probability must be from 0 to 1 (e.g 0,005) . Try again.");
        }
        catch (FormatException)
        {
            Console.WriteLine("Failure probability must be from 0 to 1 (e.g 0,005). Try again.");
        }
    }

    if (option == 1)
    {
        BitArray vector = new(12);

        Console.WriteLine("Write a 12 length vector [from 0 and 1] to code:");

        while (true)
        {
            input = Console.ReadLine();

            if (input != null && input.Length == 12)
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

        Console.Clear();
        Console.WriteLine("Golay Code program");
        Console.WriteLine("Starting coding...");

        Golay codingService = new(probability);
        codingService.CodingVector(vector);

        Console.ReadLine();
        Console.Clear();
    }
    else if (option == 2)
    {
        Console.WriteLine("Write a not empty string to code:");

        while (true)
        {
            input = Console.ReadLine();

            if (input != null && input.Length > 0)
            {
                break;
            }

            Console.WriteLine("String is empty. Try again.");
        }

        Console.Clear();
        Console.WriteLine("Golay Code program");
        Console.WriteLine("Starting coding...");

        Golay codingService = new(probability);
        codingService.CodingString(input);

        Console.ReadLine();
        Console.Clear();
    }
    else if (option == 3)
    {
        Console.WriteLine("Write a full path to BMP image to code:");

        while (true)
        {
            input = Console.ReadLine();

            if (input != null && input.Length > 0)
            {
                break;
            }

            Console.WriteLine("String is empty. Try again.");
        }

        Console.Clear();
        Console.WriteLine("Golay Code program");
        Console.WriteLine("Starting coding...");

        Golay codingService = new(probability);

        try
        {
            codingService.CodingBmpImage(input);
        }
        catch
        {
            Console.WriteLine("File not founded or error while processing it");
        }

        Console.ReadLine();
        Console.Clear();
    }
}