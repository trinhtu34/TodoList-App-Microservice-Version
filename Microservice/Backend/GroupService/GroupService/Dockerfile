# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID
# WORKDIR /app
# EXPOSE 8081

# # This stage is used to build the service project
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["GroupService/GroupService.csproj", "GroupService/"]
# RUN dotnet restore "./GroupService/GroupService.csproj"
# COPY . .
# WORKDIR "/src/GroupService"
# RUN dotnet build "./GroupService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# # This stage is used to publish the service project to be copied to the final stage 
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./GroupService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# # This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "GroupService.dll"]


from mcr.microsoft.com/dotnet/aspnet:8.0 AS base
user $APP_UID
workdir /app
expose 8080

from mcr.microsoft.com/dotnet/sdk:8.0 AS build
arg BUILD_CONFIGURATION=Release
workdir /src

copy ["GroupService.csproj", "."]
run dotnet restore "GroupService.csproj"
copy . .

run dotnet build "./GroupService.csproj" -c $BUILD_CONFIGURATION -o /app/build

from build AS publish
arg BUILD_CONFIGURATION=Release
run dotnet publish "GroupService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

from base AS final
workdir /app
copy --from=publish /app/publish .
entrypoint ["dotnet", "GroupService.dll"]

# docker build -t groupservice-image -f Dockerfile .
# docker run -dit -p 32771:8080 --name groupservice-container groupservice-image