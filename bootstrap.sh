#!/bin/bash

# Check if project name is provided
if [ -z "$1" ]; then
  echo "Please provide a project name."
  echo "Usage: ./bootstrap.sh <project-name>"
  exit 1
fi

PROJECT_NAME=$1
LIBRARY_NAME="${PROJECT_NAME}.Domain"
CONSOLE_NAME="${PROJECT_NAME}"
TEST_NAME="${PROJECT_NAME}.Tests"

echo "Creating $PROJECT_NAME solution and projects..."

# Create the solution
dotnet new sln -n $PROJECT_NAME

# Create the class library
dotnet new classlib -n $LIBRARY_NAME
# Create the console application
dotnet new console -n $CONSOLE_NAME
# Create the test project
dotnet new xunit -n $TEST_NAME

# Add projects to solution
dotnet sln $PROJECT_NAME.sln add $LIBRARY_NAME/$LIBRARY_NAME.csproj
dotnet sln $PROJECT_NAME.sln add $CONSOLE_NAME/$CONSOLE_NAME.csproj
dotnet sln $PROJECT_NAME.sln add $TEST_NAME/$TEST_NAME.csproj

# Add references
# Console app references the library
dotnet add $CONSOLE_NAME/$CONSOLE_NAME.csproj reference $LIBRARY_NAME/$LIBRARY_NAME.csproj
# Test project references the library
dotnet add $TEST_NAME/$TEST_NAME.csproj reference $LIBRARY_NAME/$LIBRARY_NAME.csproj

# Initialize git repository
git init

# Create .gitignore file
cat > .gitignore << EOF
# Build results
[Bb]in/
[Oo]bj/

# Visual Studio files
.vs/
*.user
*.userosscache
*.suo
*.userprefs

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# Visual Studio Code
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json

# Rider
.idea/
*.sln.iml

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# NuGet Packages
*.nupkg
# NuGet Symbol Packages
*.snupkg
# The packages folder can be ignored because of Package Restore
**/[Pp]ackages/*
# except build/, which is used as an MSBuild target.
!**/[Pp]ackages/build/
# Uncomment if necessary however generally it will be regenerated when needed
#!**/[Pp]ackages/repositories.config
# NuGet v3's project.json files produces more ignorable files
*.nuget.props
*.nuget.targets
EOF

# Create .vscode directory and settings.json
mkdir -p .vscode
cat > .vscode/settings.json << EOF
{
	"cSpell.words": [
		"ackages",
		"consoleloggerparameters",
		"coreclr",
		"docstates",
		"dotnettools",
		"omnisharp",
		"rsuser",
		"snupkg",
		"userosscache",
		"userprefs",
		"xunit"
	],

	"editor.bracketPairColorization.enabled": true,
	"editor.codeActionsOnSave": {
		"source.fixAll": "always",
		"source.fixAll.eslint": "always",
		"source.addMissingImports": "always"
	},
	"dotnet.defaultSolution": "${PROJECT_NAME}.sln",
	"editor.detectIndentation": false,
	"editor.formatOnSave": true,
	"editor.formatOnType": true,
    "editor.guides.bracketPairs": true,
	"editor.insertSpaces": false,
    "editor.minimap.enabled": true,
	"editor.renderWhitespace": "all",
	"editor.rulers": [100],
	"editor.tabSize": 4,

    "files.insertFinalNewline": true,
    "files.trimFinalNewlines": true,
    "files.trimTrailingWhitespace": true,

	"csharp.debug.expressionEvaluationOptions.allowImplicitFuncEval": false,
	"csharp.format.enable": true,
    "csharp.semanticHighlighting.enabled": true,
    "omnisharp.enableEditorConfigSupport": true,

	// C# Dev Kit formatting settings
	"[csharp]": {
		"editor.defaultFormatter": "ms-dotnettools.csharp",
		"editor.codeActionsOnSave": {
			"source.fixAll": "always"
		}
	}
}
EOF

# Create launch.json for debugging
cat > .vscode/launch.json << EOF
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch Console App",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-console",
            "program": "\${workspaceFolder}/${CONSOLE_NAME}/bin/Debug/net9.0/${CONSOLE_NAME}",
            "args": [],
            "cwd": "\${workspaceFolder}/${CONSOLE_NAME}",
            "console": "internalConsole",
            "stopAtEntry": false
        },
        {
            "name": "Run All Tests",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-tests",
            "program": "dotnet",
            "args": [
                "test",
                "\${workspaceFolder}/${TEST_NAME}/${TEST_NAME}.csproj"
            ],
            "cwd": "\${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        },
        {
            "name": "Debug Tests",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build-tests",
            "program": "dotnet",
            "args": [
                "test",
                "--no-build",
                "\${workspaceFolder}/${TEST_NAME}/${TEST_NAME}.csproj"
            ],
            "cwd": "\${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
EOF

# Create tasks.json for build tasks
cat > .vscode/tasks.json << EOF
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build-solution",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "\${workspaceFolder}/${PROJECT_NAME}.sln",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile",
            "group": {
                "kind": "build",
                "isDefault": true
            }
        },
        {
            "label": "build-console",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "\${workspaceFolder}/${CONSOLE_NAME}/${CONSOLE_NAME}.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile"
        },
        {
            "label": "build-lib",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "\${workspaceFolder}/${LIBRARY_NAME}/${LIBRARY_NAME}.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile"
        },
        {
            "label": "build-tests",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "\${workspaceFolder}/${TEST_NAME}/${TEST_NAME}.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile"
        },
        {
            "label": "test",
            "command": "dotnet",
            "type": "process",
            "args": [
                "test",
                "\${workspaceFolder}/${TEST_NAME}/${TEST_NAME}.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile",
            "group": {
                "kind": "test",
                "isDefault": true
            }
        },
        {
            "label": "publish",
            "command": "dotnet",
            "type": "process",
            "args": [
                "publish",
                "\${workspaceFolder}/${CONSOLE_NAME}/${CONSOLE_NAME}.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "\$msCompile"
        },
        {
            "label": "watch",
            "command": "dotnet",
            "type": "process",
            "args": [
                "watch",
                "run",
                "--project",
                "\${workspaceFolder}/${CONSOLE_NAME}/${CONSOLE_NAME}.csproj"
            ],
            "problemMatcher": "\$msCompile"
        }
    ]
}
EOF

# Make initial commit
git add .
git commit -m "Initial project setup"

echo "Project structure created successfully!"
echo "Solution: $PROJECT_NAME.sln"
echo "Class Library: $LIBRARY_NAME"
echo "Console Application: $CONSOLE_NAME"
echo "Test Project: $TEST_NAME"
echo "Git repository initialized with .gitignore"
echo "VS Code settings, launch, and task configurations added"

# Display project structure
echo -e "\nProject structure:"
find . -type f -name "*.csproj" -o -name "*.sln" | sort
