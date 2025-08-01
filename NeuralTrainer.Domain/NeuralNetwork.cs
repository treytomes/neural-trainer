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
// TODO: - (half done...) Make network architecture configurable (layers, neurons) without modifying this class
// TODO: - Allow different optimization algorithms without changing Train method
// TODO: - (done) Support different loss functions without modifying the training loop

// TODO: Liskov Substitution Principle (LSP) preparations:
// TODO: - Define base abstractions (INeuralNetwork) that derived types can properly implement
// TODO: - Ensure any future network types can be substituted without breaking behavior

// TODO: Interface Segregation Principle (ISP) considerations:
// TODO: - Create focused interfaces: IForwardPropagation, ITrainable, IPredictable
// TODO: - Clients shouldn't be forced to depend on methods they don't use

// TODO: Dependency Inversion Principle (DIP) fixes:
// TODO: - Inject Random instance or IRandom interface instead of creating internally
// TODO: - Remove direct Console.WriteLine dependency - inject ILogger or IProgressReporter
// TODO: - Constructor should depend on abstractions, not create concrete implementations

// TODO: Additional refactoring for clean architecture:
// TODO: - Make weight and bias immutable after training (or provide read-only access)
// TODO: - Add parameter validation for Train method (null checks, epochs > 0)
// TODO: - Consider making this class focus only on inference, not training
// TODO: - Extract hyperparameters (learning rate) into a configuration object
// TODO: - Add proper abstraction for network state persistence/loading

public class NeuralNetwork : INeuralNetwork
{
	#region Fields

	private INeuron _neuron;

	#endregion

	#region Constructors

	public NeuralNetwork(int inputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
	{
		InputSize = inputSize;
		_neuron = new Neuron(inputSize, activationFunction, weightInitializer);
	}

	#endregion

	#region Properties

	public int InputSize { get; }

	#endregion

	#region Methods

	public double Forward(IReadOnlyList<double> inputs)
	{
		return _neuron.Forward(inputs);
	}

	public void UpdateParameters(IReadOnlyList<double> weightDeltas, double biasDelta)
	{
		_neuron.UpdateParameters(weightDeltas, biasDelta);
	}

	public (IReadOnlyList<double> weightGradients, double biasGradient) CalculateGradients(IReadOnlyList<double> inputs, double outputGradient)
	{
		return _neuron.CalculateGradients(inputs, outputGradient);
	}

	#endregion
}
