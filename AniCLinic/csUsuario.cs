using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCLinic
{
    internal class csUsuario
    {
        int id { get; set; }
        string usuario { get; set; }
        string contraseña { get; set; }
        string cargo { get; set; }
        decimal sueldo { get; set; }
        public int Id 
        { 
            get { return id; } 
            set { id = value; } 
        }
        public string Usuario 
        {
            get { return usuario; }
            set { usuario = value; }
        }
        public string Contraseña
        {
            get { return contraseña; }
            set { contraseña = value; }
        }
        public string Cargo
        {
            get { return  cargo; }
            set { cargo = value; }
        }
        public decimal Sueldo
        {
            get { return sueldo; }
            set { sueldo = value; }
        }
        public csUsuario() { }
        public csUsuario(string user, string pass, string cargo, decimal sueld)
        {
            Usuario = user;
            Contraseña = pass;
            Cargo = cargo;
            Sueldo = sueld;
        }
        public csUsuario(int id, string user, string pass, string cargo, decimal sueld)
        {
            Id = id;
            Usuario = user;
            Contraseña = pass;
            Cargo = cargo;
            Sueldo = sueld;
        } 
    }
}
