namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var nodes = Parse(input);
        var result = FindPaths(nodes["you"], nodes["out"]);
        return result;
    }

    public long Solve2(List<string> input)
    {
        var nodes = Parse(input);
        var srv2fft = FindPathsWithCache(nodes["svr"], nodes["fft"]);
        var srv2dac = FindPathsWithCache(nodes["svr"], nodes["dac"]);
        var fft2dac = FindPathsWithCache(nodes["fft"], nodes["dac"]);
        var dac2fft = FindPathsWithCache(nodes["dac"], nodes["fft"]);
        var fft2out = FindPathsWithCache(nodes["fft"], nodes["out"]);
        var dac2out = FindPathsWithCache(nodes["dac"], nodes["out"]);

        // One of the paths is 0 and could be skipped completely
        // Since the three has no loops, it is impossible to have fft->dac and dac->fft
        var path1 = srv2fft * fft2dac * dac2out;
        var path2 = srv2dac * dac2fft * fft2out;
        return path1 + path2;
    }

    public long FindPathsWithCache(Node startNode, Node target)
    {
        Dictionary<string, long> cache = [];
        cache.Add(target.Value, 1);

        var result = WalkPath(startNode, cache);
        return result;
    }

    public long WalkPath(Node startNode, Dictionary<string, long> cache)
    {
        if (cache.ContainsKey(startNode.Value))
        {
            return cache[startNode.Value];
        }

        long goalRoutes = 0;
        foreach (var node in startNode.Nodes)
        {
            var goals = WalkPath(node, cache);
            cache[node.Value] = goals;
            goalRoutes += goals;
        }

        cache[startNode.Value] = goalRoutes;
        return goalRoutes;
    }

    public long FindPaths(Node startNode, Node target)
    {
        long result = 0;
        Queue<Node> queue = [];
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (node == target)
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
