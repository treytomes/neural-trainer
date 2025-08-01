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

	private IList<INeuron> _neurons;

	#endregion

	#region Constructors

	public NeuralNetwork(IActivationFunction activationFunction, IWeightInitializer weightInitializer)
		: this(1, activationFunction, weightInitializer)
	{
	}

	public NeuralNetwork(int inputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
		: this(inputSize, 1, activationFunction, weightInitializer)
	{
	}

	public NeuralNetwork(int inputSize, int outputSize, IActivationFunction activationFunction, IWeightInitializer weightInitializer)
	{
		InputSize = inputSize;
		OutputSize = outputSize;

		_neurons = new List<INeuron>(outputSize);
		for (int i = 0; i < outputSize; i++)
		{
			_neurons.Add(new Neuron(inputSize, activationFunction, weightInitializer));
		}
	}

	#endregion

	#region Properties

	public int InputSize { get; }
	public int OutputSize { get; }

	#endregion

	#region Methods

	public IReadOnlyList<double> Forward(IReadOnlyList<double> inputs)
	{
		var outputs = new double[OutputSize];
		for (int i = 0; i < OutputSize; i++)
		{
			outputs[i] = _neurons[i].Forward(inputs);
		}
		return outputs;
	}

	public void UpdateParameters(IReadOnlyList<IReadOnlyList<double>> weightDeltas, IReadOnlyList<double> biasDeltas)
	{
		for (int i = 0; i < OutputSize; i++)
		{
			_neurons[i].UpdateParameters(weightDeltas[i], biasDeltas[i]);
		}
	}

	public IReadOnlyList<(IReadOnlyList<double> weightGradients, double biasGradient)> CalculateGradients(IReadOnlyList<double> inputs, IReadOnlyList<double> outputGradients)
	{
		var gradients = new (IReadOnlyList<double> weightGradients, double biasGradient)[OutputSize];
		for (int i = 0; i < OutputSize; i++)
		{
			gradients[i] = _neurons[i].CalculateGradients(inputs, outputGradients[i]);
		}
		return gradients;
	}

	#endregion
}
