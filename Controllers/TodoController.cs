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

private ElasticClient _client;
public TodoController()
{
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("todo");

    _client = new ElasticClient(settings);
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
            var todoItem = new TodoItem() { Id = id, Name = "Todo # 1", IsComplete = true};
            return Ok(todoItem);
        }

    }
}