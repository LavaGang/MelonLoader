using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MelonLoader.InternalUtils
{
    internal class DependencyGraph<T> where T : MelonBase
    {
        public static void TopologicalSort(IList<T> melons)
        {
            if (melons.Count <= 0)
                return;
            DependencyGraph<T> dependencyGraph = new DependencyGraph<T>(melons);
            melons.Clear();
            dependencyGraph.TopologicalSortInto(melons);
        }

        private readonly Vertex[] vertices;

        private DependencyGraph(IList<T> melons)
        {
            int size = melons.Count;
            vertices = new Vertex[size];
            Dictionary<string, Vertex> nameLookup = new Dictionary<string, Vertex>(size);

            // Create a vertex in the dependency graph for each Melon to load
            for (int i = 0; i < size; ++i)
            {
                Assembly melonAssembly = melons[i].MelonAssembly.Assembly;
                string melonName = melons[i].Info.Name;

                Vertex melonVertex = new Vertex(i, melons[i], melonName);
                vertices[i] = melonVertex;
                nameLookup[melonAssembly.GetName().Name] = melonVertex;
            }

            // Add an edge for each dependency between Melons
            SortedDictionary<string, IList<AssemblyName>> melonsWithMissingDeps = new SortedDictionary<string, IList<AssemblyName>>();
            SortedDictionary<string, IList<AssemblyName>> melonsWithIncompatibilities = new SortedDictionary<string, IList<AssemblyName>>();
            List<AssemblyName> missingDependencies = new List<AssemblyName>();
            List<AssemblyName> incompatibilities = new List<AssemblyName>();
            HashSet<string> optionalDependencies = new HashSet<string>();
            HashSet<string> additionalDependencies = new HashSet<string>();

            foreach (Vertex melonVertex in vertices)
            {
                Assembly melonAssembly = melonVertex.melon.MelonAssembly.Assembly;
                missingDependencies.Clear();
                optionalDependencies.Clear();
                incompatibilities.Clear();
                additionalDependencies.Clear();

                MelonOptionalDependenciesAttribute optionals = (MelonOptionalDependenciesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonOptionalDependenciesAttribute));
                if (optionals != null
                    && optionals.AssemblyNames != null)
                    optionalDependencies.UnionWith(optionals.AssemblyNames);

                MelonAdditionalDependenciesAttribute additionals = (MelonAdditionalDependenciesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonAdditionalDependenciesAttribute));
                if (additionals != null
                    && additionals.AssemblyNames != null)
                    additionalDependencies.UnionWith(additionals.AssemblyNames);

                MelonIncompatibleAssembliesAttribute incompatibleAssemblies = (MelonIncompatibleAssembliesAttribute)Attribute.GetCustomAttribute(melonAssembly, typeof(MelonIncompatibleAssembliesAttribute));
                if (incompatibleAssemblies != null
                    && incompatibleAssemblies.AssemblyNames != null)
                {
                    foreach (string name in incompatibleAssemblies.AssemblyNames)
                        foreach (Vertex v in vertices)
                        {
                            AssemblyName assemblyName = v.melon.MelonAssembly.Assembly.GetName();
                            if (v != melonVertex
                                && assemblyName.Name == name)
                            {
                                incompatibilities.Add(assemblyName);
                                v.skipLoading = true;
                            }
                        }
                }

                foreach (AssemblyName dependency in melonAssembly.GetReferencedAssemblies())
                {
                    if (nameLookup.TryGetValue(dependency.Name, out Vertex dependencyVertex))
                    {
                        melonVertex.dependencies.Add(dependencyVertex);
                        dependencyVertex.dependents.Add(melonVertex);
                    }
                    else if (!TryLoad(dependency) && !optionalDependencies.Contains(dependency.Name))
                        missingDependencies.Add(dependency);
                }

                foreach (string dependencyName in additionalDependencies)
                {
                    AssemblyName dependency = new AssemblyName(dependencyName);
                    if (nameLookup.TryGetValue(dependencyName, out Vertex dependencyVertex))
                    {
                        melonVertex.dependencies.Add(dependencyVertex);
                        dependencyVertex.dependents.Add(melonVertex);
                    }
                    else if (!TryLoad(dependency))
                        missingDependencies.Add(dependency);
                }

                if (missingDependencies.Count > 0)
                    // melonVertex.skipLoading = true;
                    melonsWithMissingDeps.Add(melonVertex.melon.Info.Name, missingDependencies.ToArray());

                if (incompatibilities.Count > 0)
                    melonsWithIncompatibilities.Add(melonVertex.melon.Info.Name, incompatibilities.ToArray());
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
                Assembly.Load(assembly);
                return true;
            }
            catch (FileNotFoundException) { return false; }
            catch (Exception ex)
            {
                MelonLogger.Error("Loading Melon Dependency Failed: " + ex);
                return false;
            }
        }

        private static string BuildMissingDependencyMessage(IDictionary<string, IList<AssemblyName>> melonsWithMissingDeps)
        {
            StringBuilder messageBuilder = new StringBuilder("Some Melons are missing dependencies, which you may have to install.\n" +
                "If these are optional dependencies, mark them as optional using the MelonOptionalDependencies attribute.\n" +
                "This warning will turn into an error and Melons with missing dependencies will not be loaded in the next version of MelonLoader.\n");
            foreach (string melonName in melonsWithMissingDeps.Keys)
            {
                messageBuilder.Append($"- '{melonName}' is missing the following dependencies:\n");
                foreach (AssemblyName dependency in melonsWithMissingDeps[melonName])
                    messageBuilder.Append($"    - '{dependency.Name}' v{dependency.Version}\n");
            }
            messageBuilder.Length -= 1; // Remove trailing newline
            return messageBuilder.ToString();
        }

        private static string BuildIncompatibleAssembliesMessage(IDictionary<string, IList<AssemblyName>> melonsWithIncompatibilities)
        {
            StringBuilder messageBuilder = new StringBuilder("Some Melons are marked as incompatible with each other.\n" +
                "To avoid any errors, these Melons will not be loaded.\n");
            foreach (string melonName in melonsWithIncompatibilities.Keys)
            {
                messageBuilder.Append($"- '{melonName}' is incompatible with the following Melons:\n");
                foreach (AssemblyName dependency in melonsWithIncompatibilities[melonName])
                {
                    messageBuilder.Append($"    - '{dependency.Name}'\n");
                }
            }
            messageBuilder.Length -= 1; // Remove trailing newline
            return messageBuilder.ToString();
        }

        private void TopologicalSortInto(IList<T> loadedMelons)
        {
            int[] unloadedDependencies = new int[vertices.Length];
            SortedList<string, Vertex> loadableMelons = new SortedList<string, Vertex>();
            int skippedMelons = 0;

            // Find all sinks in the dependency graph, i.e. Melons without any dependencies on other Melons
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vertex vertex = vertices[i];
                int dependencyCount = vertex.dependencies.Count;

                unloadedDependencies[i] = dependencyCount;
                if (dependencyCount == 0)
                    loadableMelons.Add(vertex.name, vertex);
            }

            // Perform the (reverse) topological sorting
            while (loadableMelons.Count > 0)
            {
                Vertex melon = loadableMelons.Values[0];
                loadableMelons.RemoveAt(0);

                if (!melon.skipLoading)
                    loadedMelons.Add(melon.melon);
                else
                    ++skippedMelons;

                foreach (Vertex dependent in melon.dependents)
                {
                    unloadedDependencies[dependent.index] -= 1;
                    dependent.skipLoading |= melon.skipLoading;

                    if (unloadedDependencies[dependent.index] == 0)
                        loadableMelons.Add(dependent.name, dependent);
                }
            }

            // Check if all Melons were either loaded or skipped. If this is not the case, there is a cycle in the dependency graph
            if (loadedMelons.Count + skippedMelons < vertices.Length)
            {
                StringBuilder errorMessage = new StringBuilder("Some Melons could not be loaded due to a cyclic dependency:\n");
                for (int i = 0; i < vertices.Length; ++i)
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

                dependencies = new List<Vertex>();
                dependents = new List<Vertex>();
                skipLoading = false;
            }
        }
    }
}