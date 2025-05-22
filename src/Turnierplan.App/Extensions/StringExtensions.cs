using System.Text;

namespace Turnierplan.App.Extensions;

internal static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        var result = new StringBuilder();

        for (var index = 0; index < input.ToCharArray().Length; index++)
        {
            var c = input.ToCharArray()[index];

            if (char.IsLower(c))
            {
                result.Append(c);
            }
            else
            {
                if (index > 0 && char.IsLower(input[index - 1]))
                {
                    result.Append('-');
                }

                result.Append(char.ToLower(c));
            }
        }

        return result.ToString();
    }
}
