/* =====================================================================
   01 - TẠO CƠ SỞ DỮ LIỆU
   Thứ tự chạy trong SSMS: 01 -> 02 -> 03 -> 04 -> 05 -> 06 -> (07 tùy chọn, dữ liệu thử)
   Các script đều chạy lại được nhiều lần, không xóa dữ liệu hiện có.
   Yêu cầu: SQL Server 2017 trở lên (dùng STRING_AGG, CREATE OR ALTER).
   ===================================================================== */
IF DB_ID(N'QuanLyMuonTra') IS NULL
    CREATE DATABASE QuanLyMuonTra;
GO
