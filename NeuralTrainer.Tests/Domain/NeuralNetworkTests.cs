using Moq;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Tests.Domains;

public class NeuralNetworkTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(-0.001)]
	[InlineData(-0.1)]
	[InlineData(-1)]
	[InlineData(-10)]
	[InlineData(double.MinValue)]
	public void Constructor_ShouldThrowArgumentException_WhenLearningRateIsNotPositive(double invalidLearningRate)
	{
		// Arrange
		var mockActivation = new MockBuilder().GetMockActivationFunction();
		var mockWeightInitializer = new MockBuilder().GetMockWeightInitializer();
		var mockLoss = new MockBuilder().GetMockLossFunction();

		// Act & Assert
		var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new NeuralNetwork(invalidLearningRate, mockActivation.Object, mockWeightInitializer.Object, mockLoss.Object));
		Assert.Contains("Learning rate must be positive.", exception.Message);
		Assert.Equal("learningRate", exception.ParamName);
	}

	[Theory]
	[InlineData(1.001)]
	[InlineData(1.1)]
	[InlineData(2)]
	[InlineData(10)]
	[InlineData(100)]
	[InlineData(double.MaxValue)]
	public void Constructor_ShouldThrowArgumentException_WhenLearningRateIsGreaterThanOne(double invalidLearningRate)
	{
		// Arrange
		var mockActivation = new MockBuilder().GetMockActivationFunction();
		var mockWeightInitializer = new MockBuilder().GetMockWeightInitializer();
		var mockLoss = new MockBuilder().GetMockLossFunction();

		// Act & Assert
		var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new NeuralNetwork(invalidLearningRate, mockActivation.Object, mockWeightInitializer.Object, mockLoss.Object));
		Assert.Contains("Learning rate should not exceed 1 for stable training.", exception.Message);
		Assert.Equal("learningRate", exception.ParamName);
	}

	[Theory]
	[InlineData(0.001)]   // Minimum reasonable value
	[InlineData(0.01)]    // Conservative
	[InlineData(0.1)]     // Default
	[InlineData(0.5)]     // Aggressive
	[InlineData(0.9)]     // Very aggressive
	[InlineData(1.0)]     // Maximum allowed
	public void Constructor_ShouldAcceptValidLearningRates(double validLearningRate)
	{
		// Arrange
		var mockActivation = new MockBuilder().GetMockActivationFunction();
		var mockWeightInitializer = new MockBuilder().GetMockWeightInitializer();
		var mockLoss = new MockBuilder().GetMockLossFunction();

		// Act
		var network = new NeuralNetwork(validLearningRate, mockActivation.Object, mockWeightInitializer.Object, mockLoss.Object);

		// Assert
		Assert.NotNull(network);
		// Note: If you expose LearningRate as a property, you could also verify:
		// Assert.Equal(validLearningRate, network.LearningRate);
	}

	[Theory]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	[InlineData(double.NegativeInfinity)]
	public void Constructor_ShouldThrowArgumentException_WhenLearningRateIsNotFinite(double invalidLearningRate)
	{
		// Arrange
		var mockActivation = new MockBuilder().GetMockActivationFunction();
		var mockWeightInitializer = new MockBuilder().GetMockWeightInitializer();
		var mockLoss = new MockBuilder().GetMockLossFunction();

		// Act & Assert
		var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new NeuralNetwork(invalidLearningRate, mockActivation.Object, mockWeightInitializer.Object, mockLoss.Object));
		Assert.NotNull(exception);
		Assert.Equal("learningRate", exception.ParamName);
	}

	[Theory]
	[InlineData(0.1)]
	[InlineData(0.5)]
	public void Train_ShouldConverge_WithDifferentValidLearningRates(double learningRate)
	{
		// Arrange
		var activationFunction = new SigmoidActivationFunction();
		var weightInitializer = new UniformRandomInitializer();
		var lossFunction = new SquaredErrorLossFunction();
		var network = new NeuralNetwork(learningRate, activationFunction, weightInitializer, lossFunction);
		var trainingData = new[]
		{
				new TrainingExample(0, 1),
				new TrainingExample(1, 0)
			};

		// Act
		network.Train(trainingData, epochs: 5000);

		// Assert - network should learn the NOT gate
		var output0 = network.Forward(0);
		var output1 = network.Forward(1);

		Assert.True(output0 > 0.9, $"For input 0, expected output > 0.9 but got {output0:F4}");
		Assert.True(output1 < 0.1, $"For input 1, expected output < 0.1 but got {output1:F4}");
	}

	[Fact]
	public void Train_ShouldConvergeFaster_WithHigherLearningRate()
	{
		// Arrange
		var activationFunction = new SigmoidActivationFunction();
		var weightInitializer = new UniformRandomInitializer();
		var lossFunction = new SquaredErrorLossFunction();
		var slowNetwork = new NeuralNetwork(0.1, activationFunction, weightInitializer, lossFunction);
		var fastNetwork = new NeuralNetwork(0.5, activationFunction, weightInitializer, lossFunction);
		var trainingData = new[]
		{
				new TrainingExample(0, 1),
				new TrainingExample(1, 0)
			};

		// Train for fewer epochs
		int epochs = 1000;

		// Act
		slowNetwork.Train(trainingData, epochs);
		fastNetwork.Train(trainingData, epochs);

		// Calculate errors
		var slowError0 = Math.Abs(1 - slowNetwork.Forward(0));
		var slowError1 = Math.Abs(0 - slowNetwork.Forward(1));
		var fastError0 = Math.Abs(1 - fastNetwork.Forward(0));
		var fastError1 = Math.Abs(0 - fastNetwork.Forward(1));

		var slowTotalError = slowError0 + slowError1;
		var fastTotalError = fastError0 + fastError1;

		// Assert - higher learning rate should have lower error after same epochs
		Assert.True(fastTotalError < slowTotalError,
			$"Expected faster convergence with higher learning rate. " +
			$"Slow error: {slowTotalError:F4}, Fast error: {fastTotalError:F4}");
	}

	[Fact]
	public void NeuralNetwork_ShouldAcceptCustomActivationFunction()
	{
		// Arrange
		var activationFunction = new MockBuilder().GetMockActivationFunction();
		var weightInitializer = new MockBuilder().GetMockWeightInitializer();
		var mockLoss = new MockBuilder().GetMockLossFunction();

		// Act
		var network = new NeuralNetwork(0.1, activationFunction.Object, weightInitializer.Object, mockLoss.Object);
		var output = network.Forward(1.0);

		// Assert
		Assert.Equal(0.5, output);
		activationFunction.Verify(a => a.Activate(It.IsAny<double>()), Times.Once);
	}
}
