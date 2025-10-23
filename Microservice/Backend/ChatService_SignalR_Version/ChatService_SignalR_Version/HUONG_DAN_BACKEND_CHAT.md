# 🚀 HƯỚNG DẪN BACKEND CHAT REALTIME VỚI SIGNALR + SCYLLADB

## 📋 TỔNG QUAN

Backend chat của bạn hoạt động theo mô hình:
```
Client (Browser/App) 
    ↕️ WebSocket (SignalR)
ChatHub (SignalR Hub)
    ↕️
ChatServiceMVP (Business Logic)
    ↕️
MessageRepository (Data Access)
    ↕️
ScyllaDB (Database)
```

---

## 🏗️ CẤU TRÚC ĐÃ CÓ

### 1. **Models** (ChatModels.cs)
- `ChatMessage`: Entity chính cho tin nhắn
- `SendMessageRequest`: DTO để gửi tin nhắn

### 2. **Repository Pattern**
- `IMessageRepository`: Interface định nghĩa các phương thức truy cập dữ liệu
- `MessageRepository`: Implementation kết nối với ScyllaDB

### 3. **Service Layer**
- `ChatServiceMVP`: Business logic + broadcast tin nhắn qua SignalR

### 4. **SignalR Hub**
- `ChatHub`: WebSocket endpoint cho client kết nối

### 5. **REST API**
- `ChatControllerMVP`: HTTP endpoints để test và lấy lịch sử tin nhắn

---

## 🔄 LUỒNG HOẠT ĐỘNG

### **Kịch bản 1: Client gửi tin nhắn qua WebSocket**

```
1. Client kết nối WebSocket → ws://localhost:5000/chatHub
2. Client join group → hub.invoke("JoinGroup", groupId)
3. Client gửi tin nhắn → hub.invoke("SendMessageRealtime", groupId, "Hello", "text")
4. ChatHub nhận request
5. ChatHub gọi ChatServiceMVP.SendMessageAsync()
6. ChatServiceMVP lưu vào ScyllaDB qua MessageRepository
7. ChatServiceMVP broadcast tin nhắn đến tất cả clients trong group
8. Clients nhận tin nhắn qua event "ReceiveMessage"
```

### **Kịch bản 2: Client gửi tin nhắn qua REST API**

```
1. Client POST → /api/ChatMVP/send
2. ChatController nhận request
3. ChatController gọi ChatServiceMVP.SendMessageAsync()
4. ChatServiceMVP lưu vào ScyllaDB
5. ChatServiceMVP broadcast qua SignalR
6. Clients đang online nhận tin nhắn realtime
```

---

## 🔌 CÁCH CLIENT KẾT NỐI

### **JavaScript/TypeScript Client**

```javascript
// 1. Cài đặt
npm install @microsoft/signalr

// 2. Kết nối
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/chatHub")
    .withAutomaticReconnect()
    .build();

// 3. Lắng nghe sự kiện
connection.on("ReceiveMessage", (message) => {
    console.log("Nhận tin nhắn:", message);
    // message = { groupId, messageId, userId, messageText, createdAt, ... }
});

connection.on("UserJoined", (data) => {
    console.log(`${data.userName} đã vào nhóm ${data.groupId}`);
});

connection.on("UserStartedTyping", (data) => {
    console.log(`${data.userName} đang gõ...`);
});

// 4. Kết nối
await connection.start();

// 5. Join group (BẮT BUỘC!)
await connection.invoke("JoinGroup", 123);

// 6. Gửi tin nhắn
await connection.invoke("SendMessageRealtime", 123, "Hello World!", "text");

// 7. Typing indicators
await connection.invoke("StartTyping", 123);
setTimeout(() => connection.invoke("StopTyping", 123), 3000);

// 8. Leave group
await connection.invoke("LeaveGroup", 123);
```

---

## 🧪 CÁCH TEST

### **1. Test với REST API (Postman/curl)**

```bash
# Gửi tin nhắn
POST http://localhost:5000/api/ChatMVP/send
Content-Type: application/json

{
  "groupId": 123,
  "userId": "user-abc-123",
  "messageText": "Hello from REST API",
  "messageType": "text"
}

# Lấy lịch sử tin nhắn
GET http://localhost:5000/api/ChatMVP/messages/123?limit=50

# Lấy 1 tin nhắn cụ thể
GET http://localhost:5000/api/ChatMVP/message/123/{messageId}
```

### **2. Test với SignalR (Browser Console)**

```javascript
// Mở browser console tại http://localhost:5000
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", msg => console.log("📩", msg));
await connection.start();
await connection.invoke("JoinGroup", 123);
await connection.invoke("SendMessageRealtime", 123, "Test message", "text");
```

---

## ⚙️ CẤU HÌNH CẦN THIẾT

