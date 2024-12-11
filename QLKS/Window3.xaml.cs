using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
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

namespace QLKS
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }

        private void bt_boqua_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        string ConnectionStrin = "";
        SqlConnection Conn = new SqlConnection();

        private void bt_doimk_Click(object sender, RoutedEventArgs e)
        {
            if((txt_mkcu.Password.Trim() != "") && (txt_mkmoi1.Password.Trim() != "") && (txt_mkmoi2.Password.Trim() != ""))
            {
                if(txt_mkmoi1.Password != txt_mkmoi2.Password)
                {
                    lb_thongbao.Content = "Mật khẩu nhập lại không khớp";
                }
                else
                {
                    try
                    {
                        ConnectionStrin = @"Data Source=.\NAM;Initial Catalog=qlks;Integrated Security=True;";
                        Conn.ConnectionString = ConnectionStrin;
                        Conn.Open();

                        string sql = "Select * From tblUser Where (id =" + User.id + ") and (matkhau = '" + Mahoa.EncodeMD5(txt_mkcu.Password) + "')";
                        SqlDataAdapter adapter = new SqlDataAdapter(sql, Conn);
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        if (dataSet.Tables[0].Rows.Count <= 0)
                        {
                            lb_thongbao.Content = "Mật khẩu cũ không chính xác";
                        }
                        else
                        {
                            string sqlStr = "";
                            sqlStr = "Update tblUser Set matkhau = '" + Mahoa.EncodeMD5(txt_mkmoi2.Password) + "' Where id =" + User.id;
                            SqlCommand cmd = new SqlCommand(sqlStr, Conn);
                            cmd.ExecuteNonQuery();
                            this.Close();
                            MessageBox.Show("Đổi mật khẩu thành công");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi kết nối đến máy chủ\n" + ex.Message);
                    }
                    Conn.Close();
                }

            }
            else
            {
                lb_thongbao.Content = "Chưa nhập mật khẩu cũ hoặc mật khẩu mới";
                if(txt_mkmoi2.Password.Trim() == "")
                {
                    txt_mkmoi2.Focus();
                }
                if (txt_mkmoi1.Password.Trim() == "")
                {
                    txt_mkmoi1.Focus();
                }
                if (txt_mkcu.Password.Trim() == "")
                {
                    txt_mkcu.Focus();
                }
            }
            
        }

        private void txt_mkcu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txt_mkmoi1.Focus();
            }
        }

        private void txt_mkmoi1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txt_mkmoi2.Focus();
            }
        }
    }
}
