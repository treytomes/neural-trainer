namespace NeuralTrainer.Domain.Training;

public class TrainingExample(double[] input, double target)
{
	public IReadOnlyList<double> Inputs { get; } = input.AsReadOnly();
	public double Target { get; } = target;

	public override string ToString()
	{
		var inputs = string.Join(',', Inputs.Select(x => x.ToString()));
		return $"Inputs: [{inputs}], expected: {Target}";
	}
}
