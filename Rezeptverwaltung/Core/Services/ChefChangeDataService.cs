using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class ChefChangeDataService
{
    private readonly ChefRepository chefRepository;
    private readonly ImageService imageService;

    public ChefChangeDataService(
        ChefRepository chefRepository,
        ImageService imageService)
        : base()
    {
        this.chefRepository = chefRepository;
        this.imageService = imageService;
    }

    public void ChangeData(Chef chef, string? firstName, string? lastName, Image? image)
    {
        if (firstName is not null)
        {
            chef.Name = new Name(firstName, chef.Name.LastName);
        }

        if (lastName is not null)
        {
            chef.Name = new Name(chef.Name.FirstName, lastName);
        }

        if (firstName is not null || lastName is not null)
        {
            chefRepository.Update(chef);
        }

        if (image.HasValue)
        {
            imageService.SaveImage(chef, image.Value);
        }
    }
}