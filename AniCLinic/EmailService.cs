using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace AniCLinic
{
    public static class EmailService
    {
        private static readonly string Host = ConfigurationManager.AppSettings["Smtp.Host"];
        private static readonly int Port = int.Parse(ConfigurationManager.AppSettings["Smtp.Port"]);
        private static readonly string User = ConfigurationManager.AppSettings["Smtp.User"];
        private static readonly string Pass = ConfigurationManager.AppSettings["Smtp.Pass"];
        private static readonly string From = ConfigurationManager.AppSettings["Smtp.From"];

        public static void Send(string to, string subject, string htmlBody)
        {
            using (var c = new SmtpClient(Host, Port))
            {
                c.EnableSsl = true;
                c.Credentials = new NetworkCredential(User, Pass);
                var m = new MailMessage();
                m.From = new MailAddress(From.Split('<').Length > 1 ? From.Split('<')[1].Trim('>') : User, From.Split('<')[0].Trim());
                m.To.Add(to);
                m.Subject = subject;
                m.Body = htmlBody;
                m.IsBodyHtml = true;
                m.BodyEncoding = Encoding.UTF8;
                c.Send(m);
            }
        }

        public static string RenderBody(string propietario, string mascota, DateTime fechaHora, string motivo)
        {
            return $@"
<div style='font-family:Segoe UI,Arial,sans-serif'>
  <h2>Recordatorio de Cita – AniClinic</h2>
  <p>Hola <b>{propietario}</b>,</p>
  <p>Te recordamos la cita de <b>{mascota}</b>:</p>
  <ul>
    <li><b>Fecha:</b> {fechaHora:dddd, dd 'de' MMMM 'de' yyyy}</li>
    <li><b>Hora:</b> {fechaHora:HH\\:mm}</li>
    <li><b>Motivo:</b> {motivo}</li>
  </ul>
  <p>Si no puedes asistir, contáctanos para reprogramar.</p>
  <p>— Equipo de AniClinic</p>
</div>";
        }
    }
}