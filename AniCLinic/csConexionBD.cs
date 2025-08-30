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

        public void abrirConexion()
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
            }
        }

        public void cerrarConexion()
        {
            try
            {
                oCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public SqlConnection obtenerConexion()
        {
            return oCon;
        }
    }
}
