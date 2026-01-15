namespace Puzzle;

public class Solver
{

    private Directions directions = new Directions();

    public long Solve(List<string> input)
    {
        var bounds = new Vector2(input[0].Length, input.Count);

        HashSet<Vector2> beams = [];
        HashSet<Vector2> splitters = [];
        for (var y = 0; y < bounds.Y; y++)
        {
            for (var x = 0; x < bounds.X; x++)
            {
                var character = input[y][x];
                var position = new Vector2(x, y);

                if (beams.Contains(position)) continue;

                var positionAbove = position + directions.Up;
                if (character == 'S')
                {
                    var positionBelow = position + directions.Down;
                    beams.Add(positionBelow);
                    break;
                }
                else if (character == '^' && beams.Contains(positionAbove))
                {
                    splitters.Add(position);
                    var positionLeft = position + directions.Left;
                    var positionRight = position + directions.Right;

                    if (!OutOfBounds(positionLeft, bounds))
                    {
                        beams.Add(positionLeft);
                    }

                    if (!OutOfBounds(positionRight, bounds))
                    {
                        beams.Add(positionRight);
                    }
                }
                else if (character == '.' && beams.Contains(positionAbove))
                {
                    beams.Add(position);
                }
            }
        }

        // Console.WriteLine("Beams: " + string.Join(", ", beams));
        // Console.WriteLine("Splitters: " + string.Join(", ", splitters));

        return splitters.Count;
    }

    public long Solve2(List<string> input)
    {
        var bounds = new Vector2(input[0].Length, input.Count);

        Dictionary<Vector2, long> beams = [];
        Dictionary<Vector2, long> splitters = [];
        for (var y = 0; y < bounds.Y; y++)
        {
            for (var x = 0; x < bounds.X; x++)
            {
                var character = input[y][x];
                var position = new Vector2(x, y);

                var positionAbove = position + directions.Up;
                if (character == 'S')
                {
                    var positionBelow = position + directions.Down;
                    beams.Add(positionBelow, 1);
                    break;
                }
                else if (character == '^' && beams.ContainsKey(positionAbove))
                {
                    var numberOfBeams = beams[positionAbove];
                    splitters.Add(position, numberOfBeams);
                    var positionLeft = position + directions.Left;
                    var positionRight = position + directions.Right;

                    if (beams.ContainsKey(positionLeft))
                    {
                        beams[positionLeft] += numberOfBeams;
                    }
                    else
                    {
                        beams.Add(positionLeft, numberOfBeams);
                    }

                    if (beams.ContainsKey(positionRight))
                    {
                        beams[positionRight] += numberOfBeams;
                    }
                    else
                    {
                        beams.Add(positionRight, numberOfBeams);
                    }
                }
                else if (character == '.' && beams.ContainsKey(positionAbove))
                {
                    if (beams.ContainsKey(position))
                    {
                        beams[position] += beams[positionAbove];
                    }
                    else
                    {
                        beams.Add(position, beams[positionAbove]);
                    }
                }
            }
        }

        // Console.WriteLine("Beams: " + string.Join(", ", beams));
        // Console.WriteLine("Splitters: " + string.Join(", ", splitters));
        // PrintBeams(input, beams);

        long sum = 0;
        var beamEnds = beams.Where(entry => entry.Key.Y == bounds.Y - 1);
        foreach (var (_, value) in beamEnds)
        {
            sum += value;
        }

        return sum;
    }

    private bool OutOfBounds(Vector2 position, Vector2 bounds)
    {
        return position.X < 0 || position.X >= bounds.X || position.Y < 0 || position.Y >= bounds.Y;
    }

    private void PrintBeams(List<string> input, Dictionary<Vector2, long> beams)
    {
        var bounds = new Vector2(input[0].Length, input.Count);

        for (var y = 0; y < bounds.Y; y++)
        {
            var line = string.Empty;
            for (var x = 0; x < bounds.X; x++)
            {
                var position = new Vector2(x, y);
                if (beams.ContainsKey(position))
                {
                    line += beams[position];
                }
                else
                {
                    line += input[y][x];
                }
            }
            Console.WriteLine(line);
        }
    }

}

public record Vector2(long X, long Y)
{
    public override string ToString() => $"{this.X},{this.Y}";

    public static Vector2 operator +(Vector2 left, Vector2 right) => new Vector2(left.X + right.X, left.Y + right.Y);
}

public struct Directions()
{
    public readonly Vector2 Up = new Vector2(0, -1);
    public readonly Vector2 Right = new Vector2(1, 0);
    public readonly Vector2 Down = new Vector2(0, 1);
    public readonly Vector2 Left = new Vector2(-1, 0);
}
