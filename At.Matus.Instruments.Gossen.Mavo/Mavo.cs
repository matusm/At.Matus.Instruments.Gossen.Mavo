//*************************************************************************************************
//
// This file is part of the At.Matus.Instruments.Gossen.Mavo library.
// At.Matus.Instruments.Gossen.Mavo is a .NET library for communicating with 
// Gossen light meters.
// It provides methods to query the device, retrieve measurements,
// and handle errors.
//
// supported devices: MAVOLUX, MAVOMONITOR, MAVO SPOT2
// incompatible devices: MAVOPROBE, MAVOMASTER
//
// Attention: -------
// 
// Author: Michael Matus, 2025
//
// Usage:
// 1.) Instantiate the Mavo class with the serial port name of the device:
// 2.) Call the desired methods to interact with the device.
//
// Example:
// Mavo mavo = new Mavo("COM3");
// double photometricValue = mavo.GetPhotometricValue();
// 
// version history
//
// 0.1.10 First working version
// 
//*************************************************************************************************

using System.IO.Ports;

namespace At.Matus.Instruments.Gossen.Mavo
{
    public class Mavo
    {
        public string DevicePort { get; }
        public string InstrumentManufacturer { get; private set; } = "";
        public string InstrumentType { get; private set; } = "";
        public string InstrumentSerialNumber { get; private set; } = "";
        public string InstrumentFirmwareVersion { get; private set; } = "";
        public string InstrumentHardwareRevision { get; private set; } = "";
        public string InstrumentID => $"{InstrumentType} {InstrumentFirmwareVersion} SN:{InstrumentSerialNumber} @ {DevicePort}";
        public string InstrumentCommandInterpreterVersion => Query("SYSTEM:VERSION?");
        public double InstrumentOperatingHours => GetOperatingHours();
        public int InstrumentOperatingDays => (int)Math.Truncate(InstrumentOperatingHours / 24);
        public ErrorHandler ErrorHandler => errorHandler;

        public Mavo(string portName)
        {
            DevicePort = portName.Trim();
            comPort = new SerialPort(DevicePort, 9600, Parity.Even, 7, StopBits.Two)
            {
                Handshake = Handshake.None,
                NewLine = "\n"
            };
            Initialize();
        }

        public double GetPhotometricValue() => Parser.ParseMeasurementResponse(Query("?")); // MEASURE:PHOTOMETRIC?

        public int GetMeasurementRange() => Parser.ParseRangeResponse(Query("RAN?")); // MEASURE:PHOTOMETRIC:RANGE?

        public void SetMeasurementRange(int range)
        {
            // Negative values do not induce an error!
            // But drives the instrument in an undefined state!
            if (range < 0 || range > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(range), "Range must be between 0 and 4.");
            }
            DeselectAutoRange();
            Query($"RAN {range}"); // MEASURE:PHOTOMETRIC:RANGE
        }

        public void SelectAutoRange() => Query("RAN:AUTO ON"); // MEASURE:PHOTOMETRIC:RANGE:AUTO ON

        public void DeselectAutoRange() => Query("RAN:AUTO OFF"); // MEASURE:PHOTOMETRIC:RANGE:AUTO OFF

        public void Reset()
        {
            Query("*RST");
            Thread.Sleep(4000); // Wait for the device to reset
        }

        // By making this method public one can gain full control over the instrument
        public string Query(string command)
        {
            string response = _Query(command);
            errorHandler.HandleError(Parser.GetErrorNumber(response));
            if (errorHandler.ErrorNumber == 1)
            {
                response = _Query(command); // Retry the command if an overrun error occurs
                errorHandler.HandleError(Parser.GetErrorNumber(response));
            }
            return (errorHandler.IsError || (command.Trim() == "?")) ? response : Parser.RemoveEcho(response); // no echo in error responses
        }

        private string _Query(string command)
        {
            OpenPort();
            comPort.WriteLine(command);
            Thread.Sleep(DELAY_AFTERWRITE);
            string response = comPort.ReadLine().Trim();
            ClosePort();
            return response;
        }

        private void OpenPort()
        {
            if (!comPort.IsOpen)
            {
                comPort.Open();
            }
        }

        private void ClosePort()
        {
            if (comPort.IsOpen)
            {
                comPort.Close();
            }
        }

        private void Initialize()
        {
            SetEchoON();    // Enable echo to get responses from the device
            var response = Query("*IDN?");
            if (string.IsNullOrEmpty(response))
            {
                throw new InvalidOperationException("Failed to initialize Mavo: No response from device.");
            }
            var parts = Parser.SplitAtAllKommata(response);
            if (parts.Length < 5)
            {
                throw new InvalidOperationException("Failed to initialize Mavo: Invalid response format.");
            }
            InstrumentManufacturer = parts[0];
            InstrumentType = parts[1];
            InstrumentSerialNumber = parts[2];
            InstrumentHardwareRevision = parts[3];
            InstrumentFirmwareVersion = parts[4];
        }

        private double GetOperatingHours() => Parser.ParseOperatingHoursResponse(Query("TIM?")); // SYSTEM:TIME?

        private void SetEchoON() => Query("ECH ON"); // SYSTEM:ECHO ON

        private const int DELAY_AFTERWRITE = 10; // Delay in milliseconds after writing a command   
        private readonly SerialPort comPort;
        private readonly ErrorHandler errorHandler = new ErrorHandler() { OnError = ErrorReaction.Ignore };

    }
}
