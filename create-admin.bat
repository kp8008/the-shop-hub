@echo off
echo ========================================
echo   Create Admin User for The Shop Hub
echo ========================================
echo.
echo This script will help you create a new admin user.
echo Make sure your backend is running on https://localhost:7077
echo.
pause
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0create-admin.ps1"
