using System.Runtime.CompilerServices;
using System.Text;

namespace Database;

[InterpolatedStringHandler]
internal class QueryInterpolatedStringHandler
{ 
    private readonly StringBuilder query;
    private readonly IDictionary<string, object?> parameters = new Dictionary<string, object?>();
    private readonly ParameterNameGenerator parameterNameGenerator = new ParameterNameGenerator();

    public QueryInterpolatedStringHandler(int literalLength)
    {
        query = new StringBuilder(literalLength);
    }

    public void AppendLiteral(string s)
    {
        query.Append(s);
    }

    public void AppendFormatted<T>(T? t)
    {
        var parameterName = parameterNameGenerator.GetNextParameterName();
        parameters.Add(parameterName, t);
        query.Append(parameterName);
    }

    public string GetQuery() => query.ToString();

    public IDictionary<string, object?> GetParameters() => parameters;
}
