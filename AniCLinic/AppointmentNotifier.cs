using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Configuration;

namespace AniCLinic
{
    public static class AppointmentNotifier
    {
        public static void EnqueueForAppointment(int idCita, DateTime fechaHora)
        {
            TimeSpan fallbackHour = TimeSpan.Parse(ConfigurationManager.AppSettings["Notifier.ReminderHour"] ?? "08:00");

            DateTime n1 = DateTime.Now;                         
            DateTime n2 = fechaHora.AddDays(-7);                
            DateTime n3 = fechaHora.AddDays(-1);                

            if (n2 < DateTime.Now) n2 = DateTime.Now;
            if (n3 < DateTime.Now) n3 = DateTime.Now;

            n2 = new DateTime(n2.Year, n2.Month, n2.Day, fallbackHour.Hours, fallbackHour.Minutes, 0);
            n3 = new DateTime(n3.Year, n3.Month, n3.Day, fallbackHour.Hours, fallbackHour.Minutes, 0);

            using (var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionBD"].ConnectionString))
            {
                cn.Open();
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.NotificacionCita(IdCita, Tipo, ProgramadaPara) VALUES
(@id,'CREACION', @n1),
(@id,'SEMANA',   @n2),
(@id,'DIA',      @n3);", cn))
                {
                    cmd.Parameters.AddWithValue("@id", idCita);
                    cmd.Parameters.AddWithValue("@n1", n1);
                    cmd.Parameters.AddWithValue("@n2", n2);
                    cmd.Parameters.AddWithValue("@n3", n3);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }


    public sealed class ReminderWorker : IDisposable
    {
        private Timer _timer;
        private readonly TimeSpan _period;

        public ReminderWorker()
        {
            int mins = int.Parse(ConfigurationManager.AppSettings["Notifier.IntervalMinutes"] ?? "5");
            _period = TimeSpan.FromMinutes(Math.Max(mins, 1));
            _timer = new Timer(Tick, null, TimeSpan.FromSeconds(5), _period); 
        }

        private void Tick(object state)
        {
            try
            {
                ProcessPending();
            }
            catch { }
        }

        public static void ProcessPending()
        {
            var cs = ConfigurationManager.ConnectionStrings["ConexionBD"].ConnectionString;

            using (var cn = new SqlConnection(cs))
            {
                cn.Open();

                using (var da = new SqlDataAdapter(@"
SELECT TOP (50)
    n.IdNotificacion, n.IdCita, n.Tipo, g.FechaHora, g.Motivo,
    (p.Nombre + ' ' + p.Apellido) AS Propietario,
    p.Correo AS Correo,
    m.Nombre AS Mascota
FROM dbo.NotificacionCita n
JOIN dbo.GestionCita g ON g.IdCita = n.IdCita
JOIN dbo.Mascota m     ON m.IdMascota = g.IdMascota
JOIN dbo.Persona p     ON p.IdPersona = m.IdPersona
WHERE n.Enviada = 0 AND n.ProgramadaPara <= GETDATE()
ORDER BY n.ProgramadaPara ASC, n.IdNotificacion ASC;", cn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        int idNotif = Convert.ToInt32(r["IdNotificacion"]);
                        int idCita = Convert.ToInt32(r["IdCita"]);
                        string tipo = Convert.ToString(r["Tipo"]);
                        string correo = Convert.ToString(r["Correo"]);
                        if (string.IsNullOrWhiteSpace(correo))
                        {
                            MarcarError(cn, idNotif, "Cliente sin correo");
                            continue;
                        }

                        string propietario = Convert.ToString(r["Propietario"]);
                        string mascota = Convert.ToString(r["Mascota"]);
                        DateTime fh = Convert.ToDateTime(r["FechaHora"]);
                        string motivo = Convert.ToString(r["Motivo"]);

                        string asunto;
                        switch (tipo)
                        {
                            case "CREACION":
                                asunto = "AniClinic: Cita creada";
                                break;
                            case "SEMANA":
                                asunto = "AniClinic: Recordatorio (1 semana antes)";
                                break;
                            case "DIA":
                                asunto = "AniClinic: Recordatorio (mañana)";
                                break;
                            default:
                                asunto = "AniClinic: Recordatorio de cita";
                                break;
                        }


                        try
                        {
                            var html = EmailService.RenderBody(propietario, mascota, fh, motivo);
                            EmailService.Send(correo, asunto, html);

                            using (var cmd = new SqlCommand(@"
UPDATE dbo.NotificacionCita
   SET Enviada = 1, FechaEnvio = GETDATE(), UltimoError=NULL
 WHERE IdNotificacion=@id;", cn))
                            {
                                cmd.Parameters.AddWithValue("@id", idNotif);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            MarcarError(cn, idNotif, ex.Message);
                        }
                    }
                }
            }
        }

        private static void MarcarError(SqlConnection cn, int idNotif, string err)
        {
            using (var cmd = new SqlCommand(@"
UPDATE dbo.NotificacionCita
   SET Intentos = Intentos + 1,
       UltimoError = @e
 WHERE IdNotificacion=@id;", cn))
            {
                cmd.Parameters.AddWithValue("@id", idNotif);
                cmd.Parameters.AddWithValue("@e", (object)err ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

