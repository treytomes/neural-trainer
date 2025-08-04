using NeuralTrainer.Domain.LossFunctions;

namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Implements stochastic gradient descent training algorithm with support for multi-layer networks.
/// </summary>
public class GradientDescentTrainer : ITrainer
{
	#region Fields

	private readonly double _learningRate;
	private readonly ILossFunction _lossFunction;
	private readonly IProgressReporter _progressReporter;

	#endregion

	#region Constructors

	public GradientDescentTrainer(double learningRate, ILossFunction lossFunction, IProgressReporter progressReporter)
	{
		if (double.IsNaN(learningRate) || double.IsInfinity(learningRate))
		{
			throw new ArgumentOutOfRangeException(nameof(learningRate), "Learning rate must be a finite number.");
		}
		if (learningRate <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(learningRate), "Learning rate must be positive.");
		}
		if (learningRate > 1)
		{
			throw new ArgumentOutOfRangeException(nameof(learningRate), "Learning rate should not exceed 1 for stable training.");
		}

		_learningRate = learningRate;
		_lossFunction = lossFunction ?? throw new ArgumentNullException(nameof(lossFunction));
		_progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
	}

	#endregion

	#region Methods

	public void Train(INeuralNetwork network, IEnumerable<TrainingExample> examples, int epochs)
	{
		if (epochs <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(epochs), "Number of epochs must be positive.");
		}

		for (var epoch = 0; epoch < epochs; epoch++)
		{
			var totalLoss = 0.0;

			foreach (var example in examples)
			{
				// Forward pass
				var outputs = network.Forward(example.Inputs);

				// Calculate loss
				var loss = _lossFunction.Calculate(outputs, example.Targets);
				totalLoss += loss;

				// Calculate error gradients for each output
				var errorGradients = _lossFunction.Derivative(outputs, example.Targets);

				// Backpropagate to get gradients for all layers
				var layerGradients = network.Backpropagate(example.Inputs, errorGradients);

				// Prepare updates for all layers
				var layerUpdates = new List<IReadOnlyList<(IReadOnlyList<double> weightDeltas, double biasDelta)>>();

				foreach (var layerGradient in layerGradients)
				{
					var updates = new List<(IReadOnlyList<double> weightDeltas, double biasDelta)>();

					foreach (var (weightGradients, biasGradient) in layerGradient)
					{
						// Scale by learning rate
						var weightDeltas = weightGradients.Select(g => _learningRate * g).ToList();
						var biasDelta = _learningRate * biasGradient;

						updates.Add((weightDeltas, biasDelta));
					}

					layerUpdates.Add(updates);
				}

				// Update all layers
				network.UpdateAllLayers(layerUpdates);
			}

			_progressReporter.ReportProgress(epoch, totalLoss / examples.Count());
		}
	}

	#endregion
}
