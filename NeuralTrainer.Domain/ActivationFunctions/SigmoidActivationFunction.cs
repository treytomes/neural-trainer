namespace NeuralTrainer.Domain.ActivationFunctions;

public class SigmoidActivationFunction : IActivationFunction
{
	public double Activate(double input)
	{
		return 1.0 / (1.0 + Math.Exp(-input));
	}

	public double Derivative(double output)
	{
		// Derivative of sigmoid when given sigmoid output
		return output * (1 - output);
	}
}
