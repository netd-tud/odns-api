using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SecurityEntities.Request;
using SecurityEntities.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SecurityRepository.SecurityMethods.Authorization.ApiKey
{
    public class ApiKeyPostgresRepository : IApiKeyRepository
    {

        private readonly IConfiguration _config;
        private ILogger<ApiKeyPostgresRepository> _logger;

        private string _connectionString { get; set; }
        private NpgsqlDataSource _dataSource;

        //private List<NpgsqlBatchCommand> commmands = new List<NpgsqlBatchCommand>();
        //private Queue<NpgsqlBatchCommand> commandQueue = new Queue<NpgsqlBatchCommand>();
        //private bool isBatchSent = false;

        private JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ApiKeyPostgresRepository(IConfiguration config, ILogger<ApiKeyPostgresRepository> logger)
        {
            _config = config;
            _logger = logger;


            _connectionString = _config.GetSection("Database:ConnectionString").Value;
            //_connection = new NpgsqlConnection(_connectionString);
            _dataSource = NpgsqlDataSource.Create(_connectionString);
        }

        private async Task<TResult> QueryDB<TInput, TResult>(TInput input, string functionName)
        {
            TResult result = default(TResult);
            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();
            try
            {

                NpgsqlCommand cmd = null;
                string commandText = $"select * from {functionName}(@p_input)";
                NpgsqlDataReader reader = null;

                using (cmd = new NpgsqlCommand(commandText, connection))
                {
                    cmd.Parameters.AddWithValue("p_input", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonSerializer.Serialize<TInput>(input, options));
                    //cmd.Parameters.AddWithValue("p_input", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonSerializer.Serialize<TInput>(input));
                    await cmd.PrepareAsync();

                    object? rawResult = await cmd.ExecuteScalarAsync();
                    if (!System.DBNull.Value.Equals(rawResult) && !String.IsNullOrEmpty((string?)rawResult))
                    {
                        result = JsonSerializer.Deserialize<TResult>(
                            (string)rawResult
                        );
                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error DB : " + ex.Message.ToString());
                throw new Exception($"Error occured while connecting to Database and calling {functionName} ", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<TokenRequestResponse> requestToken(TokenRequest tokenRequest)
        {
            try
            {
                TokenRequestResponse result = await QueryDB<TokenRequest, TokenRequestResponse>(tokenRequest, _config["Database:Functions:RequestToken"]);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TokenInspectResponse> inspectToken(TokenInspectRequest request)
        {
            try
            {
                TokenInspectResponse result = await QueryDB<TokenInspectRequest, TokenInspectResponse>(request, _config["Database:Functions:InspectToken"]);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
