-- =============================================
-- Script tạo Database, Bảng và Stored Procedures cho OrderManagement
-- Bài 2 - Lab 7 - Lập Trình C#4
-- =============================================

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'OrderManagement')
BEGIN
    CREATE DATABASE OrderManagement;
END
GO

USE OrderManagement;
GO

-- Xóa các Stored Procedures cũ nếu tồn tại
IF OBJECT_ID('dbo.sp_DeleteOrder', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DeleteOrder;
IF OBJECT_ID('dbo.sp_CreateOrder', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateOrder;
IF OBJECT_ID('dbo.sp_GetOrderDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetOrderDetails;
IF OBJECT_ID('dbo.sp_GetOrders', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_GetOrders;
IF OBJECT_ID('dbo.sp_SearchOrders', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_SearchOrders;
GO

-- Xóa bảng cũ nếu tồn tại (theo thứ tự FK)
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
GO

-- =============================================
-- TẠO BẢNG ORDERS
-- =============================================
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    CustomerName NVARCHAR(100) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL
);
GO

-- =============================================
-- TẠO BẢNG ORDERDETAILS
-- =============================================
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) 
        REFERENCES Orders(OrderId) ON DELETE CASCADE
);
GO

-- =============================================
-- STORED PROCEDURE 1: sp_GetOrders
-- Lấy danh sách tất cả đơn hàng
-- =============================================
CREATE PROCEDURE sp_GetOrders
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        OrderId,
        OrderDate,
        CustomerName,
        TotalAmount
    FROM Orders
    ORDER BY OrderDate DESC;
END
GO

-- =============================================
-- STORED PROCEDURE 2: sp_GetOrderDetails
-- Lấy chi tiết của một đơn hàng theo OrderId
-- =============================================
CREATE PROCEDURE sp_GetOrderDetails
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Lấy thông tin Order
    SELECT 
        OrderId,
        OrderDate,
        CustomerName,
        TotalAmount
    FROM Orders
    WHERE OrderId = @OrderId;
    
    -- Lấy danh sách chi tiết
    SELECT 
        OrderDetailId,
        OrderId,
        ProductId,
        ProductName,
        Quantity,
        UnitPrice
    FROM OrderDetails
    WHERE OrderId = @OrderId;
END
GO

-- =============================================
-- STORED PROCEDURE 3: sp_CreateOrder
-- Tạo mới đơn hàng và chi tiết
-- Nhận JSON string cho danh sách items
-- =============================================
CREATE PROCEDURE sp_CreateOrder
    @CustomerName NVARCHAR(100),
    @OrderDate DATETIME,
    @TotalAmount DECIMAL(18,2),
    @ProductId INT,
    @ProductName NVARCHAR(100),
    @Quantity INT,
    @UnitPrice DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Tạo Order mới
        INSERT INTO Orders (OrderDate, CustomerName, TotalAmount)
        VALUES (@OrderDate, @CustomerName, @TotalAmount);
        
        -- Lấy OrderId vừa tạo
        DECLARE @NewOrderId INT = SCOPE_IDENTITY();
        
        -- Tạo OrderDetail
        INSERT INTO OrderDetails (OrderId, ProductId, ProductName, Quantity, UnitPrice)
        VALUES (@NewOrderId, @ProductId, @ProductName, @Quantity, @UnitPrice);
        
        COMMIT TRANSACTION;
        
        -- Trả về OrderId mới tạo
        SELECT @NewOrderId AS NewOrderId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- STORED PROCEDURE 4: sp_DeleteOrder
-- Xóa đơn hàng và tất cả chi tiết (CASCADE)
-- =============================================
CREATE PROCEDURE sp_DeleteOrder
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Kiểm tra đơn hàng tồn tại
    IF NOT EXISTS (SELECT 1 FROM Orders WHERE OrderId = @OrderId)
    BEGIN
        RAISERROR('Không tìm thấy đơn hàng với ID: %d', 16, 1, @OrderId);
        RETURN;
    END
    
    -- Xóa đơn hàng (OrderDetails sẽ bị xóa do CASCADE)
    DELETE FROM Orders WHERE OrderId = @OrderId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- STORED PROCEDURE 5: sp_SearchOrders
-- Tìm kiếm đơn hàng theo tên khách hàng
-- =============================================
CREATE PROCEDURE sp_SearchOrders
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        OrderId,
        OrderDate,
        CustomerName,
        TotalAmount
    FROM Orders
    WHERE CustomerName LIKE '%' + @SearchTerm + '%'
    ORDER BY OrderDate DESC;
END
GO

-- =============================================
-- THÊM DỮ LIỆU MẪU
-- =============================================
INSERT INTO Orders (OrderDate, CustomerName, TotalAmount) VALUES
(GETDATE(), N'Nguyễn Văn A', 1500000),
(DATEADD(DAY, -1, GETDATE()), N'Trần Thị B', 2350000),
(DATEADD(DAY, -2, GETDATE()), N'Lê Văn C', 890000);

INSERT INTO OrderDetails (OrderId, ProductId, ProductName, Quantity, UnitPrice) VALUES
(1, 101, N'Laptop Dell XPS 13', 1, 1500000),
(2, 102, N'iPhone 15 Pro', 1, 2000000),
(2, 103, N'Ốp lưng iPhone', 1, 350000),
(3, 104, N'Tai nghe Bluetooth', 2, 445000);
GO

-- Hiển thị dữ liệu
SELECT 'Orders' AS TableName;
SELECT * FROM Orders;

SELECT 'OrderDetails' AS TableName;
SELECT * FROM OrderDetails;
GO

PRINT N'Tạo database, bảng và Stored Procedures thành công!';
GO
