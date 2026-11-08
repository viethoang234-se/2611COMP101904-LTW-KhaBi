using System.Data;
using Dapper;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class OverdueRepository
{
    /// <summary>F14 - gọi stored procedure sp_GetOverdueSlips.</summary>
    public static List<OverdueRow> Get(string? keyword, int? minDaysOverdue)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        using var conn = Db.Open();
        return conn.Query<OverdueRow>(
            "dbo.sp_GetOverdueSlips",
            new { Keyword = kw, MinDaysOverdue = minDaysOverdue },
            commandType: CommandType.StoredProcedure).ToList();
    }
}
