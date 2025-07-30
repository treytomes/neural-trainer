namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Interface for neural network training algorithms.
/// </summary>
public interface ITrainer
{
	/// <summary>
	/// Train the neural network using the provided training examples
	/// </summary>
	/// <param name="network">The neural network to train</param>
	/// <param name="examples">Training examples</param>
	/// <param name="epochs">Number of training epochs</param>
	void Train(INeuralNetwork network, IEnumerable<TrainingExample> examples, int epochs);
}
