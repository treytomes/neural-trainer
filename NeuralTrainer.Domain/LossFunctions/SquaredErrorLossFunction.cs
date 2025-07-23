namespace NeuralTrainer.Domain.LossFunctions;

/// <summary>
/// Mean squared error loss function.
/// </summary>
public class SquaredErrorLossFunction : ILossFunction
{
	public double Calculate(double predicted, double target)
	{
		var error = target - predicted;
		return error * error;
	}

	public double Derivative(double predicted, double target)
	{
		// Derivative of squared error: -2(target - predicted).
		// We often omit the factor of 2 as it's absorbed by learning rate.
		return -(target - predicted);
	}
}
