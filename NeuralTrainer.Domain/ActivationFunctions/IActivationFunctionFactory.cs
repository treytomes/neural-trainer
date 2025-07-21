namespace NeuralTrainer.Domain.ActivationFunctions;

public interface IActivationFunctionFactory
{
	IActivationFunction GetDefaultActivationFunction();
	IActivationFunction GetActivationFunction(ActivationFunctionType type);
}
