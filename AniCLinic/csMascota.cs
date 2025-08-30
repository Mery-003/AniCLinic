using System;
using System.Collections.Generic;
using System.Data;
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
        int idMascota { get; set; }
        string nombre { get; set; }
        string especie { get; set; }
        string raza { get; set; }
        string sexo { get; set; }
        string edad { get; set; }
        decimal peso { get; set; }
        string discapacidad { get; set; }
        byte[] foto { get; set; }
        int idPersona { get; set; }

        public int IdMascota
        {
            get { return idMascota; }
            set { idMascota = value; } 
        }
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Especie
        {
            get { return especie; }
            set { especie = value; }
        }
        public string Raza
        {
            get { return raza; }
            set { raza = value; }
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
        public int IdPersona
        {
            get { return idPersona; }
            set { idPersona = value; }
        }

        public csMascota() { }

        public csMascota(string nom, string esp, string raz, string sex, string ed, decimal pes, string disc, byte[] fot, int idpro)
        {
            Nombre = nom;
            Especie = esp;
            Raza = raz;
            Sexo = sex;
            Edad = ed;
            Peso = pes;
            Discapacidad = disc;
            Foto = fot;
            IdPersona = idpro;
        }

        public csMascota(int idMascota, string nombre, string especie, string raza, string sexo, string edad, decimal peso, string discapacidad, byte[] foto, int idPropietario)
        {
            IdMascota = idMascota;
            Nombre = nombre;
            Especie = especie;
            Raza = raza;
            Sexo = sexo;
            Edad = edad;
            Peso = peso;
            Discapacidad = discapacidad;
            Foto = foto;
            IdPersona = idPropietario;
        }

        public bool agregarMascota()
        {
            csCRUD crud = new csCRUD();
            return crud.agregarBD(
                "INSERT INTO Mascota (Imagen, Nombre, Especie, Raza, Sexo, Edad, PesoKg, Discapacidad, IdPersona) " +
                "VALUES (@Foto, @Nombre, @Especie, @Raza, @Sexo, @Edad, @Peso, @Discapacidad, @IdPropietario)",
                new SqlParameter("@Foto", Foto),
                new SqlParameter("@Nombre", Nombre),
                new SqlParameter("@Especie", Especie),
                new SqlParameter("@Raza", Raza),
                new SqlParameter("@Sexo", Sexo),
                new SqlParameter("@Edad", Edad),
                new SqlParameter("@Peso", Peso),
                new SqlParameter("@Discapacidad", Discapacidad),
                new SqlParameter("@IdPropietario", IdPersona));
        }

        public bool editarMascota(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.editarBD(
                "UPDATE Mascota SET Nombre=@Nombre, Especie=@Especie, Raza=@Raza, Sexo=@Sexo, Edad=@Edad, PesoKg=@Peso, Discapacidad=@Discapacidad, Imagen=@Foto, IdPersona=@IdPropietario " +
                "WHERE IdMascota=@Id",
                id,
                new SqlParameter("@Nombre", Nombre),
                new SqlParameter("@Especie", Especie),
                new SqlParameter("@Raza", Raza),
                new SqlParameter("@Sexo", Sexo),
                new SqlParameter("@Edad", Edad),
                new SqlParameter("@Peso", Peso),
                new SqlParameter("@Discapacidad", Discapacidad),
                new SqlParameter("@Foto", Foto),
                new SqlParameter("@IdPropietario", IdPersona));
        }

        public bool eliminarMascota(int id)
        {
            csCRUD crud = new csCRUD();
            return crud.eliminarBD("DELETE FROM Mascota WHERE IdMascota=@Id", id);
        }
    }
}

