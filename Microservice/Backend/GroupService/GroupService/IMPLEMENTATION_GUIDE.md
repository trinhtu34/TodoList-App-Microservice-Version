# Group Service - Clean Architecture + CQRS Implementation

## Cấu trúc đã tạo

```
GroupService/
├── Application/
│   ├── Common/
│   │   ├── ICommand.cs          # Interface cho Commands
│   │   └── IQuery.cs            # Interface cho Queries
│   └── Groups/
│       ├── Commands/
│       │   ├── CreateGroupCommand.cs
│       │   ├── UpdateGroupCommand.cs
│       │   ├── DeleteGroupCommand.cs
│       │   └── ArchiveGroupCommand.cs
│       └── Queries/
│           ├── GetUserGroupsQuery.cs
│           └── GetGroupByIdQuery.cs
├── Controllers/
│   └── GroupController.cs       # API endpoints
├── DTOs/
│   └── group.cs                 # Request/Response DTOs
└── Models/
    ├── GroupsR.cs               # Entity (đã cập nhật)
    └── GroupMember.cs           # Entity (đã cập nhật)
```

## APIs đã implement

### 1. POST /api/groups
Tạo nhóm mới, tự động thêm creator làm owner

**Request:**
```json
{
  "groupName": "My Group",
  "groupAvatar": "https://...",
  "groupDescription": "Description"
}
```

### 2. GET /api/groups
Lấy danh sách nhóm của user hiện tại

### 3. GET /api/groups/{groupId}
Lấy chi tiết nhóm (chỉ member mới xem được)

### 4. PUT /api/groups/{groupId}
Cập nhật thông tin nhóm (chỉ owner/admin)

**Request:**
```json
{
  "groupName": "Updated Name",
  "groupAvatar": "https://...",
  "groupDescription": "New description"
}
```

### 5. DELETE /api/groups/{groupId}
Xóa nhóm (chỉ owner)

### 6. PATCH /api/groups/{groupId}/archive
Đánh dấu nhóm không active (chỉ owner/admin)

## Cài đặt

1. Restore packages:
```bash
dotnet restore
```

2. Cập nhật database (thêm cột GroupDescription):
```sql
ALTER TABLE groups_r ADD COLUMN group_description TEXT COMMENT 'Group description/bio';
ALTER TABLE group_members ADD COLUMN is_active BOOLEAN DEFAULT TRUE;
ALTER TABLE group_members ADD COLUMN left_at DATETIME;
```

3. Build và chạy:
```bash
dotnet build
dotnet run
```

## Nguyên tắc Clean Architecture

- **Application Layer**: Chứa business logic (Commands, Queries, Handlers)
- **Controllers**: Chỉ nhận request và gọi MediatR
- **Models**: Entity từ database
- **DTOs**: Data transfer objects

## CQRS Pattern

- **Commands**: Thay đổi state (Create, Update, Delete, Archive)
- **Queries**: Chỉ đọc data (Get, List)
- **MediatR**: Dispatch commands/queries đến handlers tương ứng

## Bước tiếp theo

1. Thêm validation (FluentValidation)
2. Thêm exception handling middleware
3. Implement member management APIs
4. Thêm unit tests
5. Thêm logging
