﻿using MelonLoader.Resolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace MelonLoader.InternalUtils;

internal class DependencyGraph<T> where T : MelonBase
{
    public static void TopologicalSort(IList<T> melons)
    {
        if (melons.Count <= 0)
            return;
        var dependencyGraph = new DependencyGraph<T>(melons);
        melons.Clear();
        dependencyGraph.TopologicalSortInto(melons);
    }

    private readonly Vertex[] vertices;

    private DependencyGraph(IList<T> melons)
    {
        var size = melons.Count;
        vertices = new Vertex[size];
        var nameLookup = new Dictionary<string, Vertex>(size);

        // Create a vertex in the dependency graph for each Melon to load
        for (var i = 0; i < size; ++i)
        {
            var melonAssembly = melons[i].MelonAssembly.Assembly;
            var melonName = melons[i].Info.Name;

            var melonVertex = new Vertex(i, melons[i], melonName);
            vertices[i] = melonVertex;
            nameLookup[melonAssembly.GetName().Name] = melonVertex;
        }

        // Add an edge for each dependency between Melons
        SortedDictionary<string, IList<AssemblyName>> melonsWithMissingDeps = [];
        SortedDictionary<string, IList<AssemblyName>> melonsWithIncompatibilities = [];
        List<AssemblyName> missingDependencies = [];
        List<AssemblyName> incompatibilities = [];
        HashSet<string> optionalDependencies = [];
        HashSet<string> additionalDependencies = [];

        foreach (var melonVertex in vertices)
        {
            var melonAssembly = melonVertex.melon.MelonAssembly.Assembly;
            missingDependencies.Clear();
            optionalDependencies.Clear();
            incompatibilities.Clear();
            additionalDependencies.Clear();

            var optionals = (MelonOptionalDependenciesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonOptionalDependenciesAttribute));
            if (optionals != null
                && optionals.AssemblyNames != null)
                optionalDependencies.UnionWith(optionals.AssemblyNames);

            var additionals = (MelonAdditionalDependenciesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonAdditionalDependenciesAttribute));
            if (additionals != null
                && additionals.AssemblyNames != null)
                additionalDependencies.UnionWith(additionals.AssemblyNames);

