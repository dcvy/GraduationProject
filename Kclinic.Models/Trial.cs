using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kclinic.Models
{
    public class Trial
    {
        public int Id { get; set; }
        [Required]
        public string TenPhongKham { get; set; }
        public string DiaChi { get; set; }
        public string NguoiLienHe { get; set; }
        public int Dienthoai { get; set; }
        public string Email { get; set; }
        public int SoLuongNhanVienSuDungPhanMem { get; set; }
        public int SoBacSiKhamBenh { get; set; }
        public int SoBanKham { get; set; }
        public int SoPhongCanLamSang { get; set; }
        public bool DanhMucKyThuatDichVu { get; set; }
        public bool PhongKhamCoDanhMucBHYT { get; set; }
        public bool PhongKhamCoChucNangBanThuoc { get; set; }
        public bool GioiThieuThongTinVePhongKham { get; set; }
        public bool KhachHangDangKyKhamChuaBenh { get; set; }
        public bool KhachHangTraCuuBenhSu { get; set; }
        public bool QuanLyLichHen { get; set; }
        public bool QuanLyTiepDon { get; set; }
        public bool KhamBenh { get; set; }
        public bool QuanLyKhamSucKhoe { get; set; }
        public bool QuanLyChiDinhVaTraLoiKetQuaXetNghiem { get; set; }
        public bool QuanLyChiDinhVaTraLoiKetQuaHinhAnh { get; set; }
        public bool QuanLyDuocPhamVatTuYTe { get; set; }
        public bool QuanLyThuNgan { get; set; }
        public bool HeThongBaoCao { get; set; }
        public bool QuanLyDanhMuc { get; set; }
        public bool QuanTriHeThong { get; set; }
        public bool KetNoiMayXetNghiem { get; set; }
        public bool KetNoiMaySieuAmGiaiPhauBenh { get; set; }
        public bool TraLoiKetQuaQuaZalo { get; set; }
        public string Message { get; set; }
    }
}
