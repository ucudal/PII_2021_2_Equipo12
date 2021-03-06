//--------------------------------------------------------------------------------
// <copyright file="TestOfertasVendidasDesde.cs" company="Universidad Católica del Uruguay">
//     Copyright (c) Programación II. Derechos reservados.
// </copyright>
//--------------------------------------------------------------------------------
using NUnit.Framework;
using Handlers;
using Library;
using Telegram.Bot.Types;
using System;

namespace ProgramTests
{
    /// <summary>
    /// Esta clase prueba el handler de HistorialDesde. Concretamente desde el punto de vista de una empresa
    /// Le permitira a la empresa saber todos los materiales o residuos entregados en un período de tiempo.
    /// </summary>
    public class TestOfertasVendidasDesde
    {
        HistorialUsuarioHandler handler;
        Message message;

        TelegramMSGadapter msj;
        Contenedor db = Contenedor.Instancia;
        Clasificacion clasificacionTest;

        Emprendedor emprendedor;


        /// <summary>
        /// Crea una Empresa y un emprendedor (Asi como un rubro). Luego se crea la oferta de la empresa
        /// Luego se le agrega un comprador a la oferta (el emprendedor), Asi como agregarlo al registro de c/usuario
        /// (Esto se demuestra que se hace automaticamente en el handler de /AnadirComprador).
        /// Por ultimo se crea el message asi como el TelegramMSGadpater.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Rubro rubroMadera = new Rubro("Forestal", "Leñeria", "Recursos");
            db.AddEmpresa("Madera SRL", rubroMadera, "San Bautista", "Ruta 6","201","099222333");
            db.AddEmprendedor("Emprendedor Prueba", rubroMadera,"San Bautista", "Ruta 6","madera","301");
            Clasificacion madera = new Clasificacion("Madera", "Madera natural");
            Oferta uno = new Oferta("Madera tratada", db.Empresas["24"] , "San Ramon", "Tala", "madera", madera, 1, "Tonelada", 5000, 0, DateTime.Parse("13/09/2021"));
            DateTime fecha = DateTime.Parse("16/12/2021");
            uno.AddComprador("301",fecha);
            db.AddOferta(uno);
            db.Emprendedores["301"].AddToRegister(uno);
            db.Empresas["201"].AddToRegister(uno);
            handler = new HistorialUsuarioHandler(null);
            message = new Message();
            message.From = new User();
            message.From.Id = 201;
            msj = new TelegramMSGadapter(message);
        }

        /// <summary>
        /// Test que simula una interaccion desde un usuario (Empresa) que consulta las ofertas vendidas desde
        /// una fecha anterior.
        /// </summary>
        [Test]
        public void EmpresaHistorialDesde()
        {
            message.Text = handler.Keywords[0];
            string response;
            IHandler result = handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Ingrese la fecha desde donde desee consultar el registro.\nRecuerde que la fecha debe tener la forma dd/mm/aaaa"
                ));

            message.Text = "20/11/2021";
            string lineas ="------------------------------------------------------";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Ofertas Consumidas Desde: 20/11/2021\n\nNOMBRE: Madera tratada\nNOMBRE MATERIAL: madera 1 Tonelada\n\nFECHA COMPRA: 16/12/2021 0:00:00\n\n"+lineas+"\n"
                ));
        }

    }
}