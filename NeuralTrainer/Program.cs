// Program.cs

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NeuralTrainer.Domain;
using NeuralTrainer.Domain.ActivationFunctions;

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

		var activationFunction = new Option<ActivationFunctionType>("--activation")
		{
			Description = "Activation function type",
			DefaultValueFactory = parseResult => ActivationFunctionType.Sigmoid,
		};

		// Create root command
		var rootCommand = new RootCommand("Neural Trainer")
		{
			configFileOption,
			debugOption,
		};

		rootCommand.SetAction((parseResult) =>
		{
			var configFile = parseResult.GetValue(configFileOption)!;
			var debug = parseResult.GetValue(debugOption);
			var task = RunAsync(configFile, debug);
			task.Wait();
			return task.Result;
		});

		var parseResult = rootCommand.Parse(args);
		return await parseResult.InvokeAsync();
	}

	static async Task<int> RunAsync(string configFile, bool debug)
	{
		try
		{
			// Build host with DI container.
			using var host = CreateHostBuilder(configFile, debug).Build();

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

	static IHostBuilder CreateHostBuilder(string configFile, bool debug)
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
				services.AddTransient<IAppState>(sp => new NOTGateTrainingAppState(sp, sp.GetRequiredService<IActivationFunctionFactory>()));
				services.AddTransient<IActivationFunctionFactory>(sp => new ActivationFunctionFactory(
					sp.GetRequiredService<IOptions<AppSettings>>().Value.DefaultActivationFunction
				));
				services.AddTransient(sp => sp.GetRequiredService<IActivationFunctionFactory>().GetDefaultActivationFunction());
			});
	}
}
