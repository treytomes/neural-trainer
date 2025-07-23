namespace NeuralTrainer.Domain.ActivationFunctions;

public class ActivationFunctionFactory : IActivationFunctionFactory
{
	#region Fields

	private readonly ActivationFunctionType _defaultActivationFunctionType;

	#endregion

	#region Constructors

	public ActivationFunctionFactory(ActivationFunctionType defaultActivationFunctionType)
	{
		_defaultActivationFunctionType = defaultActivationFunctionType;
	}

	#endregion

	#region Methods

	public IActivationFunction GetDefaultActivationFunction()
	{
		return GetActivationFunction(_defaultActivationFunctionType);
	}

	public IActivationFunction GetActivationFunction(ActivationFunctionType type)
	{
		switch (type)
		{
			case ActivationFunctionType.ReLU:
				return new ReLUActivationFunction();
			case ActivationFunctionType.Sigmoid:
			default:
				return new SigmoidActivationFunction();
			case ActivationFunctionType.Tanh:
				return new TanhActivationFunction();
		}
	}

	#endregion
}
