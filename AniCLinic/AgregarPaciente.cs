using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AniCLinic
{
    public partial class AgregarPaciente : Form
    {
        csCRUD crud = new csCRUD();
        fPacientes fp;

        public int IdMascota { get; set; }
        public int IdPropietario { get; set; }

        private bool edicion = false;
        private byte[] _fotoMascotaOriginal = null;
        private byte[] _fotoPropietarioOriginal = null;

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
            edicion = true;

            if (idMascota != 0)
            {
                csMascota mascota = cargarMascota(idMascota);
                if (mascota == null)
                {
                    MessageBox.Show("No se encontró la mascota.");
                    return;
                }

                csPersona propietario = cargarPropietario(mascota.IdPersona);
                if (propietario == null)
                {
                    MessageBox.Show("No se encontró el propietario.");
                    return;
                }

                txtMascotaNombre.Text = mascota.Nombre ?? "";
                cmbEspecie.Text = mascota.Especie ?? "";
                cmbRaza.Text = mascota.Raza ?? "";
                cmbSexo.Text = mascota.Sexo ?? "";
                txtEdad.Text = (mascota.Edad ?? "").Split(' ')[0]; 
                if (!string.IsNullOrWhiteSpace(mascota.Edad))
                {
                    var partes = mascota.Edad.Split(' ');
                    if (partes.Length > 1) 
                        cmbEdadUnidad.Text = partes[1];
                }
                txtPeso.Text = Convert.ToString(mascota.Peso, CultureInfo.CurrentCulture);
                cmbDiscapacidad.Text = mascota.Discapacidad ?? "";

                if (mascota.Foto != null && mascota.Foto.Length > 0)
                {
                    _fotoMascotaOriginal = mascota.Foto;
                    try
                    {
                        picMascota.Image = Image.FromStream(new MemoryStream(mascota.Foto));
                        picMascota.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch 
                    { 
                        picMascota.Image = null; 
                    }
                }
                else
                {
                    picMascota.Image = null;
                }

                txtNombreD.Text = propietario.Nombre ?? "";
                txtApellido.Text = propietario.Apellido ?? "";
                txtCedula.Text = propietario.Cedula ?? "";
                txtCelular.Text = propietario.Celular ?? "";
                txtCorreo.Text = propietario.Correo ?? "";
                txtDireccion.Text = propietario.Direccion ?? "";

                if (propietario.Foto != null && propietario.Foto.Length > 0)
                {
                    _fotoPropietarioOriginal = propietario.Foto;
                    try
                    {
                        picPropietario.Image = Image.FromStream(new MemoryStream(propietario.Foto));
                        picPropietario.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch 
                    { 
                        picPropietario.Image = null;
                    }
                }
                else
                {
                    picPropietario.Image = null;
                }

                this.IdMascota = idMascota;
                this.IdPropietario = mascota.IdPersona;
            }
        }

        private csMascota cargarMascota(int idMascota)
        {
            csMascota mascota = null;
            string sentencia = "SELECT * FROM Mascota WHERE IdMascota = " + idMascota;
            using (SqlDataReader reader = crud.EjecutarQuery(sentencia))
            {
                if (reader != null && reader.Read())
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
                        reader["Imagen"] == DBNull.Value ? null : (byte[])reader["Imagen"],
                        Convert.ToInt32(reader["IdPersona"])
                    );
                }
            }
            return mascota;
        }

        private csPersona cargarPropietario(int idPropietario)
        {
            csPersona propietario = null;
            string sentencia = "SELECT * FROM Persona WHERE IdPersona = " + idPropietario;
            using (SqlDataReader reader = crud.EjecutarQuery(sentencia))
            {
                if (reader != null && reader.Read())
                {
                    propietario = new csPersona(
                        reader["Nombre"].ToString(),
                        reader["Apellido"].ToString(),
                        reader["Celular"].ToString(),
                        reader["Cedula"].ToString(),
                        reader["Correo"].ToString(),
                        reader["DireccionDomiciliaria"].ToString(),
                        reader["Imagen"] == DBNull.Value ? null : (byte[])reader["Imagen"]
                    );
                }
            }
            return propietario;
        }

        private void btncancelar2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static byte[] ImageToBytesOrNull(Image img)
        {
            if (img == null) return null;
            using (var ms = new MemoryStream())
            {
                img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] fotoM = ImageToBytesOrNull(picMascota.Image) ?? _fotoMascotaOriginal;
                byte[] fotoP = ImageToBytesOrNull(picPropietario.Image) ?? _fotoPropietarioOriginal;

                if (string.IsNullOrWhiteSpace(txtMascotaNombre.Text))
                {
                    MessageBox.Show("Ingrese el nombre de la mascota.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    MessageBox.Show("Ingrese la cédula del propietario.");
                    return;
                }

                csPersona pro = new csPersona(
                    txtNombreD.Text,
                    txtApellido.Text,
                    txtCelular.Text,
                    txtCedula.Text,
                    txtCorreo.Text,
                    txtDireccion.Text,
                    fotoP
                );

                int idPropietarioParaGuardar = IdPropietario;

                if (edicion)
                {
                    var idCedula = new csPersona().obtenerIdPorCedula(txtCedula.Text);
                    if (idCedula > 0 && idCedula != IdPropietario)
                    {
                        MessageBox.Show("La cédula ingresada ya pertenece a otro propietario.");
                        return;
                    }

                    if (!pro.editarPersona(IdPropietario))
                    {
                        MessageBox.Show("No se pudo actualizar el propietario.");
                        return;
                    }
                }
                else
                {
                    var idExistente = new csPersona().obtenerIdPorCedula(txtCedula.Text);
                    if (idExistente <= 0)
                    {
                        idPropietarioParaGuardar = idExistente;
                    }
                    else
                    {
                        if (pro.agregarPersona())
                        {
                            idPropietarioParaGuardar = pro.obtenerIdPersona();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo registrar el propietario.");
                            return;
                        }
                    }
                }

                string edadConUnidad = string.IsNullOrWhiteSpace(cmbEdadUnidad.Text)
                    ? txtEdad.Text : (txtEdad.Text + " " + cmbEdadUnidad.Text).Trim();

                decimal pesoDecimal = 0m;
                if (!string.IsNullOrWhiteSpace(txtPeso.Text))
                    pesoDecimal = Convert.ToDecimal(txtPeso.Text, CultureInfo.CurrentCulture);

                csMascota masc = new csMascota(
                    txtMascotaNombre.Text,
                    cmbEspecie.Text,
                    cmbRaza.Text,
                    cmbSexo.Text,
                    edadConUnidad,
                    pesoDecimal,
                    cmbDiscapacidad.Text,
                    fotoM,
                    idPropietarioParaGuardar
                );

                bool okMascota = edicion ? masc.editarMascota(IdMascota) : masc.agregarMascota();

                if (!okMascota)
                {
                    MessageBox.Show("No se pudo guardar la mascota.");
                    return;
                }

                MessageBox.Show(edicion ? "Registro actualizado correctamente." : "Registro creado correctamente.");

                RefrescarGridPacientes();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void RefrescarGridPacientes()
        {
            if (fp == null) 
                return;

            var tipo = fp.GetType();

            var mCargarData = tipo.GetMethod("CargarData", Type.EmptyTypes);
            if (mCargarData != null)
            {
                mCargarData.Invoke(fp, null);
                return;
            }

            var mCargarDataOld = tipo.GetMethod("cargarData", Type.EmptyTypes);
            if (mCargarDataOld != null)
            {
                mCargarDataOld.Invoke(fp, null);
                return;
            }
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
                    "Labrador Retriever", "Lobo Siberiano", "Pastor Alemán", "Perro Peruano","Pug", "San Bernardo", "Staffordshire Bull Terrier", "Yorkshire Terrier" });
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
                cmbRaza.Items.AddRange(new string[] { "Angora", "Californiano", "Conejo Gigante de Flandes", "Conejo Mini Lop", "Conejo Netherland Dwarf" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 3)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Hámster Roborovski", "Hámster Ruso enano", "Hámster Sirio (dorado)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 4)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Beige", "Blanco mosaico", "Gris estándar" });
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
                cmbRaza.Items.AddRange(new string[] { "Albino", "Azul", "Lutino", "Verde común" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 7)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Amazona de frente azul", "Conuro aratinga", "Guacamayo azul y amarillo", "Guacamayo escarlata", "Loro yaco (gris africano)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 8)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Cacatúa alba (moño blanco)", "Cacatúa galerita (sulfur crest)", "Cacatúa ninfa (Carolina)" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 9)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Tortuga de orejas rojas", "Tortuga rusa", "Tortuga sulcata" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 10)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Camaleón de Jackson", "Camaleón pantera", "Camaleón velado" });
                return;
            }
            if (cmbEspecie.SelectedIndex == 11)
            {
                cmbRaza.Items.Clear();
                cmbRaza.Items.AddRange(new string[] { "Andaluz (PRE)", "Árabe", "Cuarto de Milla", "Frisón", "Pura Sangre Inglés" });
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
            if ((!char.IsDigit(e.KeyChar) || txtEdad.Text.Length >= 2) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPeso_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char.IsDigit(e.KeyChar) && txtPeso.Text.Replace(",", "").Replace(".", "").Length >= 4)
                || (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != ',')
                || (e.KeyChar == ',' && txtPeso.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void txtCelular_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCelular.Text.Length >= 10) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsDigit(e.KeyChar) || txtCedula.Text.Length >= 10) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }
    }
}
