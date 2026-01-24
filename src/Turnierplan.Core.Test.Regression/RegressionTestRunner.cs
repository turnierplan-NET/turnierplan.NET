using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Turnierplan.Core.Test.Regression;

public sealed class RegressionTestRunner(ITestOutputHelper output)
{
    private static readonly string __outputFolderName = Path.GetFullPath($"test_output/{DateTime.UtcNow.Ticks}/");
    private static readonly Type __selfType = typeof(RegressionTestRunner);

    static RegressionTestRunner()
    {
        if (!Directory.Exists(__outputFolderName))
        {
            Directory.CreateDirectory(__outputFolderName);
        }
    }

    [Theory]
    [RegressionTestData]
    public async Task RegressionTest___When_Executed___Produces_Expected_Result(string regressionTestName)
    {
        var typeName = $"{__selfType.Namespace}.Tests.{regressionTestName}";
        var type = __selfType.Assembly.GetType(typeName) ?? throw new InvalidOperationException($"Test type not found: '{typeName}'");

        if (Activator.CreateInstance(type) is not IRegressionTest regressionTest)
        {
            throw new InvalidOperationException($"Test type could not be instantiated found: '{typeName}'");
        }

        output.WriteLine("The test type {0} was successfully instantiated.", typeName);

        regressionTest.Execute();
        var regressionTestOutput = regressionTest.Result.TrimEnd();
        output.WriteLine("All test steps were successfully executed and the result length is {0}", regressionTestOutput.Length);

        var outputFileName = Path.Join(__outputFolderName, $"{regressionTestName}.out");
        output.WriteLine("Writing test output to: '{0}'", outputFileName);
        await File.WriteAllTextAsync(outputFileName, regressionTestOutput);

        var expectedFileResourceName = $"{__selfType.Namespace}.Expected.{regressionTestName}.out";
        output.WriteLine("Expected test output file: {0}", expectedFileResourceName);
        await using var expectedFileStream = __selfType.Assembly.GetManifestResourceStream(expectedFileResourceName);

        if (expectedFileStream is null)
        {
            throw new InvalidOperationException($"Missing expected test output: '{expectedFileResourceName}'");
        }

        using var streamReader = new StreamReader(expectedFileStream);
        var expectedTestOutput = (await streamReader.ReadToEndAsync()).TrimEnd();
        output.WriteLine("Expected test output was read and has a length of {0}", expectedTestOutput.Length);

        regressionTestOutput.Should().Be(expectedTestOutput);
    }

    [AttributeUsage(AttributeTargets.Method)]
    private sealed class RegressionTestDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var interfaceType = typeof(IRegressionTest);

            return __selfType.Assembly
                .GetTypes()
                .Where(type => type.IsAssignableTo(interfaceType) && type is { IsClass: true, IsAbstract: false })
                .Select(type => type.Name)
                .Select(name => new object[] { name });
        }
    }
}
