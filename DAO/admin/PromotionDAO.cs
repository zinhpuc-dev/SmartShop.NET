using MySql.Data.MySqlClient;
using SmartShop.DTO;
using SmartShop.DTO.Contracts;

namespace SmartShop.DAO;

public sealed class PromotionDAO(string connectionString) : IPromotionRepository
{
    public IReadOnlyList<ManagedPromotion> GetPromotions()
    {
        List<ManagedPromotion> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string query = "SELECT k.MaKM,k.TenKM,k.MoTa,k.LoaiKhuyenMai,k.HinhThucGiam,k.GiaTriGiam,k.GiaTriDonToiThieu,k.NgayBatDau,k.NgayKetThuc,k.TrangThai," +
            "(SELECT COUNT(*) FROM CHI_TIET_KHUYEN_MAI ct WHERE ct.MaKM=k.MaKM) AS ProductCount,(SELECT COUNT(*) FROM VOUCHER v WHERE v.MaKM=k.MaKM) AS VoucherCount FROM KHUYEN_MAI k ORDER BY k.NgayBatDau DESC,k.MaKM";
        using MySqlCommand command = new(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new ManagedPromotion(reader.GetString("MaKM").Trim(), reader.GetString("TenKM"),
            reader.IsDBNull(reader.GetOrdinal("MoTa")) ? string.Empty : reader.GetString("MoTa"), reader.GetString("LoaiKhuyenMai"), reader.GetString("HinhThucGiam"),
            reader.GetDecimal("GiaTriGiam"), reader.GetDecimal("GiaTriDonToiThieu"), reader.GetDateTime("NgayBatDau"), reader.GetDateTime("NgayKetThuc"),
            reader.GetBoolean("TrangThai"), reader.GetInt32("ProductCount"), reader.GetInt32("VoucherCount")));
        return rows;
    }

    public ManagedVoucher? GetVoucher(string code) => GetVouchers().SingleOrDefault(v => v.Code == code);

    public void InsertPromotion(PromotionDraft draft)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string sql = "INSERT INTO KHUYEN_MAI (MaKM,TenKM,MoTa,LoaiKhuyenMai,HinhThucGiam,GiaTriGiam,GiaTriDonToiThieu,NgayBatDau,NgayKetThuc,TrangThai) VALUES (@Code,@Name,@Description,@Type,@Form,@Value,@Minimum,@Starts,@Ends,@Active)";
        using MySqlCommand command = new(sql, connection);
        AddPromotionParameters(command, draft);
        command.ExecuteNonQuery();
    }

    public void UpdatePromotion(PromotionDraft draft)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string sql = "UPDATE KHUYEN_MAI SET TenKM=@Name,MoTa=@Description,LoaiKhuyenMai=@Type,HinhThucGiam=@Form,GiaTriGiam=@Value,GiaTriDonToiThieu=@Minimum,NgayBatDau=@Starts,NgayKetThuc=@Ends,TrangThai=@Active WHERE MaKM=@Code";
        using MySqlCommand command = new(sql, connection);
        AddPromotionParameters(command, draft);
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy chương trình khuyến mại.");
    }

    public IReadOnlyList<ManagedVoucher> GetVouchers()
    {
        List<ManagedVoucher> rows = [];
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string sql = "SELECT v.MaVoucher,v.MaKM,k.TenKM,v.MaCode,v.PhanTramGiam,v.SoTienGiam,v.GiaTriDonToiThieu,v.SoLuong,v.SoLuongDaSuDung,v.NgayBatDau,v.NgayKetThuc,v.TrangThai FROM VOUCHER v JOIN KHUYEN_MAI k ON k.MaKM=v.MaKM ORDER BY v.NgayBatDau DESC,v.MaVoucher";
        using MySqlCommand command = new(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new ManagedVoucher(reader.GetString("MaVoucher").Trim(), reader.GetString("MaKM").Trim(), reader.GetString("TenKM"), reader.GetString("MaCode"),
            reader.GetDecimal("PhanTramGiam"), reader.GetDecimal("SoTienGiam"), reader.GetDecimal("GiaTriDonToiThieu"), reader.GetInt32("SoLuong"), reader.GetInt32("SoLuongDaSuDung"),
            reader.GetDateTime("NgayBatDau"), reader.GetDateTime("NgayKetThuc"), reader.GetBoolean("TrangThai")));
        return rows;
    }

    public void InsertVoucher(VoucherDraft draft)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string sql = "INSERT INTO VOUCHER (MaVoucher,MaKM,MaCode,PhanTramGiam,SoTienGiam,GiaTriDonToiThieu,SoLuong,NgayBatDau,NgayKetThuc,TrangThai) VALUES (@Code,@Promotion,@Voucher,@Percent,@Amount,@Minimum,@Quantity,@Starts,@Ends,@Active)";
        using MySqlCommand command = new(sql, connection);
        AddVoucherParameters(command, draft);
        command.ExecuteNonQuery();
    }

    public void UpdateVoucher(VoucherDraft draft)
    {
        using MySqlConnection connection = new(connectionString);
        connection.Open();
        const string sql = "UPDATE VOUCHER SET MaKM=@Promotion,MaCode=@Voucher,PhanTramGiam=@Percent,SoTienGiam=@Amount,GiaTriDonToiThieu=@Minimum,SoLuong=@Quantity,NgayBatDau=@Starts,NgayKetThuc=@Ends,TrangThai=@Active WHERE MaVoucher=@Code AND SoLuong>=SoLuongDaSuDung";
        using MySqlCommand command = new(sql, connection);
        AddVoucherParameters(command, draft);
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("Không tìm thấy voucher hoặc số lượng mới nhỏ hơn lượt đã sử dụng.");
    }

    private static void AddPromotionParameters(MySqlCommand command, PromotionDraft draft)
    {
        command.Parameters.AddWithValue("@Code", draft.Code);
        command.Parameters.AddWithValue("@Name", draft.Name);
        command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(draft.Description) ? DBNull.Value : draft.Description);
        command.Parameters.AddWithValue("@Type", draft.Type);
        command.Parameters.AddWithValue("@Form", draft.DiscountForm);
        command.Parameters.AddWithValue("@Value", draft.DiscountValue);
        command.Parameters.AddWithValue("@Minimum", draft.MinimumOrderValue);
        command.Parameters.AddWithValue("@Starts", draft.StartsAt);
        command.Parameters.AddWithValue("@Ends", draft.EndsAt);
        command.Parameters.AddWithValue("@Active", draft.IsActive);
    }

    private static void AddVoucherParameters(MySqlCommand command, VoucherDraft draft)
    {
        command.Parameters.AddWithValue("@Code", draft.Code);
        command.Parameters.AddWithValue("@Promotion", draft.PromotionCode);
        command.Parameters.AddWithValue("@Voucher", draft.VoucherCode);
        command.Parameters.AddWithValue("@Percent", draft.PercentOff);
        command.Parameters.AddWithValue("@Amount", draft.AmountOff);
        command.Parameters.AddWithValue("@Minimum", draft.MinimumOrderValue);
        command.Parameters.AddWithValue("@Quantity", draft.Quantity);
        command.Parameters.AddWithValue("@Starts", draft.StartsAt);
        command.Parameters.AddWithValue("@Ends", draft.EndsAt);
        command.Parameters.AddWithValue("@Active", draft.IsActive);
    }
}
