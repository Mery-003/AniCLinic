using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    internal class csVenta
    {
        int idVenta { get; set; }
        int idProducto { get; set; }
        int idPersona { get; set; }
        int idEmpl { get; set; }
        int cantidadVendida { get; set; }
        decimal precioUnitario { get; set; }
        public int IdVenta
        {
            get { return idVenta; }
            set { idVenta = value; }
        }
        public int IdProducto
        {
            get { return idProducto; }
            set { idProducto = value; }
        }
        public int IdPersona
        {
            get { return idPersona; }
            set { idPersona = value; }
        }
        public int IdEmpl
        {
            get { return idEmpl; }
            set { idEmpl = value; }
        }
        public int CantidadVendida
        {
            get { return cantidadVendida; }
            set { cantidadVendida = value; }
        }
        public decimal PrecioUnitario
        {
            get { return precioUnitario; }
            set { precioUnitario = value; }
        }

        public csVenta() { }
        public csVenta(int idPro, int idPer, int idEmp, int cant, decimal preUni)
        {
            IdProducto = idPro;
            IdPersona = idPer;
            IdEmpl = idEmp;
            CantidadVendida = cant;
            PrecioUnitario = preUni;
        }
        public bool agregarVenta()
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD("Insert into [Venta Inventario] (IdProducto, IdPersona, IdEmpleado, " +
                "CantidadVendida, PrecioUnitario, FechaVenta) values (@IdProducto, @IdPersona, @IdEmpleado, " +
                "@CantidadVendida, @PrecioUnitario, GETDATE())", 
                new SqlParameter("@IdProducto", IdProducto),
                new SqlParameter("@IdPersona", IdPersona),
                new SqlParameter("@IdEmpleado", IdEmpl),
                new SqlParameter("@CantidadVendida", CantidadVendida),
                new SqlParameter("@PrecioUnitario", PrecioUnitario));
        }
        public void obtenerId()
        {
            csCRUD crud = new csCRUD();
            SqlDataReader reader = crud.EjecutarQuery("Select top 1 IdVenta from [Venta Inventario] Order by IdVenta desc");
            if (reader.Read())
                IdVenta = reader.GetInt32(0);
            reader.Close();
        }
    }
}
