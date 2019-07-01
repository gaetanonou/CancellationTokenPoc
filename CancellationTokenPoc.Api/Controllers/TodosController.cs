using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CancellationTokenPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        public TodosController(IHostingEnvironment hostingEnvironment, ILogger<TodosController> logger)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> Get(string searchTerms, CancellationToken cancellationToken)
        {
            Guid guid = Guid.NewGuid();
            try
            {
                logger.LogInformation($"****** REQUEST STARTED {guid} ******");
                
                await Task.Delay(2000, cancellationToken);
                IEnumerable<Todo> todos = GetTodos(searchTerms);
                
                logger.LogInformation($"****** REQUEST FINISHED {guid} ******");
                return Ok(todos);

            } catch (OperationCanceledException) 
            {
                logger.LogWarning($"****** REQUEST CANCELED {guid} ******");
                return NoContent();
            }         
        }


        private IEnumerable<Todo> GetTodos(string searchTerms)
        {
            IEnumerable<Todo> todos = Enumerable.Empty<Todo>();
            string filePath = $"{hostingEnvironment.ContentRootPath}\\Data\\todos.json";

            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    todos = JsonConvert.DeserializeObject<List<Todo>>(streamReader.ReadToEnd());
                }

                if(!string.IsNullOrWhiteSpace(searchTerms)){
                    string[] terms = searchTerms.Split(' ');
                    foreach(string term in terms){
                        todos = todos.Where(a => a.Title.Contains(term));
                    }
                }
            }

            return todos;
        }
    }
}
