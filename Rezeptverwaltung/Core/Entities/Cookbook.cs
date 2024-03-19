using Core.ValueObjects;

namespace Core.Entities;

public class Cookbook
{
    public Identifier Identifier { get; }
    public Text Title { get; }
    public Identifier Creator { get; }
    public List<Identifier> Recipes { get; }
}
