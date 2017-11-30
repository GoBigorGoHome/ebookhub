using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ebookhub.Data;
using ebookhub.Infrastructure;
using ebookhub.Models;
using Microsoft.Extensions.Options;

namespace ebookhub.Controllers
{
    [Route("api/[controller]")]
    public class UserController
    {
        private readonly IUserRepository _repository;
        private readonly TokenOptions _tokenOptions;
        private readonly ILogger _logger;
        private readonly AuthenticationTools _authTools;
        public UserController(IUserRepository repository, IOptions<TokenOptions> tokenOptions, ILogger<UserController> logger)
        {
            _repository = repository;
            _tokenOptions = tokenOptions.Value;
            _logger = logger;
            _authTools = new AuthenticationTools(tokenOptions);
        }

        [HttpPut("login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var users = await _repository.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Name == userName);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

            if (!_authTools.CheckPassword(user, password))
            {
                return new UnauthorizedResult();
            }

            var requestAt = DateTime.Now;
            var expiresIn = requestAt + new TimeSpan(0, _tokenOptions.ExpiryMinutes, 0);
            var token = _authTools.GenerateToken(user, expiresIn);

            return new OkObjectResult(new {token=token});
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(User user)
        {
            var users = await _repository.GetAllUsers();
            if (users.Any(u => u.Name == user.Name))
            {
                return new BadRequestResult();
            }

            _authTools.HashPassword(user);

            await _repository.AddUser(user);

            return new CreatedResult("api/user", user);
        }

        [HttpGet]
        public async Task<User> GetUser()
        {
            var result = await _repository.GetAllUsers();
            return result.FirstOrDefault();
        }

        [HttpPost]
        public async Task<ActionResult> StoreSettings([FromBody] User user)
        {
            await _repository.UpdateUser(user);
            return new OkResult();
        }
    }
}
