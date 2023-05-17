using Microsoft.AspNetCore.Mvc;
using ModelBinder.Models;

namespace ModelBinder.Controllers;

[Route("api/[controller]")]
public class TestController: ControllerBase
{
    [HttpPost("processpost")]
    public IActionResult ProcessPost(BoundObject? testObject)
    {
        var a = ModelState;

        var valid = ModelState.IsValid;

        if (testObject == null)
            return BadRequest();

        return Ok(testObject);
    }

    [HttpPost("processyear")]
    public IActionResult ProcessYear([FromBody]YearObject yearObject)
    {
        if(!ModelState.IsValid)
        {
            var Errors = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            var result = new { Error = true, ErrorMessages = Errors.Select(a => a.ErrorMessage).ToList() };

            return BadRequest(result);
        }

        return Ok(yearObject);
    }

    [HttpPost("processlist")]
    public IActionResult ProcessList([FromBody] NatsRecordList recordList)
    {
        return Ok(recordList);
    }
}