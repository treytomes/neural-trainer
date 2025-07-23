namespace NeuralTrainer.Domain.WeightInitializers;

public interface IWeightInitializerFactory
{
	IWeightInitializer GetDefaultWeightInitializer();
	IWeightInitializer GetWeightInitializer(WeightInitializerType type);
}
