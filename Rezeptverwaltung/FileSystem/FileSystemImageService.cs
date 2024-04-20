using Core.Interfaces;
using Core.Services;
using Core.ValueObjects;

namespace FileSystem;

public class FileSystemImageService : ImageService
{
    private readonly FileSystem fileSystem;

    public FileSystemImageService(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public void SaveImage<T>(T entity, Image image) where T : UniqueIdentity
    {
        var fileName = GetImageFileFor(entity, image);
        fileSystem.Save(fileName, image.Data);
        DeleteOldImageFiles(
            GetAllPossibleFileNamesFor(entity),
            fileName
        );
    }

    public void DeleteImage<T>(T entity) where T : UniqueIdentity
    {
        GetAllPossibleFileNamesFor(entity)
            .Select(tuple => tuple.FileName)
            .ToList()
            .ForEach(fileSystem.Delete);
    }

    public Image? GetImageFor<T>(T entity) where T : UniqueIdentity
    {
        var possibleImage = GetAllPossibleFileNamesFor(entity)
                .Where(tuple => fileSystem.Exists(tuple.FileName))
                .FirstOrDefault();

        if (possibleImage is null)
            return null;

        var imageType = possibleImage.ImageType;
        var fileName = possibleImage.FileName;
        var data = fileSystem.Load(fileName);
        if (data is null)
            return null;

        return new Image(data, imageType);
    }

    private void DeleteOldImageFiles(IEnumerable<ImageTypeFileNameTuple> possibleOldFileNames, FileName newFileName)
    {
        foreach (var possibleOldFileName in possibleOldFileNames)
        {
            if (possibleOldFileName.FileName == newFileName)
                continue;

            fileSystem.Delete(possibleOldFileName.FileName);
        }
    }

    private FileName GetImageFileFor<T>(T entity, Image image) where T : UniqueIdentity
    {
        var extension = image.ImageType.FileExtension;
        var typeName = entity.GetType().Name.ToLower();
        var identity = entity.GetUniqueIdentity();
        return GetFileNameFrom(typeName, identity, extension);
    }

    private IEnumerable<ImageTypeFileNameTuple> GetAllPossibleFileNamesFor<T>(T entity) where T : UniqueIdentity
    {
        var typeName = entity.GetType().Name.ToLower();
        var identity = entity.GetUniqueIdentity();
        return ImageType.ALL_IMAGE_TYPES
          .Select(type => new ImageTypeFileNameTuple(type, GetFileNameFrom(typeName, identity, type.FileExtension)));
    }

    private FileName GetFileNameFrom(string typeName, string identity, string extension) =>
        new FileName($"{typeName}-{identity}", new FileExtension(extension));

    private record class ImageTypeFileNameTuple(ImageType ImageType, FileName FileName);
}
