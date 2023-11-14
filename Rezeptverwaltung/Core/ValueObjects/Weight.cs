namespace Core.ValueObjects
{
    public enum WeightUnit
    {
        KILOGRAMM, GRAMM,
        GRAIN,
        POUND, OUNCE,
        TABLESPOON, TEASPOON,
        LITERE, DECILITRE, CENTILITERE, MILILITERE, 
        CUP,
        PINCH
    }

    public enum StateOfMatter
    {
        LIQUID,
        SOLID
    }

    // TODO: Umrechung der Einheiten (https://www.ordnungsliebe.net/amerikanische-masse-umrechnen/) / Benutzereinstellung (metrisch / imperial) 
    public record struct Weight(decimal Amount, WeightUnit Unit, StateOfMatter StateOfMatter): Amount
    {
        // equals-Methode muss überschrieben werden


    }
}
