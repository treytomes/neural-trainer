using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;
using NeuralTrainer.Domain.LossFunctions;
using NeuralTrainer.Domain.Training;
using NeuralTrainer.Domain.WeightInitializers;

namespace NeuralTrainer;

class Program
{
	static async Task<int> Main(string[] args)
	{
		var configFileOption = new Option<string>("--config")
		{
			Description = "Path to the configuration file",
			DefaultValueFactory = parseResult => "appsettings.json",
		};

		var debugOption = new Option<bool>("--debug")
		{
			Description = "Enable debug mode",
			DefaultValueFactory = parseResult => false,
		};

		var activationFunctionTypeOption = new Option<ActivationFunctionType>("--activation")
		{
			Description = "Activation function type",
			DefaultValueFactory = parseResult => ActivationFunctionType.Sigmoid,
		};

		var weightInitializerTypeOption = new Option<WeightInitializerType>("--weightInitializer")
		{
			Description = "Weight initializer type",
			DefaultValueFactory = parseResult => WeightInitializerType.Uniform,
		};

		// Create root command
		var rootCommand = new RootCommand("Neural Trainer")
		{
			configFileOption,
			debugOption,
			activationFunctionTypeOption,
			weightInitializerTypeOption
		};

		rootCommand.SetAction((parseResult) =>
		{
			var configFile = parseResult.GetValue(configFileOption)!;
			var debug = parseResult.GetValue(debugOption);
			var activationFunctionType = parseResult.GetValue(activationFunctionTypeOption);
			var weightInitializerType = parseResult.GetValue(weightInitializerTypeOption);
			var task = RunAsync(configFile, debug, activationFunctionType, weightInitializerType);
			task.Wait();
			return task.Result;
		});

		var parseResult = rootCommand.Parse(args);
		return await parseResult.InvokeAsync();
	}

	static async Task<int> RunAsync(string configFile, bool debug, ActivationFunctionType activationFunctionType, WeightInitializerType weightInitializerType)
	{
		try
		{
			// Build host with DI container.
			using var host = CreateHostBuilder(configFile, debug, activationFunctionType, weightInitializerType).Build();

			host.Services.GetRequiredService<IAppState>().Run();

			Console.WriteLine("Done!");
			await Task.Yield();
			return 0;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Error: {ex.Message}");
			return 1;
		}
	}

	static IHostBuilder CreateHostBuilder(string configFile, bool debug, ActivationFunctionType activationFunctionType, WeightInitializerType weightInitializerType)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration((hostContext, config) =>
			{
				config.Sources.Clear();
				config.SetBasePath(Directory.GetCurrentDirectory());
				config.AddJsonFile(configFile, optional: true, reloadOnChange: false);

				// Add command line overrides
				var commandLineConfig = new Dictionary<string, string?>();
				if (debug)
				{
					commandLineConfig["Debug"] = "true";
				}
				commandLineConfig["DefaultActivationFunction"] = activationFunctionType.ToString();
				commandLineConfig["DefaultWeightInitializer"] = weightInitializerType.ToString();

				config.AddInMemoryCollection(commandLineConfig);
			})
			.ConfigureLogging((hostContext, logging) =>
			{
				logging.ClearProviders();
				logging.AddConsole();

				// Set minimum log level based on debug setting
				var debugEnabled = hostContext.Configuration.GetValue<bool>("Debug");
				if (debugEnabled)
				{
					logging.SetMinimumLevel(LogLevel.Debug);
				}
				else
				{
					logging.SetMinimumLevel(LogLevel.Information);
				}
			})
			.ConfigureServices((hostContext, services) =>
			{
				services.Configure<AppSettings>(hostContext.Configuration);
				// services.AddTransient<IAppState, NOTGateTrainingAppState>();
				// services.AddTransient<IAppState, BinaryGateTrainingAppState>();
				services.AddTransient<IAppState, NonLinearTrainingAppState>();

				services.AddTransient<IActivationFunctionFactory>(sp => new ActivationFunctionFactory(
					sp.GetRequiredService<IOptions<AppSettings>>().Value.DefaultActivationFunction
				));
				services.AddTransient(sp => sp.GetRequiredService<IActivationFunctionFactory>().GetDefaultActivationFunction());

				services.AddTransient<IWeightInitializerFactory>(sp => new WeightInitializerFactory(
					sp.GetRequiredService<IOptions<AppSettings>>().Value.DefaultWeightInitializer
				));
				services.AddTransient(sp => sp.GetRequiredService<IWeightInitializerFactory>().GetDefaultWeightInitializer());

				services.AddTransient<IProgressReporter, ConsoleProgressReporter>();
				services.AddTransient<ILossFunction, SquaredErrorLossFunction>();

				// NeuralNetwork now takes a configurable number of inputs, so the default constructor no longer makes sense.
				// services.AddTransient<INeuralNetwork, NeuralNetwork>();
			});
	}
}
