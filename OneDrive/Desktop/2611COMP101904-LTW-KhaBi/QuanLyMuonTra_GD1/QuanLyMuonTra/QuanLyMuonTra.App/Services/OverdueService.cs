using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>F14 - danh sách phiếu quá hạn (stored procedure sp_GetOverdueSlips).</summary>
public static class OverdueService
{
    public static Result<List<OverdueRow>> Get(string? keyword, int? minDaysOverdue) =>
        Svc.Run(new[] { Perm.OverdueView }, () =>
        {
            if (minDaysOverdue is < 0) throw new BusinessException("Số ngày quá hạn tối thiểu không được âm.");
            return OverdueRepository.Get(keyword, minDaysOverdue);
        });
}
