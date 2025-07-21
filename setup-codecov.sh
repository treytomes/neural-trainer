#!/bin/bash

# Usage: ./setup-codecov.sh testProjectName

if [ -z "$1" ]; then
  echo "Usage: ./setup-codecov.sh <TestProjectName>"
  exit 1
fi

TEST_PROJECT="$1"
BASE_DIR=$(pwd)
VSCODE_DIR="$BASE_DIR/.vscode"

echo "Setting up code coverage for test project: $TEST_PROJECT"

# Step 1: Install required packages
echo "Installing required packages..."
cd "$BASE_DIR/$TEST_PROJECT"
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild
dotnet add package ReportGenerator

# Step 2: Update the test project file
echo "Updating $TEST_PROJECT.csproj..."
if grep -q "</Project>" "$TEST_PROJECT.csproj"; then
  # Check if the PropertyGroup for coverage already exists
  if ! grep -q "<CollectCoverage>" "$TEST_PROJECT.csproj"; then
    # Insert PropertyGroup before the closing Project tag
    sed -i -e 's|</Project>|  <PropertyGroup>\n    <CollectCoverage>true</CollectCoverage>\n    <CoverletOutputFormat>cobertura</CoverletOutputFormat>\n    <CoverletOutput>$(MSBuildThisFileDirectory)../TestResults/</CoverletOutput>\n    <Exclude>[xunit.*]*</Exclude>\n  </PropertyGroup>\n</Project>|' "$TEST_PROJECT.csproj"
    echo "Added code coverage properties to $TEST_PROJECT.csproj"
  else
    echo "Code coverage properties already exist in $TEST_PROJECT.csproj"
  fi
else
  echo "Error: Could not find closing Project tag in the csproj file."
  exit 1
fi

cd "$BASE_DIR"

# Step 3: Create .vscode directory if it doesn't exist
mkdir -p "$VSCODE_DIR"

# Function to add settings to settings.json
add_settings_to_json() {
    local file="$VSCODE_DIR/settings.json"

    # Create backup if file exists
    if [ -f "$file" ]; then
        cp "$file" "${file}.bak"
        echo "Backup created: ${file}.bak"
    else
        # Create an empty JSON object if file doesn't exist
        echo "{}" > "$file"
    fi

    # Create a temporary file
    local temp_file="${file}.tmp"

    # The settings we want to add
    local settings=$(cat << EOF
  "dotnet-test-explorer.testProjectPath": "**/*Tests.csproj",
  "dotnet-test-explorer.testArguments": "/p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./TestResults/lcov.info",
  "coverage-gutters.lcovname": "TestResults/lcov.info",
  "coverage-gutters.showLineCoverage": true,
  "coverage-gutters.showRulerCoverage": true,
  "coverage-gutters.highlightdark": "rgba(45, 121, 10, 0.5)",
  "coverage-gutters.noHighlightDark": "rgba(121, 31, 10, 0.5)"
EOF
)

    # Process the file: remove last closing brace, add settings, close with brace
    sed '$d' "$file" > "$temp_file"

    # Add comma if the last line doesn't end with { or [
    if [ -s "$temp_file" ] && grep -q '[^{[,]$' "$temp_file"; then
        echo "," >> "$temp_file"
    fi

    # Add our settings
    echo "$settings" >> "$temp_file"
    echo "}" >> "$temp_file"

    # Replace the original
    mv "$temp_file" "$file"
    echo "Updated $file with code coverage settings"
}

