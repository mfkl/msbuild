using BenchmarkDotNet.Attributes;
using Microsoft.Build.Tasks;

namespace MSBuild.Benchmarks
{
    public class BenchmarkTest
    {
        [Benchmark]
        public void Test()
        {
            ResolveAssemblyReference t = new ResolveAssemblyReference();

            //t.Execute();
        }
    }
}
