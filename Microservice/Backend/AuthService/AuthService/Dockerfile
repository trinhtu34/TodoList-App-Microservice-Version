# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["AuthService/AuthService.csproj", "AuthService/"]
#RUN dotnet restore "./AuthService/AuthService.csproj"
#COPY . .
#WORKDIR "/src/AuthService"
#RUN dotnet build "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "AuthService.dll"]


# section 2: This is the dockerfile used to build the image for AuthService when unuse Visual Studio

from mcr.microsoft.com/dotnet/aspnet:8.0 AS base
user $APP_UID
workdir /app
expose 8080

from mcr.microsoft.com/dotnet/sdk:8.0 AS build
arg BUILD_CONFIGURATION=Release
workdir /src

copy ["AuthService.csproj", "."]
run dotnet restore "AuthService.csproj"
copy . .

run dotnet build "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

from build AS publish
arg BUILD_CONFIGURATION=Release
run dotnet publish "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

from base AS final
workdir /app
copy --from=publish /app/publish .
entrypoint ["dotnet", "AuthService.dll"]

# to build this dockerfile , you have to navigate to the root/AuthService folder and run the command below

# docker build -t authservice-image -f Dockerfile .
# docker run -dit -p 32770:8080 --name authservice-container authservice-image

# to stop the container , run the command below:
# docker stop authservice-container	

# to remove the container, run the command below:
# docker rm authservice-container