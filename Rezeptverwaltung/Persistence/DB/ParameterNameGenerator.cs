using System.Text;

namespace Persistence.DB;

public class ParameterNameGenerator
{
    private const string prefix = "$";
    private int index = 0;

    public ParameterNameGenerator() : base() { }

    public string GetNextParameterName()
    {
        var next = GenerateNextParameterName();
        return $"{prefix}{next}";
    }

    private string GenerateNextParameterName()
    {
        var stringBuilder = new StringBuilder();

        int tempIndex = index;
        do
        {
            int remainder = tempIndex % 26;
            stringBuilder.Insert(0, (char)('a' + remainder));
            tempIndex /= 26;
        }
        while (tempIndex > 0);

        index += 1;

        return stringBuilder.ToString();
    }
}
