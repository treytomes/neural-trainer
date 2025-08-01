namespace NeuralTrainer.Domain.LossFunctions;

/// <summary>
/// Interface for loss/cost functions
/// </summary>
public interface ILossFunction
{
	/// <summary>
	/// Calculate the loss between predicted and target values
	/// </summary>
	double Calculate(IReadOnlyList<double> predicted, IReadOnlyList<double> actual);

	/// <summary>
	/// Calculate the derivative of the loss with respect to the predicted value
	/// </summary>
	IReadOnlyList<double> Derivative(IReadOnlyList<double> predicted, IReadOnlyList<double> actual);
}
