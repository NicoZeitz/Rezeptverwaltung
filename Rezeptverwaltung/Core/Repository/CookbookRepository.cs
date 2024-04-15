using Core.Entities;
using Core.ValueObjects;

namespace Core.Repository;

public interface CookbookRepository
{
    void add(Cookbook cookbook);

    void remove(Cookbook cookbook);

    Cookbook? findByIdentifier(Identifier identifier);

    IEnumerable<Cookbook> findForChef(Chef chef);
}
