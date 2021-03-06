//--------------------------------------------------------------------------------
// <copyright file="TesPublicarOfertaRecurrente.cs" company="Universidad Católica del Uruguay">
//     Copyright (c) Programación II. Derechos reservados.
// </copyright>
//--------------------------------------------------------------------------------
using NUnit.Framework;
using Handlers;
using Library;
using Telegram.Bot.Types;

namespace ProgramTests
{
    /// <summary>
    /// Esta clase prueba el handler de PublicarOferta. Concretamente cuando se toma la ruta de oferta recurrente.
    /// </summary>
    public class PublicarOfertaRecurrenteTests
    {
        PublicarOfertaHandler handler;
        Message message;

        TelegramMSGadapter msj;
        Contenedor db;
        Clasificacion clasificacionTest;

        Empresa empresaTest;

        Rubro rubroTest;

        /// <summary>
        /// Crea una instancia de clasificacion, de rubro, de contenedor, el handler a utilizar, un message junto a
        /// un user que se le agrega la ID asi como el msj Adapter. Por ultimo se crea la empresa a publicar la oferta.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            clasificacionTest = new Clasificacion("Reciclable","Material Reciclable");
            rubroTest = new Rubro("Prueba","Prueba","Prueba");
            db = Contenedor.Instancia;
            db.AddClasificacion(clasificacionTest);
            handler = new PublicarOfertaHandler(null);
            message = new Message();
            message.From = new User();
            message.From.Id = 13;
            msj = new TelegramMSGadapter(message);
            db.AddEmpresa("EmpresaTest",rubroTest,"Montevideo","calle 13","13","099222333");
        }

        /// <summary>
        /// Este test prueba como se procesan los mensajes involucrados en la creacion de una oferta recurrente.
        /// </summary>
        [Test]
        public void TestPublicarOfertaRecurrenteHandler()
        {
            message.Text = handler.Keywords[0];
            string response;

            IHandler result = handler.Handle(msj, out response);

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Se procedera con la publicacion, primero indique el tipo de oferta que desea crear:\n1 - Oferta Única \n2 - Oferta Recurrente"
                ));
            
            message.Text = "2";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Usted eligió crear una Oferta Recurrente"+"\n \n"+"Ingrese el nombre de la oferta"
                ));

            message.Text = "NombreOferta";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "El nombre de la oferta es: "+"NombreOferta"+"\n \n" +"Ingrese la ciudad donde se encuentra la oferta:"
                ));
            
                message.Text = "Montevideo";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "La ciudad de la oferta es: "+"Montevideo"+"\n \n"+"Ingrese la calle donde se encuentra la oferta:"
                ));
            
            message.Text = "calle";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "La calle de la oferta es: "+"calle"+"\n \n"+"Ahora ingrese el nombre del material que quiere ofertar:"
                ));

            message.Text = "NombreMaterial";
            handler.Handle(msj, out response);
            string opciones = "";
            int i =0;
            foreach (Clasificacion clasificacion in db.Clasificaciones)
            {
                opciones = opciones + i.ToString() + " - " + clasificacion.Nombre +"\n";
                i++;
            }
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "El nombre del material es: "+ "NombreMaterial" +"\n \n" +"Ahora seleccione la clasificación del mismo. (Ingresando el numero correspondiente)\n"+opciones
                ));

            message.Text = "0";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Ahora indique la cantidad de material: \n (A continuación le pediremos la unidad)"
                ));

            message.Text="45";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Ahora ingrese la unidad de medida correspondiente:"
                ));

            message.Text="kg";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "kg"+" Es la unidad seleccionada, ahora ingrese el valor ($U) que desea darle a la oferta:"
                ));

            message.Text = "40";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Como usted selecciono una Oferta Recurrente, ingrese la primera fecha de disponibilidad de la oferta \n (Debe tener la forma dd/mm/aaaa)"
                ));

            message.Text="30/11/2021";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Fecha inicial: 30/11/2021, ahora ingrese la recurrencia semanal (Cada cúantas semanas vuelve a estar disponible la oferta.):"
                ));

            message.Text = "1";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Se a creado la oferta NombreOferta a nombre de la empresa EmpresaTest.\n Características:\nNombreMaterial\n45 kg\n Recurrencia: cada 1 semanas."+"\nRecuerde que si desea agreagr una habilitacion a la oferta lo puede hacer con /AddHabilitacion.\nTambien puede agregar mas palabras claves a la oferta con el comando /AddPalabraClave."
                ));
        }
    }
}