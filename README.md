# RobertEcommerce.Shop

Microservices Architecture

📌 Giới thiệu

Dự án này sử dụng kiến trúc Microservices kết hợp với Aspire và Docker để triển khai một hệ thống phân tán, linh hoạt và dễ mở rộng.

📂 Cấu trúc thư mục

src
│── Common
│── Services # Các Microservices chính
│ ├── ProductService # Quản lý sản phẩm
│ ├── OrderService # Quản lý đơn hàng
│ ├── IdentityService # Quản lý người dùng
│── Shared
│── AdminPanel # Web Admin (Blazor)
│── AspireHost # Dự án Aspire quản lý toàn bộ service

🔧 Công nghệ sử dụng

.NET 9 – Xây dựng API

Aspire – Điều phối và giám sát Microservices

Ocelot/YARP – API Gateway

Entity Framework Core – ORM quản lý dữ liệu

Docker & Kubernetes – Triển khai và scale hệ thống

Blazor – Frontend UI

Redis & RabbitMQ – Caching và Message Queue

PostgreSQL – Cơ sở dữ liệu chính

🚀 Cách khởi chạy dự án

🏗 Yêu cầu cài đặt

.NET 8

Docker & Docker Compose

Node.js (nếu chạy frontend)

PostgreSQL hoặc MongoDB

🏃‍♂️ Chạy toàn bộ hệ thống bằng Aspire

dotnet run --project AspireHost

🏃‍♂️ Chạy từng Microservice riêng lẻ

dotnet run --project src/Services/ProductService

📊 Giám sát & Logging

Health Checks: http://localhost:5000/health

Swagger API Docs: http://localhost:5000/swagger

Jaeger/Prometheus (nếu có cài đặt)

📌 Đóng góp

Fork dự án

Tạo branch mới (git checkout -b feature/ten-chuc-nang)

Commit thay đổi (git commit -m "Mô tả ngắn gọn thay đổi")

Push lên GitHub (git push origin feature/ten-chuc-nang)

Tạo Pull Request (PR) để review

📝 License

Dự án này tuân theo giấy phép MIT License.

🔥 Happy Coding! 🚀
