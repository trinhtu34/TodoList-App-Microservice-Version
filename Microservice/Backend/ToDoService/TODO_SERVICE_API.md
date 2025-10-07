# TodoService API Documentation

## Overview
TodoService manages todos for users. Todos can be personal or belong to a group.

## Base URL
```
http://localhost:5002/api/todos
```

## Authentication
All endpoints require JWT Bearer token:
```
Authorization: Bearer <your_jwt_token>
```

---

## API Endpoints

### 1. Get All Todos
**GET** `/api/todos`

Get todos for current user (personal or group).

**Query Parameters:**
- `groupId` (optional): Filter todos by group

**Examples:**
```bash
# Get personal todos
GET /api/todos

# Get group todos
GET /api/todos?groupId=5
```

**Response:**
```json
[
  {
    "todoId": 1,
    "description": "Complete project",
    "isDone": false,
    "dueDate": "2024-01-20T10:00:00Z",
    "createAt": "2024-01-15T09:00:00Z",
    "updateAt": "2024-01-15T09:00:00Z",
    "groupId": null,
    "assignedTo": null,
    "tags": [
      {
        "tagId": 1,
        "tagName": "Urgent",
        "color": "#FF5733"
      }
    ]
  }
]
```

---

### 2. Get Todo by ID
**GET** `/api/todos/{id}`

**Response:**
```json
{
  "todoId": 1,
  "description": "Complete project",
  "isDone": false,
  "dueDate": "2024-01-20T10:00:00Z",
  "createAt": "2024-01-15T09:00:00Z",
  "updateAt": "2024-01-15T09:00:00Z",
  "groupId": null,
  "assignedTo": null,
  "tags": []
}
```

---

### 3. Create Todo
**POST** `/api/todos`

**Request Body:**
```json
{
  "description": "New task",
  "dueDate": "2024-01-25T15:00:00Z",
  "groupId": null,
  "assignedTo": null,
  "tagIds": [1, 2]
}
```

**Validation:**
- `description`: Required, max 1000 characters
- `dueDate`: Optional
- `groupId`: Optional (null = personal todo)
- `assignedTo`: Optional (cognito_sub of assignee)
- `tagIds`: Optional list of tag IDs

**Response:** `201 Created`

---

### 4. Update Todo
**PUT** `/api/todos/{id}`

**Request Body:**
```json
{
  "description": "Updated task",
  "isDone": true,
  "dueDate": "2024-01-26T15:00:00Z",
  "assignedTo": "user-cognito-sub"
}
```

**Response:** `204 No Content`

---

### 5. Delete Todo
**DELETE** `/api/todos/{id}`

Deletes todo and all associated tags.

**Response:** `204 No Content`

---

### 6. Verify Todo Ownership (Internal)
**GET** `/api/todos/verify/{todoId}/owner/{userId}`

Used by other services to verify access.

**Response:**
```json
true
```

---

## Business Rules

### Todo Ownership
- **Personal Todo**: `groupId = null`
  - Only creator can access
  - `cognitoSub` = creator

- **Group Todo**: `groupId = <group_id>`
  - All group members can access
  - Verified via GroupService

### Assignment
- `assignedTo` = `null`: Unassigned
- `assignedTo` = `cognito_sub`: Assigned to specific user

### Tags
- Tags managed by TagService
- Tags added/removed via TagService API
- TodoService fetches tags when returning todos

---

## Service Dependencies

### TagService
- `GET /api/tag/todo/{todoId}` - Get tags for todo
- `POST /api/tag/todo/{todoId}` - Add tag to todo
- `DELETE /api/tag/cleanup/todo/{todoId}` - Remove all tags

### GroupService
- `GET /api/group/{groupId}/verify-member/{userId}` - Verify membership

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied"
}
```

### 404 Not Found
```json
{
  "message": "Todo not found"
}
```

---

## Example Usage

### Create Personal Todo
```bash
curl -X POST http://localhost:5002/api/todos \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Buy groceries",
    "dueDate": "2024-01-20T18:00:00Z",
    "tagIds": [1]
  }'
```

### Create Group Todo
```bash
curl -X POST http://localhost:5002/api/todos \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Team meeting",
    "dueDate": "2024-01-22T10:00:00Z",
    "groupId": 5,
    "assignedTo": "user-123-cognito-sub",
    "tagIds": [2, 3]
  }'
```

### Update Todo
```bash
curl -X PUT http://localhost:5002/api/todos/1 \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "isDone": true
  }'
```

---

## Configuration

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=todo_service_db;Uid=root;Pwd=your_password;"
  },
  "JWT": {
    "Issuer": "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_YOUR_POOL_ID",
    "Audience": "YOUR_CLIENT_ID"
  },
  "ServiceEndpoints": {
    "TagService": "http://localhost:5003",
    "GroupService": "http://localhost:5002"
  }
}
```
