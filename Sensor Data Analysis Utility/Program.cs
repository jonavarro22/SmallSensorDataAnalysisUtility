using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Water Quality Data Analysis Tool");
            Console.WriteLine("------------------------------\n");

            string csvPath = args.Length > 0 ? args[0] : "sensor_data.csv";
            Console.WriteLine($"Analyzing data from: {csvPath}\n");

            var analyzer = new SensorDataAnalyzer(csvPath);
            var numericColumns = analyzer.GetNumericColumns();

            Console.WriteLine("Found numeric columns:");
            Console.WriteLine(string.Join(", ", numericColumns));
            Console.WriteLine();

            // Analyze each numeric column
            foreach (var column in numericColumns)
            {
                Console.WriteLine($"\nAnalyzing {column}:");
                Console.WriteLine(new string('-', column.Length + 10));

                var analysis = analyzer.AnalyzeColumn(column);

                if (analysis.ContainsKey("BasicStats"))
                {
                    Console.WriteLine("Basic Statistics:");
                    foreach (var stat in analysis["BasicStats"])
                    {
                        Console.WriteLine($"{stat.Key}: {stat.Value}");
                    }
                }

                if (analysis.ContainsKey("QualityStats"))
                {
                    Console.WriteLine("\nQuality Distribution:");
                    var qualityDist = (Dictionary<string, int>)analysis["QualityStats"]["QualityDistribution"];
                    foreach (var quality in qualityDist)
                    {
                        Console.WriteLine($"{quality.Key}: {quality.Value} measurements");
                    }
                }
            }

            // Correlation analysis between key parameters
            Console.WriteLine("\nCorrelation Analysis:");
            Console.WriteLine("--------------------");
            var parameters = new[] { "Temperature", "Dissolved Oxygen", "pH", "Chlorophyll" }
                .Where(p => numericColumns.Contains(p))
                .ToArray();

            for (int i = 0; i < parameters.Length; i++)
            {
                for (int j = i + 1; j < parameters.Length; j++)
                {
                    var correlation = analyzer.AnalyzeCorrelations(parameters[i], parameters[j]);
                    Console.WriteLine($"\n{parameters[i]} vs {parameters[j]}:");
                    foreach (var result in correlation)
                    {
                        Console.WriteLine($"{result.Key}: {result.Value}");
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: CSV file not found. Please check the file path.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}