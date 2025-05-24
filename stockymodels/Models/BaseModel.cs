using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

[Index(nameof(Id), IsUnique = true)]
public abstract class BaseModel
{
    [Key]
    public virtual Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    protected BaseModel()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}