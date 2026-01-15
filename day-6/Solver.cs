using System.Text.RegularExpressions;

namespace Puzzle;

public class Solver
{
    public long Solve(List<string> input)
    {
        var calculations = ParseCalculations(input);

        long sum = 0;
        foreach (var calculation in calculations)
        {
            // Console.WriteLine(calculation);
            sum += calculation.Calculate();
        }
        return sum;
    }

    public long Solve2(List<string> input)
    {
        var calculations = ParseCalculationsVertical(input);

        long sum = 0;
        foreach (var calculation in calculations)
        {
            // Console.WriteLine(calculation);
            sum += calculation.Calculate();
        }
        return sum;
    }

    private List<Calculation> ParseCalculations(List<string> input)
    {
        var pattern = @"\s+";
        List<Calculation> calculations = [];
        foreach (var line in input)
        {
            var values = Regex.Split(line, pattern)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var isOperation = value == "+" || value == "*";

                if (i >= calculations.Count)
                {
                    calculations.Add(new Calculation()
                    {
                        Values = new List<long>() { long.Parse(value) },
                        Operation = '+'
                    });
                }
                else
                {
                    if (isOperation)
                    {
                        calculations[i].Operation = value[0];
                    }
                    else
                    {
                        calculations[i].Values.Add(long.Parse(value));
                    }
                }
            }
        }

        return calculations;
    }

    private List<Calculation> ParseCalculationsVertical(List<string> input)
    {
        var width = input[0].Length;
        var height = input.Count;
        List<Calculation> calculations = [];

        var calculation = new Calculation { Values = [], Operation = '+' };
        for (var x = width - 1; x >= 0; x--)
        {
            var buffer = string.Empty;
            for (var y = 0; y < height; y++)
            {
                var character = input[y][x];
                if (character == ' ')
                {
                    continue;
                }
                else if (character == '+' || character == '*')
                {
                    calculation.Operation = character;
                }
                else
                {
                    buffer += character;
                }
            }

            if (buffer.Length > 0)
            {
                calculation.Values.Add(long.Parse(buffer));
            }
            else
            {
                // Empty column, calculation is done
                calculations.Add(calculation);
                calculation = new Calculation { Values = [], Operation = '+' };
            }
        }

        // Data doesn't start with empty column, add last calculation manually
        calculations.Add(calculation);

        return calculations;
    }
}

public class Calculation
{
    public required List<long> Values;
    public required char Operation;

    public long Calculate()
    {
        if (this.Operation == '+')
        {
            long total = 0;
            foreach (var value in this.Values)
            {
                total += value;
            }
            return total;
        }
        else if (this.Operation == '*')
        {
            long total = 1;
            foreach (var value in this.Values)
            {
                total *= value;
            }
            return total;
        }
        else
        {
            throw new ArgumentException($"Invalid operation {this.Operation}");
        }
    }

    public override string ToString() => $"{this.Operation} {string.Join(",", this.Values)}";
}
