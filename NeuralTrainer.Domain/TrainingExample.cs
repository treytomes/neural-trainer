namespace NeuralTrainer.Domain;

public class TrainingExample(double input, double target)
{
	public double Input { get; } = input;
	public double Target { get; } = target;
}
