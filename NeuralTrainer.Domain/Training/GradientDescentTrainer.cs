using NeuralTrainer.Domain.LossFunctions;

namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Implements stochastic gradient descent training algorithm.
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
				var output = network.Forward(example.Inputs);

				// Calculate loss
				var loss = _lossFunction.Calculate(output, example.Target);
				totalLoss += loss;

				// Calculate error gradient
				var errorGradient = _lossFunction.Derivative(output, example.Target);

				// Get gradients from the network
				var (weightGradients, biasGradient) = network.CalculateGradients(example.Inputs, errorGradient);

				// Scale by learning rate
				var weightDeltas = weightGradients.Select(g => _learningRate * g).ToList();
				var biasDelta = _learningRate * biasGradient;

				// Update parameters
				network.UpdateParameters(weightDeltas, biasDelta);
			}

			_progressReporter.ReportProgress(epoch, totalLoss / examples.Count());
		}
	}

	#endregion
}
