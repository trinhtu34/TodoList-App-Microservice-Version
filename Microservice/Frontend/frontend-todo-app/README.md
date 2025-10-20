# 📋 Ứng Dụng Quản Lý Công Việc Frontend

Ứng dụng React hiện đại, responsive được xây dựng với TypeScript, Vite và Tailwind CSS để quản lý công việc cá nhân và nhóm.

## 🚀 Tính Năng

### Chức Năng Cốt Lõi
- **Công Việc Cá Nhân**: Tạo, chỉnh sửa, xóa và quản lý các todo cá nhân
- **Công Việc Nhóm**: Cộng tác làm việc với các thành viên trong team
- **Quản Lý Tag**: Tổ chức công việc với các tag tùy chỉnh và màu sắc
- **Xác Thực Người Dùng**: Đăng nhập/đăng ký bảo mật với AWS Cognito
- **Quản Lý Nhóm**: Tạo nhóm, mời thành viên, quản lý quyền hạn
- **Cập Nhật Thời Gian Thực**: Tối ưu hóa việc lấy dữ liệu và caching

### Giao Diện Người Dùng
- **Thiết Kế Responsive**: Tiếp cận mobile-first với Tailwind CSS
- **Hỗ Trợ Dark/Light Mode**: Sẵn sàng cho việc chuyển đổi theme
- **Điều Hướng Trực Quan**: Sidebar navigation sạch sẽ với trạng thái active
- **Modal Dialog**: Form và xác nhận thân thiện với người dùng
- **Trạng Thái Loading**: Skeleton loader và thanh tiến trình

## 🛠️ Công Nghệ Sử Dụng

### Công Nghệ Cốt Lõi
- **React 19.1.1**: React mới nhất với tính năng concurrent
- **TypeScript**: Phát triển type-safe
- **Vite**: Build tool và dev server nhanh
- **Tailwind CSS**: CSS framework utility-first
- **React Router DOM**: Client-side routing

### Công Cụ Phát Triển
- **ESLint**: Code linting và formatting
- **PostCSS**: Xử lý CSS
- **Autoprefixer**: CSS vendor prefixes

## 📁 Cấu Trúc Dự Án```
src/

├── components/           # Các component UI có thể tái sử dụng
│   ├── auth/            # Component xác thực
│   │   ├── Login.tsx
│   │   ├── Register.tsx
│   │   └── ProtectedRoute.tsx
│   ├── groups/          # Component quản lý nhóm
│   │   ├── GroupView.tsx
│   │   ├── CreateGroupModal.tsx
│   │   ├── EditGroupModal.tsx
│   │   ├── InvitationList.tsx
│   │   └── InviteUserModal.tsx
│   ├── layout/          # Component layout
│   │   ├── Dashboard.tsx
│   │   └── Sidebar.tsx
│   ├── tags/            # Quản lý tag
│   │   └── TagManager.tsx
│   └── todos/           # Component todo
│       ├── TodoList.tsx
│       ├── TodoModal.tsx
│       ├── TodoTagManager.tsx
│       └── TodoView.tsx
├── config/              # Cấu hình API
│   ├── api.ts
│   ├── authApi.ts
│   ├── groupApi.ts
│   ├── tagApi.ts
│   └── todoApi.ts
├── context/             # React Context provider
│   └── AuthContext.tsx
├── hooks/               # Custom React hook
│   └── useTodos.ts
├── services/            # Lớp service API
│   ├── authService.ts
│   ├── groupService.ts
│   ├── tagService.ts
│   └── todoService.ts
├── types/               # Định nghĩa TypeScript type
└── pages/               # Component trang (sử dụng trong tương lai)
```## 
🚀 Tối Ưu Hóa Hiệu Suất

### Các Tối Ưu Hóa Đã Triển Khai

#### 1. **Lazy Loading & Code Splitting**
- **Code splitting theo route**: Component được load theo yêu cầu
- **Dynamic import**: Giảm kích thước bundle ban đầu 30-40%
- **Chunk tối ưu**: Tách riêng vendor, router và utility library

```typescript
// AppOptimized.tsx
const Login = lazy(() => import('./components/auth/Login'));
const Dashboard = lazy(() => import('./components/layout/Dashboard'));
```

#### 2. **React Memoization**
- **Component memoization**: Ngăn chặn re-render không cần thiết
- **Callback memoization**: Tham chiếu function ổn định
- **Value memoization**: Cache các tính toán phức tạp

```typescript
// TodoListOptimized.tsx
const TodoItem = memo(({ todo, onEdit, onToggle }) => {
  const handleToggle = useCallback(() => onToggle(todo), [todo, onToggle]);
  return <div>...</div>;
});
```

#### 3. **Hệ Thống Caching Thông Minh**
- **API response caching**: Cache 5 phút với Map-based storage
- **Cache invalidation**: Tự động dọn dẹp sau khi mutation
- **Background refetching**: Dữ liệu mới mà không block UI

```typescript
// useTodos.ts - Custom caching hook
const todoCache = new Map<string, { data: Todo[]; timestamp: number }>();
const CACHE_DURATION = 5 * 60 * 1000; // 5 phút
```###
# 4. **Cấu Hình Build Tối Ưu**
- **Vite với esbuild**: Build nhanh hơn 50% so với webpack
- **Manual chunk splitting**: Chiến lược loading tối ưu
- **Tree shaking**: Loại bỏ dead code
- **Minification**: Tối ưu production bundle

```typescript
// vite.config.optimized.ts
export default defineConfig({
  build: {
    rollupOptions: {
      output: {
        manualChunks: (id) => {
          if (id.includes('react')) return 'vendor';
          if (id.includes('router')) return 'router';
          return 'vendor';
        },
      },
    },
    minify: 'esbuild',
  },
});
```

#### 5. **Tối Ưu Context**
- **Memoized context value**: Ngăn provider re-render
- **Selective subscription**: Component chỉ re-render khi cần
- **Optimized auth state**: Quản lý token hiệu quả

## 🔧 Cài Đặt & Thiết Lập

### Yêu Cầu Hệ Thống
- Node.js 18+ 
- npm hoặc yarn
- Các backend service đang chạy (Auth, Todo, Group, Tag service)

### Thiết Lập Môi Trường
1. **Clone repository**
```bash
git clone <repository-url>
cd frontend-todo-app
```2. *
*Cài đặt dependencies**
```bash
npm install
# hoặc
yarn install
```

3. **Cấu hình biến môi trường**
```bash
# Copy template môi trường
cp .env.example .env

