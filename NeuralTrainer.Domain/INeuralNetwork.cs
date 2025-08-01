namespace NeuralTrainer.Domain;

/// <summary>
/// Interface for neural networks that can be trained.
/// </summary>
public interface INeuralNetwork
{
	/// <summary>
	/// The number of inputs accepted in this layer.
	/// </summary>
	int InputSize { get; }

	/// <summary>
	/// Perform forward propagation.
	/// </summary>
	double Forward(IReadOnlyList<double> inputs);

	/// <summary>
	/// Update the network parameters.
	/// </summary>
	/// <remarks>
	/// Note that external entities are not allowed to directly modify the data.
	/// </remarks>
	void UpdateParameters(IReadOnlyList<double> weightDeltas, double biasDelta);

	(IReadOnlyList<double> weightGradients, double biasGradient) CalculateGradients(IReadOnlyList<double> inputs, double outputGradient);
}
