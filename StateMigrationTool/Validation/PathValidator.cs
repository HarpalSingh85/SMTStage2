using System.IO;

namespace StateMigrationBackend.Validation
{
    class PathValidator
    {
       static bool _result = false;

        protected internal bool Validate(string _path)
        {
            if (Directory.Exists(_path))
                _result = true;

            return _result;
        }

    }
}
