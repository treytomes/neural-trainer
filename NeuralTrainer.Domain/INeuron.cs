namespace NeuralTrainer.Domain;

/// <summary>
/// A neuron has multiple input weights, a bias, an activation function, and a single output.
/// </summary>
public interface INeuron
{
	// Note: You don't need regions if the entire type is < ~20 lines.
	IReadOnlyList<double> Weights { get; }
	double Bias { get; }

	double Forward(IReadOnlyList<double> inputs);
	void UpdateParameters(IReadOnlyList<double> weightDeltas, double biasDelta);
	(IReadOnlyList<double> weightGradients, double biasGradient) CalculateGradients(IReadOnlyList<double> inputs, double outputGradient);
}
