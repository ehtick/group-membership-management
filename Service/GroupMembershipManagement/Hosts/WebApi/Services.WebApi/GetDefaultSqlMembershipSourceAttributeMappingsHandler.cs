// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Hosts.WebApi;
using Microsoft.Data.SqlClient;
using Repositories.Contracts;
using Services.Contracts;
using Services.Messages.Requests;
using Services.Messages.Responses;
using Microsoft.Extensions.Logging;
using SqlMembershipAttributeValueDTO = WebApi.Models.DTOs.SqlMembershipAttributeMapping;

namespace Services
{
    public class GetDefaultSqlMembershipSourceAttributeMappingsHandler : RequestHandlerBase<GetDefaultSqlMembershipSourceAttributeMappingsRequest, GetDefaultSqlMembershipSourceAttributeMappingsResponse>
    {
        private readonly ILogger<GetDefaultSqlMembershipSourceAttributeMappingsHandler> _logger;
        private readonly IDataFactoryRepository _dataFactoryRepository;
        private readonly ISqlMembershipRepository _sqlMembershipRepository;

        public GetDefaultSqlMembershipSourceAttributeMappingsHandler(ILogger<GetDefaultSqlMembershipSourceAttributeMappingsHandler> logger,
                              IDataFactoryRepository dataFactoryRepository,
                              ISqlMembershipRepository sqlMembershipRepository) : base(logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataFactoryRepository = dataFactoryRepository ?? throw new ArgumentNullException(nameof(dataFactoryRepository));
            _sqlMembershipRepository = sqlMembershipRepository ?? throw new ArgumentNullException(nameof(sqlMembershipRepository));
        }

        protected override async Task<GetDefaultSqlMembershipSourceAttributeMappingsResponse> ExecuteCoreAsync(GetDefaultSqlMembershipSourceAttributeMappingsRequest request)
        {
            try
            {
                var response = new GetDefaultSqlMembershipSourceAttributeMappingsResponse();
                var (attributeMappings, hasMore) = await GetSqlAttributeMappingsAsync(request.Attribute, request.Search, request.Top);
                foreach (var attributeValue in attributeMappings)
                {
                    var dto = new SqlMembershipAttributeValueDTO(attributeValue.Code, attributeValue.Description);

                    response.Model.Mappings.Add(dto);
                }
                response.Model.HasMore = hasMore;
                return response;
            }
            catch (Exception ex)
            {
                _logger.SqlFilterAttributeMappingsRetrievalFailed(ex);
                throw ex;
            }
        }

        private async Task<(List<(string Code, string Description)> Mappings, bool HasMore)> GetSqlAttributeMappingsAsync(string attribute, string? search, int? top)
        {
            var tableName = await GetTableNameAsync();
            var attributes = await GetAttributeMappingsAsync(attribute, tableName, search, top);
            return attributes;
        }

        private async Task<(List<(string Code, string Description)> Mappings, bool HasMore)> GetAttributeMappingsAsync(string attribute, string tableName, string? search, int? top)
        {
            try
            {
                // Omitting top lets the repository apply its own default page size.
                return top.HasValue
                    ? await _sqlMembershipRepository.GetAttributeMappingsPageAsync(attribute, tableName, search, top.Value)
                    : await _sqlMembershipRepository.GetAttributeMappingsPageAsync(attribute, tableName, search);
            }
            catch (SqlException ex)
            {
                _logger.SqlAttributeMappingsRetrievalFailed(tableName, ex);
                throw ex;
            }
        }

        private async Task<string> GetTableNameAsync()
        {
            var adfRunId = await GetADFRunIdAsync();
            var tableName = adfRunId.Replace("-", "");
            var tableExists = await CheckIfMappingsTableExistsAsync(tableName);

            return tableExists ? tableName : "";
        }

        private async Task<bool> CheckIfMappingsTableExistsAsync(string tableName)
        {
            bool tableExists = false;

            try
            {
                tableExists = await _sqlMembershipRepository.CheckIfMappingsTableExistsAsync(tableName);
            }
            catch (SqlException ex)
            {
                _logger.SqlMappingsTableExistsCheckFailed(tableName, ex);
                throw ex;
            }

            return tableExists;
        }

        private async Task<string> GetADFRunIdAsync()
        {
            var lastSqlMembershipRunId = await _dataFactoryRepository.GetMostRecentSucceededRunIdAsync();

            if (string.IsNullOrWhiteSpace(lastSqlMembershipRunId))
            {
                _logger.SqlMembershipAdfRunIdNotFound();
                throw new ArgumentException("No SqlMembershipObtainer pipeline run has been found");
            }

            return lastSqlMembershipRunId;
        }
    }
}