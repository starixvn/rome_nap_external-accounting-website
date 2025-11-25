# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Fanex.ExternalAccount.Reports.csproj", "."]
COPY ["../../Shared/Core/Core.csproj", "../../Shared/Core/"]
COPY ["../../Shared/Fanex.ExternalAccount.Controllers/Fanex.ExternalAccount.Controllers.csproj", "../../Shared/Fanex.ExternalAccount.Controllers/"]
COPY ["../../Shared/Fanex.ExternalAccount.Core.Shared/Fanex.ExternalAccount.Core.Shared.csproj", "../../Shared/Fanex.ExternalAccount.Core.Shared/"]
COPY ["../../Shared/Fanex.ExternalAccount.DataAccess/Fanex.ExternalAccount.DataAccess.csproj", "../../Shared/Fanex.ExternalAccount.DataAccess/"]
COPY ["../../Shared/Fanex.ExternalAccount.DTO/Fanex.ExternalAccount.DTO.csproj", "../../Shared/Fanex.ExternalAccount.DTO/"]
COPY ["../../Shared/Fanex.ExternalAccount.Excel/Fanex.ExternalAccount.Excel.csproj", "../../Shared/Fanex.ExternalAccount.Excel/"]
COPY ["../../Shared/Fanex.ExternalAccount.Resources/Fanex.ExternalAccount.Resources.csproj", "../../Shared/Fanex.ExternalAccount.Resources/"]
COPY ["../../Shared/Fanex.ExternalAccount.Service/Fanex.ExternalAccount.Service.csproj", "../../Shared/Fanex.ExternalAccount.Service/"]
COPY ["../../Shared/Fanex.ExternalAccount.Utility/Fanex.ExternalAccount.Utilities.csproj", "../../Shared/Fanex.ExternalAccount.Utility/"]
COPY ["../../Shared/Fanex.ExternalAccount.ViewModel/Fanex.ExternalAccount.ViewModel.csproj", "../../Shared/Fanex.ExternalAccount.ViewModel/"]
COPY ["../../Shared/Modules/Fanex.ExternalAccount.MatchWinLossReports/Fanex.ExternalAccount.MatchWinLossReports.csproj", "../../Shared/Modules/Fanex.ExternalAccount.MatchWinLossReports/"]
COPY ["../../Shared/Modules/Fanex.ExternalAccount.Statement/Fanex.ExternalAccount.Statement.csproj", "../../Shared/Modules/Fanex.ExternalAccount.Statement/"]
COPY ["../../Shared/Modules/Fanex.ExternalAccount.WhiteLabelWinLossReports/Fanex.ExternalAccount.WhiteLabelWinLossReports.csproj", "../../Shared/Modules/Fanex.ExternalAccount.WhiteLabelWinLossReports/"]
COPY ["../../Shared/Modules/Fanex.ExternalAccount.WinLossDetailBySubGroup/Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.csproj", "../../Shared/Modules/Fanex.ExternalAccount.WinLossDetailBySubGroup/"]

RUN dotnet restore "Fanex.ExternalAccount.Reports.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "Fanex.ExternalAccount.Reports.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Fanex.ExternalAccount.Reports.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for logs and temp files
RUN mkdir -p /app/logs /tmp

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Fanex.ExternalAccount.Reports.dll"]
