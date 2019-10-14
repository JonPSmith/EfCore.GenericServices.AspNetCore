using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExampleDatabase;

[assembly: InternalsVisibleTo("Test")]

namespace CommonWebParts
{
    public static class DatabaseStartupHelpers
    {

        public static void SeedDatabase(this ExampleDbContext context)
        {
            var todos = new List<TodoItem>
            {
                new TodoItem("Create ASP.NET Core API project", 1),
                new TodoItem("Create simple EF Core database", 1),
                new TodoItem("Add EfCore.GenericServices to web app", 1),
                new TodoItem("Create a example WebAPI controller", 3),
                new TodoItem("Write unit tests", 2),
                new TodoItem("Add Swagger for manual testing", 2)
            };
            context.AddRange(todos);
            var todoHybrids = new List<TodoItemHybrid>
            {
                new TodoItemHybrid("Create ASP.NET Core API project", 1),
                new TodoItemHybrid("Create simple EF Core database", 1),
                new TodoItemHybrid("Add EfCore.GenericServices to web app", 1),
                new TodoItemHybrid("Create a example WebAPI controller", 3),
                new TodoItemHybrid("Write unit tests", 2),
                new TodoItemHybrid("Add Swagger for manual testing", 2)
            };
            context.AddRange(todoHybrids);
            context.SaveChanges();
        }
    }
}