            var incompatibleAssemblies = (MelonIncompatibleAssembliesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonIncompatibleAssembliesAttribute));
            if (incompatibleAssemblies != null
                && incompatibleAssemblies.AssemblyNames != null)
            {
                foreach (var name in incompatibleAssemblies.AssemblyNames)
                    foreach (var v in vertices)
                    {
                        var assemblyName = v.melon.MelonAssembly.Assembly.GetName();
                        if (v != melonVertex
                            && assemblyName.Name == name)
                        {
                            incompatibilities.Add(assemblyName);
                            v.skipLoading = true;
                        }
                    }
            }

            foreach (var dependency in melonAssembly.GetReferencedAssemblies())
            {
                if (nameLookup.TryGetValue(dependency.Name, out var dependencyVertex))
                {
                    if (!melonVertex.dependencies.Contains(dependencyVertex))
                        melonVertex.dependencies.Add(dependencyVertex);
                    if (!dependencyVertex.dependents.Contains(melonVertex))
                        dependencyVertex.dependents.Add(melonVertex);
                }
                else if (!TryLoad(dependency)
                    && !TryResolve(dependency)
                    && !optionalDependencies.Contains(dependency.Name)
                    && !missingDependencies.Contains(dependency))
                    missingDependencies.Add(dependency);
            }

            foreach (var dependencyName in additionalDependencies)
            {
                var dependency = new AssemblyName(dependencyName);
                if (nameLookup.TryGetValue(dependencyName, out var dependencyVertex))
                {
                    if (!melonVertex.dependencies.Contains(dependencyVertex))
                        melonVertex.dependencies.Add(dependencyVertex);
                    if (!dependencyVertex.dependents.Contains(melonVertex))
                        dependencyVertex.dependents.Add(melonVertex);
                }
                else if (!TryLoad(dependency)
                    && !TryResolve(dependency)
                    && !missingDependencies.Contains(dependency))
                    missingDependencies.Add(dependency);
            }

            if ((missingDependencies.Count > 0)
                && !melonsWithMissingDeps.ContainsKey(melonVertex.melon.Info.Name))
                // melonVertex.skipLoading = true;
                melonsWithMissingDeps.Add(melonVertex.melon.Info.Name, [.. missingDependencies]);

            if ((incompatibilities.Count > 0)
                && !melonsWithIncompatibilities.ContainsKey(melonVertex.melon.Info.Name))
                melonsWithIncompatibilities.Add(melonVertex.melon.Info.Name, [.. incompatibilities]);
        }

        // Some Melons are missing dependencies. Don't load these Melons and show an error message
        if (melonsWithMissingDeps.Count > 0)
            MelonLogger.Warning(BuildMissingDependencyMessage(melonsWithMissingDeps));

        if (melonsWithIncompatibilities.Count > 0)
            MelonLogger.Warning(BuildIncompatibleAssembliesMessage(melonsWithIncompatibilities));
    }

    // Returns true if 'assembly' was already loaded or could be loaded, false if the required assembly was missing.
    private static bool TryLoad(AssemblyName assembly)
    {
        try
        {
#if NET6_0_OR_GREATER
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyName(assembly);
#else
            var asm = Assembly.Load(assembly);
#endif

            return asm != null;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Loading Melon Dependency Failed: " + ex);
            return false;
        }
    }

    // Returns true if 'assembly' was already resolved or could be resolved, false if the required assembly was missing.
    private static bool TryResolve(AssemblyName assembly)
    {
        try
        {
            var asm = SearchDirectoryManager.Scan(assembly.Name);
            return asm != null;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            MelonLogger.Error("Resolving Melon Dependency Failed: " + ex);
            return false;
        }
    }

    private static string BuildMissingDependencyMessage(IDictionary<string, IList<AssemblyName>> melonsWithMissingDeps)
    {
        var messageBuilder = new StringBuilder("Some Melons are missing dependencies, which you may have to install.\n" +
            "If these are optional dependencies, mark them as optional using the MelonOptionalDependencies attribute.\n" +
            "This warning will turn into an error and Melons with missing dependencies will not be loaded in future versions of MelonLoader.\n");
        foreach (var melonName in melonsWithMissingDeps.Keys)
        {
            messageBuilder.Append($"- '{melonName}' is missing the following dependencies:\n");
            foreach (var dependency in melonsWithMissingDeps[melonName])
                messageBuilder.Append($"    - '{dependency.Name}' v{dependency.Version}\n");
        }

        messageBuilder.Length -= 1; // Remove trailing newline
        return messageBuilder.ToString();
    }

    private static string BuildIncompatibleAssembliesMessage(IDictionary<string, IList<AssemblyName>> melonsWithIncompatibilities)
    {
        var messageBuilder = new StringBuilder("Some Melons are marked as incompatible with each other.\n" +
            "To avoid any errors, these Melons will not be loaded.\n");
        foreach (var melonName in melonsWithIncompatibilities.Keys)
        {
            messageBuilder.Append($"- '{melonName}' is incompatible with the following Melons:\n");
            foreach (var dependency in melonsWithIncompatibilities[melonName])
            {
                messageBuilder.Append($"    - '{dependency.Name}'\n");
            }
        }

        messageBuilder.Length -= 1; // Remove trailing newline
        return messageBuilder.ToString();
    }

    private void TopologicalSortInto(IList<T> loadedMelons)
    {
        var unloadedDependencies = new int[vertices.Length];
        SortedList<string, Vertex> loadableMelons = [];
        var skippedMelons = 0;

        // Find all sinks in the dependency graph, i.e. Melons without any dependencies on other Melons
        for (var i = 0; i < vertices.Length; ++i)
        {
            var vertex = vertices[i];
            var dependencyCount = vertex.dependencies.Count;

            unloadedDependencies[i] = dependencyCount;
            if ((dependencyCount == 0)
                && !loadableMelons.ContainsKey(vertex.name))
                loadableMelons.Add(vertex.name, vertex);
        }

        // Perform the (reverse) topological sorting
        while (loadableMelons.Count > 0)
        {
            var melon = loadableMelons.Values[0];
            loadableMelons.RemoveAt(0);

            if (!melon.skipLoading
                && !loadableMelons.ContainsKey(melon.name))
                loadedMelons.Add(melon.melon);
            else
                ++skippedMelons;

            foreach (var dependent in melon.dependents)
            {
                unloadedDependencies[dependent.index] -= 1;
                dependent.skipLoading |= melon.skipLoading;

                if ((unloadedDependencies[dependent.index] == 0)
                    && !loadableMelons.ContainsKey(dependent.name))
                    loadableMelons.Add(dependent.name, dependent);
            }
        }

        // Check if all Melons were either loaded or skipped. If this is not the case, there is a cycle in the dependency graph
        if (loadedMelons.Count + skippedMelons < vertices.Length)
        {
            var errorMessage = new StringBuilder("Some Melons could not be loaded due to a cyclic dependency:\n");
            for (var i = 0; i < vertices.Length; ++i)
                if (unloadedDependencies[i] > 0)
                    errorMessage.Append($"- '{vertices[i].name}'\n");
            errorMessage.Length -= 1; // Remove trailing newline
            MelonLogger.Error(errorMessage.ToString());
        }
    }

    private class Vertex
    {
        internal readonly int index;
        internal readonly T melon;
        internal readonly string name;

        internal readonly List<Vertex> dependencies;
        internal readonly List<Vertex> dependents;
        internal bool skipLoading;

        internal Vertex(int index, T melon, string name)
        {
            this.index = index;
            this.melon = melon;
            this.name = name;

            dependencies = [];
            dependents = [];
            skipLoading = false;
        }
    }
}