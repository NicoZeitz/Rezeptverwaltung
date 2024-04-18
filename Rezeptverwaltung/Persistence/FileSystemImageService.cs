using Core.Interfaces;
using Core.Services;
using Core.ValueObjects;

namespace Persistence;

public class FileSystemImageService : ImageService
{
    private readonly FileSystem fileSystem;

    public FileSystemImageService(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public void SaveImage<T>(T entity, Image image) where T : UniqueIdentity
    {
        var fileName = GetImageFileNameFor(entity, image);
        fileSystem.Save(fileName, image.Data);
        DeleteOldImageFiles(
            GetAllPossibleFileNamesFor(entity),
            fileName
        );
    }

    public void DeleteImage<T>(T entity) where T : UniqueIdentity
    {
        GetAllPossibleFileNamesFor(entity)
            .ToList()
            .ForEach(fileSystem.Delete);
    }

    public Image GetImageFor<T>(T entity) where T : UniqueIdentity
    {
        var data = GetAllPossibleFileNamesFor(entity)
               .Select(fileSystem.Load)
               .First();

        return new Image(data, ImageType.WEBP); // TODO: Image type
    }

    private void DeleteOldImageFiles(IEnumerable<string> possibleOldFileNames, string newFileName)
    {
        foreach (var possibleOldFileName in possibleOldFileNames)
        {
            if (possibleOldFileName == newFileName)
                continue;

            fileSystem.Delete(possibleOldFileName);
        }
    }

    private string GetImageFileNameFor<T>(T entity, Image image) where T : UniqueIdentity
    {
        var extension = ImageTypeExtensions.GetFileExtension(image.imageType);
        var typeName = entity.GetType().Name.ToLower();
        var identity = entity.GetUniqueIdentity();
        return GetFileNameFrom(typeName, identity, extension);
    }

    private IEnumerable<string> GetAllPossibleFileNamesFor<T>(T entity) where T : UniqueIdentity
    {
        var typeName = entity.GetType().Name.ToLower();
        var identity = entity.GetUniqueIdentity();
        return ImageTypeExtensions
          .GetFileExtension(Enum.GetValues<ImageType>())
          .Select(extension => GetFileNameFrom(typeName, identity, extension));
    }

    private string GetFileNameFrom(string typeName, string identity, string extension) =>
        $"{typeName}-{identity}.{extension}";
}
