using BenchmarkDotNet.Attributes;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.UnitTests;
using Microsoft.Build.Utilities;
using System.Collections.Generic;

namespace MSBuild.Benchmarks
{
    /// <summary>
    /// from https://github.com/dotnet/msbuild/blob/master/src/Tasks.UnitTests/AssemblyDependency/Perf.cs
    /// https://github.com/dotnet/msbuild/blob/master/src/Tasks.UnitTests/AssemblyDependency/Miscellaneous.cs
    /// </summary>
    public class RarBenchmarkTest
    {
        ResolveAssemblyReference t;

        [GlobalSetup(Target = nameof(RAR))]
        public void RARSetup()
        {
            var engine = new MockEngine();

            ITaskItem[] assemblyNames =
            {
                new TaskItem(@"C:\DependsOnNuget\A.dll"), // depends on N, version 1.0.0.0
                new TaskItem(@"C:\NugetCache\N\lib\N.dll", // version 2.0.0.0
                    new Dictionary<string, string>
                    {
                        {"ExternallyResolved", "true"}
                    })
            };

            t = new ResolveAssemblyReference
            {
                BuildEngine = engine,
                Assemblies = assemblyNames,
                SearchPaths = new[] { "{RawFileName}" },
                AutoUnify = true
            };
        }

        [Benchmark]
        public void RAR()
        {
            t.Execute();
        }
    }
}
