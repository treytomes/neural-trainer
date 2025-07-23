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

	public void Train(INeuralNetwork network, TrainingExample[] examples, int epochs)
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
				// Forward pass.
				var output = network.Forward(example.Input);

				// Calculate loss.
				var loss = _lossFunction.Calculate(output, example.Target);
				totalLoss += loss;

				// Calculate error gradient.
				var errorGradient = _lossFunction.Derivative(output, example.Target);

				// Backpropagation.
				var outputGradient = errorGradient * network.ActivationFunction.Derivative(output);

				// Calculate parameter updates.
				var weightDelta = _learningRate * outputGradient * example.Input;
				var biasDelta = _learningRate * outputGradient;

				// Update weights and bias.
				network.UpdateParameters(weightDelta, biasDelta);
			}

			// Report progress.
			_progressReporter.ReportProgress(epoch, totalLoss / examples.Length);
		}
	}

	#endregion
}
