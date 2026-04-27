using System;
using System.Linq;
using System.Text;

namespace BearerAuthDemo;

public class UserReport
{
    internal static string Generate(User[] users)
    {
        var output = new StringBuilder();

        var total = users.Count();

        output.AppendLine("## USER SUMMARY");

        output.AppendLine($"Your organization contains {total:n0} user accounts.");

        foreach (var user in users)
        {
            output.AppendLine($"  - {user.FullName}");
        }

        return output.ToString();
    }
}