# Function to add tasks to tasks.json
add_tasks_to_json() {
    local file="$VSCODE_DIR/tasks.json"
    local tasks_content

    # Our coverage tasks as JSON
    tasks_content=$(cat << EOF
[
    {
      "label": "Generate Coverage Report",
      "command": "dotnet",
      "type": "process",
      "args": [
        "reportgenerator",
        "-reports:TestResults/coverage.cobertura.xml",
        "-targetdir:TestResults/CoverageReport",
        "-reporttypes:Html"
      ],
      "problemMatcher": []
    },
    {
      "label": "Run Tests with Coverage",
      "command": "dotnet",
      "type": "process",
      "args": [
        "test",
        "\${workspaceFolder}/${TEST_PROJECT}",
        "/p:CollectCoverage=true",
        "/p:CoverletOutputFormat=\"cobertura,lcov\"",
        "/p:CoverletOutput=\${workspaceFolder}/TestResults/"
      ],
      "problemMatcher": "\$msCompile",
      "group": {
        "kind": "test",
        "isDefault": true
      }
    }
]
EOF
)

    # Create backup if file exists
    if [ -f "$file" ]; then
        cp "$file" "${file}.bak"
        echo "Backup created: ${file}.bak"
    else
        # Create a basic tasks.json if it doesn't exist
        cat << EOF > "$file"
{
  "version": "2.0.0",
  "tasks": []
}
EOF
    fi

    # Check if our tasks already exist
    if grep -q "Generate Coverage Report" "$file" || grep -q "Run Tests with Coverage" "$file"; then
        echo "Coverage tasks already exist in tasks.json"
        return
    fi

    # Using a more reliable Python approach if available
    if command -v python3 &>/dev/null; then
        echo "Using Python to update tasks.json..."
        python3 - "$file" "$tasks_content" << 'EOF'
import json
import sys

file_path = sys.argv[1]
new_tasks_json = sys.argv[2]

# Load the existing tasks file
with open(file_path, 'r') as f:
    try:
        tasks_file = json.load(f)
    except json.JSONDecodeError:
        # If the file is not valid JSON, create a new one
        tasks_file = {"version": "2.0.0", "tasks": []}

# Load the new tasks
new_tasks = json.loads(new_tasks_json)

# Ensure tasks key exists
if "tasks" not in tasks_file:
    tasks_file["tasks"] = []

# Add the new tasks
if tasks_file["tasks"]:
    # Add a comma if there are existing tasks
    tasks_file["tasks"].extend(new_tasks)
else:
    tasks_file["tasks"] = new_tasks

# Write the updated file
with open(file_path, 'w') as f:
    json.dump(tasks_file, f, indent=2)
EOF
        if [ $? -ne 0 ]; then
            echo "Python error occurred. Using fallback method..."
            use_fallback=true
        else
            echo "Successfully updated tasks.json with Python"
            return
        fi
    else
        use_fallback=true
    fi

    # Fallback approach if Python is not available or failed
    if [ "$use_fallback" = true ]; then
        echo "Using fallback approach to update tasks.json..."

        # Read the current file
        local file_content=$(<"$file")
        local temp_file="${file}.tmp"

        # Look for tasks array pattern
        if [[ $file_content =~ \"tasks\"[[:space:]]*:[[:space:]]*\[ ]]; then
            # Tasks array exists - check if it's empty
            if [[ $file_content =~ \"tasks\"[[:space:]]*:[[:space:]]*\[[[:space:]]*\] ]]; then
                # Empty tasks array - replace it with our tasks
                sed 's/\"tasks\"[[:space:]]*:[[:space:]]*\[[[:space:]]*\]/\"tasks\": '"$tasks_content"'/' "$file" > "$temp_file"
            else
                # Non-empty tasks array - add our tasks to it
                # Extract beginning of file until tasks array opening bracket
                local start_part=$(sed -n '1,/\"tasks\"[[:space:]]*:[[:space:]]*\[/p' "$file")
                # Extract end of file from first task to end
                local end_part=$(sed -n '/\"tasks\"[[:space:]]*:[[:space:]]*\[/,${p}' "$file" | sed '1d')

                # Combine with our tasks
                echo "$start_part" > "$temp_file"
                # Remove [ from tasks_content and first line of end_part
                echo "${tasks_content:1}" | sed '$ s/]/,/' >> "$temp_file"
                echo "$end_part" >> "$temp_file"
            fi
        else
            # No tasks array found - create a new file structure
            cat << EOF > "$temp_file"
{
  "version": "2.0.0",
  "tasks": $tasks_content
}
EOF
        fi

        mv "$temp_file" "$file"
        echo "Updated $file with code coverage tasks using fallback method"
    fi
}

# Execute the functions
echo "Updating VS Code settings.json..."
add_settings_to_json

echo "Updating VS Code tasks.json..."
add_tasks_to_json

echo "Creating TestResults directory..."
mkdir -p "$BASE_DIR/TestResults"

echo "===================================="
echo "✅ Code coverage setup is complete!"
echo "===================================="
echo ""
echo "Please install these VS Code extensions:"
echo "  1. .NET Core Test Explorer"
echo "  2. Coverage Gutters"
echo ""
echo "Run tests with coverage:        Run 'Run Tests with Coverage' task"
echo "View coverage in editor:        Run 'Coverage Gutters: Watch' from Command Palette"
echo "Generate HTML coverage report:  Run 'Generate Coverage Report' task"
echo ""
echo "Note: Backups of your original files have been created as .bak files"
