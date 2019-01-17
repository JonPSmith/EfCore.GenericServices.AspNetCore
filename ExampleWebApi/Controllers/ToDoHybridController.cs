using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Dtos;
using GenericBizRunner;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExampleWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoHybridController : ControllerBase
    {
        /// <summary>
        /// Gets all the TodoItemHybrid items
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<WebApiMessageAndResult<List<TodoItemHybrid>>>> GetManyAsync([FromServices]ICrudServices service)
        {
            return service.Response(await service.ReadManyNoTracked<TodoItemHybrid>().ToListAsync());
        }

        /// <summary>
        /// Gets the TodoItemHybrid with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetSingleHybridTodo")]
        public async Task<ActionResult<WebApiMessageAndResult<TodoItemHybrid>>> GetSingleAsync(int id, [FromServices]ICrudServicesAsync service)
        {
            return service.Response(await service.ReadSingleAsync<TodoItemHybrid>(id));
        }

        /// <summary>
        /// Creates a new item and returns the created entity, with the Id value provided by the database
        /// NOTE: to show how business logic might work I added extra validation (name can't end with !) in the business logic
        /// </summary>
        /// <param name="item"></param>
        /// <param name="service"></param>
        /// <returns>If successful it returns a CreatedAtRoute response - see
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.1#implement-the-other-crud-operations
        /// </returns>
        [ProducesResponseType(typeof (CreateTodoHybridDto), 201)] //You need this, otherwise Swagger says the success status is 200, not 201
        [HttpPost]
        public async Task<ActionResult<CreateTodoHybridDto>> PostAsync(CreateTodoHybridDto item, [FromServices]ICrudServicesAsync service)
        {
            var result = await service.CreateAndSaveAsync(item);
            //NOTE: to get this to work you MUST set the name of the HttpGet, e.g. [HttpGet("{id}", Name= "GetSingleTodo")],
            //on the Get you want to call, then then use the Name value in the Response.
            //Otherwise you get a "No route matches the supplied values" error.
            //see https://stackoverflow.com/questions/36560239/asp-net-core-createdatroute-failure for more on this
            return service.Response(this, "GetSingleHybridTodo", new { id = result.Id },  item);
        }

        /// <summary>
        /// Updates the Name. It does this via a DDD-styles entity access method.
        /// NOTE: There is extra validation (name can't end with !) in the DDD access method
        /// </summary>
        /// <param name="dto">dto containing Id and Name</param>
        /// <param name="service"></param>
        [Route("name")]
        [HttpPatch()]
        public ActionResult<WebApiMessageOnly> Name(ChangeNameHybridDto dto, [FromServices]ICrudServices service)
        {
            service.UpdateAndSave(dto);
            return service.Response();
        }

        /// <summary>
        /// Updates the Difficulty. It does this using JSON Patch
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch">contains the patch information</param>
        /// <param name="service"></param>
        /// <returns></returns>
        [Route("difficulty")]
        [HttpPatch()]
        public ActionResult<WebApiMessageOnly> Update(int id, JsonPatchDocument<TodoItemHybrid> patch, [FromServices]ICrudServices service)
        {
            service.UpdateAndSave(patch, id);
            return service.Response();
        }

        /// <summary>
        /// Updates the Difficulty. It does this using JSON Patch
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch">contains the patch information</param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Route("handcoded")]
        [HttpPatch()]
        public IActionResult HandCodedUpdate(int id, JsonPatchDocument<TodoItemHybrid> patch, [FromServices]ExampleDbContext context)
        {
            var entity = context.Find<TodoItemHybrid>(id);
            if (entity == null)
            {
                return NoContent();
            }
            patch.ApplyTo(entity);
            context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Deletes the TodoItemHybrid with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        // DELETE api/todo/5
        [HttpDelete("{id}")]
        public ActionResult<WebApiMessageOnly> Delete(int id, [FromServices]ICrudServices service)
        {
            service.DeleteAndSave<TodoItemHybrid>(id);
            return service.Response();
        }
    }
}
