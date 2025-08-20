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
        int id {  get; set; }
        string nombre { get; set; }
        string propietario { get; set; }
        string especie {  get; set; }
        string raza {  get; set; }
        string sexo { get; set; }
        string edad {  get; set; }
        string discapacidad { get; set; }
        byte[] foto { get; set; }
        
        public int Id
        {
            get {  return id; }
            set { id = value; }
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Propietario
        {
            get { return propietario; } 
            set { propietario = value; }
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
        public csMascota(int idp,string nom, string prop, string esp, string raz, string sex, string ed, string disc, byte[] fot)
        {
            Id = idp;
            Nombre = nom;
            Propietario = prop;
            Especie = esp;
            Raza = raz;
            Sexo = sex;
            Edad = ed;
            Discapacidad = disc;
            Foto = fot;
        }
        public int agregarMascota()
        {
            csConexionBD conexionBD = new csConexionBD();
            
            int retorna = conexionBD.guardarRegistro("insert into tblMascotas " +
                "values("+ Id + ",'"+ Nombre +"','"+ Propietario +"','"+ Especie +"','"+ Raza +"','"+ Sexo +"','"+ Edad +"','"+ Discapacidad +"', @Foto)", Foto);
            return retorna;
        }
    }
}
