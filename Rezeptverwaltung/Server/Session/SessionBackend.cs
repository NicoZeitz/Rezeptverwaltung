using Core.ValueObjects;

namespace Server.Session;

public interface SessionBackend<T> where T : class
{
    Identifier CreateSession(T chef, Duration? expiresAfter = null);
    
    T? GetBySessionId(Identifier sessionId);

    void RemoveSession(Identifier sessionId);
}