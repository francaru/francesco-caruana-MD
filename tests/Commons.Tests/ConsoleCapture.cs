namespace Commons.Tests;

public static class ConsoleCapture
{
    public static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            action();

            return stringWriter.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
