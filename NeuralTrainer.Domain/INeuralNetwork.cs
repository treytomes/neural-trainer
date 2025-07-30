using NeuralTrainer.Domain.ActivationFunctions;

namespace NeuralTrainer.Domain;

/// <summary>
/// Interface for neural networks that can be trained.
/// </summary>
public interface INeuralNetwork
{
	/// <summary>
	/// Get the activation function used by the network.
	/// </summary>
	IActivationFunction ActivationFunction { get; }

	/// <summary>
	/// Get the current bias value.
	/// </summary>
	double Bias { get; }

	/// <summary>
	/// Get the current weight values.
	/// </summary>
	IReadOnlyList<double> Weights { get; }

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
}
