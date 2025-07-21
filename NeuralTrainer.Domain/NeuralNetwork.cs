namespace NeuralTrainer.Domain;

public class NeuralNetwork
{
	private double weight;
	private double bias;
	private readonly double learningRate;

	public NeuralNetwork(double learningRate = 0.1)
	{
		this.learningRate = learningRate;
		// Initialize with random values
		var random = new Random();
		weight = random.NextDouble() * 2 - 1; // Random value between -1 and 1
		bias = random.NextDouble() * 2 - 1;
	}

	public double Forward(double input)
	{
		double z = input * weight + bias;
		return Sigmoid(z);
	}

	public void Train(TrainingExample[] examples, int epochs)
	{
		for (int epoch = 0; epoch < epochs; epoch++)
		{
			double totalLoss = 0;

			foreach (var example in examples)
			{
				// Forward pass
				double output = Forward(example.Input);

				// Calculate loss (squared error)
				double error = example.Target - output;
				totalLoss += error * error;

				// Backpropagation
				double outputGradient = error * SigmoidDerivative(output);

				// Update weights and bias
				weight += learningRate * outputGradient * example.Input;
				bias += learningRate * outputGradient;
			}

			// Print progress every 1000 epochs
			if (epoch % 1000 == 0)
			{
				Console.WriteLine($"Epoch {epoch}, Loss: {totalLoss / examples.Length:F4}");
			}
		}
	}

	private static double Sigmoid(double x)
	{
		return 1.0 / (1.0 + Math.Exp(-x));
	}

	private static double SigmoidDerivative(double sigmoidOutput)
	{
		return sigmoidOutput * (1 - sigmoidOutput);
	}
}
