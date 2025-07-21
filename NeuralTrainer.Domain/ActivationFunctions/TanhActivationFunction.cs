namespace NeuralTrainer.Domain.ActivationFunctions;

public class TanhActivationFunction : IActivationFunction
{
	public double Activate(double input)
	{
		return Math.Tanh(input);
	}

	public double Derivative(double output)
	{
		// Derivative of tanh when given tanh output
		return 1 - output * output;
	}
}
