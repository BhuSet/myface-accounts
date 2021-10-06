using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using MyFace.Services;
using System.Text;
using System;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController : ControllerBase
    {    
        private readonly IPostsRepo _posts;
        private readonly IAuthService _authservice;

        public PostsController(IPostsRepo posts, IAuthService authService)
        {
            _posts = posts;
            _authservice = authService;
        }
        
        [HttpGet("")]
        public ActionResult<PostListResponse> Search(
                [FromQuery] PostSearchRequest searchRequest,
                [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if(_authservice.Authorize(authorizationHeader))
            {
                var posts = _posts.Search(searchRequest);
                var postCount = _posts.Count(searchRequest);
                return PostListResponse.Create(searchRequest, posts, postCount);
            }
            else
                return new UnauthorizedResult();
        }

        [HttpGet("{id}")]
        public ActionResult<PostResponse> GetById([FromRoute] int id)
        {
            var post = _posts.GetById(id);
            return new PostResponse(post);
        }

        [HttpPost("create")]
        public IActionResult Create(
                    [FromBody] CreatePostRequest newPost,
                    [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_authservice.Authorize(authorizationHeader))
            {
                
                string encodedUsernamePassword = authorizationHeader.Substring("Basic ".Length).Trim();
                string decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                
                string[] usernamePasswordArray = decodedUsernamePassword.Split(':');
                string username = usernamePasswordArray[0];

                var user = _posts.GetByUsername(username);
                Console.WriteLine("user = ");
            
                var post = _posts.Create(newPost, user.Id);
                Console.WriteLine(post.UserId);

                var url = Url.Action("GetById", new { id = post.Id });
                var postResponse = new PostResponse(post);
                return Created(url, postResponse);
            }
            else
                return new UnauthorizedResult();

        }

        [HttpPatch("{id}/update")]
        public ActionResult<PostResponse> Update([FromRoute] int id, [FromBody] UpdatePostRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = _posts.Update(id, update);
            return new PostResponse(post);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _posts.Delete(id);
            return Ok();
        }
    }
}