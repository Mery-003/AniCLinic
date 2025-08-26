using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class VerHistorial : Form
    {
        private readonly ConexionBD _db = new ConexionBD();

        
        public VerHistorial(int idRegistroClinico)
        {
        }
        public VerHistorial(int idMascota, bool listarPorMascota)
        {
          
        }

        private void VerHistorial_Load(object sender, EventArgs e)
        {
            
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();

       
        private void guna2Button3_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel3_Click(object sender, EventArgs e) { }
    }
}
