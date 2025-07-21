namespace NeuralTrainer.Domain.ActivationFunctions;

public class ActivationFunctionFactory : IActivationFunctionFactory
{
	private readonly ActivationFunctionType _defaultActivationFunctionType;

	public ActivationFunctionFactory(ActivationFunctionType defaultActivationFunctionType)
	{
		_defaultActivationFunctionType = defaultActivationFunctionType;
	}

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
}
