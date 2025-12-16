# Сигурност и авторизация

## Защита на Admin панела

Приложението използва многослойна защита за admin областта:

### 1. Session-based Authentication
- При вход служителите получават Session данни:
  - `UserId` - ID на служителя
  - `UserType` - "Employee" (важно за разграничаване от клиенти)
  - `UserName` - Име на служителя
  - `UserRole` - Роля (Admin, Manager и т.н.)

### 2. AdminAuthorizationAttribute (Action Filter)
Прилага се на всеки контролер в Admin областта:
```csharp
[AdminAuthorization]
public class EmployeesController : Controller
```

**Какво прави:**
- Проверява дали потребителят е логнат (`UserId` != null)
- Проверява дали е служител (`UserType` == "Employee")
- Ако не - пренасочва към `/Account/Login` с returnUrl

**Приложен на:**
- Всички MVC контролери в `Areas/Admin/Controllers/`
- Всички API контролери в `Areas/Admin/Controllers/Api/`

### 3. AdminAuthorizationMiddleware
Глобален middleware който проверява всеки HTTP заявка:

```csharp
app.UseMiddleware<AdminAuthorizationMiddleware>();
```

**Какво прави:**
- Проверява дали URL започва с `/admin` или `/api/admin`
- Ако да - проверява Session за `UserType` == "Employee"
- Ако не е валиден - пренасочва към Login

**Предимства:**
- Защитава дори статични файлове в admin областта
- Работи преди контролерите да бъдат извикани
- Допълнителен слой защита

### 4. Session Configuration
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

**Важни настройки:**
- `HttpOnly = true` - Предотвратява XSS атаки
- `IsEssential = true` - Cookie-то винаги се изпраща
- `IdleTimeout = 60min` - Автоматичен logout след 60 минути неактивност

## Как работи защитата:

1. **Неоторизиран потребител** опитва да отвори `/Admin/Employees`
   → Middleware го пренасочва към Login
   
2. **Клиент** (не служител) опитва да отвори admin панел
   → Проверката на `UserType` != "Employee" го блокира
   
3. **Служител** влиза успешно
   → Session се създава с правилни данни
   → Middleware го пропуска
   → Контролерите работят нормално

## Logout
```csharp
public IActionResult Logout()
{
    HttpContext.Session.Clear();
    return RedirectToAction("Index", "Home");
}
```

- Изчиства всички Session данни
- Пренасочва към началната страница

## Препоръки за Production:

1. **HTTPS винаги активно** - защитава Session cookies
2. **Сложни пароли** - BCrypt hash с cost factor 12+
3. **Rate limiting** на Login endpoint
4. **Logging** на неуспешни опити за вход
5. **Two-Factor Authentication** (бъдеща функционалност)

## Тестване на сигурността:

1. Отворете `/Admin/Employees` без да сте влезли
   → Трябва да ви пренасочи към Login
   
2. Влезте като клиент и опитайте да отворите `/Admin`
   → Трябва да бъдете блокиран
   
3. Влезте като служител
   → Трябва да имате достъп до целия admin панел

## Файлове за авторизация:

- `Filters/AdminAuthorizationAttribute.cs` - Action filter за контролери
- `Middleware/AdminAuthorizationMiddleware.cs` - Глобален middleware
- `Controllers/AccountController.cs` - Login/Logout логика
- `Program.cs` - Конфигурация на middleware и session
