namespace NeuralTrainer.Domain.LossFunctions;

/// <summary>
/// Mean squared error loss function.
/// </summary>
public class SquaredErrorLossFunction : ILossFunction
{
	public double Calculate(IReadOnlyList<double> predicted, IReadOnlyList<double> actual)
	{
		if (predicted == null) throw new ArgumentNullException(nameof(predicted));
		if (actual == null) throw new ArgumentNullException(nameof(actual));
		if (predicted.Count != actual.Count)
			throw new ArgumentException("Predicted and actual must have the same number of elements.");

		var totalError = 0.0;
		for (int i = 0; i < predicted.Count; i++)
		{
			var error = actual[i] - predicted[i];
			totalError += error * error;
		}

		// Return mean squared error
		return totalError / predicted.Count;
	}

	public IReadOnlyList<double> Derivative(IReadOnlyList<double> predicted, IReadOnlyList<double> actual)
	{
		if (predicted == null) throw new ArgumentNullException(nameof(predicted));
		if (actual == null) throw new ArgumentNullException(nameof(actual));
		if (predicted.Count != actual.Count)
			throw new ArgumentException("Predicted and actual must have the same number of elements.");

		var derivatives = new double[predicted.Count];
		for (int i = 0; i < predicted.Count; i++)
		{
			// Derivative of squared error: -2(actual - predicted).
			// We often omit the factor of 2 as it's absorbed by learning rate.
			// Also dividing by Count for the mean.
			derivatives[i] = (actual[i] - predicted[i]) / predicted.Count;
		}

		return derivatives;
	}
}
