using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{

    internal class csPropietario
    {
        string nombre { get; set; }
        string apellido { get; set; }
        string celular { get; set; }
        string cedula { get; set; }
        string correo { get; set; }
        string direccion { get; set; }
        byte[] foto { get; set; }

        public string Nombre
        {
            get { return nombre; } 
            set { nombre = value; }
        }
        public string Apellido
        {
            get { return apellido; }
            set { apellido = value; }
        }
        public string Celular 
        {
            get { return celular; }
            set { celular = value; }
        }
        public string Cedula
        {
            get { return cedula; }
            set { cedula = value; }
        }
        public string Correo
        {
            get { return correo; }
            set { correo = value; }
        }
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        public byte[] Foto
        {
            get { return foto; }
            set { foto = value; }
        }

        public csPropietario() { }

        public csPropietario(string nom, string ape, string cel, string ced, string cor, string dir, byte[] fot)
        {
            Nombre = nom;
            Apellido = ape;
            Celular = cel;
            Cedula = ced;
            Correo = cor;
            Direccion = dir;
            Foto = fot;
        }

        public bool agregarPropietario()
        {
            csCRUD conexionBD = new csCRUD();
            return conexionBD.agregarBD(
                "INSERT INTO Persona (Nombre, Apellido, Celular, Cedula, Correo, DireccionDomiciliaria, Imagen) " +
                "VALUES (@Nombre, @Apellido, @Celular, @Cedula, @Correo, @Direccion, @Foto)",
                new SqlParameter("@Nombre", Nombre),
                new SqlParameter("@Apellido", Apellido),
                new SqlParameter("@Celular", Celular),
                new SqlParameter("@Cedula", Cedula),
                new SqlParameter("@Correo", Correo),
                new SqlParameter("@Direccion", Direccion),
                new SqlParameter("@Foto", Foto));
        }

        public bool editarPropietario(int idPropietario)
        {
            csCRUD conexionBD = new csCRUD();
            return conexionBD.editarBD(
                "UPDATE Persona SET Nombre=@Nombre, Apellido=@Apellido, Celular=@Celular, Cedula=@Cedula, Correo=@Correo, DireccionDomiciliaria=@Direccion, Imagen=@Foto " +
                "WHERE IdPersona=@Id",
                idPropietario,
                new SqlParameter("@Nombre", Nombre),
                new SqlParameter("@Apellido", Apellido),
                new SqlParameter("@Celular", Celular),
                new SqlParameter("@Cedula", Cedula),
                new SqlParameter("@Correo", Correo),
                new SqlParameter("@Direccion", Direccion),
                new SqlParameter("@Foto", Foto));
        }

        public bool eliminarPropietario(int idPropietario)
        {
            csCRUD conexionBD = new csCRUD();
            return conexionBD.eliminarBD("DELETE FROM Persona WHERE IdPersona=@Id", idPropietario);
        }

        public int obtenerIdPropietario()
        {
            int idPropietario = 0;
            try
            {
                csCRUD conexionBD = new csCRUD();
                string query = "SELECT TOP 1 IdPersona FROM Persona ORDER BY IdPersona DESC";
                using (SqlDataReader reader = conexionBD.EjecutarQuery(query))
                {
                    if (reader != null && reader.Read())
                    {
                        idPropietario = Convert.ToInt32(reader["IdPersona"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el ID del propietario: " + ex.Message);
            }
            return idPropietario;
        }
    }
}