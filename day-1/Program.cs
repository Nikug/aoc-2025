using Puzzle;

class Program
{
    static void Main(string[] args)
    {
        var inputFile = "test.txt";
        if (args.Length > 0)
        {
            inputFile = args[0];
        }

        var lines = ReadFile(inputFile);

        var solver = new Solver();
        var solution = solver.Solve2(lines);

        Console.WriteLine($"Solution: {solution}");
    }

    static private List<string> ReadFile(string fileName)
    {
        List<string> lines = [];
        using StreamReader reader = new(fileName);

        while (reader.Peek() >= 0)
        {
            var line = reader.ReadLine();
            if (line is not null)
            {
                lines.Add(line);
            }
        }

        return lines;
    }
}
