using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer;

// Note: "Primary constructor"
class NOTGateTrainingAppState(IActivationFunction activationFunction, IWeightInitializer weightInitializer, ILossFunction lossFunction, IProgressReporter progressReporter) : IAppState
{
	#region Methods

	public void Run()
	{
		Console.WriteLine("Training Neural Network to learn NOT gate...\n");

		// Create training data for NOT gate
		var trainingData = new[]
		{
			// TODO: In a more complete example, the inputs and outputs would be vectors.
			new TrainingExample([0], 1),
			new TrainingExample([1], 0)
		};

		// Create and train the network.

		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine("==========================================");

		var network = new NeuralNetwork(1, activationFunction, weightInitializer);

		var trainer = new GradientDescentTrainer(
			learningRate: 0.1,
			lossFunction: lossFunction,
			progressReporter: progressReporter);

		trainer.Train(network, trainingData, epochs: 10000);

		// Test the trained network
		Console.WriteLine("\nTesting trained network:");
		Console.WriteLine($"Input: 0, Output: {network.Forward([0]):F4}, Expected: 1");
		Console.WriteLine($"Input: 1, Output: {network.Forward([1]):F4}, Expected: 0");

		// Test with intermediate values
		Console.WriteLine("\nTesting with intermediate values:");
		for (var x = 0.0; x <= 1.0; x += 0.1)
		{
			Console.WriteLine($"Input: {x:F1}, Output: {network.Forward([x]):F4}");
		}
		Console.WriteLine("==========================================");
		Console.WriteLine();
		Console.WriteLine();
	}

	#endregion
}
