//--------------------------------------------------------------------------------
// <copyright file="TestBusquedaClasific.cs" company="Universidad Católica del Uruguay">
//     Copyright (c) Programación II. Derechos reservados.
// </copyright>
//--------------------------------------------------------------------------------
using NUnit.Framework;
using Handlers;
using Library;
using Telegram.Bot.Types;
using System;
using System.Collections.ObjectModel;

namespace ProgramTests
{
    /// <summary>
    /// Esta clase prueba el handler de PublicarOferta. Concretamente cuando se toma la ruta de oferta única.
    /// </summary>
    public class TestBusquedaClasific
    {
        BuscarClasificHandler handler;
        Message message;

        TelegramMSGadapter msj;
        Contenedor db = Contenedor.Instancia;
        Clasificacion clasificacionTest;

        Emprendedor emprendedor;

        Busqueda buscador = Busqueda.Instancia;

        /// <summary>
        /// Crea una instancia de rubro, emprendedor, dos empresas, clasificación y dos ofertas para la búsqueda.
        /// estas se utilizan para el test de búsqueda a continuación.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Rubro rubroMadera = new Rubro("Forestal", "Leñeria", "Recursos");
            Empresa barracaFernandez = new Empresa("Madera SRL", rubroMadera, "San Bautista", "Ruta 6","66444","099222333");
            Empresa carpinteriaRodriguez = new Empresa("Madera SRL", rubroMadera, "San Bautista", "Ruta 6","6664333","099222333");
            Clasificacion madera = new Clasificacion("Madera", "Madera natural");
            Oferta uno = new Oferta("Madera tratada", barracaFernandez, "San Ramon", "Tala", "madera", madera, 1, "Tonelada", 5000, 0, DateTime.Parse("13/09/2021"));
            db.AddOferta(uno);
            Oferta dos = new Oferta("Madera encofrado", carpinteriaRodriguez, "Montevideo", "Bulevar Artigas", "madera", madera, 100, "Kilos", 4000, 0, DateTime.Parse("11/11/2021"));
            db.AddOferta(dos);
            db.AddClasificacion(madera);
            db.AddRubro(rubroMadera);
            handler = new BuscarClasificHandler(null);
            message = new Message();
            message.From = new User();
            message.From.Id = 14444;
            msj = new TelegramMSGadapter(message);
            db.AddEmprendedor("Gaston Pereira", rubroMadera, "San Ramon", "Ruta 12", "Emprendimiento","14444");
        }

        /// <summary>
        /// Este test prueba la busqueda por clasificación de los materiales.
        /// </summary>
        [Test]
        public void TestBusquedaPalabras()
        {
            message.Text = "/bclasificacion madera";
            string response;
            string respuestaesperada;

            string mensaje = message.Text.Remove(0,15);
            Clasificacion clasificacionBuscar = new Clasificacion(mensaje.Trim(),"descripcion");
            Collection<Oferta> ofertasvalidas = buscador.BuscarOferta(db.Emprendedores["14444"], clasificacionBuscar, db);
            if (ofertasvalidas.Count == 0)
            {
                respuestaesperada = "No hay ofertas disponibles";
            }
            else
            {
                respuestaesperada = "OFERTAS DISPONIBLES: \n";
                foreach (Oferta oferta in ofertasvalidas)
                {
                string textorecurrente = "";
                    if (oferta.RecurrenciaSemanal == 0)
                    {
                        textorecurrente = "Oferta única";
                    }
                    else if (oferta.RecurrenciaSemanal > 0)
                    {
                        textorecurrente = "Oferta recurrente cada " + oferta.RecurrenciaSemanal + " semanas";
                    }
                    respuestaesperada += "\n";
                    respuestaesperada += "Nombre: " + oferta.Nombreoferta + "\n";
                    respuestaesperada += "Material: " + oferta.Material.Nombre + "\n";
                    respuestaesperada += "Cantidad: " + oferta.Material.Cantidad +" "+oferta.Material.Unidad + "\n";
                    respuestaesperada += "Precio: $" + oferta.Material.Valor + "\n";
                    respuestaesperada += textorecurrente + "\n";
                    respuestaesperada += "Identificador: " + db.Ofertas.IndexOf(oferta).ToString() + "\n";
                    respuestaesperada += "Ofrecido por: " + oferta.Empresa.Nombre + "\n";
                    respuestaesperada += "Teléfono de contacto: " + oferta.Empresa.Telefono+"\n";
                    respuestaesperada += "\n";
                    respuestaesperada += "---------------------------------------" + "\n";
                }   
            }

            IHandler result = handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(respuestaesperada));
            
        }

    }

}
