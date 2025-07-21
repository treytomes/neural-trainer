# NeuralTrainer

A C# console application designed to teach object-oriented architecture principles through the implementation of a linear regression model.

## Overview

NeuralTrainer is an educational project that demonstrates clean architecture and SOLID principles by building a linear regression model capable of solving for slope (m) and intercept (b) in the equation `y = mx + b`. While intentionally over-architected for learning purposes, this project showcases best practices in C# application design.

## Project Structure

The solution consists of three projects:

- **NeuralTrainer** - Console application (entry point)
- **NeuralTrainer.Domain** - Class library containing core business logic and domain models
- **NeuralTrainer.Tests** - xUnit test project for unit and integration tests

## Learning Objectives

This project is designed to demonstrate:

- Clean Architecture principles (very lightly)
- SOLID design principles
    - Single responsibility
    - Open/closed
    - Liskov substitution
    - Interface segregation
    - Dependency inversion
- Separation of concerns
- Dependency injection
- Unit testing with xUnit
- Domain-driven design concepts
- Interface-based programming

## Linear Regression Model

The application implements a linear regression algorithm that:
- Takes a set of (x, y) data points as input
- Calculates the best-fit line using least squares method
- Returns the slope (m) and y-intercept (b) values
- Provides predictions for new x values using the formula: `y = mx + b`

## Architecture Overview

The project follows a clean architecture approach with clear separation between:
- Presentation layer (Console app)
- Domain layer (Business logic and entities)
- Infrastructure layer (Data access, external services)
- Cross-cutting concerns (Logging, validation)
