using Moq;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer.Tests.Unit.Domain;

public class NeuronTests
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
		var neuron = new Neuron(1, mockActivation.Object, mockInitializer.Object);

		// Assert
		Assert.Equal(0.5, neuron.Weights[0]);
		Assert.Equal(0.3, neuron.Bias);
	}

	[Fact]
	public void Constructor_WithInvalidInputSize_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new Neuron(0, mockActivation.Object, mockInitializer.Object));
		Assert.Throws<ArgumentException>(() => new Neuron(-1, mockActivation.Object, mockInitializer.Object));
	}

	// [Fact]
	// public void Constructor_WithNullParameters_ThrowsException()
	// {
	// 	// Arrange
	// 	var mockActivation = new Mock<IActivationFunction>();
	// 	var mockInitializer = new Mock<IWeightInitializer>();

	// 	// Act & Assert
	// 	Assert.Throws<ArgumentNullException>(() => new Neuron(1, null, mockInitializer.Object));
	// 	Assert.Throws<ArgumentNullException>(() => new Neuron(1, mockActivation.Object, null));
	// }

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

		var neuron = new Neuron(1, mockActivation.Object, mockInitializer.Object);

		// Act
		double[] inputs = [2.0];
		var result = neuron.Forward(inputs);

		// Assert
		// Expected calculation: (2.0 * 0.5 + 0.3) * 2 = 2.6
		mockActivation.Verify(a => a.Activate(1.3), Times.Once);
		Assert.Equal(2.6, result);
	}

	[Fact]
	public void CalculateGradients_ReturnsCorrectGradients()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		mockActivation.Setup(a => a.Activate(It.IsAny<double>())).Returns(0.8);
		mockActivation.Setup(a => a.Derivative(0.8)).Returns(0.16); // sigmoid derivative for output 0.8

		var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

		// Act
		double[] inputs = [1.0, 2.0];
		neuron.Forward(inputs); // Need to call forward first to cache values
		var (weightGradients, biasGradient) = neuron.CalculateGradients(inputs, 0.5);

		// Assert
		// neuronGradient = outputGradient * activationDerivative = 0.5 * 0.16 = 0.08
		// weightGradients[0] = neuronGradient * inputs[0] = 0.08 * 1.0 = 0.08
		// weightGradients[1] = neuronGradient * inputs[1] = 0.08 * 2.0 = 0.16
		Assert.Equal(2, weightGradients.Count);
		Assert.Equal(0.08, weightGradients[0], precision: 6);
		Assert.Equal(0.16, weightGradients[1], precision: 6);
		Assert.Equal(0.08, biasGradient, precision: 6);
	}

	[Fact]
	public void UpdateParameters_ModifiesWeightAndBias()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		var neuron = new Neuron(1, mockActivation.Object, mockInitializer.Object);

		// Act
		neuron.UpdateParameters([0.1], -0.2);

		// Assert
		Assert.Equal([0.6], neuron.Weights);
		Assert.Equal(0.1, neuron.Bias, precision: 6);
	}
}

public class NeuralNetworkTests
{
	[Fact]
	public void Constructor_WithValidParameters_CreatesInstance()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		// Act
		var network = new NeuralNetwork(3, mockActivation.Object, mockInitializer.Object);

