//--------------------------------------------------------------------------------
// <copyright file="BuscarClasificHandler.cs" company="Universidad Católica del Uruguay">
//     Copyright (c) Programación II. Derechos reservados.
// </copyright>
//--------------------------------------------------------------------------------
using System;
using Library;

namespace Handlers
{
    /// <summary>
    /// Un "handler" del patrón Chain of Responsibility que implementa el comando "chau".
    /// </summary>
    public class BuscarClasificHandler : BaseHandler
    {
        /// <summary>
        /// El usuario que busca ofertas.
        /// </summary>
        public Emprendedor emprendedor;

        private Impresora impresora;

        /// <summary>
        /// base de datos.
        /// </summary>
        public Contenedor db;
        /// <summary>
        /// Buscador de ofertas.
        /// </summary>
        public Busqueda buscador;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BuscarHandler"/>. Esta clase procesa el mensaje "chau"
        /// y el mensaje "adiós" -un ejemplo de cómo un "handler" puede procesar comandos con sinónimos.
        /// </summary>
        /// <param name="next">El próximo "handler".</param>
        public BuscarClasificHandler(BaseHandler next) : base(next)
        {
            this.Keywords = new string[] { "/BClasificacion" };
        }

        /// <summary>
        /// Procesa el mensaje "chau" y retorna true; retorna false en caso contrario.
        /// </summary>
        /// <param name="message">El mensaje a procesar.</param>
        /// <param name="response">La respuesta al mensaje procesado.</param>
        /// <returns>true si el mensaje fue procesado; false en caso contrario.</returns>
        protected override bool InternalHandle(IMessage message, out string response)
        {
            Contenedor db = Contenedor.Instancia;
            Busqueda buscador = Busqueda.Instancia;
            Impresora impresora = Impresora.Instancia;
            if (this.CanHandle(message))
            {
                if (db.Emprendedores.ContainsKey(message.ID))
                {
                    string busca = message.Text.Remove(0,15);
                    busca = busca.Trim();
                    if (busca.Length <= 0)
                    {
                        response = "No se ha ingresado ningun criterio de busqueda. Use /bclasificacion \"Clasificacion\"";
                        return true;
                    }
                    else
                    {
                    foreach (Clasificacion clasificacion in db.Clasificaciones)
                    {
                        if (clasificacion.Nombre.Equals(busca, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string emprend = message.ID.ToString();
                            string OfertasValidas = impresora.Imprimir(buscador.BuscarOferta(db.Emprendedores[emprend],clasificacion,db));
                            response = $"{OfertasValidas}";
                            return true;
                        }
                    }
                    }
                }
                else
                {
                    response = "No estás registrado como emprendedor";
                    return true;
                }
            }
            response = string.Empty;
            return false;
        }
    }
}