namespace NeuralTrainer.Domain.ActivationFunctions;

public class ReLUActivationFunction : IActivationFunction
{
	public double Activate(double input)
	{
		return Math.Max(0, input);
	}

	public double Derivative(double output)
	{
		// Note: This assumes we know if the original input was positive
		// In practice, ReLU might need the original input, not just output
		return output > 0 ? 1 : 0;
	}
}
