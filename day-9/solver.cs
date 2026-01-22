namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        List<Vector2> corners = Parse(input);

        long largestArea = 0;
        var indexA = 0;
        var indexB = 0;
        for (var i = 0; i < corners.Count - 1; i++)
        {
            for (var j = i + 1; j < corners.Count; j++)
            {
                var area = RectangleArea(corners[i], corners[j]);
                if (area > largestArea)
                {
                    largestArea = area;
                    indexA = i;
                    indexB = j;
                }
            }
        }

        // Console.WriteLine($"{corners[indexA]} - {corners[indexB]}");
        var result = RectangleArea(corners[indexA], corners[indexB]);

        return result;
    }

    private List<Vector2> Parse(List<string> input)
    {
        List<Vector2> result = [];
        foreach (var line in input)
        {
            var parts = line.Split(",");
            result.Add(new Vector2(long.Parse(parts[0]), long.Parse(parts[1])));
        }

        return result;
    }

    private long RectangleArea(Vector2 a, Vector2 b)
    {
        var x = Math.Abs(a.X - b.X);
        var y = Math.Abs(a.Y - b.Y);
        return (x + 1) * (y + 1);
    }
}

public record Vector2(long X, long Y)
{
    public override string ToString() => $"{X},{Y}";
}