### **1. ScyllaDB phải chạy**
```bash
# Kiểm tra ScyllaDB
docker ps | grep scylla

# Nếu chưa có, chạy:
docker run --name scylla -d -p 9042:9042 scylladb/scylla

# Tạo keyspace và tables (chạy file .cql của bạn)
docker exec -it scylla cqlsh
```

### **2. Redis phải chạy (nếu dùng)**
```bash
docker run --name redis -d -p 6379:6379 redis
```

### **3. appsettings.json**
```json
{
  "ScyllaDB": {
    "ContactPoints": ["localhost"],
    "Port": 9042,
    "Keyspace": "chat_service"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

---

## 🎯 CÁC TÍNH NĂNG ĐÃ CÓ

✅ Gửi/nhận tin nhắn realtime  
✅ Lưu tin nhắn vào ScyllaDB  
✅ Lấy lịch sử tin nhắn  
✅ Join/Leave group  
✅ Typing indicators (không lưu DB)  
✅ Broadcast tin nhắn đến tất cả clients trong group  
✅ REST API để test  

---

## 🚧 CÁC TÍNH NĂNG CÓ THỂ BỔ SUNG

### **Phase 2: Nâng cao**
- ❌ Unread counters (bảng `unread_counts`)
- ❌ User conversations list (bảng `user_conversations`)
- ❌ Message reactions (bảng `message_reactions`)
- ❌ Read receipts (bảng `message_read_receipts`)
- ❌ Edit/Delete messages
- ❌ File upload (images, videos)
- ❌ Authentication (JWT)
- ❌ Online/Offline status (Redis)

---

## 🐛 TROUBLESHOOTING

### **Lỗi: "Cannot connect to ScyllaDB"**
```bash
# Kiểm tra ScyllaDB có chạy không
docker ps | grep scylla

# Kiểm tra port 9042
netstat -an | findstr 9042

# Test kết nối
docker exec -it scylla cqlsh -e "DESCRIBE KEYSPACES"
```

### **Lỗi: "SignalR connection failed"**
- Kiểm tra CORS đã cấu hình đúng chưa
- Kiểm tra URL: `http://localhost:5000/chatHub`
- Kiểm tra browser console có lỗi không

### **Tin nhắn không broadcast**
- Client phải gọi `JoinGroup(groupId)` trước khi gửi tin nhắn
- Kiểm tra log trong console

---

## 📝 LƯU Ý QUAN TRỌNG

1. **BẮT BUỘC phải JoinGroup trước khi gửi tin nhắn**
   - SignalR dùng "groups" để broadcast tin nhắn
   - Nếu không join, client sẽ không nhận được tin nhắn

2. **GroupId là INT, không phải GUID**
   - Theo schema ScyllaDB của bạn

3. **MessageId là TIMEUUID**
   - Tự động generate khi lưu vào DB
   - Đảm bảo thứ tự tin nhắn đúng

4. **UserId là STRING (cognito_sub)**
   - Không phải GUID
   - Lấy từ JWT token trong production

5. **Typing indicators không lưu DB**
   - Chỉ broadcast realtime
   - Nếu muốn lưu, dùng bảng `typing_indicators`

---

## 🎓 KIẾN THỨC CẦN NẮM

### **SignalR Concepts**
- **Hub**: Server-side endpoint cho WebSocket
- **Groups**: Nhóm các connections lại (như chat rooms)
- **Clients**: Gửi message đến clients
- **Invoke**: Client gọi method trên server
- **SendAsync**: Server gửi event đến client

### **Repository Pattern**
- **Interface**: Định nghĩa contract
- **Implementation**: Thực thi logic truy cập dữ liệu
- **Dependency Injection**: Inject vào Service

### **ScyllaDB/Cassandra**
- **Partition Key**: `group_id` - dữ liệu cùng group nằm cùng node
- **Clustering Key**: `created_at, message_id` - sắp xếp trong partition
- **TIMEUUID**: UUID có timestamp, đảm bảo thứ tự

---

## 🚀 CHẠY ỨNG DỤNG

```bash
# 1. Chạy ScyllaDB
docker run --name scylla -d -p 9042:9042 scylladb/scylla

# 2. Tạo schema (đợi 30s để ScyllaDB khởi động)
docker exec -it scylla cqlsh -f /path/to/schema.cql

# 3. Chạy backend
cd ChatService_SignalR_Version
dotnet run

# 4. Test
# Mở browser: http://localhost:5000/swagger
# Hoặc dùng Postman/curl
```

---

## 📚 TÀI LIỆU THAM KHẢO

- SignalR: https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction
- ScyllaDB: https://docs.scylladb.com/
- Cassandra C# Driver: https://docs.datastax.com/en/developer/csharp-driver/latest/

---

**Chúc bạn học tốt! 🎉**
