using Moq;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.WeightInitializers;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;

namespace NeuralTrainer.Tests.Integration.Domain;

public class NeuralNetworkTrainingTests
{
	[Fact]
	public void TrainNetwork_WithGradientDescent_ReducesError()
	{
		// Arrange
		var activationFunction = new SigmoidActivationFunction();
		var weightInitializer = new Mock<IWeightInitializer>();

		// Use fixed initial values to make test deterministic
		weightInitializer.Setup(w => w.InitializeWeight(1, 1)).Returns(0.5);
		weightInitializer.Setup(w => w.InitializeBias()).Returns(0.1);

		var network = new NeuralNetwork(activationFunction, weightInitializer.Object);

		var lossFunction = new SquaredErrorLossFunction();
		var progressReporter = new StatisticsProgressReporter();
		var trainer = new GradientDescentTrainer(0.5, lossFunction, progressReporter);

		var trainingData = new[]
		{
			new TrainingExample(0, 0),
			new TrainingExample(1, 1)
		};

		// Calculate initial error
		double initialError = trainingData.Average(ex =>
			lossFunction.Calculate(network.Forward(ex.Input), ex.Target));

		// Act
		trainer.Train(network, trainingData, epochs: 1000);

		// Calculate final error
		double finalError = trainingData.Average(ex =>
			lossFunction.Calculate(network.Forward(ex.Input), ex.Target));

		// Assert
		Assert.True(finalError < initialError);
		Assert.True(progressReporter.Statistics.Count >= 1000);

		// Verify loss decreases over time
		var firstBatchAvgLoss = progressReporter.Statistics.Take(10).Average(s => s.AverageLoss);
		var lastBatchAvgLoss = progressReporter.Statistics.Skip(990).Average(s => s.AverageLoss);
		Assert.True(lastBatchAvgLoss < firstBatchAvgLoss);
	}

	[Fact]
	public void TrainedNetwork_PredictsSampleDataCorrectly()
	{
		// Arrange - create network with fixed initialization for deterministic results
		var activationFunction = new SigmoidActivationFunction();
		var weightInitializer = new Mock<IWeightInitializer>();
		weightInitializer.Setup(w => w.InitializeWeight(1, 1)).Returns(0.0);
		weightInitializer.Setup(w => w.InitializeBias()).Returns(0.0);

		var network = new NeuralNetwork(activationFunction, weightInitializer.Object);

		var lossFunction = new SquaredErrorLossFunction();
		var progressReporter = new NullProgressReporter();
		var trainer = new GradientDescentTrainer(0.5, lossFunction, progressReporter);

		var trainingData = new[]
		{
				new TrainingExample(0, 0),
				new TrainingExample(1, 1)
			};

		// Act - train the network
		trainer.Train(network, trainingData, epochs: 5000);

		// Predict
		double output0 = network.Forward(0);
		double output1 = network.Forward(1);

		// Assert
		Assert.InRange(output0, 0, 0.1); // Output for input 0 should be close to 0
		Assert.InRange(output1, 0.9, 1.0); // Output for input 1 should be close to 1
	}

	[Fact]
	public void TrainedNetwork_GeneralizesToUnseen_InterpolatedData()
	{
		// Arrange
		var activationFunction = new SigmoidActivationFunction();
		var weightInitializer = new UniformRandomInitializer(-0.5, 0.5, seed: 42);
		var network = new NeuralNetwork(activationFunction, weightInitializer);

		var lossFunction = new SquaredErrorLossFunction();
		var progressReporter = new NullProgressReporter();
		var trainer = new GradientDescentTrainer(0.5, lossFunction, progressReporter);

		// Simple linear relation: y = x
		var trainingData = new[]
		{
			new TrainingExample(0.0, 0.0),
			new TrainingExample(0.2, 0.2),
			new TrainingExample(0.8, 0.8),
			new TrainingExample(1.0, 1.0)
		};

		// Act - train the network
		trainer.Train(network, trainingData, epochs: 10000);

		// Test on interpolated value
		double output = network.Forward(0.5);

		// Assert - should be close to 0.5
		Assert.InRange(output, 0.4, 0.6);
	}

	[Fact]
	public void CompareTrainers_MomentumVsStandardGradientDescent()
	{
		// Arrange - create identical networks
		var activation = new SigmoidActivationFunction();
		var weightInitializer = new UniformRandomInitializer(-0.5, 0.5, seed: 42);
		var lossFunction = new SquaredErrorLossFunction();

		var standardNetwork = new NeuralNetwork(activation, weightInitializer);
		var momentumNetwork = new NeuralNetwork(activation, weightInitializer);

		var standardReporter = new StatisticsProgressReporter();
		var momentumReporter = new StatisticsProgressReporter();

		var standardTrainer = new GradientDescentTrainer(0.1, lossFunction, standardReporter);
		var momentumTrainer = new MomentumTrainer(0.1, 0.9, lossFunction, momentumReporter);

		// XOR problem training data
		var trainingData = new[]
		{
			new TrainingExample(0, 0),
			new TrainingExample(1, 1)
		};

		// Act - train both networks
		standardTrainer.Train(standardNetwork, trainingData, epochs: 2000);
		momentumTrainer.Train(momentumNetwork, trainingData, epochs: 2000);

		// Assert - compare final loss
		double standardFinalLoss = standardReporter.Statistics.Last().AverageLoss;
		double momentumFinalLoss = momentumReporter.Statistics.Last().AverageLoss;

		// Both should reach reasonably low error
		Assert.True(standardFinalLoss < 0.1);
		Assert.True(momentumFinalLoss < 0.1);
	}
}
