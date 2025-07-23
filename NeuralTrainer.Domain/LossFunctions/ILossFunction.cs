namespace NeuralTrainer.Domain.LossFunctions;

/// <summary>
/// Interface for loss/cost functions
/// </summary>
public interface ILossFunction
{
	/// <summary>
	/// Calculate the loss between predicted and target values
	/// </summary>
	double Calculate(double predicted, double target);

	/// <summary>
	/// Calculate the derivative of the loss with respect to the predicted value
	/// </summary>
	double Derivative(double predicted, double target);
}
