using System.Reflection;

namespace ServiceA.Tests;

[TestClass]
public sealed class ServiceATests
{
    [TestMethod]
    public void Main_WhenCalled_PrintsGreeting()
    {
        var output = ConsoleCapture.CaptureConsoleOutput(() =>
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            typeof(ServiceA).GetMethod(
                name: "Main", 
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Static
            ).Invoke(
                obj: null, 
                parameters: [Array.Empty<string>()]
            );
        });

        StringAssert.Contains("Hello, World! I am Service A!", output);
    }
}
