@echo off
echo ========================================
echo  The Shop Hub - Setup Script
echo ========================================
echo.

echo [1/3] Installing dependencies...
call npm install

if %errorlevel% neq 0 (
    echo.
    echo ERROR: Failed to install dependencies
    echo Please check your internet connection and try again
    pause
    exit /b 1
)

echo.
echo [2/3] Checking configuration...
if not exist ".env" (
    echo Creating .env file from .env.example...
    copy .env.example .env
)

echo.
echo [3/3] Setup complete!
echo.
echo ========================================
echo  Next Steps:
echo ========================================
echo.
echo 1. Make sure your .NET backend is running on http://localhost:5000
echo 2. Run: npm run dev
echo 3. Open: http://localhost:3000
echo.
echo For detailed instructions, see INSTALLATION.md
echo.
pause
