using System;
using System.Reflection;
using System.Windows.Forms;

namespace AniCLinic
{
    internal static class UxBuscarCedulaHelper
    {
        public static void Wire(Control txt, Action<string> onBuscar)
        {
            var p = txt.GetType().GetProperty("MaxLength", BindingFlags.Public | BindingFlags.Instance);
            if (p != null && p.CanWrite) p.SetValue(txt, 10);

            txt.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            txt.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var ced = txt.Text?.Trim() ?? "";
                    onBuscar(ced.Length == 10 ? ced : null);
                    e.SuppressKeyPress = true;
                }
            };

            txt.TextChanged += (s, e) =>
            {
                var t = txt.Text?.Trim() ?? "";
                if (t.Length == 10) onBuscar(t);
                else if (t.Length == 0) onBuscar(null);
            };
        }
    }
}
