#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId {get;set;}

    [Required]
    public string Wedder1 {get;set;}
    
    [Required]
    public string Wedder2 {get;set;}

    [Required]
    public DateTime Date {get;set;}

    [Required]
    public string Address {get;set;}

    [Required]
    public int CreatorId {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public List<Association> Guests {get;set;} = new List<Association>();
}