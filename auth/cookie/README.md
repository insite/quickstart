# Cookie Authentication Demo

This is a very simple ASP.NET Core web application to demonstrate cookie-based authentication for API access. This
localhost-only tool helps developers test and debug a cookie authentication workflow with the Shift API server.

## Overview

This application provides a step-by-step example to show:
1. Obtaining authentication tokens from a web UI
2. Health checking API server connectivity
3. Generating an authentication cookie for localhost
4. Testing API endpoints that support cookie authentication

## Prerequisites

- .NET 10.0 or later
- A running Shift API server at `http://localhost:5000/`
- Access to the Shift iQ web UI at `https://local-demo.shiftiq.com`

## Project Structure

```
├── Program.cs              # Main application entry point
├── Example.csproj          # Project configuration (net10.0)
├── appsettings.json        # Application settings
├── wwwroot/
│   ├── index.html          # Main UI interface
│   ├── script.js           # JavaScript functionality
│   ├── styles.css          # Application styling
│   └── favicon.ico         # Favicon
```

## Getting Started

### 1. Clone and Run

```bash
# Navigate to project directory
cd auth/cookie

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The application starts on `http://localhost:5005` by default.

### 2. Authentication Workflow

Each step displays the fully-qualified Request URL it will call so you can see exactly what is being requested.

#### Step 1: Obtain Authentication Token
1. Navigate to `https://local-demo.shiftiq.com`
2. Log in with your email address
3. Open browser Developer Tools (F12)
4. Find the HTTP cookie named `InSite.WebToken`
5. Copy the cookie value
6. Return to `http://localhost:5005`

#### Step 2: Health Check
1. Click **Health Check** to confirm the API server is online
2. Calls `GET http://localhost:5000/api/diagnostic/health`

#### Step 3: Generate Cookie
1. Paste the authentication token from Step 1 into the textarea
2. Click **Request Cookie** to generate a localhost-scoped cookie
3. Calls `POST http://localhost:5000/api/security/cookies/generate`
4. Verify the cookie was created using Developer Tools

#### Step 4: Test API Endpoint
1. Click **Use Cookie** to call a protected endpoint with the new cookie
2. Calls `GET http://localhost:5000/api/security/cookies/introspect`
3. View the introspection report below the button

## API Endpoints Used

- **Health Check**: `GET /api/diagnostic/health`
- **Cookie Generation**: `POST /api/security/cookies/generate`
- **Cookie Introspection**: `GET /api/security/cookies/introspect`

## Configuration

### Base URL and Endpoint Paths
The API base URL and endpoint paths are defined as constants at the top of `wwwroot/script.js`:

```js
const BASE_URL = "http://localhost:5000/";
const HEALTH_PATH = "api/diagnostic/health";
const GENERATE_COOKIE_PATH = "api/security/cookies/generate";
const INTROSPECT_PATH = "api/security/cookies/introspect";
```

Edit these constants if your API server runs at a different address.

### Logging
Logging configuration is available in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Features

- **Step-by-step UI**: Guided workflow for authentication testing
- **Visible Request URLs**: Each step displays the fully-qualified URL it will call
- **Real-time feedback**: Status messages for each operation
- **JSON formatting**: Pretty-printed API responses
- **Responsive design**: Clean, modern interface using Inter font
- **Error handling**: Comprehensive error messages and HTTP status codes

## Security Notes

- **Localhost only**: This tool is designed for local development only
- **Token handling**: Authentication tokens are processed client-side
- **Cookie scope**: Generated cookies are scoped to localhost domain
- **Development use**: Not intended for production environments

## Troubleshooting

### Common Issues

1. **API Server Offline**
   - Verify the API server is running at `http://localhost:5000/`
   - Check firewall and network connectivity

2. **Authentication Token Invalid**
   - Ensure the token is copied correctly from the web UI
   - Verify the token hasn't expired
   - Re-authenticate if necessary

3. **Cookie Generation Failed**
   - Check that the authentication token is valid
   - Ensure the API server accepts the token format
   - Verify CORS settings if applicable

4. **Cookie Introspection Failed (401)**
   - Confirm the cookie was generated successfully
   - Check that cookies are enabled in your browser
   - Verify the cookie hasn't expired

## Development

### Building
```bash
dotnet build
```

### Running in Development Mode
```bash
dotnet run --environment Development
```

### Static Files
Static files are served from the `wwwroot` directory and include:
- HTML interface (`index.html`)
- CSS styling (`styles.css`)
- JavaScript functionality (`script.js`)
- Favicon (`favicon.ico`)
- Sample JSON data (`sample.json`)

## Dependencies

- **ASP.NET Core 10.0**: Web framework
- **Static Files Middleware**: For serving HTML/CSS/JS
- **Default Files Middleware**: For serving index.html

## License

This project is intended only for development and testing purposes.
