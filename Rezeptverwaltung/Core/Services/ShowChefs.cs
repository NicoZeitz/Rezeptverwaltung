using Core.Entities;
using Core.Repository;
using Core.Services.Retrieval;
using Core.ValueObjects;

namespace Core.Services;

public class ShowChefs
{
    private readonly ChefRepository chefRepository;

    public ShowChefs(ChefRepository chefRepository) : base()
    {
        this.chefRepository = chefRepository;
    }

    public Chef? ShowSingleChef(Username username)
    {
        var retrievalGraph = new OptionalSingleItemListRetrieval<Chef>(
            chefRepository.FindByUsername(username)
        );
        return retrievalGraph
            .Retrieve()
            .FirstOrDefault();
    }
}