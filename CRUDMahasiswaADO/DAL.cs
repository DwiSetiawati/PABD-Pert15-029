using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    class DAL
    {
        public static string GetConnectionString()
        {
            string connectionString = "Data Source=LAPTOP-PQSI1Q9H\\TIAA;Initial Catalog=DBAkademikADO;User ID=sa;Password=Semangat27;TrustServerCertificate=True";
            return connectionString;
        }

        SqlConnection conn = new SqlConnection(GetConnectionString());
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public static string GetLoacalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }



        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            object result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public void InsertMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pNIM", nim);
                command.Parameters.AddWithValue("@pNama", nama);
                command.Parameters.AddWithValue("@pAlamat", alamat);
                command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
                command.Parameters.AddWithValue("@pFoto", (object)foto ?? DBNull.Value);

                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@pNIM", nim);
            command.Parameters.AddWithValue("@pNama", nama);
            command.Parameters.AddWithValue("@pAlamat", alamat);
            command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
            command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
            command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);
            command.Parameters.AddWithValue("@pFoto", (object)foto ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
            cmd.Parameters.AddWithValue("@NIM", nim); // ← cek ini, kemungkinan "@NIM" bukan "@pNIM"
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.ExecuteNonQuery();
        }

        public void resetData()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string deleteQuery = "DELETE FROM mahasiswa;";
            SqlCommand cmdDelete = new SqlCommand(deleteQuery, conn);
            cmdDelete.ExecuteNonQuery();

            string insertQuery = @"
            INSERT INTO mahasiswa
            SELECT * FROM mahasiswa_backup;";
            SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
            cmdInsert.ExecuteNonQuery();
        }

        public void testInject(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string query = "Update mahasiswa set nama = 'HACKED' where NIM = " + nim;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        public DataTable GetMhsByNIM(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("pNIM", nim);
            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public void InsertLog(string message)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_LogMessage", conn);

            cmd.Parameters.AddWithValue("psn", message);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public DataTable getProdi()
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

            return dtProdi;
        }

        public DataTable getDataRekap(string prodi, DateTime tanggalMasuk)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand("sp_Report", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inProdi", prodi);
            cmd.Parameters.AddWithValue("@inTglMsuk", tanggalMasuk.Year.ToString());

            da = new SqlDataAdapter(cmd);

            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_DashBoard", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_DashBoardByTahun", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year);
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }
    }
}