using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;

namespace AniCLinic
{
    public partial class FormularioCarnet : Form
    {
        csCRUD crud = new csCRUD();
        bool botonesAgregados = false;

        public FormularioCarnet()
        {
            InitializeComponent();
            PrepararGrid();
            CargarDatosC();
        }

        private void CargarDatosC(string filtro = "")
        {
            string sentencia =
                "Select M.IdMascota, M.Nombre, E.Especie, R.Raza, M.Sexo, M.FechaNacimiento as [Fecha de Nacimiento], " +
                "M.Discapacidad " + // <- QUITADA la columna Edad (Años)
                "from Mascota M " +
                "INNER JOIN Especie   E ON E.IdEspecie = M.IdEspecie " +
                "INNER JOIN Raza      R ON R.IdRaza = M.IdRaza " +
                "where Nombre like @filtro + '%'";

            dgvCarnet.DataSource = crud.cargarBDData(sentencia, new SqlParameter("@filtro", filtro));
            ConfigurarColumnas();
        }

        private void PrepararGrid()
        {
            dgvCarnet.ReadOnly = true;
            dgvCarnet.MultiSelect = false;
            dgvCarnet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarnet.RowHeadersVisible = false;
            dgvCarnet.AllowUserToAddRows = false;
            dgvCarnet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarnet.AutoGenerateColumns = true;
        }

        private void ConfigurarColumnas()
        {
            var g = dgvCarnet;

            if (g.Columns.Contains("IdMascota"))
            {
                g.Columns["IdMascota"].Width = 60;
                g.Columns["IdMascota"].DisplayIndex = 0;
            }

            if (!botonesAgregados)
            {
                var colCarnet = new DataGridViewButtonColumn
                {
                    Name = "VerCarnet",
                    HeaderText = "",
                    Text = "Ver Carnet",
                    UseColumnTextForButtonValue = true,
                    Width = 80
                };
                g.Columns.Add(colCarnet);

                botonesAgregados = true;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarDatosC(txtBuscar.Text);
        }

        private void dgvCarnet_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var nomCol = dgvCarnet.Columns[e.ColumnIndex].Name;
            if (nomCol != "VerCarnet")
                return;

            var rowView = dgvCarnet.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null)
                return;

            int idMascota = Convert.ToInt32(rowView["IdMascota"]);

            SqlDataReader reader = crud.EjecutarQuery(
                "Select * from Mascota M " +
                "INNER JOIN Especie   E ON E.IdEspecie = M.IdEspecie  " +
                "INNER JOIN Raza      R ON R.IdRaza = M.IdRaza " +
                "Where IdMascota = " + idMascota
            );

            VerCarnet carnet = new VerCarnet(reader);
            carnet.ShowDialog();
        }
    }
}
