namespace P1Dash;

public class Util
{
    public static string FormatMetric(string name, string type, string value, string? help = null) =>
        $"# HELP {name} {help}\n# TYPE {name} {type}\n{name} {value}\n";
}
