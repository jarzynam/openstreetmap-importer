using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using OpenStreetMap.Importer.Importer;
using OpenStreetMap.Infrastructure;


Console.WriteLine(">>>Start Importing Open Street Map Files to Mongo<<<");
var openStreetMapDataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "OpenStreetMapFiles");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
var mongoDbConnectionString = configuration["MongoDb"];


var placesRepository = await InfrastructureModule.CreatePlacesRepository(mongoDbConnectionString ?? 
                                                                         throw new ConfigurationErrorsException("missing MongoDB connection string"));
var importer = new OpenStreetMapImporter(placesRepository);
var files = Directory.GetFiles(openStreetMapDataDirectory, "*.pbf");

Console.WriteLine($"Found {files.Length} .pbf files in directory {openStreetMapDataDirectory}");

var stopWatch = new Stopwatch();
foreach (var file in files)
{
    stopWatch.Restart();
    Console.WriteLine($"Start processing {file}");
    await importer.ImportAsync(file);

    Console.WriteLine($"Completed processing file in {stopWatch.Elapsed}");
}

Console.WriteLine(">>> Completed execution. You can find results in Mongo-Express<<<");