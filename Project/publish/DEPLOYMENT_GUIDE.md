# BIDMOTORS - Deployment Guide

## 📦 Какво имате

Папката `publish` съдържа всички необходими файлове за хостване на приложението.

## 🔧 Преди хостване

### 1. Конфигурация на Connection String

Редактирайте `appsettings.json` и променете connection string-а:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=BIDMOTORSDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

### 2. Email конфигурация (optional)

Ако искате email известия, променете:

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "FromEmail": "your-email@gmail.com",
  "FromPassword": "your-app-password",
  "AdminEmail": "admin@bidmotors.com"
}
```

### 3. Database Migration

След качване на сървъра, трябва да създадете базата данни:

```bash
dotnet ef database update
```

Или можете да използвате SQL скриптове за миграция.

## 🌐 Опции за хостване

### Вариант 1: Windows Server с IIS

1. **Инсталирайте .NET 9.0 Runtime** на сървъра
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0

2. **Инсталирайте IIS Hosting Bundle**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0

3. **Конфигурирайте IIS:**
   - Отворете IIS Manager
   - Създайте нов Application Pool:
     - Name: BIDMOTORSPool
     - .NET CLR Version: No Managed Code
   - Създайте нов Website:
     - Site name: BIDMOTORS
     - Physical path: C:\inetpub\wwwroot\BIDMOTORS (копирайте съдържанието на publish тук)
     - Application Pool: BIDMOTORSPool
     - Binding: Port 80 или 443 (за HTTPS)

4. **Permissions:**
   - Дайте на IIS_IUSRS група пълни права върху папката

### Вариант 2: Azure App Service

1. Създайте Azure App Service (Windows, .NET 9)
2. Качете папката `publish` през Visual Studio, VS Code или Azure CLI:
   ```bash
   az webapp up --name bidmotors --resource-group YourResourceGroup
   ```
3. Конфигурирайте Connection String в Azure Portal → Configuration → Connection strings

### Вариант 3: Linux Server

1. **Инсталирайте .NET 9.0 Runtime:**
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   sudo chmod +x dotnet-install.sh
   ./dotnet-install.sh --channel 9.0 --runtime aspnetcore
   ```

2. **Копирайте файловете:**
   ```bash
   scp -r ./publish/* user@server:/var/www/bidmotors/
   ```

3. **Създайте systemd service** (`/etc/systemd/system/bidmotors.service`):
   ```ini
   [Unit]
   Description=BIDMOTORS ASP.NET Core App

   [Service]
   WorkingDirectory=/var/www/bidmotors
   ExecStart=/usr/bin/dotnet /var/www/bidmotors/Project.dll
   Restart=always
   RestartSec=10
   SyslogIdentifier=bidmotors
   User=www-data
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

   [Install]
   WantedBy=multi-user.target
   ```

4. **Стартирайте службата:**
   ```bash
   sudo systemctl enable bidmotors
   sudo systemctl start bidmotors
   ```

5. **Nginx reverse proxy** (`/etc/nginx/sites-available/bidmotors`):
   ```nginx
   server {
       listen 80;
       server_name your-domain.com;
       
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
           proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
           proxy_set_header X-Forwarded-Proto $scheme;
       }
   }
   ```

### Вариант 4: Docker

1. Създайте `Dockerfile` в root на проекта:
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:9.0
   WORKDIR /app
   COPY ./publish .
   EXPOSE 80
   ENTRYPOINT ["dotnet", "Project.dll"]
   ```

2. Build и run:
   ```bash
   docker build -t bidmotors .
   docker run -d -p 80:80 --name bidmotors bidmotors
   ```

## 📋 Checklist преди production

- [ ] Променен Connection String за production database
- [ ] Email конфигурация настроена (ако се използва)
- [ ] Database миграция изпълнена
- [ ] HTTPS сертификат настроен
- [ ] Admin акаунт създаден (email: admin@bidmotors.com, парола: ще трябва да смените първоначалната)
- [ ] Backup стратегия за базата данни
- [ ] Логове конфигурирани (проверете wwwroot и други папки)
- [ ] Firewall правила (отворете порт 80/443)

## 🔐 Сигурност

### Важни стъпки:

1. **Сменете Admin паролата** веднага след първи вход
2. **Използвайте HTTPS** - никога HTTP в production
3. **Connection String** - пазете в безопасно място (Azure Key Vault, Environment Variables)
4. **CORS Policy** - ограничете в production ако е нужно

## 📊 Database

### SQL Server Requirements:
- SQL Server 2016 или по-нов
- Или Azure SQL Database

### Първоначални данни:
При първо стартиране автоматично се създава admin акаунт:
- Email: `admin@bidmotors.com`
- Парола: Проверете в кода или сменете след вход

## 🆘 Troubleshooting

### Грешка: "Unable to connect to database"
- Проверете Connection String
- Проверете firewall правила на SQL Server
- Уверете се че User има достъп до базата

### Грешка: "500 Internal Server Error"
- Проверете логовете в Event Viewer (Windows) или `/var/log/` (Linux)
- Включете detailed errors временно за debug

### SignalR не работи
- Проверете че WebSockets са enabled в IIS
- Проверете firewall rules

## 📞 Support

За въпроси относно deployment, моля обърнете се към разработчика.

---

**Version:** 1.0  
**Last Updated:** December 2025
