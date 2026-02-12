-- =============================================
-- Script tạo Database và Bảng cho StudentManagement
-- Bài 1 - Lab 7 - Lập Trình C#4
-- =============================================

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StudentManagement')
BEGIN
    CREATE DATABASE StudentManagement;
END
GO

USE StudentManagement;
GO

-- Xóa bảng cũ nếu tồn tại (để chạy lại script)
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL
    DROP TABLE dbo.Students;
GO

-- Tạo bảng Students
CREATE TABLE Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE NULL,
    Email VARCHAR(100) NULL
);
GO

-- Thêm dữ liệu mẫu
INSERT INTO Students (FirstName, LastName, DateOfBirth, Email) VALUES
(N'Nguyễn', N'Văn An', '2000-01-15', 'nguyenvanan@email.com'),
(N'Trần', N'Thị Bình', '2001-05-20', 'tranthibibh@email.com'),
(N'Lê', N'Văn Cường', '1999-12-10', 'levancuong@email.com'),
(N'Phạm', N'Thị Dung', '2000-07-25', 'phamthidung@email.com'),
(N'Hoàng', N'Văn Em', '2002-03-08', 'hoangvanem@email.com');
GO

-- Hiển thị dữ liệu đã thêm
SELECT * FROM Students;
GO

PRINT N'Tạo database và bảng thành công!';
GO
