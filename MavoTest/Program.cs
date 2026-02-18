using System.Globalization;
using System.Reflection;
using At.Matus.Instruments.Gossen.Mavo;
using At.Matus.StatisticPod;

namespace MavoTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string portname = args.Length > 0 ? args[0] : @"/dev/tty.usbserial-00gossen";
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Mavo mavo = new Mavo(portname);

            string? appName = Assembly.GetExecutingAssembly().GetName().Name;
            Version? appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string appVersionString = $"{appVersion?.Major}.{appVersion?.Minor}";

            Console.WriteLine($"Application: {appName} version {appVersion.ToString()}");
            Console.WriteLine($"Manufacturer:        {mavo.InstrumentManufacturer}");
            Console.WriteLine($"Type:                {mavo.InstrumentType}");
            Console.WriteLine($"Serial Number:       {mavo.InstrumentSerialNumber}");
            Console.WriteLine($"Firmware Version:    {mavo.InstrumentFirmwareVersion}");
            Console.WriteLine($"Hardware Revision:   {mavo.InstrumentHardwareRevision}");
            Console.WriteLine($"Command Interpreter: {mavo.InstrumentCommandInterpreterVersion}");
            Console.WriteLine($"Operating days:      {mavo.InstrumentOperatingDays} d");
            Console.WriteLine($"Device port:         {mavo.DevicePort}");

            mavo.Reset();

            StatisticPod statistic = new StatisticPod();

            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    statistic.Restart();
                    mavo.SetMeasurementRange(j);
                    Console.WriteLine($"Set range to {mavo.GetMeasurementRange()}");
                    Thread.Sleep(500);
                    mavo.GetPhotometricValue(); // Trigger a measurement to clear the error state
                    Thread.Sleep(500);
                    while (statistic.SampleSize < 10)
                    {
                        double value = mavo.GetPhotometricValue();
                        Console.WriteLine($"{statistic.SampleSize:D4} {value} lx      [#errors: {mavo.ErrorHandler.NumberOfErrors}/{mavo.ErrorHandler.TotalNumberOfQueries} ]");
                        statistic.Update(value);
                        Thread.Sleep(500);
                    }
                    Console.WriteLine($"Mean: {statistic.AverageValue} lx, StdDev: {statistic.StandardDeviation} lx, Min: {statistic.MinimumValue} lx, Max: {statistic.MaximumValue} lx");

                }
            }

        }

    }
}

