# Windows Hello 身份認證系統

一個基於 ASP.NET Core 9.0 的現代化身份認證系統，集成了 Windows Hello 和 FIDO2/WebAuthn 技術，提供安全便捷的無密碼身份認證體驗。

## 🚀 項目特性

### 核心功能
- **傳統身份認證**: 支援用戶名/密碼註冊和登錄
- **Windows Hello 認證**: 使用生物識別技術進行安全認證
- **FIDO2/WebAuthn 支持**: 符合國際標準的無密碼認證
- **多重認證**: 支援多種認證方式並存
- **安全存儲**: 使用 Entity Framework Core 安全存儲憑據

### 技術棧
- **框架**: ASP.NET Core 9.0
- **身份認證**: ASP.NET Core Identity + FIDO2
- **資料庫**: SQL Server + Entity Framework Core
- **前端**: Bootstrap + jQuery
- **安全**: HTTPS + Cookie 策略 + 防偽權杖

## 📋 系統要求

### 運行環境
- **.NET 9.0** 或更高版本
- **SQL Server** (本地或遠端實例)
- **Windows 10/11** (推薦，用於 Windows Hello 功能)
- **現代流覽器**: Chrome 67+、Firefox 60+、Safari 14+、Edge 18+

### 硬體要求
- 支援 **Windows Hello** 的設備 (指紋識別器、攝像頭或 TPM)
- 或支持 **FIDO2** 的安全金鑰

## 🛠️ 安裝配置

### 1. 克隆項目
```dos command
git clone https://github.com/hankwu61/WindowsHelloAuth.git
cd WindowsHelloAuth
```

### 2. 配置資料庫連接

編輯 `appsettings.json` 檔中的連接字串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\sqlexpress;Database=WindowsHelloAuthDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

**連接字串說明：**
- `Server=.\\sqlexpress`: SQL Server Express 本地實例
- `Database=WindowsHelloAuthDb`: 資料庫名稱
- `Trusted_Connection=true`: 使用 Windows 身份認證
- `TrustServerCertificate=true`: 信任伺服器憑證

### 3. 安裝依賴包
```dos command
dotnet restore
```

### 4. 創建資料庫
```dos command
dotnet ef database update
```

如果沒有安裝 EF 工具，請先安裝：
```dos command
dotnet tool install --global dotnet-ef
```

### 5. 配置 HTTPS 證書
```dos command
dotnet dev-certs https --trust
```

## 🚀 運行應用

### 開發環境
```dos command
dotnet run
```

應用將在以下地址啟動：
- **HTTP**: http://localhost:5017
- **HTTPS**: https://localhost:5001 (推薦)

### 生產環境部署
```dos command
dotnet publish -c Release -o ./publish
```

## 📚 使用指南

### 使用者註冊流程

#### 1. 傳統註冊
1. 訪問 `/Account/Register`
2. 填寫郵箱、顯示名稱和密碼
3. 提交表單完成註冊
4. 自動登錄並重定向到首頁

#### 2. Windows Hello 註冊
1. 完成傳統註冊後登錄系統
2. 訪問 `/Account/WindowsHelloRegister`
3. 點擊"註冊 Windows Hello"按鈕
4. 按照流覽器提示完成生物識別設置
5. 註冊成功後可使用 Windows Hello 登錄

### 使用者登錄流程

#### 1. 傳統登錄
1. 訪問 `/Account/Login`
2. 輸入郵箱和密碼
3. 點擊"登錄"按鈕

#### 2. Windows Hello 登錄
1. 訪問 `/Account/Login`
2. 點擊"使用 Windows Hello 登錄"按鈕
3. 按照提示完成生物識別驗證
4. 自動登錄成功

### 功能頁面

- **首頁**: `/` - 應用主頁面
- **使用者資料**: `/Home/Profile` - 查看使用者資訊和已註冊的認證器
- **隱私政策**: `/Home/Privacy` - 隱私政策頁面

## 🔧 配置說明

### FIDO2 配置

在 `appsettings.json` 中配置 FIDO2 參數：

