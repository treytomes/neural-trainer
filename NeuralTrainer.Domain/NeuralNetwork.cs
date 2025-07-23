using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Domain;


// TODO: Single Responsibility Principle (SRP) violations to fix:
// TODO: - (done) Extract activation function (Sigmoid/SigmoidDerivative) into IActivationFunction interface
// TODO: - (done) Extract weight initialization logic into IWeightInitializer interface
// TODO: - Extract training/backpropagation logic into separate ITrainer or IOptimizer class
// TODO: - Extract loss calculation into ILossFunction interface
// TODO: - Extract progress reporting (Console.WriteLine) into IProgressReporter interface

// TODO: Open/Closed Principle (OCP) improvements:
// TODO: - Make network architecture configurable (layers, neurons) without modifying this class
// TODO: - Allow different optimization algorithms without changing Train method
// TODO: - Support different loss functions without modifying the training loop

// TODO: Dependency Inversion Principle (DIP) fixes:
// TODO: - Inject Random instance or IRandom interface instead of creating internally
// TODO: - Remove direct Console.WriteLine dependency - inject ILogger or IProgressReporter
// TODO: - Constructor should depend on abstractions, not create concrete implementations

// TODO: Interface Segregation Principle (ISP) considerations:
// TODO: - Create focused interfaces: IForwardPropagation, ITrainable, IPredictable
// TODO: - Clients shouldn't be forced to depend on methods they don't use

// TODO: Liskov Substitution Principle (LSP) preparations:
// TODO: - Define base abstractions (INeuralNetwork) that derived types can properly implement
// TODO: - Ensure any future network types can be substituted without breaking behavior

// TODO: Additional refactoring for clean architecture:
// TODO: - Make weight and bias immutable after training (or provide read-only access)
// TODO: - Add parameter validation for Train method (null checks, epochs > 0)
// TODO: - Consider making this class focus only on inference, not training
// TODO: - Extract hyperparameters (learning rate) into a configuration object
// TODO: - Add proper abstraction for network state persistence/loading


public class NeuralNetwork
{
	#region Fields

	private double _weight;
	private double _bias;
	private readonly double _learningRate;
	private readonly IActivationFunction _activationFunction;

	#endregion

	#region Constructors

	public NeuralNetwork(double learningRate, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
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
		_activationFunction = activationFunction;

		_weight = weightInitializer.InitializeWeight();
		_bias = weightInitializer.InitializeBias();
	}

	#endregion

	#region Methods

	public double Forward(double input)
	{
		var z = input * _weight + _bias;
		return _activationFunction.Activate(z);
	}

	public void Train(TrainingExample[] examples, int epochs)
	{
		for (var epoch = 0; epoch < epochs; epoch++)
		{
			var totalLoss = 0.0;

			foreach (var example in examples)
			{
				// Forward pass
				var output = Forward(example.Input);

				// Calculate loss (squared error)
				var error = example.Target - output;
				totalLoss += error * error;

				// Backpropagation
				var outputGradient = error * _activationFunction.Derivative(output);

				// Update weights and bias
				_weight += _learningRate * outputGradient * example.Input;
				_bias += _learningRate * outputGradient;
			}

			// Print progress every 1000 epochs
			if (epoch % 1000 == 0)
			{
				Console.WriteLine($"Epoch {epoch}, Loss: {totalLoss / examples.Length:F4}");
			}
		}
	}

	#endregion
}
