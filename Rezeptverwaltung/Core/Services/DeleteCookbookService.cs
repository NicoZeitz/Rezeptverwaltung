using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class DeleteCookbookService
{
    private readonly CookbookRepository cookbookRepository;

    public DeleteCookbookService(CookbookRepository cookbookRepository)
        : base()
    {
        this.cookbookRepository = cookbookRepository;
    }

    public bool DeleteCookbook(Identifier id, Chef chef)
    {
        var cookbook = cookbookRepository.FindByIdentifier(id);
        if (cookbook is null)
        {
            return false;
        }

        if (cookbook.Creator != chef.Username)
        {
            return false;
        }

        cookbookRepository.Remove(cookbook);
        return true;
    }
}