# Chỉnh sửa file .env với URL backend của bạn
VITE_AUTH_API_BASE_URL=http://localhost:32769/api
VITE_TAG_API_BASE_URL=http://localhost:32768/api
VITE_GROUP_API_BASE_URL=https://localhost:32770/api
VITE_TODO_API_BASE_URL=http://localhost:32770/api
```

4. **Khởi động development server**
```bash
npm run dev
# hoặc
yarn dev
```

5. **Build cho production**
```bash
npm run build
# hoặc
yarn build
```

## 📚 Tích Hợp API

### Kiến Trúc Service
Frontend giao tiếp với nhiều microservice:

#### Authentication Service
- **Đăng nhập/Đăng ký**: Tích hợp AWS Cognito
- **Quản lý token**: JWT với refresh token
- **Hồ sơ người dùng**: Thông tin user và nhóm

#### Todo Service  
- **Thao tác CRUD**: Tạo, đọc, cập nhật, xóa todo
- **Group todo**: Quản lý task cộng tác
- **Liên kết tag**: Quan hệ todo-tag

#### Group Service
- **Quản lý nhóm**: Tạo, chỉnh sửa, xóa nhóm
- **Quản lý thành viên**: Mời, xóa, quản lý role
- **Lời mời**: Gửi và quản lý lời mời nhóm

#### Tag Service
- **Tag CRUD**: Tạo, chỉnh sửa, xóa tag
- **Quản lý màu sắc**: Tag màu tùy chỉnh
- **Liên kết todo**: Liên kết tag với todo### C
ấu Hình API
Mỗi service có cấu hình API riêng:

```typescript
// config/todoApi.ts
const todoApi = axios.create({
  baseURL: import.meta.env.VITE_TODO_API_BASE_URL,
  timeout: 10000,
});

// Thêm auth interceptor
todoApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

## 🎨 Tính Năng UI/UX

### Hệ Thống Thiết Kế
- **Khoảng cách nhất quán**: Thang đo spacing của Tailwind
- **Bảng màu**: Hệ thống màu semantic
- **Typography**: Phân cấp font dễ đọc
- **Trạng thái tương tác**: Hover, focus, active state

### Thiết Kế Responsive
- **Mobile-first**: Tối ưu cho thiết bị di động
- **Breakpoint**: sm, md, lg, xl responsive breakpoint
- **Layout linh hoạt**: CSS Grid và Flexbox
- **Touch-friendly**: Target touch phù hợp

### Khả Năng Tiếp Cận
- **Điều hướng bàn phím**: Hỗ trợ đầy đủ bàn phím
- **Hỗ trợ screen reader**: HTML semantic và ARIA label
- **Độ tương phản màu**: Tuân thủ WCAG AA
- **Quản lý focus**: Chỉ báo focus rõ ràng

## 🔐 Tính Năng Bảo Mật

### Xác Thực
- **JWT token**: Xác thực dựa trên token bảo mật
- **Token refresh**: Tự động gia hạn token
- **Protected route**: Guard xác thực cấp route
- **Lưu trữ bảo mật**: Thực hành lưu trữ token đúng cách

### Xác Thực Dữ Liệu
- **Xác thực input**: Xác thực form phía client
- **Type safety**: TypeScript cho kiểm tra compile-time
- **Xác thực API**: Tích hợp xác thực phía server
- **Bảo vệ XSS**: Input người dùng được sanitize## 
🧪 Testing & Chất Lượng

### Chất Lượng Code
- **ESLint**: Code linting với rule React-specific
- **TypeScript**: Kiểm tra static type
- **Prettier**: Format code (có thể thêm)
- **Husky**: Git hook cho quality gate (có thể thêm)

