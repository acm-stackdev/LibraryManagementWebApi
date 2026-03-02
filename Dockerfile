FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BackendApi.csproj", "./"]
RUN dotnet restore "BackendApi.csproj"

COPY . .
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app


COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "BackendApi.dll"]