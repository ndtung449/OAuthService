using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OAuthService.Core.Exceptions;
using OAuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace OAuthService.Core.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ILogger _logger;
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        
        public UserService(
            IHttpContextAccessor contextAccessor,
            ILogger<UserService> logger,
            IRepository<User> userRepository,
            UserManager<User> userManager) : base(contextAccessor)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task Create(UserCreateDto dto)
        {
            EnsureModelValid(dto);

            User existUser = await FindByUserName(dto.UserName);
            if(existUser != null)
            {
                throw new BadRequestException($"An user with UserName '{dto.UserName}' already exist.");
            }

            using (IDbContextTransaction transaction = _userRepository.BeginTransaction())
            {
                try
                {
                    await _userManager.CreateAsync(new User
                    {
                        UserName = dto.UserName,
                        Email = dto.Email,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    }, dto.Password);

                    User createdUser = await _userManager.FindByNameAsync(dto.UserName);

                    if (dto.Roles != null && dto.Roles.Any())
                    {
                        await _userManager.AddToRolesAsync(createdUser, dto.Roles);
                    }

                    if (dto.Claims != null && dto.Claims.Any())
                    {
                        await _userManager.AddClaimsAsync(createdUser, dto.Claims.Select(c => new Claim(c.Type, c.Value)).ToArray());
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Error while creating user");
                    throw;
                }
            }
        }

        public async Task Update(UserUpdateDto dto)
        {
            EnsureModelValid(dto);

            User user = await FindByUserName(dto.UserName, tracking: true, throwsIfNotFound: true);

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.IsBlocked = dto.IsBlocked;

            IList<string> currentRoles = await _userManager.GetRolesAsync(user);
            IList<Claim> currentClaims = await _userManager.GetClaimsAsync(user);
            IList<Claim> newClaims = dto.Claims?.Select(c => new Claim(c.Type, c.Value)).ToList();

            using (IDbContextTransaction transaction = _userRepository.BeginTransaction())
            {
                try
                {
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }

                    if (currentClaims.Any())
                    {
                        await _userManager.RemoveClaimsAsync(user, currentClaims);
                    }

                    if (dto.Roles != null && dto.Roles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, dto.Roles);
                    }

                    if (newClaims != null && newClaims.Any())
                    {
                        await _userManager.AddClaimsAsync(user, newClaims);
                    }

                    await _userRepository.SaveChangesAsync();

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Error while updating user");
                    throw;
                }
            }
        }

        public async Task<PageResult<UserDto>> Get(string name, bool isBlocked = false, int skip = 0, int take = 100)
        {
            IQueryable<User> query = _userRepository
                .Query()
                .AsNoTracking()
                .Where(user => user.IsBlocked == isBlocked);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(user => 
                    user.FirstName.ToLower().Contains(name.ToLower())
                    || user.LastName.ToLower().Contains(name.ToLower()));
            }

            int total = await query.CountAsync();

            List<User> users = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            List<UserDto> items = new List<UserDto>();
            foreach(User user in users)
            {
                UserDto item = await MapUserDtoWithRolesAndClaims(user);
                items.Add(item);
            }

            return new PageResult<UserDto>(total, items);
        }

        public async Task<UserDto> GetByUserName(string userName)
        {
            User user = await FindByUserName(userName, throwsIfNotFound: true);

            return await MapUserDtoWithRolesAndClaims(user);
        }

        public async Task Delete(string userName)
        {
            User user = await FindByUserName(userName, tracking: true, throwsIfNotFound: true);
            await _userManager.DeleteAsync(user);
        }

        private async Task<User> FindByUserName(
            string userName,
            bool tracking = false,
            bool throwsIfNotFound = false)
        {
            IQueryable<User> query = _userManager.Users;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            User user = await query.FirstOrDefaultAsync(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user == null && throwsIfNotFound)
            {
                throw new BadRequestException($"No User with UserName '{userName}' found.");
            }

            return user;
        }

        private async Task<UserDto> MapUserDtoWithRolesAndClaims (User user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            List<UserClaim> userClaims = claims.Select(claim => new UserClaim
            {
                Type = claim.Type,
                Value = claim.Value
            }).ToList();

            return new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Claims = userClaims,
                Roles = roles.ToList()
            };
        }
    }
}
