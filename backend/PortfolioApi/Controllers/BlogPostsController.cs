using Microsoft.AspNetCore.Mvc;
using PortfolioApi.DTOs;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/blogposts")]  // route die je moet nemen om naar deze endpoint te komen
public class BlogPostsController(IBlogPostService service) : ControllerBase // dependency injection (Iblogpostservice hoort bij controller dus dat inject je hier)
{
    [HttpGet]
    public async Task<ActionResult<List<BlogPostReadDto>>> GetAll() =>    // waarde niet direct terug --> belofte waarde later komt
        Ok(await service.GetAllAsync()); // await want waarde niet direct terug --> eerst CRUD uitvoeren en dan waarde teruggeven (statuscode of dto als json)

// api/blogposts/id (id moet gevuld worden --> komt uit URL van HTTP-request)
    [HttpGet("{id:int}")]  // vertelt asp.net dat id int moet zijn  (modelbinding--> id wordt automatisch gevuld)
    public async Task<ActionResult<BlogPostReadDto>> GetById(int id) // modelbinding (asp.net framework zet waarde in id en dto zelf (dto bij create & delete)) --> bepaalt via waar parameter vandaan komt en waar waarde wordt gezocht
    {
        var post = await service.GetByIdAsync(id); 
        return post is null ? NotFound() : Ok(post); // ternary operator ?: is if/else. voorwaarde ? waardeTrue : waardeFalse;
    } // dus voorwaarde ? ifTrue --> return (...) : ifFalse --> return (...)

// samengevat
// ?: = if/else die waarde oplevert 
// modelbinding = parameters van endpoint worden automatisch gevuld (route voor id, body voor DTO)
// Task<ActionResult<T>> = asynchrone methode die HTTP-antwoord teruggeeft (of statuscode (200/404) of JSON-data)
// GET /api/blogposts/5  (modelbinding --> 5 wordt gevuld als id automatisch)

// Route van HTTP-request --> database --> antwoord:
// HTTP-request --> modelbinding vult id uit URL --> service haalt bericht op --> ternary if/else --> asp.net zet om naar HTTP-statuscode met JSON-body --> client krijgt json

    [HttpPost]
    public async Task<ActionResult<BlogPostReadDto>> Create(BlogPostCreateDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BlogPostUpdateDto dto) =>
        await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await service.DeleteAsync(id) ? NoContent() : NotFound(); // voorwaarde ? IfTrue (nocontent) : IfFalse (notfound)
}