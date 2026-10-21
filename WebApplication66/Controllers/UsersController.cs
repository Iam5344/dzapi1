using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManager.Data;
using UserManager.DTOs;

namespace UserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryParameters queryParams)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryParams.Username))
            {
                query = query.Where(u => u.Username.Contains(queryParams.Username));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Email))
            {
                query = query.Where(u => u.Email.Contains(queryParams.Email));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Role))
            {
                query = query.Where(u => u.Role.ToLower() == queryParams.Role.ToLower());
            }

            query = queryParams.SortBy?.ToLower() switch
            {
                "username" => query.OrderBy(u => u.Username),
                "username_desc" => query.OrderByDescending(u => u.Username),
                "email" => query.OrderBy(u => u.Email),
                "email_desc" => query.OrderByDescending(u => u.Email),
                "date" => query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };

            int totalCount = await query.CountAsync();
            var users = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<UserReadDto>>(users);

            var result = new PagedResultDto<UserReadDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };

            return Ok(result);
        }
    }
}
