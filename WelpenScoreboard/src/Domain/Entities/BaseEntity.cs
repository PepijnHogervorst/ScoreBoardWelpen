using System.ComponentModel.DataAnnotations;

namespace WelpenScoreboard.Domain.Entities;
/// <summary>
/// The base class for all database entities
/// </summary>
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
