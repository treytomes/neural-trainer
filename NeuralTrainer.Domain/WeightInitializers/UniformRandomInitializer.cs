namespace NeuralTrainer.Domain.WeightInitializers;

/// <summary>
/// Initializes weights and biases with uniformly distributed random values
/// </summary>
public class UniformRandomInitializer : IWeightInitializer
{
	#region Fields

	private readonly Random _random;
	private readonly double _minValue;
	private readonly double _maxValue;

	#endregion

	#region Constructors

	/// <summary>
	/// Creates a new uniform random initializer.
	/// </summary>
	/// <param name="minValue">Minimum random value (default: -1).</param>
	/// <param name="maxValue">Maximum random value (default: 1).</param>
	/// <param name="seed">Optional seed for random number generator.</param>
	public UniformRandomInitializer(double minValue = -1.0, double maxValue = 1.0, int? seed = null)
	{
		_minValue = minValue;
		_maxValue = maxValue;
		_random = seed.HasValue ? new Random(seed.Value) : new Random();
	}

	#endregion

	#region Methods

	/// <inheritdoc />
	public double InitializeWeight(int inputSize = 1, int outputSize = 1)
	{
		return _random.NextDouble() * (_maxValue - _minValue) + _minValue;
	}

	/// <inheritdoc />
	public double InitializeBias()
	{
		return _random.NextDouble() * (_maxValue - _minValue) + _minValue;
	}

	#endregion
}
