
namespace StateMigrationBackend.Models
{
    class SystemDetails
    {
        public int LogicalCPUCount { get; set; }
        public bool Is64Bit { get; set; }
        public long RAMSize { get; set; }
        public string MachineName { get; set; }
        public string OperatingSystem { get; set; }
    }
}
