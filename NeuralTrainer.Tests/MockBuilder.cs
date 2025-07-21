using Moq;
using NeuralTrainer.Domain.ActivationFunctions;

namespace NeuralTrainer.Tests;

sealed class MockBuilder
{
	public IActivationFunction GetMockActivationFunction()
	{
		var mockActivation = new Mock<IActivationFunction>();
		mockActivation.Setup(a => a.Activate(It.IsAny<double>())).Returns(0.5);
		mockActivation.Setup(a => a.Derivative(It.IsAny<double>())).Returns(0.25);
		return mockActivation.Object;
	}
}
