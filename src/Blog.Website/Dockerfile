# ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER app
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS='80'

# ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /build
COPY ["src/Blog.Website/Blog.Website.csproj", "./src/Blog.Website/"]
RUN dotnet restore './src/Blog.Website/Blog.Website.csproj'
COPY . .
RUN dotnet build './src/Blog.Website/Blog.Website.csproj' -c $BUILD_CONFIGURATION -o /app/build

# ---
FROM build AS publish

ARG BUILD_CONFIGURATION=Release
RUN dotnet publish './src/Blog.Website/Blog.Website.csproj' -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.Website.dll"]