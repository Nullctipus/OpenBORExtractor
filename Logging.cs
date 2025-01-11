public static class Logging
{
    private static void Logf(ConsoleColor color, string prepend, string format, params object[] param)
    {
        ConsoleColor last = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(prepend + String.Format(format, param));
        Console.ForegroundColor = last;
    }
    public static void Log(string format, params object[] param) =>
        Logf(ConsoleColor.White, "[MSG] ", format, param);

    public static void Warn(string format, params object[] param) =>
        Logf(ConsoleColor.Yellow, "[WARN] ", format, param);

    public static void Error(string format, params object[] param) =>
        Logf(ConsoleColor.Red, "[ERROR] ", format, param);

    public static void Error(Exception e) =>
        Error("{0}: {1}\n{2}", e.GetType().Name, e.Message, e.StackTrace ?? "No stacktrace available");
}
