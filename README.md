# 🤖 qBittorrent Telegram Bot

Легковесный бот для дистанционного управления вашим qBittorrent сервером через Telegram.

## ✨ Основные возможности

*   📥 **Добавление торрентов:** Поддержка .torrent файлов и magnet ссылок.
*   🏷 **Категории:** Установка категории для торрента.
*   🗑 **Удаление:** Возможность удалить торрент вместе с загруженными файлами.
*   🔔 **Уведомления:** Мгновенные оповещения о старте и завершении загрузки.

---

## ⚙️ Установка

### Вариант 1: Docker (Рекомендуется)
1. Создайте файл `.env` рядом с `docker-compose.yml` (используйте `ref.env` как шаблон):
   ```env
   TORRENTHOST=http://host.docker.internal:8080
   BOTAPI=ваш_токен_бота
   ADMIN=ваш_chat_id
   ```
2. Запустите контейнер:
   ```bash
   sudo docker-compose up -d --build
   ```
> [!NOTE]  
> При использовании Docker Compose порт **6060** пробрасывается автоматически.

### Вариант 2: Локальный запуск (без Docker)
1. Создайте системные переменные окружения (`TORRENTHOST`, `BOTAPI`, `ADMIN`).
2. Запустите проект:
   ```bash
   dotnet run
   ```
> [!IMPORTANT]  
> В этом режиме бот использует стандартный порт .NET приложения (обычно 5000/5001), если не указано иное.

---

## 🔔 Настройка уведомлений в qBittorrent

Чтобы бот отправлял уведомления, необходимо настроить выполнение команд в клиенте qBittorrent (Сервис -> Настройки -> Загрузки -> Запуск внешней программы).

### Для Linux / macOS (через curl)
*   **При добавлении торрента:**
    ```bash
    curl -X POST "http://localhost:6060/api/start" -H "Content-Type: text/plain" -d "%I"
    ```
*   **При завершении загрузки:**
    ```bash
    curl -X POST "http://localhost:6060/api/complete" -H "Content-Type: text/plain" -d "%I"
    ```

### Для Windows (через PowerShell)
*   **При добавлении торрента:**
    ```powershell
    powershell -command "Invoke-RestMethod -Method Post -Uri 'http://localhost:6060/api/start' -Body '%I' -ContentType 'text/plain'"
    ```
*   **При завершении загрузки:**
    ```powershell
    powershell -command "Invoke-RestMethod -Method Post -Uri 'http://localhost:6060/api/complete' -Body '%I' -ContentType 'text/plain'"
    ```

> [!TIP]
> Если бот запущен **не** в Docker, замените порт `6060` на стандартный порт вашего .NET приложения.

---

## 📋 Конфигурация

| Переменная | Описание |
| :--- | :--- |
| `TORRENTHOST` | URL Web UI qBittorrent (например, http://192.168.1.10:8080) |
| `BOTAPI` | API токен бота от @BotFather |
| `ADMIN` | Ваш числовой Telegram ID |

---
*Разработано для автоматизации домашнего медиасервера.*
