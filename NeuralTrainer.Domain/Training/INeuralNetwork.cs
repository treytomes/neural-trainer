namespace NeuralTrainer.Domain;

public interface INeuralNetwork
{
	int InputSize { get; }
	int OutputSize { get; }
	IReadOnlyList<double> Forward(IReadOnlyList<double> inputs);

	// Keep these for backward compatibility, but they might throw for multi-layer networks
	void UpdateParameters(IReadOnlyList<IReadOnlyList<double>> weightDeltas, IReadOnlyList<double> biasDeltas);
	IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)> CalculateGradients(
		IReadOnlyList<double> inputs,
		IReadOnlyList<double> outputGradients);

	// New methods for multi-layer support
	IList<IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)>> Backpropagate(
		IReadOnlyList<double> inputs,
		IReadOnlyList<double> outputGradients);
	void UpdateAllLayers(IList<IReadOnlyList<(IReadOnlyList<double> weightDeltas, double biasDelta)>> layerUpdates);
}
