using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class fRegistroClinico : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        private bool _hoyBotonesAgregados = false;
        private bool _antBotonesAgregados = false;

        public fRegistroClinico()
        {
            InitializeComponent();

            this.Load += fRegistroClinico_Load;

            dvgHoy.CellContentClick += dvgHoy_CellContentClick;
            dvgProxima.CellContentClick += dvgProxima_CellContentClick;
            dvgAnteriores.CellContentClick += dvgAnteriores_CellContentClick;

            PrepGrid(dvgHoy);
            PrepGrid(dvgProxima);
            PrepGrid(dvgAnteriores);
        }

        private void fRegistroClinico_Load(object sender, EventArgs e) => CargarTodo();

        // ================= CARGA =================
        private void CargarTodo()
        {
            var cn = _db.AbrirConexion();
            try
            {
                var hoy = DateTime.Today;

                // ===== HOY (solo citas de HOY SIN registro aún) =====
                var dtHoy = new DataTable();
                using (var da = new SqlDataAdapter(@"
SELECT 
    c.IdCita,
    m.IdMascota,
    m.Nombre                         AS Mascota,
    (pe.Nombre + N' ' + pe.Apellido) AS Propietario,
    CAST(c.FechaHora AS date)        AS Fecha,
    CONVERT(varchar(5), c.FechaHora, 108) AS Hora
FROM GestionCita c
JOIN Mascota      m  ON m.IdMascota      = c.IdMascota
JOIN Propietario  pr ON pr.IdPropietario = c.IdPropietario
JOIN Persona      pe ON pe.IdPersona     = pr.IdPersona
WHERE CAST(c.FechaHora AS date) = @hoy
  AND c.Estado IN (N'Programada', N'Reprogramada')
  AND NOT EXISTS (
      SELECT 1
      FROM RegistroClinico rc
      WHERE rc.IdMascota = c.IdMascota
        AND CAST(rc.FechaRegistro AS date) = CAST(c.FechaHora AS date)
  )
ORDER BY c.FechaHora;", cn))
                {
                    da.SelectCommand.Parameters.Add("@hoy", SqlDbType.Date).Value = hoy;
                    da.Fill(dtHoy);
                }

                dvgHoy.Columns.Clear();
                dvgHoy.DataSource = dtHoy;

                Renombrar(dvgHoy, "IdCita", "ID Cita");
                Renombrar(dvgHoy, "IdMascota", "ID Mascota");
                Renombrar(dvgHoy, "Mascota", "Mascota");
                Renombrar(dvgHoy, "Propietario", "Propietario");
                Renombrar(dvgHoy, "Fecha", "Fecha");
                Renombrar(dvgHoy, "Hora", "Hora");

                // SOLO botón Registrar (sin Editar)
                if (!_hoyBotonesAgregados)
                {
                    var btnRegistrar = new DataGridViewButtonColumn
                    {
                        Name = "Registrar",
                        HeaderText = "",
                        Text = "Registrar",
                        UseColumnTextForButtonValue = true,
                        Width = 90
                    };
                    dvgHoy.Columns.Add(btnRegistrar);
                    _hoyBotonesAgregados = true;
                }
                if (dvgHoy.Columns.Contains("Registrar"))
                    dvgHoy.Columns["Registrar"].DisplayIndex = dvgHoy.Columns.Count - 1;

                Ajustar(dvgHoy);

                // ===== PRÓXIMAS (igual que antes) =====
                var dtProx = new DataTable();
                using (var da = new SqlDataAdapter(@"
SELECT 
    c.IdCita,
    m.IdMascota,
    m.Nombre                         AS Mascota,
    (pe.Nombre + N' ' + pe.Apellido) AS Propietario,
    CAST(c.FechaHora AS date)        AS Fecha,
    CONVERT(varchar(5), c.FechaHora, 108) AS Hora
FROM GestionCita c
JOIN Mascota      m  ON m.IdMascota      = c.IdMascota
JOIN Propietario  pr ON pr.IdPropietario = c.IdPropietario
JOIN Persona      pe ON pe.IdPersona     = pr.IdPersona
WHERE CAST(c.FechaHora AS date) > @hoy
  AND c.Estado IN (N'Programada', N'Reprogramada')
ORDER BY c.FechaHora;", cn))
                {
                    da.SelectCommand.Parameters.Add("@hoy", SqlDbType.Date).Value = hoy;
                    da.Fill(dtProx);
                }

                dvgProxima.Columns.Clear();
                dvgProxima.DataSource = dtProx;

                Renombrar(dvgProxima, "IdCita", "ID Cita");
                Renombrar(dvgProxima, "IdMascota", "ID Mascota");
                Renombrar(dvgProxima, "Mascota", "Mascota");
                Renombrar(dvgProxima, "Propietario", "Propietario");
                Renombrar(dvgProxima, "Fecha", "Fecha");
                Renombrar(dvgProxima, "Hora", "Hora");

                Ajustar(dvgProxima);

                // ===== ANTERIORES (citas <= hoy CON registro clínico) =====
                // ===== ANTERIORES (citas <= hoy CON registro clínico) =====
                var dtAnt = new DataTable();
                using (var da = new SqlDataAdapter(@"
SELECT 
    c.IdCita,
    m.IdMascota,
    rc.IdRegistroClinico,                            -- << clave para editar
    m.Nombre                         AS Mascota,
    (pe.Nombre + N' ' + pe.Apellido) AS Propietario,
    CAST(c.FechaHora AS date)        AS Fecha,
    CONVERT(varchar(5), c.FechaHora, 108) AS Hora,
    rc.MotivoConsulta                AS Motivo,
    rc.Diagnostico                   AS Diagnóstico,
    rc.Tratamiento                   AS Tratamiento,
    rc.AplicacionTratamiento         AS Receta,
    (ISNULL(pv.Nombre, N'') + CASE WHEN pv.Nombre IS NULL THEN N'' ELSE N' ' END + ISNULL(pv.Apellido, N'')) AS Veterinario
FROM GestionCita c
JOIN Mascota      m  ON m.IdMascota      = c.IdMascota
JOIN Propietario  pr ON pr.IdPropietario = c.IdPropietario
JOIN Persona      pe ON pe.IdPersona     = pr.IdPersona
JOIN RegistroClinico rc 
       ON rc.IdMascota = m.IdMascota 
      AND CAST(rc.FechaRegistro AS date) = CAST(c.FechaHora AS date)
LEFT JOIN Empleado ev ON ev.IdEmpleado = rc.IdVeterinario
LEFT JOIN Persona  pv ON pv.IdPersona  = ev.IdPersona
WHERE CAST(c.FechaHora AS date) <= @hoy
ORDER BY c.FechaHora DESC;", cn))
                {
                    da.SelectCommand.Parameters.Add("@hoy", SqlDbType.Date).Value = hoy;
                    da.Fill(dtAnt);
                }

                dvgAnteriores.Columns.Clear();
                dvgAnteriores.DataSource = dtAnt;

                Renombrar(dvgAnteriores, "IdCita", "ID Cita");
                Renombrar(dvgAnteriores, "IdMascota", "ID Mascota");
                Renombrar(dvgAnteriores, "Mascota", "Mascota");
                Renombrar(dvgAnteriores, "Propietario", "Propietario");
                Renombrar(dvgAnteriores, "Fecha", "Fecha");
                Renombrar(dvgAnteriores, "Hora", "Hora");
                Renombrar(dvgAnteriores, "Motivo", "Motivo");
                Renombrar(dvgAnteriores, "Diagnóstico", "Diagnóstico");
                Renombrar(dvgAnteriores, "Tratamiento", "Tratamiento");
                Renombrar(dvgAnteriores, "Receta", "Receta");
                Renombrar(dvgAnteriores, "Veterinario", "Veterinario");

                if (dvgAnteriores.Columns.Contains("IdRegistroClinico"))
                    dvgAnteriores.Columns["IdRegistroClinico"].Visible = false;   // oculto

                if (!_antBotonesAgregados)
                {
                    var btnEditarAnt = new DataGridViewButtonColumn
                    {
                        Name = "Editar",
                        HeaderText = "",
                        Text = "Editar",
                        UseColumnTextForButtonValue = true,
                        Width = 80
                    };
                    dvgAnteriores.Columns.Add(btnEditarAnt);
                    _antBotonesAgregados = true;
                }
                if (dvgAnteriores.Columns.Contains("Editar"))
                    dvgAnteriores.Columns["Editar"].DisplayIndex = dvgAnteriores.Columns.Count - 1;

                Ajustar(dvgAnteriores);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }


        // ================= ACCIONES =================
        private void dvgHoy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dvgHoy.Columns[e.ColumnIndex].Name;
            if (col != "Registrar") return; // ya no hay Editar en HOY

            var row = dvgHoy.Rows[e.RowIndex];
            int idCita = Convert.ToInt32(row.Cells["IdCita"].Value);

            using (var frm = new AggRegistroClinico(idCita, SesionActual.IdEmpleado, SesionActual.NombreEmpleado))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    CargarTodo(); // refresca: sale de HOY y aparece en ANTERIORES
            }
        }



        private void dvgProxima_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // sin acciones/botones
        }

        private void dvgAnteriores_CellContentClick(object sender, DataGridViewCellEventArgs e)
{
    if (e.RowIndex < 0) return;

    var col = dvgAnteriores.Columns[e.ColumnIndex];
    if (col == null || col.Name != "Editar" || !(col is DataGridViewButtonColumn))
        return;

    var row = dvgAnteriores.Rows[e.RowIndex];

    // 1) Tomar IdRegistroClinico directamente de la grilla
    int? idRegistro = null;
    if (dvgAnteriores.Columns.Contains("IdRegistroClinico"))
    {
        var v = row.Cells["IdRegistroClinico"].Value;
        if (v != null && v != DBNull.Value)
            idRegistro = Convert.ToInt32(v);
    }

    // 2) Si por alguna razón no está, buscar por (cita, mascota)
    if (!idRegistro.HasValue)
    {
        int idCita = Convert.ToInt32(row.Cells["IdCita"].Value);
        int idMascota = Convert.ToInt32(row.Cells["IdMascota"].Value);
        idRegistro = BuscarRegistroClinicoDeCita(idCita, idMascota);
    }

    if (!idRegistro.HasValue)
    {
        MessageBox.Show("No se encontró el Registro Clínico para editar.");
        return;
    }

    // 3) Abrir formulario de edición (carga y permite actualizar)
    using (var frm = new AggRegistroClinico(idRegistro.Value))
    {
        if (frm.ShowDialog() == DialogResult.OK)
            CargarTodo();   // refrescar grillas
    }
}


        // ================= Helpers de UI =================
        private static void PrepGrid(DataGridView g)
        {
            g.AutoGenerateColumns = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;

            g.BackgroundColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 255);

            try
            {
                g.GetType()
                 .GetProperty("DoubleBuffered",
                     System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                 ?.SetValue(g, true, null);
            }
            catch { }
        }

        private static void Ajustar(DataGridView g)
        {
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (DataGridViewColumn c in g.Columns)
                c.SortMode = DataGridViewColumnSortMode.Automatic;
        }

        private static void Renombrar(DataGridView g, string name, string header)
        {
            if (g.Columns.Contains(name))
                g.Columns[name].HeaderText = header;
        }

        private static void OcultarCol(DataGridView g, string name)
        {
            if (g.Columns.Contains(name))
                g.Columns[name].Visible = false;
        }

        // Botón “Agregar Registro” general (si lo usas)
        private void btnNuvCita_Click(object sender, EventArgs e)
        {
            using (var frm = new AggRegistroClinico(SesionActual.IdEmpleado, SesionActual.NombreEmpleado))
            {
                frm.ShowDialog();
            }
            CargarTodo();
        }


        // Buscar si hay registro clínico para esa cita/mascota en la misma fecha
        private int? BuscarRegistroClinicoDeCita(int idCita, int idMascota)
        {
            var cn = _db.AbrirConexion();
            try
            {
                using (var cmd = new SqlCommand(@"
SELECT TOP 1 rc.IdRegistroClinico
FROM GestionCita c
JOIN RegistroClinico rc 
     ON rc.IdMascota = c.IdMascota
    AND CAST(rc.FechaRegistro AS date) = CAST(c.FechaHora AS date)
WHERE c.IdCita = @cita AND c.IdMascota = @mascota;", cn))
                {
                    cmd.Parameters.AddWithValue("@cita", idCita);
                    cmd.Parameters.AddWithValue("@mascota", idMascota);
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        return Convert.ToInt32(o);
                    return null;
                }
            }
            catch { return null; }
            finally { _db.CerrarConexion(); }
        }
    }
}
