namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var nodes = Parse(input);
        var result = FindPaths(nodes["you"]);
        return result;
    }

    public long FindPaths(Node startNode)
    {
        long result = 0;
        Queue<Node> queue = [];
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (node.Value == "out")
            {
                result += 1;
                continue;
            }

            foreach (var nextNode in node.Nodes)
            {
                queue.Enqueue(nextNode);
            }
        }

        return result;
    }

    public Dictionary<string, Node> Parse(List<string> input)
    {
        Dictionary<string, Node> nodes = [];

        foreach (var line in input)
        {
            var parts = line.Split(":");
            var source = parts[0];
            var values = parts[1].Split(" ").Where(value => !string.IsNullOrWhiteSpace(value));

            Node node = null!;
            if (nodes.ContainsKey(source))
            {
                node = nodes[source];
            }
            else
            {
                node = new Node { Value = source };
                nodes.Add(source, node);
            }

            foreach (var value in values)
            {
                if (nodes.ContainsKey(value))
                {
                    var childNode = nodes[value];
                    node.Nodes.Add(childNode);
                }
                else
                {
                    var childNode = new Node { Value = value };
                    node.Nodes.Add(childNode);
                    nodes.Add(value, childNode);
                }
            }
        }

        return nodes;
    }
}

public record Node
{
    public required string Value;
    public List<Node> Nodes = [];
}
