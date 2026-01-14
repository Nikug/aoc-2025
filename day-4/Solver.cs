namespace Puzzle;

public class Solver
{
    private Vector2[] directions = [
        new Vector2(0, -1), // Up
        new Vector2(1, -1), // Up Right
        new Vector2(1, 0), // Right
        new Vector2(1, 1), // Down Right
        new Vector2(0, 1), // Down
        new Vector2(-1, 1), // Down Left
        new Vector2(-1, 0), // Left
        new Vector2(-1, -1), // Up Left
    ];

    public long Solve(List<string> input)
    {
        var result = 0;
        var size = new Vector2(input[0].Length, input.Count);
        var maxSurroundingObjects = 3;

        for (var y = 0; y < size.Y; y++)
        {
            for (var x = 0; x < size.X; x++)
            {
                var position = new Vector2(x, y);
                var positionCharacter = input[position.Y][position.X];
                if (positionCharacter != '@')
                {
                    continue;
                }

                var surroundingObjects = 0;
                foreach (var direction in directions)
                {
                    var newPosition = position + direction;
                    if (outOfBounds(newPosition, size))
                    {
                        continue;
                    }

                    var character = input[newPosition.Y][newPosition.X];
                    if (character == '@')
                    {
                        surroundingObjects += 1;
                        if (surroundingObjects > maxSurroundingObjects)
                        {
                            break;
                        }
                    }
                }

                if (surroundingObjects <= maxSurroundingObjects)
                {
                    // Console.WriteLine($"Valid position {position}");
                    result += 1;
                }
            }
        }

        return result;
    }

    public long Solve2(List<string> input)
    {
        var size = new Vector2(input[0].Length, input.Count);
        var maxSurroundingObjects = 3;
        var removedCount = 0;
        var removed = new HashSet<Vector2>();

        do
        {
            removedCount = 0;
            for (var y = 0; y < size.Y; y++)
            {
                for (var x = 0; x < size.X; x++)
                {
                    var position = new Vector2(x, y);
                    var positionCharacter = input[position.Y][position.X];
                    if (positionCharacter != '@' || removed.Contains(position))
                    {
                        continue;
                    }

                    var surroundingObjects = 0;
                    foreach (var direction in directions)
                    {
                        var newPosition = position + direction;
                        if (outOfBounds(newPosition, size) || removed.Contains(newPosition))
                        {
                            continue;
                        }

                        var character = input[newPosition.Y][newPosition.X];
                        if (character == '@')
                        {
                            surroundingObjects += 1;
                            if (surroundingObjects > maxSurroundingObjects)
                            {
                                break;
                            }
                        }
                    }

                    if (surroundingObjects <= maxSurroundingObjects)
                    {
                        // Console.WriteLine($"Valid position {position}");
                        removed.Add(position);
                        removedCount += 1;
                    }
                }
            }
        } while (removedCount > 0);

        return removed.Count;
    }

    private bool outOfBounds(Vector2 position, Vector2 limits)
    {
        return position.X < 0 || position.X >= limits.X || position.Y < 0 || position.Y >= limits.Y;
    }
}

public record Vector2(int X, int Y)
{
    public static Vector2 operator +(Vector2 left, Vector2 right)
        => new Vector2(left.X + right.X, left.Y + right.Y);

    public override string ToString() => $"{this.X},{this.Y}";
}
