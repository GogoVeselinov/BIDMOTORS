# 🔧 TROUBLESHOOTING - HTTP Error 500.30

## Стъпки за решаване на проблема:

### 1. ✅ Проверете дали .NET 9.0 Runtime е инсталиран

**На сървъра отворете CMD/PowerShell и изпълнете:**
```bash
dotnet --list-runtimes
```

**Трябва да видите:**
```
Microsoft.AspNetCore.App 9.0.x
Microsoft.NETCore.App 9.0.x
```

**Ако липсва, инсталирайте:**
- Download: https://dotnet.microsoft.com/download/dotnet/9.0
- Изберете: **ASP.NET Core Runtime 9.0.x - Windows Hosting Bundle**

### 2. ✅ Поправете Connection String

**Проблем:** `appsettings.json` използва `(localdb)` което НЕ работи на production сървър!

**Решение:**

Редактирайте `appsettings.Production.json` (създаден е в publish папката):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=BidMotorsDb;User Id=YOUR_USER;Password=YOUR_PASS;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Заменете:**
- `YOUR_SQL_SERVER` → IP адрес или име на SQL Server (например: `localhost` или `192.168.1.100` или `sqlserver.example.com`)
- `YOUR_USER` → SQL Server username (например: `sa` или създаден user)
- `YOUR_PASS` → SQL Server password

**Ако използвате Windows Authentication:**
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=BidMotorsDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 3. ✅ Проверете логовете

**Включен е stdout logging в web.config!**

Отидете в папката: `C:\inetpub\wwwroot\BIDMOTORS\logs\`

Отворете последния `stdout_*.log` файл - там ще видите **точната грешка**!

### 4. ✅ Създайте базата данни

**Ако базата данни не съществува:**

**Вариант А - SQL Management Studio:**
1. Отворете SQL Server Management Studio
2. Connect към сървъра
3. New Query
4. Изпълнете:
```sql
CREATE DATABASE BidMotorsDb;
```

**Вариант Б - От приложението:**

В publish папката отворете PowerShell и изпълнете:
```bash
dotnet ef database update
```

**⚠️ ВАЖНО:** Ако нямате `dotnet ef`, инсталирайте го:
```bash
dotnet tool install --global dotnet-ef
```

### 5. ✅ Permissions в IIS

**Дайте права на IIS потребителя:**

1. Отворете папката `C:\inetpub\wwwroot\BIDMOTORS`
2. Right-click → Properties → Security tab
3. Edit → Add
4. Въведете: `IIS_IUSRS`
5. Check Names → OK
6. Дайте **Full Control**
7. Apply → OK

### 6. ✅ Application Pool настройки

**В IIS Manager:**

1. Application Pools → BIDMOTORSPool (или каквото се казва вашият pool)
2. Advanced Settings
3. Проверете:
   - **.NET CLR Version:** `No Managed Code` ✓
   - **Identity:** ApplicationPoolIdentity ✓
   - **Start Mode:** AlwaysRunning
   - **Enable 32-Bit Applications:** False ✓

### 7. ✅ Restart на всичко

```bash
# Рестартирайте Application Pool
iisreset

# Или само вашия pool:
# В IIS Manager → Application Pools → 
# Right-click BIDMOTORSPool → Recycle
```

---

## 🔍 Най-чести грешки и решения:

### Грешка: "Could not load file or assembly"
**Причина:** Липсва .NET Runtime  
**Решение:** Инсталирайте ASP.NET Core Hosting Bundle

### Грешка: "A connection was successfully established with the server, but then an error occurred"
**Причина:** Connection String използва localdb  
**Решение:** Сменете на истински SQL Server connection string

### Грешка: "Login failed for user"
**Причина:** Грешен username/password или липсват permissions  
**Решение:** 
1. Проверете username/password
2. В SQL Server дайте права на потребителя:
```sql
CREATE LOGIN [YOUR_USER] WITH PASSWORD = 'YOUR_PASSWORD';
USE BidMotorsDb;
CREATE USER [YOUR_USER] FOR LOGIN [YOUR_USER];
ALTER ROLE db_owner ADD MEMBER [YOUR_USER];
```

### Грешка: "Cannot open database 'BidMotorsDb'"
**Причина:** Базата данни не съществува  
**Решение:** Създайте я (виж стъпка 4 по-горе)

### Грешка: "Access is denied"
**Причина:** IIS няма права  
**Решение:** Дайте права на IIS_IUSRS (виж стъпка 5)

---

## 📋 Checklist за проверка:

- [ ] .NET 9.0 Runtime инсталиран
- [ ] ASP.NET Core Hosting Bundle инсталиран
- [ ] Connection String променен (НЕ localdb!)
- [ ] SQL Server working и достъпен
- [ ] База данни `BidMotorsDb` създадена
- [ ] Database migrations изпълнени
- [ ] IIS_IUSRS има права върху папката
- [ ] Application Pool използва "No Managed Code"
- [ ] web.config има правилен processPath
- [ ] Логовете enabled (stdoutLogEnabled="true")
- [ ] IIS рестартиран

---

## 🆘 Все още не работи?

**Изпратете ми съдържанието на:**
1. Файл от `logs\stdout_*.log` (най-новия)
2. Screenshot на грешката в браузъра
3. Connection String (без паролата!)
4. Резултат от `dotnet --list-runtimes`

**За бърза помощ:**
- Проверете Event Viewer → Windows Logs → Application
- Търсете грешки свързани с IIS или ASP.NET Core

---

**Успех!** 🚀
