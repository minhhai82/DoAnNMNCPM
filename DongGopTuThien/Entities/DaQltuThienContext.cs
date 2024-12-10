using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DongGopTuThien.Entities;

public partial class DaQltuThienContext : DbContext
{
    public DaQltuThienContext()
    {
    }

    public DaQltuThienContext(DbContextOptions<DaQltuThienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanDo> BanDos { get; set; }

    public virtual DbSet<BanDoChiTiet> BanDoChiTiets { get; set; }

    public virtual DbSet<BanTin> BanTins { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChienDich> ChienDiches { get; set; }

    public virtual DbSet<DongGop> DongGops { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanHoiDanhGium> PhanHoiDanhGia { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TaiTro> TaiTros { get; set; }

    public virtual DbSet<XinTaiTro> XinTaiTros { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.;Initial Catalog=DA_QLTuThien;Persist Security Info=True;User ID=sa;Password=123456;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanDo>(entity =>
        {
            entity.HasKey(e => e.IdbanDo).HasName("PK__BanDo__398A31B3CF84914F");

            entity.ToTable("BanDo");

            entity.Property(e => e.IdbanDo).HasColumnName("IDBanDo");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
        });

        modelBuilder.Entity<BanDoChiTiet>(entity =>
        {
            entity.HasKey(e => e.IdchiTiet).HasName("PK__BanDoChi__EF38009B41EBB213");

            entity.ToTable("BanDoChiTiet");

            entity.Property(e => e.IdchiTiet).HasColumnName("IDChiTiet");
            entity.Property(e => e.GiaTri1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTri2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTri3).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTri4).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GiaTri5).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IdbanDo).HasColumnName("IDBanDo");
        });

        modelBuilder.Entity<BanTin>(entity =>
        {
            entity.HasKey(e => e.IdbanTin).HasName("PK__BanTin__AC3D3C48956314C2");

            entity.ToTable("BanTin");

            entity.Property(e => e.IdbanTin).HasColumnName("IDBanTin");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.Nguon).HasMaxLength(500);
            entity.Property(e => e.NoiDungNgan).HasMaxLength(500);
            entity.Property(e => e.TieuDe).HasMaxLength(250);
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.IdbinhLuan).HasName("PK__BinhLuan__5CDBC03C0EEABC28");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.IdbinhLuan).HasColumnName("IDBinhLuan");
            entity.Property(e => e.IdbanTin).HasColumnName("IDBanTin");
            entity.Property(e => e.IdnguoiBinhLuan).HasColumnName("IDNguoiBinhLuan");
            entity.Property(e => e.NgayBinhLuan).HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);

            entity.HasOne(d => d.IdbanTinNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.IdbanTin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BinhLuan_ChienDich");

            entity.HasOne(d => d.IdnguoiBinhLuanNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.IdnguoiBinhLuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BinhLuan_NguoiDung");
        });

