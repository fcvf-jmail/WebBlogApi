# Ступень сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Устанавливаем dotnet-ef для создания и применения миграций
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем весь проект, включая appsettings.json и папку Data
COPY . ./

# Создаем миграции (если их нет)
RUN dotnet ef migrations add InitialCreate --context BlogService.API.Data.BlogDbContext --output-dir Migrations

# Применяем миграции (для проверки, можно убрать, если применяем в runtime)
# RUN dotnet ef database update --context BlogService.API.Data.BlogDbContext

# Публикуем приложение
RUN dotnet publish -c Release -o out

# Ступень выполнения
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
COPY --from=build /app/appsettings.json ./

# Указываем порт
ENV ASPNETCORE_URLS=http://+:5107
EXPOSE 5107

# Запускаем приложение
ENTRYPOINT ["dotnet", "WebBlog.dll"]