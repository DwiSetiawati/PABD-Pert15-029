using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace CRUDMahasiswaADO
{
    public partial class Form2 : Form
    {
        static string connectionString =
            "Data Source=LAPTOP-PQSI1Q9H\\TIAA;" +
            "Initial Catalog=DBAkademikADO;" +
            "User ID=sa;" +
            "Password=Semangat27;" +
            "TrustServerCertificate=True";

        string prodi;
        DateTime tglmasuk;

        public Form2(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();
            prodi = Prodi;
            tglmasuk = TglMasuk;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_Report", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inProdi", prodi);
                    cmd.Parameters.AddWithValue("@inTglMsuk", tglmasuk.Year.ToString());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                MessageBox.Show("Baris: " + dt.Rows.Count); // cek dulu datanya ada

                // Langsung pakai DataTable, tanpa konversi ke List<Data>
                ListMahasiswa rpt = new ListMahasiswa();
                rpt.SetDataSource(dt); // ← pakai DataTable langsung
                crystalReportViewer3.ReportSource = rpt;
                crystalReportViewer3.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:\n" + ex.ToString());
            }
        }
    }
}