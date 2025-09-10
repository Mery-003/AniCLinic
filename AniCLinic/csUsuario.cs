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
        string cargo { get; set; }
        decimal sueldo { get; set; }
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
        public string Cargo
        {
            get { return  cargo; }
            set { cargo = value; }
        }
        public decimal Sueldo
        {
            get { return sueldo; }
            set { sueldo = value; }
        }
        public csUsuario() { }
        public csUsuario(string user, string pass, string cargo, decimal sueld)
        {
            Usuario = user;
            Contraseña = pass;
            Cargo = cargo;
            Sueldo = sueld;
        }
        public csUsuario(int id, string user, string pass, string cargo, decimal sueld)
        {
            Id = id;
            Usuario = user;
            Contraseña = pass;
            Cargo = cargo;
            Sueldo = sueld;
        }

        public bool agregarUsuario(int idPer, int Admin)
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD(
                "Insert into Empleados (IdPersona, Sueldo, Usuario, Password, Activo, Administrador) " +
                "Values(@IdPersona, @Sueldo, @User, @Pass, @Activo, @Admin)",
                new SqlParameter("@IdPersona", idPer),
                new SqlParameter("@Sueldo", Sueldo),
                new SqlParameter("@User", Usuario),
                new SqlParameter("@Pass", Contraseña),
                new SqlParameter("@Activo", 1),
                new SqlParameter("@Admin", Admin));
        }
        public bool editarProveedor(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.editarBD(
                "Update Usuario Set Usuario = @User, Password = @Pass, Administrador = @Admin " +
                "Where IdEmpleado = @Id",
                new SqlParameter("@User", Usuario),
                new SqlParameter("@Pass", Contraseña));
        }
        public bool eliminarProveedor(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.eliminarBD("DELETE FROM Proveedor WHERE IdProveedor=@Id", id);
        }
    }
}
