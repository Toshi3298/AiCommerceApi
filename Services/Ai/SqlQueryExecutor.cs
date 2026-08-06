using AiCommerceApi.Dtos.Products;
using Microsoft.Data.SqlClient;

namespace AiCommerceApi.Services.Ai;

public sealed class SqlQueryExecutor : ISqlQueryExecutor
{
    private readonly string _connectionString;

    public SqlQueryExecutor(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString(
                "AiReadOnlyConnection")
            ?? throw new InvalidOperationException(
                "AI salt-okunur bağlantısı bulunamadı.");
    }

    public async Task<List<ProductDto>>
        ExecuteProductQueryAsync(
            string sql,
            CancellationToken cancellationToken)
    {
        var products = new List<ProductDto>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            connection.CreateCommand();

        command.CommandText = sql;
        command.CommandTimeout = 5;

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(new ProductDto
            {
                Id = reader.GetInt32(
                    reader.GetOrdinal("Id")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                Description = reader.IsDBNull(
                    reader.GetOrdinal("Description"))
                        ? string.Empty
                        : reader.GetString(
                            reader.GetOrdinal("Description")),

                Brand = reader.GetString(
                    reader.GetOrdinal("Brand")),

                Price = reader.GetDecimal(
                    reader.GetOrdinal("Price")),

                Stock = reader.GetInt32(
                    reader.GetOrdinal("Stock")),

                IsActive = reader.GetBoolean(
                    reader.GetOrdinal("IsActive")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                CategoryId = reader.GetInt32(
                    reader.GetOrdinal("CategoryId")),
                    
                ImageUrl = reader.IsDBNull(
                    reader.GetOrdinal("ImageUrl"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("ImageUrl")),

                CategoryName = reader.GetString(
                    reader.GetOrdinal("CategoryName"))
            });
        }

        return products;
    }
}