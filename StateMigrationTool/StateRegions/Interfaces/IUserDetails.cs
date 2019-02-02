using System.Threading.Tasks;
using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateRegions
{
    public interface IUserDetails
    {
        Task<Person> GetUserDetails(string _userID);
    }
}