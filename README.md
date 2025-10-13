# Mục tiêu dự án 

Dự án này được tạo ra với mục đích để học Microservice , cùng với việc sử dụng AWS Services . Frontend hoàn toàn là Vide Coding .

# Thông tin về dự án 

Dự án này là một Todo list website , nhằm nhắc nhở người dùng làm các việc cần làm đã có , sử dụng AWS Services để thông báo đến tài khoản người dùng thông qua email đã đăng ký tài khoản , nhưng vẫn cố gắng giảm thiểu chi phí duy trì website nhưng vẫn đạt được trải nhiệm ổn định . Tương lai sẽ hướng app này giống với Slack. 

Các chức năng hiện có như sau : 

- Tạo và xác thực tài khoản người dùng bằng AWS Cognito
- Tạo Tag chỉ cho Premium user 

Lộ trình các chức năng sẽ phát triển :

- Phát triển chức năng **Tạo nhóm** , **tạo Todo list cho nhóm** .
- Người dùng có thể thao tác với Todo list , thêm bỏ tag khỏi Todo list.
- Cho phép người dùng tải lên các Attachment kèm theo Todo list , đi kèm với tính năng Premium-user thì tải được nhiều Attachment hơn Normal-user
- Sử dụng các AWS Services để **Notification tới người dùng riêng lẻ** , **Notification theo nhóm** khi tới hạn , ví dụ các services như : AWS Lambda , SNS , SQS , EventBridge
- Phát triển chức năng **Chat nhóm** , **Chat 1-1** , sử dụng **ScyllaDB** cho việc lưu trữ dữ liệu Chat. Sử dụng ASP.NET Core + SignalR cho Websocket 
- Tìm cách làm Notification cho hệ thống chat , ví dụ như có người nhắn tin thì sẽ có thông báo thông qua Web app giống Whatsapp.
- Tối ưu hệ thống Logging , Monitoring .


Kiến trúc hướng tới dài hạn : 

- Phát triển backend theo hướng kiến trúc Clean Architecture. 
- Sử dụng các Pattern như : CQRS.
- Hướng tới các nguyên lý như : SOLID. 
- Tối ưu chi phí AWS Services từ từ.


Công việc hiện tại - Bắt đầu từ ngày 08/10

- Phát triển tính năng Group