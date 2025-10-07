# Setup Frontend Todo App

## Cài đặt dependencies

```bash
npm install
```

## Chạy development server

```bash
npm run dev
```

Frontend sẽ chạy tại: http://localhost:5173

## Cấu hình

Backend AuthService phải chạy tại: http://localhost:32770

## Tính năng đã hoàn thành

- ✅ Đăng nhập (Login)
- ✅ Đăng ký (Register) 
- ✅ Xác nhận email (Confirm Sign Up)
- ✅ Protected Routes
- ✅ Auto refresh token khi hết hạn
- ✅ Dashboard cơ bản

## Routes

- `/login` - Trang đăng nhập
- `/register` - Trang đăng ký
- `/dashboard` - Trang chính (yêu cầu đăng nhập)
- `/` - Redirect về `/login`

## Tech Stack

- React 19 + TypeScript
- Vite
- React Router v7
- Axios
- Tailwind CSS
- AWS Cognito (qua AuthService)
