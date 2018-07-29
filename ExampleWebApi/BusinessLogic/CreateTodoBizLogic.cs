// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using ExampleDatabase;
using ExampleWebApi.Dtos;
using GenericBizRunner;

namespace ExampleWebApi.BusinessLogic
{
    public interface ICreateTodoBizLogic : IGenericActionWriteDb<CreateTodoDto, TodoItem> { }

    public class CreateTodoBizLogic : BizActionStatus, ICreateTodoBizLogic
    {
        private readonly ExampleDbContext _context;

        public CreateTodoBizLogic(ExampleDbContext context)
        {
            _context = context;
        }

        public TodoItem BizAction(CreateTodoDto inputData)
        {
            if (inputData.Name.EndsWith('!'))
                AddError("Business logic says the name canot end with !", nameof(inputData.Name));

            var item = new TodoItem(inputData.Name, inputData.Difficulty);
            _context.Add(item);
            
            Message = $"Successfully saved the todo item '{inputData.Name}'.";
            return item;
        }
    }
}