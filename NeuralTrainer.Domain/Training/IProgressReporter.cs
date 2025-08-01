namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Interface for reporting training progress
/// </summary>
public interface IProgressReporter
{
	/// <summary>
	/// Report training progress
	/// </summary>
	void ReportProgress(int epoch, double averageLoss);
}
