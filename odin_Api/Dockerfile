# 1. Base Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy ONLY the project files (No .slnx needed!)
COPY ["odin_Domain/odin_Domain.csproj", "odin_Domain/"]
COPY ["odin_Application/odin_Application.csproj", "odin_Application/"]
COPY ["odin_Infrastructure/odin_Infrastructure.csproj", "odin_Infrastructure/"]
COPY ["odin_Api/odin_Api.csproj", "odin_Api/"]

# Restore by pointing directly to the API project (it will automatically find the others)
RUN dotnet restore "odin_Api/odin_Api.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/odin_Api"
RUN dotnet build "odin_Api.csproj" -c Release -o /app/build

# 3. Publish Stage
FROM build AS publish
RUN dotnet publish "odin_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final Stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "odin_Api.dll"]