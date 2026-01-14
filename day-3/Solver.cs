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
}
