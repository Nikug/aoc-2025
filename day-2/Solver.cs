namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var ranges = ParseRanges(input[0]);

        long sum = 0;

        foreach (var range in ranges)
        {
            var startNumber = long.Parse(range.Start);
            var endNumber = long.Parse(range.End);

            for (var i = startNumber; i <= endNumber; i++)
            {
                var value = i.ToString().AsSpan();
                var length = value.Length;

                if (length % 2 != 0) continue;
                var half = length / 2;

                if (MemoryExtensions.SequenceEqual(value[0..half], value[half..]))
                {
                    sum += i;
                }

            }
        }

        return sum;
    }

    private List<Range> ParseRanges(string input)
    {
        List<Range> results = [];
        var ranges = input.Split(",");

        foreach (var range in ranges)
        {
            var values = range.Split("-");
            var newRange = new Range()
            {
                Start = values[0],
                End = values[1]
            };
            results.Add(newRange);
        }

        return results;
    }
}

public record Range
{
    public required string Start { get; set; }
    public required string End { get; set; }
}
