using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    internal class csConexionBD
    {
        SqlConnection oCon;
        SqlCommand oCom;
        SqlDataAdapter oDA;
        DataTable oDT;
        SqlDataReader oDTR;

        string servidor;
        string basedatos;
        string usuario;
        string clave;

        public csConexionBD(string server, string bd, string user, string pass)
        {
            servidor = server;
            basedatos = bd;
            usuario = user;
            clave = pass;
        }

        public csConexionBD()
        {
            servidor = "ROONY\\SQLEXPRESS";
            basedatos = "AniClinic";
            usuario = "Roony";
            clave = "reinaramon15";
        }

        private bool abrirConexion()
        {
            oCon = new SqlConnection();
            oCon.ConnectionString = "Server = " + servidor + "; Database = " + basedatos + "; User id = " + usuario + "; Password = " + clave;
            try
            {
                oCon.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        private bool cerrarConexion()
        {
            try
            {
                oCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public DataTable retornaRegistro(string sentencia)
        {
            abrirConexion();
            oCom = new SqlCommand(sentencia, oCon);
            oDA = new SqlDataAdapter(oCom);
            oDT = new DataTable();
            oDA.Fill(oDT);
            cerrarConexion();
            return oDT;
        }
        public int guardarRegistro(string sentencia, byte[] f)
        {
            int result = 0;
            try
            {
                abrirConexion();
                oCom = new SqlCommand(sentencia, oCon);
                oCom.Parameters.AddWithValue("@Foto", f);
                result = oCom.ExecuteNonQuery();
                cerrarConexion();
                return result;
            } catch (Exception ex)
            { 
                MessageBox.Show(ex.Message); 
                return result; 
            }
        }
        public int devolverID()
        {
            string sentencia = "SELECT MAX([ID Mascota]) AS UltimoID FROM tblMascotas;";
            int result = 0;
            try
            {
                abrirConexion();
                oCom = new SqlCommand(sentencia, oCon);
                oDTR = oCom.ExecuteReader();
                if (oDTR.Read())
                    if (!oDTR.IsDBNull(0))
                        result = oDTR.GetInt32(0);
                oDTR.Close();
                cerrarConexion();
                return result + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
    }
}
