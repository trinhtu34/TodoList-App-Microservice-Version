# TagService API Documentation

## Overview
TagService manages tags for todos. Tags must belong to a `cognito_sub` (user) and can optionally belong to a group.

## Base URL
```
http://localhost:5003/api/tag
```

## Authentication
All endpoints require JWT Bearer token in Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

---

## API Endpoints

### 1. Get All Tags
**GET** `/api/tag`

Get all tags for the current user (personal + group tags).

**Query Parameters:**
- `groupId` (optional): Filter tags by group

**Response:**
```json
[
  {
    "tagId": 1,
    "tagName": "Urgent",
    "color": "#FF5733",
    "groupId": null,
    "createdAt": "2024-01-15T10:30:00Z"
  },
  {
    "tagId": 2,
    "tagName": "Work",
    "color": "#3498DB",
    "groupId": 5,
    "createdAt": "2024-01-16T14:20:00Z"
  }
]
```

---

### 2. Get Tag by ID
**GET** `/api/tag/{id}`

Get a specific tag by ID (must belong to current user).

**Response:**
```json
{
  "tagId": 1,
  "tagName": "Urgent",
  "color": "#FF5733",
  "groupId": null,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

### 3. Create Tag
**POST** `/api/tag`

Create a new tag.

**Request Body:**
```json
{
  "tagName": "Important",
  "color": "#FF5733",
  "groupId": null
}
```

**Validation:**
- `tagName`: Required, max 50 characters
- `color`: Optional, must be hex format (#RRGGBB), default: #808080
- `groupId`: Optional, null = personal tag

**Response:** `201 Created`
```json
{
  "tagId": 3,
  "tagName": "Important",
  "color": "#FF5733",
  "groupId": null,
  "createdAt": "2024-01-17T09:15:00Z"
}
```

---

### 4. Update Tag
**PUT** `/api/tag/{id}`

Update an existing tag (must belong to current user).

**Request Body:**
```json
{
  "tagName": "Very Important",
  "color": "#E74C3C"
}
```

**Response:** `204 No Content`

---

### 5. Delete Tag
**DELETE** `/api/tag/{id}`

Delete a tag and all its todo associations.

**Response:** `204 No Content`

---

### 6. Add Tag to Todo
**POST** `/api/tag/todo/{todoId}`

Add a tag to a specific todo.

**Request Body:**
```json
{
  "tagId": 1
}
```

**Response:** `204 No Content`

---

### 7. Remove Tag from Todo
**DELETE** `/api/tag/todo/{todoId}/tag/{tagId}`

Remove a tag from a specific todo.

**Response:** `204 No Content`

---

### 8. Get Tags for Todo
**GET** `/api/tag/todo/{todoId}`

Get all tags associated with a specific todo.

**Response:**
```json
[
  {
    "tagId": 1,
    "tagName": "Urgent",
    "color": "#FF5733",
    "groupId": null,
    "createdAt": "2024-01-15T10:30:00Z"
  }
]
```

---

### 9. Cleanup Tags for Todo (Internal)
**DELETE** `/api/tag/cleanup/todo/{todoId}`

Remove all tags for a todo. Used by TodoService when deleting a todo.

**Note:** This endpoint is `[AllowAnonymous]` for inter-service communication.

**Response:** `204 No Content`

---

## Business Rules

### Tag Ownership
- Every tag MUST belong to a `cognito_sub` (user)
- Users can only see/modify their own tags
- Tags are automatically filtered by `cognito_sub`

### Tag Scope
- **Personal Tag**: `groupId = null`
  - Only visible to the creator
  - Can be used on personal todos
  
- **Group Tag**: `groupId = <group_id>`
  - Visible to all group members
  - Can be used on group todos

### Unique Constraint
- Tag name must be unique per user per scope
- Same tag name can exist for:
  - Different users
  - Same user in different groups
  - Same user (personal vs group)

**Example:**
```
✅ User A: "Work" (personal)
✅ User A: "Work" (group 1)
✅ User B: "Work" (personal)
❌ User A: "Work" (personal) - duplicate!
```

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Tag with this name already exists"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 404 Not Found
```json
{
  "message": "Tag not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "Internal server error"
}
```

---

## Example Usage

### Create Personal Tag
```bash
curl -X POST http://localhost:5003/api/tag \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "tagName": "Personal",
    "color": "#3498DB"
  }'
```

### Create Group Tag
```bash
curl -X POST http://localhost:5003/api/tag \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "tagName": "Team Project",
    "color": "#2ECC71",
    "groupId": 5
  }'
```

### Get All Personal Tags
```bash
curl -X GET "http://localhost:5003/api/tag" \
  -H "Authorization: Bearer <token>"
```

### Get Group Tags
```bash
curl -X GET "http://localhost:5003/api/tag?groupId=5" \
  -H "Authorization: Bearer <token>"
```

### Add Tag to Todo
```bash
curl -X POST http://localhost:5003/api/tag/todo/10 \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "tagId": 1
  }'
```

---

## Database Schema

```sql
CREATE TABLE tags (
    tag_id INT AUTO_INCREMENT PRIMARY KEY,
    tag_name VARCHAR(50) NOT NULL,
    cognito_sub VARCHAR(50) NOT NULL,
    group_id INT DEFAULT NULL,
    color VARCHAR(7) DEFAULT '#808080',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_tag_per_scope (tag_name, cognito_sub, group_id)
);

CREATE TABLE todo_tags (
    todo_id INT NOT NULL,
    tag_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (todo_id, tag_id),
    FOREIGN KEY (tag_id) REFERENCES tags(tag_id) ON DELETE CASCADE
);
```

---

## Configuration

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=tag_service_db;Uid=root;Pwd=your_password;"
  },
  "JWT": {
    "Issuer": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_YOUR_POOL_ID",
    "Audience": "YOUR_CLIENT_ID"
  },
  "AWS": {
    "Cognito": {
      "Authority": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_YOUR_POOL_ID"
    }
  }
}
```

Update `.env`:
```
DB_PASSWORD=your_database_password
```
