using Core.Entities;
using Core.Repository;
using Core.ValueObjects;

namespace Core.Services;

public class ChangeChefDetailsService
{
    private readonly ChefRepository chefRepository;
    private readonly ImageService imageService;

    public ChangeChefDetailsService(
        ChefRepository chefRepository,
        ImageService imageService)
        : base()
    {
        this.chefRepository = chefRepository;
        this.imageService = imageService;
    }

    public void ChangeChefNameAndImage(Chef chef, string? firstName, string? lastName, Image? image)
    {
        ChangeChefName(chef, firstName, lastName);
        if (image is not null)
        {
            ChangeChefImage(chef, image.Value);
        }
    }

    public void ChangeChefName(Chef chef, string? firstName, string? lastName)
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
    }

    public void ChangeChefImage(Chef chef, Image image)
    {
        imageService.SaveImage(chef, image);
    }
}