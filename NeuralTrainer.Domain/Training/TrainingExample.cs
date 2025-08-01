namespace NeuralTrainer.Domain.Training;

public class TrainingExample(double[] input, double[] targets)
{
	public IReadOnlyList<double> Inputs { get; } = input.AsReadOnly();
	public IReadOnlyList<double> Targets { get; } = targets.AsReadOnly();

	public override string ToString()
	{
		var inputs = string.Join(',', Inputs);
		var targets = string.Join(',', Targets);
		return $"Inputs: [{inputs}], expected: {targets}";
	}
}
