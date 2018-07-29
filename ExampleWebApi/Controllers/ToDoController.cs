using System.Collections.Generic;
using System.Linq;
using ExampleDatabase;
using ExampleWebApi.BusinessLogic;
using ExampleWebApi.Dtos;
using GenericBizRunner;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<IEnumerable<TodoItem>> Get([FromServices]ICrudServices service)
        {
            return service.ReadManyNoTracked<TodoItem>().ToList();
        }

        /// <summary>
        /// Gets the TodoItem with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        // GET api/todo/5
        [HttpGet("{id}")]
        public ActionResult<TodoItem> Get(int id, [FromServices]ICrudServices service)
        {
            return service.Response(service.ReadSingle<TodoItem>(id));
        }

        // POST api/todo {name='name', difficulty=1}
        /// <summary>
        /// Creates a new item and returns the created entity, with the Id value provided by the database
        /// NOTE: to show how business logic might work I added extra validation (name can't end with !) in the business logic
        /// </summary>
        /// <param name="item"></param>
        /// <param name="service"></param>
        [HttpPost]
        public ActionResult<TodoItem> Post(CreateTodoDto item, [FromServices]IActionService<ICreateTodoBizLogic> service)
        {
            return service.Status.Response(service.RunBizAction<TodoItem>(item));
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
        public IActionResult PutName(ChangeNameDto dto, [FromServices]ICrudServices service)
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
        public IActionResult PutDifficuty(ChangeDifficultyDto dto, [FromServices]ICrudServices service)
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
        public IActionResult Delete(int id, [FromServices]ICrudServices service)
        {
            service.DeleteAndSave<TodoItem>(id);
            return service.Response();
        }
    }
}
