using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AgregarPaciente : Form
    {
        csCRUD crud = new csCRUD();
        fPacientes fp;
        public int IdMascota { get; set; }
        public int IdPropietario { get; set; }

        public AgregarPaciente()
        {
            InitializeComponent();
        }

        public AgregarPaciente(fPacientes p)
        {
            InitializeComponent();
            fp = p;
        }

        public AgregarPaciente(fPacientes p, int idMascota)
        {
            InitializeComponent();
            fp = p;

            if (idMascota != 0)
            {
                csMascota mascota = cargarMascota(idMascota);
                csPropietario propietario = cargarPropietario(mascota.IdPersona);
                txtMascotaNombre.Text = mascota.Nombre;
                cmbEspecie.SelectedItem = mascota.Especie;
                cmbRaza.SelectedItem = mascota.Raza;
                cmbSexo.SelectedItem = mascota.Sexo;
                txtEdad.Text = mascota.Edad;
                txtPeso.Text = mascota.Peso.ToString();
                cmbDiscapacidad.SelectedItem = mascota.Discapacidad;
                picMascota.Image = Image.FromStream(new MemoryStream(mascota.Foto));
                txtNombreD.Text = propietario.Nombre;
                txtApellido.Text = propietario.Apellido;
                txtCedula.Text = propietario.Cedula;
                txtCelular.Text = propietario.Celular;
                txtCorreo.Text = propietario.Correo;
                txtDireccion.Text = propietario.Direccion;
                picPropietario.Image = Image.FromStream(new MemoryStream(propietario.Foto));
                this.IdMascota = idMascota;
                this.IdPropietario = mascota.IdPersona;
                
            }
        }

        private csMascota cargarMascota(int idMascota)
        {
            csMascota mascota = null;
            string sentencia = "SELECT * FROM Mascota WHERE IdMascota = ";
            SqlDataReader reader = crud.EjecutarQuery(sentencia + idMascota);

            if (reader.Read())
            {
                mascota = new csMascota(
                    Convert.ToInt32(reader["IdMascota"]),
                    reader["Nombre"].ToString(),
                    reader["Especie"].ToString(),
                    reader["Raza"].ToString(),
                    reader["Sexo"].ToString(),
                    reader["Edad"].ToString(),
                    Convert.ToDecimal(reader["PesoKg"]),
                    reader["Discapacidad"].ToString(),
                    (byte[])reader["Imagen"],
                    Convert.ToInt32(reader["IdPersona"])
                );
            }

            reader.Close();
            return mascota;
        }

        private csPropietario cargarPropietario(int idPropietario)
        {
            csPropietario propietario = null;
            string sentencia = "Select * from Persona where IdPersona = ";
            SqlDataReader reader = crud.EjecutarQuery(sentencia + idPropietario);
            if (reader.Read())
            {
                propietario = new csPropietario(
                    reader["Nombre"].ToString(),
                    reader["Apellido"].ToString(),
                    reader["Celular"].ToString(),
                    reader["Cedula"].ToString(),
                    reader["Correo"].ToString(),
                    reader["DireccionDomiciliaria"].ToString(),
                    (byte[])reader["Imagen"]);
            }
            reader.Close();
            return propietario;
        }
        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                ImageConverter converter = new ImageConverter();
                byte[] fotoM = (byte[])converter.ConvertTo(picMascota.Image, typeof(byte[]));
                byte[] fotoP = (byte[])converter.ConvertTo(picPropietario.Image, typeof(byte[]));

                csPropietario pro = new csPropietario(txtNombreD.Text, txtApellido.Text, txtCelular.Text, txtCedula.Text, txtCorreo.Text, txtDireccion.Text, fotoP);

                if (pro.agregarPropietario())
                {
                    MessageBox.Show("El propietario se agregó correctamente.");
                    int idPropietario = pro.obtenerIdPropietario();

                    csMascota masc = new csMascota(txtMascotaNombre.Text, cmbEspecie.Text, cmbRaza.Text, cmbSexo.Text, txtEdad.Text + " " + cmbEdadUnidad.Text, Convert.ToDecimal(txtPeso.Text), cmbDiscapacidad.Text, fotoM, idPropietario);

                    if (masc.agregarMascota())
                    {
                        MessageBox.Show("La mascota se agregó correctamente.");
                    }
                }

                fp.cargarData();
                LimpiarFormulario();
            } catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            txtMascotaNombre.Text = "";
            txtEdad.Text = "";
            txtPeso.Text = "";
            cmbEspecie.SelectedIndex = -1;
            cmbRaza.SelectedIndex = -1;
            cmbSexo.SelectedIndex = -1;
            cmbEdadUnidad.SelectedIndex = -1;
            cmbDiscapacidad.SelectedIndex = -1;
            txtNombreD.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtCelular.Text = "";
            txtCorreo.Text = "";
            txtDireccion.Text = "";
        }

        private void btnFotoMascota_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Seleccionar Imagen";
            ofd.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picMascota.Image = Image.FromFile(ofd.FileName);
                picMascota.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnFotoPropietario_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Seleccionar Imagen";
            ofd.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                picPropietario.Image = Image.FromFile(ofd.FileName);
                picPropietario.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void cmbEspecie_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEspecie.SelectedIndex == 0)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "American Bully", "American Pit Bull Terrier", "American Staffordshire Terrier", "Beagle", "Bulldog Inglés", "Chihuahua", "Dálmata", "Golden Retriever", "Husky Siberiano", 
                    "Labrador Retriever", "Lobo Siberiano", "Pastor Alemán", "Perro Peruano","Pug", "San Bernardo", "Staffordshire Bull Terrier", "Yorkshire Terrier" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 1)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Abisinio", "Bengalí", "British Shorthair", "Esfinge (Sphynx)", "Maine Coon", "Persa", "Ragdoll", "Siamés" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 2)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Angora", "Californiano", "Conejo Gigante de Flandes", "Conejo Mini Lop", "Conejo Netherland Dwarf" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 3)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Hámster Roborovski", "Hámster Ruso enano", "Hámster Sirio (dorado)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 4)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Beige", "Blanco mosaico", "Gris estándar" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 5)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Albino", "Champagne", "Sable" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 6)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Albino", "Azul", "Lutino", "Verde común" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 7)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Amazona de frente azul", "Conuro aratinga", "Guacamayo azul y amarillo", "Guacamayo escarlata", "Loro yaco (gris africano)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 8)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Cacatúa alba (moño blanco)", "Cacatúa galerita (sulfur crest)", "Cacatúa ninfa (Carolina)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 9)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Tortuga de orejas rojas", "Tortuga rusa", "Tortuga sulcata" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 10)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Camaleón de Jackson", "Camaleón pantera", "Camaleón velado" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 11)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Andaluz (PRE)", "Árabe", "Cuarto de Milla", "Frisón", "Pura Sangre Inglés" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 12)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Angus", "Hereford", "Holstein", "Jersey" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 13)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Dorper", "Merino", "Suffolk", "Texel" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 14)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Alpina", "Boer", "Saanen" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 15)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Duroc", "Ibérico", "Landrace", "Yorkshire" });
                return;
            }
        }

        private void txtEdad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPeso_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private void txtCelular_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}
