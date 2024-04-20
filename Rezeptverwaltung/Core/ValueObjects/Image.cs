namespace Core.ValueObjects;

public record struct Image(Stream Data, ImageType ImageType);
