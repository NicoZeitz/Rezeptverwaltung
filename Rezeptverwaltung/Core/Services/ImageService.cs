using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Services;

public interface ImageService
{
    void SaveImage<T>(T entity, Image image)
        where T : UniqueIdentity;

    void DeleteImage<T>(T entity)
        where T : UniqueIdentity;

    Image? GetImageFor<T>(T entity)
        where T : UniqueIdentity;
}
