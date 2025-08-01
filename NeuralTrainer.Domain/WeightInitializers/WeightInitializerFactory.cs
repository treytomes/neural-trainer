namespace NeuralTrainer.Domain.WeightInitializers;

public class WeightInitializerFactory : IWeightInitializerFactory
{
	#region Fields

	private readonly WeightInitializerType _defaultWeightInitializerType;

	#endregion

	#region Constructors

	public WeightInitializerFactory(WeightInitializerType defaultActivationFunctionType)
	{
		_defaultWeightInitializerType = defaultActivationFunctionType;
	}

	#endregion

	#region Methods

	public IWeightInitializer GetDefaultWeightInitializer()
	{
		return GetWeightInitializer(_defaultWeightInitializerType);
	}

	public IWeightInitializer GetWeightInitializer(WeightInitializerType type)
	{
		switch (type)
		{
			case WeightInitializerType.He:
				return new HeInitializer();
			case WeightInitializerType.Uniform:
			default:
				return new UniformRandomInitializer();
		}
	}

	#endregion
}
