using System;
using System.Reflection;
using System.Text;

namespace ConveyorSimulation.Core.Services;

public static class DiagnosticService
{
    public static string GetObjectState(object obj)
    {
        if (obj == null) return "Null";

        var sb = new StringBuilder();
        var type = obj.GetType();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        sb.AppendLine($"--- State of {type.Name} ---");
        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);
                sb.AppendLine($"{prop.Name}: {value}");
            }
            catch
            {
                sb.AppendLine($"{prop.Name}: <Access Denied>");
            }
        }
        sb.AppendLine("-------------------------");

        return sb.ToString();
    }
}