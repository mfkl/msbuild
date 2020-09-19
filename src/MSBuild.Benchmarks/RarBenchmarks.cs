using BenchmarkDotNet.Attributes;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.UnitTests;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;

namespace MSBuild.Benchmarks
{
    /// <summary>
    /// from https://github.com/dotnet/msbuild/blob/master/src/Tasks.UnitTests/AssemblyDependency/Perf.cs
    /// https://github.com/dotnet/msbuild/blob/master/src/Tasks.UnitTests/AssemblyDependency/Miscellaneous.cs
    /// </summary>
    public class RarBenchmarks
    {
        // retrieved from ResolveAssemblyReferenceTestFixture
        static readonly string s_rootPathPrefix = /* NativeMethodsShared.IsWindows */ true ? "C:\\" : Path.VolumeSeparatorChar.ToString();
        static readonly string s_myLibrariesRootPath = Path.Combine(s_rootPathPrefix, "MyLibraries");
        static readonly string s_myLibraries_V1Path = Path.Combine(s_myLibrariesRootPath, "v1");
        static readonly string s_myLibraries_V2Path = Path.Combine(s_myLibrariesRootPath, "v2");
        ResolveAssemblyReference t;

        [GlobalSetup(Target = nameof(DependeeDirectoryShouldNotBeProbedForDependencyWhenDependencyResolvedExternally))]
        public void DependeeDirectoryShouldNotBeProbedForDependencyWhenDependencyResolvedExternallySetup()
        {
            t = new ResolveAssemblyReference
            {
                BuildEngine = new MockEngine(),
                Assemblies = new ITaskItem[]
                {
                    new TaskItem(@"C:\DependsOnNuget\A.dll"), // depends on N, version 1.0.0.0
                    new TaskItem(@"C:\NugetCache\N\lib\N.dll", // version 2.0.0.0
                    new Dictionary<string, string>
                    {
                        {"ExternallyResolved", "true"}
                    })
                },
                SearchPaths = new[] { "{RawFileName}" },
                AutoUnify = true
            };
        }

        [GlobalSetup(Target = nameof(ConflictBetweenBackAndForeVersionsNotCopyLocal))]
        public void ConflictBetweenBackAndForeVersionsNotCopyLocalSetup()
        {
            t = new ResolveAssemblyReference
            {
                Assemblies = new ITaskItem[]
                {
                    new TaskItem("D, Version=2.0.0.0, Culture=neutral, PublicKeyToken=aaaaaaaaaaaaaaaa"),
                    new TaskItem("D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=aaaaaaaaaaaaaaaa")
                },
                BuildEngine = new MockEngine(),
                SearchPaths = new string[] {
                    s_myLibrariesRootPath, s_myLibraries_V2Path, s_myLibraries_V1Path
                },

            };
        }

        [Benchmark]
        public void ConflictBetweenBackAndForeVersionsNotCopyLocal()
        {
            t.Execute();
        }
        
        [Benchmark]
        public void DependeeDirectoryShouldNotBeProbedForDependencyWhenDependencyResolvedExternally()
        {
            t.Execute();
        }
    }
}
