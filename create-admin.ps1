# Create Admin User Script
# This script creates a new admin user via the API

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Create Admin User for Shop Hub" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Get user input
$userName = Read-Host "Enter admin name (e.g., John Doe)"
$email = Read-Host "Enter admin email (e.g., admin@shophub.com)"
$password = Read-Host "Enter password" -AsSecureString
$passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
$phone = Read-Host "Enter phone number (e.g., 1234567890)"
$address = Read-Host "Enter address (optional)" 

if ([string]::IsNullOrWhiteSpace($address)) {
    $address = "Admin Office"
}

Write-Host ""
Write-Host "Creating admin user..." -ForegroundColor Yellow

# Create request body
$body = @{
    userName = $userName
    address = $address
    phone = $phone
    email = $email
    password = $passwordPlain
    userTypeID = 1  # 1 = Admin
    isActive = $true
} | ConvertTo-Json

try {
    # Make API request
    $response = Invoke-RestMethod -Uri "https://localhost:7077/api/User" `
        -Method Post `
        -Body $body `
        -ContentType "application/json" `
        -SkipCertificateCheck

    Write-Host ""
    Write-Host "✓ Admin user created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Login Details:" -ForegroundColor Cyan
    Write-Host "  Email: $email" -ForegroundColor White
    Write-Host "  Password: [hidden]" -ForegroundColor White
    Write-Host "  User Type: Admin" -ForegroundColor White
    Write-Host ""
    Write-Host "You can now login at: http://localhost:3000/login" -ForegroundColor Yellow
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "✗ Error creating admin user:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "  1. Backend is running on https://localhost:7077" -ForegroundColor White
    Write-Host "  2. Email is not already registered" -ForegroundColor White
    Write-Host "  3. All required fields are filled" -ForegroundColor White
    Write-Host ""
}

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
