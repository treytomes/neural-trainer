namespace NeuralTrainer.Domain;

public class TrainingExample
{
	public double Input { get; }
	public double Target { get; }

	public TrainingExample(double input, double target)
	{
		Input = input;
		Target = target;
	}
}
