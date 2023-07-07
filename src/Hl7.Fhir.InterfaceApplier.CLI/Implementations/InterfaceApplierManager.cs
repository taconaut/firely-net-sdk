﻿using Hl7.Fhir.InterfaceApplier.CLI.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace Hl7.Fhir.InterfaceApplier.CLI.Implementations;

public class InterfaceApplierManager : IInterfaceApplierManager
{
    #region Private Fields

    private readonly IInterfaceDiscoveryService _interfaceDiscoveryService;
    private readonly IProjectManager _projectManager;
    private readonly IClassDiscoveryService _classDiscoveryService;
    private readonly IInterfaceApplierService _interfaceApplierService;
    private readonly ILogger<InterfaceApplierManager> _logger;

    #endregion

    #region Constructors

    public InterfaceApplierManager(
        IProjectManager projectManager,
        IInterfaceDiscoveryService interfaceDiscoveryService,
        IClassDiscoveryService classDiscoveryService,
        IInterfaceApplierService interfaceApplierService,
        ILogger<InterfaceApplierManager> logger)
    {
        _projectManager = projectManager;
        _interfaceDiscoveryService = interfaceDiscoveryService;
        _classDiscoveryService = classDiscoveryService;
        _interfaceApplierService = interfaceApplierService;
        _logger = logger;
    }

    #endregion

    #region Public Methods

    public void Run()
    {
        var fhirBaseProjectDirectory = _projectManager.GetFhirBaseProjectDirectory();
        var fhirConformanceProjectDirectory = _projectManager.GetFhirConformanceProjectDirectory();

        foreach (var projectVersionPath in _projectManager.GetFhirVersionProjectFilePaths())
        {
            applyInterfacesToProject(projectVersionPath, fhirBaseProjectDirectory, fhirConformanceProjectDirectory);
        }
    }

    #endregion

    #region Private Methods

    private void applyInterfacesToProject(string projectFilePath, string fhirBaseProjectDirectory, string fhirConformanceProjectDirectory)
    {
        _logger.LogInformation("Start applying interfaces on classes for project {0}", projectFilePath);

        (string projectBuildDirectory, string projectDirectory) = _projectManager.PublishProject(projectFilePath);
        _logger.LogInformation("Published project to {0}", projectBuildDirectory);

        // Load the assemblies in their own context as we will load e.g. Hl7.Fhir.Base multiple times, once for each version
        var context = new AssemblyLoadContext("ApplyInterfacesToProjectContext", isCollectible: true);

        var projectAssemblies = loadAssembliesForProject(projectBuildDirectory, context).ToList();
        logEntriesFormatted("Loaded assemblies from published project", projectAssemblies.Select(assembly => assembly.GetName().FullName));

        var interfaceTypesToApply = _interfaceDiscoveryService.GetInterfaceTypesToApply(projectAssemblies).ToList();
        logEntriesFormatted("Found interfaces, marked with attribute [ApplyInterfaceToGeneratedClasses]", interfaceTypesToApply.Select(type => type.FullName!));

        var classTypesForInterface = _classDiscoveryService.GetClassTypesToApplyInterfaceTo(projectAssemblies, interfaceTypesToApply);
        if (classTypesForInterface.Any())
        {
            logEntriesFormatted("Start applying interfaces on classes", classTypesForInterface.Select(kv => $"{kv.Key.Name}: {kv.Value.Count} classes"));
        }
        else
        {
            _logger.LogInformation("No classes found to apply interfaces on");
        }

        // Consider sources for the project itself as well as Base and Conformance, if they are referenced by the project.
        // R4, R4B and R5 reference Conformance which references Base
        // STU3 references Base directly
        var loadedAssemblyNames = projectAssemblies.Select(assembly => assembly.GetName().Name!);
        var sourceFilesDirectories = new[] { projectDirectory, fhirBaseProjectDirectory,  fhirConformanceProjectDirectory }
            .Where(directory => loadedAssemblyNames.Any(directory.EndsWith));

        _interfaceApplierService.Apply(sourceFilesDirectories, classTypesForInterface);

        context.Unload();

        _logger.LogInformation("Finished applying interfaces on classes");
    }

    private static IEnumerable<Assembly> loadAssembliesForProject(string projectBuildDirectory,
        AssemblyLoadContext context) =>
        Directory.GetFiles(projectBuildDirectory, "*.dll")
            .Select(context.LoadFromAssemblyPath)
            .ToList();

    private void logEntriesFormatted(string message, IEnumerable<string> entries,
        LogLevel logLevel = LogLevel.Information)
        => _logger.Log(logLevel, $"{message}:{Environment.NewLine}{string.Join(Environment.NewLine, entries.Select(line => $"   - {line}"))}");

    #endregion
}