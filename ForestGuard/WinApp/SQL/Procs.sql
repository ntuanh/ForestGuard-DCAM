USE KTPM;
GO

-- ====================================================================================
-- STORED PROCEDURE CHO DonVi
-- ====================================================================================
IF OBJECT_ID('dbo.updateDonVi', 'P') IS NOT NULL
    DROP PROCEDURE dbo.updateDonVi;
GO

CREATE PROCEDURE dbo.updateDonVi
(
    @action INT,
    @Id INT OUTPUT,
    @Ten NVARCHAR(50) = NULL,
    @HanhChinhId INT = NULL,
    @TenHanhChinh NVARCHAR(50) = NULL,
    @TrucThuocId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    IF @action = -1 -- Delete
    BEGIN
        DELETE FROM dbo.DonVi WHERE Id = @Id;
        RETURN;
    END

    IF @action = 0 -- Update
    BEGIN
        UPDATE dbo.DonVi SET
            Ten = @Ten,
            HanhChinhId = @HanhChinhId,
            TenHanhChinh = @TenHanhChinh,
            TrucThuocId = @TrucThuocId
        WHERE Id = @Id;
        RETURN;
    END

    -- Nếu @action không phải -1 hoặc 0, thì mặc định là Insert (hoặc bạn có thể thêm điều kiện @action = 1)
    INSERT INTO dbo.DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId)
    VALUES (@Ten, @HanhChinhId, @TenHanhChinh, @TrucThuocId);
    SET @Id = SCOPE_IDENTITY(); -- Sử dụng SCOPE_IDENTITY() thay vì @@IDENTITY để an toàn hơn
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: updateDonVi';
GO

