namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var (ranges, values) = ParseValues(input);

        // Naive solution
        long sum = 0;
        foreach (var value in values)
        {
            foreach (var range in ranges)
            {
                if (value >= range.Start && value <= range.End)
                {
                    sum += 1;
                    break;
                }
            }
        }

        return sum;
    }

    private (List<Range> ranges, List<long> values) ParseValues(List<string> input)
    {
        List<Range> ranges = [];
        List<long> values = [];

        var rangesEnded = false;
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                rangesEnded = true;
                continue;
            }
            if (!rangesEnded)
            {
                var parts = line.Split("-");
                ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
            }
            else
            {
                values.Add(long.Parse(line));
            }
        }

        return (ranges, values);
    }
}

public record Range(long Start, long End);
