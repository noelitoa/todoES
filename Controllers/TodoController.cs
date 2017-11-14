using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using Microsoft.AspNetCore;
using Nest;
using Elasticsearch.Net;

namespace TodoApi.Controllers{

[Route("api/[controller]")]
    public class TodoController : Controller
    {

private static ElasticClient _client;

        private static ElasticClient GetESClient()
        {
            
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                                    .DefaultIndex("todo");

                return new ElasticClient(settings);
        }        

        public TodoController()
        {
            if (_client == null)
            {
                _client = GetESClient();
            }
        }

        // POST api/todo
        [HttpPost]
        public IActionResult  Post([FromBody]TodoItem todoItem)
        {
            if (todoItem == null)
            return NoContent();

           
            var response = _client.Index(todoItem, i => i
      .Index("todo")
      .Type("todoitem")
      .Id(todoItem.Id)
      .Refresh(Refresh.True));

            return CreatedAtRoute("GetTodo", new { id = todoItem.Id }, todoItem);
        }

        // GET api/todo/5
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult  Get(int id)
        {
            //var todoItem = new TodoItem() { Id = id, Name = "Todo # 1", IsComplete = true};

            var result = _client.Search<TodoItem>(s => s
                    .Index("todo")
                    .Type("todoitem")
                    .From(0)
                    .Size(1000)
                    .Query(q => q.MatchAll()));

            var listRequests = new List<TodoItem>();

                foreach (var hit in result.Hits)
                {
                        listRequests.Add(new TodoItem()
                        {
                            Name = hit.Source.Name,
                            IsComplete = hit.Source.IsComplete
                        });
                }

            return Ok(listRequests);
        }

        // GET api/todo
        [HttpGet( Name = "GetTodos")]
        public IActionResult  GetAll(int id)
        {
            //var todoItem = new TodoItem() { Id = id, Name = "Todo # 1", IsComplete = true};

            var result = _client.Search<TodoItem>(s => s
                    .Index("todo")
                    .Type("todoitem")
                    .From(0)
                    .Size(1000)
                    .Query(q => q.MatchAll()));

            return Ok(result);
        }

    }
}