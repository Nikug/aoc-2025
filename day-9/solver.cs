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

    public long Solve2(List<string> input)
    {
        List<Vector2> corners = Parse(input);
        var polygon = new Polygon();

        for (var i = 0; i < corners.Count - 1; i++)
        {
            var newEdge = new Edge(corners[i], corners[i + 1]);
            polygon.AddEdge(newEdge);
        }
        polygon.AddEdge(new Edge(corners[corners.Count - 1], corners[0]));

        long largestArea = 0;
        var indexA = 0;
        var indexB = 0;
        for (var i = 0; i < corners.Count - 1; i++)
        {
            for (var j = i + 1; j < corners.Count; j++)
            {
                var area = RectangleArea(corners[i], corners[j]);
                if (area > largestArea && polygon.ContainsRectangle(corners[i], corners[j]))
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

public record Edge(Vector2 Start, Vector2 End);

public class Polygon
{
    private HashSet<Vector2> nodes = [];
    private HashSet<Edge> horizontalEdges = [];
    private HashSet<Edge> verticalEdges = [];

    public void AddEdge(Edge newEdge)
    {
        var isHorizontal = newEdge.Start.Y == newEdge.End.Y;
        var isVertical = newEdge.Start.X == newEdge.End.X;

        if (!isHorizontal && !isVertical)
        {
            throw new ArgumentException("Edge is not a straight line");
        }

        nodes.Add(newEdge.Start);
        nodes.Add(newEdge.End);

        if (isHorizontal)
        {
            horizontalEdges.Add(newEdge);
            // Console.WriteLine($"H edge {newEdge}");
        }

        if (isVertical)
        {
            verticalEdges.Add(newEdge);
            // Console.WriteLine($"V edge {newEdge}");
        }
    }

    public bool ContainsRectangle(Vector2 a, Vector2 b)
    {
        // No node of the polygon can be inside rectangle
        var minX = Math.Min(a.X, b.X);
        var maxX = Math.Max(a.X, b.X);
        var minY = Math.Min(a.Y, b.Y);
        var maxY = Math.Max(a.Y, b.Y);

        foreach (var node in nodes)
        {
            if (node.X > minX && node.X < maxX && node.Y > minY && node.Y < maxY)
            {
                // Console.WriteLine($"Rectangle {a}-{b} contains polygon node {node}");
                return false;
            }
        }

        // No edge of the rectangle can intersect polygon edge (ends excluded)
        var edges = GetEdges(a, b);
        foreach (var edge in edges)
        {
            if (Intersects(edge))
            {
                // Console.WriteLine($"Rectangle {a}-{b} intersects with edge {edge}");
                return false;
            }
        }

        // No corner of the rectangle can be outside of polygon
        List<Vector2> corners = [
            a,
            b,
            new Vector2(a.X, b.Y),
            new Vector2(b.X, a.Y),
        ];
        foreach (var corner in corners)
        {
            if (!IsPointInside(corner))
            {
                // Console.WriteLine($"Rectangle {a}-{b} corner {corner} is outside of polygon");
                return false;
            }
        }

        return true;
    }

    private bool IsPointInside(Vector2 point)
    {
        // Polygon nodes are always inside
        if (nodes.Contains(point)) return true;

        // Edge nodes are always inside
        var containedInEdge = false;
        foreach (var verticalEdge in verticalEdges)
        {
            if (point.X == verticalEdge.Start.X)
            {
                var minY = Math.Min(verticalEdge.Start.Y, verticalEdge.End.Y);
                var maxY = Math.Max(verticalEdge.Start.Y, verticalEdge.End.Y);
                if (point.Y >= minY && point.Y <= maxY)
                {
                    containedInEdge = true;
                    break;
                }
            }
        }

        if (containedInEdge)
        {
            return true;
        }

        foreach (var horizontalEdge in horizontalEdges)
        {
            if (point.Y == horizontalEdge.Start.Y)
            {
                var minX = Math.Min(horizontalEdge.Start.X, horizontalEdge.End.X);
                var maxX = Math.Max(horizontalEdge.Start.X, horizontalEdge.End.X);
                if (point.X >= minX && point.X <= maxX)
                {
                    containedInEdge = true;
                    break;
                }
            }
        }

        if (containedInEdge)
        {
            return true;
        }

        // Raycast to see if point is inside
        long raycastLength = 1_000_000;
        var e1 = new Edge(point, new Vector2(point.X + raycastLength, point.Y));

        var intersects = 0;
        var e1minx = e1.Start.X;
        var e1maxx = e1.End.X;
        var e1y = e1.Start.Y;

        foreach (var e2 in verticalEdges)
        {
            var e2miny = Math.Min(e2.Start.Y, e2.End.Y);
            var e2maxy = Math.Max(e2.Start.Y, e2.End.Y);
            var e2x = e2.Start.X;

            if (e1minx <= e2x && e2x <= e1maxx && e2miny <= e1y && e1y <= e2maxy)
            {
                intersects += 1;
            }
        }

        return intersects % 2 == 1;
    }

    private bool Intersects(Edge e1)
    {
        var isHorizontal = e1.Start.Y == e1.End.Y;
        var isVertical = e1.Start.X == e1.End.X;

        if (!isHorizontal && !isVertical)
        {
            throw new ArgumentException("Edge is not a straight line");
        }

        if (isHorizontal)
        {
            var e1minx = Math.Min(e1.Start.X, e1.End.X);
            var e1maxx = Math.Max(e1.Start.X, e1.End.X);
            var e1y = e1.Start.Y;

            foreach (var e2 in verticalEdges)
            {
                var e2miny = Math.Min(e2.Start.Y, e2.End.Y);
                var e2maxy = Math.Max(e2.Start.Y, e2.End.Y);
                var e2x = e2.Start.X;

                if (e1minx < e2x && e2x < e1maxx && e2miny < e1y && e1y < e2maxy)
                {
                    return true;
                }
            }
        }
        else if (isVertical)
        {
            var e1miny = Math.Min(e1.Start.Y, e1.End.Y);
            var e1maxy = Math.Max(e1.Start.Y, e1.End.Y);
            var e1x = e1.Start.X;

            foreach (var e2 in horizontalEdges)
            {
                var e2minx = Math.Min(e2.Start.X, e2.End.X);
                var e2maxx = Math.Max(e2.Start.X, e2.End.X);
                var e2y = e2.Start.Y;

                if (e1miny < e2y && e2y < e1maxy && e2minx < e1x && e1x < e2maxx)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private List<Edge> GetEdges(Vector2 a, Vector2 b)
    {
        List<Edge> result = [
            new Edge(a, new Vector2(a.X, b.Y)),
            new Edge(a, new Vector2(b.X, a.Y)),
            new Edge(b, new Vector2(b.X, a.Y)),
            new Edge(b, new Vector2(a.X, b.Y))
        ];

        return result;
    }

    public override string ToString() => $"Nodes: {nodes.Count}, Edges: {horizontalEdges.Count + verticalEdges.Count}";
}
