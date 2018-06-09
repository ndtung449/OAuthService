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
    public class ClientService : BaseService, IClientService
    {
        private readonly IConfigurationRepository<Client> _clientRepository;
        private readonly ISecretGenerator _secretGenerator;

        public ClientService(
            IHttpContextAccessor contextAccessor,
            IConfigurationRepository<Client> clientRepository,
            ISecretGenerator secretGenerator) : base(contextAccessor)
        {
            _clientRepository = clientRepository;
            _secretGenerator = secretGenerator;
        }

        public async Task<ClientDto> GetByClientId(string clientId)
        {
            Client client = await FindByClientId(clientId, includeRelatedEntities: true);

            if (client == null)
            {
                return null;
            }

            return MapClientViewModel(client);
        }

        public async Task<ClientDto> GetByUri(string uri)
        {
            Client client = await _clientRepository
                .Query()
                .Include(c => c.AllowedGrantTypes)
                .Include(c => c.AllowedScopes)
                .Include(c => c.PostLogoutRedirectUris)
                .Include(c => c.RedirectUris)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => string.Equals(c.ClientUri, uri, StringComparison.OrdinalIgnoreCase));

            if (client == null)
            {
                return null;
            }

            return MapClientViewModel(client);
        }

        public async Task<ClientCreatedDto> CreateHasRedirectUri(HasRedirectUriClientCreateDto dto, ICollection<string> grantTypes)
        {
            EnsureModelValid(dto);

            Client client = BuildClient(
                dto.Name,
                dto.Uri,
                dto.Scopes,
                grantTypes,
                dto.RedirectUri,
                dto.PostLogoutRedirectUri);

            return await Create(client);
        }

        public async Task<ClientCreatedDto> CreateNoRedirectUri(NoRedirectUriClientCreateDto dto, ICollection<string> grantTypes)
        {
            EnsureModelValid(dto);

            Client client = BuildClient(
                dto.Name,
                dto.Uri,
                dto.Scopes,
                grantTypes);

            return await Create(client);
        }

        public async Task<PageResult<ClientDto>> Get(string name, string grantType, int take = 100, int skip = 0)
        {
            IQueryable<Client> query = _clientRepository
                .Query()
                .Include(c => c.AllowedGrantTypes)
                .Include(c => c.AllowedScopes)
                .Include(c => c.PostLogoutRedirectUris)
                .Include(c => c.RedirectUris)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(client => client.ClientName.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(grantType))
            {
                query = query.Where(client =>
                    client.AllowedGrantTypes.FirstOrDefault(t => t.GrantType.Equals(grantType, StringComparison.OrdinalIgnoreCase)) == null);
            }

            int total = await query.CountAsync();

            List<Client> clients = await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            List<ClientDto> items = clients.Select(c => MapClientViewModel(c)).ToList();

            return new PageResult<ClientDto>(total, items);
        }

        public async Task Update(ClientUpdateDto dto)
        {
            EnsureModelValid(dto);

            Client existClient = await FindByClientId(
                dto.ClientId,
                tracking: true,
                includeRelatedEntities: true,
                throwIfNotFound: true);

            existClient.ClientName = dto.ClientName;
            existClient.ClientUri = dto.Uri;
            existClient.AllowedGrantTypes = dto.GrantTypes?.Select(t => new ClientGrantType { GrantType = t }).ToList();
            existClient.AllowedScopes = dto.Scopes?.Select(s => new ClientScope { Scope = s }).ToList();
            existClient.PostLogoutRedirectUris = dto
                .PostLogoutRedirectUris?
                .Select(u => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = u })
                .ToList();
            existClient.RedirectUris = dto.RedirectUris?.Select(u => new ClientRedirectUri { RedirectUri = u }).ToList();

            await _clientRepository.SaveChangesAsync();
        }

        public async Task Delete(string clientId)
        {
            Client existClient = await FindByClientId(clientId, tracking: true, throwIfNotFound: true);
            _clientRepository.Remove(existClient);
            await _clientRepository.SaveChangesAsync();
        }

        private async Task<ClientCreatedDto> Create(Client client)
        {
            if (!string.IsNullOrEmpty(client.ClientUri))
            {
                Client existClient = await FindByUri(client.ClientUri);

                if (existClient != null)
                {
                    throw new BadRequestException($"A client with uri '{client.ClientUri}' already exists.");
                }
            }

            string clientId = Guid.NewGuid().ToString();
            client.ClientId = clientId;

            string secret = _secretGenerator.Create();
            client.ClientSecrets = new List<ClientSecret>
            {
                new ClientSecret
                {
                    Value = _secretGenerator.Hash(secret),
                    Expiration = DateTime.Now.AddYears(Constants.ClientSecretLifeTimeInYear)
                }
            };

            _clientRepository.Add(client);
            await _clientRepository.SaveChangesAsync();

            return new ClientCreatedDto
            {
                ClientId = clientId,
                Secret = secret
            };
        }

        private Client BuildClient(
            string name,
            string uri,
            List<string> scopes,
            ICollection<string> grantTypes,
            string redirectUri = null,
            string postLogoutRedirectUri = null)
        {
            return new Client
            {
                ClientName = name,
                ClientUri = uri,
                AllowedScopes = scopes.Select(scope => new ClientScope { Scope = scope }).ToList(),
                AllowedGrantTypes = grantTypes.Select(granType => new ClientGrantType { GrantType = granType }).ToList(),
                RedirectUris = redirectUri != null
                    ? new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = redirectUri } }
                    : new List<ClientRedirectUri>(),
                PostLogoutRedirectUris = postLogoutRedirectUri != null
                    ? new List<ClientPostLogoutRedirectUri> { new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = postLogoutRedirectUri } }
                    : new List<ClientPostLogoutRedirectUri>(),
            };
        }

        private async Task<Client> FindByClientId(
            string clientId,
            bool tracking = false,
            bool includeRelatedEntities = false,
            bool throwIfNotFound = false)
        {
            IQueryable<Client> query = _clientRepository.Query();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (includeRelatedEntities)
            {
                query = query
                    .Include(c => c.AllowedGrantTypes)
                    .Include(c => c.AllowedScopes)
                    .Include(c => c.PostLogoutRedirectUris)
                    .Include(c => c.RedirectUris);
            }

            Client client = await query.FirstOrDefaultAsync(c => c.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase));

            if (client == null && throwIfNotFound)
            {
                throw new BadRequestException($"No Client with ClientId '{clientId}' found.");
            }

            return client;
        }

        private async Task<Client> FindByUri(string uri, bool tracking = false)
        {
            IQueryable<Client> query = _clientRepository.Query();

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(c => c.ClientUri == uri);
        }

        private ClientDto MapClientViewModel(Client client)
        {
            return new ClientDto
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                GrantTypes = client.AllowedGrantTypes.Select(t => t.GrantType).ToList(),
                Scope = client.AllowedScopes.Select(s => s.Scope).ToList(),
                RedirectUris = client.RedirectUris.Select(u => u.RedirectUri).ToList(),
                PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(u => u.PostLogoutRedirectUri).ToList()
            };
        }
    }
}
