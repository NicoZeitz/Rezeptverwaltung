namespace Database;

internal class ParameterNameGenerator
{
    private const string prefix = "$";
    private string? lastParameterName = null;

    public string GetNextParameterName()
    {
        var next = GenerateNextParameterName();
        lastParameterName = next;

        return $"{prefix}{next}";
    }

    private string GenerateNextParameterName()
    {
        if (lastParameterName == null)
        {
            return "a";
        }
        

        return "TODO:";
    }
}
