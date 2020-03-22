using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Interfaces;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTodo.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext _context;

        public TodoItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TodoItem[]> GetIncompleteItemsAsync(ApplicationUser user)
        {
            return await _context.Items
                .Where(x => x.IsDone == false && x.UserId == user.Id)
                .ToArrayAsync();
        }

        public async Task<bool> AddItemAsync(TodoItem newItem, ApplicationUser currentUser)
        {
            newItem.Id = Guid.NewGuid();
            newItem.IsDone = false;
            newItem.UserId = currentUser.Id;

            _context.Items.Add(newItem);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> MarkDoneAsync(Guid id, ApplicationUser currentUser)
        {
            try
            {
                var item = await _context.Items.SingleAsync(x => x.Id == id && x.UserId == currentUser.Id);

                if (item == null) return false;
                
                item.IsDone = true;

                var saveResult = await _context.SaveChangesAsync();
                return saveResult == 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}