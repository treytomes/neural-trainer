namespace NeuralTrainer.Domain.WeightInitializers;

// Note: In our shallow networks, this initializer seems to be complete rubbish...

/// <summary>
/// Implements He (Kaiming) initialization for neural network weights.
/// </summary>
/// <remarks>
/// Optimized for ReLU activations, this method scales weights by sqrt(2/fan_in)
/// to prevent vanishing/exploding gradients in deep networks. Biases are
/// typically initialized to zero.
///
/// Reference: He et al. (2015), "Delving Deep into Rectifiers"
/// https://arxiv.org/abs/1502.01852
/// </remarks>
public class HeInitializer : IWeightInitializer
{
	#region Fields

	private readonly Random _random;

	#endregion

	#region Constructors

	public HeInitializer(int? seed = null)
	{
		_random = seed.HasValue ? new Random(seed.Value) : new Random();
	}

	#endregion

	#region Methods

	public double InitializeWeight(int fanIn = 1, int fanOut = 1)
	{
		// He initialization: sqrt(2/fanIn)
		var scale = Math.Sqrt(2.0 / fanIn);
		return (2 * _random.NextDouble() - 1) * scale; // Uniform between -scale and scale
	}

	public double InitializeBias()
	{
		return 0.0; // Common practice is to initialize biases to zero
	}

	#endregion
}
