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
        public async Task<ActionResult<WebApiMessageAndResult<List<TodoItem>>>> GetAsync([FromServices]ICrudServices service)
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
        [HttpGet("{id}", Name= "Get")]
        public async Task<ActionResult<WebApiMessageAndResult<TodoItem>>> GetAsync(int id, [FromServices]ICrudServicesAsync service)
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
        [ProducesResponseType(typeof (CreateTodoDto), 201)]
        [HttpPost]
        public ActionResult<CreateTodoDto> Post(CreateTodoDto item, [FromServices]IActionService<ICreateTodoBizLogic> service)
        {
            var result = service.RunBizAction<TodoItem>(item);
            //NOTE: to get this to work you must:
            //Set the name of the HttpGet, e.g. [HttpGet("{id}", Name= "Get")], and then use the Name value in the CreatedAtRoute
            //see https://stackoverflow.com/questions/36560239/asp-net-core-createdatroute-failure for more on this
            return service.Status.Response(this, "Get", new { id = result.Id },  item);
        }

        // PUT api/todo {id=1, name='NewName'}
        /// <summary>
        /// Updates the Name. It does this via a DDD-styles entity access method.
        /// NOTE: There is extra validation (name can't end with !) in the DDD access method
        /// </summary>
        /// <param name="dto">dto containing Id and Name</param>
        /// <param name="service"></param>
        [Route("putname")]
        [HttpPut()]
        public ActionResult<WebApiMessageOnly> PutName(ChangeNameDto dto, [FromServices]ICrudServices service)
        {
            service.UpdateAndSave(dto);
            return service.Response();
        }

        /// <summary>
        /// Updates the Difficulty. It does this via a DDD-styles entity access method.
        /// NOTE: this access method doesn't return a status, i.e. there is no extra validation in the access method
        /// </summary>
        /// <param name="dto">dto containing Id and Difficulty number</param>
        /// <param name="service"></param>
        /// <returns></returns>
        // PUT api/todo {id=1, difficulty=3}
        [Route("putdifficulty")]
        [HttpPut]
        public ActionResult<WebApiMessageOnly> PutDifficuty(ChangeDifficultyDto dto, [FromServices]ICrudServices service)
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
