using Core.ValueObjects;

namespace Server.Session;


public class InMemorySessionService<T> : ISessionService<T> where T : class
{
    private readonly IDictionary<Identifier, SessionItem<T>> sessions = new Dictionary<Identifier, SessionItem<T>>();

    public Identifier CreateSession(T userObject, Duration? expiresAfter = null)
    {
        sessions.Where(session => session.Value.Value == userObject)
                .ToList()
                .ForEach(session => sessions.Remove(session.Key));

        var sessionId = Identifier.NewId();
        sessions.Add(sessionId, new SessionItem<T>(userObject, expiresAfter));
        return sessionId;
    }

    public T? GetBySessionId(Identifier sessionId)
    {
        if (!sessions.ContainsKey(sessionId))
        {
            return null;
        }

        var session = sessions[sessionId];

        if (session.ExpiresAfter.HasValue && session.Created + session.ExpiresAfter.Value.TimeSpan < DateTimeOffset.Now)
        {
            sessions.Remove(sessionId);
            return null;
        }

        return session.Value;
    }

    public void RemoveSession(Identifier sessionId)
    {
        sessions.Remove(sessionId);
    }

    private record struct SessionItem<TValue>(TValue Value, Duration? ExpiresAfter = null)
    {
        public DateTimeOffset Created { get; } = DateTimeOffset.Now;
    }
}
