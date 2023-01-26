#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["./Day3-4/OB-Net-Core-Tutorial/OB-Net-Core-Tutorial.sln", "."]
COPY ["./Day3-4/OB-Net-Core-Tutorial/API/API.csproj", "API/"]
COPY ["./Day3-4/OB-Net-Core-Tutorial/BLL/BLL.csproj", "BLL/"]
COPY ["./Day3-4/OB-Net-Core-Tutorial/DAL/DAL.csproj", "DAL/"]
COPY ["./Day3-4/OB-Net-Core-Tutorial/BLL.TEST/BLL.TEST.csproj", "BLL.TEST/"]
COPY ["./Day3-4/OB-Net-Core-Tutorial/ListenerProject/ListenerProject.csproj", "ListenerProject/"]
COPY ["./Day3-4/OB-Net-Core-Tutorial/QuartzProject/QuartzProject.csproj", "QuartzProject/"]
RUN dotnet restore "API/API.csproj"

RUN dotnet build "API/API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API/API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENTRYPOINT ["dotnet", "API.dll"]