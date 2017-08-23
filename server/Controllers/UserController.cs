using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ebookhub.Data;
using ebookhub.Models;

namespace ebookhub.Controllers
{
    [Route("api/[controller]")]
    public class UserController
    {
        private readonly IUserRepository _repository;
        private readonly ILogger _logger;
        public UserController(IUserRepository repository, ILogger<UserController> logger)
        {
            _repository = repository;
            _logger = logger;
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
