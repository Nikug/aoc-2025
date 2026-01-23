namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var machines = Parse(input);

        List<Machine> results = [];
        foreach (var machine in machines)
        {
            HashSet<string> seen = [machine.LightsHash()];
            Queue<Machine> queue = [];
            queue.Enqueue(machine);

            while (queue.Peek() is not null)
            {
                var next = queue.Dequeue();
                if (next.IsCorrect())
                {
                    results.Add(next);
                    break;
                }

                foreach (var button in next.Buttons)
                {
                    var newMachine = next.PressButton(button);
                    if (!seen.Contains(newMachine.LightsHash()))
                    {
                        seen.Add(newMachine.LightsHash());
                        queue.Enqueue(newMachine);
                    }
                }
            }
        }

        long result = results.Sum(machine => machine.ButtonPresses);

        return result;
    }

    public List<Machine> Parse(List<string> input)
    {
        List<Machine> machines = [];
        foreach (var line in input)
        {
            var parts = line.Split(" ");
            var machine = new Machine();
            foreach (var part in parts)
            {
                if (part.StartsWith("["))
                {
                    var lights = part.Substring(1, part.Length - 2);
                    machine.Lights = new bool[lights.Length];
                    machine.LightsTarget = new bool[lights.Length];
                    for (var i = 0; i < lights.Length; i++)
                    {
                        machine.LightsTarget[i] = lights[i] == '#';
                    }
                }
                else if (part.StartsWith("("))
                {
                    var buttons = part.Substring(1, part.Length - 2).Split(",");
                    var button = new long[buttons.Length];
                    for (var i = 0; i < buttons.Length; i++)
                    {
                        button[i] = long.Parse(buttons[i]);
                    }
                    machine.Buttons.Add(button);
                }
                else if (part.StartsWith("{"))
                {
                    var joltages = part.Substring(1, part.Length - 2).Split(",");
                    machine.Joltages = new long[joltages.Length];
                    for (var i = 0; i < joltages.Length; i++)
                    {
                        machine.Joltages[i] = long.Parse(joltages[i]);
                    }
                }
                else
                {
                    throw new Exception($"Parsing failed for part {part}");
                }
            }
            machines.Add(machine);
        }

        return machines;
    }
}

public record Machine
{
    public bool[] Lights = [];
    public bool[] LightsTarget = [];
    public List<long[]> Buttons = [];
    public long ButtonPresses = 0;
    public long[] Joltages = [];

    public Machine PressButton(long[] button)
    {
        var newLights = Lights.Select((light, index) => button.Contains(index) ? !light : light);
        return this with { Lights = newLights.ToArray(), ButtonPresses = ButtonPresses + 1 };
    }

    public bool IsCorrect()
    {
        for (var i = 0; i < Lights.Length; i++)
        {
            if (Lights[i] != LightsTarget[i])
            {
                return false;
            }
        }

        return true;
    }

    public string LightsHash() => string.Join("", Lights.Select(light => light ? "#" : "."));

    public override string ToString()
    {
        var lights = string.Join("", this.Lights.Select(light => light ? "#" : "."));
        var lightsTarget = string.Join("", this.LightsTarget.Select(light => light ? "#" : "."));
        var buttons = string.Join(" ", this.Buttons.Select(button => $"({string.Join(",", button)})"));
        var joltages = string.Join(",", this.Joltages);
        return $"Target: {lightsTarget}, Actual: {lights}, Buttons: {buttons}, Joltages: {joltages}";
    }
}
