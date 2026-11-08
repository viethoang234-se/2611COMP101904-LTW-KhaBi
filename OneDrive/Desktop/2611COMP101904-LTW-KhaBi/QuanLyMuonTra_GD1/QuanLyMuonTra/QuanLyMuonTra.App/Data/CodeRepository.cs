using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace QuanLyMuonTra.App.Data;

/// <summary>Mã tự động (F16). Việc sinh mã nằm trong stored procedure sp_GetNextCode nên không bao giờ trùng khi nhiều người thao tác.</summary>
public static class CodeRepository
{
    public const string LoanSlip = "LoanSlip";
    public const string ReturnRecord = "ReturnRecord";
    public const string Device = "Device";
    public const string Borrower = "Borrower";

    /// <summary>Lấy mã tiếp theo và tăng bộ đếm. Phải gọi bên trong transaction lưu dữ liệu để rollback cùng nhau.</summary>
    public static string Next(SqlConnection conn, SqlTransaction? tx, string entity) => Call(conn, tx, entity, peek: false);

    /// <summary>Xem mã tiếp theo, không tăng bộ đếm (dùng để gợi ý trên giao diện).</summary>
    public static string Peek(string entity)
    {
        using var conn = Db.Open();
        return Call(conn, null, entity, peek: true);
    }

    static string Call(SqlConnection conn, SqlTransaction? tx, string entity, bool peek)
    {
        var p = new DynamicParameters();
        p.Add("@EntityName", entity, DbType.AnsiString, size: 30);
        p.Add("@Code", dbType: DbType.AnsiString, direction: ParameterDirection.Output, size: 50);
        p.Add("@Peek", peek);
        conn.Execute("dbo.sp_GetNextCode", p, transaction: tx, commandType: CommandType.StoredProcedure);
        return p.Get<string>("@Code");
    }

    /// <summary>
    /// Khi người dùng tự nhập mã dạng TB0012 (mã không chứa ngày), kéo bộ đếm lên để các mã gợi ý sau không trùng.
    /// </summary>
    public static void SyncCounter(SqlConnection conn, SqlTransaction? tx, string entity, string code)
    {
        var prefix = conn.ExecuteScalar<string?>(
            "SELECT Prefix FROM CodeConfigs WHERE EntityName = @entity AND UseDate = 0", new { entity }, tx);
        if (prefix is null || !code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return;
        var digits = code[prefix.Length..];
        if (digits.Length == 0 || digits.Length > 9 || !digits.All(char.IsAsciiDigit)) return;

        conn.Execute(
            "UPDATE CodeConfigs SET CurrentNumber = @n WHERE EntityName = @entity AND CurrentNumber < @n",
            new { n = int.Parse(digits), entity }, tx);
    }
}
