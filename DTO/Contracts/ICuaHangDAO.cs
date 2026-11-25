using System.Collections;

namespace SmartShop.DTO.Contracts;

/// <summary>Storage contract shared by BUS and DAO, independent of a database provider.</summary>
public interface ICuaHangDAO
{
    IReadOnlyList<SanPhamDTO> DocSanPham();
    ArrayList NapMangSanPham();
    IReadOnlyList<DonHangDTO> DocDonHang(string phone);
    void CapNhatSanPham(SanPhamDTO value);
    void CapNhatTrangThaiDonHang(string code, string status);

    /// <summary>Persist the order and stock changes atomically, or leave storage unchanged.</summary>
    DonHangDTO LuuDonHang(DonHangDTO order);
}
