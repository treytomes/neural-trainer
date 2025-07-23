using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Domain;

// TODO: Single Responsibility Principle (SRP) violations to fix:
// TODO: - (done) Extract activation function (Sigmoid/SigmoidDerivative) into IActivationFunction interface
// TODO: - (done) Extract weight initialization logic into IWeightInitializer interface
// TODO: - (done) Extract loss calculation into ILossFunction interface
// TODO: - (done) Extract training/backpropagation logic into separate ITrainer or IOptimizer class
// TODO: - (done) Extract progress reporting (Console.WriteLine) into IProgressReporter interface

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


public class NeuralNetwork : INeuralNetwork
{
	#region Fields

	private double _weight;
	private double _bias;

	#endregion

	#region Constructors

	public NeuralNetwork(IActivationFunction activationFunction, IWeightInitializer weightInitializer)
	{
		_weight = weightInitializer.InitializeWeight();
		_bias = weightInitializer.InitializeBias();
		ActivationFunction = activationFunction;
	}

	#endregion

	#region Properties

	public double Weight => _weight;
	public double Bias => _bias;
	public IActivationFunction ActivationFunction { get; }

	#endregion

	#region Methods

	public double Forward(double input)
	{
		var z = input * _weight + _bias;
		return ActivationFunction.Activate(z);
	}

	public void UpdateParameters(double weightDelta, double biasDelta)
	{
		_weight += weightDelta;
		_bias += biasDelta;
	}

	#endregion
}
