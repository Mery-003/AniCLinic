using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fRegistroClinico : Form
    {
        public fRegistroClinico()
        {
            InitializeComponent();

            try { dgvHoy.CellContentClick -= DgvHoy_CellContentClick; } catch { }
            try { dgvProximas.CellContentClick -= DgvProximas_CellContentClick; } catch { }
            try { dgvAnteriores.CellContentClick -= DgvAnteriores_CellContentClick; } catch { }

            ConfigurarGridsBase();
            PrepararGrid_Hoy(dgvHoy);
            PrepararGrid_Proximas(dgvProximas);
            PrepararGrid_Anteriores(dgvAnteriores);

            dgvHoy.CellContentClick += Grid_CellContentClick;
            dgvProximas.CellContentClick += Grid_CellContentClick;
            dgvAnteriores.CellContentClick += Grid_CellContentClick;

            RecargarTodo();
        }

        private void ConfigurarGridsBase()
        {
            foreach (var g in new DataGridView[] { dgvHoy, dgvProximas, dgvAnteriores })
            {
                if (g == null) continue;
                g.AutoGenerateColumns = false;
                g.AllowUserToAddRows = false;
                g.MultiSelect = false;
                g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                g.ReadOnly = true;
                g.Columns.Clear();

                g.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                g.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            }
        }

        private DataGridViewTextBoxColumn MkText(string header, string prop, int width)
            => new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = true
            };

        private void PrepararGrid_Hoy(DataGridView grid)
        {
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            grid.Columns.Add(MkText("Estado", "Estado", 100));

        }

        private void PrepararGrid_Proximas(DataGridView grid)
        {
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            grid.Columns.Add(MkText("Estado", "Estado", 100));
        }

        private void PrepararGrid_Anteriores(DataGridView grid)
        {
            grid.Columns.Add(MkText("Id", "IdCita", 60));
            grid.Columns.Add(MkText("Mascota", "Mascota", 120));
            grid.Columns.Add(MkText("Especie", "Especie", 100));
            grid.Columns.Add(MkText("Raza", "Raza", 120));
            grid.Columns.Add(MkText("Fecha", "Fecha", 90));
            grid.Columns.Add(MkText("Hora", "Hora", 70));
            grid.Columns.Add(MkText("Motivo", "Motivo", 220));
            grid.Columns.Add(MkText("Propietario", "Propietario", 160));
            grid.Columns.Add(MkText("Veterinario", "Veterinario", 140));
            grid.Columns.Add(MkText("Estado", "Estado", 120));

        }

        private void RecargarTodo()
        {
            RecargarHoy();
            RecargarProximas();
            RecargarAnteriores();
        }

        private void RecargarHoy()
        {
            var dt = CedulaUtils.CitasListado(null);
            AsegurarColumnaEstado(dt);
            foreach (DataRow r in dt.Rows) r["Estado"] = "Pendiente";
            dgvHoy.DataSource = FiltrarPorFecha(dt, TipoSeccion.Hoy);
        }

        private void RecargarProximas()
        {
            var dt = CedulaUtils.CitasListado(null);
            AsegurarColumnaEstado(dt);
            foreach (DataRow r in dt.Rows) r["Estado"] = "Próximo";
            dgvProximas.DataSource = FiltrarPorFecha(dt, TipoSeccion.Proximas);
        }

        private void RecargarAnteriores()
        {
            var dt = CedulaUtils.CitasListado(null);
            AsegurarColumnaEstado(dt);
            foreach (DataRow r in dt.Rows)
            {
                var registrada = (r.Table.Columns.Contains("Registrada"))
                                 ? Convert.ToInt32(r["Registrada"]) == 1
                                 : false;
                r["Estado"] = registrada ? "Registrado" : "No registrado";
            }
            dgvAnteriores.DataSource = FiltrarPorFecha(dt, TipoSeccion.Anteriores);
        }

        private void AsegurarColumnaEstado(DataTable dt)
        {
            if (dt != null && !dt.Columns.Contains("Estado"))
                dt.Columns.Add("Estado", typeof(string));
        }

        private enum TipoSeccion { Hoy, Proximas, Anteriores }

        private DataTable FiltrarPorFecha(DataTable dt, TipoSeccion seccion)
        {
            if (dt == null) return null;

            var dv = new DataView(dt);
            var hoy = DateTime.Today;
            string d0 = hoy.ToString("yyyy-MM-dd");
            string d1 = hoy.AddDays(1).ToString("yyyy-MM-dd");

            switch (seccion)
            {
                case TipoSeccion.Hoy:
                    dv.RowFilter = $"CONVERT(Fecha, 'System.DateTime') >= #{d0}# AND CONVERT(Fecha, 'System.DateTime') < #{d1}#";
                    break;
                case TipoSeccion.Proximas:
                    dv.RowFilter = $"CONVERT(Fecha, 'System.DateTime') >= #{d1}#";
                    break;
                case TipoSeccion.Anteriores:
                    dv.RowFilter = $"CONVERT(Fecha, 'System.DateTime') < #{d0}#";
                    break;
            }
            return dv.ToTable();
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var grid = (DataGridView)sender;
            var col = grid.Columns[e.ColumnIndex].Name;
            if (col != "colRegistrar" && col != "colEditar") return;

            int idCita = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["IdCita"].Value);

            var info = ObtenerCitaInfo(idCita); 
            if (info == null) return;

            bool isEdit = (col == "colEditar");
            using (var frm = new AggRegistroClinico(info, isEdit))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    RecargarSegunGrid(grid);
            }
        }

        private void RecargarSegunGrid(DataGridView grid)
        {
            if (grid == dgvHoy) RecargarHoy();
            else if (grid == dgvProximas) RecargarProximas();
            else if (grid == dgvAnteriores) RecargarAnteriores();
        }

        private CitaInfo ObtenerCitaInfo(int idCita) => GetCitaInfo_SinPropietario(idCita);

        private CitaInfo GetCitaInfo_SinPropietario(int idCita)
        {
            const string SQL_A = @"
SELECT c.IdCita, c.Fecha, c.Hora,
       m.IdMascota, m.Nombre AS Mascota, m.Especie, m.Raza,
       (p.Nombres + ' ' + p.Apellidos)                  AS Propietario,
       COALESCE(p.Cedula, p.DNI, p.CI, p.Documento, '') AS CedulaPropietario,
       (v.Nombres + ' ' + v.Apellidos)                  AS Veterinario,
       c.Motivo
FROM dbo.GestionCita   AS c
JOIN dbo.Mascota       AS m ON m.IdMascota     = c.IdMascota
JOIN dbo.Personas      AS p ON p.IdPersona     = c.IdPropietario
JOIN dbo.Veterinario   AS v ON v.IdVeterinario = c.IdVeterinario
WHERE c.IdCita = @id;";

            const string SQL_B = @"
SELECT c.IdCita, c.Fecha, c.Hora,
       m.IdMascota, m.Nombre AS Mascota, m.Especie, m.Raza,
       (p.Nombres + ' ' + p.Apellidos)                  AS Propietario,
       COALESCE(p.Cedula, p.DNI, p.CI, p.Documento, '') AS CedulaPropietario,
       (v.Nombres + ' ' + v.Apellidos)                  AS Veterinario,
       c.Motivo
FROM dbo.GestionCita   AS c
JOIN dbo.Mascota       AS m ON m.IdMascota     = c.IdMascota
JOIN dbo.Personas      AS p ON p.IdPersona     = c.IdPersona
JOIN dbo.Veterinario   AS v ON v.IdVeterinario = c.IdVeterinario
WHERE c.IdCita = @id;";

            if (TryLeerCita(SQL_A, idCita, out var info)) return info;
            if (TryLeerCita(SQL_B, idCita, out info)) return info;

            MessageBox.Show(
                "No se pudo leer la cita con Personas.\n" +
                "Verifica si la FK en GestionCita es IdPropietario o IdPersona y que Personas tenga Nombres/Apellidos.",
                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        private bool TryLeerCita(string sql, int idCita, out CitaInfo info)
        {
            info = null;
            var db = new csConexionBD();
            try
            {
                db.abrirConexion();
                using (var cmd = new SqlCommand(sql, db.obtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return false;

                        DateTime fecha = (rd["Fecha"] is DateTime df)
                                         ? df.Date
                                         : DateTime.Parse(rd["Fecha"].ToString()).Date;

                        TimeSpan hora = TimeSpan.Zero;
                        var hs = rd["Hora"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(hs))
                        {
                            if (!TimeSpan.TryParse(hs, out hora) && DateTime.TryParse(hs, out var ht))
                                hora = ht.TimeOfDay;
                        }

                        info = new CitaInfo
                        {
                            IdCita = Convert.ToInt32(rd["IdCita"]),
                            IdMascota = Convert.ToInt32(rd["IdMascota"]),
                            Mascota = rd["Mascota"].ToString(),
                            Especie = rd["Especie"].ToString(),
                            Raza = rd["Raza"].ToString(),
                            Propietario = rd["Propietario"].ToString(),
                            CedulaPropietario = rd["CedulaPropietario"].ToString(),
                            Veterinario = rd["Veterinario"].ToString(),
                            Motivo = rd["Motivo"].ToString(),
                            FechaHora = fecha.Add(hora)
                        };
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 208 || ex.Number == 207) return false;
                throw;
            }
            finally { db.cerrarConexion(); }
        }

        private void DgvHoy_CellContentClick(object s, DataGridViewCellEventArgs e) { }
        private void DgvProximas_CellContentClick(object s, DataGridViewCellEventArgs e) { }
        private void DgvAnteriores_CellContentClick(object s, DataGridViewCellEventArgs e) { }
    }

    public class CitaInfo
    {
        public int IdCita { get; set; }
        public int IdMascota { get; set; }
        public string Mascota { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public string Propietario { get; set; }
        public string CedulaPropietario { get; set; }
        public string Veterinario { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
