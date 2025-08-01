using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer;

// Note: "Primary constructor"
class BinaryGateTrainingAppState(IActivationFunction activationFunction, IWeightInitializer weightInitializer, ILossFunction lossFunction, IProgressReporter progressReporter) : IAppState
{
	#region Methods

	public void Run()
	{
		// Create and train the network.
		var andNetwork = TrainGate("AND",
		[
			new TrainingExample([0, 0], [0]),
			new TrainingExample([0, 1], [0]),
			new TrainingExample([1, 0], [0]),
			new TrainingExample([1, 1], [1]),
		]);

		// Create and train the network.
		var nandNetwork = TrainGate("NAND",
		[
			new TrainingExample([0, 0], [1]),
			new TrainingExample([0, 1], [1]),
			new TrainingExample([1, 0], [1]),
			new TrainingExample([1, 1], [0]),
		]);

		// Create and train the network.
		var orNetwork = TrainGate("OR",
		[
			new TrainingExample([0, 0], [0]),
			new TrainingExample([0, 1], [1]),
			new TrainingExample([1, 0], [1]),
			new TrainingExample([1, 1], [1]),
		]);

		// Create and train the network.
		var norNetwork = TrainGate("NOR",
		[
			new TrainingExample([0, 0], [1]),
			new TrainingExample([0, 1], [0]),
			new TrainingExample([1, 0], [0]),
			new TrainingExample([1, 1], [0]),
		]);

		// Note: The following 2 networks will not converge due to their non-linearity.

		// Create and train the network.
		var xorNetwork = TrainGate("XOR",
		[
			new TrainingExample([0, 0], [0]),
			new TrainingExample([0, 1], [1]),
			new TrainingExample([1, 0], [1]),
			new TrainingExample([1, 1], [0]),
		]);

		// Create and train the network.
		var xnorNetwork = TrainGate("XNOR",
		[
			new TrainingExample([0, 0], [1]),
			new TrainingExample([0, 1], [0]),
			new TrainingExample([1, 0], [0]),
			new TrainingExample([1, 1], [1]),
		]);
	}

	/// <summary>
	/// Create and train a neural network.
	/// </summary>
	/// <param name="gateName">Name used for reporting.</param>
	/// <param name="trainingData"></param>
	/// <returns>The completed network.</returns>
	private INeuralNetwork TrainGate(string gateName, IEnumerable<TrainingExample> trainingData)
	{
		Console.WriteLine();
		Console.WriteLine("==========================================");
		Console.WriteLine($"Training Neural Network to learn {gateName} gate...");
		Console.WriteLine();

		var network = new NeuralNetwork(2, activationFunction, weightInitializer);

		var trainer = new GradientDescentTrainer(
			learningRate: 0.1,
			lossFunction: lossFunction,
			progressReporter: progressReporter);

		trainer.Train(network, trainingData, epochs: 10000);

		// Test the trained network.
		Console.WriteLine();
		Console.WriteLine("Testing trained network:");
		foreach (var example in trainingData)
		{
			Console.WriteLine($"{example}, actual: {string.Join(',', network.Forward(example.Inputs))}");
		}

		Console.WriteLine("==========================================");
		Console.WriteLine();

		return network;
	}

	#endregion
}
