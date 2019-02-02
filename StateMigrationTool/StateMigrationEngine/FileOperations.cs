using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using StateMigrationBackend.Models;


namespace StateMigrationBackend.StateMigrationEngine
{
    public class FileOperations
    {      

        protected internal static void Write(string _path, OperationModel _values)
        {            
            Directory.CreateDirectory($@"{_path}");
            string _filepath = $@"{_path}\backupdevices.json";
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            
            using (StreamWriter sw = new StreamWriter(_filepath))          
            {
                JsonWriter writer = new JsonTextWriter(sw);
                serializer.Serialize(writer, _values);
                File.SetAttributes(_filepath, FileAttributes.Hidden);               
            }
            
                    
        }

        protected internal static OperationModel Read(string _path)
        {
            OperationModel model = new OperationModel();
            string _filepath = $@"{_path}\backupdevices.json";
            if(File.Exists(_filepath))
            {
                string _values = File.ReadAllText(_filepath);
                model = JsonConvert.DeserializeObject<OperationModel>(_values);
            }

            return model;
        }
    }
}
