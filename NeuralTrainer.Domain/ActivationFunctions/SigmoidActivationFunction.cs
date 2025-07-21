namespace NeuralTrainer.Domain.ActivationFunctions;

// Single Responsibility Principle:
//
// The activation function has been extracted, and can now be easily replaced with
// other activation functions.
//
// The neural network class now has 1 less thing that it's responsible for,
// but the refactoring is not done.
//
// Also, this refactoring has hurt our test coverage score! (oh no!)
//

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
