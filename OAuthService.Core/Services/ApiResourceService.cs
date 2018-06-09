using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace OAuthService.Core.Services
{
    public class ApiResourceService : BaseService, IApiResourceService
    {
        private readonly IConfigurationRepository<ApiResource> _apiResourceRepository;
        private readonly ISecretGenerator _secretGenerator;

        public ApiResourceService(
            IHttpContextAccessor contextAccessor,
            IConfigurationRepository<ApiResource> apiResourceRepository,
            ISecretGenerator secretGenerator) : base(contextAccessor)
        {
            _apiResourceRepository = apiResourceRepository;
            _secretGenerator = secretGenerator;
        }

        public async Task<PageResult<ApiResourceDto>> Get(int take = 100, int skip = 0)
        {
            IQueryable<ApiResource> query = _apiResourceRepository.Query().AsNoTracking();

            int total = await query.CountAsync();

            List<ApiResourceDto> items = await query
                .Include(r => r.Scopes)
                .Include(r => r.UserClaims)
                .Skip(skip)
                .Take(take)
                .Select(resource => MapApiResourceViewModel(resource))
                .ToListAsync();

            return new PageResult<ApiResourceDto>(total, items);
        }

        public async Task<ApiResourceDto> GetByName(string name)
        {
            ApiResource resource = await FindByName(name, includeRelatedEntities: true, throwIfNotFound: true);

            return MapApiResourceViewModel(resource);
        }

        public async Task Create(ApiResourceCreateDto dto)
        {
            EnsureModelValid(dto);

            ApiResource existResource = await FindByName(dto.Name);

            if (existResource != null)
            {
                throw new BadRequestException($"ApiResource with name '{dto.Name}' already exist.");
            }

            ApiResource resource = BuildApiResource(dto);
            _apiResourceRepository.Add(resource);
            await _apiResourceRepository.SaveChangesAsync();
        }

        private ApiResource BuildApiResource(ApiResourceCreateDto form)
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

        public async Task Update(ApiResourceUpdateDto dto)
        {
            EnsureModelValid(dto);

            ApiResource resource = await FindByName(
                dto.Name,
                tracking: true,
                includeRelatedEntities: true,
                throwIfNotFound: true);

            resource.Description = dto.Description;
            resource.DisplayName = dto.DisplayName;
            resource.Scopes = dto.Scopes?.Select(s => new ApiScope { Name = s }).ToList();
            resource.UserClaims = dto.UserClaims?.Select(c => new ApiResourceClaim { Type = c }).ToList();

            await _apiResourceRepository.SaveChangesAsync();
        }

        public async Task CreateApiSecret(string name, ApiSecretCreateDto dto)
        {
            EnsureModelValid(dto);

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
                Description = dto.Description,
                Value = _secretGenerator.Hash(dto.Secret),
                Expiration = DateTime.Now.AddYears(Constants.ApiSecretLifeTimeInYear)
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

        private ApiResourceDto MapApiResourceViewModel(ApiResource resource)
        {
            return new ApiResourceDto
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
