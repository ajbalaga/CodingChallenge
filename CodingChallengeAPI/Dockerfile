# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CodingChallengeAPI/CodingChallengeAPI.csproj", "CodingChallengeAPI/"]
COPY ["CodingChallenge.core/CodingChallenge.core.csproj", "CodingChallenge.core/"]
RUN dotnet restore "./CodingChallengeAPI/CodingChallengeAPI.csproj"
COPY . .
WORKDIR "/src/CodingChallengeAPI"
RUN dotnet build "./CodingChallengeAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CodingChallengeAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variable for API_KEY
ENV API_KEY="Kj3GhPqR7sT5W8zx9Ly7E4nF1aB2cD3"

ENTRYPOINT ["dotnet", "CodingChallengeAPI.dll"]
