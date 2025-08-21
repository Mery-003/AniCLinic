using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    internal class ConexionBD
    {
        // 👇 Cambia a tu servidor si hace falta
        private static string cadena =
@"Data Source=.\SQLEXPRESS;Initial Catalog=AniClinic;User ID=sa;Password=abcdef;Encrypt=False";




        private SqlConnection cn = new SqlConnection(cadena);

        public SqlConnection AbrirConexion()
        {
            if (cn.State == System.Data.ConnectionState.Closed) cn.Open();
            return cn;
        }

        public void CerrarConexion()
        {
            if (cn.State == System.Data.ConnectionState.Open) cn.Close();
        }
    }
}