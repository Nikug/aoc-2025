namespace Puzzle;

public class Solver
{
    private int knobMinValue = 0;
    private int knobMaxValue = 99;

    public int Solve(List<string> input)
    {
        var knob = 50;
        var numberOfPointingZero = 0;

        foreach (var rotation in input)
        {
            var direction = rotation[0];
            var steps = int.Parse(rotation.AsSpan(1));
            var stepsWithoutFullRotations = steps % (knobMaxValue + 1);

            var knobBefore = knob;

            if (direction == 'L')
            {
                knob -= stepsWithoutFullRotations;
                if (knob < knobMinValue)
                {
                    knob = knobMaxValue + knob + 1;
                }
            }
            else
            {
                knob += stepsWithoutFullRotations;
                if (knob > knobMaxValue)
                {
                    knob -= knobMaxValue + 1;
                }
            }

            // Console.WriteLine($"Knob: {knobBefore}->{knob}, Step: {rotation}");

            if (knob == 0)
            {
                numberOfPointingZero += 1;
            }
        }

        return numberOfPointingZero;
    }

    public int Solve2(List<string> input)
    {
        var knob = 50;
        var numberOfPassingZero = 0;

        foreach (var rotation in input)
        {
            var direction = rotation[0];
            var steps = int.Parse(rotation.AsSpan(1));
            var stepsWithoutFullRotations = steps % (knobMaxValue + 1);

            var fullRotations = steps / (int)(knobMaxValue + 1);
            numberOfPassingZero += fullRotations;

            var knobBefore = knob;

            if (direction == 'L')
            {
                knob -= stepsWithoutFullRotations;
                if (knob < knobMinValue)
                {
                    knob = knobMaxValue + knob + 1;
                    if (knobBefore != 0)
                    {
                        numberOfPassingZero += 1;
                    }
                }
                else if (knob == 0)
                {
                    numberOfPassingZero += 1;
                }
            }
            else
            {
                knob += stepsWithoutFullRotations;
                if (knob > knobMaxValue)
                {
                    knob -= knobMaxValue + 1;
                    if (knobBefore != 0)
                    {
                        numberOfPassingZero += 1;
                    }
                }
                else if (knob == 0)
                {
                    numberOfPassingZero += 1;
                }
            }

            Console.WriteLine($"Knob: {knobBefore}->{knob}, Step: {rotation}, Zeros: {numberOfPassingZero}");
        }

        return numberOfPassingZero;
    }
}
