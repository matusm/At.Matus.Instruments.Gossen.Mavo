//*****************************************************************************
// This file is part of the At.Matus.Instruments.Gossen.Mavo project.
//
// The Parser class provides methods to parse responses from
// Gossen Mavo light meters
//
// supported devices: MAVOLUX, MAVOMONITOR, MAVO SPOT2
// incompatible devices: MAVOPROBE, MAVOMASTER
//
//*****************************************************************************

namespace At.Matus.Instruments.Gossen.Mavo
{
    internal static class Parser
    {
        public static double ParseMeasurementResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return double.NaN; // Return NaN for invalid input
            string[] parts = SplitAtAllSpacees(input);
            if (parts.Length < 3)
                return double.NaN; // Return NaN if the response is not valid
            string value = parts[0] + parts[1];
            if (double.TryParse(value, out double result))
                return ConvertToSIUnits(result, parts[2]);
            return double.NaN; // Return NaN if parsing fails (also for ERROR responses!)
        }

        public static double ParseOperatingHoursResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return double.NaN; // Return NaN for invalid input
            string[] parts = SplitAtAllKommata(input);
            if (parts.Length < 3)
                return double.NaN; // Return NaN if the response is not valid
            if (!int.TryParse(parts[0], out int hours))
                return double.NaN; // Return NaN if parsing fails
            if (!int.TryParse(parts[1], out int minutes))
                return double.NaN; // Return NaN if parsing fails
            if (!int.TryParse(parts[2], out int seconds))
                return double.NaN; // Return NaN if parsing fails
            int totalSeconds = hours * 3600 + minutes * 60 + seconds; // Convert to total seconds
            double totalHours = (double)totalSeconds / 3600.0; // Convert to total hours
            return totalHours; // Return the parsed value as hours
        }

        public static int ParseRangeResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return -1; // Return -1 for invalid input
            if (int.TryParse(input, out int range))
                return range; // Return the parsed range
            return -1; // Return -1 if parsing fails
        }

        public static string RemoveEcho(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            // Remove the echo part, which is the first part before the first space
            int index = input.IndexOf(' ');
            if (index == -1)
                return input; // No space found, return the original input
            return input.Substring(index + 1).Trim(); // Return everything after the first space
        }

        public static int GetErrorNumber(string response)
        {
            if (response.StartsWith("ERROR = "))
            {
                // Extract the error code from the response
                string errorCodeStr = response.Substring(8).Trim();
                if (int.TryParse(errorCodeStr, out int errorNumber))
                {
                    return errorNumber;
                }
                else
                {
                    return -1; // Non-numerical error code
                }
            }
            return 0; // No error
        }

        public static string[] SplitAtAllKommata(string response)
        {
            string[] parts = response.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts;
        }

        public static string[] SplitAtAllSpacees(string response)
        {
            string[] parts = response.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts;
        }

        public static double ConvertToSIUnits(double value, string unit)
        {
            switch (unit.ToUpperInvariant())
            {
                case "LX":
                    return value; // Already in SI units
                case "FC":
                    return value * 10.76391042; // Convert footcandles to lux
                case "FL":
                    return value * 3.4262591; // Convert footlamberts to cd/m^2   
                case "CD_M2":
                    return value; // Already in SI units
                default:
                    throw new ArgumentException($"Unsupported unit: {unit}");
            }
        }
    }
}
