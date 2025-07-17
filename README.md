# Windows Hello Authentication System

A modern authentication system based on ASP.NET Core 9.0, integrating Windows Hello and FIDO2/WebAuthn technologies to provide secure and convenient passwordless authentication experience.

## 🚀 Project Features

### Core Functionality
- **Traditional Authentication**: Support for username/password registration and login
- **Windows Hello Authentication**: Secure authentication using biometric technology
- **FIDO2/WebAuthn Support**: International standard compliant passwordless authentication
- **Multi-Factor Authentication**: Support for multiple authentication methods
- **Secure Storage**: Safe credential storage using Entity Framework Core

### Technology Stack
- **Framework**: ASP.NET Core 9.0
- **Authentication**: ASP.NET Core Identity + FIDO2
- **Database**: SQL Server + Entity Framework Core
- **Frontend**: Bootstrap + jQuery
- **Security**: HTTPS + Cookie Policy + Anti-Forgery Tokens

## 📋 System Requirements

### Runtime Environment
- **.NET 9.0** or higher
- **SQL Server** (local or remote instance)
- **Windows 10/11** (recommended for Windows Hello functionality)
- **Modern Browsers**: Chrome 67+, Firefox 60+, Safari 14+, Edge 18+

### Hardware Requirements
- **Windows Hello** compatible device (fingerprint reader, camera, or TPM)
- Or **FIDO2** compatible security key

## 🛠️ Installation & Configuration

### 1. Clone Repository
```dos command
git clone https://github.com/hankwu61/WindowsHelloAuth.git
cd WindowsHelloAuth
```

### 2. Configure Database Connection

Edit the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\sqlexpress;Database=WindowsHelloAuthDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

**Connection String Parameters:**
- `Server=.\\sqlexpress`: SQL Server Express local instance
- `Database=WindowsHelloAuthDb`: Database name
- `Trusted_Connection=true`: Use Windows authentication
- `TrustServerCertificate=true`: Trust server certificate

### 3. Install Dependencies
```dos command
dotnet restore
```

### 4. Create Database
```dos command
dotnet ef database update
```

If EF tools are not installed, install them first:
```dos command
dotnet tool install --global dotnet-ef
```

### 5. Configure HTTPS Certificate
```dos command
dotnet dev-certs https --trust
```

## 🚀 Running the Application

### Development Environment
```dos command
dotnet run
```

The application will start on the following addresses:
- **HTTP**: http://localhost:5017
- **HTTPS**: https://localhost:5001 (recommended)

### Production Deployment
```dos command
dotnet publish -c Release -o ./publish
```

## 📚 User Guide

### User Registration Flow

#### 1. Traditional Registration
1. Visit `/Account/Register`
2. Fill in email, display name, and password
3. Submit the form to complete registration
4. Automatically log in and redirect to homepage

#### 2. Windows Hello Registration
1. Complete traditional registration and log into the system
2. Visit `/Account/WindowsHelloRegister`
3. Click "Register Windows Hello" button
4. Follow browser prompts to complete biometric setup
5. After successful registration, you can use Windows Hello to log in

### User Login Flow

#### 1. Traditional Login
1. Visit `/Account/Login`
2. Enter email and password
3. Click "Login" button

#### 2. Windows Hello Login
1. Visit `/Account/Login`
2. Click "Login with Windows Hello" button
3. Follow prompts to complete biometric verification
4. Automatically log in successfully

### Feature Pages

- **Homepage**: `/` - Application main page
- **User Profile**: `/Home/Profile` - View user information and registered authenticators
- **Privacy Policy**: `/Home/Privacy` - Privacy policy page

## 🔧 Configuration Guide

### FIDO2 Configuration

Configure FIDO2 parameters in `appsettings.json`:

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

**Parameter Description:**
- `ServerDomain`: Server domain name
- `ServerName`: Service name displayed to users
- `Origins`: List of allowed origin domains
- `TimestampDriftTolerance`: Timestamp tolerance (milliseconds)

### Authentication Configuration

The system uses ASP.NET Core Identity with password policy:
- Minimum 8 characters
- Must contain uppercase letters
- Must contain lowercase letters
- Must contain digits
- Must contain special characters

### Session Configuration
- Session timeout: 30 minutes
- Cookie security policy: HTTPS only
- Anti-forgery token protection

## 🔐 Security Features

### Data Protection
- **HTTPS Enforcement**: All communications encrypted via HTTPS
- **Cookie Security**: HttpOnly, Secure, SameSite policies
- **Anti-Forgery Protection**: CSRF attack protection
- **Password Encryption**: Uses ASP.NET Core Identity password hashing

### FIDO2 Security
- **Local Verification**: Biometric data not transmitted to server
- **Public Key Cryptography**: Uses asymmetric encryption technology
- **Replay Protection**: Built-in challenge-response mechanism
- **Device Binding**: Credentials bound to specific devices

## 🌐 API Endpoints

### FIDO2 API Endpoints

#### Registration Flow
```
POST /api/Fido2/makeCredentialOptions
POST /api/Fido2/makeCredential
```

#### Authentication Flow
```
POST /api/Fido2/assertionOptions
POST /api/Fido2/makeAssertion
```

#### Credential Management
```
GET /api/Fido2/credentials/{userId}
DELETE /api/Fido2/credentials/{credentialId}
```

## 🛠️ Development Guide

### Project Structure
```
WindowsHelloAuth/
├── Controllers/          # Controllers
│   ├── AccountController.cs
│   ├── Fido2Controller.cs
│   └── HomeController.cs
├── Models/               # Data Models
│   ├── ApplicationUser.cs
│   ├── FidoCredential.cs
│   └── *ViewModels.cs
├── Services/             # Business Services
│   ├── IFidoCredentialStorage.cs
│   └── FidoCredentialStorage.cs
├── Views/                # View Templates
│   ├── Account/
│   ├── Home/
│   └── Shared/
└── wwwroot/              # Static Resources
```

### Extension Development

#### Adding New Authentication Methods
1. Implement `IAuthenticationSchemeProvider`
2. Register services in `Program.cs`
3. Create corresponding controllers and views

#### Custom FIDO2 Configuration
1. Modify FIDO2 configuration in `Program.cs`
2. Implement custom `IFidoCredentialStorage`
3. Update related views and JavaScript

## 📝 Troubleshooting

### Q: Windows Hello registration fails
**A**: Please check:
- Does the device support Windows Hello?
- Does the browser support WebAuthn API?
- Are you accessing via HTTPS protocol?

### Q: Database connection fails
**A**: Please check:
- Is SQL Server service running?
- Is the connection string correct?
- Do you have sufficient database permissions?

### Q: HTTPS certificate issues
**A**: Run the following commands to trust development certificates:
```dos command
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Welcome to submit Issues and Pull Requests to improve this project.

### Before submitting code, please ensure:
1. Code follows project coding standards
2. Add necessary unit tests
3. Update relevant documentation

## 📞 Technical Support

For technical questions, please contact via:
- Submit GitHub Issues
- Email project maintainers

---

**Note**: Before deploying to production environment, please ensure:
1. Change default database connection string
2. Configure appropriate CORS policies
3. Enable appropriate logging
4. Configure monitoring and error tracking 