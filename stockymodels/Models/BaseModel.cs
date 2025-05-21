using System.ComponentModel.DataAnnotations;

namespace stockymodels.models;

public abstract class BaseModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime? UpdatedAt { get; set; }

    protected BaseModel()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}