namespace NeuralTrainer.Domain.WeightInitializers;

/// <summary>
/// Interface for weight initialization strategies in neural networks.
/// </summary>
public interface IWeightInitializer
{
	/// <summary>
	/// Initialize weights with a specific strategy.
	/// </summary>
	/// <param name="fanIn">Number of input connections to the neuron.</param>
	/// <param name="fanOut">Number of output connections from the neuron.</param>
	/// <returns>Initialized weight value.</returns>
	double InitializeWeight(int fanIn = 1, int fanOut = 1);

	/// <summary>
	/// Initialize bias with a specific strategy.
	/// </summary>
	/// <returns>Initialized bias value.</returns>
	double InitializeBias();
}
