﻿using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Exceptions;
using OAuthService.Core;

namespace OAuthService.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IConfigurationRepository<Client> _clientRepository;
        private readonly ISecretGenerator _secretGenerator;

        public ClientService(IConfigurationRepository<Client> clientRepository, ISecretGenerator secretGenerator)
        {
            _clientRepository = clientRepository;
            _secretGenerator = secretGenerator;
        }

        public async Task<ClientViewModel> GetByClientId(string clientId)
        {
            Client client = await FindByClientId(clientId, includeRelatedEntities: true);

            if (client == null)
            {
                return null;
            }

            return MapClientViewModel(client);
        }

        public async Task<ClientViewModel> GetByUri(string uri)
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

        public async Task<ClientCreated> Create(Client client)
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

            return new ClientCreated
            {
                ClientId = clientId,
                Secret = secret
            };
        }

        public async Task<PageResult<ClientViewModel>> Get(string name, string grantType, int take = 100, int skip = 0)
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

            List<ClientViewModel> items = clients.Select(c => MapClientViewModel(c)).ToList();

            return new PageResult<ClientViewModel>(total, items);
        }

        public async Task Update(UpdateClientForm form)
        {
            Client existClient = await FindByClientId(
                form.ClientId,
                tracking: true,
                includeRelatedEntities: true,
                throwIfNotFound: true);

            existClient.ClientName = form.ClientName;
            existClient.ClientUri = form.Uri;
            existClient.AllowedGrantTypes = form.GrantTypes?.Select(t => new ClientGrantType { GrantType = t }).ToList();
            existClient.AllowedScopes = form.Scopes?.Select(s => new ClientScope { Scope = s }).ToList();
            existClient.PostLogoutRedirectUris = form
                .PostLogoutRedirectUris?
                .Select(u => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = u })
                .ToList();
            existClient.RedirectUris = form.RedirectUris?.Select(u => new ClientRedirectUri { RedirectUri = u }).ToList();

            await _clientRepository.SaveChangesAsync();
        }

        public async Task Delete(string clientId)
        {
            Client existClient = await FindByClientId(clientId, tracking: true, throwIfNotFound: true);
            _clientRepository.Remove(existClient);
            await _clientRepository.SaveChangesAsync();
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

        private ClientViewModel MapClientViewModel(Client client)
        {
            return new ClientViewModel
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