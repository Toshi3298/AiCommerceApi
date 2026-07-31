using System.Text.RegularExpressions;

namespace AiCommerceApi.Services.Ai;

public sealed class SqlSecurityService : ISqlSecurityService
{
    private static readonly string[] ForbiddenKeywords =
    {
        "INSERT",
        "UPDATE",
        "DELETE",
        "DROP",
        "ALTER",
        "TRUNCATE",
        "EXEC",
        "EXECUTE",
        "MERGE",
        "CREATE",
        "GRANT",
        "REVOKE",
        "DENY",
        "DBCC",
        "BACKUP",
        "RESTORE",
        "WAITFOR",
        "OPENROWSET",
        "OPENDATASOURCE"
    };

    public SqlValidationResult Validate(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            return SqlValidationResult.Failure(
                "Üretilen SQL sorgusu boş.");
        }

        string normalizedSql = sql.Trim();

        if (!Regex.IsMatch(
                normalizedSql,
                @"^SELECT\b",
                RegexOptions.IgnoreCase))
        {
            return SqlValidationResult.Failure(
                "Yalnızca SELECT sorgularına izin verilir.");
        }

        foreach (string keyword in ForbiddenKeywords)
        {
            if (Regex.IsMatch(
                    normalizedSql,
                    $@"\b{Regex.Escape(keyword)}\b",
                    RegexOptions.IgnoreCase))
            {
                return SqlValidationResult.Failure(
                    $"Yasaklı SQL ifadesi bulundu: {keyword}");
            }
        }

        string sqlWithoutFinalSemicolon =
            normalizedSql.TrimEnd().TrimEnd(';');

        if (sqlWithoutFinalSemicolon.Contains(';'))
        {
            return SqlValidationResult.Failure(
                "Aynı istekte birden fazla SQL sorgusuna izin verilmez.");
        }

        if (!Regex.IsMatch(
                normalizedSql,
                @"\bFROM\s+(?:\[?dbo\]?\.)?\[?Products\]?\b",
                RegexOptions.IgnoreCase))
        {
            return SqlValidationResult.Failure(
                "Sorgu yalnızca Products tablosundan başlamalıdır.");
        }

        var tableMatches = Regex.Matches(
            normalizedSql,
            @"\b(?:FROM|JOIN)\s+(?:\[?dbo\]?\.)?\[?([A-Za-z0-9_]+)\]?",
            RegexOptions.IgnoreCase);

        var allowedTables = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "Products",
            "Categories"
        };

        foreach (Match tableMatch in tableMatches)
        {
            string tableName = tableMatch.Groups[1].Value;

            if (!allowedTables.Contains(tableName))
            {
                return SqlValidationResult.Failure(
                    $"Bu tabloya erişim izni yok: {tableName}");
            }
        }

        if (!Regex.IsMatch(
                normalizedSql,
                @"\bTOP\s*\(\s*(?:[1-9]|[1-4][0-9]|50)\s*\)",
                RegexOptions.IgnoreCase))
        {
            return SqlValidationResult.Failure(
                "Sorguda TOP (1-50) sınırı bulunmalıdır.");
        }
        
        if (!Regex.IsMatch(
                normalizedSql,
                @"\bp\.IsActive\s*=\s*1\b",
                RegexOptions.IgnoreCase))
        {
            return SqlValidationResult.Failure(
                "Sorguda aktif ürün filtresi bulunmalıdır.");
        }
        return SqlValidationResult.Success();
    }
}