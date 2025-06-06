FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/VanManager.API/VanManager.API.csproj", "src/VanManager.API/"]
COPY ["src/VanManager.Infrastructure/VanManager.Infrastructure.csproj", "src/VanManager.Infrastructure/"]
COPY ["src/VanManager.Application/VanManager.Application.csproj", "src/VanManager.Application/"]
COPY ["src/VanManager.Domain/VanManager.Domain.csproj", "src/VanManager.Domain/"]
RUN dotnet restore "src/VanManager.API/VanManager.API.csproj"
COPY . .
WORKDIR "/src/src/VanManager.API"
RUN dotnet build "VanManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VanManager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VanManager.API.dll"]