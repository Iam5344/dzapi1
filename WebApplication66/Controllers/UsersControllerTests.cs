using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManager.Controllers;
using UserManager.Data;
using UserManager.DTOs;
using UserManager.Models;
using UserManager.Profiles;
using Xunit;

namespace UserManager.Tests
{
    public class UsersControllerTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserProfile>(); 
            });
            _mapper = config.CreateMapper();
            _controller = new UsersController(_context, _mapper);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
        {
            _context.Users.Add(new User { Id = 1, Username = "testuser1", Email = "test1@gmail.com", Role = "Admin" });
            _context.Users.Add(new User { Id = 2, Username = "testuser2", Email = "test2@gmail.com", Role = "User" });
            await _context.SaveChangesAsync();


            var result = await _controller.GetUsers(new UserQueryParameters());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var pagedResult = Assert.IsAssignableFrom<PagedResultDto<UserReadDto>>(okResult.Value);
            Assert.Equal(2, pagedResult.TotalCount);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedResult_WhenModelIsValid()
        {
            var createDto = new UserCreateDto
            {
                Username = "newuser",
                Email = "new@gmail.com",
                Role = "User"
            };


            var result = await _controller.CreateUser(createDto);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdUser = Assert.IsType<UserReadDto>(actionResult.Value);
            Assert.Equal("newuser", createdUser.Username);
        }
    }
}
