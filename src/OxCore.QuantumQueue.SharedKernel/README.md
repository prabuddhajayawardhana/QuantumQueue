# OxCore Quantum Queue Shared Kernel

[![NuGet](https://img.shields.io/nuget/v/OxCore.QuantumQueue.SharedKernel.svg)](https://www.nuget.org/packages/OxCore.QuantumQueue.SharedKernel/)
![.NET 9.0](https://img.shields.io/badge/.NET-9.0-blue)

## Overview

**OxCore Quantum Queue Shared Kernel** is an internal support library for the OxCore Quantum Queue ecosystem.  
**Do not install manually.** This package is intended for internal use only.

- **Target Framework:** .NET 9.0
- **Author:** Prabuddha Jayawardhana
- **License:** MIT

## Description

This library provides shared abstractions and utilities used across the OxCore Quantum Queue solution.  
It is not intended for direct consumption or external use.

## Features

- Common interfaces and base types for queue-related operations
- Logging support via [Serilog](https://serilog.net/) and [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions)
- Internal utilities to support the Quantum Queue infrastructure

## Dependencies

- [Microsoft.Extensions.Logging.Abstractions (9.0.4)](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions)
- [Serilog (4.2.0)](https://www.nuget.org/packages/Serilog)
- [Serilog.AspNetCore (9.0.0)](https://www.nuget.org/packages/Serilog.AspNetCore)
- [Serilog.Settings.Configuration (9.0.0)](https://www.nuget.org/packages/Serilog.Settings.Configuration)
- [Serilog.Sinks.Console (6.0.0)](https://www.nuget.org/packages/Serilog.Sinks.Console)
- [Serilog.Sinks.File (6.0.0)](https://www.nuget.org/packages/Serilog.Sinks.File)

## Usage

This package is referenced by other projects within the OxCore Quantum Queue solution.  
No public API is exposed for external use.

## License

This project is licensed under the MIT License.  
See the [LICENSE](../LICENSE) file for details.

---

> **Note:**  
> This is an internal support library for OxCore. Do not install manually.
