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

            while (queue.Count > 0)
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

    public long Solve2(List<string> input)
    {
        var machines = Parse(input);

        List<Machine> results = [];
        var rounds = 0;
        foreach (var machine in machines)
        {


            // The machines need to get smaller
            // For each joltage index get only buttons where the joltage index is first
            // For each button
            // - Press it 1->max times and remove the button
            // If first joltage is not solved
            // - For each remaining button
            //   - Press it 1->max times and remove the button
            // Once first joltage is solved, move to next joltage with the remaining buttons

            rounds++;
            Console.WriteLine($"machine {rounds} / {machines.Count}");

            Dictionary<long, List<long[]>> buttonsByJoltageIndex = [];
            for (var i = 0; i < machine.Joltages.Length; i++)
            {
                var buttons = machine.Buttons.Where(button => button.Contains(i)).ToList();
                buttons.Sort((a, b) => b.Length - a.Length); // Largest first
                buttonsByJoltageIndex.Add(i, buttons);
            }

            HashSet<string> seen = [machine.JoltagesHash()];
            Queue<Machine> queue = new();
            queue.Enqueue(machine);

            while (queue.Count > 0)
            {
                var next = queue.Dequeue();

                // Console.WriteLine(next.MachineJoltageString());

                if (next.IsOverJoltage())
                {
                    continue;
                }

                if (next.IsCorrectJoltage())
                {
                    results.Add(next);
                    break;
                }

                var joltageIndex = next.FirstIncorrectJoltage();
                var joltageDifference = next.JoltagesTarget[joltageIndex] - next.Joltages[joltageIndex];
                for (var i = 1; i <= joltageDifference; i++)
                {
                    var buttons = buttonsByJoltageIndex[joltageIndex];
                    foreach (var button in buttons)
                    {
                        var max = next.MaxNumberOfPresses(button);
                        if (i > max)
                        {
                            goto leave;
                        }

                        var newMachine = next.PressButtonJoltage(button, i);
                        if (seen.Contains(newMachine.JoltagesHash()))
                        {
                            continue;
                        }
                        seen.Add(newMachine.JoltagesHash());
                        queue.Enqueue(newMachine);
                    }
                }
            leave:;
            }
        }

        Console.WriteLine($"Found solution for {results.Count}/{machines.Count}");

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
                    machine.JoltagesTarget = new long[joltages.Length];
                    for (var i = 0; i < joltages.Length; i++)
                    {
                        machine.JoltagesTarget[i] = long.Parse(joltages[i]);
                    }
                }
                else
                {
                    throw new Exception($"Parsing failed for part {part}");
                }
            }
            machine.Buttons.Sort((a, b) => b.Length - a.Length);
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
    public long[] JoltagesTarget = [];

    public Machine PressButton(long[] button)
    {
        var newLights = Lights.Select((light, index) => button.Contains(index) ? !light : light);
        return this with { Lights = newLights.ToArray(), ButtonPresses = ButtonPresses + 1 };
    }

    public Machine PressButtonJoltage(long[] button, long times)
    {
        var newJoltages = Joltages.Select((joltage, index) => button.Contains(index) ? joltage + times : joltage);
        return this with { Joltages = newJoltages.ToArray(), ButtonPresses = ButtonPresses + times };
    }

    public long MaxNumberOfPresses(long[] button)
    {
        var differenceArray = JoltagesTarget.Select((target, index) => target - Joltages[index]);
        var maxNumberOfPresses = differenceArray.Where((_, index) => button.Contains(index)).Min();
        return maxNumberOfPresses;
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

    public bool IsCorrectJoltage()
    {
        for (var i = 0; i < Joltages.Length; i++)
        {
            if (Joltages[i] != JoltagesTarget[i])
            {
                return false;
            }
        }

        return true;
    }

    public bool IsOverJoltage()
    {
        for (var i = 0; i < Joltages.Length; i++)
        {
            if (Joltages[i] > JoltagesTarget[i])
            {
                return true;
            }
        }

        return false;
    }

    public double DistanceFromSolution()
    {
        double sum = 0;
        for (var i = 0; i < Joltages.Length; i++)
        {
            sum += (JoltagesTarget[i] - Joltages[i]) / JoltagesTarget[i];
        }

        return sum;
    }

    public long FirstIncorrectJoltage()
    {
        for (var i = 0; i < Joltages.Length; i++)
        {
            if (Joltages[i] != JoltagesTarget[i])
            {
                return i;
            }
        }

        return -1;
    }

    public string LightsHash() => string.Join("", Lights.Select(light => light ? "#" : "."));
    public string JoltagesHash() => string.Join(",", Joltages);

    public string MachineLightString()
    {
        var lights = string.Join("", this.Lights.Select(light => light ? "#" : "."));
        var lightsTarget = string.Join("", this.LightsTarget.Select(light => light ? "#" : "."));
        var buttons = string.Join(" ", this.Buttons.Select(button => $"({string.Join(",", button)})"));
        return $"Target: {lightsTarget}, Actual: {lights}, Buttons: {buttons}";
    }

    public string MachineJoltageString()
    {
        var joltages = string.Join(",", Joltages);
        var joltagesTarget = string.Join(",", JoltagesTarget);
        var buttons = string.Join(" ", this.Buttons.Select(button => $"({string.Join(",", button)})"));
        return $"Target: {joltagesTarget} Actual: {joltages} Buttons: {buttons}";
    }
}
