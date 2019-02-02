using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateRegions
{
    interface IDestination
    {
        string Resolve(Person _person);
    }
}