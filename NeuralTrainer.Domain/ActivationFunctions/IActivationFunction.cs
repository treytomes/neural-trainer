namespace NeuralTrainer.Domain.ActivationFunctions;

public interface IActivationFunction
{
	double Activate(double input);
	double Derivative(double output);
}
