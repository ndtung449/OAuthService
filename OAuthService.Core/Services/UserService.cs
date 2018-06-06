using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OAuthService.Exceptions;
using OAuthService.Domain.Entities;

namespace OAuthService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        
        public UserService(
            IRepository<User> userRepository,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task Create(UserForm form)
        {
            User existUser = await FindByUserName(form.UserName);
            if(existUser != null)
            {
                throw new BadRequestException($"An user with UserName '{form.UserName}' already exist.");
            }

            await _userManager.CreateAsync(new User
            {
                UserName = form.UserName,
                Email = form.Email,
                FirstName = form.FirstName,
                LastName = form.LastName
            }, form.Password);
        }

        public async Task Update(UserUpdateForm form)
        {
            User user = await FindByUserName(form.UserName, tracking: true, throwsIfNotFound: true);

            user.Email = form.Email;
            user.FirstName = form.FirstName;
            user.LastName = form.LastName;

            //using(IDbContextTransaction transaction = _userRepository.)
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

            if(user == null && throwsIfNotFound)
            {
                throw new BadRequestException($"No User with UserName '{userName}' found.");
            }

            return user;
        }

        public async Task<PageResult<UserViewModel>> Get(string name, bool isBlocked = false, int take = 100, int skip = 0)
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

            List<UserViewModel> items = new List<UserViewModel>();
            foreach(User user in users)
            {
                UserViewModel item = await MapUserViewModel(user);
                items.Add(item);
            }

            return new PageResult<UserViewModel>(total, items);
        }


        
        private async Task<UserViewModel> MapUserViewModel (User user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            List<UserClaim> userClaims = claims.Select(claim => new UserClaim
            {
                Type = claim.Type,
                Value = claim.Value
            }).ToList();

            return new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Claims = userClaims,
                Roles = roles.ToList()
            };
        }
    }
}
