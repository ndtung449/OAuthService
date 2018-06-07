using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Core.Exceptions;

namespace OAuthService.Core.Services
{
    public class ApiResourceService : IApiResourceService
    {
        private readonly IConfigurationRepository<ApiResource> _apiResourceRepository;
        private readonly ISecretGenerator _secretGenerator;

        public ApiResourceService(IConfigurationRepository<ApiResource> apiResourceRepository, ISecretGenerator secretGenerator)
        {
            _apiResourceRepository = apiResourceRepository;
            _secretGenerator = secretGenerator;
        }

        public async Task<PageResult<ApiResourceViewModel>> Get(int take = 100, int skip = 0)
        {
            IQueryable<ApiResource> query = _apiResourceRepository.Query().AsNoTracking();

            int total = await query.CountAsync();

            List<ApiResourceViewModel> items = await query
                .Include(r => r.Scopes)
                .Include(r => r.UserClaims)
                .Skip(skip)
                .Take(take)
                .Select(resource => MapApiResourceViewModel(resource))
                .ToListAsync();

            return new PageResult<ApiResourceViewModel>(total, items);
        }

        public async Task<ApiResourceViewModel> GetByName(string name)
        {
            ApiResource resource = await FindByName(name, includeRelatedEntities: true, throwIfNotFound: true);

            return MapApiResourceViewModel(resource);
        }

        public async Task Create(ApiResourceForm form)
        {
            ApiResource existResource = await FindByName(form.Name);

            if (existResource != null)
            {
                throw new BadRequestException($"ApiResource with name '{form.Name}' already exist.");
            }

            ApiResource resource = BuildApiResource(form);
            _apiResourceRepository.Add(resource);
            await _apiResourceRepository.SaveChangesAsync();
        }

        private ApiResource BuildApiResource(ApiResourceForm form)
        {
            ApiResource resource = new ApiResource
            {
                Description = form.Description,
                DisplayName = form.DisplayName,
                Name = form.Name
            };

            if (form.Scopes != null && form.Scopes.Any())
            {
                resource.Scopes = form
                    .Scopes
                    .Select(s => new ApiScope { Name = s })
                    .ToList();
            }

            if (form.UserClaims != null && form.UserClaims.Any())
            {
                resource.UserClaims = form
                    .UserClaims
                    .Select(c => new ApiResourceClaim { Type = c })
                    .ToList();
            }

            return resource;
        }

        public async Task Update(UpdateApiResourceForm form)
        {
            ApiResource resource = await FindByName(
                form.Name,
                tracking: true,
                includeRelatedEntities: true,
                throwIfNotFound: true);

            resource.Description = form.Description;
            resource.DisplayName = form.DisplayName;
            resource.Scopes = form.Scopes?.Select(s => new ApiScope { Name = s }).ToList();
            resource.UserClaims = form.UserClaims?.Select(c => new ApiResourceClaim { Type = c }).ToList();

            await _apiResourceRepository.SaveChangesAsync();
        }

        public async Task CreateApiSecret(string name, ApiSecretForm form)
        {
            ApiResource resource = await _apiResourceRepository
                .Query()
                .Include(r => r.Secrets)
                .FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (resource == null)
            {
                throw new BadRequestException($"No ApiResource with name '{name}' found.");
            }

            resource.Secrets.Add(new ApiSecret
            {
                Description = form.Description,
                Value = _secretGenerator.Hash(form.Secret)
            });

            await _apiResourceRepository.SaveChangesAsync();
        }

        public async Task Delete(string name)
        {
            ApiResource resource = await FindByName(name, tracking: true, throwIfNotFound: true);

            _apiResourceRepository.Remove(resource);

            await _apiResourceRepository.SaveChangesAsync();
        }

        private async Task<ApiResource> FindByName(
            string name,
            bool tracking = false,
            bool includeRelatedEntities = false,
            bool throwIfNotFound = false)
        {
            IQueryable<ApiResource> query = _apiResourceRepository.Query();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (includeRelatedEntities)
            {
                query = query
                .Include(r => r.Scopes)
                .Include(r => r.UserClaims);
            }

            ApiResource resource = await query.FirstOrDefaultAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (resource == null && throwIfNotFound)
            {
                throw new BadRequestException($"No ApiResource with name '{name}' found.");
            }

            return resource;
        }

        private ApiResourceViewModel MapApiResourceViewModel(ApiResource resource)
        {
            return new ApiResourceViewModel
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                Name = resource.Name,
                Scopes = resource.Scopes.Select(s => s.Name).ToList(),
                UserClaims = resource.UserClaims.Select(s => s.Type).ToList()
            };
        }
    }
}
