# Anki Focus Enforcer
Một ứng dụng WinForms nhỏ gọn giúp bạn duy trì sự tập trung tuyệt đối khi học Anki bằng cách khóa các tính năng gây xao nhãng trên máy tính.

## Tính năng chính
- **Hẹn giờ tập trung:** Tùy chỉnh thời gian học dựa theo số phút. 
- **Yêu cầu mở Anki:** Ứng dụng tự động kiểm tra xem Anki có đang chạy hay không trước khi bắt đầu.
- **Tùy chỉnh tính năng khóa:**
  - **Khóa phím tắt:** Chặn Alt+Tab, phím Windows để tránh chuyển đổi cửa sổ.
  - **Khóa Taskbar:** Vô hiệu hóa thanh Taskbar và Start Menu của Windows.
  - **Ẩn Desktop:** Ẩn toàn bộ các biểu tượng trên màn hình nền.
  - **Dừng Media:** Tự động tạm dừng nhạc hoặc video đang phát dưới nền.
  - **Chống Sleep:** Ngăn máy tính tự động rơi vào chế độ ngủ (Sleep mode) khi đang dùng.
- **Thông báo hệ thống:** Gửi thông báo Toast nhắc nhở khi bắt đầu phiên.

## Hướng dẫn sử dụng
1. Bật phần mềm **Anki** trên máy tính của bạn.
2. Mở ứng dụng **Anki Focus Enforcer**.
3. Nhập số phút bạn muốn học (mặc định là 15 phút).
4. Nhấn **Tùy chỉnh ➡** để chọn các chế độ khóa mong muốn (Bật/Tắt dễ dàng).
5. Nhấn **BẮT ĐẦU KHÓA**. Ứng dụng sẽ đếm ngược 5 giây trước khi vào chế độ tập trung.
6. Thưởng thức.

## Công nghệ sử dụng
- Ngôn ngữ: C# 
- Giao diện: Windows Forms
- Gói bổ sung: `Microsoft.Toolkit.Uwp.Notifications` (hỗ trợ Toast Notifications)

---
