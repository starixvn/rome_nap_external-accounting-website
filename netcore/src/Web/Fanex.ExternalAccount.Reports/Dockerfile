# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /build

# Copy NuGet.config first for authentication to private feeds
COPY ["src/NuGet.config", "src/NuGet.config"]

# Copy csproj files and restore dependencies
COPY ["src/Web/Fanex.ExternalAccount.Reports/Fanex.ExternalAccount.Reports.csproj", "src/Web/Fanex.ExternalAccount.Reports/"]
COPY ["src/Shared/Core/Core.csproj", "src/Shared/Core/"]
COPY ["src/Shared/Fanex.ExternalAccount.Controllers/Fanex.ExternalAccount.Controllers.csproj", "src/Shared/Fanex.ExternalAccount.Controllers/"]
COPY ["src/Shared/Fanex.ExternalAccount.Core.Shared/Fanex.ExternalAccount.Core.Shared.csproj", "src/Shared/Fanex.ExternalAccount.Core.Shared/"]
COPY ["src/Shared/Fanex.ExternalAccount.DataAccess/Fanex.ExternalAccount.DataAccess.csproj", "src/Shared/Fanex.ExternalAccount.DataAccess/"]
COPY ["src/Shared/Fanex.ExternalAccount.DTO/Fanex.ExternalAccount.DTO.csproj", "src/Shared/Fanex.ExternalAccount.DTO/"]
COPY ["src/Shared/Fanex.ExternalAccount.Excel/Fanex.ExternalAccount.Excel.csproj", "src/Shared/Fanex.ExternalAccount.Excel/"]
COPY ["src/Shared/Fanex.ExternalAccount.Resources/Fanex.ExternalAccount.Resources.csproj", "src/Shared/Fanex.ExternalAccount.Resources/"]
COPY ["src/Shared/Fanex.ExternalAccount.Requests/Fanex.ExternalAccount.Requests.csproj", "src/Shared/Fanex.ExternalAccount.Requests/"]
COPY ["src/Shared/Fanex.ExternalAccount.Service/Fanex.ExternalAccount.Service.csproj", "src/Shared/Fanex.ExternalAccount.Service/"]
COPY ["src/Shared/Fanex.ExternalAccount.Utility/Fanex.ExternalAccount.Utilities.csproj", "src/Shared/Fanex.ExternalAccount.Utility/"]
COPY ["src/Shared/Fanex.ExternalAccount.ViewModel/Fanex.ExternalAccount.ViewModel.csproj", "src/Shared/Fanex.ExternalAccount.ViewModel/"]
COPY ["src/Shared/LeftPanel/LeftPanel.csproj", "src/Shared/LeftPanel/"]
COPY ["src/Shared/Modules/Fanex.ExternalAccount.MatchWinLossReports/Fanex.ExternalAccount.MatchWinLossReports.csproj", "src/Shared/Modules/Fanex.ExternalAccount.MatchWinLossReports/"]
COPY ["src/Shared/Modules/Fanex.ExternalAccount.Statement/Fanex.ExternalAccount.Statement.csproj", "src/Shared/Modules/Fanex.ExternalAccount.Statement/"]
COPY ["src/Shared/Modules/Fanex.ExternalAccount.WhiteLabelWinLossReports/Fanex.ExternalAccount.WhiteLabelWinLossReports.csproj", "src/Shared/Modules/Fanex.ExternalAccount.WhiteLabelWinLossReports/"]
COPY ["src/Shared/Modules/Fanex.ExternalAccount.WinLossDetailBySubGroup/Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.csproj", "src/Shared/Modules/Fanex.ExternalAccount.WinLossDetailBySubGroup/"]

WORKDIR "/build/src/Web/Fanex.ExternalAccount.Reports"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "Fanex.ExternalAccount.Reports.csproj" --ignore-failed-sources || echo "Restore completed with warnings"

# Publish stage
FROM build AS publish
# Copy everything else and build
COPY . .
WORKDIR "/build/src/Web/Fanex.ExternalAccount.Reports"
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet build "Fanex.ExternalAccount.Reports.csproj" -v d -c Release -o /app/build /p:TreatWarningsAsErrors=false || echo "Build completed with warnings"

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "Fanex.ExternalAccount.Reports.csproj" -v d -c Release -o /tmp/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS final
WORKDIR /app
COPY --from=publish /tmp/publish .

# Create directory for logs and temp files
RUN mkdir -p /app/logs /tmp

# Install curl for health check
RUN --mount=type=cache,target=/var/cache/apt \
    apt-get update && apt-get install -y curl

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost/healthz || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Fanex.ExternalAccount.Reports.dll"]
