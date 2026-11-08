using Dapper;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class CategoryRepository
{
    sealed class DuplicateHit
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public static List<CategoryRow> Search(string? keyword)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";
        using var conn = Db.Open();
        return conn.Query<CategoryRow>(
            @"SELECT c.CategoryId, c.CategoryCode, c.CategoryName, ISNULL(c.Description, N'') AS Description,
                     (SELECT COUNT(*) FROM Devices d WHERE d.CategoryId = c.CategoryId AND d.IsActive = 1) AS DeviceCount
              FROM DeviceCategories c
              WHERE c.IsActive = 1
                AND (@like IS NULL OR c.CategoryCode LIKE @like OR c.CategoryName LIKE @like)
              ORDER BY c.CategoryName", new { like }).ToList();
    }

    /// <summary>Kiểm tra trùng mã hoặc tên (không phân biệt hoa thường) trong các loại đang dùng.</summary>
    public static string? FindDuplicate(int categoryId, string code, string name)
    {
        using var conn = Db.Open();
        var hit = conn.QueryFirstOrDefault<DuplicateHit>(
            @"SELECT TOP 1 CategoryCode AS Code, CategoryName AS Name FROM DeviceCategories
              WHERE CategoryId <> @categoryId AND IsActive = 1
                AND (CategoryCode = @code OR CategoryName = @name)",
            new { categoryId, code, name });
        if (hit is null) return null;
        return string.Equals(hit.Code, code, StringComparison.OrdinalIgnoreCase)
            ? "Mã loại thiết bị đã tồn tại."
            : "Tên loại thiết bị đã tồn tại.";
    }

    public static void Insert(CategoryRow c)
    {
        using var conn = Db.Open();
        // Nếu mã từng bị xóa mềm (IsActive = 0) thì dùng lại bản ghi cũ để không vi phạm UNIQUE
        int revived = conn.Execute(
            @"UPDATE DeviceCategories SET CategoryName = @CategoryName, Description = @Description, IsActive = 1
              WHERE CategoryCode = @CategoryCode AND IsActive = 0", c);
        if (revived > 0) return;
        conn.Execute(
            "INSERT INTO DeviceCategories (CategoryCode, CategoryName, Description) VALUES (@CategoryCode, @CategoryName, @Description)", c);
    }

    public static bool Update(CategoryRow c)
    {
        using var conn = Db.Open();
        return conn.Execute(
            @"UPDATE DeviceCategories SET CategoryCode = @CategoryCode, CategoryName = @CategoryName, Description = @Description
              WHERE CategoryId = @CategoryId", c) > 0;
    }

    /// <summary>Xóa loại chưa có thiết bị nào (kể cả thiết bị đã ngừng dùng). Trả về false nếu đang được sử dụng.</summary>
    public static bool DeleteIfUnused(int categoryId) =>
        Db.InTransaction((conn, tx) =>
        {
            int used = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Devices WITH (UPDLOCK, HOLDLOCK) WHERE CategoryId = @categoryId", new { categoryId }, tx);
            if (used > 0) return false;
            conn.Execute("DELETE FROM DeviceCategories WHERE CategoryId = @categoryId", new { categoryId }, tx);
            return true;
        });
}
