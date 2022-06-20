# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Optimized based on https://www.thorsten-hans.com/how-to-build-smaller-and-secure-docker-images-for-net5/
# Trimming is not yet working because some Microsoft Assemblies use the older JSON serialization that does not support trimming

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["SampledStreamCollector/SampledStreamCollector.csproj", "SampledStreamCollector/"]
RUN dotnet restore "SampledStreamCollector/SampledStreamCollector.csproj"
COPY . .
WORKDIR "/src/SampledStreamCollector"
RUN dotnet build "SampledStreamCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampledStreamCollector.csproj" -c Release -o /app/publish \
    --runtime alpine-x64 \
    --self-contained true \
    /p:PublishSingleFile=true \
    /p:PublishTrimmed=false

#    /p:TrimmerSingleWarn=false
#    /p:TrimmerDefaultAction=link

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS final

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser

WORKDIR /app

EXPOSE 5000

COPY --from=publish /app/publish .
ENTRYPOINT ["./SampledStreamCollector", "--urls", "http://localhost:5000"]
