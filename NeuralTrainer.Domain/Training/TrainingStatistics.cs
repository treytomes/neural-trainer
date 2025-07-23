namespace NeuralTrainer.Domain.Training;


public class TrainingStatistics(int epoch, double averageLoss, DateTime timestamp)
{
	public int Epoch { get; } = epoch;
	public double AverageLoss { get; } = averageLoss;
	public DateTime Timestamp { get; } = timestamp;
}
