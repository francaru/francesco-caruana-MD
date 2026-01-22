using System.Reflection;

namespace ServiceB.Tests;

[TestClass]
public sealed class ServiceBTests
{
    [TestMethod]
    public void Main_WhenCalled_PrintsGreeting()
    {
        var output = ConsoleCapture.CaptureConsoleOutput(() =>
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            typeof(ServiceB).GetMethod(
                name: "Main",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Static
            ).Invoke(
                obj: null,
                parameters: [Array.Empty<string>()]
            );
        });

        Assert.Contains("Hello, World! I am Service B!", output);
    }
}
