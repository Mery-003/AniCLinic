using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    internal class csFactura
    {
        int idFactura { get; set; }
        int numeroFactura { get; set; }
        int idPersona { get; set; }
        int idEmpleado { get; set; }
        decimal subtotal { get; set; }
        decimal iva { get; set; }
        decimal total { get; set; }
        string metodoPago { get; set; }

        public int IdFactura
        {
            get { return idFactura; }
            set { idFactura = value; }
        }
        public int NumeroFactura
        {
            get { return numeroFactura; }
            set { numeroFactura = value; }
        }
        public int IdPersona
        {
            get { return idPersona; }
            set { idPersona = value; }
        }
        public int IdEmpleado
        {
            get { return idEmpleado; }
            set { idEmpleado = value; }
        }
        public decimal Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }
        public decimal IVA
        {
            get { return  iva; }
            set {  iva = value; }
        }
        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }
        public string MetodoPago
        {
            get { return metodoPago; }
            set { metodoPago = value; }
        }
        public csFactura() { }
        public csFactura(int idPer, int idEmp, decimal subtot, decimal Iva, decimal tot, string metPago)
        {
            IdPersona = idPer;
            IdEmpleado = idEmp;
            Subtotal = subtot;
            IVA = Iva;
            Total = tot;
            MetodoPago = metPago;
        }
        public bool agregarFactura()
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD("INSERT INTO Factura (NumeroFactura, IdPersona, IdEmpleado, FechaFactura, Subtotal, IVA, Total, MetodoPago) " +
                "VALUES (@NumeroFactura, @IdPersona, @IdEmpleado, GETDATE(), @Subtotal, @IVA, @Total, @MetodoPago)",
                new SqlParameter("@NumeroFactura", NumeroFactura),
                new SqlParameter("@IdPersona", IdPersona),
                new SqlParameter("@IdEmpleado", IdEmpleado),
                new SqlParameter("@Subtotal", Subtotal),
                new SqlParameter("@IVA", IVA),
                new SqlParameter("@Total", Total),
                new SqlParameter("@MetodoPago", MetodoPago));

        }
        public void obtenerNumFactura()
        {
            csCRUD crud = new csCRUD();
            SqlDataReader reader = crud.EjecutarQuery("Select top 1 NumeroFactura from Factura Order by IdFactura desc");
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    NumeroFactura = Convert.ToInt32(reader["NumeroFactura"]) + 1;
                }
                reader.Close();
            }
            else
            {
                NumeroFactura = 1000;
            }
        }
        public void obtenerId()
        {
            csCRUD crud = new csCRUD();
            SqlDataReader reader = crud.EjecutarQuery("Select top 1 IdFactura from Factura Order by IdFactura desc");
            if (reader.Read())
            {
                IdFactura = Convert.ToInt32(reader["IdFactura"]);
            }
            reader.Close();
        }
    }
}
