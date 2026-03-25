# Inventory KPI Calculation System

Tiểu luận cuối kỳ môn **Lập Trình Hệ Thống**  
Khoa Công Nghệ Thông Tin Kinh Doanh — Đại học Kinh tế TP. Hồ Chí Minh  
Mã lớp học phần: `25C1INF50915401`

## Thành viên nhóm

| Tên thành viên | MSSV |
|---|---|
| Phạm Duy Hoàng | 31231026117 |
| Phùng Chí Tâm | 31231024593 |
| Trần Thành Đạt | 31231021353 |
| Võ Nguyên Bảo | 31231021638 |

---

## Giới thiệu

Hệ thống tự động đọc, xử lý các tệp JSON từ hệ thống POS/ERP và tính toán các chỉ số KPI cho quản lý tồn kho theo thời gian thực.

---

## Công nghệ sử dụng

- **Ngôn ngữ:** C# (.NET 8)
- **Thư viện:** System.Text.Json, System.IO, System.Threading
- **Công cụ:** Visual Studio 2022, Git

---

## Kiến trúc hệ thống
```
JSON Files → FileWatcher → Queue → FileProcessor → InventoryService → KPIService → Console Output
```

### Các thành phần chính

| Service | Chức năng |
|---|---|
| `FileWatcherService` | Theo dõi thư mục, phát hiện file JSON mới |
| `FileProcessorService` | Điều phối xử lý file, quản lý hàng đợi |
| `JsonLoaderService` | Đọc và deserialize dữ liệu JSON |
| `InventoryService` | Xây dựng và cập nhật tồn kho |
| `KPIService` | Tính toán các chỉ số KPI |

---

## Các chỉ số KPI được tính toán

| KPI | Mô tả |
|---|---|
| **Total SKUs** | Tổng số sản phẩm khác nhau trong kho |
| **Stock Value** | Tổng giá trị tồn kho (số lượng chưa bán × giá vốn trung bình) |
| **Out-of-Stock** | Số sản phẩm đã hết hàng |
| **Average Daily Sales** | Số lượng bán trung bình mỗi ngày |
| **Inventory Age** | Tuổi trung bình của hàng tồn kho (ngày) |

---

## Cấu trúc thư mục
```
InventoryKPI/
├── Models/
│   ├── Invoice.cs
│   ├── InvoiceResponse.cs
│   ├── InventoryState.cs
│   ├── KPIResult.cs
│   ├── LineItem.cs
│   └── Pagination.cs
├── Services/
│   ├── FileProcessorService.cs
│   ├── FileWatcherService.cs
│   ├── InventoryService.cs
│   ├── JsonLoaderService.cs
│   └── KPIService.cs
├── Data/
│   └── invoices/        # Đặt file JSON vào đây
├── Program.cs
└── InventoryKPI.csproj
```

---

## Hướng dẫn chạy

**1. Clone repository**
```bash
git clone https://github.com/Sunphuynx/InventoryKPI.git
cd InventoryKPI
```

**2. Tạo thư mục dữ liệu** (nếu chưa có)
```bash
mkdir -p Data/invoices
```

**3. Chạy chương trình**
```bash
dotnet run
```

**4. Thêm file JSON vào thư mục `Data/invoices/`**  
Hệ thống sẽ tự động phát hiện và xử lý, kết quả KPI hiển thị ngay trên console.

---

## Định dạng dữ liệu đầu vào

File JSON cần có cấu trúc:
```json
{
  "Invoices": [
    {
      "InvoiceID": "...",
      "Type": "ACCREC",
      "DateString": "2024-01-15",
      "LineItems": [
        {
          "AccountCode": "SKU001",
          "Description": "Tên sản phẩm",
          "Quantity": 10,
          "UnitAmount": 50000,
          "LineAmount": 500000
        }
      ]
    }
  ]
}
```

> `Type = "ACCREC"` → Hóa đơn bán hàng  
> `Type = "ACCPAY"` → Đơn nhập hàng

---

## Giảng viên hướng dẫn

Thầy **Trần Hồng Thái**
