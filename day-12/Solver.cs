
namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var (Areas, Shapes) = Parse(input);
        Console.WriteLine($"Areas: {Areas.Count}, Shapes: {Shapes.Count}");

        long result = 0;
        foreach (var area in Areas)
        {
            result += SolveArea(area, Shapes);
        }

        return result;
    }

    public long SolveArea(Area area, List<Shape> shapes)
    {
        long shapesMinSize = 0;
        List<Shape> shapesToFit = [];
        for (var i = 0; i < area.ShapeCounts.Count; i++)
        {
            for (var count = 0; count < area.ShapeCounts[i]; count++)
            {
                shapesToFit.Add(shapes[i]);
                shapesMinSize += shapes[i].Size;
            }
        }

        long areaSize = area.Size.X * area.Size.Y;

        if (shapesMinSize > areaSize)
        {
            Console.WriteLine("it is impossible");
            return 0;
        }

        return 1;
    }

    public (List<Area> Areas, List<Shape> Shapes) Parse(List<string> input)
    {
        List<Area> areas = [];
        List<Shape> shapes = [];

        string mode = "indexOrArea";
        long currentIndex = 0;
        List<bool> buffer = [];
        foreach (var line in input)
        {
            switch (mode)
            {
                case "indexOrArea":
                    // Area
                    if (line.Contains("x"))
                    {
                        var parts = line.Split(":");
                        var sizeParts = parts[0].Split("x");
                        var size = new Vector2(long.Parse(sizeParts[0]), long.Parse(sizeParts[1]));
                        var shapeCounts = parts[1]
                            .Split(" ")
                            .Where(value => !string.IsNullOrWhiteSpace(value))
                            .Select(value => long.Parse(value))
                            .ToList();

                        areas.Add(new Area(size, shapeCounts));

                        break;
                    }
                    // Index
                    else
                    {
                        currentIndex = long.Parse(line.Substring(0, line.Length - 1));
                        mode = "shape";
                        break;
                    }
                case "shape":
                    if (line == string.Empty)
                    {
                        var shape = new Shape(currentIndex, buffer);
                        shapes.Add(shape);
                        buffer = [];

                        mode = "indexOrArea";
                        break;
                    }

                    foreach (var character in line)
                    {
                        buffer.Add(character == '#');
                    }
                    break;
            }
        }

        return (Areas: areas, Shapes: shapes);
    }
}

public record Area
{
    public Vector2 Size;
    public List<long> ShapeCounts;
    public bool[,] Grid;

    public Area(Vector2 size, List<long> shapeCounts)
    {
        Size = size;
        ShapeCounts = shapeCounts!;
        Grid = new bool[size.Y, size.X];
    }
}

public record Shape
{
    public long Index;
    public List<bool[,]> Variations = [];
    public long Size;

    public Shape(long index, List<bool> area)
    {
        Index = index;
        Size = area.Where(value => value).Count();

        Dictionary<string, bool[,]> seen = [];
        var matrixShape = new bool[3, 3];
        for (var y = 0; y < 3; y++)
        {
            for (var x = 0; x < 3; x++)
            {
                matrixShape[y, x] = area[y * 3 + x];
            }
        }

        for (var i = 0; i < 4; i++)
        {
            seen.TryAdd(Hash(matrixShape), matrixShape);
            seen.TryAdd(Hash(FlipVertical(matrixShape)), matrixShape);
            seen.TryAdd(Hash(FlipHorizontal(matrixShape)), matrixShape);
            matrixShape = RotateClockwise(matrixShape);
        }

        Variations = seen.Values.ToList();
    }

    public bool[,] RotateClockwise(bool[,] input)
    {
        var value = new bool[input.GetLength(0), input.GetLength(1)];
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                var newY = x;
                var newX = input.GetLength(0) - 1 - y;
                value[newY, newX] = input[y, x];
            }
        }

        return value;
    }

    public bool[,] FlipVertical(bool[,] input)
    {
        var value = new bool[input.GetLength(0), input.GetLength(1)];
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                var flippedY = input.GetLength(0) - 1 - y;
                value[flippedY, x] = input[y, x];
            }
        }

        return value;
    }

    public bool[,] FlipHorizontal(bool[,] input)
    {
        var value = new bool[input.GetLength(0), input.GetLength(1)];
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                var flippedX = input.GetLength(1) - 1 - x;
                value[y, flippedX] = input[y, x];
            }
        }

        return value;
    }

    public string Hash(bool[,] input)
    {
        var value = string.Empty;
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                value += input[y, x] ? "#" : ".";
            }
        }

        return value;
    }

    public void PrintShape(bool[,] input)
    {
        for (var y = 0; y < input.GetLength(0); y++)
        {
            var value = string.Empty;
            for (var x = 0; x < input.GetLength(1); x++)
            {
                value += input[y, x] ? "#" : ".";
            }
            Console.WriteLine(value);
        }
    }
}

public record Vector2(long X, long Y)
{
    public override string ToString() => $"({X},{Y})";
}
