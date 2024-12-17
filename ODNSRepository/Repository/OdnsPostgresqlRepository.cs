using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entities.DTO.Request;
using Entities.DTO.Response;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ODNSRepository.Repository
{
    public class OdnsPostgresqlRepository : IOdnsRepository
    {
        private readonly IConfiguration _config;
        private ILogger<OdnsPostgresqlRepository> _logger;

        private string _connectionString { get; set; }
        private NpgsqlDataSource _dataSource;

        //private List<NpgsqlBatchCommand> commmands = new List<NpgsqlBatchCommand>();
        //private Queue<NpgsqlBatchCommand> commandQueue = new Queue<NpgsqlBatchCommand>();
        //private bool isBatchSent = false;

        private JsonSerializerOptions options= new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull

        };
        

        public OdnsPostgresqlRepository(IConfiguration config, ILogger<OdnsPostgresqlRepository> logger)
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
                    cmd.Parameters.AddWithValue("p_input", NpgsqlTypes.NpgsqlDbType.Jsonb, JsonSerializer.Serialize<TInput>(input,options));
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

        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequest request)
        {
            try
            {
                DnsEntryFilterDTO dtoFilter = new DnsEntryFilterDTO(request.filter);
                GetDnsEntriesRequestDTO dtoRequest = new GetDnsEntriesRequestDTO()
                {
                    filter = dtoFilter,
                    pagination = request.pagination,
                    sort = request.sort,
                    rid = request.rid,
                    latest = request.latest
                };
                GetDnsEntriesResponseDTO result = await QueryDB<GetDnsEntriesRequestDTO, GetDnsEntriesResponseDTO>(dtoRequest, _config["Database:Functions:GetDnsEntries"]);
                GetDnsEntriesResponse finalResult = result.GetDisplayResponse();
                return finalResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
