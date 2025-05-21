using System.ComponentModel.DataAnnotations;

namespace stockymodels.models;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime? UpdatedAt { get; set; }

    protected BaseModel()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}