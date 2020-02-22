using System.Threading.Tasks;
using AspNetCoreTodo.Models;

namespace AspNetCoreTodo.Interfaces
{
    public interface ITodoItemService
    {
        Task<TodoItem[]> GetIncompleteItemsAsync();
    }
}