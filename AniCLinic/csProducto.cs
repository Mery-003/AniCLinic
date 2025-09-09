using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;

namespace AniCLinic
{
    internal class csProducto
    {
        int idProducto { get; set; }
        int idProveedor { get; set; }
        string nombreProducto { get; set; }
        string descripcion { get; set; }
        string categoria { get; set; }
        decimal precioUnitario { get; set; }
        int cantidad { get; set; }
        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }
        public int IdProveedor
        {
            get { return idProveedor; }
            set { idProveedor = value; }
        }
        public string NombreProducto
        {
            get { return nombreProducto; }
            set { nombreProducto = value; }
        }
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }
        public string Categoria
        {
            get { return categoria; }
            set { categoria = value; }
        }
        public decimal PrecioUnitario
        {
            get { return precioUnitario; }
            set { precioUnitario = value; }
        }
        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        public csProducto(int idProv, string nombre, string descr, string cat, decimal preUni, int cant)
        {
            IdProveedor = idProv;
            NombreProducto = nombre;
            Descripcion = descr;
            Categoria = cat;
            PrecioUnitario = preUni;
            Cantidad = cant;
        }
        public csProducto(int idProd, int idProv, string nombre, string descr, string cat, decimal preUni, int cant)
        {
            IdProducto = idProd;
            IdProveedor = idProv;
            NombreProducto = nombre;
            Descripcion = descr;
            Categoria = cat;
            PrecioUnitario = preUni;
            Cantidad = cant;
        }
        public bool agregarProducto()
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD("Insert into Inventario (IdProveedor, NombreProducto, Descripcion, Categoria, PrecioUnitario, CantidadDisponible) " +
                "values (@IdProveedor, @NombreP, @Descripcion, @Categoria, @Precio, @Cantidad)", 
                new SqlParameter("@IdProveedor", IdProveedor),
                new SqlParameter("@NombreP", NombreProducto),
                new SqlParameter("@Descripcion", Descripcion),
                new SqlParameter("@Categoria", Categoria),
                new SqlParameter("@Precio", PrecioUnitario),
                new SqlParameter("@Cantidad", Cantidad));
        }
        public bool editarProducto(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.editarBD("Update Inventario set IdProveedor = @IdProveedor, NombreProducto = @Nombre, Descripcion = @Descripcion, " +
                "Categoria = @Categoria, PrecioUnitario = @Precio, CantidadDisponible = @Cantidad " +
                "Where IdProducto = @Id",
                new SqlParameter("@IdProveedor", IdProveedor),
                new SqlParameter("@Nombre", NombreProducto),
                new SqlParameter("@Descripcion", Descripcion),
                new SqlParameter("@Categoria", Categoria),
                new SqlParameter("@Precio", PrecioUnitario),
                new SqlParameter("@Cantidad", Cantidad),
                new SqlParameter("@Id", id));
        }
        public bool eliminarProducto(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.eliminarBD("DELETE FROM Inventario WHERE IdProducto=@Id", id);
        }
    }
}
