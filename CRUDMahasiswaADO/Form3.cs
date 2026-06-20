using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace CRUDMahasiswaADO
{

    public partial class Form3 : Form
    {
        DAL dbLogic = new DAL();
        static string connectionString = "Data Source=LAPTOP-PQSI1Q9H\\TIAA;Initial Catalog=DBAkademikADO;User ID=sa;Password=Semangat27;TrustServerCertificate=True";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public Form3()
        {
            InitializeComponent();

          
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCetak.Enabled = false;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("select namaprodi from programstudi", conn);
                cmd.CommandType = CommandType.Text;
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);
                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";
                cmbProdi.ValueMember = "namaprodi";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = cmbProdi.SelectedValue;
                cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = dtpTanggalMasuk.Value.Year.ToString();

                da = new SqlDataAdapter(cmd);

                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                dataGridView1.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            try
            {
                // Ambil nilai yang benar dari DataRowView
                string prodi = cmbProdi.SelectedValue.ToString();
                DateTime tglMasuk = dtpTanggalMasuk.Value;

                MessageBox.Show("Prodi: " + prodi + "\nTahun: " + tglMasuk.Year);

                Form2 frm2 = new Form2(prodi, tglMasuk);
                frm2.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
