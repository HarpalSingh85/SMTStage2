using System.Collections.Generic;


namespace StateMigrationBackend.Models
{
    class CBO
    {

        public Dictionary<string,string> GetLogTypes()
        {
            Dictionary<string, string> _logtypes = new Dictionary<string, string>();
            _logtypes.Add("Auto","Auto");
            _logtypes.Add("Append", "Append");
            _logtypes.Add("Overwrite", "Overwrite");

            return _logtypes;
        }

        public Dictionary<string, string> GetBRTypes()
        {
            Dictionary<string, string> _logtypes = new Dictionary<string, string>();
            _logtypes.Add("Auto", "Auto");
            _logtypes.Add("Full", "Full");
            _logtypes.Add("Difference", "Difference");

            return _logtypes;

        }
    }
}
     

