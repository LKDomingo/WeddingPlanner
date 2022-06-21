#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
// Needed for the [NotMapped] functionality
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class LogUser
{
    [EmailAddress]
    [Required]
    public string LogEmail {get;set;}

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string LogPassword {get;set;}
}