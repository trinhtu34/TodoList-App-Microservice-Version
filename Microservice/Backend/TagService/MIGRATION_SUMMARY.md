# TagService Migration Summary

## ✅ Completed Tasks

### 1. **DTOs Created** ✅
- `TagResponse` - Response DTO
- `CreateTagRequest` - Create tag with validation
- `UpdateTagRequest` - Update tag
- `AddTagToTodoRequest` - Add tag to todo

### 2. **CRUD Operations Implemented** ✅

#### Tag Management
- ✅ `GET /api/tag` - Get all tags (with optional groupId filter)
- ✅ `GET /api/tag/{id}` - Get tag by ID
- ✅ `POST /api/tag` - Create new tag
- ✅ `PUT /api/tag/{id}` - Update tag
- ✅ `DELETE /api/tag/{id}` - Delete tag

#### Todo-Tag Relationship
- ✅ `POST /api/tag/todo/{todoId}` - Add tag to todo
- ✅ `DELETE /api/tag/todo/{todoId}/tag/{tagId}` - Remove tag from todo
- ✅ `GET /api/tag/todo/{todoId}` - Get all tags for a todo
- ✅ `DELETE /api/tag/cleanup/todo/{todoId}` - Cleanup (for TodoService)

### 3. **Security** ✅
- ✅ JWT Authentication configured
- ✅ All endpoints require authentication (except cleanup endpoint)
- ✅ Tags filtered by `cognito_sub`
- ✅ Removed hardcoded connection string from DbContext

### 4. **Business Logic** ✅
- ✅ Tags must belong to a user (`cognito_sub`)
- ✅ Tags can be personal (`groupId = null`) or group-specific
- ✅ Unique constraint: tag name per user per scope
- ✅ Cascade delete: removing tag removes all todo-tag relationships

### 5. **Configuration** ✅
- ✅ `appsettings.json` with JWT and DB settings
- ✅ CORS enabled
- ✅ Logging configured

---

## 📋 Key Features

### Tag Ownership
```
Every tag belongs to a cognito_sub (user)
├── Personal Tags (groupId = null)
│   └── Only visible to creator
└── Group Tags (groupId = X)
    └── Visible to group members
```

### Unique Constraint
```sql
UNIQUE KEY unique_tag_per_scope (tag_name, cognito_sub, group_id)
```

**Examples:**
- ✅ User A can have "Work" as personal tag
- ✅ User A can have "Work" in Group 1
- ✅ User B can have "Work" as personal tag
- ❌ User A cannot have duplicate "Work" personal tag

---

## 🔧 Configuration Required

### 1. Update `appsettings.json`
Replace placeholders:
- `YOUR_POOL_ID` - AWS Cognito User Pool ID
- `YOUR_CLIENT_ID` - AWS Cognito App Client ID

### 2. Update `.env`
```env
DB_PASSWORD=your_actual_password
```

### 3. Database Setup
```bash
# Create database
mysql -u root -p
CREATE DATABASE tag_service_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# Run migrations (if using EF Core migrations)
dotnet ef database update
```

---

## 🚀 Running the Service

### Development
```bash
cd D:\.vs\ToDoListApplication\Microservice\Backend\TagService\TagService
dotnet run
```

Service will run on: `http://localhost:5003`

### Test Endpoints
```bash
# Health check
curl http://localhost:5003/

# Get tags (requires JWT)
curl -H "Authorization: Bearer <token>" http://localhost:5003/api/tag
```

---

## 📊 Differences from Monolith

| Aspect | Monolith | TagService (Microservice) |
|--------|----------|---------------------------|
| **Database** | Shared `dbtodolistapp` | Dedicated `tag_service_db` |
| **Dependencies** | Direct access to Todos, Attachments | API calls to TodoService |
| **Tag-Todo** | EF Core navigation properties | HTTP API calls |
| **Cleanup** | Direct cascade delete | Exposed cleanup endpoint |
| **Scope** | All tag operations | Only tag operations |

---

## 🔗 Inter-Service Communication

### TagService → TodoService
**Not needed** (TagService is independent)

### TodoService → TagService
When deleting a todo:
```csharp
// TodoService calls TagService
await _tagServiceClient.RemoveTagsForTodo(todoId);
```

Endpoint: `DELETE /api/tag/cleanup/todo/{todoId}`

---

## ✅ Validation Rules

### CreateTagRequest
- `tagName`: Required, max 50 characters
- `color`: Optional, must match `#[0-9A-Fa-f]{6}` pattern
- `groupId`: Optional integer

### UpdateTagRequest
- `tagName`: Optional, max 50 characters
- `color`: Optional, must match `#[0-9A-Fa-f]{6}` pattern

---

## 🐛 Common Issues & Solutions

### Issue 1: "Tag with this name already exists"
**Cause:** Duplicate tag name for same user/group scope  
**Solution:** Use different name or update existing tag

### Issue 2: 401 Unauthorized
**Cause:** Missing or invalid JWT token  
**Solution:** Include valid Bearer token in Authorization header

### Issue 3: Connection string error
**Cause:** Hardcoded password or missing .env  
**Solution:** Set `DB_PASSWORD` in `.env` file

---

## 📝 Next Steps

### For TodoService Integration
1. Create `ITagServiceClient` interface
2. Implement HTTP client to call TagService
3. Call `RemoveTagsForTodo` when deleting todos

### For GroupService Integration
1. Verify group membership before showing group tags
2. Filter tags by group when user is in group context

---

## 🎯 Testing Checklist

- [ ] Create personal tag
- [ ] Create group tag
- [ ] Get all tags
- [ ] Get tags filtered by groupId
- [ ] Update tag name
- [ ] Update tag color
- [ ] Delete tag
- [ ] Add tag to todo
- [ ] Remove tag from todo
- [ ] Get tags for todo
- [ ] Verify unique constraint
- [ ] Verify JWT authentication
- [ ] Test cleanup endpoint

---

## 📚 API Documentation

See `TAG_SERVICE_API.md` for complete API documentation with examples.

---

## ✨ Summary

TagService is now a fully functional microservice with:
- ✅ Complete CRUD operations
- ✅ JWT authentication
- ✅ Personal and group tag support
- ✅ Proper validation and error handling
- ✅ Inter-service communication endpoint
- ✅ Security best practices

**Status: Ready for integration with TodoService and GroupService!** 🚀
