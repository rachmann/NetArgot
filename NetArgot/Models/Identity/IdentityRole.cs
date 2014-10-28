using Microsoft.AspNet.Identity;

namespace NetArgot.Models.Identity
{
    public class IdentityRole : IRole<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
