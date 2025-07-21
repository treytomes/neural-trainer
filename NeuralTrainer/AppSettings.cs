using NeuralTrainer.Domain.ActivationFunctions;

namespace NeuralTrainer;

public class AppSettings
{
	public bool Debug { get; set; }
	public ActivationFunctionType DefaultActivationFunction { get; set; }
}
