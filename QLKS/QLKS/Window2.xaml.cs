using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlTypes;
using static System.Net.WebRequestMethods;
using System.Windows.Controls.Primitives;
using System.Runtime.Remoting.Messaging;

namespace QLKS
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        String ConnectionStrin = "";

        SqlConnection Conn = new SqlConnection();

        DataTable DataSource = null;
        int selectedID = 0;
        public Window2()
        {
            InitializeComponent();
        }

        private void NapDuLieuTuMayChu(DataGrid grdt,String table)
        {
            grdt.ItemsSource = null;
            if (Conn.State != ConnectionState.Open) return;

            String SqlStr = "Select * from " + table ;
            SqlDataAdapter adapter = new SqlDataAdapter(SqlStr, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, table);
            DataSource = dataSet.Tables[table];
            grdt.ItemsSource = DataSource.DefaultView;
        }

        //Đăng xuất
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                User.name = "";
                User.quyen = "";
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        //Đổi mật khẩu
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Window3 window3 = new Window3();
            window3.ShowDialog();
        }

        //Thoát chương trình
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn thoát chương trình ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        //Khi cửa sổ chính được mở
        private void frm_quanli_Loaded(object sender, RoutedEventArgs e)
        {
            // Hiển thị thông tin trong menu User
            txtb_ttnv.Text = "Thông tin người dùng:\n   Họ và tên:   " + User.name + "\n   Quyền đăng nhập:   " + User.quyen + "\n";

            //Kiểm tra quyền 
            if(User.quyen == "Nhân viên")
            {
                tab_qlnv.IsEnabled = false;
                tab_qlp.IsEnabled = false;
                tab_qldv.IsEnabled = false;
                //menu_file.IsEnabled = false;
            }
            ConnectionStrin = @"Data Source=.\HUONG;Initial Catalog=qlks;Integrated Security=True;";
            Conn.ConnectionString = ConnectionStrin;
            Conn.Open();

            NapDuLieuTuMayChu(grdt_nv, "tblUser");
            NapDuLieuTuMayChu(grdt_phong, "tblPhong");
            NapDuLieuTuMayChu(grdt_dv, "tblDv");
            NapDuLieuTuMayChu(grdt_kh, "tblKh");
            NapDuLieuTuMayChu(grdt_bk, "tblBookings");
            NapDuLieuTuMayChu(grdt_bkdv, "tblBookingDv");
            addmabk();
            addmadv();
            addphongdangdung();
            NapDuLieuPhongDangSuDungTuMayChu(grdt_phongdangsd, "tblPhong");
            NapDuLieuTuMayChu(grdt_hd, "tblHoadon");
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        //Quan li nhan vien:
        private void setbtnv(bool edit)
        {
            bt_capnhatnv.Visibility = edit ? Visibility.Visible : Visibility.Hidden;//edit true dữ liệu hiển thị ngược lại fale sẽ ẩn đi 
            bt_boquanv.Visibility = edit ? Visibility.Visible : Visibility.Hidden;

            txt_tnv.IsEnabled = edit;
            comb_gtnv.IsEnabled = edit;
            txt_tdnnv.IsEnabled = edit;
            txt_mknv.IsEnabled = edit;
            txt_dcnv.IsEnabled = edit;
            dp_nsnv.IsEnabled = edit;
            comb_quyen.IsEnabled = edit;
            txt_mtnv.IsEnabled = edit;
            txt_gcnv.IsEnabled = edit;
            dp_nsnv.IsEnabled = edit;
            edit = !edit;
            bt_them.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_sua.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_xoa.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_lammoi.Visibility = edit ? Visibility.Visible : Visibility.Hidden;

            grdt_nv.IsEnabled = edit;
        }
        bool cnnv = false;
        private void bt_them_Click(object sender, RoutedEventArgs e)
        {
            setbtnv(true);
            cnnv = true;
            txt_tnv.Focus();
        }

        private void bt_sua_Click(object sender, RoutedEventArgs e)
        {
            setbtnv(true);
            bt_bqcn.Visibility = Visibility.Hidden;
            cnnv = false;
            txt_tnv.Focus();
        }
        int mnv = 0;
        // Chon vao 1 dtgr cua danh sach nhan vien
        private void grdt_nv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bt_them.Visibility = Visibility.Hidden;
                bt_bqcn.Visibility = Visibility.Visible;
                if (grdt_nv.SelectedItem != null)// kiem tra hang duoc chon
                {
                    DataRowView row = (DataRowView)grdt_nv.CurrentItem;
                    mnv = (int)row[0];
                    txt_tnv.Text = row[1].ToString();
                    DateTime ns = (DateTime)row[2];
                    dp_nsnv.SelectedDate = ns;
                    // tìm kiếm một mục trong cbb có nội dung bằng giá trị được chọn
                    var item = comb_gtnv.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row[3].ToString().Trim());
                    if (item != null)
                    {
                        comb_gtnv.SelectedItem = item;
                    }
                    txt_dcnv.Text = row[4].ToString();
                    var item1 = comb_quyen.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row[5].ToString().Trim());
                    if (item1 != null)
                    {
                        comb_quyen.SelectedItem = item1;
                    }
                    txt_tdnnv.Text = row[6].ToString();
                    txt_mknv.Text = row[7].ToString();
                    txt_mtnv.Text = row[8].ToString();
                    txt_gcnv.Text = row[9].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_bqcn_Click(object sender, RoutedEventArgs e)
        {
            grdt_nv.SelectedItem = false;
            bt_them.Visibility = Visibility.Visible;
            bt_bqcn.Visibility = Visibility.Hidden;
        }

        private void bt_capnhatnv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cnnv)
                {
                    ComboBoxItem it = comb_gtnv.SelectedItem as ComboBoxItem;
                    ComboBoxItem it1 = comb_quyen.SelectedItem as ComboBoxItem;
                    String ns = dp_nsnv.SelectedDate.Value.ToString("yyyy-MM-dd");
                    string sqlStr = "";
                    sqlStr = "Insert Into tblUser(name,ngaysinh,gioitinh,address,quyen,tendangnhap,matkhau,mota,ghichu)values('" + txt_tnv.Text + "','" + ns + "','" + it.Content.ToString() + "','" + txt_dcnv.Text + "','" + it1.Content.ToString() + "','" + txt_tdnnv.Text + "','" + Mahoa.EncodeMD5(txt_mknv.Text) + "','" + txt_mtnv.Text + "','" + txt_gcnv.Text + "')";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_nv, "tblUser");
                }
                else
                {
                    ComboBoxItem it = comb_gtnv.SelectedItem as ComboBoxItem;
                    ComboBoxItem it1 = comb_quyen.SelectedItem as ComboBoxItem;
                    String ns = dp_nsnv.SelectedDate.Value.ToString("yyyy-MM-dd");
                    String sqlStr = "Update tblUser Set " + "name = '" + txt_tnv.Text + "',ngaysinh = '" + ns + "',gioitinh = '" + it.Content.ToString() + "',address = '" + txt_dcnv.Text + "',quyen = '" + it1.Content.ToString() + "', tendangnhap = '" + txt_tdnnv.Text + "', matkhau = '" + Mahoa.EncodeMD5(txt_mknv.Text) + "',mota = '" + txt_mtnv.Text + "',ghichu = '" + txt_gcnv.Text + "' Where id = " + mnv;
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_nv, "tblUser");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật\n" + ex.Message);
            }
            setbtnv(false);
            bt_bqcn.Visibility = Visibility.Hidden;
        }

        private void bt_boquanv_Click(object sender, RoutedEventArgs e)
        {
            setbtnv(false);
        }

        private void bt_lammoi_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_nv, "tblUser");
            bt_bqcn.Visibility = Visibility.Hidden;
            bt_them.Visibility = Visibility.Visible;
        }

        private void bt_xoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin nhân viên?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    String sqlStr = "Delete from tblUser Where id = " + mnv;
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_nv, "tblUser");                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Quan li phong:

        private void setbtphong(bool edit)
        {
            bt_capnhatphong.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_boquaphong.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            txt_maphong.IsEnabled = edit;
            txt_sophong.IsEnabled = edit;
            cmb_loaiphong.IsEnabled = edit;
            txt_giaphong.IsEnabled = edit;
            cmb_ttphong.IsEnabled = edit;
            txt_ghichuphong.IsEnabled = edit;
            edit = !edit;
            bt_themphong.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_suaphong.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_xoaphong.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_refresh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
           
            grdt_phong.IsEnabled = edit;
        }
        bool cn = false;
        string maphong = "";
        private void bt_themphong_Click(object sender, RoutedEventArgs e)
        {
            setbtphong(true);
            cn = true;
            txt_sophong.Focus();
        }

        private void bt_suaphong_Click(object sender, RoutedEventArgs e)
        {
            setbtphong(true);
            bt_bqcnp.Visibility = Visibility.Hidden;
            cn = false;
            txt_sophong.Focus();
        }

        private void bt_bqcnp_Click(object sender, RoutedEventArgs e)
        {
            grdt_phong.SelectedItem = false;
            bt_themphong.Visibility = Visibility.Visible;
            bt_bqcnp.Visibility = Visibility.Hidden;
        }

        private void bt_refresh_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_phong, "tblPhong");
            bt_bqcnp.Visibility = Visibility.Hidden;
            bt_themphong.Visibility = Visibility.Visible;
        }

        private void bt_capnhatphong_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cn)
                {
                    //ComboBoxItem it = cmb_loaiphong.SelectedItem as ComboBoxItem;
                    //ComboBoxItem it1 = cmb_ttphong.SelectedItem as ComboBoxItem;
                    if(cmb_loaiphong.SelectedItem != null && cmb_ttphong.SelectedItem != null)
                    {
                        string sqlStr = "";
                        sqlStr = "Insert Into tblPhong(maphong,sophong,loaiphong,gia,trangthai,ghichu)values('" + Ma.Phong(txt_sophong.Text) + "','" + txt_sophong.Text + "','" + cmb_loaiphong.Text + "'," + float.Parse(txt_giaphong.Text) + ",N'" + cmb_ttphong.Text + "',N'" + txt_ghichuphong.Text + "')";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_phong, "tblPhong");
                        setbtphong(false);
                        bt_bqcnp.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn loại phòng và trạng thái phòng");
                    }

                }
                else
                {
                    //ComboBoxItem it = cmb_loaiphong.SelectedItem as ComboBoxItem;
                    //ComboBoxItem it1 = cmb_ttphong.SelectedItem as ComboBoxItem;
                    if (cmb_loaiphong.SelectedItem != null && cmb_ttphong.SelectedItem != null)
                    {
                        String sqlStr = "Update tblPhong Set maphong ='" + Ma.Phong(txt_sophong.Text) + "', sophong = '" + txt_sophong.Text + "', loaiphong = '" + cmb_loaiphong.Text + "', gia = " + float.Parse(txt_giaphong.Text) + ", trangthai = '" + cmb_ttphong.Text + "',ghichu = '" + txt_ghichuphong.Text + "' Where maphong = '" + maphong +"'";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_phong, "tblPhong");
                        setbtphong(false);
                        bt_bqcnp.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn loại phòng và trạng thái phòng");
                    }
                }
                addmaphong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật\n" + ex.Message);
            }
        }

        private void grdt_phong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bt_bqcnp.Visibility = Visibility.Visible;
                bt_themphong.Visibility = Visibility.Hidden;
                if (grdt_phong.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_phong.CurrentItem;
                maphong = row[0].ToString();
                txt_maphong.Text = row[0].ToString();
                txt_sophong.Text = row[1].ToString();
                var item = cmb_loaiphong.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row[2].ToString().Trim());
                if (item != null)
                {
                    cmb_loaiphong.SelectedItem = item;
                }
                txt_giaphong.Text = row[3].ToString();
                var item1 = cmb_ttphong.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row[4].ToString().Trim());
                if (item1 != null)
                {
                    cmb_ttphong.SelectedItem = item1;
                }
                
                txt_ghichuphong.Text = row[5].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_boquaphong_Click(object sender, RoutedEventArgs e)
        {
            setbtphong(false);
        }

        private void bt_xoaphong_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    String sqlStr = "SELECT COUNT(maphong) FROM tblBookings WHERE maphong = '" + maphong + "'";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    int slbk = 0;
                    while (reader.Read())
                    {
                        slbk = reader.GetInt32(0);
                    }
                    reader.Close();
                    String[] mabk = new String[slbk];
                    if (slbk > 0)
                    {
                        sqlStr = "SELECT mabooking FROM tblBookings WHERE maphong = '" + maphong + "'";
                        cmd = new SqlCommand(sqlStr, Conn);
                        reader = cmd.ExecuteReader();
                        int sl = 0;
                        while (reader.Read())
                        {
                            mabk[sl] = reader.GetString(0);
                            sl++;
                        }
                        reader.Close();

                        for (int i = 0; i < slbk; i++)
                        {
                            sqlStr = "Delete from tblBookingDv Where mabooking = '" + mabk[i] + "';" +
                                     "Delete from tblHoadon Where mabooking ='" + mabk[i] + "';" +
                                     "Delete from tblBookings Where mabooking = '" + mabk[i] + "';";
                            cmd = new SqlCommand(sqlStr, Conn);
                            cmd.ExecuteNonQuery();
                        }

                        sqlStr = "Delete from tblPhong Where maphong = '" + maphong + "'";
                        cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_phong, "tblPhong");
                    }
                    else
                    {
                        sqlStr = "Delete from tblPhong Where maphong = '" + maphong + "'";
                        cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_phong, "tblPhong");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_sophong_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txt_sophong.Text != "")
            {
                txt_maphong.Text = Ma.Phong(txt_sophong.Text);
            }
            else
            {
                txt_maphong.Text = "";
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Quản lí khách hàng
        private void setbtkh(bool edit)
        {
            bt_cnkh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_bqkh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            txt_tenkh.IsEnabled = edit;
            txt_dckh.IsEnabled = edit;
            txt_sdtkh.IsEnabled = edit;
            comb_gtkh.IsEnabled = edit;
            txt_gckh.IsEnabled = edit;
            edit = !edit;
            bt_suakh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_xoakh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;
            bt_rfkh.Visibility = edit ? Visibility.Visible : Visibility.Hidden;

            grdt_kh.IsEnabled = edit;
        }
        private void bt_suakh_Click(object sender, RoutedEventArgs e)
        {
            setbtkh(true);
            txt_tenkh.Focus();
        }
        String makh = "";
        private void grdt_kh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grdt_kh.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_kh.CurrentItem;
                makh = row["makh"].ToString();
                txt_makh.Text = makh;
                txt_tenkh.Text = row["tenkh"].ToString();
                txt_dckh.Text = row["diachi"].ToString();
                txt_sdtkh.Text = row["sdt"].ToString();
                var item = comb_gtkh.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row["gioitinh"].ToString().Trim());
                if (item != null)
                {
                    comb_gtkh.SelectedItem = item;
                }
                txt_gckh.Text = row["ghichu"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_xoakh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    String sqlStr = "Delete from tblKh Where makh = '" + makh + "'";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_kh, "tblKh");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bt_rfkh_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_kh, "tblKh");
        }

        private void bt_cnkh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBoxItem it = comb_gtkh.SelectedItem as ComboBoxItem;
                String sqlStr = "Update tblKh Set tenkh = '" + txt_tenkh.Text + "', diachi = '" + txt_dckh.Text + "', sdt = '" + txt_sdtkh.Text + "', gioitinh = '" + it.Content.ToString() + "',ghichu = '" + txt_gckh.Text + "' Where makh = '" + makh + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                cmd.ExecuteNonQuery();
                NapDuLieuTuMayChu(grdt_kh, "tblKh");
            }catch(Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật\n" + ex.Message);
            }
            setbtkh(false);
        }

        private void bt_bqkh_Click(object sender, RoutedEventArgs e)
        {
            setbtkh(false);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Quản lí dịch vụ

        private void setbtdv(bool set)
        {
            bt_cndv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            txt_tendv.IsEnabled = set;
            txt_giadv.IsEnabled = set;
            txt_gcdv.IsEnabled = set;
            txt_madv.IsEnabled = set;
            set = !set;
            bt_themdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_suadv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_xoadv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_rfdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;

            grdt_dv.IsEnabled = set;
        }
        bool cndv = false;
        String madv = "";
        private void bt_themdv_Click(object sender, RoutedEventArgs e)
        {
            setbtdv(true);
            cndv = true;
        }

        private void bt_bqcndv_Click(object sender, RoutedEventArgs e)
        {
            grdt_dv.SelectedItem = false;
            bt_themdv.Visibility = Visibility.Visible;
            bt_bqcndv.Visibility = Visibility.Hidden;
        }

        private void grdt_dv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bt_bqcndv.Visibility = Visibility.Visible;
                bt_themdv.Visibility = Visibility.Hidden;
                if (grdt_dv.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_dv.CurrentItem;
                madv = row["madv"].ToString();
                txt_madv.Text = madv;
                txt_tendv.Text = row["tendv"].ToString();
                txt_giadv.Text = row["gia"].ToString();
                txt_gcdv.Text = row["ghichu"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_suadv_Click(object sender, RoutedEventArgs e)
        {
            setbtdv(true);
            bt_bqcndv.Visibility = Visibility.Hidden;
            txt_madv.IsEnabled = false;
            cndv = false;
        }

        private void bt_xoadv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    String sqlStr = "Delete from tblDv Where madv = '" + madv + "'";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_dv, "tblDv");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa dịch vụ\n" + ex.Message);
            }
        }

        private void bt_cndv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cndv)
                {
                    String sqlStr = "Insert Into tblDV(madv,tendv,gia,ghichu)values('" + txt_madv.Text + "','" + txt_tendv.Text + "'," + float.Parse(txt_giadv.Text) + ",N'" + txt_gcdv.Text + "')";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_dv, "tblDv");
                }
                else
                {
                    String sqlStr = "Update tblDv Set " + "tendv = '" + txt_tendv.Text + "', gia = " + float.Parse(txt_giadv.Text) + ",ghichu = '" + txt_gcdv.Text + "' Where madv = '" + madv + "'";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_dv, "tblDv");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật\n" + ex.Message);
            }
            setbtdv(false);
            bt_bqcndv.Visibility = Visibility.Hidden;
        }

        private void bt_bqdv_Click(object sender, RoutedEventArgs e)
        {
            setbtdv(false);
        }

        private void bt_rfdv_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_dv, "tblDv");
            bt_bqcndv.Visibility = Visibility.Hidden;
            bt_themdv.Visibility = Visibility.Visible;
        }
        //Set tieu de tren toolbar
        private void tab_ql_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabItem tabItem = tabControl.SelectedItem as TabItem;
            toolbar_ql.Header = "Quản lí " + tabItem.Header.ToString().ToLower();

        }

        private void tab_bk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tabItem = tab_bk.SelectedItem as TabItem;
            toolbar_bk.Header = "Books " + tabItem.Header.ToString().ToLower();
        }

        private void tab_traphong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tabItem = tab_traphong.SelectedItem as TabItem;
            if(tab_traphong.SelectedItem != null)
            {
                toolbar_tp.Header = tabItem.Header.ToString();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txt_gcbk.Text = dp_nnp.SelectedDate.Value.ToString();
            DateTime dateTime = dp_nnp.SelectedDate.Value;
            txt_dckh_bk.Text = dateTime.ToShortDateString();
            dateTime = DateTime.Parse(txt_dckh_bk.Text);
            txt_gcbk.Text = dateTime.ToString();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // Đặt phòng

        private void setbt_bk(bool set)
        {
            bt_cnbk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqbk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            txt_dckh_bk.IsReadOnly = !set;
            txt_tenkh_bk.IsReadOnly = !set;
            txt_sdtkh_bk.IsReadOnly = !set;
            comb_gtkh_bk.IsEnabled = set;
            txt_gckh_bk.IsReadOnly = !set;
            comb_maphong.IsEnabled = set;
            txt_gcbk.IsReadOnly = !set;
            
            bt_dtp.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_dp.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_rfbk.Visibility = !set ? Visibility.Visible : Visibility.Hidden;

            grdt_bk.IsEnabled = !set;

            bt_bqcnbk.Visibility = Visibility.Hidden;
            bt_suabk.Visibility = Visibility.Hidden;
            bt_huybk.Visibility = Visibility.Hidden;
        }

        private void setbt_dtgr_bk_selection(bool set)
        {
            bt_bqcnbk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_suabk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_huybk.Visibility = set ? Visibility.Visible : Visibility.Hidden;

            bt_dtp.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_dp.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_rfbk.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            grdt_bk.IsEnabled = !set;
        }

        private void setbt_sua_bk(bool set)
        {
            bt_cnbk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqbk.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            txt_dckh_bk.IsReadOnly = !set;
            txt_tenkh_bk.IsReadOnly = !set;
            txt_sdtkh_bk.IsReadOnly = !set;
            comb_gtkh_bk.IsEnabled = set;
            txt_gckh_bk.IsReadOnly = !set;
            comb_maphong.IsEnabled = set;
            txt_gcbk.IsReadOnly = !set;
            bt_bqcnbk.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_suabk.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
            bt_huybk.Visibility = !set ? Visibility.Visible : Visibility.Hidden;
        }

        private void addmaphong()
        {
            try
            {
                comb_maphong.Items.Clear();
                String sqlStr = "SELECT maphong FROM tblPhong Where trangthai = 'Trống'" ;
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string maphong = reader.GetString(0);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = maphong;
                    comb_maphong.Items.Add(item);
                }
                reader.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi tham chiếu đến danh sách phòng");
            }
        }

        private void addmakh()
        {
            try
            {
                comb_makh.Items.Clear();
                String sqlStr = "SELECT makh FROM tblKh";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string makh = reader.GetString(0);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = makh;
                    comb_makh.Items.Add(item);
                }
                reader.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi tham chiếu đến danh sách khách hàng");
            }
        }

        bool cnbk = false;

        private void bt_dp_Click(object sender, RoutedEventArgs e)
        {
            addmaphong();
            setbt_bk(true);
            cnbk = true;
            
            txt_mabk.Text = Ma.BookingPhong();
            txt_makh_bk.Text = Ma.KhachHang();
        }

        private void bt_bqcnbk_Click(object sender, RoutedEventArgs e)
        {
           // grdt_bk.SelectedItem = null;
            setbt_dtgr_bk_selection(false);  
            
        }

        String mabk = "";
        String makh_bk = "";
        String maphongcu = "";
        private void grdt_bk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                setbt_dtgr_bk_selection(true);
                addmaphong();
                if (grdt_bk.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_bk.CurrentItem;
                makh_bk = row["makh"].ToString();
                String sqlStr = "SELECT * FROM tblKh where makh ='" + makh_bk + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_makh_bk.Text = reader.GetString(0);
                    txt_tenkh_bk.Text = reader.GetString(1);
                    txt_dckh_bk.Text = reader.GetString(2);
                    txt_sdtkh_bk.Text = reader.GetString(3);
                    var item1 = comb_gtkh_bk.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == reader.GetString(4).Trim());
                    if (item1 != null)
                    {
                        comb_gtkh_bk.SelectedItem = item1;
                    }
                    txt_gckh_bk.Text = reader.GetString(5);
                }
                reader.Close();
                mabk = row["mabooking"].ToString();
                txt_mabk.Text = mabk;
                txt_makh_bk.Text = makh_bk;
                ComboBoxItem item = new ComboBoxItem();
                item.Content = row["maphong"];
                maphongcu = row["maphong"].ToString();
                comb_maphong.Items.Add(item);
                comb_maphong.SelectedItem = item;
                DateTime nnp = (DateTime)row["ngaynhan"];
                dp_nnp.SelectedDate = nnp;
                DateTime ntp = (DateTime)row["ngaytra"];
                dp_ntp.SelectedDate = ntp;
                txt_gcbk.Text = row["ghichu"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
            grdt_bk.SelectedItem = null;
        }

        private void bt_suabk_Click(object sender, RoutedEventArgs e)
        {
            addmaphong();
            setbt_sua_bk(true);
            
            cnbk = false;
            
            txt_tenkh_bk.Focus();
        }

        private void comb_makh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem it = comb_makh.SelectedItem as ComboBoxItem;
            if (comb_makh.SelectedItem != null)
            {
                makh_bk = it.Content.ToString();
                String sqlStr = "SELECT * FROM tblKh where makh ='" + makh_bk + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_makh_bk.Text = reader.GetString(0);
                    txt_tenkh_bk.Text = reader.GetString(1);
                    txt_dckh_bk.Text = reader.GetString(2);
                    txt_sdtkh_bk.Text = reader.GetString(3);
                 
                    var item1 = comb_gtkh_bk.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == reader.GetString(4).Trim());
                    if (item1 != null)
                    {
                        comb_gtkh_bk.SelectedItem = item1;
                    }
                    txt_gckh_bk.Text = reader.GetString(5);
                }
                reader.Close();
            }

        }

        private void bt_cnbk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtp)
                {
                    if ((comb_maphong.SelectedItem != null) && (comb_makh.SelectedItem != null))
                    {
                        String maphong = comb_maphong.Text;
                        String makh_bk = comb_makh.Text;
                        String nnp = dp_nnp.SelectedDate.Value.ToString("yyyy-MM-dd");
                        String ntp = dp_ntp.SelectedDate.Value.ToString("yyyy-MM-dd");
                        String sqlStr = "";
                        sqlStr = "Insert Into tblBookings(mabooking,makh,maphong,ngaynhan,ngaytra,trangthai,ghichu)values('" + txt_mabk.Text + "','" + makh_bk + "','" + maphong + "','" + nnp + "','" + ntp + "',N'Chưa thanh toán',N'" + txt_gcbk.Text + "');" +
                                 "Update tblPhong Set trangthai = 'Đang sử dụng' Where maphong = '" + maphong + "';";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_bk, "tblBookings");

                        dtp = false;
                        addmaphong();
                        addphongdangdung();
                        NapDuLieuPhongDangSuDungTuMayChu(grdt_phongdangsd, "tblPhong");
                        setbt_bk(false);
                       
                    }
                    else
                    {
                        MessageBox.Show("Chưa chọn mã phòng hoặc giới tính khách hàng");
                    }

                }
                else
                {
                    if (cnbk)
                    {
                       if((comb_maphong.SelectedItem != null) && (comb_gtkh_bk.SelectedItem != null))
                       {
                            String maphong = comb_maphong.Text;
                            String gt = comb_gtkh_bk.Text;
                            String nnp = dp_nnp.SelectedDate.Value.ToString("yyyy-MM-dd");
                            String ntp = dp_ntp.SelectedDate.Value.ToString("yyyy-MM-dd");
                            String sqlStr = "";
                            sqlStr = "Insert Into tblKh(makh,tenkh,diachi,sdt,gioitinh,ghichu)values('" + txt_makh_bk.Text + "','" + txt_tenkh_bk.Text + "','" + txt_dckh_bk.Text + "','" + txt_sdtkh_bk.Text + "','" + gt + "',N'" + txt_gckh_bk.Text + "');" +
                                     "Insert Into tblBookings(mabooking,makh,maphong,ngaynhan,ngaytra,trangthai,ghichu)values('" + txt_mabk.Text + "','" + txt_makh_bk.Text + "','" + maphong + "','" + nnp + "','" + ntp + "',N'Chưa thanh toán',N'" + txt_gcbk.Text + "');" +
                                     "Update tblPhong Set trangthai = 'Đang sử dụng' Where maphong = '" + maphong + "';";
                            SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                            cmd.ExecuteNonQuery();
                            NapDuLieuTuMayChu(grdt_bk, "tblBookings");

                            dtp = false;
                            addmaphong();
                            addphongdangdung();
                            NapDuLieuPhongDangSuDungTuMayChu(grdt_phongdangsd, "tblPhong");
                            setbt_bk(false);
                           
                       }
                       else
                       {
                            MessageBox.Show("Chưa chọn mã phòng hoặc giới tính khách hàng");
                       }
                    }
                    else
                    {
                        if ((comb_maphong.SelectedItem != null) && (comb_gtkh_bk.SelectedItem != null))
                        {
                            String maphong = comb_maphong.Text;
                            String gt = comb_gtkh_bk.Text;
                            String nnp = dp_nnp.SelectedDate.Value.ToString("yyyy-MM-dd");
                            String ntp = dp_ntp.SelectedDate.Value.ToString("yyyy-MM-dd");
                            String sqlStr = "";
                            sqlStr = "Update tblKh Set tenkh = '" + txt_tenkh_bk.Text + "', diachi = N'" + txt_dckh_bk.Text + "', sdt = '" + txt_sdtkh_bk.Text + "', gioitinh = '" + gt + "',ghichu = N'" + txt_gckh_bk.Text + "' Where makh = '" + makh_bk + "';" +
                                     "Update tblBookings Set maphong = '" + maphong + "',ngaynhan = '" + nnp + "',ngaytra = '" + ntp + "',ghichu = '" + txt_gckh.Text + "' Where mabooking = '" + mabk + "';" +
                                     "Update tblPhong Set trangthai = 'Trống' Where maphong = '" + maphongcu + "';" +
                                     "Update tblPhong Set trangthai = 'Đang sử dụng' Where maphong = '" + maphong + "';";
                            SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                            cmd.ExecuteNonQuery();
                            
                            NapDuLieuTuMayChu(grdt_kh, "tblKh");

                            dtp = false;
                            addmaphong();
                            addphongdangdung();
                            NapDuLieuPhongDangSuDungTuMayChu(grdt_phongdangsd, "tblPhong");
                            setbt_bk(false);
                            NapDuLieuTuMayChu(grdt_bk, "tblBookings");
                            NapDuLieuTuMayChu(grdt_phong, "tblPhong");
                        }
                        else
                        {
                            MessageBox.Show("Chưa chọn mã phòng hoặc giới tính khách hàng");
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        bool dtp = false;
        private void bt_dtp_Click(object sender, RoutedEventArgs e)
        {
            addmaphong();
            addmakh();
            setbt_bk(true);
            addmakh();
            dtp = true;
            comb_makh.Visibility = Visibility.Visible;
            txt_mabk.Text = Ma.BookingPhong();
        }

        private void bt_huybk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn hủy phòng ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DateTime now = DateTime.Now;
                    if (now < dp_nnp.SelectedDate.Value)
                    {
                        String sqlStr = "";
                        sqlStr = "Update tblBookings set trangthai = 'Đã hủy' Where mabooking = '" + mabk + "';" + 
                                 "Update tblPhong set trangthai = 'Trống' Where maphong = '" + comb_maphong.Text + "';";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        setbt_bk(false);
                        NapDuLieuTuMayChu(grdt_bk, "tblBookings");
                    }
                    else
                    {
                        MessageBox.Show("Phòng đang được sử dụng\nKhông thể hủy");
                    }                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bt_bqbk_Click(object sender, RoutedEventArgs e)
        {
            comb_makh.Visibility = Visibility.Hidden;
            setbt_bk(false);
            dtp = false;
        }

        private void bt_rfbk_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_bk, "tblBookings");
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Đặt dịch vụ

        private void setbtbkdv(bool set)
        {
            bt_cnbkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqbkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            comb_mabk.IsEnabled = set;
            comb_madv.IsEnabled = set;
            txt_sldv_bk.IsEnabled = set;
            txt_gcbkdv.IsEnabled = set;
            dp_nddv.IsEnabled = set;
            set = !set;
            bt_bkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_sbkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_hbkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_rfbkdv.Visibility = set ? Visibility.Visible : Visibility.Hidden;

            grdt_bkdv.IsEnabled = set;
        }

        private void addmabk()
        {
            try
            {
                comb_mabk.Items.Clear();
                String sqlStr = "SELECT mabooking FROM tblBookings Where trangthai = 'Chưa thanh toán'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string mabk = reader.GetString(0);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = mabk;
                    comb_mabk.Items.Add(item);
                }
                reader.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi tham chiếu đến danh sách đặt phòng");
            }
        }

        private void addmadv()
        {
            try
            {
                comb_madv.Items.Clear();
                String sqlStr = "SELECT madv FROM tblDv";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string madv = reader.GetString(0);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = madv;
                    comb_madv.Items.Add(item);
                }
                reader.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi tham chiếu đến danh sách dịch vụ");
            }
        }

        bool cnbkdv = false;
        String mabkdv = "";
        String madv_bk = "";
        DateTime nsddv;
        private void bt_bkdv_Click(object sender, RoutedEventArgs e)
        {
            setbtbkdv(true);
            cnbkdv = true;
            addmadv();
            addmabk();
        }

        private void bt_bqbkdv_Click(object sender, RoutedEventArgs e)
        {
            setbtbkdv(false);
            comb_madv.SelectedItem = null;
            comb_mabk.SelectedItem = null;
        }

        private void bt_bqcnbkdv_Click(object sender, RoutedEventArgs e)
        {
            grdt_bkdv.SelectedItem = false;
            bt_bkdv.Visibility = Visibility.Visible;
            bt_bqcnbkdv.Visibility = Visibility.Hidden;
        }

        private void comb_madv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = comb_madv.SelectedItem as ComboBoxItem;
            if (comb_madv.SelectedItem != null)
            {
                String sqlStr = "SELECT tendv,gia FROM tblDv where madv ='" + item.Content.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_tdv_bk.Text = reader.GetString(0);
                    txt_giadv_bk.Text = reader.GetDecimal(1).ToString();
                }
                reader.Close();
            }
        }

        private void comb_mabk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = comb_mabk.SelectedItem as ComboBoxItem;
            if (comb_mabk.SelectedItem != null)
            {
                String sqlStr = "SELECT maphong FROM tblBookings where mabooking ='" + item.Content.ToString().Trim() + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_maphong_bkdv.Text = reader.GetString(0);
                }
                reader.Close();
            }
        }

        private void grdt_bkdv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                bt_bqcnbkdv.Visibility = Visibility.Visible;
                bt_bkdv.Visibility = Visibility.Hidden;
                if (grdt_bkdv.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_bkdv.CurrentItem;
                mabkdv = row["mabooking"].ToString();
                madv_bk = row["madv"].ToString();
                String sqlStr = "SELECT tendv,gia FROM tblDv where madv ='" + madv_bk + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_tdv_bk.Text = reader.GetString(0);
                    txt_giadv_bk.Text = reader.GetDecimal(1).ToString();
                }
                reader.Close();
                sqlStr = "SELECT maphong FROM tblBookings where mabooking ='" + mabkdv + "'";
                cmd = new SqlCommand(sqlStr, Conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    txt_maphong_bkdv.Text = reader.GetString(0);
                }
                reader.Close();
                //ComboBoxItem item = new ComboBoxItem();
                //item.Content = mabkdv;
                //comb_mabk.SelectedItem = item;
                comb_mabk.Text = row["mabooking"].ToString();
                //var item1 = comb_madv.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row["madv"].ToString().Trim());
                //if (item1 != null)
                //{
                //    comb_madv.SelectedItem = item1;
                //}
                comb_madv.Text = row["madv"].ToString();
                txt_sldv_bk.Text = row["soluong"].ToString();
                nsddv = (DateTime)row["ngaydung"];
                dp_nddv.SelectedDate = nsddv;
                txt_gcbkdv.Text = row["ghichu"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_sbkdv_Click(object sender, RoutedEventArgs e)
        {
            setbtbkdv(true);
            comb_mabk.IsEnabled = false;
            bt_bqcnbkdv.Visibility = Visibility.Hidden;
            cnbkdv = false;
            addmadv();
            addmabk();
        }

        private void bt_cnbkdv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cnbkdv)
                {
                    //ComboBoxItem it = comb_mabk.SelectedItem as ComboBoxItem;
                    //ComboBoxItem it1 = comb_madv.SelectedItem as ComboBoxItem;
                    if(comb_mabk.SelectedItem != null && comb_madv.SelectedItem != null)
                    {
                        String mabk = comb_mabk.Text; mabk = mabk.Trim(); //it.Content.ToString();
                        String madv = comb_madv.Text; madv = madv.Trim(); //.Content.ToString();
                        String nsd = dp_nddv.SelectedDate.Value.ToString("yyyy-MM-dd");
                        String sqlStr = "";
                        sqlStr = "Insert Into tblBookingDv(mabooking,madv,soluong,ngaydung,trangthai,ghichu)values('" + mabk + "','" + madv + "'," + int.Parse(txt_sldv_bk.Text) + ",'" + nsd + "','Chưa thanh toán','" + txt_gcbkdv.Text + "')";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_bkdv, "tblBookingDv");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn mã booking phòng và mã dịch vụ");
                    }

                }
                else
                {
                    //ComboBoxItem it = comb_madv.SelectedItem as ComboBoxItem;
                    if(comb_madv.SelectedItem != null)
                    {
                        String madv = comb_madv.Text; madv = madv.Trim();
                        String nsd = dp_nddv.SelectedDate.Value.ToString("yyyy-MM-dd");
                        String sqlStr = "";
                        sqlStr = "Update tblBookingDV Set madv = '" + madv + "', soluong = " + int.Parse(txt_sldv_bk.Text) + ",ngaydung = '" + nsd + "',ghichu = '" + txt_gcbkdv.Text + "' Where (mabooking = '" + mabkdv + "') AND (madv ='" + madv + "')";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_bkdv, "tblBookingDv");
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn mã dịch vụ");
                    }

                }
                comb_madv.SelectedItem = null;
                comb_mabk.SelectedItem = null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            setbtbkdv(false);
            bt_bqcnbkdv.Visibility = Visibility.Hidden;
        }

        private void bt_hbkdv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn hủy dịch vụ ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DateTime now = DateTime.Now;
                    if(now < nsddv)
                    {
                        String sqlStr = "";
                        sqlStr = "DELETE FROM tblBookingDv Where (mabooking = '" + mabkdv + "') AND (madv ='" + madv_bk + "')";
                        SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                        cmd.ExecuteNonQuery();
                        NapDuLieuTuMayChu(grdt_bkdv, "tblBookingDv");
                    }
                    else
                    {
                        MessageBox.Show("Dịch vụ đã được sử dụng\nKhông thể hủy");
                    }                   
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi hủy dịch vụ\n" + ex.Message);
            }
        }

        private void bt_rfbkdv_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_bkdv, "tblBookingDv");
            bt_bqcnbkdv.Visibility = Visibility.Hidden;
            bt_bkdv.Visibility = Visibility.Visible;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //Trả phòng, thanh toán

        private void setbttp(bool set)
        {
            bt_cntt.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqtt.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            txt_tienp.IsEnabled = set;
            txt_tiendv.IsEnabled = set;
            txt_thue.IsEnabled = set;
            dp_nttt.IsEnabled = set;
            comb_giamgia.IsEnabled = set;
            txt_tongtien.IsEnabled = set;
            comb_phongdangdung.IsEnabled = set;
            set = !set;
            bt_thanhtoan.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            

            grdt_phongdangsd.IsEnabled = set;
        }

        private void addphongdangdung()
        {
            try
            {
                comb_phongdangdung.Items.Clear();
                String sqlStr = "SELECT maphong FROM tblPhong Where trangthai = 'Đang sử dụng'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string maphong = reader.GetString(0);
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = maphong;
                    comb_phongdangdung.Items.Add(item);
                }
                reader.Close();
            }
            catch
            {
                MessageBox.Show("Lỗi tham chiếu đến danh sách phòng");
            }
        }

        private void NapDuLieuPhongDangSuDungTuMayChu(DataGrid grdt, String table)
        {
            grdt.ItemsSource = null;
            if (Conn.State != ConnectionState.Open) return;

            String SqlStr = "Select * from " + table + " where trangthai = 'Đang sử dụng'";
            SqlDataAdapter adapter = new SqlDataAdapter(SqlStr, Conn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, table);
            DataSource = dataSet.Tables[table];
            grdt.ItemsSource = DataSource.DefaultView;

        }

        int songayo = 0;
        double tienphong = 0;
        double giaphong = 0;
        double giadv = 0;
        double tiendv = 0;
        double thue = 0;
        double tiendv1 = 0;
        double giamgia = 0;
        double tiengiam = 0;
        double tong = 0;
        double tamtinh = 0;
        
        private void bt_thanhtoan_Click(object sender, RoutedEventArgs e)
        {
            setbttp(true);
        }

        private void bt_bqtt_Click(object sender, RoutedEventArgs e)
        {
            setbttp(false);
            dp_nttt.SelectedDate = null;
            comb_giamgia.SelectedItem = null;
            tiengiam = 0;
        }

        String makh_tt = "";
        String maphong_tt = "";
        String mabooking_tt = "";
        private void comb_phongtra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tienphong = 0;
                tiendv = 0;
                thue = 0;
                tong = 0;

                ComboBoxItem item = comb_phongdangdung.SelectedItem as ComboBoxItem;
                if (comb_phongdangdung.SelectedItem != null)
                {
                    comb_dvdsd.Items.Clear();
                    String sqlStr = "SELECT * FROM tblBookings where (maphong = '" + item.Content.ToString().Trim() + "') AND (trangthai = 'Chưa thanh toán')";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mabooking_tt = reader.GetString(0);
                        txt_mabk_tt.Text = mabooking_tt;
                        makh_tt = reader.GetString(1);
                        txt_makh_tt.Text = makh_tt;
                        maphong_tt = reader.GetString(2);
                        dp_nnp_tt.SelectedDate = reader.GetDateTime(3);
                        dp_ntp_tt.SelectedDate = reader.GetDateTime(4);
                    }
                    reader.Close();

                    sqlStr = "SELECT * FROM tblKh where makh = '" + makh_tt + "'";
                    cmd = new SqlCommand(sqlStr, Conn);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txt_tenkh_tt.Text = reader.GetString(1);
                        txt_dckh_tt.Text = reader.GetString(2);
                        txt_sdtkh_tt.Text = reader.GetString(3);
                        txt_gtkh_tt.Text = reader.GetString(4);
                        txt_gckh_tt.Text = reader.GetString(5);
                    }
                    reader.Close();

                    DateTime now = DateTime.Now;
                    dp_nttt.SelectedDate = now;
                    songayo = (int)(now - dp_nnp_tt.SelectedDate.Value).TotalDays;
                    txt_sno.Text = songayo.ToString();
                    sqlStr = "SELECT sophong,gia FROM tblPhong where maphong = '" + maphong_tt + "'";
                    cmd = new SqlCommand(sqlStr, Conn);
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txt_sophong_tt.Text = reader.GetString(0);
                        giaphong = decimal.ToDouble(reader.GetDecimal(1));
                        tienphong = decimal.ToDouble(reader.GetDecimal(1)) * songayo;
                        txt_tienp.Text = tienphong.ToString();
                    }
                    reader.Close();

                    sqlStr = "SELECT COUNT(mabooking) FROM tblBookingDv WHERE (mabooking = '" + mabooking_tt + "') AND (trangthai = 'Chưa thanh toán')";
                    cmd = new SqlCommand(sqlStr, Conn);
                    reader = cmd.ExecuteReader();
                    int sldv = 0;
                    while (reader.Read())
                    {
                        sldv = reader.GetInt32(0);
                    }
                    reader.Close();
                    String[] madv = new String[sldv];
                    int[] sl = new int[sldv];

                    if(sldv > 0)
                    {
                        sqlStr = "SELECT madv,soluong FROM tblBookingDv where (mabooking = '" + mabooking_tt + "') AND (trangthai = 'Chưa thanh toán')";
                        cmd = new SqlCommand(sqlStr, Conn);
                        reader = cmd.ExecuteReader();
                        int a = 0;
                        while (reader.Read())
                        {
                            madv[a] = reader.GetString(0);
                            sl[a] = reader.GetInt32(1);
                            a++;
                        }
                        reader.Close();
                        for (int i = 0; i < sldv; i++)
                        {
                            sqlStr = "SELECT tendv,gia FROM tblDv where madv = '" + madv[i] + "'";
                            cmd = new SqlCommand(sqlStr, Conn);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                String tendv = reader.GetString(0);
                                ComboBoxItem it = new ComboBoxItem();
                                it.Content = tendv;
                                comb_dvdsd.Items.Add(it);
                                giadv = decimal.ToDouble(reader.GetDecimal(1));
                            }
                            tiendv += giadv * sl[i];
                            reader.Close();
                        }
                    }
                    tiendv1 = tiendv;
                    txt_tiendv.Text = tiendv.ToString();
                    thue = (tienphong + tiendv) * 0.1;
                    txt_thue.Text = thue.ToString();
                    tong = tienphong + tiendv + thue - (tienphong + tiendv + thue) * giamgia;
                    txt_tongtien.Text = tong.ToString();
                    tamtinh = tienphong + tiendv;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void dp_nttt_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dp_nttt.SelectedDate != null)
            {
                tienphong = 0;
                tiendv1 = 0;
                thue = 0;
                tong = 0;

                songayo = (int)(dp_nttt.SelectedDate.Value - dp_nnp_tt.SelectedDate.Value).TotalDays;
                txt_sno.Text = songayo.ToString();

                tienphong = giaphong * songayo;
                txt_tienp.Text = tienphong.ToString();

                thue = thue = (tienphong + tiendv1) * 0.1;
                txt_thue.Text = thue.ToString();

                tong = tienphong + tiendv + thue - (tienphong + tiendv + thue) * giamgia;
                txt_tongtien.Text = tong.ToString();
                tamtinh = tienphong + tiendv1;
            }

        }

        private void comb_giamgia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem it1 = comb_giamgia.SelectedItem as ComboBoxItem;
            if (comb_giamgia.SelectedItem != null)
            {
                tienphong = 0;
                tiendv1 = 0;
                thue = 0;
                tong = 0;

                txt_sno.Text = songayo.ToString();
                tienphong = giaphong * songayo;
                txt_tienp.Text = tienphong.ToString();
                thue = thue = (tienphong + tiendv1) * 0.1;
                txt_thue.Text = thue.ToString();

                String gg = it1.Content.ToString();
                gg = gg.Substring(0, gg.Length - 1);
                int giam = int.Parse(gg);
                giamgia = (double)giam / (double)100;
                tong = tienphong + tiendv + thue - (tienphong + tiendv + thue) * giamgia;
                txt_tongtien.Text = tong.ToString();
                tamtinh = tienphong + tiendv1;
                tiengiam = (tienphong + tiendv + thue) * giamgia;
            }
        }

        private void grdt_phongdangsd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tienphong = 0;
                tiendv = 0;
                thue = 0;
                tong = 0;

                if (grdt_phongdangsd.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_phongdangsd.CurrentItem;
                comb_dvdsd.Items.Clear();
                var item1 = comb_phongdangdung.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == row["maphong"].ToString().Trim());
                if (item1 != null)
                {
                    comb_phongdangdung.SelectedItem = item1;
                }
                String sqlStr = "SELECT * FROM tblBookings where (maphong = '" + row["maphong"].ToString() + "') AND (trangthai = 'Chưa thanh toán')";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    mabooking_tt = reader.GetString(0);
                    txt_mabk_tt.Text = mabooking_tt;
                    makh_tt = reader.GetString(1);
                    txt_makh_tt.Text = makh_tt;
                    maphong_tt = reader.GetString(2);
                    dp_nnp_tt.SelectedDate = reader.GetDateTime(3);
                    dp_ntp_tt.SelectedDate = reader.GetDateTime(4);
                }
                reader.Close();

                sqlStr = "SELECT * FROM tblKh where makh = '" + makh_tt + "'";
                cmd = new SqlCommand(sqlStr, Conn);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txt_tenkh_tt.Text = reader.GetString(1);
                    txt_dckh_tt.Text = reader.GetString(2);
                    txt_sdtkh_tt.Text = reader.GetString(3);
                    txt_gtkh_tt.Text = reader.GetString(4);
                    txt_gckh_tt.Text = reader.GetString(5);
                }
                reader.Close();

                DateTime now = DateTime.Now;
                dp_nttt.SelectedDate = now;
                songayo = (int)(now - dp_nnp_tt.SelectedDate.Value).TotalDays;
                txt_sno.Text = songayo.ToString();
                sqlStr = "SELECT sophong,gia FROM tblPhong where maphong = '" + maphong_tt + "'";
                cmd = new SqlCommand(sqlStr, Conn);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txt_sophong_tt.Text = reader.GetString(0);
                    giaphong = decimal.ToDouble(reader.GetDecimal(1));
                    tienphong = decimal.ToDouble(reader.GetDecimal(1)) * songayo;
                    txt_tienp.Text = tienphong.ToString();
                }
                reader.Close();

                sqlStr = "SELECT COUNT(mabooking) FROM tblBookingDv WHERE (mabooking = '" + mabooking_tt + "') AND (trangthai = 'Chưa thanh toán')";
                cmd = new SqlCommand(sqlStr, Conn);
                reader = cmd.ExecuteReader();
                int sldv = 0;
                while (reader.Read())
                {
                    sldv = reader.GetInt32(0);
                }
                reader.Close();
                String[] madv = new String[sldv];
                int[] sl = new int[sldv];

                if (sldv > 0)
                {
                    sqlStr = "SELECT madv,soluong FROM tblBookingDv where (mabooking = '" + mabooking_tt + "') AND (trangthai = 'Chưa thanh toán')";
                    cmd = new SqlCommand(sqlStr, Conn);
                    reader = cmd.ExecuteReader();
                    int a = 0;
                    while (reader.Read())
                    {
                        madv[a] = reader.GetString(0);
                        sl[a] = reader.GetInt32(1);
                        a++;
                    }
                    reader.Close();
                    for (int i = 0; i < sldv; i++)
                    {
                        sqlStr = "SELECT tendv,gia FROM tblDv where madv = '" + madv[i] + "'";
                        cmd = new SqlCommand(sqlStr, Conn);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            String tendv = reader.GetString(0);
                            ComboBoxItem it = new ComboBoxItem();
                            it.Content = tendv;
                            comb_dvdsd.Items.Add(it);
                            giadv = decimal.ToDouble(reader.GetDecimal(1));
                        }
                        tiendv += giadv * sl[i];
                        reader.Close();
                    }
                }
                tiendv1 = tiendv;
                txt_tiendv.Text = tiendv.ToString();
                thue = (tienphong + tiendv) * 0.1;
                txt_thue.Text = thue.ToString();
                tong = tienphong + tiendv + thue - (tienphong + tiendv + thue) * giamgia;
                txt_tongtien.Text = tong.ToString();
                tamtinh = tienphong + tiendv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_cntt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;
                String nthd_tt = now.ToString("yyyy-MM-dd");
                String nttt = "";
                if (dp_nttt.SelectedDate == null)
                {
                    nttt = now.ToString("yyyy-MM-dd");
                }
                else
                {
                    nttt = dp_nttt.SelectedDate.Value.ToString("yyyy-MM-dd");
                }

                String sqlStr = "";
                sqlStr = "Update tblPhong Set trangthai = 'Trống' Where maphong = '" + maphong_tt + "';" +
                         "Update tblBookings Set trangthai = 'Đã thanh toán' Where mabooking = '" + mabooking_tt + "';" +
                         "Update tblBookingDv Set trangthai = 'Đã thanh toán' Where mabooking = '" + mabooking_tt + "';" +
                         "Insert Into tblHoadon(mahoadon,mabooking,ngaytrathucte,ngaytaohd,tamtinh,thue,giamgia,tong)values('" + Ma.HoaDon() + "','" + txt_mabk_tt.Text + "','" + nttt + "','" + nthd_tt + "',@tamtinh, @thue, @giamgia, @tong);";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                cmd.Parameters.AddWithValue("@tamtinh", tamtinh);
                cmd.Parameters.AddWithValue("@thue", double.Parse(txt_thue.Text));
                cmd.Parameters.AddWithValue("@giamgia", tiengiam);
                cmd.Parameters.AddWithValue("@tong", tong);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Thanh toán thành công");
                //Chuyển qua tap hóa đơn và hiển thị thông tin hóa đơn
                tab_tt.IsSelected = false;
                tab_hd.IsSelected = true;

                txt_mabk_hd.Text = mabooking_tt;
                DateTime date1 = DateTime.ParseExact(nttt, "yyyy-MM-dd", null);
                dp_ntp_hd.SelectedDate = date1;
                DateTime date2 = DateTime.ParseExact(nthd_tt, "yyyy-MM-dd", null);
                dp_nthd.SelectedDate = date2;
                txt_ttt.Text = tamtinh.ToString();
                txt_tt.Text = txt_thue.Text;
                txt_tgg.Text = tiengiam.ToString();
                txt_tongtien_hd.Text = tong.ToString();
                txt_tenkh_hd.Text = txt_tenkh_tt.Text;
                txt_dckh_hd.Text = txt_dckh_tt.Text;
                txt_sdtkh_hd.Text = txt_sdtkh_tt.Text;

                NapDuLieuTuMayChu(grdt_hd, "tblHoadon");

            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán\n" + ex.Message);
            }
            comb_giamgia.SelectedItem = null;
            tiengiam = 0;
            setbttp(false);
            addphongdangdung();
            NapDuLieuPhongDangSuDungTuMayChu(grdt_phongdangsd, "tblPhong");

            
        }

        //Hóa đơn
        private void setbthd(bool set)
        {
            bt_cnhd.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_bqhd.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            txt_mabk_hd.IsEnabled = set;
            dp_ntp_hd.IsEnabled = set;
            dp_nthd.IsEnabled = set;
            txt_ttt.IsEnabled = set;
            txt_tt.IsEnabled = set;
            txt_tgg.IsEnabled = set;
            txt_tongtien_hd.IsEnabled = set;
            txt_gchd.IsEnabled = set;
            set = !set;
            bt_suahd.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_xoahd.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_rfhd.Visibility = set ? Visibility.Visible : Visibility.Hidden;
            bt_xuathd.Visibility = set ? Visibility.Visible : Visibility.Hidden;

            grdt_hd.IsEnabled = set;
        }

        private void bt_suahd_Click(object sender, RoutedEventArgs e)
        {
            setbthd(true);
        }

        private void bt_bqhd_Click(object sender, RoutedEventArgs e)
        {
            setbthd(false);
        }
        String mahd = "";
        private void grdt_hd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grdt_hd.CurrentItem == null) { return; }
                DataRowView row = (DataRowView)grdt_hd.CurrentItem;
                mahd = row["mahoadon"].ToString();
                txt_mabk_hd.Text = row["mabooking"].ToString();
                DateTime ntp = (DateTime)row["ngaytrathucte"];
                dp_ntp_hd.SelectedDate = ntp;
                DateTime nthd = (DateTime)row["ngaytaohd"];
                dp_nthd.SelectedDate = nthd;
                txt_ttt.Text = row["tamtinh"].ToString();
                txt_tt.Text = row["thue"].ToString();
                txt_tgg.Text = row["giamgia"].ToString();
                txt_tongtien_hd.Text = row["tong"].ToString();
                txt_gchd.Text = row["ghichu"].ToString();

                //Lấy mã khách hàng
                String sqlStr = "SELECT makh FROM tblBookings where mabooking = '" + row["mabooking"].ToString() + "'";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                SqlDataReader reader = cmd.ExecuteReader();
                String makh = "";
                if (reader.Read())
                {
                    makh = reader.GetString(0);
                }
                reader.Close();
                //Lấy thông tin khách hàng
                sqlStr = "SELECT tenkh,diachi,sdt FROM tblKh where makh = '" + makh + "'";
                cmd = new SqlCommand(sqlStr, Conn);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txt_tenkh_hd.Text = reader.GetString(0);
                    txt_dckh_hd.Text = reader.GetString(1);
                    txt_sdtkh_hd.Text = reader.GetString(2);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mục Trống\n" + ex.Message);
            }
        }

        private void bt_cnhd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String ntp = dp_ntp_hd.SelectedDate.Value.ToString("yyyy-MM-dd");
                String nthd = dp_nthd.SelectedDate.Value.ToString("yyyy-MM-dd");
                String sqlStr = "";
                sqlStr = "Update tblHoadon Set mabooking = '" + txt_mabk_hd.Text + "', ngaytrathucte = '" + ntp + "', ngaytaohd = '" + nthd + "', tamtinh = @tamtinh, thue = @thue, giamgia = @giamgia, tong = @tong, ghichu = '" + txt_gchd.Text + "' Where (mahoadon ='" + mahd + "') OR (mabooking = '" + txt_mabk_hd.Text + "')";
                SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                cmd.Parameters.AddWithValue("@tamtinh", double.Parse(txt_ttt.Text));
                cmd.Parameters.AddWithValue("@thue", double.Parse(txt_tt.Text));
                cmd.Parameters.AddWithValue("@giamgia", double.Parse(txt_tgg.Text));
                cmd.Parameters.AddWithValue("@tong", double.Parse(txt_tongtien_hd.Text));
                cmd.ExecuteNonQuery();
                NapDuLieuTuMayChu(grdt_hd, "tblHoadon");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            setbthd(false);
        }

        private void bt_rfhd_Click(object sender, RoutedEventArgs e)
        {
            NapDuLieuTuMayChu(grdt_hd, "tblHoadon");
        }

        private void bt_xoahd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    String sqlStr = "";
                    sqlStr = "DELETE From tblHoadon Where mahoadon = '" + mahd + "'";
                    SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                    cmd.ExecuteNonQuery();
                    NapDuLieuTuMayChu(grdt_hd, "tblHoadon");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi xóa hóa đơn\n" + ex.Message);
            }
        }
    }
}
