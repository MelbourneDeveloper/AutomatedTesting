using System.Threading.Tasks;

namespace AutomatedTestingFramework.Model
{
    public interface IExecutable
    {
        Task<object> Execute();
    }
}
