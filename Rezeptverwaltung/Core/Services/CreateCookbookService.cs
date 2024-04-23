using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class CreateCookbookService
{
    private readonly CookbookRepository cookbookRepository;

    public CreateCookbookService(CookbookRepository cookbookRepository)
        : base()
    {
        this.cookbookRepository = cookbookRepository;
    }

    public Cookbook CreateCookbook(
        Text title,
        Text description,
        Chef chef,
        Visibility visibility,
        IEnumerable<Identifier> recipes)
    {
        var cookbook = new Cookbook(
            Identifier.NewId(),
            title,
            description,
            chef.Username,
            visibility,
            recipes
        );

        cookbookRepository.Add(cookbook);
        return cookbook;
    }
}