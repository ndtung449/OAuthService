using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Exceptions;
using OAuthService.Domain.Entities;

namespace OAuthService.Core.Services
{
    public class ClientProfileService : IClientProfileService
    {
        private readonly IRepository<ClientProfile> _clientRepository;

        public ClientProfileService(IRepository<ClientProfile> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientProfileViewModel> Get(string clientId)
        {
            ClientProfile profile = await FindByClientId(clientId, throwsIfNotFound: true);

            return new ClientProfileViewModel
            {
                ClientId = profile.ClientId,
                FaviconLocalUrl = profile.FaviconLocalUrl,
                FaviconUrl = profile.FaviconUrl
            };
        }

        public async Task Create(string clientId, ClientProfileForm form)
        {
            ClientProfile existProfile = await FindByClientId(clientId, tracking: true);
            if(existProfile != null)
            {
                throw new BadRequestException($"ClientProfile with ClientId '{clientId}' already exist.");
            }

            ClientProfile profile = new ClientProfile
            {
                ClientId = clientId,
                FaviconLocalUrl = form.FaviconLocalUrl,
                FaviconUrl = form.FaviconUrl
            };

            _clientRepository.Add(profile);
            await _clientRepository.SaveChangesAsync();
        }

        public async Task Update(string clientId, ClientProfileUpdateForm form)
        {
            ClientProfile profile = await FindByClientId(clientId, tracking: true, throwsIfNotFound: true);
            profile.FaviconLocalUrl = form.FaviconLocalUrl;
            profile.FaviconUrl = form.FaviconUrl;
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
