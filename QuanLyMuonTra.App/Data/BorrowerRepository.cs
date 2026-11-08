using Dapper;
using Microsoft.Data.SqlClient;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class BorrowerRepository
{
    const string SelectSql =
        @"SELECT b.BorrowerId, b.BorrowerCode, b.FullName, ISNULL(b.Phone, N'') AS Phone, ISNULL(b.Email, N'') AS Email,
                 ISNULL(b.Department, N'') AS Department, ISNULL(b.Address, N'') AS Address, ISNULL(b.Note, N'') AS Note,
                 b.IsActive,
                 (SELECT COUNT(*) FROM LoanSlips s WHERE s.BorrowerId = b.BorrowerId) AS LoanCount
          FROM Borrowers b ";

    public static List<BorrowerRow> Search(string? keyword, bool includeInactive)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";
        using var conn = Db.Open();
        return conn.Query<BorrowerRow>(
            SelectSql +
            @"WHERE (@includeInactive = 1 OR b.IsActive = 1)
                AND (@like IS NULL OR b.BorrowerCode LIKE @like OR b.FullName LIKE @like OR b.Phone LIKE @like
                     OR b.Email LIKE @like OR b.Department LIKE @like)
              ORDER BY b.FullName", new { like, includeInactive }).ToList();
    }

    public static BorrowerRow? Get(int borrowerId)
    {
        using var conn = Db.Open();
        return conn.QuerySingleOrDefault<BorrowerRow>(SelectSql + "WHERE b.BorrowerId = @borrowerId", new { borrowerId });
    }

    /// <summary>Mã người mượn tiếp theo (chỉ để gợi ý, mã thật được cấp lúc lưu).</summary>
    public static string SuggestCode() => CodeRepository.Peek(CodeRepository.Borrower);

    /// <summary>Thêm người mượn, mã tự động cấp trong transaction. Trả về mã vừa tạo.</summary>
    public static string Insert(BorrowerRow b) =>
        Db.InTransaction((conn, tx) =>
        {
            string code = CodeRepository.Next(conn, tx, CodeRepository.Borrower);
            conn.Execute(
                @"INSERT INTO Borrowers (BorrowerCode, FullName, Phone, Email, Department, Address, Note, IsActive)
                  VALUES (@code, @FullName, @Phone, @Email, @Department, @Address, @Note, @IsActive)",
                new { code, b.FullName, b.Phone, b.Email, b.Department, b.Address, b.Note, b.IsActive }, tx);
            return code;
        });

    public static bool Update(BorrowerRow b)
    {
        using var conn = Db.Open();
        return conn.Execute(
            @"UPDATE Borrowers
              SET FullName = @FullName, Phone = @Phone, Email = @Email, Department = @Department,
                  Address = @Address, Note = @Note, IsActive = @IsActive
              WHERE BorrowerId = @BorrowerId", b) > 0;
    }

    public static bool HasHistory(int borrowerId)
    {
        using var conn = Db.Open();
        return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM LoanSlips WHERE BorrowerId = @borrowerId", new { borrowerId }) > 0;
    }

    /// <summary>Xóa vật lý chỉ khi người mượn chưa có giao dịch nào. Trả về false nếu có lịch sử.</summary>
    public static bool DeleteIfNoHistory(int borrowerId) =>
        Db.InTransaction((conn, tx) =>
        {
            int used = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM LoanSlips WITH (UPDLOCK, HOLDLOCK) WHERE BorrowerId = @borrowerId", new { borrowerId }, tx);
            if (used > 0) return false;
            conn.Execute("DELETE FROM Borrowers WHERE BorrowerId = @borrowerId", new { borrowerId }, tx);
            return true;
        });
}
