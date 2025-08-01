using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Domain;

/// <inheritdoc />
public class Neuron : INeuron
{
	#region Fields

	private readonly List<double> _weights = new();
	private double _bias = 0.0;
	private IActivationFunction _activationFunction;

	private double _lastOutput; // Cache for gradient calculation
	private double _lastPreActivation;

	#endregion

	#region Constructors

	public Neuron(int inputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
	{
		_weights = Enumerable.Range(0, inputSize).Select(x => weightInitializer.InitializeWeight()).ToList();
		_bias = weightInitializer.InitializeBias();
		_activationFunction = activationFunction;
	}

	#endregion

	#region Properties

	public IReadOnlyList<double> Weights => _weights.AsReadOnly();
	public double Bias => _bias;

	#endregion

	#region Methods

	public double Forward(IReadOnlyList<double> inputs)
	{
		if (inputs.Count != _weights.Count) throw new ArgumentException("Input size mismatch.");

		_lastPreActivation = _bias;
		for (var i = 0; i < inputs.Count; i++)
		{
			_lastPreActivation += inputs[i] * _weights[i];
		}
		_lastOutput = _activationFunction.Activate(_lastPreActivation);
		return _lastOutput;
	}

	public (IReadOnlyList<double> weightGradients, double biasGradient) CalculateGradients(IReadOnlyList<double> inputs, double outputGradient)
	{
		var activationDerivative = _activationFunction.Derivative(_lastOutput);
		var neuronGradient = outputGradient * activationDerivative;

		var weightGradients = new double[inputs.Count];
		for (int i = 0; i < inputs.Count; i++)
		{
			weightGradients[i] = neuronGradient * inputs[i];
		}

		return (weightGradients, neuronGradient);
	}

	public void UpdateParameters(IReadOnlyList<double> weightDeltas, double biasDelta)
	{
		if (weightDeltas.Count != _weights.Count) throw new ArgumentException("Weight size mismatch.");

		for (var i = 0; i < _weights.Count; i++)
		{
			_weights[i] += weightDeltas[i];
		}
		_bias += biasDelta;
	}

	#endregion
}
