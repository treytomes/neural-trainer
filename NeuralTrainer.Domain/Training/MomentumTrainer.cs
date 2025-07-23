using NeuralTrainer.Domain.LossFunctions;

namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Implements gradient descent with momentum for neural network training.
/// </summary>
/// <remarks>
/// Momentum accelerates training by accumulating a velocity vector in directions of persistent gradient,
/// helping to speed up convergence and escape local minima. This optimizer is particularly beneficial for:
/// <list type="bullet">
///   <item>Training deeper or more complex networks</item>
///   <item>Navigating ravines or plateaus in the error surface</item>
///   <item>Reducing oscillations during training</item>
///   <item>Achieving faster convergence than standard gradient descent</item>
/// </list>
/// Typical momentum values range from 0.8 to 0.99, with 0.9 being a common starting point.
/// May not be necessary for very simple problems where standard gradient descent converges quickly.
/// </remarks>
public class MomentumTrainer : ITrainer
{
	#region Fields

	private readonly double _learningRate;
	private readonly double _momentum;
	private readonly ILossFunction _lossFunction;
	private readonly IProgressReporter _progressReporter;

	private double _previousWeightDelta = 0;
	private double _previousBiasDelta = 0;

	#endregion

	#region Constructors

	public MomentumTrainer(double learningRate, double momentum, ILossFunction lossFunction, IProgressReporter progressReporter)
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
		_momentum = momentum;
		_lossFunction = lossFunction;
		_progressReporter = progressReporter;
	}

	#endregion

	#region Methods

	public void Train(INeuralNetwork network, TrainingExample[] examples, int epochs)
	{
		for (var epoch = 0; epoch < epochs; epoch++)
		{
			var totalLoss = 0.0;

			foreach (var example in examples)
			{
				var output = network.Forward(example.Input);
				var loss = _lossFunction.Calculate(output, example.Target);
				totalLoss += loss;

				var errorGradient = _lossFunction.Derivative(output, example.Target);
				var outputGradient = errorGradient * network.ActivationFunction.Derivative(output);

				// Calculate updates with momentum
				var weightDelta = _learningRate * outputGradient * example.Input + _momentum * _previousWeightDelta;
				var biasDelta = _learningRate * outputGradient + _momentum * _previousBiasDelta;

				network.UpdateParameters(weightDelta, biasDelta);

				_previousWeightDelta = weightDelta;
				_previousBiasDelta = biasDelta;
			}

			_progressReporter.ReportProgress(epoch, totalLoss / examples.Length);
		}
	}

	#endregion
}
