using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AggCita : Form
    {
        public AggCita()
        {
            InitializeComponent();
        }
        private readonly ConexionBD _db = new ConexionBD();

        private int? _idCitaEdit;
        private int? _idMascota;
        private int? _idPropietario; // dueño principal
        private int? _idEmpleado = 1;


        public AggCita(int idCita) : this() // modo edición
        {
            _idCitaEdit = idCita;
        }


        //public NuevaCita(int idEmpleado)
        //{
        //    InitializeComponent();
        //    _idEmpleado = idEmpleado;

        //    // formato bonito de fecha (opcional)
        //    dtpFecha.Format = DateTimePickerFormat.Custom;
        //    dtpFecha.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy";

        //    // hora con máscara HH:mm (agrega un MaskedTextBox llamado mtbHora en el diseñador)
        //    // mtbHora.Mask = "00:00";   <-- hazlo en diseñador
        //}


        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            // Lee el ID desde txtIdMascota si existe; de lo contrario, desde txtNombreMascota
            string idTxt = this.Controls.Find("txtIdMascota", true).FirstOrDefault() is TextBox t ? t.Text : txtNombreMascota.Text;

            if (!int.TryParse(idTxt.Trim(), out int id))
            {
                MessageBox.Show("Ingresa un ID de mascota válido (solo números).");
                return;
            }

            var cn = _db.AbrirConexion();
            try
            {
                string sql = @"
                SELECT TOP 1 
                       m.IdMascota, m.Nombre, m.Especie, m.Raza,
                       pm.IdPropietario
                FROM Mascota m
                LEFT JOIN PropietarioMascota pm
                       ON pm.IdMascota = m.IdMascota
                WHERE m.IdMascota = @id;";

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            _idMascota = Convert.ToInt32(rd["IdMascota"]);
                            _idPropietario = rd["IdPropietario"] == DBNull.Value ? (int?)null : Convert.ToInt32(rd["IdPropietario"]);

                            txtNombreMascota.Text = rd["Nombre"]?.ToString();
                            txtEspecie.Text = rd["Especie"]?.ToString();
                            txtRaza.Text = rd["Raza"] as string;
                        }
                        else
                        {
                            _idMascota = _idPropietario = null;
                            txtNombreMascota.Clear(); txtEspecie.Clear(); txtRaza.Clear();
                            MessageBox.Show("No se encontró una mascota con ese ID.");
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al buscar mascota: " + ex.Message); }
            finally { _db.CerrarConexion(); }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBuscarVet_Click(object sender, EventArgs e)
        {

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!_idMascota.HasValue) { MessageBox.Show("Primero selecciona la mascota."); return; }
            if (!_idPropietario.HasValue) { MessageBox.Show("La mascota no tiene dueño principal asociado."); return; }
            if (!_idEmpleado.HasValue) { MessageBox.Show("Selecciona el veterinario."); return; }

            if (!TimeSpan.TryParseExact(mtbHora.Text, "HH\\:mm", CultureInfo.InvariantCulture, out var hora) &&
                !TimeSpan.TryParse(mtbHora.Text, out hora))
            {
                MessageBox.Show("Hora inválida. Usa formato 24h HH:mm (ej. 09:30).");
                return;
            }
            var fechaHora = dtpFecha.Value.Date + hora;

            var cn = _db.AbrirConexion();
            try
            {
                if (_idCitaEdit.HasValue)
                {
                    // UPDATE
                    using (var cmd = new SqlCommand(@"
                        UPDATE GestionCita
                           SET IdPropietario = @p,
                               IdMascota     = @m,
                               IdEmpleado    = 1,
                               FechaHora     = @fh,
                               Motivo        = @motivo
                         WHERE IdCita       = @id;", cn))
                    {
                        cmd.Parameters.AddWithValue("@p", _idPropietario.Value);
                        cmd.Parameters.AddWithValue("@m", _idMascota.Value);
                        cmd.Parameters.AddWithValue("@e", 1);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@motivo",
                            string.IsNullOrWhiteSpace(txtMotivo.Text) ? (object)DBNull.Value : txtMotivo.Text.Trim());
                        cmd.Parameters.AddWithValue("@id", _idCitaEdit.Value);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Cita actualizada ✅");
                }
                else
                {
                    // INSERT
                    using (var cmd = new SqlCommand(@"
                    INSERT INTO GestionCita
                        (IdPropietario, IdMascota, IdEmpleado, FechaHora, Motivo, Estado)
                    VALUES
                        (@p, @m, @e, @fh, @motivo, N'Programada');", cn))
                    {
                        cmd.Parameters.AddWithValue("@p", _idPropietario.Value);
                        cmd.Parameters.AddWithValue("@m", _idMascota.Value);
                        cmd.Parameters.AddWithValue("@e", 1);
                        cmd.Parameters.AddWithValue("@fh", fechaHora);
                        cmd.Parameters.AddWithValue("@motivo",
                            string.IsNullOrWhiteSpace(txtMotivo.Text) ? (object)DBNull.Value : txtMotivo.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Cita guardada correctamente ✅");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la cita: " + ex.Message);
            }
            finally
            {
                _db.CerrarConexion();
            }
        }

        private void AggCita_Load(object sender, EventArgs e)
        {
            dtpFecha.Format = DateTimePickerFormat.Custom;
            dtpFecha.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy";

            if (_idCitaEdit.HasValue)
                CargarCitaParaEditar(_idCitaEdit.Value);
        }

        private void CargarCitaParaEditar(int idCita)
        {
            var cn = _db.AbrirConexion();
            try
            {
                string sql = @"
            SELECT gc.IdPropietario, gc.IdMascota, gc.IdEmpleado, gc.FechaHora, gc.Motivo,
                   m.Nombre AS MascotaNombre, m.Especie, m.Raza,
                   CONCAT(pv.Nombre,' ',pv.Apellido) AS Veterinario
            FROM GestionCita gc
            JOIN Mascota  m  ON m.IdMascota  = gc.IdMascota
            JOIN Empleado e  ON e.IdEmpleado = gc.IdEmpleado
            JOIN Persona  pv ON pv.IdPersona = e.IdPersona
            WHERE gc.IdCita = @id;";

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            _idPropietario = Convert.ToInt32(rd["IdPropietario"]);
                            _idMascota = Convert.ToInt32(rd["IdMascota"]);
                            _idEmpleado = Convert.ToInt32(rd["IdEmpleado"]);

                            // Si tienes txtIdMascota, pinta ahí también
                            var ctl = this.Controls.Find("txtIdMascota", true).FirstOrDefault() as TextBox;
                            if (ctl != null) ctl.Text = _idMascota.ToString();

                            txtNombreMascota.Text = rd["MascotaNombre"]?.ToString();
                            txtEspecie.Text = rd["Especie"]?.ToString();
                            txtRaza.Text = rd["Raza"] as string;

                            var fh = Convert.ToDateTime(rd["FechaHora"]);
                            dtpFecha.Value = fh.Date;
                            mtbHora.Text = fh.ToString("HH:mm");

                            txtMotivo.Text = rd["Motivo"] as string;
                            txtVeterinario.Text = rd["Veterinario"]?.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró la cita.");
                            Close();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar la cita: " + ex.Message); }
            finally { _db.CerrarConexion(); }
        }
    }
}