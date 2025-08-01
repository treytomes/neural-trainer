using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer;

public class AppSettings
{
	public bool Debug { get; set; }
	public ActivationFunctionType DefaultActivationFunction { get; set; }
	public WeightInitializerType DefaultWeightInitializer { get; set; }
}
