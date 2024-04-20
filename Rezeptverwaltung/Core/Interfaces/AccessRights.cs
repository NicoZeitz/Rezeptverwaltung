using Core.Entities;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface AccessRights
{
    Visibility Visibility { get; }

    bool IsVisibleTo(Chef viewer);
}