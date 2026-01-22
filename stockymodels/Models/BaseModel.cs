using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public abstract class BaseModel
{
    [Key]
    public virtual Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    protected BaseModel() { }
}
