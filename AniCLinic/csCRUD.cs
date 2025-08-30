using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    internal class csCRUD
    {
        public csConexionBD conexion;
        SqlCommand oCom;
        SqlDataAdapter oDA;
        DataTable oDT;
        SqlDataReader oDTR;

        public csCRUD() { }

        // === EXISTENTE (sin parámetros)
        public DataTable cargarBDData(string sentencia)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oDA = new SqlDataAdapter(sentencia, conexion.obtenerConexion());
                oDT = new DataTable();
                oDA.Fill(oDT);
                conexion.cerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            return oDT;
        }

        // === NUEVO (con parámetros)
        public DataTable cargarBDData(string sentencia, params SqlParameter[] parametros)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                if (parametros != null)
                {
                    foreach (var p in parametros)
                        oCom.Parameters.Add(p);
                }
                oDA = new SqlDataAdapter(oCom);
                oDT = new DataTable();
                oDA.Fill(oDT);
                conexion.cerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            return oDT;
        }

        public bool agregarBD(string sentencia, params SqlParameter[] parametros)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                foreach (var parametro in parametros)
                    oCom.Parameters.Add(parametro);
                oCom.ExecuteNonQuery();
                conexion.cerrarConexion();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public bool eliminarBD(string sentencia, int id)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                oCom.Parameters.AddWithValue("@id", id);
                oCom.ExecuteNonQuery();
                conexion.cerrarConexion();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public bool editarBD(string sentencia, int id, params SqlParameter[] parametros)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                oCom.Parameters.AddWithValue("@id", id);
                foreach (var parametro in parametros)
                    oCom.Parameters.Add(parametro);
                oCom.ExecuteNonQuery();
                conexion.cerrarConexion();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public SqlDataReader EjecutarQuery(string sentencia)
        {
            SqlDataReader reader = null;
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                reader = oCom.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar consulta: " + ex.Message);
            }
            return reader;
        }

        public int login(string sentencia, string user, string pass)
        {
            try
            {
                conexion = new csConexionBD();
                conexion.abrirConexion();
                oCom = new SqlCommand(sentencia, conexion.obtenerConexion());
                oDTR = oCom.ExecuteReader();
                while (oDTR.Read())
                    if (oDTR["Usuario"].ToString() == user && oDTR["Password"].ToString() == pass)
                        return 1;
                conexion.cerrarConexion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return 0;
            }
            return 0;
        }

        // === Helper para AgregarPaciente (carga TODO por ID)
        public DataTable ObtenerMascotaDetalladaPorId(int idMascota)
        {
            string sql = @"
SELECT
    M.IdMascota,
    M.Nombre,
    M.Especie,
    M.Raza,
    M.Sexo,
    M.Edad,
    M.PesoKg,
    M.Discapacidad,
    M.Imagen,
    P.IdPersona,
    P.Cedula,
    P.Nombre AS NombreProp,
    P.Apellido AS ApellidoProp,
    (P.Nombre + ' ' + P.Apellido) AS Propietario
FROM Mascota M
INNER JOIN Persona P ON P.IdPersona = M.IdPersona
WHERE M.IdMascota = @Id;";
            return cargarBDData(sql, new SqlParameter("@Id", idMascota));
        }
    }
}
