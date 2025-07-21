namespace NeuralTrainer.Domain;

public class NeuralNetwork
{
	private double weight;
	private double bias;
	private readonly double learningRate;

	public NeuralNetwork(double learningRate = 0.1)
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

		this.learningRate = learningRate;

		// Initialize with random values
		var random = new Random();

		weight = random.NextDouble() * 2 - 1; // Random value between -1 and 1
		bias = random.NextDouble() * 2 - 1;
	}

	public double Forward(double input)
	{
		var z = input * weight + bias;
		return Sigmoid(z);
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
				var outputGradient = error * SigmoidDerivative(output);

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
