# 📋 Ứng Dụng Quản Lý Công Việc - Kiến Trúc Microservice

Ứng dụng full-stack quản lý công việc được xây dựng với kiến trúc microservice nhằm mục đích học tập.

## 🎯 Tổng Quan Dự Án

**Mục tiêu**: Học kiến trúc microservice và tích hợp AWS service thông qua việc xây dựng ứng dụng todo thực tế.

**Tầm nhìn**: Phát triển thành nền tảng cộng tác giống Slack với tính năng chat và quản lý team.

## 🚀 Tính Năng Hiện Tại

- **Xác thực**: Đăng ký và đăng nhập với AWS Cognito
- **Todo cá nhân**: Tạo, chỉnh sửa và quản lý task với tag
- **Quản lý nhóm**: Tạo nhóm và mời thành viên qua email
- **Todo nhóm**: Quản lý task cộng tác trong nhóm
- **Hệ thống tag**: Premium user có thể tạo tag tùy chỉnh
- **Phân quyền**: Quyền Owner/Admin/Member

## 🏗️ Kiến Trúc

### Frontend
- **React + TypeScript**: UI hiện đại với tối ưu hiệu suất
- **Tailwind CSS**: Thiết kế responsive
- **Vite**: Development và build nhanh

### Backend
- **4 Microservice**: Auth, Todo, Group, Tag service
- **.NET Core**: Clean Architecture + CQRS pattern
- **MySQL**: Database riêng cho mỗi service
- **Docker**: Triển khai container

### Hạ tầng
- **AWS Cognito**: Xác thực người dùng
- **Docker Compose**: Development local

## 🚧 Lộ Trình Phát Triển

### Giai đoạn 1 (Đang thực hiện)
- ✅ Quản lý todo và nhóm cơ bản
- ✅ Xác thực và phân quyền người dùng
- ✅ Quản lý tag
- 🔄 Làm hệ thống up file kèm theo todo
- 🔄 Tính năng Chat trong nhóm và chat 1-1 dùng : SignalR + ScyllaDB + Redis

### Giai đoạn 2 (Dự kiến)
- 📧 **Hệ thống thông báo**: AWS Lambda + SES cho cảnh báo deadline
- 📎 **File đính kèm**: Premium user có nhiều storage hơn
- 🔍 **Tìm kiếm nâng cao**: Lọc và sắp xếp todo
- **Tối ưu truy xuất**: Triển khai caching layer cho các tính năng bằng Redis

### Giai đoạn 3 (Tương lai)
- 📱 **Mobile app**: Phiên bản React Native
- 📊 **Dashboard phân tích**: Thống kê sử dụng

## 🛠️ Công Nghệ Sử Dụng

| Thành phần | Công nghệ |
|------------|-----------|
| Frontend | React, TypeScript, Tailwind CSS |
| Backend | .NET Core, Clean Architecture, CQRS |
| Database | MySQL |
| Xác thực | AWS Cognito |
| Container | Docker |
| Giám sát | CloudWatch (dự kiến) |

## 🚀 Chạy ứng dụng ở Development Environment

### Yêu cầu
- Node.js 18+
- .NET 8 SDK
- Docker & Docker Compose
- MySQL

### Chạy Local
```bash
# Clone repository
git clone <repo-url>
cd TodoList-App-Microservice-Version

# Khởi động backend service
cd Microservice/Backend
docker-compose up -d

# Khởi động frontend
cd ../Frontend/frontend-todo-app
npm install
npm run dev
```

### Truy cập ứng dụng
- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:32770-32773

## 📁 Cấu Trúc Dự Án

```
├── Microservice/
│   ├── Backend/
│   │   ├── AuthService/          # Xác thực người dùng
│   │   ├── TodoService/          # Thao tác CRUD todo
│   │   ├── GroupService/         # Quản lý nhóm
│   │   └── TagService/           # Quản lý tag
│   └── Frontend/
│       └── frontend-todo-app/    # Ứng dụng React
├── k8s/                          # Config Kubernetes
└── NotificationService/          # Hệ thống thông báo AWS (thiết kế)
```