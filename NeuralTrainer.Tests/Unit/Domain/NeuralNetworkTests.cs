using Moq;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Tests.Unit.Domain;

public class NeuralNetworkTests
{
	[Fact]
	public void Constructor_WithValidParameters_CreatesInstance()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		// Act
		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Assert
		Assert.Equal(0.5, network.Weight);
		Assert.Equal(0.3, network.Bias);
		Assert.Same(mockActivation.Object, network.ActivationFunction);
	}

	[Fact]
	public void Forward_CalculatesCorrectly()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		mockActivation.Setup(a => a.Activate(It.IsAny<double>()))
			.Returns<double>(z => z * 2); // Simple mock function that doubles the input

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act
		var input = 2.0;
		var result = network.Forward(input);

		// Assert
		// Expected calculation: (2.0 * 0.5 + 0.3) * 2 = 2.6
		mockActivation.Verify(a => a.Activate(1.3), Times.Once);
		Assert.Equal(2.6, result);
	}

	[Fact]
	public void UpdateParameters_ModifiesWeightAndBias()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act
		network.UpdateParameters(0.1, -0.2);

		// Assert
		Assert.Equal(0.6, network.Weight);
		Assert.Equal(0.1, network.Bias, precision: 6);
	}

	[Fact]
	public void Weight_ReturnsCurrentValue()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.42);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.0);

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Equal(0.42, network.Weight);
	}

	[Fact]
	public void Bias_ReturnsCurrentValue()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.0);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.24);

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Equal(0.24, network.Bias);
	}

	[Fact]
	public void ActivationFunction_ReturnsSameInstanceUsedInConstructor()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Same(mockActivation.Object, network.ActivationFunction);
	}
}

public class NeuralNetworkIntegrationTests
{
	[Fact]
	public void Forward_WithSigmoidActivation_CalculatesExpectedOutput()
	{
		// Arrange
		var activation = new SigmoidActivationFunction();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(2.0);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(-1.0);

		var network = new NeuralNetwork(activation, mockInitializer.Object);

		// Act
		var output = network.Forward(0.5);

		// Assert
		// Expected: sigmoid(0.5 * 2.0 - 1.0) = sigmoid(0) = 0.5
		Assert.Equal(0.5, output, precision: 6);
	}

	[Fact]
	public void UpdateParameters_WithMultipleUpdates_AccumulatesCorrectly()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(1.0);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.0);

		var network = new NeuralNetwork(mockActivation.Object, mockInitializer.Object);

		// Act
		network.UpdateParameters(0.1, 0.2);
		network.UpdateParameters(0.3, -0.1);
		network.UpdateParameters(-0.2, 0.5);

		// Assert
		Assert.Equal(1.2, network.Weight, precision: 6);
		Assert.Equal(0.6, network.Bias);
	}
}

public class WeightInitializerTests
{
	[Fact]
	public void UniformRandomInitializer_InitializesWeightsWithinRange()
	{
		// Arrange
		var initializer = new UniformRandomInitializer(-0.5, 0.5, seed: 42);

		// Act
		var weights = new double[1000];
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = initializer.InitializeWeight();
		}

		// Assert
		foreach (var weight in weights)
		{
			Assert.InRange(weight, -0.5, 0.5);
		}

		// Check distribution properties
		var avg = weights.Average();
		Assert.InRange(avg, -0.1, 0.1); // Should be close to 0 for uniform distribution
	}

	[Fact]
	public void UniformRandomInitializer_InitializesBiasesWithinRange()
	{
		// Arrange
		var initializer = new UniformRandomInitializer(-0.3, 0.3, seed: 42);

		// Act
		var biases = new double[1000];
		for (int i = 0; i < biases.Length; i++)
		{
			biases[i] = initializer.InitializeBias();
		}

		// Assert
		foreach (var bias in biases)
		{
			Assert.InRange(bias, -0.3, 0.3);
		}

		// Check distribution properties
		var avg = biases.Average();
		Assert.InRange(avg, -0.1, 0.1); // Should be close to 0 for uniform distribution
	}
}

public class ActivationFunctionTests
{
	[Fact]
	public void SigmoidActivation_Activate_ReturnsExpectedValues()
	{
		// Arrange
		var activation = new SigmoidActivationFunction();

		// Act & Assert
		Assert.Equal(0.5, activation.Activate(0), precision: 6);
		Assert.InRange(activation.Activate(10), 0.999, 1);
		Assert.InRange(activation.Activate(-10), 0, 0.001);
	}

	[Fact]
	public void SigmoidActivation_Derivative_ReturnsExpectedValues()
	{
		// Arrange
		var activation = new SigmoidActivationFunction();

		// Act & Assert
		Assert.Equal(0.25, activation.Derivative(0.5), precision: 6);
		Assert.InRange(activation.Derivative(0.999), 0, 0.001);
		Assert.InRange(activation.Derivative(0.001), 0, 0.001);
	}
}
