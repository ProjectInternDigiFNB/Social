# 🌐 Social Frontend — Mạng Xã Hội Thông Minh & Sàn Giao Dịch Hàng Số

<div align="center">

![React 19](https://img.shields.io/badge/React_19.2-20232A?style=for-the-badge&logo=react&logoColor=61DAFB)
![TypeScript 6](https://img.shields.io/badge/TypeScript_6.0-3178C6?style=for-the-badge&logo=typescript&logoColor=white)
![Vite 8](https://img.shields.io/badge/Vite_8.3-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![Zustand 5](https://img.shields.io/badge/Zustand_5.0-433E38?style=for-the-badge&logo=react&logoColor=white)
![SignalR 10](https://img.shields.io/badge/SignalR_10-WebSocket-512BD4?style=for-the-badge&logoColor=white)
![Oxlint](https://img.shields.io/badge/Oxlint_1.81-Fast_Linter-FFC107?style=for-the-badge&logoColor=black)

Giao diện người dùng (SPA) hiệu năng cao cho nền tảng **Social Commerce** — kết hợp mạng xã hội cá nhân hóa bằng AI, thông báo thời gian thực qua WebSocket (SignalR), chợ giao dịch sản phẩm số (Digital Marketplace), cổng người bán (Seller Portal) và bảng điều khiển quản trị viên (Admin Dashboard).

</div>

---

## ✨ Tính Năng & Phân Hệ Giao Diện

| Phân hệ | Mô tả chi tiết |
| :--- | :--- |
| 🔐 **Xác thực & Phân quyền (Auth & RBAC)** | Đăng ký, đăng nhập truyền thống & Google OAuth 2.0 (`@react-oauth/google`), xác minh email, quên/đặt lại mật khẩu. Tự động làm mới Access Token ngầm (Silent Refresh với cơ chế khử trùng lặp request & grace period chống đăng xuất khi reload trang nhanh). Bảo vệ định tuyến 3 cấp (`GuestRoute`, `ProtectedRoute`, `AdminRoute`). |
| 📰 **Bảng tin thông minh (Hybrid Social Feed)** | Chuyển đổi mượt mà giữa 3 luồng: **Dành cho bạn (AI Personalized)**, **Đang theo dõi (Following)** và **Mới nhất (Latest)**. Hỗ trợ tạo bài viết đa phương tiện, gắn thẻ sở thích, đính kèm sản phẩm số từ chợ (Social Commerce Product Tagging), bình luận lồng nhau và cuộn vô tận không điểm dừng (Infinite Feed Recirculation). |
| ⚡ **Tương tác Thời gian thực (SignalR)** | Kết nối WebSocket qua `@microsoft/signalr` nhận thông báo tức thì (Like, Comment, Follow, cập nhật trạng thái đơn hàng), hiển thị `RealtimeToast`, chuông thông báo (`NotificationBell`) và cảnh báo bài viết mới (`NewPostAlertPill`). |
| 🛍️ **Chợ Hàng Số (Digital Marketplace)** | Duyệt cây danh mục đa cấp, lọc & tìm kiếm sản phẩm số, xem chi tiết biến thể SKU, hồ sơ Shop, giỏ hàng (`cartStore`), giữ chỗ tồn kho 15 phút và thanh toán mã QR tự động (PayOS VietQR / Mock Payment). |
| 🎯 **Gợi ý Sản phẩm AI (Recommendations UI)** | Tích hợp 3 khối đề xuất thông minh: **Sản phẩm tương tự** (`RelatedProductsSection`), **Thường mua cùng nhau** (`BoughtTogetherSection`) và **Dành riêng cho bạn** (`ForYouSection`), kèm bộ theo dõi thời gian dừng trang (`useProductDwellTracker`). |
| 📦 **Đơn hàng & Nhận Mã Bảo Mật** | Quản lý lịch sử đơn hàng, hiển thị mã kích hoạt/tài khoản số đã giải mã (`OrderItemCredentials`), tải tệp qua Signed URL, đánh giá sao (`OrderReviewModal`) và gửi khiếu nại bảo hành trong 48h (`OrderComplaintModal`). |
| 🏪 **Cổng Người Bán (Seller Portal)** | Quản lý toàn diện gian hàng qua 5 tab: **Tổng quan (`OverviewTab`)**, **Sản phẩm (`ProductsTab`)**, **Đơn hàng (`OrdersTab`)**, **Tài chính & Sổ cái (`FinanceTab`)**, **Khiếu nại (`ComplaintsTab`)**. Hỗ trợ nạp kho mã số hàng loạt (`BulkImportCodesModal`), kiểm tra kho mã (`InspectCodesModal`) và nhập/xuất Excel. |
| 🛡️ **Bảng Quản Trị (Admin Dashboard)** | Giao diện điều hành sàn gồm 8 phân khu: **Gian hàng (`AdminShopsTab`)**, **Danh mục & Thuộc tính (`AdminCategoriesTab`)**, **Sản phẩm (`AdminProductsTab`)**, **Rút tiền (`AdminPayoutsTab`)**, **Khiếu nại (`AdminComplaintsTab`)**, **Báo cáo vi phạm (`AdminReportsTab`)**, **Đối soát sổ cái (`AdminAuditTab`)** và **Người dùng (`AdminUsersTab`)**. |

---

## 🚀 Tối Ưu Hóa Hiệu Năng (Performance Engineering)

Frontend được thiết kế tối ưu cho tốc độ tải trang (LCP), độ mượt khi cuộn (60–120 FPS) và tiết kiệm băng thông mạng:

1. **Normalized Entity Store (`feedStore.ts`):**
   - Lưu trữ bài viết dạng phẳng `entities: Record<string, PostDto>` làm nguồn dữ liệu duy nhất (*Single Source of Truth*).
   - Khi người dùng Like, bình luận hoặc xóa bài viết, trạng thái được đồng bộ hóa tức thì với độ phức tạp `O(1)` trên cả 3 tab bảng tin và modal chi tiết.
2. **On-Demand Micro-Bundling & Code Splitting:**
   - Tách riêng các thư viện lõi qua `manualChunks` trong Vite (`vendor-react`, `vendor-network`, `vendor-auth`).
   - Lazy-load theo tuyến đường (`React.lazy` trong `AppRoutes.tsx`) và nạp động theo hành vi đối với các thành phần phụ như `CommentSection` và `ConfirmModal` trong `PostCard.tsx` (giảm ~24.4% kích thước bundle chính).
3. **Ảo hóa DOM bằng CSS (`content-visibility: auto`):**
   - Áp dụng `content-visibility: auto; contain-intrinsic-size: 0 450px;` cho từng `.post-entry` và đặt khung giữ chỗ tối thiểu (`CLS = 0`), giúp trình duyệt bỏ qua chi phí Layout/Paint cho các bài viết nằm ngoài khung nhìn.
4. **Adaptive Media CDN (`utils/media.ts`):**
   - Tự động biến đổi URL ảnh Cloudinary sang định dạng nén thế hệ mới (`f_auto,q_auto` -> AVIF/WebP) và co giãn kích thước chuẩn theo ngữ cảnh hiển thị (`w_900` cho ảnh bài đăng, `w_96,c_fill` cho avatar, `w_160,c_fill` cho ảnh thu nhỏ sản phẩm).
   - Ưu tiên tải trước (`loading="eager"`, `fetchPriority="high"`) cho bài viết đầu tiên trên màn hình (Above-the-fold LCP).
5. **Telemetry Batching Queue (`utils/interactionQueue.ts`):**
   - Gom các sự kiện xem bài viết (`InteractionType.View`) vào hàng đợi đệm trên bộ nhớ và chỉ gửi batch qua `POST /api/interactions/track-batch` khi đủ **10 sự kiện** hoặc sau **2.5 giây** (kèm cơ chế tự động xả khi ẩn/đóng tab qua `visibilitychange` & `beforeunload`), giảm ~90% số lượng HTTP request lẻ.
6. **Keyset Cursor Pagination & In-Memory Seen Cache (`utils/seenPosts.ts`):**
   - Phân trang bài viết bằng con trỏ thời gian `before` (`CreatedAtUtc`) kết hợp bộ đệm RAM + Debounced `localStorage` để ghi nhận các bài đã đọc mà không gây nghẽn luồng chính (Main Thread).

---

## 📁 Cấu Trúc Thư Mục (`Frontend/src`)

```text
src/
├── api/                        # Tầng giao tiếp HTTP (Axios instances & Domain API modules)
│   ├── client.ts               # Axios instance, JWT Bearer interceptor & Silent Refresh queue
│   ├── authApi.ts              # Đăng ký, đăng nhập, OAuth, refresh token, xác minh email
│   ├── postApi.ts              # CRUD bài viết, bảng tin AI, phân trang cursor (before)
│   ├── interactionApi.ts       # Ghi nhận tương tác đơn lẻ & theo lô (track-batch)
│   ├── marketplaceApi.ts       # Catalog, giỏ hàng, checkout, đơn hàng, cổng Seller
│   ├── marketplaceRecommendationApi.ts # API gợi ý sản phẩm AI (Related, Bought Together, For You)
│   ├── adminApi.ts             # API quản trị sàn, duyệt shop, đối soát sổ cái, xử lý khiếu nại
│   ├── followApi.ts            # Theo dõi / Hủy theo dõi người dùng
│   ├── interestApi.ts          # Chủ đề quan tâm & Onboarding
│   ├── notificationApi.ts      # Lịch sử thông báo & đánh dấu đã đọc
│   └── profileApi.ts           # Hồ sơ cá nhân & tải ảnh đại diện
├── components/                 # Hệ thống UI Components tái sử dụng
│   ├── auth/                   # Guard components: ProtectedRoute, GuestRoute, AdminRoute
│   ├── common/                 # Design System primitives: Button, Input, Select, ConfirmModal, Skeleton...
│   ├── feedback/               # ErrorBoundary, LoadingScreen
│   ├── layout/                 # Layout, Navbar, MobileNav, Footer, NotificationBell, RealtimeToast
│   ├── marketplace/            # ProductCard, OrderStatusBadge, khối gợi ý AI (recommendations/)
│   ├── posts/                  # CreatePostBox, PostList, PostCard, CommentSection, ProductPickerModal...
│   └── profile/                # MyPostsList
├── config/
│   └── env.ts                  # Quản lý & kiểm chuẩn tập trung các biến môi trường (ENV)
├── constants/
│   └── routes.ts               # Định nghĩa tập trung toàn bộ đường dẫn (ROUTES)
├── hooks/                      # Custom React Hooks
│   ├── useAuth.ts              # Truy cập nhanh trạng thái và hành động xác thực
│   ├── useInfiniteScroll.ts    # IntersectionObserver phục vụ cuộn vô tận
│   ├── useProductDwellTracker.ts # Đo thời gian xem sản phẩm để huấn luyện gợi ý AI
│   ├── useSignalR.ts           # Quản lý vòng đời kết nối WebSocket theo phiên đăng nhập
│   └── useTitle.ts             # Cập nhật tiêu đề trang (Document Title)
├── pages/                      # Các trang màn hình chính (Lazy-loaded theo route)
│   ├── auth/                   # Login, Register, VerifyEmail, ForgotPassword, ResetPassword
│   ├── home/                   # HomePage (Bảng tin 3 tab, khung đăng bài, gợi ý kết bạn/sản phẩm)
│   ├── onboarding/             # OnboardingPage (Chọn sở thích lần đầu)
│   ├── post-detail/            # PostDetailPage
│   ├── profile/                # ProfilePage
│   ├── marketplace/            # MarketplacePage, ProductDetailPage, ShopProfilePage, Cart, Checkout, Orders
│   ├── seller/                 # SellerPortalPage (5 tabs & 9 modals nghiệp vụ người bán)
│   ├── admin/                  # AdminDashboardPage (8 tabs quản trị toàn sàn)
│   └── not-found/              # NotFoundPage (404)
├── routes/
│   └── AppRoutes.tsx           # Cấu hình React Router v7 & Code-splitting Suspense
├── services/
│   └── signalrService.ts       # Quản lý kết nối SignalR Hub, tự động kết nối lại (Auto-reconnect)
├── store/                      # Quản lý trạng thái toàn cục với Zustand 5
│   ├── authStore.ts            # Phiên người dùng, khởi tạo Auth phi chặn (Non-blocking init)
│   ├── feedStore.ts            # Normalized Entity Store cho bài viết & bộ nhớ đệm tab
│   ├── cartStore.ts            # Giỏ hàng sản phẩm số
│   ├── notificationStore.ts    # Danh sách thông báo & bộ đếm chưa đọc
│   └── postModalStore.ts       # Trạng thái đóng/mở hộp thoại chi tiết bài viết
├── styles/                     # Kiến trúc CSS thuần theo Design Tokens (Light Mode Clean UI)
│   ├── variables.css           # Biến màu sắc, khoảng cách, đổ bóng, bo góc, phông chữ
│   ├── base.css                # Reset & thiết lập nền tảng
│   ├── components.css          # Style cho các thành phần dùng chung
│   ├── social.css              # Giao diện bảng tin, bài đăng, bình luận, ảo hóa DOM
│   ├── marketplace.css         # Giao diện chợ số, giỏ hàng, thanh toán, cổng Seller
│   └── admin.css               # Giao diện bảng quản trị Admin
├── types/                      # Định nghĩa kiểu dữ liệu TypeScript đồng bộ với DTO Backend
└── utils/                      # Các tiện ích xử lý logic
    ├── media.ts                # Biến đổi URL ảnh Cloudinary thích ứng (AVIF/WebP & Resize)
    ├── interactionQueue.ts     # Hàng đợi gom lô sự kiện xem bài viết (Telemetry Batching)
    ├── seenPosts.ts            # Bộ nhớ đệm bài viết đã xem (RAM + Debounced LocalStorage)
    ├── storage.ts              # Đọc/ghi token và dữ liệu cục bộ an toàn
    ├── date.ts                 # Định dạng thời gian tương đối (vừa xong, x phút trước...)
    └── error.ts                # Chuẩn hóa thông báo lỗi từ ProblemDetails của Backend
```

---

## 🧭 Sơ Đồ Định Tuyến (`ROUTES`)

| Đường dẫn | Component Trang | Quyền truy cập |
| :--- | :--- | :--- |
| `/` | `HomePage` | Công khai (Khách xem bảng tin chung / Thành viên xem bảng tin cá nhân hóa) |
| `/login`, `/register`, `/verify-email`, `/forgot-password`, `/reset-password` | Cụm trang `pages/auth/*` | `GuestRoute` (Chuyển hướng về `/` nếu đã đăng nhập) |
| `/onboarding` | `OnboardingPage` | `ProtectedRoute` (Thành viên mới chọn sở thích) |
| `/profile` | `ProfilePage` | `ProtectedRoute` |
| `/posts/:id` | `PostDetailPage` | `ProtectedRoute` |
| `/marketplace` | `MarketplacePage` | Công khai |
| `/marketplace/products/:slug` | `ProductDetailPage` | Công khai |
| `/marketplace/shops/:slug` | `ShopProfilePage` | Công khai |
| `/cart`, `/checkout/:id`, `/orders`, `/orders/:id` | Cụm trang Mua hàng & Đơn hàng | `ProtectedRoute` |
| `/seller`, `/seller/*` | `SellerPortalPage` | `ProtectedRoute` (Người dùng đăng ký Shop hoặc có vai trò `Seller` / `Admin`) |
| `/admin`, `/admin/*` | `AdminDashboardPage` | `AdminRoute` (Chỉ dành cho vai trò `Admin`) |

---

## ⚙️ Cấu Hình Biến Môi Trường (`.env`)

Tạo tệp `.env` hoặc `.env.local` tại thư mục `Frontend/` (cùng cấp với `package.json`). Nếu không khai báo, ứng dụng sẽ sử dụng các giá trị mặc định thông minh trong [`src/config/env.ts`](./src/config/env.ts):

```env
# Đường dẫn gốc gọi REST API (mặc định '/api' để đi qua Vite Proxy ở local hoặc Vercel Rewrite)
VITE_API_BASE_URL=/api

# Địa chỉ máy chủ Backend ASP.NET Core (Vite Dev Server sẽ proxy '/api' và '/hubs' tới địa chỉ này)
VITE_BACKEND_URL=http://localhost:5126

# (Tùy chọn) Endpoint kết nối trực tiếp SignalR Hub
VITE_HUB_URL=/hubs/social

# Google OAuth 2.0 Client ID phục vụ đăng nhập một chạm bằng Google
VITE_GOOGLE_CLIENT_ID=your-google-client-id.apps.googleusercontent.com

# Tên hiển thị của ứng dụng
VITE_APP_NAME="Social Network"
```

---

## 🛠️ Hướng Dẫn Cài Đặt & Phát Triển Cục Bộ

### 1. Yêu cầu hệ thống
- **Node.js:** `>= 20.x` (khuyến nghị bản LTS mới nhất)
- **Backend API:** Đang chạy tại `http://localhost:5126` (xem hướng dẫn tại [README gốc](../README.md))

### 2. Cài đặt gói phụ thuộc
```bash
cd Frontend
npm install
```

### 3. Các lệnh thực thi (Scripts)

| Lệnh | Mô tả |
| :--- | :--- |
| `npm run dev` | Khởi chạy máy chủ phát triển Vite tại `http://localhost:5173` (kèm HMR & Proxy `/api`, `/hubs` sang Backend). |
| `npm run lint` | Kiểm tra chất lượng mã nguồn siêu tốc bằng **Oxlint**. |
| `npm run build` | Kiểm tra kiểu dữ liệu TypeScript (`tsc -b`) và đóng gói bản Production tối ưu vào thư mục `dist/`. |
| `npm run preview` | Chạy thử bản đóng gói Production cục bộ trước khi triển khai. |
