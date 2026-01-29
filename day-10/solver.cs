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
        long sum = 0;

        foreach (var machine in machines)
        {
            Dictionary<string, long> cache = [];
            machine.Joltages = machine.JoltagesTarget.ToArray();
            var result = HalveAndRecurse(machine, cache);
            sum += result;
            // Console.WriteLine($"Result: {result}");
        }

        return sum;
    }

    public long HalveAndRecurse(Machine machine, Dictionary<string, long> cache)
    {
        if (machine.Joltages.All(joltage => joltage == 0))
        {
            // Console.WriteLine($"Solution found!");
            return 0;
        }

        var hash = machine.JoltagesHash();
        if (cache.ContainsKey(hash))
        {
            return cache[hash];
        }

        long bestResult = 1000000;
        var combinations = CombinationsMatchingEvenOdd(machine);
        foreach (var combination in combinations)
        {
            var newButtonPresses = combination.ButtonPresses.Count;
            var reducedVoltage = machine.ReduceJoltage(combination.ButtonPresses);

            if (reducedVoltage.Joltages.Any(joltage => joltage < 0))
            {
                continue;
            }


            var newMachine = reducedVoltage.HalveJoltage();
            var result = HalveAndRecurse(newMachine, cache) * 2 + newButtonPresses;
            bestResult = Math.Min(bestResult, result);
        }

        if (!cache.ContainsKey(hash))
        {
            cache[hash] = bestResult;
        }

        return bestResult;
    }

    public List<MiniMachine> CombinationsMatchingEvenOdd(Machine machine)
    {
        List<MiniMachine> results = [];
        var evenOdd = machine.GetEvenOdd();
        var miniMachine = new MiniMachine
        {
            Buttons = new List<long[]>(machine.Buttons)
        };
        miniMachine.Init(evenOdd);

        Queue<MiniMachine> queue = [];
        queue.Enqueue(miniMachine);

        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            var isCorrect = Enumerable.SequenceEqual(next.Target, next.Current);
            if (isCorrect)
            {
                results.Add(next);
            }

            if (next.ButtonIndexes.Count > 0)
            {
                queue.Enqueue(next.PressNextButton());
                queue.Enqueue(next.SkipNextButton());
            }
        }

        return results;
    }


    // Should work but is too slow to finish
    public long Solve2ButDoesntFinish(List<string> input)
    {
        var machines = Parse(input);

        var round = 0;
        List<Machine> results = [];
        foreach (var machine in machines)
        {
            round += 1;
            Console.WriteLine($"Round {round}");
            // Sort by first index, then by size
            machine.Buttons.Sort((a, b) =>
            {
                if (a[0] < b[0]) return -1;
                if (a[0] > b[0]) return 1;
                if (a[0] == b[0])
                {
                    if (a.Length > b.Length) return -1;
                    if (a.Length < b.Length) return 1;
                }
                return 0;
            });

            var joltagesByIndex = machine.JoltagesTarget
                .Select((target, index) => (target, index))
                .ToList();
            joltagesByIndex
                .Sort((a, b) => (int)a.target - (int)b.target);
            machine.Indexes = joltagesByIndex.Select(j => (long)j.index).ToArray();

            Stack<Machine> stack = [];
            stack.Push(machine);
            while (stack.Count > 0)
            {
                var currentMachine = stack.Pop();
                var joltageIndex = currentMachine.Indexes.First();
                var target = currentMachine.JoltagesTarget[joltageIndex];
                var current = currentMachine.Joltages[joltageIndex];
                var buttons = currentMachine.Buttons.Where(button => button.Any(index => index == joltageIndex));
                var remainingButtons = buttons.Count();
                var button = buttons.FirstOrDefault();
                var isFinalIndex = currentMachine.Indexes.Length == 1;

                if (button is null)
                {
                    if (!isFinalIndex && current == target)
                    {
                        var newIndexes = currentMachine.Indexes[1..];
                        stack.Push(currentMachine with { Indexes = newIndexes });
                    }

                    continue;
                }

                // Console.WriteLine($"Current: {string.Join(",", currentMachine.Joltages)}, target: {string.Join(",", currentMachine.JoltagesTarget)}");
                // Console.WriteLine($"Buttons {string.Join(" ", currentMachine.Buttons.Select(b => string.Join(",", b)))}");
                // Console.WriteLine($"Target {target}, current {current}");

                for (var i = 0; i <= target - current; i++)
                {
                    if (remainingButtons == 1 && i + current != target)
                    {
                        continue;
                    }

                    var newMachine = currentMachine.PressButtonJoltage(button, i);
                    if (newMachine.IsOverJoltage()) break;

                    var newCurrent = newMachine.Joltages[joltageIndex];
                    var newTarget = newMachine.JoltagesTarget[joltageIndex];

                    // Console.WriteLine($"Status {newTarget}={newCurrent} {joltageIndex} {newMachine.MachineJoltageString()}");

                    if (newCurrent == newTarget)
                    {
                        if (newMachine.IsCorrectJoltage())
                        {
                            Console.WriteLine($"Found solution! {newMachine.ButtonPresses}");
                            results.Add(newMachine);
                            goto gotoNextmachine;
                        }
                        else if (!isFinalIndex)
                        {
                            // Buttons for next joltage
                            if (newMachine.Buttons.Count > 0)
                            {
                                // Remove all buttons affecting current index, increase index
                                newMachine.Buttons = newMachine.Buttons
                                    .Where(button => button.All(index => index != joltageIndex))
                                    .ToList();
                                newMachine.Indexes = newMachine.Indexes[1..];
                                stack.Push(newMachine);
                            }
                        }
                    }
                    else
                    {
                        // Buttons for this joltage index
                        if (buttons.Count() > 0)
                        {
                            // Remove this button, use the next for this same index
                            newMachine.Buttons = newMachine.Buttons.Where(b => b != button).ToList();
                            stack.Push(newMachine);
                        }
                    }

                }
            }
        gotoNextmachine:;
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

public record MiniMachine
{
    public bool[] Target = [];
    public bool[] Current = [];
    public List<long[]> Buttons = [];
    public List<int> ButtonIndexes = [];
    public List<int> ButtonPresses = [];

    public void Init(List<bool> target)
    {
        Target = target.ToArray();
        Current = target.Select(_ => true).ToArray();
        ButtonIndexes = Buttons.Select((_, i) => i).ToList();
    }

    public MiniMachine PressNextButton()
    {
        var buttonIndex = ButtonIndexes[0];
        var newButtonPresses = new List<int>(ButtonPresses);
        newButtonPresses.Add(buttonIndex);

        var button = Buttons[buttonIndex];
        var newCurrent = Current.Select((value, index) => button.Contains(index) ? !value : value).ToArray();
        var newButtonIndexes = ButtonIndexes.Slice(1, ButtonIndexes.Count - 1);
        return this with { Current = newCurrent, ButtonIndexes = newButtonIndexes, ButtonPresses = newButtonPresses };
    }

    public MiniMachine SkipNextButton()
    {
        var newButtonIndexes = ButtonIndexes.Slice(1, ButtonIndexes.Count - 1);
        return this with { ButtonIndexes = newButtonIndexes };
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
    public long Index = 0;
    public long[] Indexes = [];

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

    public Machine ReduceJoltage(List<int> buttonIndexes)
    {
        var newJoltages = Joltages.ToArray();
        foreach (var buttonIndex in buttonIndexes)
        {
            var button = Buttons[buttonIndex];
            foreach (var index in button)
            {
                newJoltages[index] -= 1;
            }
        }

        return this with { Joltages = newJoltages };
    }

    public Machine HalveJoltage()
    {
        var newJoltages = Joltages.Select(joltage => joltage / 2).ToArray();
        return this with { Joltages = newJoltages };
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

    public List<bool> GetEvenOdd()
    {
        List<bool> evenOdd = [];
        foreach (var joltage in Joltages)
        {
            evenOdd.Add(joltage % 2 == 0);
        }

        return evenOdd;
    }

    public string LightsHash() => string.Join("", Lights.Select(light => light ? "#" : "."));
    public string JoltagesHash() => $"{string.Join(",", Joltages)} {string.Join(",", Buttons.Select(b => string.Join(",", b)))}";

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
