using System.Threading.Tasks;

namespace AutomatedTesting.Model
{
    public interface IExecutable
    {
        Task<object> Execute();
    }
}
