# GroupService Clean Architecture

Đây là phiên bản Clean Architecture của GroupService, được migrate từ phiên bản gốc.

## Cấu trúc dự án

```
GroupService_CleanArchitecture/
├── Domain/                     # Domain Layer - Entities, Enums, Repository Interfaces
│   ├── Entities/              # Domain Entities
│   ├── Enums/                 # Domain Enums
│   ├── Repositories/          # Repository Interfaces
│   └── Common/                # Base classes
├── Application/               # Application Layer - Use Cases, DTOs
│   ├── Features/              # Feature-based organization
│   │   ├── Groups/           # Group-related commands/queries
│   │   └── Members/          # Member-related commands/queries
│   ├── DTOs/                 # Data Transfer Objects
│   └── Common/               # Common interfaces (ICommand, IQuery)
├── Infrastructure.Persistence/ # Data Access Layer
│   ├── Data/                 # DbContext
│   └── Repositories/         # Repository Implementations
├── Infrastructure/            # Infrastructure Layer (External services)
└── Presentation/             # Presentation Layer - API Controllers
    └── Controllers/          # API Controllers
```

## Các tính năng đã migrate

### Groups
- ✅ Create Group
- ✅ Get User Groups
- ✅ Get Group By ID
- ✅ Update Group
- ✅ Delete Group (Soft delete)

### Members
- ✅ Get Group Members
- ✅ Leave Group
- ✅ Remove Member
- ✅ Update Member Role
- ✅ Update Member Settings (Nickname, Mute)

### Invitations
- ✅ Create Invitation
- ✅ Accept Invitation
- ✅ Decline Invitation
- ✅ Get User Invitations

### Direct Messages
- ✅ Create/Get Direct Message
- ✅ Get User Direct Messages
- ⚠️ **Note**: DirectMessageRepository có issue với DI registration, cần fix

### Users
- ✅ Get User By Email (AWS Cognito integration)

## API Endpoints

### Groups
- `POST /api/Group` - Create group
- `GET /api/Group` - Get user groups
- `GET /api/Group/{groupId}` - Get group by ID
- `PUT /api/Group/{groupId}` - Update group
- `DELETE /api/Group/{groupId}` - Delete group

### Members
- `GET /api/groups/{groupId}/Member` - Get group members
- `DELETE /api/groups/{groupId}/Member/{memberUserId}` - Remove member
- `POST /api/groups/{groupId}/Member/leave` - Leave group
- `PATCH /api/groups/{groupId}/Member/{memberUserId}/role` - Update member role
- `PATCH /api/groups/{groupId}/Member/settings` - Update member settings
- `PATCH /api/groups/{groupId}/Member/mute` - Toggle mute

### Invitations
- `POST /api/Invitation` - Create invitation
- `GET /api/Invitation` - Get user invitations
- `POST /api/Invitation/{invitationId}/accept` - Accept invitation
- `POST /api/Invitation/{invitationId}/decline` - Decline invitation

### Direct Messages
- `POST /api/DirectMessage` - Create/Get direct message
- `GET /api/DirectMessage` - Get user direct messages

### Users
- `GET /api/User/by-email/{email}` - Get user by email

## Cách chạy

1. **Cập nhật Connection String**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=groupservice;User=root;Password=yourpassword;"
     }
   }
   ```

2. **Cập nhật JWT Configuration**
   ```json
   {
     "Jwt": {
       "Key": "your-secret-key-here",
       "Issuer": "your-issuer",
       "Audience": "your-audience"
     }
   }
   ```

3. **Cập nhật AWS Cognito Configuration**
   ```json
   {
     "AWS": {
       "Region": "us-east-1",
       "AccessKey": "your-access-key",
       "SecretKey": "your-secret-key",
       "Cognito": {
         "UserPoolId": "your-user-pool-id",
         "Authority": "https://cognito-idp.us-east-1.amazonaws.com/your-user-pool-id"
       }
     }
   }
   ```

4. **Build và chạy**
   ```bash
   cd Presentation
   dotnet build
   dotnet run
   ```

## So sánh với phiên bản gốc

### Ưu điểm của Clean Architecture:
- **Separation of Concerns**: Mỗi layer có trách nhiệm riêng biệt
- **Dependency Inversion**: Domain không phụ thuộc vào Infrastructure
- **Testability**: Dễ dàng unit test từng layer
- **Maintainability**: Code dễ bảo trì và mở rộng
- **SOLID Principles**: Tuân thủ các nguyên tắc SOLID

### Cấu trúc Dependencies:
```
Presentation → Application → Domain
Infrastructure.Persistence → Application → Domain
Infrastructure → Application → Domain
```

## Migration Notes

1. **Entities**: Đã chuyển từ Models sang Domain/Entities với proper enums
2. **Repository Pattern**: Implement đầy đủ Repository pattern với interfaces
3. **CQRS**: Sử dụng MediatR để implement Command Query Responsibility Segregation
4. **DTOs**: Tách biệt DTOs khỏi Domain entities
5. **Dependency Injection**: Cấu hình DI container đúng cách

## TODO - Hoàn thiện migration

### Bugs cần fix
- [ ] Fix DirectMessageRepository DI registration issue
- [ ] Fix null reference warnings trong Program.cs

### Enhancements
- [ ] Add Validation using FluentValidation
- [ ] Add Exception Handling middleware
- [ ] Add Logging (Serilog)
- [ ] Add Health Checks
- [ ] Add API Versioning
- [ ] Add Rate Limiting
- [ ] Add Caching (Redis)

### Testing
- [ ] Add Unit Tests
- [ ] Add Integration Tests
- [ ] Add Performance Tests

### DevOps
- [ ] Copy Dockerfile và deployment configs
- [ ] Add CI/CD pipeline
- [ ] Add monitoring và alerting

### Documentation
- [ ] Add API documentation (Swagger)
- [ ] Add architecture diagrams
- [ ] Add deployment guide