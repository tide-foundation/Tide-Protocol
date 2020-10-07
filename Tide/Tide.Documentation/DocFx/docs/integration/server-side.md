# Server Side

## Prerequisite

The following components are required to be set up ahead of the deployment:  
[.NET Core 2.2 Build apps - SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2 ".net Core 2.2 Download")

## Installation

### Via nuget

In your package manager terminal:

```
Install-Package Tide
```

## Initializing

In startup.cs:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddTideEndpoint(settings.Keys);
    ...
}
```
