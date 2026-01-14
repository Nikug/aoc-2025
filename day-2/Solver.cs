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

    public long Solve2(List<string> input)
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

                // For 12345 half is 2, since 3 long sequence is too long to repeat
                // so integer division works
                var half = length / 2;

                for (var sequenceLength = 1; sequenceLength <= half; sequenceLength++)
                {
                    if (length % sequenceLength != 0) continue;

                    var sequence = value[0..sequenceLength];

                    var isInvalid = true;
                    for (var window = sequenceLength; window < length; window += sequenceLength)
                    {
                        if (!MemoryExtensions.SequenceEqual(sequence, value[window..(window + sequenceLength)]))
                        {
                            isInvalid = false;
                            break;
                        }
                    }

                    if (isInvalid)
                    {
                        // Console.WriteLine($"Invalid value {i}, invalid sequence {sequence}");
                        sum += i;
                        break;
                    }
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
