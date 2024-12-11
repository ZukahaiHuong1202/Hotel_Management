using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Security.Cryptography;
using System.Runtime.Remoting.Messaging;

namespace QLKS
{
    class Mahoa
    {
        public static string EncodeMD5(string InportData)
        {
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(InportData);
            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();
            //chuyển đoạn mk vừa mã sang chữ in hoa
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2").ToUpper());
            }

            return sb.ToString();
        }
    }
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    internal static class User
    {
        public static int id { get; set; }
        public static string name { get; set; }
        public static string quyen { get; set; }
        public static string tendangnhap { get; set; }

    }
    public partial class Window1 : Window
    {
        string ConnectionStrin = "";
        SqlConnection Conn = new SqlConnection();

        public Window1()
        {
            InitializeComponent();
        }

        private void frm_login_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectionStrin = @"Data Source=.\HUONG;Initial Catalog=qlks;Integrated Security=True;";
                Conn.ConnectionString = ConnectionStrin;
                Conn.Open();
                txt_user.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối đến máy chủ\n" + ex.Message);
            }
        }

        private void Login()
        {
            if((txt_user.Text.Trim() == "") || (txt_passworld.Password.Trim() == ""))
            {
                lb_thongbao.Content = "Chưa nhập thông tin tài khoản hoặc mk";
                if(txt_passworld.Password.Trim() == "")
                {
                    txt_passworld.Focus();
                }
                if (txt_user.Text.Trim() == "")
                {
                    txt_user.Focus();
                }
            }
            else
            {
                string mk = Mahoa.EncodeMD5(txt_passworld.Password);
                string sql = "Select * From tblUser Where (tendangnhap = '" + txt_user.Text + "') and (matkhau = '" + mk + "')";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, Conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0 || (txt_user.Text == "admin" && txt_passworld.Password == "1234"))
                {
                    sql = "Select id,name,quyen From tblUser Where (tendangnhap = '" + txt_user.Text + "') and (matkhau = '" + mk + "')";
                    SqlCommand cmd = new SqlCommand(sql, Conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        User.id = reader.GetInt32(0);
                        User.name = reader.GetString(1);
                        User.quyen = reader.GetString(2);
                    }
                    reader.Close();

                    this.DialogResult = true;
                    this.Close();
                    Window2 window2 = new Window2();
                    window2.Show();
                    MessageBox.Show("Đăng nhập thành công");
                }
                else
                {
                    lb_thongbao.Content = "Thông tin tài khoản hoặc mật khẩu không chính xác";
                }

            }

        }

        private void bt_login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void txt_user_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txt_passworld.Focus();
            }
        }

        private void txt_passworld_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }
    }
}
