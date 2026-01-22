namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var boxes = ParseJunctionBoxes(input);
        PriorityQueue<DistancePair, double> pairs = new();

        for (var i = 0; i < boxes.Count - 1; i++)
        {
            var current = boxes[i];
            for (var j = i + 1; j < boxes.Count; j++)
            {
                var next = boxes[j];
                var distance = Distance(current, next);
                pairs.Enqueue(new DistancePair(current, next, distance), distance);
            }
        }

        List<Circuit> circuits = [];

        // Change this depending on test vs. actual puzzle input
        var rounds = 1000; // 10, 1000
        for (var i = 0; i < rounds; i++)
        {
            var minPair = pairs.Dequeue();
            var newConnection = new Connection(minPair.First, minPair.Second);
            // Console.WriteLine($"Round {i}: Checking connection {minPair.First} {minPair.Second}");

            var circuitsWithFirst = circuits.Where(circuit => circuit.Nodes.Contains(minPair.First)).ToArray();
            var circuitsWithSecond = circuits.Where(circuit => circuit.Nodes.Contains(minPair.Second)).ToArray();

            if (circuitsWithFirst.Length > 1 || circuitsWithSecond.Length > 1)
            {
                throw new Exception("Something has gone wrong");
            }

            if (circuitsWithFirst.Length == 1 && circuitsWithSecond.Length == 1)
            {
                if (circuitsWithFirst[0] != circuitsWithSecond[0])
                {
                    // Combine circuits
                    foreach (var node in circuitsWithSecond[0].Nodes)
                    {
                        circuitsWithFirst[0].Nodes.Add(node);
                    }
                    foreach (var connection in circuitsWithSecond[0].Connections)
                    {
                        circuitsWithFirst[0].Connections.Add(connection);
                    }
                    circuits.Remove(circuitsWithSecond[0]);
                }
                else
                {
                    // Circuits are the same, no need to do anything
                }
            }
            else if (circuitsWithFirst.Length == 1 && circuitsWithSecond.Length == 0)
            {
                circuitsWithFirst[0].Nodes.Add(minPair.Second);
                circuitsWithFirst[0].Connections.Add(newConnection);
            }
            else if (circuitsWithFirst.Length == 0 && circuitsWithSecond.Length == 1)
            {
                circuitsWithSecond[0].Nodes.Add(minPair.First);
                circuitsWithSecond[0].Connections.Add(newConnection);
            }
            else if (circuitsWithFirst.Length == 0 && circuitsWithSecond.Length == 0)
            {
                var newCircuit = new Circuit();
                newCircuit.Connections.Add(newConnection);
                newCircuit.Nodes.Add(minPair.First);
                newCircuit.Nodes.Add(minPair.Second);
                circuits.Add(newCircuit);
            }
        }

        circuits.Sort((a, b) =>
        {
            if (a.Nodes.Count > b.Nodes.Count) return -1;
            if (a.Nodes.Count < b.Nodes.Count) return 1;
            return 0;
        });

        // foreach (var circuit in circuits)
        // {
        //     Console.WriteLine($"Circuit: {circuit.Nodes.Count}");
        // }

        long result = 1;
        for (var i = 0; i < 3; i++)
        {
            result *= circuits[i].Nodes.Count;
        }

        return result;
    }

    public long Solve2(List<string> input)
    {
        var boxes = ParseJunctionBoxes(input);
        PriorityQueue<DistancePair, double> pairs = new();

        for (var i = 0; i < boxes.Count - 1; i++)
        {
            var current = boxes[i];
            for (var j = i + 1; j < boxes.Count; j++)
            {
                var next = boxes[j];
                var distance = Distance(current, next);
                pairs.Enqueue(new DistancePair(current, next, distance), distance);
            }
        }

        List<Circuit> circuits = [];
        foreach (var box in boxes)
        {
            circuits.Add(new Circuit() { Nodes = [box] });
        }

        Connection? latestConnection = null;
        while (circuits.Count > 1)
        {
            var minPair = pairs.Dequeue();
            var newConnection = new Connection(minPair.First, minPair.Second);
            // Console.WriteLine($"Round {i}: Checking connection {minPair.First} {minPair.Second}");

            var circuitsWithFirst = circuits.Where(circuit => circuit.Nodes.Contains(minPair.First)).ToArray();
            var circuitsWithSecond = circuits.Where(circuit => circuit.Nodes.Contains(minPair.Second)).ToArray();

            if (circuitsWithFirst.Length > 1 || circuitsWithSecond.Length > 1)
            {
                throw new Exception("Something has gone wrong");
            }

            if (circuitsWithFirst.Length == 1 && circuitsWithSecond.Length == 1)
            {
                if (circuitsWithFirst[0] != circuitsWithSecond[0])
                {
                    // Combine circuits
                    foreach (var node in circuitsWithSecond[0].Nodes)
                    {
                        circuitsWithFirst[0].Nodes.Add(node);
                    }
                    foreach (var connection in circuitsWithSecond[0].Connections)
                    {
                        circuitsWithFirst[0].Connections.Add(connection);
                    }
                    circuits.Remove(circuitsWithSecond[0]);
                    latestConnection = newConnection;
                }
                else
                {
                    // Circuits are the same, no need to do anything
                }
            }
            else if (circuitsWithFirst.Length == 1 && circuitsWithSecond.Length == 0)
            {
                circuitsWithFirst[0].Nodes.Add(minPair.Second);
                circuitsWithFirst[0].Connections.Add(newConnection);
                latestConnection = newConnection;
            }
            else if (circuitsWithFirst.Length == 0 && circuitsWithSecond.Length == 1)
            {
                circuitsWithSecond[0].Nodes.Add(minPair.First);
                circuitsWithSecond[0].Connections.Add(newConnection);
                latestConnection = newConnection;
            }
            else if (circuitsWithFirst.Length == 0 && circuitsWithSecond.Length == 0)
            {
                var newCircuit = new Circuit();
                newCircuit.Connections.Add(newConnection);
                newCircuit.Nodes.Add(minPair.First);
                newCircuit.Nodes.Add(minPair.Second);
                circuits.Add(newCircuit);
                latestConnection = newConnection;
            }
        }

        // Console.WriteLine($"Latest connection {latestConnection}");
        var result = latestConnection!.First.X * latestConnection!.Second.X;

        return result;
    }

    private List<Vector3> ParseJunctionBoxes(List<string> input)
    {
        List<Vector3> results = [];
        foreach (var line in input)
        {
            var parts = line.Split(",");
            results.Add(new Vector3(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2])));
        }

        return results;
    }

    private double Distance(Vector3 left, Vector3 right)
    {
        // Square root can be removed for small optimisation
        return Math.Sqrt(Math.Pow(left.X - right.X, 2) + Math.Pow(left.Y - right.Y, 2) + Math.Pow(left.Z - right.Z, 2));
    }
}

public record Vector3(long X, long Y, long Z)
{
    public override string ToString() => $"{X},{Y},{Z}";
}

public record DistancePair(Vector3 First, Vector3 Second, double Distance);

public record Connection(Vector3 First, Vector3 Second);

public record Circuit()
{
    public HashSet<Connection> Connections = [];
    public HashSet<Vector3> Nodes = [];
}
