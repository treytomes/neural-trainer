using Moq;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Tests;

sealed class MockBuilder
{
	public Mock<IActivationFunction> GetMockActivationFunction()
	{
		var mock = new Mock<IActivationFunction>();
		mock.Setup(a => a.Activate(It.IsAny<double>())).Returns(0.5);
		mock.Setup(a => a.Derivative(It.IsAny<double>())).Returns(0.25);
		return mock;
	}

	public Mock<IWeightInitializer> GetMockWeightInitializer()
	{
		var mock = new Mock<IWeightInitializer>();
		mock.Setup(a => a.InitializeBias()).Returns(0.5);
		mock.Setup(a => a.InitializeWeight(It.IsAny<int>(), It.IsAny<int>())).Returns(0.25);
		return mock;
	}

	public Mock<ILossFunction> GetMockLossFunction()
	{
		var mock = new Mock<ILossFunction>();
		mock.Setup(a => a.Calculate(It.IsAny<double>(), It.IsAny<double>())).Returns(0.5);
		mock.Setup(a => a.Derivative(It.IsAny<double>(), It.IsAny<double>())).Returns(0.25);
		return mock;
	}

	public Mock<IProgressReporter> GetMockProgressReporter()
	{
		var mock = new Mock<IProgressReporter>();
		mock.Setup(a => a.ReportProgress(It.IsAny<int>(), It.IsAny<double>()));
		return mock;
	}

	public Mock<ITrainer> GetMockTrainer()
	{
		var mock = new Mock<ITrainer>();
		mock.Setup(t => t.Train(It.IsAny<INeuralNetwork>(), It.IsAny<TrainingExample[]>(), It.IsAny<int>()));
		return mock;
	}
}
