namespace WelpenScoreboard.Domain.Entities;
public class Group : BaseEntity
{
    public int GroupNumber { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public List<Person> Persons { get; set; } = [];
}
