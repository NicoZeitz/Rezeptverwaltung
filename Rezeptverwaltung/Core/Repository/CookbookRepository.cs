using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface CookbookRepository
{
    void Add(Cookbook cookbook);

    void Remove(Cookbook cookbook);

    Cookbook? FindByIdentifier(Identifier identifier);

    IEnumerable<Cookbook> FindForChef(Chef chef);
}
