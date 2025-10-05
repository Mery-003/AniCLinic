using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class FControlLaboral : Form
    {
        private readonly Menu _menu;
        private readonly Panel _host;

        public FControlLaboral(Menu menu, Panel host)
        {
            InitializeComponent();
            _menu = menu;
            _host = host;

            try
            {
                btnAsistencias.Click -= AbrirAsistencias;
                btnAsistencias.Click += AbrirAsistencias;
            }
            catch { }

            HookClicksByTag("Asistencias", AbrirAsistencias);
        }

        public FControlLaboral() : this(null, null) { }

        private void AbrirAsistencias(object sender, EventArgs e)
        {
            var frm = new FrmAsistencia(); 

            if (_menu != null && _host != null && !_host.IsDisposed)
                _menu.AbrirEnPanel(_host, frm);  
            else
                frm.Show();                       
        }


        private void HookClicksByTag(string tag, EventHandler handler)
        {
            foreach (var c in GetAllControls(this).Where(c =>
                     (c.Tag != null && string.Equals(c.Tag.ToString(), tag, StringComparison.OrdinalIgnoreCase))))
            {
                c.Cursor = Cursors.Hand;
                c.Click -= handler;
                c.Click += handler;
            }
        }

        private static IEnumerable<Control> GetAllControls(Control root)
        {
            var stack = new Stack<Control>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var ctrl = stack.Pop();
                foreach (Control child in ctrl.Controls) stack.Push(child);
                if (ctrl != root) yield return ctrl;
            }
        }

        private void btnAsistencias_Click(object sender, EventArgs e)
        {
            if (this.MdiParent is Menu menu)
            {
                if (_menu != null && _host != null)
                    _menu.AbrirEnPanel(_host, new FrmAsistencia());
                else
                    new FrmAsistencia().Show();
            }
        }

        private void btnPermisos_Click(object sender, EventArgs e)
        {
            if (_menu != null && _host != null)
                _menu.AbrirEnPanel(_host, new FrmPermiso());
            else
                new FrmAsistencia().Show();
        }

        private void btnPagos_Click(object sender, EventArgs e)
        {
            if (_menu != null && _host != null)
                _menu.AbrirEnPanel(_host, new FrmPagos());
            else
                new FrmAsistencia().Show();
        }
    }
}
