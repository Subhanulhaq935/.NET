@echo off
echo ========================================
echo SkillHub Application - Quick Start
echo ========================================
echo.

echo Step 1: Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo Error restoring packages!
    pause
    exit /b 1
)

echo.
echo Step 2: Checking for EF Core tools...
dotnet ef --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Installing EF Core tools...
    dotnet tool install --global dotnet-ef
)

echo.
echo Step 3: Creating database migration (if needed)...
if not exist "Migrations" (
    echo Creating initial migration...
    dotnet ef migrations add InitialCreate
)

echo.
echo Step 4: Updating database...
dotnet ef database update
if %errorlevel% neq 0 (
    echo Error updating database! Check your connection string.
    pause
    exit /b 1
)

echo.
echo Step 5: Starting the application...
echo.
echo ========================================
echo Application is starting...
echo Open your browser to: https://localhost:5001
echo ========================================
echo.
echo Default Login:
echo   Admin: admin@skillhub.com / Admin@123
echo   Provider: provider@skillhub.com / Provider@123
echo   Customer: customer@skillhub.com / Customer@123
echo.
echo Press Ctrl+C to stop the application
echo.

dotnet run





