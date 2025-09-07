using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    internal class csProveedor
    {
        int idProveedor { get; set; }
        string nombreProveedor { get; set; }
        string ruc { get; set; }
        string telefono { get; set; }
        string correo { get; set; }
        string direccion { get; set; }

        public int IdProveedor
        {
            get { return idProveedor; }
            set { idProveedor = value; }
        }
        public string NombreProveedor
        {
            get { return nombreProveedor; }
            set { nombreProveedor = value; }
        }
        public string RUC
        {
            get { return ruc; }
            set { ruc = value; }
        }
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; }
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
        public csProveedor(string nombre, string ced, string tel, string cor, string dir)
        {
            NombreProveedor = nombre;
            RUC = ced;
            Telefono = tel;
            Correo = cor;
            Direccion = dir;
        }

        public csProveedor(int id, string nombre, string ced, string tel, string cor, string dir)
        {
            IdProveedor = id;
            NombreProveedor = nombre;
            RUC = ced;
            Telefono = tel;
            Correo = cor;
            Direccion = dir;
        }

        public bool agregarProveedor()
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD(
                "Insert into Proveedor (NombreProveedor, RUC, Telefono, Correo, Direccion) " +
                "values (@Nombre, @Ruc, @Telefono, @Correo , @Direccion)",
                new SqlParameter("@Nombre", NombreProveedor),
                new SqlParameter("@Ruc", RUC),
                new SqlParameter("@Telefono", Telefono),
                new SqlParameter("@Correo", Correo),
                new SqlParameter("@Direccion", Direccion));
        }
        public bool editarProveedor(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.editarBD(
                "UPDATE Proveedor SET NombreProveedor=@Nombre', RUC=@Ruc, " +
                "Telefono=@Telefono, Correo=@Correo, Direccion=@Direccion " +
                "WHERE IdProveedor=@Id",
                new SqlParameter("@Nombre", NombreProveedor),
                new SqlParameter("@Ruc", RUC),
                new SqlParameter("@Telefono", Telefono),
                new SqlParameter("@Correo", Correo),
                new SqlParameter("@Direccion", Direccion),
                new SqlParameter("@Id", id));
        }
        public bool eliminarProveedor(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.eliminarBD("DELETE FROM Proveedor WHERE IdProveedor=@Id", id);
        }

    }
}
