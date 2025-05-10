using Microsoft.EntityFrameworkCore;
using stockyapi.Models;

namespace stockyapi.Data;

public class ApplicationDbContext : DbContext
{
  public DbSet<UserModel> Users { get; set; }

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }
}