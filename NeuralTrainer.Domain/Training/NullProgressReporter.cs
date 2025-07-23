namespace NeuralTrainer.Domain.Training;

/// <summary>
/// A progress reporter that does nothing (Null Object Pattern).
/// </summary>
public class NullProgressReporter : IProgressReporter
{
	public void ReportProgress(int epoch, double averageLoss)
	{
		// Do nothing - useful for testing or when progress reporting is not needed.
	}
}
