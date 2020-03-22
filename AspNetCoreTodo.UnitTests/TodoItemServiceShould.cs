using System;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Interfaces;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AspNetCoreTodo.UnitTests
{
    public class TodoItemServiceShould : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ITodoItemService _service;
        private readonly ApplicationUser _fakeUser;

        public TodoItemServiceShould()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;
            _context = new ApplicationDbContext(options);
            _service = new TodoItemService(_context);
            _fakeUser = new ApplicationUser
            {
                Id = "fake-000",
                UserName = "fake@example.com",
            };
            _service.AddItemAsync(
                new TodoItem
                {
                    Title = "Testing?",
                    DueAt = DateTimeOffset.Now.AddDays(3)
                },
                _fakeUser
            );
        }

        public async void Dispose()
        {
            var todoItem = await _context.Items.FirstAsync();
            _context.Items.Remove(todoItem);
            _context.SaveChanges();
            _context.Dispose();
        }
        
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var itemsInDatabase = await _context.Items.CountAsync();
            Assert.Equal(1, itemsInDatabase);

            var item = await _context.Items.FirstAsync();
            Assert.Equal("Testing?", item.Title);
            Assert.False(item.IsDone);

            var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
            Assert.True(difference < TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task MarkItemAsComplete()
        {
            var todoItem = await _context.Items.FirstAsync();
            await _service.MarkDoneAsync(todoItem.Id, _fakeUser);

            var todoItemDone = await _context.Items.FirstAsync();
            Assert.True(todoItemDone.IsDone);
        }

        [Fact]
        public async Task MarkItemAsCompleteReturnsFalseIfBadId()
        {
            try
            {
                var response = await _service.MarkDoneAsync(new Guid("00000000-0000-0000-0000-000000000022"), _fakeUser);
                Assert.False(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Fact]
        public async Task GetIncompleteItemsAsyncDoesNotReturnForDifferentUser()
        {
            var newUser = new ApplicationUser
            {
                Id = "fake-001",
                UserName = "fake1@example.com"
            };
            var todoItems = await _service.GetIncompleteItemsAsync(newUser);
            if (todoItems != null) Assert.Empty(todoItems);
        }
    }
}