//--------------------------------------------------------------------------------
// <copyright file="Rubro.cs" company="Universidad Católica del Uruguay">
//     Copyright (c) Programación II. Derechos reservados.
// </copyright>
//--------------------------------------------------------------------------------
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library
{
    /// <summary>
    /// Clase que representa los rubros.
    /// </summary>
    public class Rubro: IJsonSerialize
    {
        private string nombre;
        private string area;
        private string descripcion;

        /// <summary>
        /// Constructor vacío de rubro para la deserialización
        /// </summary>
        public Rubro()
        {

        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Rubro"/>.
        /// </summary>
        /// <param name="nombre">El nombre del rubro.</param>
        /// <param name="area">El area del Rubro.</param>
        /// <param name="descripcion">La descripcion del rubro.</param>
        [JsonConstructor]
        public Rubro(string nombre, string area, string descripcion)
        {
            this.nombre = nombre;
            this.area = area;
            this.descripcion = descripcion;
        }

        /// <summary>
        /// Obtiene un valor que indica el nombre del rubro.
        /// </summary>
        /// <value>El nombre del Rubro.</value>
        public string Nombre
        {
            get
            {
                return this.nombre;
            }
        }

        /// <summary>
        /// Obtiene un valor que indica el area del rubro.
        /// </summary>
        /// <value>El area del Rubro.</value>
        public string Area
        {
            get
            {
                return this.area;
            }
        }

        /// <summary>
        /// Obtiene un valor que indica la descripcion del rubro.
        /// </summary>
        /// <value>La descripcion del Rubro.</value>
        public string Descripcion
        {
            get
            {
                return this.descripcion;
            }
        }

        /// <summary>
        /// Convert to Json.
        /// </summary>
        /// <returns></returns>
        public string ConvertToJson()
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = MyReferenceHandler.Instance, 
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(this, options);
            return json;
        }
    }
}