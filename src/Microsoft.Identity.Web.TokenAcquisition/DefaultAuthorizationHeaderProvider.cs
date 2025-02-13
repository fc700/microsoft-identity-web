﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Abstractions;

namespace Microsoft.Identity.Web
{
    internal sealed class DefaultAuthorizationHeaderProvider : IAuthorizationHeaderProvider
    {
        private readonly ITokenAcquisition _tokenAcquisition;

        public DefaultAuthorizationHeaderProvider(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        /// <inheritdoc/>
        public async Task<string> CreateAuthorizationHeaderForUserAsync(
            IEnumerable<string> scopes,
            AuthorizationHeaderProviderOptions? downstreamApiOptions = null,
            ClaimsPrincipal? claimsPrincipal = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _tokenAcquisition.GetAuthenticationResultForUserAsync(
                scopes,
                downstreamApiOptions?.AcquireTokenOptions.AuthenticationOptionsName,
                downstreamApiOptions?.AcquireTokenOptions.Tenant,
                downstreamApiOptions?.AcquireTokenOptions.UserFlow,
                claimsPrincipal,
                CreateTokenAcquisitionOptionsFromApiOptions(downstreamApiOptions, cancellationToken)).ConfigureAwait(false);
            return result.CreateAuthorizationHeader();
        }

        /// <inheritdoc/>
        public async Task<string> CreateAuthorizationHeaderForAppAsync(string scopes, AuthorizationHeaderProviderOptions? downstreamApiOptions = null, CancellationToken cancellationToken = default)
        {
            var result = await _tokenAcquisition.GetAuthenticationResultForAppAsync(
                scopes,
                downstreamApiOptions?.AcquireTokenOptions.AuthenticationOptionsName,
                downstreamApiOptions?.AcquireTokenOptions.Tenant,
                CreateTokenAcquisitionOptionsFromApiOptions(downstreamApiOptions, cancellationToken)).ConfigureAwait(false);
            return result.CreateAuthorizationHeader();
        }

        private static TokenAcquisitionOptions CreateTokenAcquisitionOptionsFromApiOptions(AuthorizationHeaderProviderOptions? downstreamApiOptions, CancellationToken cancellationToken)
        {
            return new TokenAcquisitionOptions()
            {
                AuthenticationOptionsName = downstreamApiOptions?.AcquireTokenOptions.AuthenticationOptionsName,
                CancellationToken = cancellationToken,
                Claims = downstreamApiOptions?.AcquireTokenOptions.Claims,
                CorrelationId = downstreamApiOptions?.AcquireTokenOptions.CorrelationId ?? Guid.Empty,
                ExtraQueryParameters = downstreamApiOptions?.AcquireTokenOptions.ExtraQueryParameters,
                ForceRefresh = downstreamApiOptions?.AcquireTokenOptions.ForceRefresh ?? false,
                LongRunningWebApiSessionKey = downstreamApiOptions?.AcquireTokenOptions.LongRunningWebApiSessionKey,
                Tenant = downstreamApiOptions?.AcquireTokenOptions.Tenant,
                UserFlow = downstreamApiOptions?.AcquireTokenOptions.UserFlow,
                PopPublicKey = downstreamApiOptions?.AcquireTokenOptions.PopPublicKey,
            };
        }
    }
}
