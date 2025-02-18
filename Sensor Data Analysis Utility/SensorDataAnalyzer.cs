using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using MathNet.Numerics.Statistics;

public class SensorDataAnalyzer
{
    private List<Dictionary<string, string>> _rawData;
    private string[] _headers;
    private HashSet<string> _numericColumns;

    public SensorDataAnalyzer(string csvPath)
    {
        _rawData = new List<Dictionary<string, string>>();
        _numericColumns = new HashSet<string>();
        ReadCsvFile(csvPath);
        IdentifyNumericColumns();
    }

    private void ReadCsvFile(string path)
    {
        using (var reader = new StreamReader(path))
        {
            // Read headers
            _headers = reader.ReadLine()?.Split(',').Select(h => h.Trim()).ToArray() ?? Array.Empty<string>();

            // Read data
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line != null)
                {
                    var values = line.Split(',').Select(v => v.Trim()).ToArray();
                    var row = new Dictionary<string, string>();

                    for (int i = 0; i < Math.Min(_headers.Length, values.Length); i++)
                    {
                        row[_headers[i]] = values[i];
                    }

                    _rawData.Add(row);
                }
            }
        }
    }

    private void IdentifyNumericColumns()
    {
        foreach (var header in _headers)
        {
            // Skip quality indicator columns and timestamp
            if (header.Contains("[quality]") || header == "Timestamp" || header == "Record number")
                continue;

            // Check if the column contains numeric data
            bool isNumeric = _rawData.Any(row => row.ContainsKey(header) &&
                                                !string.IsNullOrEmpty(row[header]) &&
                                                double.TryParse(row[header],
                                                    NumberStyles.Any,
                                                    CultureInfo.InvariantCulture,
                                                    out _));
            if (isNumeric)
            {
                _numericColumns.Add(header);
            }
        }
    }

    public Dictionary<string, Dictionary<string, object>> AnalyzeColumn(string columnName)
    {
        if (!_numericColumns.Contains(columnName))
        {
            return new Dictionary<string, Dictionary<string, object>>
            {
                ["Error"] = new Dictionary<string, object>
                {
                    ["Message"] = "Column is not numeric"
                }
            };
        }

        var columnData = _rawData
            .Where(row => row.ContainsKey(columnName) &&
                         double.TryParse(row[columnName], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            .Select(row => double.Parse(row[columnName], NumberStyles.Any, CultureInfo.InvariantCulture))
            .ToArray();

        // Get quality data if available
        string qualityColumn = $"{columnName} [quality]";
        var qualityData = _headers.Contains(qualityColumn)
            ? _rawData.Select(row => row.ContainsKey(qualityColumn) ? row[qualityColumn] : "").ToArray()
            : null;

        var analysis = new Dictionary<string, Dictionary<string, object>>
        {
            ["BasicStats"] = new Dictionary<string, object>
            {
                ["Count"] = columnData.Length,
                ["Mean"] = columnData.Mean(),
                ["Median"] = columnData.Median(),
                ["StdDev"] = columnData.StandardDeviation(),
                ["Min"] = columnData.Min(),
                ["Max"] = columnData.Max()
            }
        };

        // Add quality statistics if available
        if (qualityData != null)
        {
            var qualityCounts = qualityData
                .GroupBy(q => q)
                .ToDictionary(g => g.Key, g => g.Count());

            analysis["QualityStats"] = new Dictionary<string, object>
            {
                ["QualityDistribution"] = qualityCounts
            };
        }

        return analysis;
    }

    public Dictionary<string, object> AnalyzeCorrelations(string column1, string column2)
    {
        if (!_numericColumns.Contains(column1) || !_numericColumns.Contains(column2))
        {
            return new Dictionary<string, object>
            {
                ["Error"] = "One or both columns are not numeric"
            };
        }

        var data1 = _rawData
            .Where(row => double.TryParse(row[column1], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            .Select(row => double.Parse(row[column1], NumberStyles.Any, CultureInfo.InvariantCulture))
            .ToArray();

        var data2 = _rawData
            .Where(row => double.TryParse(row[column2], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            .Select(row => double.Parse(row[column2], NumberStyles.Any, CultureInfo.InvariantCulture))
            .ToArray();

        // Only calculate correlation if we have matching data points
        var minLength = Math.Min(data1.Length, data2.Length);
        data1 = data1.Take(minLength).ToArray();
        data2 = data2.Take(minLength).ToArray();

        return new Dictionary<string, object>
        {
            ["Correlation"] = Correlation.Pearson(data1, data2),
            ["DataPointsUsed"] = minLength
        };
    }

    public string[] GetNumericColumns() => _numericColumns.ToArray();
    public string[] GetAllHeaders() => _headers;
}