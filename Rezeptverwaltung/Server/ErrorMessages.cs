using Core.ValueObjects;

namespace Server;

public static class ErrorMessages
{
    public static readonly ErrorMessage GENERIC_ERROR_MESSAGE = new("Es ist ein Fehler aufgetreten.");
    public static readonly ErrorMessage INVALID_CREDENTIALS_ERROR_MESSAGE = new("Benutzername und/oder Passwort falsch!");
    public static readonly ErrorMessage INVALID_IMAGE_MIME_TYPE = new("Bilddatei nicht erlaubt! Bitte lade ein anderes Bild hoch.");
    public static readonly ErrorMessage PASSWORDS_DO_NOT_MATCH = new("Passwörter stimmen nicht überein!");
}