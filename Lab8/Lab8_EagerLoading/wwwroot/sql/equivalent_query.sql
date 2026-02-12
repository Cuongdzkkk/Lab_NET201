-- ============================================
-- BÀI 1: CÂU SQL THUẦN TƯƠNG ĐƯƠNG VỚI EAGER LOADING
-- ============================================

-- Câu SQL này tương đương với Eager Loading trong EF Core:
-- _context.Customers
--     .Include(c => c.Orders)
--         .ThenInclude(o => o.Dishes)
--     .ToList()

-- ============================================
-- QUERY 1: Lấy tất cả Customers với Orders và Dishes
-- ============================================
SELECT 
    c.CustomerId,
    c.Name AS CustomerName,
    c.Phone AS CustomerPhone,
    o.OrderId,
    o.OrderDate,
    o.TotalAmount,
    o.Status AS OrderStatus,
    d.DishId,
    d.Name AS DishName,
    d.Price AS DishPrice,
    d.Quantity AS DishQuantity
FROM Customers c
LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
LEFT JOIN Dishes d ON o.OrderId = d.OrderId
ORDER BY c.CustomerId, o.OrderId, d.DishId;

-- ============================================
-- QUERY 2: Thống kê đơn hàng theo khách hàng
-- ============================================
SELECT 
    c.CustomerId,
    c.Name AS CustomerName,
    COUNT(DISTINCT o.OrderId) AS TotalOrders,
    COUNT(d.DishId) AS TotalDishes,
    COALESCE(SUM(o.TotalAmount), 0) AS TotalSpent
FROM Customers c
LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
LEFT JOIN Dishes d ON o.OrderId = d.OrderId
GROUP BY c.CustomerId, c.Name
ORDER BY TotalSpent DESC;

-- ============================================
-- QUERY 3: Chi tiết 1 khách hàng cụ thể (VD: CustomerId = 1)
-- ============================================
SELECT 
    c.Name AS CustomerName,
    o.OrderId,
    o.OrderDate,
    STRING_AGG(d.Name, ', ') AS Dishes,
    o.TotalAmount
FROM Customers c
INNER JOIN Orders o ON c.CustomerId = o.CustomerId
LEFT JOIN Dishes d ON o.OrderId = d.OrderId
WHERE c.CustomerId = 1
GROUP BY c.Name, o.OrderId, o.OrderDate, o.TotalAmount
ORDER BY o.OrderDate DESC;

-- ============================================
-- SO SÁNH HIỆU NĂNG
-- ============================================
/*
EAGER LOADING (EF Core):
- Ưu điểm:
  + Tự động map vào C# objects
  + Type-safe, compile-time checking
  + Dễ bảo trì khi thay đổi schema
  + Tự động xử lý NULL và relationships

- Nhược điểm:
  + SQL có thể không tối ưu nhất
  + Khó customize complex queries

RAW SQL:
- Ưu điểm:
  + Có thể viết SQL tối ưu nhất
  + Sử dụng được các tính năng SQL Server specific
  + Linh hoạt với stored procedures

- Nhược điểm:
  + Phải tự map dữ liệu vào objects
  + Không type-safe
  + Khó bảo trì khi đổi schema
  + Dễ lỗi SQL injection nếu không cẩn thận

KẾT LUẬN:
Với hầu hết các trường hợp, Eager Loading của EF Core là đủ tốt
và dễ bảo trì hơn. Chỉ dùng Raw SQL khi cần tối ưu performance
cho các query phức tạp hoặc cần sử dụng các tính năng SQL đặc biệt.
*/
