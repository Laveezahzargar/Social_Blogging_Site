
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY ["AI_Blog_Generator/BlogGenerator/BlogGenerator/BlogGenerator.csproj", "AI_Blog_Generator/BlogGenerator/BlogGenerator/"]

# Restore dependencies
RUN dotnet restore "AI_Blog_Generator/BlogGenerator/BlogGenerator/BlogGenerator.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/AI_Blog_Generator/BlogGenerator/BlogGenerator"
RUN dotnet publish "BlogGenerator.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render provides the PORT environment variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "BlogGenerator.dll"]

