using Microsoft.EntityFrameworkCore;
namespace E_Ticaret.Areas.Admin.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
