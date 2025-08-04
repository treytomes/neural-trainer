using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Domain;

public class NeuralNetwork : INeuralNetwork
{
	#region Fields

	private readonly IList<ILayer> _layers;

	#endregion

	#region Constructors

	public NeuralNetwork(IList<ILayer> layers)
	{
		if (layers == null || layers.Count == 0)
			throw new ArgumentException("Network must have at least one layer.", nameof(layers));

		// Validate that layers are compatible
		for (int i = 1; i < layers.Count; i++)
		{
			if (layers[i].InputSize != layers[i - 1].OutputSize)
			{
				throw new ArgumentException(
					$"Layer {i} input size ({layers[i].InputSize}) doesn't match previous layer output size ({layers[i - 1].OutputSize}).",
					nameof(layers));
			}
		}

		_layers = layers;
		InputSize = layers[0].InputSize;
		OutputSize = layers[layers.Count - 1].OutputSize;
	}

	// Convenience constructor for creating a network with uniform activation and initialization
	public NeuralNetwork(int[] layerSizes, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
	{
		if (layerSizes == null || layerSizes.Length < 2)
			throw new ArgumentException("Must specify at least input and output layer sizes.", nameof(layerSizes));

		_layers = new List<ILayer>(layerSizes.Length - 1);

		for (int i = 0; i < layerSizes.Length - 1; i++)
		{
			_layers.Add(new DenseLayer(
				layerSizes[i],
				layerSizes[i + 1],
				activationFunction,
				weightInitializer));
		}

		InputSize = layerSizes[0];
		OutputSize = layerSizes[layerSizes.Length - 1];
	}

	public NeuralNetwork(int inputSize, int outputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
		: this([inputSize, outputSize], activationFunction, weightInitializer)
	{
	}

	public NeuralNetwork(int inputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
		: this(inputSize, 1, activationFunction, weightInitializer)
	{
	}

	#endregion

	#region Properties

	public int InputSize { get; }
	public int OutputSize { get; }
	public int LayerCount => _layers.Count;

	#endregion

	#region Methods

	public IReadOnlyList<double> Forward(IReadOnlyList<double> inputs)
	{
		var currentOutput = inputs;

		foreach (var layer in _layers)
		{
			currentOutput = layer.Forward(currentOutput);
		}

		return currentOutput;
	}

	public void UpdateParameters(IReadOnlyList<IReadOnlyList<double>> weightDeltas, IReadOnlyList<double> biasDeltas)
	{
		// This method signature needs to change to support multiple layers
		// For now, this only works with single-layer networks
		if (_layers.Count == 1)
		{
			_layers[0].UpdateParameters(weightDeltas, biasDeltas);
		}
		else
		{
			throw new NotImplementedException("Multi-layer parameter updates require a different approach.");
		}
	}

	public IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)> CalculateGradients(IReadOnlyList<double> inputs, IReadOnlyList<double> outputGradients)
	{
		// This method needs to implement backpropagation through all layers
		// For now, this only works with single-layer networks
		if (_layers.Count == 1)
		{
			return _layers[0].CalculateGradients(inputs, outputGradients);
		}
		else
		{
			throw new NotImplementedException("Multi-layer gradient calculation requires backpropagation implementation.");
		}
	}

	// New method to support full backpropagation
	public IList<IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)>> Backpropagate(
		IReadOnlyList<double> inputs,
		IReadOnlyList<double> outputGradients)
	{
		var layerGradients = new List<IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)>>();
		var layerInputs = new List<IReadOnlyList<double>>();

		// Forward pass to collect all layer inputs
		var currentInput = inputs;
		layerInputs.Add(currentInput);

		foreach (var layer in _layers)
		{
			currentInput = layer.Forward(currentInput);
			layerInputs.Add(currentInput);
		}

		// Backward pass
		var currentGradients = outputGradients;

		// Process layers in reverse order
		for (int i = _layers.Count - 1; i >= 0; i--)
		{
			var layer = _layers[i];
			var layerInput = layerInputs[i];

			// Calculate gradients for this layer
			var gradients = layer.CalculateGradients(layerInput, currentGradients);
			layerGradients.Insert(0, gradients); // Insert at beginning to maintain order

			// Calculate gradients to propagate to previous layer
			if (i > 0)
			{
				var nextLayerGradients = new double[layer.InputSize];

				// For each input to this layer
				for (int j = 0; j < layer.InputSize; j++)
				{
					double sum = 0.0;

					// Sum contributions from all neurons in this layer
					for (int k = 0; k < gradients.Count; k++)
					{
						sum += currentGradients[k] * gradients[k].weightGradients[j];
					}

					nextLayerGradients[j] = sum;
				}

				currentGradients = nextLayerGradients;
			}
		}

		return layerGradients;
	}

	// New method to update all layers
	public void UpdateAllLayers(IList<IReadOnlyList<(IReadOnlyList<double> weightDeltas, double biasDelta)>> layerUpdates)
	{
		if (layerUpdates.Count != _layers.Count)
			throw new ArgumentException("Number of layer updates must match number of layers.");

		for (int i = 0; i < _layers.Count; i++)
		{
			var updates = layerUpdates[i];
			var weightDeltasList = new List<IReadOnlyList<double>>(updates.Count);
			var biasDeltasList = new List<double>(updates.Count);

			foreach (var (weightDeltas, biasDelta) in updates)
			{
				weightDeltasList.Add(weightDeltas);
				biasDeltasList.Add(biasDelta);
			}

			_layers[i].UpdateParameters(weightDeltasList, biasDeltasList);
		}
	}

	#endregion
}
