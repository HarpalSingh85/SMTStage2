
namespace StateMigrationBackend.Models
{
   public class Person
    {
        public string Name { get; set; } = null;
        public string Company { get; set; } = null;
        public string HomeDirectory { get; set; } = null;
        public string ProfilePathDirectory { get; set; } = null;
        public string CustomHomeDirectory { get; set; }
        public string CustomProfilePathDirectory { get; set; }
                
    }
}
