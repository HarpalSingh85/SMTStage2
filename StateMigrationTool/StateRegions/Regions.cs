
using System.Collections.Generic;

namespace StateMigrationBackend.StateRegions
{
    class Regions
    {
        public List<string> Paths { get; private set; } = new List<string>();

        internal List<string> GetPaths(string _usrname)
        {
            List<string> _paths = new List<string>();            

            if (!string.IsNullOrEmpty(_usrname))
            {
                //_paths.Add($"C:\\Users\\{_usrname}\\AppData\\Local\\Temp");                
                //_paths.Add($@"C:\Temp\Temp");
                _paths.Add($@"C:\Users\{_usrname}");
                //_paths.Add($@"C:\Users\{_usrname}\AppData\Local\Temp");              
                //_paths.Add($@"C:\Users\{_usrname}\AppData\Local\Google");

                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\AppData"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Desktop"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Downloads"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Favorites"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Documents"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Pictures"));
                //_paths.Add(Path.Combine($"C:\\Users\\{_usrname}\\Videos"));
            }
            else
                _paths = null;            
                        

            if(Paths.Count > 0)
            {
                _paths.AddRange(Paths);
            }

            return _paths;
        }
                

        internal void SetPath(string _path)
        {
            List<string> _p = new List<string>();
            _p.Add(_path);
            Paths.AddRange(_p);       

        }

        internal void SetPaths(List<string> _paths)
        {
            List<string> _p = new List<string>();
            _p.AddRange(_paths);
            Paths.AddRange(_p);

        }        

    }

}

