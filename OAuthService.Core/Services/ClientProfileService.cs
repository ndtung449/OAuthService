using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Core.Exceptions;
using OAuthService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace OAuthService.Core.Services
{
    public class ClientProfileService : BaseService, IClientProfileService
    {
        private readonly IRepository<ClientProfile> _clientRepository;

        public ClientProfileService(IHttpContextAccessor contextAccessor, IRepository<ClientProfile> clientRepository)
            : base(contextAccessor)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientProfileDto> Get(string clientId)
        {
            ClientProfile profile = await FindByClientId(clientId, throwsIfNotFound: true);

            return new ClientProfileDto
            {
                ClientId = profile.ClientId,
                FaviconLocalUrl = profile.FaviconLocalUrl,
                FaviconUrl = profile.FaviconUrl
            };
        }

        public async Task Create(string clientId, ClientProfileCreateDto dto)
        {
            EnsureModelValid(dto);

            ClientProfile existProfile = await FindByClientId(clientId, tracking: true);
            if(existProfile != null)
            {
                throw new BadRequestException($"ClientProfile with ClientId '{clientId}' already exist.");
            }

            ClientProfile profile = new ClientProfile
            {
                ClientId = clientId,
                FaviconLocalUrl = dto.FaviconLocalUrl,
                FaviconUrl = dto.FaviconUrl
            };

            _clientRepository.Add(profile);
            await _clientRepository.SaveChangesAsync();
        }

        public async Task Update(string clientId, ClientProfileUpdateDto dto)
        {
            EnsureModelValid(dto);

            ClientProfile profile = await FindByClientId(clientId, tracking: true, throwsIfNotFound: true);
            profile.FaviconLocalUrl = dto.FaviconLocalUrl;
            profile.FaviconUrl = dto.FaviconUrl;
            await _clientRepository.SaveChangesAsync();
        }
        
        public async Task Delete(string clientId)
        {
            ClientProfile profile = await FindByClientId(clientId, tracking: true, throwsIfNotFound: true);
            _clientRepository.Remove(profile);
            await _clientRepository.SaveChangesAsync();
        }
        
        private async Task<ClientProfile> FindByClientId(
            string clientId,
            bool tracking = false,
            bool throwsIfNotFound = false)
        {
            IQueryable<ClientProfile> query = _clientRepository.Query();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            ClientProfile profile = await query.FirstOrDefaultAsync(p => p.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase));

            if (profile == null && throwsIfNotFound)
            {
                throw new BadRequestException($"No ClientProfile with ClientId '{clientId}' found.");
            }

            return profile;
        }
    }
}