		// Assert
		Assert.Equal(3, network.InputSize);
	}

	[Fact]
	public void Constructor_WithInvalidParameters_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new NeuralNetwork(0, mockActivation.Object, mockInitializer.Object));
		// Assert.Throws<ArgumentNullException>(() => new NeuralNetwork(1, null, mockInitializer.Object));
		// Assert.Throws<ArgumentNullException>(() => new NeuralNetwork(1, mockActivation.Object, null));
	}

	[Fact]
	public void Forward_DelegatesToNeuron()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();
		mockActivation.Setup(a => a.Activate(It.IsAny<double>())).Returns(0.75);

		var network = new NeuralNetwork(2, mockActivation.Object, mockInitializer.Object);

		// Act
		var result = network.Forward([1.0, 2.0]);

		// Assert
		Assert.Equal(0.75, result);
	}

	[Fact]
	public void CalculateGradients_DelegatesToNeuron()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockActivation.Setup(a => a.Activate(It.IsAny<double>())).Returns(0.8);
		mockActivation.Setup(a => a.Derivative(0.8)).Returns(0.16);

		var network = new NeuralNetwork(2, mockActivation.Object, mockInitializer.Object);

		// Act
		var inputs = new double[] { 1.0, 2.0 };
		network.Forward(inputs); // Need to call forward first
		var (weightGradients, biasGradient) = network.CalculateGradients(inputs, 0.5);

		// Assert
		Assert.NotNull(weightGradients);
		Assert.Equal(2, weightGradients.Count);
		Assert.IsType<double>(biasGradient);
	}

	[Fact]
	public void UpdateParameters_DelegatesToNeuron()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(0.5);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.3);

		var network = new NeuralNetwork(2, mockActivation.Object, mockInitializer.Object);

		// Act
		network.UpdateParameters([0.1, 0.2], -0.1);

		// Assert - verify the operation doesn't throw
		Assert.True(true);
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

		var network = new NeuralNetwork(1, activation, mockInitializer.Object);

		// Act
		var output = network.Forward([0.5]);

		// Assert
		// Expected: sigmoid(0.5 * 2.0 - 1.0) = sigmoid(0) = 0.5
		Assert.Equal(0.5, output, precision: 6);
	}

	[Fact]
	public void CalculateGradients_WithSigmoidActivation_CalculatesExpectedGradients()
	{
		// Arrange
		var activation = new SigmoidActivationFunction();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(1.0);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.0);

		var network = new NeuralNetwork(2, activation, mockInitializer.Object);

		// Act
		var inputs = new double[] { 1.0, 2.0 };
		var output = network.Forward(inputs); // sigmoid(1*1 + 2*1 + 0) = sigmoid(3) ≈ 0.9526
		var (weightGradients, biasGradient) = network.CalculateGradients(inputs, 1.0);

		// Assert
		// sigmoid'(0.9526) ≈ 0.0452
		// weightGradients[0] = 1.0 * 0.0452 * 1.0 ≈ 0.0452
		// weightGradients[1] = 1.0 * 0.0452 * 2.0 ≈ 0.0904
		Assert.Equal(2, weightGradients.Count);
		Assert.Equal(0.0452, weightGradients[0], precision: 3);
		Assert.Equal(0.0904, weightGradients[1], precision: 3);
		Assert.Equal(0.0452, biasGradient, precision: 3);
	}

	[Fact]
	public void UpdateParameters_WithMultipleUpdates_AccumulatesCorrectly()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();

		mockInitializer.Setup(i => i.InitializeWeight(1, 1)).Returns(1.0);
		mockInitializer.Setup(i => i.InitializeBias()).Returns(0.0);

		var network = new NeuralNetwork(2, mockActivation.Object, mockInitializer.Object);

		// Act
		network.UpdateParameters([0.1, 0.2], 0.2);
		network.UpdateParameters([0.3, -0.1], -0.1);
		network.UpdateParameters([-0.2, 0.4], 0.5);

		// Assert - we can't directly access weights/bias anymore, so we verify through forward pass
		mockActivation.Setup(a => a.Activate(It.IsAny<double>())).Returns<double>(z => z);

		// Original weights were [1.0, 1.0], bias was 0.0
		// After updates: weights = [1.2, 1.5], bias = 0.6
		// Forward with [1, 1] should give: 1.2*1 + 1.5*1 + 0.6 = 3.3
		var result = network.Forward([1.0, 1.0]);
		mockActivation.Verify(a => a.Activate(3.3), Times.Once);
	}

	[Fact]
	public void FullTrainingCycle_UpdatesParametersCorrectly()
	{
		// Arrange
		var activation = new SigmoidActivationFunction();
		var initializer = new UniformRandomInitializer(-0.1, 0.1, seed: 42);
		var network = new NeuralNetwork(2, activation, initializer);

		var inputs = new double[] { 1.0, 0.5 };

		// Act - simulate one training step
		var output1 = network.Forward(inputs);
		var (weightGradients, biasGradient) = network.CalculateGradients(inputs, 0.1); // small gradient

		// Apply gradients with learning rate
		var learningRate = 0.1;
		var weightDeltas = weightGradients.Select(g => learningRate * g).ToList();
		var biasDelta = learningRate * biasGradient;

		network.UpdateParameters(weightDeltas, biasDelta);
		var output2 = network.Forward(inputs);

		// Assert - output should change after parameter update
		Assert.NotEqual(output1, output2);
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

// Additional tests for edge cases and error handling
public class NeuronEdgeCaseTests
{
	// [Fact]
	// public void Forward_WithNullInputs_ThrowsException()
	// {
	// 	// Arrange
	// 	var mockActivation = new Mock<IActivationFunction>();
	// 	var mockInitializer = new Mock<IWeightInitializer>();
	// 	var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

	// 	// Act & Assert
	// 	Assert.Throws<ArgumentNullException>(() => neuron.Forward(null));
	// }

	[Fact]
	public void Forward_WithWrongInputSize_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();
		var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Throws<ArgumentException>(() => neuron.Forward([1.0])); // Expected 2 inputs
		Assert.Throws<ArgumentException>(() => neuron.Forward([1.0, 2.0, 3.0])); // Expected 2 inputs
	}

	// [Fact]
	// public void CalculateGradients_WithNullInputs_ThrowsException()
	// {
	// 	// Arrange
	// 	var mockActivation = new Mock<IActivationFunction>();
	// 	var mockInitializer = new Mock<IWeightInitializer>();
	// 	var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

	// 	// Act & Assert
	// 	Assert.Throws<ArgumentNullException>(() => neuron.CalculateGradients(null, 0.5));
	// }

	[Fact]
	public void CalculateGradients_WithInvalidGradient_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();
		var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Throws<ArgumentException>(() => neuron.CalculateGradients([1.0, 2.0], double.NaN));
		Assert.Throws<ArgumentException>(() => neuron.CalculateGradients([1.0, 2.0], double.PositiveInfinity));
	}

	// [Fact]
	// public void UpdateParameters_WithNullWeightDeltas_ThrowsException()
	// {
	// 	// Arrange
	// 	var mockActivation = new Mock<IActivationFunction>();
	// 	var mockInitializer = new Mock<IWeightInitializer>();
	// 	var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

	// 	// Act & Assert
	// 	Assert.Throws<ArgumentNullException>(() => neuron.UpdateParameters(null, 0.1));
	// }

	[Fact]
	public void UpdateParameters_WithWrongDeltaSize_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();
		var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Throws<ArgumentException>(() => neuron.UpdateParameters([0.1], 0.1)); // Expected 2 deltas
	}

	[Fact]
	public void UpdateParameters_WithInvalidBiasDelta_ThrowsException()
	{
		// Arrange
		var mockActivation = new Mock<IActivationFunction>();
		var mockInitializer = new Mock<IWeightInitializer>();
		var neuron = new Neuron(2, mockActivation.Object, mockInitializer.Object);

		// Act & Assert
		Assert.Throws<ArgumentException>(() => neuron.UpdateParameters([0.1, 0.2], double.NaN));
		Assert.Throws<ArgumentException>(() => neuron.UpdateParameters([0.1, 0.2], double.NegativeInfinity));
	}
}
