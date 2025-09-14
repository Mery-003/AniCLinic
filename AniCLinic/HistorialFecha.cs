using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class HistorialFecha : Form
    {
        private readonly DataTable _fichas;
        public int SelectedIdRegistroClinico { get; private set; }

        // Si usas diseñador, asegúrate de tener:
        // ListView lvFichas; Label lblPaciente; Button btnAceptar, btnCancelar.

        // Ctor usado desde Historial.cs
        public HistorialFecha(DataTable fichas, string nombrePaciente = "")
        {
            _fichas = fichas ?? throw new ArgumentNullException(nameof(fichas));
            InitializeComponent();

            this.Text = "Fichas del paciente";
            this.StartPosition = FormStartPosition.CenterParent;
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;


            ConfigurarListView();
            CargarListView();

            btnAceptar.Click -= BtnAceptar_Click;
            btnCancelar.Click -= BtnCancelar_Click;
            btnAceptar.Click += BtnAceptar_Click;
            btnCancelar.Click += BtnCancelar_Click;

            lvFichas.DoubleClick -= LvFichas_DoubleClick;
            lvFichas.DoubleClick += LvFichas_DoubleClick;
        }

        private void ConfigurarListView()
        {
            lvFichas.View = View.Details;
            lvFichas.FullRowSelect = true;
            lvFichas.HideSelection = false;
            lvFichas.MultiSelect = false;
            lvFichas.Font = new Font("Segoe UI", 10.5f);

            if (lvFichas.Columns.Count == 0)
            {
                lvFichas.Columns.Add("Fecha y hora", 220, HorizontalAlignment.Left);
                lvFichas.Columns.Add("Motivo", 480, HorizontalAlignment.Left);
            }
        }

        private void CargarListView()
        {
            lvFichas.BeginUpdate();
            lvFichas.Items.Clear();

            foreach (DataRow r in _fichas.Rows)
            {
                int id = SafeToInt(r, "IdRegistroClinico");
                DateTime? fecha = SafeToDate(r, "FechaRegistro");
                string motivo = Convert.ToString(r["MotivoConsulta"] ?? "");

                var it = new ListViewItem(fecha.HasValue ? fecha.Value.ToString("dd/MM/yyyy HH:mm") : "-")
                {
                    Tag = id
                };
                it.SubItems.Add(motivo);
                lvFichas.Items.Add(it);
            }

            if (lvFichas.Items.Count > 0)
                lvFichas.Items[0].Selected = true;

            // autoajuste bonita
            lvFichas.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (lvFichas.Columns.Count > 1)
                lvFichas.Columns[1].Width = lvFichas.ClientSize.Width - lvFichas.Columns[0].Width - 10;

            lvFichas.EndUpdate();
        }

        private void BtnAceptar_Click(object sender, EventArgs e) => Aceptar();
        private void LvFichas_DoubleClick(object sender, EventArgs e) => Aceptar();
        private void BtnCancelar_Click(object sender, EventArgs e) => Close();

        private void Aceptar()
        {
            if (lvFichas.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleccione una ficha.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedIdRegistroClinico = (int)lvFichas.SelectedItems[0].Tag;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private static int SafeToInt(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col) || r[col] == DBNull.Value) return 0;
            int n; return int.TryParse(r[col].ToString(), out n) ? n : 0;
        }
        private static DateTime? SafeToDate(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col) || r[col] == DBNull.Value) return null;
            DateTime d; return DateTime.TryParse(r[col].ToString(), out d) ? d : (DateTime?)null;
        }
    }
}
