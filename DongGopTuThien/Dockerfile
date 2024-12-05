# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Sao chép tệp dự án và khôi phục các gói
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . ./
RUN dotnet publish -c Release -o /out

# Giai đoạn runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Sao chép kết quả build từ giai đoạn trước
COPY --from=build /out .

# Mở cổng ứng dụng
EXPOSE 5133

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "DongGopTuThien.dll"]
