using System.Runtime.CompilerServices;
using System.Text;

namespace Database;

[InterpolatedStringHandler]
internal class QueryInterpolatedStringHandler
{
    private readonly StringBuilder query;
    private readonly IDictionary<string, object?> parameters = new Dictionary<string, object?>();
    private readonly ParameterNameGenerator parameterNameGenerator = new ParameterNameGenerator();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Formatted Count is Needed for InterpolatedStringHandler")]
    public QueryInterpolatedStringHandler(int literalLength, int _formattedCount)
    {
        query = new StringBuilder(literalLength);
    }

    public void AppendLiteral(string literal)
    {
        query.Append(literal);
    }

    public void AppendFormatted<T>(T? parameter)
    {
        var parameterList = parameter as System.Collections.IEnumerable;
        if (parameter is not string && parameterList is not null)
        {
            var parameterNames = new List<string>();
            foreach (var parameterItem in parameterList)
            {
                var parameterName = parameterNameGenerator.GetNextParameterName();
                parameters.Add(parameterName, parameterItem);
                parameterNames.Add($"({parameterName})");
            }
            query.Append(string.Join(",", parameterNames));
        }
        else
        {
            var parameterName = parameterNameGenerator.GetNextParameterName();
            parameters.Add(parameterName, parameter);
            query.Append(parameterName);
        }
    }

    public string GetQuery() => query.ToString().Trim();

    public IDictionary<string, object?> GetParameters() => parameters;
}
