# ---------- BASE RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# ?? VERY IMPORTANT FOR RAILWAY
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PdfMergeApp.csproj", "./"]
RUN dotnet restore "PdfMergeApp.csproj"

COPY . .
RUN dotnet publish "PdfMergeApp.csproj" -c Release -o /app/publish

# ---------- FINAL ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PdfMergeApp.dll"]
