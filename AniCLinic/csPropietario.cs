using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{ 

    internal class csPropietario
    {
        string nombre {  get; set; }
        string apellido { get; set; }
        string celular { get; set; }
        string cedula { get; set; }
        string correo { get; set; }
        string direccion {  get; set; }
        byte[] foto { get; set; }
        public string Nombre
        {
            get {  return nombre; }
            set {  nombre = value; }
        }
        public string Apellido
        {
            get { return apellido; }
            set { apellido = value; }
        }
        public string Celular
        {
            get { return celular; }
            set { celular = value; }
        }
        public string Cedula
        {
            get { return cedula; }
            set { cedula = value; }
        }
        public string Correo
        {
            get { return  correo; }
            set { correo = value; }
        }
        public string Direccion
        {
            get { return direccion; }
            set { direccion = value; }
        }
        public byte[] Foto
        {
            get { return foto; }
            set { foto = value; }
        }

        public csPropietario() { }

        public csPropietario(string nom, string ape, string cel, string ced, string cor, string dir, byte[] fot)
        {
            Nombre = nom;
            Apellido = ape;
            Celular = cel;
            Cedula = ced;
            Correo = cor;
            Direccion = dir;
            Foto = fot;
        }
        public int agregarPropietario()
        {
            csConexionBD conexionBD = new csConexionBD();
            int retorna = conexionBD.guardarRegistro("insert into Persona " +
                "values('" + Nombre + "','" + Apellido + "','" + Celular + "','" + Cedula + "', '" + Correo + "','" + Direccion + "', @Foto", Foto);
            return retorna;
        }
    }
}
