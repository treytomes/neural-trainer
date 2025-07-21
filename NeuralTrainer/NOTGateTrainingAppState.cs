using NeuralTrainer.Domain;

namespace NeuralTrainer;

class NOTGateTrainingAppState : IAppState
{
	public void Run()
	{
		Console.WriteLine("Training Neural Network to learn NOT gate...\n");

		// Create training data for NOT gate
		var trainingData = new[]
		{
			new TrainingExample(0, 1), // 0 -> 1
			new TrainingExample(1, 0)  // 1 -> 0
		};

		// Create and train the network
		var network = new NeuralNetwork(learningRate: 1);
		network.Train(trainingData, epochs: 100000);

		// Test the trained network
		Console.WriteLine("\nTesting trained network:");
		Console.WriteLine($"Input: 0, Output: {network.Forward(0):F4}, Expected: 1");
		Console.WriteLine($"Input: 1, Output: {network.Forward(1):F4}, Expected: 0");

		// Test with intermediate values
		Console.WriteLine("\nTesting with intermediate values:");
		for (double x = 0; x <= 1; x += 0.1)
		{
			Console.WriteLine($"Input: {x:F1}, Output: {network.Forward(x):F4}");
		}
	}
}
