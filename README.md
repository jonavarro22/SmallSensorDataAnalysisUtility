# Water Quality Analysis Tool

A C# application for analyzing water quality sensor data, providing statistical analysis and correlation detection for multiple water quality parameters.

## Features

- Automatic detection of numeric sensor data columns
- Statistical analysis of water quality parameters including:
  - Basic statistics (mean, median, standard deviation, min, max)
  - Quality indicator tracking
  - Correlation analysis between parameters
- Handles common water quality parameters:
  - Temperature
  - Dissolved Oxygen
  - pH
  - Chlorophyll
  - Salinity
  - Turbidity
  - Water Speed and Direction
  - Specific Conductance

## Prerequisites

- .NET 6.0 or higher
- MathNet.Numerics package

## Installation

1. Clone this repository:
```bash
git clone https://github.com/jonavarro22/SmallSensorDataAnalysisUtility.git
```

2. Install required NuGet package:
```bash
dotnet add package MathNet.Numerics
```

## Usage

1. Prepare your CSV file with water quality data. The program supports the following format:
```csv
Timestamp,Record number,Parameter1,Parameter1 [quality],Parameter2,Parameter2 [quality],...
```

2. Run the program with your CSV file:
```bash
dotnet run your_data.csv
```

If no file is specified, the program will look for `sensor_data.csv` in the current directory.

## Input Data Format

The analyzer expects CSV files with:
- Headers in the first row
- Optional quality indicators in `[quality]` columns
- Numeric data for sensor measurements
- Timestamp column (optional)
- Record number column (optional)

Example of valid CSV format:
```csv
Timestamp,Record number,Temperature,Temperature [quality],pH,pH [quality]
2024-02-18 10:00,1,25.4,Good,7.2,Good
2024-02-18 10:15,2,25.6,Good,7.3,Poor
```

## Output

The program provides:
1. List of detected numeric columns
2. For each parameter:
   - Basic statistical analysis
   - Quality distribution (if quality indicators are present)
3. Correlation analysis between key parameters

## Project Structure

- `Program.cs`: Main entry point and user interface
- `SensorDataAnalyzer.cs`: Core analysis functionality
- `README.md`: This documentation file

## Contributing

Feel free to open issues or submit pull requests with improvements.