```json
{
  "Fido2": {
    "ServerDomain": "localhost",
    "ServerName": "Windows Hello Auth System",
    "Origins": [ "https://localhost:5001" ],
    "TimestampDriftTolerance": 300000
  }
}
```

**參數說明：**
- `ServerDomain`: 伺服器功能變數名稱
- `ServerName`: 顯示給使用者的服務名稱
- `Origins`: 允許的源功能變數名稱列表
- `TimestampDriftTolerance`: 時間戳記容差 (毫秒)

### 身份認證配置

系統使用 ASP.NET Core Identity，密碼策略：
- 最少 8 個字元
- 必須包含大寫字母
- 必須包含小寫字母
- 必須包含數位
- 必須包含特殊字元

### 會話配置
- 會話超時：30 分鐘
- Cookie 安全性原則：僅 HTTPS
- 防偽權杖保護

## 🔐 安全特性

### 資料保護
- **HTTPS 強制**: 所有通信均通過 HTTPS 加密
- **Cookie 安全**: HttpOnly、Secure、SameSite 策略
- **防偽保護**: CSRF 攻擊防護
- **密碼加密**: 使用 ASP.NET Core Identity 密碼雜湊

### FIDO2 安全
- **本地驗證**: 生物識別資料不傳輸到伺服器
- **公開金鑰加密**: 使用非對稱加密技術
- **防重放**: 內置挑戰-回應機制
- **設備綁定**: 憑據與特定設備綁定

## 🌐 API 介面

### FIDO2 API 端點

#### 註冊流程
```
POST /api/Fido2/makeCredentialOptions
POST /api/Fido2/makeCredential
```

#### 認證流程
```
POST /api/Fido2/assertionOptions
POST /api/Fido2/makeAssertion
```

#### 憑據管理
```
GET /api/Fido2/credentials/{userId}
DELETE /api/Fido2/credentials/{credentialId}
```

## 🛠️ 開發指南

### 專案結構
```
WindowsHelloAuth/
├── Controllers/          # 控制器
│   ├── AccountController.cs
│   ├── Fido2Controller.cs
│   └── HomeController.cs
├── Models/               # 資料模型
│   ├── ApplicationUser.cs
│   ├── FidoCredential.cs
│   └── *ViewModels.cs
├── Services/             # 業務服務
│   ├── IFidoCredentialStorage.cs
│   └── FidoCredentialStorage.cs
├── Views/                # 視圖範本
│   ├── Account/
│   ├── Home/
│   └── Shared/
└── wwwroot/              # 靜態資源
```

### 擴展開發

#### 添加新的認證方式
1. 實現 `IAuthenticationSchemeProvider`
2. 在 `Program.cs` 中註冊服務
3. 創建相應的控制器和視圖

#### 自訂 FIDO2 配置
1. 修改 `Program.cs` 中的 FIDO2 配置
2. 實現自訂的 `IFidoCredentialStorage`
3. 更新相關的視圖和 JavaScript

## 📝 常見問題

### Q: Windows Hello 註冊失敗
**A**: 請檢查：
- 設備是否支援 Windows Hello
- 流覽器是否支持 WebAuthn API
- 是否使用 HTTPS 協定訪問

### Q: 資料庫連接失敗
**A**: 請檢查：
- SQL Server 服務是否啟動
- 連接字串是否正確
- 是否有足夠的資料庫許可權

### Q: HTTPS 證書問題
**A**: 運行以下命令信任開發證書：
```dos command
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

## 📄 許可證

本專案採用 MIT 許可證，詳見 [LICENSE](LICENSE) 文件。

## 🤝 貢獻指南

歡迎提交 Issue 和 Pull Request 來改進本專案。

### 提交代碼前請確保：
1. 代碼符合專案編碼規範
2. 添加必要的單元測試
3. 更新相關文檔

## 📞 技術支援

如有技術問題，請通過以下方式聯繫：
- 提交 GitHub Issue
- 發送郵件至專案維護者

---

**注意**: 在生產環境中部署前，請務必：
1. 更改預設的資料庫連接字串
2. 配置適當的 CORS 策略
3. 啟用適當的日誌記錄
4. 配置監控和錯誤跟蹤 

