using System;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AspNetCoreTodo.UnitTests
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;
            
            await CreateItem(options);

            await using (var context = new ApplicationDbContext(options))
            {
                var itemsInDatabase = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);

                var item = await context.Items.FirstAsync();
                Assert.Equal("Testing?", item.Title);
                Assert.False(item.IsDone);

                var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
                Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task MarkItemAsComplete()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_AddNewItem")
                .Options;
            
            await using var context = new ApplicationDbContext(options);
            
            var service = new TodoItemService(context);

            await CreateItem(options);
            
            var fakeUser = new ApplicationUser
            {
                Id = "fake-000",
                UserName = "fake@example.com",
            };

            var todoItem = await context.Items.FirstAsync();

            await service.MarkDoneAsync(todoItem.Id, fakeUser);
            
            var todoItemDone = await context.Items.FirstAsync();
            
            Assert.True(todoItemDone.IsDone);
        }

        private async Task CreateItem(DbContextOptions<ApplicationDbContext> options)
        {
            await using var context = new ApplicationDbContext(options);
            
            var service = new TodoItemService(context);
                        
            var fakeUser = new ApplicationUser
            {
                Id = "fake-000",
                UserName = "fake@example.com",
            };
                
            await service.AddItemAsync(
                new TodoItem
                {
                    Title = "Testing?",
                    DueAt = DateTimeOffset.Now.AddDays(3)
                },
                fakeUser
            );
        }
    }
}