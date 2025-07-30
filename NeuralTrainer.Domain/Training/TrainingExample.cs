namespace NeuralTrainer.Domain.Training;

public class TrainingExample(double[] input, double target)
{
	public IReadOnlyList<double> Inputs { get; } = input.AsReadOnly();
	public double Target { get; } = target;
}
