using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Arayuz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglanti = new SqlConnection(@"Data Source=ISKENDER;Initial Catalog=PersonelVeriTabani;Integrated Security=True");

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                baglanti.Open();
                Form2 frm = new Form2();

                string sql = "Select * from Tbl_giris where KullaniciAd=@adi AND Sifre=@sifresi";
                SqlParameter SqlAd = new SqlParameter("adi", KullanıcıTxt.Text.Trim());
                SqlParameter Sqlsifre = new SqlParameter("sifresi", SifreTxt.Text.Trim());
                SqlCommand komut = new SqlCommand(sql, baglanti);
                komut.Parameters.Add(SqlAd);
                komut.Parameters.Add(Sqlsifre);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(komut);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    
                    frm.Show();
                    this.Hide();
                    frm.adiTanım = KullanıcıTxt.Text;
                    frm.sifreniGir = SifreTxt.Text;
                    
                }
                else
                {
                    MessageBox.Show("Kullanıcı Adı veya parolanız yanlış.");
                }
  
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
            finally
            {
                baglanti.Close();
            }
            
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SifreTxt.PasswordChar = '\0';
            }
            else
            {
                SifreTxt.PasswordChar = '*';
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SifreTxt.PasswordChar = '*';
        }
    }
}
