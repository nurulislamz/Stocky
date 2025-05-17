using System.ComponentModel.DataAnnotations;

namespace stockymodels.Models;

public abstract class BaseModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
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