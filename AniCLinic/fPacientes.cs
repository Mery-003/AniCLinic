using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fPacientes : Form
    {
        csCRUD crud = new csCRUD();
        bool btnE = false;

        public fPacientes()
        {
            InitializeComponent();
            cargarData();
        }

        public void cargarData()
        {
            dgvPacientes.DataSource = crud.cargarBDData("SELECT IdMascota, Nombre, Especie, Raza, Sexo, Edad, PesoKg, Discapacidad from Mascota");
            ConfigurarGrid();
        }

        private void btnAggPaciente_Click(object sender, EventArgs e)
        {
            AgregarPaciente agregar = new AgregarPaciente(this);
            agregar.ShowDialog();
        }

        private void ConfigurarGrid()
        {
            var grid = dgvPacientes;
            if (btnE)
                return;
            grid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Editar",
                HeaderText = "Editar",
                Text = "Editar",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            grid.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Eliminar",
                HeaderText = "Eliminar",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 90
            });
            btnE = true;
        }

        private void dgvPacientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) 
                return;

            string nCol = dgvPacientes.Columns[e.ColumnIndex].Name;
            DataRowView rowView = dgvPacientes.Rows[e.RowIndex].DataBoundItem as DataRowView;

            if (rowView == null) 
                return;

            int idm = Convert.ToInt32(rowView["IdMascota"]);

            if (nCol == "Eliminar")
            {
                if (MessageBox.Show("¿Eliminar la Mascota seleccionada?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (crud.eliminarBD("DELETE FROM Mascota WHERE IdMascota = @id", idm))
                    {
                        MessageBox.Show("Mascota eliminada correctamente");
                        cargarData();
                    }
                }
            }
            else if (nCol == "Editar")
            {
                AgregarPaciente aP = new AgregarPaciente(this, idm);
                aP.ShowDialog();
            }
        }
    }
}
