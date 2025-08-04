namespace NeuralTrainer.Domain;

/// <summary>
/// Interface for neural networks that can be trained.
/// </summary>
public interface ILayer
{
	/// <summary>
	/// The number of inputs accepted in this layer.
	/// </summary>
	int InputSize { get; }

	int OutputSize { get; }

	/// <summary>
	/// Perform forward propagation.
	/// </summary>
	IReadOnlyList<double> Forward(IReadOnlyList<double> inputs);

	/// <summary>
	/// Update the network parameters.
	/// </summary>
	/// <remarks>
	/// Note that external entities are not allowed to directly modify the data.
	/// </remarks>
	void UpdateParameters(IReadOnlyList<IReadOnlyList<double>> weightDeltas, IReadOnlyList<double> biasDeltas);

	IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)> CalculateGradients(IReadOnlyList<double> inputs, IReadOnlyList<double> outputGradients);
}
