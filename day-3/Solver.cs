namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var sum = 0;

        foreach (var line in input)
        {
            var firstLargestNumber = 0;
            var firstNumberIndex = 0;
            var secondLargestNumber = 0;

            for (var i = 0; i < line.Length - 1; i++)
            {
                // Hack to convert char to int
                int value = line[i] - '0';
                if (value > firstLargestNumber)
                {
                    firstLargestNumber = value;
                    firstNumberIndex = i;
                }
            }

            for (var i = firstNumberIndex + 1; i < line.Length; i++)
            {
                // Hack to convert char to int
                int value = line[i] - '0';
                if (value > secondLargestNumber)
                {
                    secondLargestNumber = value;
                }

            }

            // Console.WriteLine($"Largest values {firstLargestNumber} {secondLargestNumber}");
            sum += firstLargestNumber * 10 + secondLargestNumber;
        }

        return sum;
    }

    public long Solve2(List<string> input)
    {
        var count = 12;
        long sum = 0;

        foreach (var line in input)
        {
            long completeNumber = 0;
            var startIndex = 0;
            for (var batteryIndex = 0; batteryIndex < count; batteryIndex++)
            {
                long largest = 0;
                var largestIndex = 0;
                for (var i = startIndex; i < line.Length - (count - batteryIndex - 1); i++)
                {
                    int value = line[i] - '0';
                    if (value > largest)
                    {
                        largest = value;
                        largestIndex = i;
                    }
                }

                // Console.WriteLine($"current largest {largest} index {largestIndex} line {line}");
                startIndex = largestIndex + 1;
                completeNumber += largest * (long)Math.Pow(10, (count - batteryIndex - 1));
            }

            sum += completeNumber;
            // Console.WriteLine($"Largest number: {completeNumber}");
        }

        return sum;
    }
}
