using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateRegions
{
    class Destination : IDestination
    {
        public string Resolve(Person _person)
        {
            string _userprofilepath = string.Empty;

            if(!string.IsNullOrWhiteSpace(_person.ProfilePathDirectory))
            {
                _userprofilepath = _person.ProfilePathDirectory;
            }
            if(!string.IsNullOrWhiteSpace(_person.CustomProfilePathDirectory))
            {
                _userprofilepath = _person.CustomProfilePathDirectory;
            }
            return _userprofilepath;

        }

    }
}
