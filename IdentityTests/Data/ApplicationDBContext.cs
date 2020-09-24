using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityTests.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext() { }
        public ApplicationDBContext(DbContextOptions options) : base(options) { }
    }
}
