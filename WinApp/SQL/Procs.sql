use KTPM 
go

drop proc updateDonVi
go
create proc updateDonVi
( @action int
, @Id int output
, @Ten nvarchar(50) = NULL
, @HanhChinhId int = NULL
, @TenHanhChinh nvarchar(50) = NULL
, @TrucThuocId int = NULL
) as
BEGIN
    if @action = -1
    begin
        delete from DonVi where Id = @Id
        return
    end
    if @action = 0
    begin
        update DonVi set
            Ten = @Ten,
            HanhChinhId = @HanhChinhId,
            TenHanhChinh = @TenHanhChinh,
            TrucThuocId = @TrucThuocId
            where Id = @Id
        return
    end
    insert into DonVi values (
        @Ten,@HanhChinhId,@TenHanhChinh,@TrucThuocId
    )
    set @Id = @@IDENTITY
END
go

drop proc updateHoSo
go
create proc updateHoSo
( @action int
, @Id int output
, @TenDangNhap varchar(50)
, @Ten nvarchar(50) = NULL
, @SDT varchar(50) = NULL
, @Email varchar(50) = NULL
, @Ext text = NULL
, @MatKhau varchar(255) = NULL
, @QuyenId int = NULL
) as
BEGIN
    if @action = -1
    begin
		delete from TaiKhoan where Ten = @TenDangNhap
        delete from HoSo where Id = @Id
        return
    end
    if @action = 0
    begin
        update HoSo set
            Ten = @Ten,
            SDT = @SDT,
            Email = @Email,
            Ext = @Ext
            where Id = @Id
        return
    end

	declare @fk int
	set @fk = (select HoSoId from TaiKhoan where Ten = @TenDangNhap)
	if @fk is null
	begin
		insert into HoSo values (@Ten,@SDT,@Email,@Ext)
		set @Id = @@IDENTITY
		insert into TaiKhoan values (@TenDangNhap,@MatKhau,@QuyenId,@Id)
	end
END
go

drop proc changePass
go

create proc changePass
( @action int
, @Ten varchar(50)
, @MatKhau varchar(255)
) as
BEGIN
	update TaiKhoan set MatKhau = @MatKhau
	where Ten = @Ten
END
go