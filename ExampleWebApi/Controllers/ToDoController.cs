using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Dtos;
using GenericBizRunner;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExampleWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        /// <summary>
        /// Gets all the TodoItem items
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<WebApiMessageAndResult<List<TodoItem>>>> GetManyAsync([FromServices]ICrudServices service)
        {
            return service.Response(await service.ReadManyNoTracked<TodoItem>().ToListAsync());
        }

        /// <summary>
        /// Gets the TodoItem with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        // GET api/todo/5
        public async Task<ActionResult<WebApiMessageAndResult<TodoItem>>> GetSingleAsync(int id, [FromServices]ICrudServicesAsync service)
        {
            return service.Response(await service.ReadSingleAsync<TodoItem>(id));
        }

        // POST api/todo {name='name', difficulty=1}
        /// <summary>
        /// Creates a new item and returns the created entity, with the Id value provided by the database
        /// NOTE: to show how business logic might work I added extra validation (name can't end with !) in the business logic
        /// </summary>
        /// <param name="item"></param>
        /// <param name="service"></param>
        /// <returns>If successful it returns a CreatedAtRoute response - see
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.1#implement-the-other-crud-operations
        /// </returns>
        [ProducesResponseType(typeof (CreateTodoDto), 201)] //You need this, otherwise Swagger says the success status is 200, not 201
        [HttpPost]
        public ActionResult<CreateTodoDto> Post(CreateTodoDto item, [FromServices]IActionService<ICreateTodoBizLogic> service)
        {
            var result = service.RunBizAction<TodoItem>(item);
            //NOTE: to get this to work you MUST set the name of the HttpGet, e.g. [HttpGet("{id}", Name= "GetSingleTodo")],
            //on the Get you want to call, then then use the Name value in the Response.
            //Otherwise you get a "No route matches the supplied values" error.
            //see https://stackoverflow.com/questions/36560239/asp-net-core-createdatroute-failure for more on this
            return service.Status.Response(this, "GetSingleTodo", new { id = result?.Id },  item);
        }

        /// <summary>
        /// Updates the Name. It does this via a DDD-styles entity access method.
        /// NOTE: There is extra validation (name can't end with !) in the DDD access method
        /// </summary>
        /// <param name="dto">dto containing Id and Name</param>
        /// <param name="service"></param>
        [Route("name")]
        [HttpPatch()]
        public ActionResult<WebApiMessageOnly> Name(ChangeNameDto dto, [FromServices]ICrudServices service)
        {
            service.UpdateAndSave(dto);
            return service.Response();
        }

        /// <summary>
        /// Updates the Difficulty. It does this via a DDD-styles entity access method.
        /// NOTE: this access method doesn't return a status, i.e. there is no extra validation in the access method
        /// but if the new difficultly value is outside 1 to 5 the database validation would return an error
        /// </summary>
        /// <param name="dto">dto containing Id and Difficulty number</param>
        /// <param name="service"></param>
        /// <returns></returns>
        [Route("difficulty")]
        [HttpPatch]
        public ActionResult<WebApiMessageOnly> Difficulty(ChangeDifficultyDto dto, [FromServices]ICrudServices service)
        {
            service.UpdateAndSave(dto);
            return service.Response();
        }

        /// <summary>
        /// Deletes the TodoItem with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        // DELETE api/todo/5
        [HttpDelete("{id}")]
        public ActionResult<WebApiMessageOnly> Delete(int id, [FromServices]ICrudServices service)
        {
            service.DeleteAndSave<TodoItem>(id);
            return service.Response();
        }
    }
}
