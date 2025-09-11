using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;

namespace AniCLinic
{
    internal class csUsuario
    {
        int id { get; set; }
        string usuario { get; set; }
        string contraseña { get; set; }
        decimal sueldo { get; set; }
        bool admin { get; set; }
        public int Id 
        { 
            get { return id; } 
            set { id = value; } 
        }
        public string Usuario 
        {
            get { return usuario; }
            set { usuario = value; }
        }
        public string Contraseña
        {
            get { return contraseña; }
            set { contraseña = value; }
        }
        public decimal Sueldo
        {
            get { return sueldo; }
            set { sueldo = value; }
        }
        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }
        public csUsuario() { }
        public csUsuario(string user, string pass, decimal sueld, bool admin)
        {
            Usuario = user;
            Contraseña = pass;
            Sueldo = sueld;
            Admin = admin;
        }
        public csUsuario(int id, string user, string pass, decimal sueld, bool admin)
        {
            Id = id;
            Usuario = user;
            Contraseña = pass;
            Sueldo = sueld;
            Admin = admin;
        }

        public bool agregarUsuario(int idPers)
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD(
                "Insert into Empleados (IdPersona, Sueldo, Usuario, Password, Activo, Administrador) " +
                "Values(@IdPersona, @Sueldo, @User, @Pass, @Activo, @Admin)",
                new SqlParameter("@IdPersona", idPers),
                new SqlParameter("@Sueldo", Sueldo),
                new SqlParameter("@User", Usuario),
                new SqlParameter("@Pass", Contraseña),
                new SqlParameter("@Activo", 1),
                new SqlParameter("@Admin", Admin));
        }
        public bool editarUsuario(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.editarBD(
                "Update Empleados Set Sueldo = @Sueldo, Usuario = @User, Password = @Pass, Administrador = @Admin " +
                "Where IdEmpleado = @Id",
                new SqlParameter("@Sueldo", Sueldo),
                new SqlParameter("@User", Usuario),
                new SqlParameter("@Pass", Contraseña),
                new SqlParameter("@Admin", Admin),
                new SqlParameter("@Id", id));
        }
        public bool eliminarUsuario(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.eliminarBD("DELETE FROM Empleados WHERE IdEmpleado =@Id", id);
        }
    }
}
