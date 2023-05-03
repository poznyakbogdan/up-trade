FROM mcr.microsoft.com/dotnet/sdk:6.0 as base
WORKDIR /src
COPY . .
CMD ["dotnet", "restore"]

FROM base as builder
WORKDIR /src
RUN ["dotnet", "build", "-o", "build", "WebUI/WebUI.csproj"]

FROM builder AS publisher
WORKDIR /src
RUN dotnet publish "WebUI/WebUI.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publisher /publish .
CMD ["dotnet", "WebUI.dll"]