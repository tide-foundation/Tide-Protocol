# Getting Started
### Prerequisite
The following components are required to be set up ahead of the deployment:
1. [.NET Core 2.2 Build apps - SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2 ".net Core 2.2 Download")
1. [Node.js - LTS](https://nodejs.org/en/download/ "node.js Download")
#### Installing Server Side SDK
1. Install the SDK Package:
   ```
   Install-Package Tide
   ```
2. In startup, add Tide to ConfigureServices
   ~~~csharp
  		services.AddTide("VendorId", configuration => configuration  
			.UseSqlServerStorage(settings.Connection)  
			.SetEndpoint("/override")  
			.UseSomethingElse(settings.something)  
		.SetThis(value));  
	// more optional settings will go here  
3. Add Tide to Configure
	~~~csharp
	app.UseTide(); // This creates endpoints for tide-js to interact with
	~~~
#### Installing Client Side SDK
1.  Installing Tide JS through npm
	~~~
	npm install tide-js --save-dev
	~~~
2.  Create Tide instance
	~~~
	var tide = new Tide(yourVendorId, yourVendorBackendApi (append the endpoint if you used an override on your backend));
	~~~
3. Learn about accounts and permissions [here](tide.org)