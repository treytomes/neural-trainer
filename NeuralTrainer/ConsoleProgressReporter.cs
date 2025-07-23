using NeuralTrainer.Domain.Output;

namespace NeuralTrainer;

/// <summary>
/// Reports training progress to the console.
/// </summary>
public class ConsoleProgressReporter : IProgressReporter
{
	#region Fields

	private readonly int _reportInterval;

	#endregion

	#region Constructors

	public ConsoleProgressReporter(int reportInterval = 1000)
	{
		if (reportInterval <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(reportInterval), "Report interval must be positive.");
		}

		_reportInterval = reportInterval;
	}

	#endregion

	#region Methods

	public void ReportProgress(int epoch, double averageLoss)
	{
		if (epoch % _reportInterval == 0)
		{
			Console.WriteLine($"Epoch {epoch}, Loss: {averageLoss:F4}");
		}
	}

	#endregion
}
