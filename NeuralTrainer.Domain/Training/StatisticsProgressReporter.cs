namespace NeuralTrainer.Domain.Training;

/// <summary>
/// Collects training statistics for analysis.
/// </summary>
public class StatisticsProgressReporter : IProgressReporter
{
	#region Fields

	private readonly List<TrainingStatistics> _statistics = new();

	#endregion

	#region Properties

	// Note: Statistics can only be read from the outside world.
	public IReadOnlyList<TrainingStatistics> Statistics => _statistics.AsReadOnly();

	#endregion

	#region Methods

	public void ReportProgress(int epoch, double averageLoss)
	{
		_statistics.Add(new TrainingStatistics(epoch, averageLoss, DateTime.UtcNow));
	}

	#endregion
}
