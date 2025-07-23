using NeuralTrainer.Domain.ActivationFunctions;

namespace NeuralTrainer.Domain;

/// <summary>
/// Interface for neural networks that can be trained.
/// </summary>
public interface INeuralNetwork
{
	/// <summary>
	/// Perform forward propagation.
	/// </summary>
	double Forward(double input);

	/// <summary>
	/// Get the current weight value.
	/// </summary>
	double Weight { get; }

	/// <summary>
	/// Get the current bias value.
	/// </summary>
	double Bias { get; }

	/// <summary>
	/// Update the network parameters.
	/// </summary>
	/// <remarks>
	/// Note that external entities are not allowed to directly modify the data.
	/// </remarks>
	void UpdateParameters(double weightDelta, double biasDelta);

	/// <summary>
	/// Get the activation function used by the network.
	/// </summary>
	IActivationFunction ActivationFunction { get; }
}
