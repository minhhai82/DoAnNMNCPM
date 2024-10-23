CREATE DATABASE DA_QLTuThien
GO
USE DA_QLTuThien
GO
--Tạo Bảng NguoiDung
CREATE TABLE NguoiDung(
	IDNguoiDung int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	TenDayDu nvarchar(100) NOT NULL,
	DiaChi nvarchar(250) NOT NULL,
	DienThoai nvarchar(20) NOT NULL ,
	Email nvarchar(50) NOT NULL,
	TenDangNhap int NOT NULL,
	MatKhau nvarchar(100) NOT NULL,
	GiayPhep varbinary(max) NULL,
	TrangThai int NOT NULL DEFAULT(0),
	Loai int NOT NULL DEFAULT(0)
)
--Tạo bảng TaiKhoan, bảng này dành cho tổ chức và người nhận tài trợ nhập thông tin tài khoản
--ngân hàng vì muốn nhận tiền quyên góp thì phải có tài khoản. Một người sẽ có nhiều tài khoản
CREATE TABLE TaiKhoan(
	IDTaiKhoan int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	IDToChuc int NOT NULL,
	TenNganHang nvarchar(100) NOT NULL,
	TenChuTaiKhoan nvarchar(100) NOT NULL,
	SoTaiKhoan nvarchar(100) NOT NULL,
	SwiftCode nvarchar(50) NULL,
	CONSTRAINT FK_TaiKhoan_NguoiDung FOREIGN KEY (IDToChuc) REFERENCES NguoiDung (IDNguoiDung)
)
GO
--Tạo bảng ChienDich
CREATE TABLE ChienDich(
	IDChienDich int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Ten nvarchar(100) NOT NULL,
	NoiDung nvarchar(max) NOT NULL,
	NgayBatDau datetime NOT NULL,
	NgayKetThuc datetime NOT NULL,
	NganSachDuKien decimal(18, 2) NOT NULL DEFAULT(0),
	ThucThu decimal(18, 2) NOT NULL DEFAULT(0),
	ThucChi decimal(18, 2) NOT NULL DEFAULT(0),
	TrangThai int NOT NULL DEFAULT(0),
	IDToChuc int NOT NULL,
	CONSTRAINT FK_ChienDich_NguoiDung FOREIGN KEY (IDToChuc) REFERENCES NguoiDung (IDNguoiDung)
)
GO
--Tạo bảng TKChienDich
CREATE TABLE TKChienDich(
	IDChienDich int NOT NULL,
	IDTaiKhoan int NOT NULL,
	GhiChu nvarchar(250),
	CONSTRAINT PK_TKChienDich PRIMARY KEY (IDChienDich,IDTaiKhoan),
	CONSTRAINT FK_TKChienDich_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich (IDChienDich)
)

GO

CREATE TABLE DongGop(
	IDDongGop int IDENTITY(1,1) PRIMARY KEY,
	IDNguoiChuyen int NOT NULL,
	IDChienDich int NOT NULL,
	NgayDongGop datetime NOT NULL,
	SoTien decimal(18,2) NOT NULL,
	HinhAnh varbinary(max) NOT NULL,
	GhiChu nvarchar(250) NOT NULL,
	CONSTRAINT FK_DongGop_NguoiDung FOREIGN KEY (IDNguoiChuyen) REFERENCES NguoiDung (IDNguoiDung),
	CONSTRAINT FK_DongGop_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich (IDChienDich)
)
GO
CREATE TABLE TaiTro(
	IDTaiTro int IDENTITY(1,1) PRIMARY KEY,
	IDNguoiNhan int NULL,
	IDChienDich int NOT NULL,
	NgayTaiTro datetime NOT NULL,
	SoTien decimal(18,2) NOT NULL,
	HinhAnh varbinary(max) NOT NULL,
	GhiChu nvarchar(250) NOT NULL,
	CONSTRAINT FK_TaiTro_NguoiDung FOREIGN KEY (IDNguoiNhan) REFERENCES NguoiDung (IDNguoiDung),
	CONSTRAINT FK_TaiTro_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich (IDChienDich)
)
GO
CREATE TABLE XinTaiTro(
	IDXinTaiTro int IDENTITY(1,1) PRIMARY KEY,
	IDNguoiNhan int NOT NULL,
	IDChienDich int NOT NULL,
	NoiDung nvarchar(500) NULL,--nội dung xin 
	PhanHoi nvarchar(500) NULL,--phản hồi của tổ chức
	TrangThai int NOT NULL DEFAULT(0),--trạng thái 0: chờ duyệt, 1 được duyệt, 2 từ chối
	CONSTRAINT FK_XinTaiTro_NguoiDung FOREIGN KEY (IDNguoiNhan) REFERENCES NguoiDung (IDNguoiDung),
	CONSTRAINT FK_XinTaiTro_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich (IDChienDich)
)
GO
CREATE TABLE PhanHoiDanhGia(
	IDPhanHoi int IDENTITY(1,1) PRIMARY KEY,
	IDNguoiPhanHoi int NOT NULL,
	IDChienDich int NOT NULL,
	NoiDung nvarchar(500) NOT NULL,
	DanhGia int NOT NULL, --từ 1-5 sao or thang điểm 0-10
	NgayPhanHoi datetime NOT NULL,
	CONSTRAINT FK_PhanHoiDanhGia_NguoiDung FOREIGN KEY (IDNguoiPhanHoi) REFERENCES NguoiDung (IDNguoiDung),
	CONSTRAINT FK_PhanHoiDanhGia_ChienDich FOREIGN KEY (IDChienDich) REFERENCES ChienDich (IDChienDich)
)
GO
CREATE TABLE BanTin(
	IDBanTin int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	TieuDe nvarchar(250) NOT NULL,
	Nguon nvarchar(500) NOT NULL,
	NoiDungNgan nvarchar(500) NOT NULL,
	NoiDung nvarchar(max) NOT NULL,
	NgayCapNhat datetime NOT NULL
)
GO

CREATE TABLE BinhLuan(
	IDBinhLuan int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	IDBanTin int NOT NULL,
	IDNguoiBinhLuan int NOT NULL,
	NoiDung nvarchar(500) NOT NULL,
	NgayBinhLuan datetime NOT NULL,
	CONSTRAINT FK_BinhLuan_NguoiDung FOREIGN KEY (IDNguoiBinhLuan) REFERENCES NguoiDung (IDNguoiDung),
	CONSTRAINT FK_BinhLuan_ChienDich FOREIGN KEY (IDBanTin) REFERENCES BanTin (IDBanTin)
)
GO
CREATE TABLE BanDo(
	IDBanDo int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	NgayCapNhat datetime NOT NULL,
	TrangThai int NOT NULL -- 0: tạo thành công, 1: hiện, 2: đóng
)
GO
CREATE TABLE BanDoChiTiet(
	IDChiTiet int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	IDBanDo int NOT NULL,
	KinhDo float NOT NULL, --Longitude
	ViDo float NOT NULL, --Latitude,
	GiaTri1 decimal(18,2) NOT NULL, --các giá trị 1-5 là các giá trị thể hiện trên map dùng làm cơ sở tô màu
	GiaTri2 decimal(18,2) NULL,
	GiaTri3 decimal(18,2) NULL,
	GiaTri4 decimal(18,2) NULL,
	GiaTri5 decimal(18,2) NULL
)