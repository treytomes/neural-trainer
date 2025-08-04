namespace NeuralTrainer.Domain.WeightInitializers;

public class XavierInitializer : IWeightInitializer
{
	private readonly Random _random;

	public XavierInitializer(Random? random = null)
	{
		_random = random ?? new Random();
	}

	public double InitializeWeight(int inputSize, int outputSize)
	{
		// Xavier initialization for sigmoid: sqrt(2 / (fan_in + fan_out))
		double limit = Math.Sqrt(2.0 / (inputSize + outputSize));
		return (_random.NextDouble() * 2 - 1) * limit;
	}

	public double InitializeBias()
	{
		return 0.0;
	}
}
