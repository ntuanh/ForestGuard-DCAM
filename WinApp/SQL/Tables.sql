use master
go
drop database KTPM
go
create database KTPM
go
use KTPM
go

create table HoSo
( Id int primary key identity
, Ten nvarchar(50)
, SDT varchar(50)
, Email varchar(50)
, Ext text
)
go
insert into HoSo values
	(N'Vũ Song Tùng', '0989154248', 'tung.vusong@hust.edu.vn', null),
	(N'Đào Lê Thu Thảo', '0989708960', 'thao.daolethu@hust.edu.vn', null)
go

create table Quyen
( Id int primary key identity
, Ten nvarchar(50)
, Ext varchar(50)
)
go
insert into Quyen values
	(N'Lập trình viên', 'Developer'),
	(N'Quản trị hệ thống', 'Admin'),
	(N'Cán bộ nghiệp vụ', 'Staff')
go

create table TaiKhoan
( Ten varchar(50) primary key
, MatKhau varchar(255)
, QuyenId int foreign key references Quyen(Id)
, HoSoId int foreign key references HoSo(Id)
)
go
insert into TaiKhoan values
	('dev', '1234', 1, null),
	('admin', '1234', 2, null),
	('0989154248', '1234', 3, 1),
	('0989708960', '1234', 3, 2)
go

create view ViewHoSo as
	SELECT HoSo.*, TaiKhoan.Ten as TenDangNhap, MatKhau, QuyenId, Quyen.Ten as Quyen FROM TaiKhoan
	INNER JOIN Quyen ON QuyenId = Quyen.Id
	INNER JOIN HoSo ON HoSoId = HoSo.Id
go

create table HanhChinh
( Id int primary key identity
, Ten nvarchar(50)
, TrucThuocId int foreign key references HanhChinh(Id)
)
go
insert into HanhChinh values
	( N'Tỉnh/Thành', NULL),
	( N'Quận/Huyện', 1),
	( N'Phường/Xã', 2),
	( N'Tổ/Thôn', 3)
go
create table TenHanhChinh
( 
	Ten nvarchar(50)
)
go
insert into TenHanhChinh values
	(N'Ấp'),
	(N'Bản'),
	(N'Buôn'),
	(N'Huyện'),
	(N'Làng'),
	(N'Phường'),
	(N'Quận'),
	(N'Sóc'),
	(N'Thành phố'),
	(N'Thị xã'),
	(N'Thị trấn'),
	(N'Thôn'),
	(N'Tỉnh'),
	(N'Tổ'),
	(N'Xã')
go
create table DonVi
( Id int primary key identity
, Ten nvarchar(50)
, HanhChinhId int foreign key references HanhChinh(Id)
, TenHanhChinh nvarchar(50)
, TrucThuocId int foreign key references DonVi(Id)
)
go

insert into DonVi values
	(N'Hà Nội', 1, N'Thành phố', NULL),
	(N'Hai Bà Trưng', 2, N'Quận', 1),
	(N'Bách Khoa', 3, N'Phường', 2),
	(N'Đồng Tâm', 3, N'Phường', 2),
	(N'Thái Bình', 1, N'Tỉnh', NULL),
	(N'Thái Thụy', 2, N'Huyện', 5),
	(N'Thụy Hải', 3, N'Xã', 6),
	(N'Thụy Xuân', 3, N'Xã', 6)
go

create view ViewDonVi as
	SELECT T.*, DonVi.Ten as TrucThuoc FROM
		(SELECT DonVi.*, HanhChinh.Ten as Cap FROM DonVi 
		inner join HanhChinh ON HanhChinhId = HanhChinh.Id) as T
	left join DonVi ON T.TrucThuocId = DonVi.Id
go

create table GiongCay
( Id int primary key identity
, Ten nvarchar(50)
, Nguon nvarchar(255)
)
go
insert into GiongCay values
	(N'Vải', N'Hải Dương'),
	(N'Nhãn', N'Hưng Yên'),
	(N'Mít', N'Nam Định'),
	(N'Dừa', N'Phú Xuyên')
go
select * from ViewDonVi