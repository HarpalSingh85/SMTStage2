using System.Threading.Tasks;
using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateRegions
{
    interface IAutoDetection
    {
        Task<SystemDetails> DetectAsync();
        Task<int> GetOptimizedValue();
    }
}