using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    // Guarda quién inició sesión para usarlo en toda la app
    public static class SesionActual
    {
        public static int IdEmpleado { get; set; } = 0;
        public static string NombreEmpleado { get; set; } = "";
    }
}