        modelBuilder.Entity<ChienDich>(entity =>
        {
            entity.HasKey(e => e.IdchienDich).HasName("PK__ChienDic__827598AE6A570145");

            entity.ToTable("ChienDich");

            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.IdtoChuc).HasColumnName("IDToChuc");
            entity.Property(e => e.NganSachDuKien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.Ten).HasMaxLength(100);
            entity.Property(e => e.ThucChi).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThucThu).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdtoChucNavigation).WithMany(p => p.ChienDiches)
                .HasForeignKey(d => d.IdtoChuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChienDich_NguoiDung");
        });

        modelBuilder.Entity<DongGop>(entity =>
        {
            entity.HasKey(e => e.IddongGop).HasName("PK__DongGop__6C7AED0D22D75819");

            entity.ToTable("DongGop");

            entity.Property(e => e.IddongGop).HasColumnName("IDDongGop");
            entity.Property(e => e.GhiChu).HasMaxLength(250);
            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.IdnguoiChuyen).HasColumnName("IDNguoiChuyen");
            entity.Property(e => e.NgayDongGop).HasColumnType("datetime");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchienDichNavigation).WithMany(p => p.DongGops)
                .HasForeignKey(d => d.IdchienDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DongGop_ChienDich");

            entity.HasOne(d => d.IdnguoiChuyenNavigation).WithMany(p => p.DongGops)
                .HasForeignKey(d => d.IdnguoiChuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DongGop_NguoiDung");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.IdnguoiDung).HasName("PK__NguoiDun__FCD7DB097B85EC53");

            entity.ToTable("NguoiDung");

            entity.Property(e => e.IdnguoiDung).HasColumnName("IDNguoiDung");
            entity.Property(e => e.DiaChi).HasMaxLength(250);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TenDayDu).HasMaxLength(100);
        });

        modelBuilder.Entity<PhanHoiDanhGium>(entity =>
        {
            entity.HasKey(e => e.IdphanHoi).HasName("PK__PhanHoiD__834B20A9B13222BC");

            entity.Property(e => e.IdphanHoi).HasColumnName("IDPhanHoi");
            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.IdnguoiPhanHoi).HasColumnName("IDNguoiPhanHoi");
            entity.Property(e => e.NgayPhanHoi).HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);

            entity.HasOne(d => d.IdchienDichNavigation).WithMany(p => p.PhanHoiDanhGia)
                .HasForeignKey(d => d.IdchienDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhanHoiDanhGia_ChienDich");

            entity.HasOne(d => d.IdnguoiPhanHoiNavigation).WithMany(p => p.PhanHoiDanhGia)
                .HasForeignKey(d => d.IdnguoiPhanHoi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhanHoiDanhGia_NguoiDung");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.IdtaiKhoan).HasName("PK__TaiKhoan__BC5F907C9D18597A");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.IdtaiKhoan).HasColumnName("IDTaiKhoan");
            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.SoTaiKhoan).HasMaxLength(100);
            entity.Property(e => e.SwiftCode).HasMaxLength(50);
            entity.Property(e => e.TenChuTaiKhoan).HasMaxLength(100);
            entity.Property(e => e.TenNganHang).HasMaxLength(100);

            entity.HasOne(d => d.IdchienDichNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.IdchienDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiKhoan_ChienDich");
        });

        modelBuilder.Entity<TaiTro>(entity =>
        {
            entity.HasKey(e => e.IdtaiTro).HasName("PK__TaiTro__1E709D66FC822784");

            entity.ToTable("TaiTro");

            entity.Property(e => e.IdtaiTro).HasColumnName("IDTaiTro");
            entity.Property(e => e.GhiChu).HasMaxLength(250);
            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.IdnguoiNhan).HasColumnName("IDNguoiNhan");
            entity.Property(e => e.IdxinTaiTro).HasColumnName("IDXinTaiTro");
            entity.Property(e => e.NgayTaiTro).HasColumnType("datetime");
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdchienDichNavigation).WithMany(p => p.TaiTros)
                .HasForeignKey(d => d.IdchienDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiTro_ChienDich");

            entity.HasOne(d => d.IdnguoiNhanNavigation).WithMany(p => p.TaiTros)
                .HasForeignKey(d => d.IdnguoiNhan)
                .HasConstraintName("FK_TaiTro_NguoiDung");

            entity.HasOne(d => d.IdxinTaiTroNavigation).WithMany(p => p.TaiTros)
                .HasForeignKey(d => d.IdxinTaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaiTro_XinTaiTro");
        });

        modelBuilder.Entity<XinTaiTro>(entity =>
        {
            entity.HasKey(e => e.IdxinTaiTro).HasName("PK__XinTaiTr__EE1754590D4F7128");

            entity.ToTable("XinTaiTro");

            entity.Property(e => e.IdxinTaiTro).HasColumnName("IDXinTaiTro");
            entity.Property(e => e.IdchienDich).HasColumnName("IDChienDich");
            entity.Property(e => e.IdnguoiNhan).HasColumnName("IDNguoiNhan");
            entity.Property(e => e.NoiDung).HasMaxLength(500);
            entity.Property(e => e.PhanHoi).HasMaxLength(500);
            entity.Property(e => e.SoTaiKhoan).HasMaxLength(100);
            entity.Property(e => e.SwiftCode).HasMaxLength(50);
            entity.Property(e => e.TenChuTaiKhoan).HasMaxLength(100);
            entity.Property(e => e.TenNganHang).HasMaxLength(100);

            entity.HasOne(d => d.IdchienDichNavigation).WithMany(p => p.XinTaiTros)
                .HasForeignKey(d => d.IdchienDich)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_XinTaiTro_ChienDich");

            entity.HasOne(d => d.IdnguoiNhanNavigation).WithMany(p => p.XinTaiTros)
                .HasForeignKey(d => d.IdnguoiNhan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_XinTaiTro_NguoiDung");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
