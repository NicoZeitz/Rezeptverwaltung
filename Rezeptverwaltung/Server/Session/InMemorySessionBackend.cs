using Core.Interfaces;
using Core.ValueObjects;
using System.Runtime.Caching;

namespace Server.Session;

public class InMemorySessionBackend<T> : SessionBackend<T> where T : class
{
    private readonly MemoryCache sessions = new MemoryCache("sessions");
    private readonly DateTimeProvider dateTimeProvider;

    public InMemorySessionBackend(DateTimeProvider dateTimeProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
    }

    public Identifier CreateSession(T userObject, Duration? expiresAfter = null)
    {
        var sessionId = Identifier.NewId();

        if (expiresAfter is null)
        {
            sessions.Set(
                sessionId.ToString(),
                userObject,
                new CacheItemPolicy() { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration }
            );
        }
        else
        {
            sessions.Set(
                sessionId.ToString(),
                userObject,
                dateTimeProvider.OffsetNow.Add(expiresAfter.Value.TimeSpan)
            );
        }

        return sessionId;
    }

    public T? GetBySessionId(Identifier sessionId)
    {
        return sessions.Get(sessionId.ToString()) as T;
    }

    public void RemoveSession(Identifier sessionId)
    {
        sessions.Remove(sessionId.ToString());
    }
}
