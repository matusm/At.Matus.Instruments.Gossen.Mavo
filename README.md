# At.Matus.Instruments.Gossen.Mavo

A .NET library for interfacing with Gossen Mavo series light meters and luminance measuring instruments.

## Overview

This library provides a comprehensive interface for communicating with Gossen Mavo devices, enabling automated measurement and data collection in .NET applications.

## Features

- 🔌 Serial communication with Gossen Mavo instruments
- 📊 Read measurement data (luminance, illuminance)
- ⚙️ Configure device settings
- 📈 Real-time measurement streaming
- 🔄 Synchronous API support
- ✅ Full .NET Standard 2.0+ compatibility

## Installation

```bash
dotnet add package At.Matus.Instruments.Gossen.Mavo
```

Or via NuGet Package Manager:

```
Install-Package At.Matus.Instruments.Gossen.Mavo
```

## Quick Start

```csharp
using At.Matus.Instruments.Gossen.Mavo;

// Create a connection to the device
Mavo mavo = new Mavo("COM3"); // Adjust port as needed

// Read a measurement
double photometricValue = mavo.GetPhotometricValue();
```

## Supported Devices

- Gossen MavoLux 
- Gossen MavoMonitor
- Gossen MavoSpot2

## Requirements

- .NET Standard 2.0 or later
- .NET Framework 4.6.1 or later
- .NET Core 2.0 or later
- .NET 5.0 or later

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or contributions, please open an issue on GitHub.

## Acknowledgments

- Gossen Foto- und Lichtmesstechnik GmbH for the Mavo series instruments

---

**Note**: This library is not officially affiliated with or endorsed by Gossen Foto- und Lichtmesstechnik GmbH.
```