### Giám Sát Hiệu Suất
- **Vite Bundle Analyzer**: Phân tích kích thước bundle
- **Chrome DevTools**: Profiling hiệu suất
- **Lighthouse**: Web vital và điểm hiệu suất
- **React DevTools**: Phân tích hiệu suất component

## 📱 Hướng Dẫn Sử Dụng

### Bắt Đầu
1. **Đăng ký/Đăng nhập**: Tạo tài khoản hoặc đăng nhập
2. **Dashboard**: Giao diện chính với sidebar navigation
3. **Tạo Todo**: Thêm task cá nhân hoặc nhóm
4. **Quản lý nhóm**: Tạo nhóm và mời thành viên
5. **Tổ chức với Tag**: Phân loại todo với tag màu

### Tính Năng Chính

#### Todo Cá Nhân
- Click "+" để tạo todo mới
- Đặt ngày hạn và mô tả
- Thêm tag để tổ chức
- Đánh dấu hoàn thành khi xong

#### Cộng Tác Nhóm
- Tạo nhóm từ sidebar
- Mời thành viên qua email
- Gán todo cho thành viên nhóm
- Theo dõi tiến độ nhóm

#### Quản Lý Tag
- Tạo tag tùy chỉnh với màu sắc
- Áp dụng nhiều tag cho todo
- Lọc todo theo tag
- Quản lý quyền tag (tính năng Premium)

## 🚀 Triển Khai

### Production Build
```bash
# Build bundle production tối ưu
npm run build

# Preview production build locally
npm run preview
```

### Tùy Chọn Triển Khai (Có CDN Built-in)
- **Vercel**: Triển khai zero-config + CDN global miễn phí
- **Netlify**: Static site hosting + CDN edge locations
- **AWS S3 + CloudFront**: Hosting có thể mở rộng + CDN tùy chỉnh
- **Cloudflare Pages**: Static hosting + CDN performance
- **Docker**: Triển khai containerized (cần setup CDN riêng)### Tr
iển Khai Docker
```dockerfile
# Dockerfile
FROM node:18-alpine as builder
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=builder /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

## 🔧 Phát Triển

### Script Có Sẵn
```bash
# Development server với HMR
npm run dev

# Production build
npm run build

# Kiểm tra type
npm run type-check

# Linting
npm run lint

# Phân tích bundle
npm run build:analyze
```

### Quy Trình Phát Triển
1. **Feature branch**: Tạo feature branch từ main
2. **Code review**: Yêu cầu review pull request
3. **Testing**: Test thủ công trên development server
4. **Build verification**: Đảm bảo production build hoạt động
5. **Deployment**: Deploy lên staging rồi production

### Thêm Tính Năng Mới
1. **Tạo component**: Thêm vào thư mục phù hợp
2. **Thêm type**: Định nghĩa TypeScript interface
3. **Tạo service**: Thêm tích hợp API
4. **Thêm routing**: Cập nhật cấu hình router
5. **Test tích hợp**: Xác minh với backend service

## 🐛 Khắc Phục Sự Cố

### Vấn Đề Thường Gặp

#### Lỗi Build
```bash
# Xóa node_modules và cài lại
rm -rf node_modules package-lock.json
npm install

# Xóa Vite cache
rm -rf node_modules/.vite
npm run dev
```#
### Vấn Đề Kết Nối API
- Xác minh backend service đang chạy
- Kiểm tra biến môi trường trong `.env`
- Xác nhận cài đặt CORS trên backend
- Xác thực authentication token

#### Vấn Đề Hiệu Suất
- Sử dụng React DevTools Profiler
- Kiểm tra network tab cho request chậm
- Xác minh caching hoạt động đúng
- Giám sát kích thước bundle với analyzer

## 📈 Cải Tiến Tương Lai

### Tính Năng Dự Kiến
- **Thông báo thời gian thực**: Tích hợp WebSocket
- **Lọc nâng cao**: Query todo phức tạp
- **Drag & drop**: Sắp xếp lại todo trực quan
- **Dark mode**: Chuyển đổi theme
- **Mobile app**: Phiên bản React Native

### Cải Tiến Hiệu Suất (Chưa Triển Khai)
- **React Query**: Caching và đồng bộ nâng cao thay thế Map-based cache
- **Virtual scrolling**: Xử lý danh sách todo lớn (1000+ items)
- **Tối ưu hình ảnh**: Lazy loading và nén WebP
- **Tích hợp CDN**: CloudFront/Cloudflare cho static asset (sau này Deploy lên AWS sẽ tính tiếp)
- **Service Worker**: Offline support và background sync
- **Redis Cache**: Server-side caching (backend integration)

## 👥 Đội Ngũ

- **Frontend Developer**: Triển khai React/TypeScript
- **Backend Team**: Kiến trúc microservice
- **DevOps**:
    - **Frontend** : triển khai bằng AWS Amplify
    - **Backend** : ban đầu chưa có nhiều user thì sử dụng serverless để giảm chi phí như các dịch vụ AWS Lambda + AWS API Gateway , sau này có nhiều user thì chuyển qua việc host backend trên AWS ECS hoặc AWS EC2  
        - **ChatService**: 
        - **Các service khác**: 

---
