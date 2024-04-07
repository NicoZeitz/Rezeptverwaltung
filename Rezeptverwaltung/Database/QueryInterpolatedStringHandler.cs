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
        if(parameter is IEnumerable<string> parameterList)
        {
            var parameterListNames = string.Join(",", parameterList.Select(parameter =>
            {
                var parameterName = parameterNameGenerator.GetNextParameterName();
                parameters.Add(parameterName, parameter);
                return parameterName;
            }));

            query.Append(parameterListNames);            
        }
        else
        {
            var parameterName = parameterNameGenerator.GetNextParameterName();
            parameters.Add(parameterName, parameter);
            query.Append(parameterName);
        }

    }

    public string GetQuery() => query.ToString();

    public IDictionary<string, object?> GetParameters() => parameters;
}
