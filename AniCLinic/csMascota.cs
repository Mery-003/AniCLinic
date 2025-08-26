using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace AniCLinic
{
    internal class csMascota
    {
        string nombre { get; set; }
        string especie {  get; set; }
        string raza {  get; set; }
        string sexo { get; set; }
        string edad { get; set; }
        decimal peso { get; set; }
        string discapacidad { get; set; }
        byte[] foto { get; set; }
        
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Especie
        {
            get { return  especie; }
            set { especie = value; }
        }
        public string Raza
        {
            get { return raza; }
            set {  raza = value; }
        }
        public string Sexo
        {
            get { return sexo; }
            set { sexo = value; }
        }
        public string Edad
        {
            get { return edad; }
            set { edad = value; }
        }
        public decimal Peso
        {
            get { return peso; }
            set { peso = value; }
        }
        public string Discapacidad
        {
            get { return discapacidad; }
            set { discapacidad = value; }
        }
        public byte[] Foto
        {
            get { return foto; }
            set { foto = value; }
        }
        public csMascota() { }
        public csMascota(string nom, string esp, string raz, string sex, string ed, decimal pes ,string disc, byte[] fot)
        {
            Nombre = nom;
            Especie = esp;
            Raza = raz;
            Sexo = sex;
            Edad = ed;
            Peso = pes;
            Discapacidad = disc;
            Foto = fot;
        }
        public int agregarMascota()
        {
            csConexionBD conexionBD = new csConexionBD();
            int retorna = conexionBD.guardarRegistro("insert into Mascota " +
                "values(@Foto, '"+ Nombre +"','"+ Especie +"','"+ Raza +"','"+ Sexo + "', '"+ Edad +"','" + Peso +"','" + Discapacidad +"'", Foto);
            return retorna;
        }
    }
}
