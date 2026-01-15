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

    public long Solve2(List<string> input)
    {
        var (ranges, _) = ParseValues(input);

        // Sort ascending by start
        ranges.Sort((Range a, Range b) =>
        {
            if (a.Start > b.Start) return 1;
            if (a.Start < b.Start) return -1;
            return 0;
        });

        for (var i = 0; i < ranges.Count - 1; i++)
        {
            var current = ranges[i];
            var next = ranges[i + 1];
            // Console.WriteLine($"Checking: {current} {next}");

            if (current.End >= next.Start)
            {
                ranges[i] = new Range(current.Start, Math.Max(current.End, next.End));
                ranges.RemoveAt(i + 1);
                // Check the new combined range against the new next in the list
                i -= 1;
            }
        }


        long sum = 0;
        foreach (var range in ranges)
        {
            // Console.WriteLine($"Range: {range}, size: {range.Count()}");
            sum += range.Count();
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

public record Range(long Start, long End)
{
    public override string ToString() => $"{this.Start}-{this.End}";
    public long Count() => this.End - this.Start + 1;
};
