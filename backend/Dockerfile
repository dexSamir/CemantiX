FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SemantiX.Domain/SemantiX.Domain.csproj", "SemantiX.Domain/"]
COPY ["src/SemantiX.Application/SemantiX.Application.csproj", "SemantiX.Application/"]
COPY ["src/SemantiX.Infrastructure/SemantiX.Infrastructure.csproj", "SemantiX.Infrastructure/"]
COPY ["src/SemantiX.WebAPI/SemantiX.WebAPI.csproj", "SemantiX.WebAPI/"]
RUN dotnet restore "SemantiX.WebAPI/SemantiX.WebAPI.csproj"

COPY src/ .
WORKDIR "/src/SemantiX.WebAPI"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "SemantiX.WebAPI.dll"]
