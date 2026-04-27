using System.Text;

namespace BearerAuthDemo;

public class GroupReport
{
    internal static string Generate(Group[] groups)
    {
        var output = new StringBuilder();

        output.AppendLine("## GROUP SUMMARY");

        output.AppendLine($"Your organization contains {groups.Length} groups.");

        foreach (var group in groups)
        {
            output.AppendLine("  - " + group.GroupName);
        }

        return output.ToString();
    }
}