-- ====================================================================================
-- STORED PROCEDURE CHO HoSo VÀ TaiKhoan
-- ====================================================================================
IF OBJECT_ID('dbo.updateHoSo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.updateHoSo;
GO

CREATE PROCEDURE dbo.updateHoSo
(
    @action INT,
    @Id INT OUTPUT, -- ID của HoSo
    @TenDangNhap VARCHAR(50),
    @Ten NVARCHAR(50) = NULL,
    @SDT VARCHAR(50) = NULL,
    @Email VARCHAR(50) = NULL,
    @Ext TEXT = NULL,
    @MatKhau VARCHAR(255) = NULL, -- Mật khẩu chỉ dùng khi tạo mới Tài Khoản
    @QuyenId INT = NULL           -- Quyền chỉ dùng khi tạo mới Tài Khoản
)
AS
BEGIN
    SET NOCOUNT ON;
    IF @action = -1 -- Delete HoSo và TaiKhoan liên quan
    BEGIN
        -- Nên xóa TaiKhoan trước nếu HoSoId trong TaiKhoan là khóa ngoại
        -- Hoặc nếu có ON DELETE CASCADE thì chỉ cần xóa HoSo
        -- Giả định TenDangNhap là duy nhất trong TaiKhoan và liên kết với HoSo.Id (nếu HoSoId là khóa ngoại)
        -- Nếu HoSoId là khóa ngoại trong TaiKhoan:
        DELETE FROM dbo.TaiKhoan WHERE HoSoId = @Id;
        -- Hoặc nếu bạn muốn xóa dựa trên TenDangNhap (cẩn thận nếu TenDangNhap không phải là PK của TaiKhoan)
        -- DELETE FROM dbo.TaiKhoan WHERE Ten = @TenDangNhap;

        DELETE FROM dbo.HoSo WHERE Id = @Id;
        RETURN;
    END

    IF @action = 0 -- Update HoSo
    BEGIN
        UPDATE dbo.HoSo SET
            Ten = @Ten,
            SDT = @SDT,
            Email = @Email,
            Ext = @Ext
        WHERE Id = @Id;
        -- Thông thường, không cập nhật TenDangNhap hoặc MatKhau/QuyenId của TaiKhoan ở đây
        -- Việc đổi mật khẩu nên có SP riêng (như changePass)
        RETURN;
    END

    -- Nếu @action không phải -1 hoặc 0, thì mặc định là Insert HoSo và TaiKhoan mới nếu chưa có
    DECLARE @existingHoSoId INT;
    SELECT @existingHoSoId = HoSoId FROM dbo.TaiKhoan WHERE Ten = @TenDangNhap;

    IF @existingHoSoId IS NULL -- Chỉ insert nếu tài khoản chưa tồn tại
    BEGIN
        INSERT INTO dbo.HoSo (Ten, SDT, Email, Ext)
        VALUES (@Ten, @SDT, @Email, @Ext);
        SET @Id = SCOPE_IDENTITY(); -- Lấy ID của HoSo vừa tạo

        INSERT INTO dbo.TaiKhoan (Ten, MatKhau, QuyenId, HoSoId)
        VALUES (@TenDangNhap, @MatKhau, @QuyenId, @Id);
    END
    ELSE
    BEGIN
        -- Có thể trả về lỗi hoặc thông báo tài khoản đã tồn tại
        -- Hoặc gán @Id = @existingHoSoId nếu muốn lấy lại Id của HoSo đã có
        SET @Id = @existingHoSoId;
        -- RAISERROR('Tài khoản với tên đăng nhập này đã tồn tại.', 16, 1);
        -- RETURN;
    END
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: updateHoSo';
GO

-- ====================================================================================
-- STORED PROCEDURE ĐỔI MẬT KHẨU
-- ====================================================================================
IF OBJECT_ID('dbo.changePass', 'P') IS NOT NULL
    DROP PROCEDURE dbo.changePass;
GO

CREATE PROCEDURE dbo.changePass
(
    -- @action INT, -- Tham số này có vẻ không cần thiết cho việc chỉ đổi mật khẩu
    @Ten VARCHAR(50),
    @MatKhau VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TaiKhoan SET
        MatKhau = @MatKhau
    WHERE Ten = @Ten;
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: changePass';
GO


-- ====================================================================================
-- CÁC STORED PROCEDURE MỚI CHO VIỆC QUẢN LÝ RỪNG TRỰC THUỘC TỈNH/THÀNH PHỐ
-- (Sử dụng bảng liên kết RungTinhThanhLienKet)
-- ====================================================================================

-- Stored Procedure để lấy TinhThanhId cho một RungId cụ thể
IF OBJECT_ID('dbo.GetTinhThanhIdForRung', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetTinhThanhIdForRung;
GO

CREATE PROCEDURE dbo.GetTinhThanhIdForRung
    @RungId INT,
    @TinhThanhId INT OUTPUT -- Trả về TinhThanhId qua OUTPUT parameter
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TinhThanhId = TinhThanhId
    FROM dbo.RungTinhThanhLienKet
    WHERE RungId = @RungId;

    IF @@ROWCOUNT = 0 -- Nếu không tìm thấy dòng nào
    BEGIN
        SET @TinhThanhId = NULL; -- Đảm bảo trả về NULL nếu không có liên kết
    END
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: GetTinhThanhIdForRung';
GO


-- Stored Procedure để lấy danh sách tất cả Rừng kèm theo Tên Tỉnh/Thành trực thuộc
IF OBJECT_ID('dbo.SelectAllRungsWithTinhThanh', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SelectAllRungsWithTinhThanh;
GO

CREATE PROCEDURE dbo.SelectAllRungsWithTinhThanh
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.*, -- Lấy tất cả các cột từ bảng Rung
        DV.Ten AS TenTinhThanhTrucThuoc -- Lấy tên Tỉnh/Thành từ bảng DonVi
                                        -- (Hoặc DV.TenDayDu nếu bạn muốn tên đầy đủ hơn)
    FROM
        dbo.Rung R
    LEFT JOIN
        dbo.RungTinhThanhLienKet RTLK ON R.Id = RTLK.RungId
    LEFT JOIN
        dbo.DonVi DV ON RTLK.TinhThanhId = DV.Id
        -- Tùy chọn: AND (DV.HanhChinhId = 1 /* Mã cho Tỉnh/TP */ OR DV.HanhChinhId IS NULL)
    ORDER BY
        R.Id; -- Hoặc R.Ten
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: SelectAllRungsWithTinhThanh';
GO


-- Stored Procedure để lưu hoặc cập nhật liên kết Rừng-Tỉnh/Thành
IF OBJECT_ID('dbo.SaveOrUpdateRungTinhThanhLienKet', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SaveOrUpdateRungTinhThanhLienKet;
GO

CREATE PROCEDURE dbo.SaveOrUpdateRungTinhThanhLienKet
    @RungId INT,
    @SelectedTinhThanhId INT -- Truyền NULL nếu muốn xóa liên kết.
                             -- Nếu UI gửi 0 cho "Không chọn", SP này sẽ không thêm liên kết.
AS
BEGIN
    SET NOCOUNT ON;

    -- Xóa liên kết cũ (nếu có) cho RungId này
    DELETE FROM dbo.RungTinhThanhLienKet
    WHERE RungId = @RungId;

    -- Nếu SelectedTinhThanhId có giá trị hợp lệ (khác NULL và > 0)
    IF @SelectedTinhThanhId IS NOT NULL AND @SelectedTinhThanhId > 0
    BEGIN
        INSERT INTO dbo.RungTinhThanhLienKet (RungId, TinhThanhId)
        VALUES (@RungId, @SelectedTinhThanhId);
    END
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: SaveOrUpdateRungTinhThanhLienKet';
GO


-- (QUAN TRỌNG - CẦN TÙY CHỈNH) Stored Procedure để cập nhật thông tin cơ bản của Rừng
-- Bạn cần thêm tất cả các cột của bảng Rung vào đây.
IF OBJECT_ID('dbo.UpdateRungInfo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.UpdateRungInfo;
GO

CREATE PROCEDURE dbo.UpdateRungInfo
    @action INT,            -- -1: Delete, 0: Update, 1: Insert
    @Id INT OUTPUT,         -- ID của Rung
    @Ten NVARCHAR(255),     -- Thay đổi kích thước NVARCHAR cho phù hợp với bảng Rung
    @ToaDo NVARCHAR(50) = NULL,
    @DienTich FLOAT = NULL, -- Hoặc DECIMAL, tùy thuộc vào kiểu dữ liệu trong bảng Rung
    @DonViId INT = NULL,    -- ID của Đơn vị hành chính cấp xã/phường (nếu có)
    @LoaiCayId INT = NULL,
    @NguonGocId INT = NULL,
    @DieuKienId INT = NULL,
    @MucDichId INT = NULL,
    @ChuId INT = NULL,
    @TruLuongId INT = NULL
    -- !!! THÊM CÁC THAM SỐ KHÁC CHO CÁC CỘT CÒN LẠI CỦA BẢNG Rung VÀO ĐÂY !!!
AS
BEGIN
    SET NOCOUNT ON;

    IF @action = -1 -- Delete Rung
    BEGIN
        -- Nếu Khóa Ngoại từ RungTinhThanhLienKet.RungId đến Rung.Id có ON DELETE CASCADE,
        -- thì các bản ghi liên kết sẽ tự động bị xóa.
        -- Nếu không, bạn cần xóa thủ công trước:
        -- DELETE FROM dbo.RungTinhThanhLienKet WHERE RungId = @Id;

        DELETE FROM dbo.Rung WHERE Id = @Id;
        PRINT 'Đã xóa Rừng với Id: ' + CAST(@Id AS VARCHAR);
        RETURN;
    END

    IF @action = 0 -- Update Rung
    BEGIN
        UPDATE dbo.Rung
        SET Ten = @Ten,
            ToaDo = @ToaDo,
            DienTich = @DienTich,
            DonViId = @DonViId,
            LoaiCayId = @LoaiCayId,
            NguonGocId = @NguonGocId,
            DieuKienId = @DieuKienId,
            MucDichId = @MucDichId,
            ChuId = @ChuId,
            TruLuongId = @TruLuongId
            -- !!! CẬP NHẬT CÁC CỘT KHÁC CỦA BẢNG Rung VÀO ĐÂY !!!
        WHERE Id = @Id;
        PRINT 'Đã cập nhật Rừng với Id: ' + CAST(@Id AS VARCHAR);
        RETURN;
    END

    -- Mặc định là Insert (hoặc bạn có thể thêm điều kiện IF @action = 1)
    INSERT INTO dbo.Rung (
        Ten, ToaDo, DienTich, DonViId, LoaiCayId, NguonGocId, DieuKienId, MucDichId, ChuId, TruLuongId
        -- !!! THÊM TÊN CÁC CỘT KHÁC CỦA BẢNG Rung VÀO ĐÂY !!!
    )
    VALUES (
        @Ten, @ToaDo, @DienTich, @DonViId, @LoaiCayId, @NguonGocId, @DieuKienId, @MucDichId, @ChuId, @TruLuongId
        -- !!! THÊM GIÁ TRỊ CÁC THAM SỐ TƯƠNG ỨNG VÀO ĐÂY !!!
    );

    SET @Id = SCOPE_IDENTITY(); -- Lấy ID của Rừng vừa được tạo
    PRINT 'Đã thêm Rừng mới với Id: ' + CAST(@Id AS VARCHAR);
    RETURN;
END
GO

PRINT 'Đã tạo/cập nhật Stored Procedure: UpdateRungInfo (CẦN TÙY CHỈNH THÊM CỘT)';
GO