//--------------------------------------------------------------------------------
// <copyright file="TestRegistroHandler.cs" company="Universidad Católica del Uruguay">
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
    /// Esta clase prueba el handler Registro.
    /// </summary>
    public class RegistroHandlerTests
    {
        Registro handler;
        Message message;

        TelegramMSGadapter msj;
        Contenedor db;
        Rubro rubroTest;


        /// <summary>
        ///Crea una instancia de contenedor, el handler a probar, un rubro, el message asi como asignarle una ID. Y ademas
        /// crea una instancia de TelegramMSG adapter.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Rubro rubroTest = new Rubro("Prueba","Prueba","Prueba");
            db = Contenedor.Instancia;
            db.AddRubro(rubroTest);
            handler = new Registro(null);
            message = new Message();
            message.From = new User();
            message.From.Id = 1454175798;
            msj = new TelegramMSGadapter(message);
        }

        /// <summary>
        /// Este test prueba como se procesan los mensajes para el registro de un emprendedor (usuario con una ID
        /// no invitada).
        /// </summary>
        [Test]
        public void TestRegistroEmprendedorHandle() //Registro de un usuario no invitado
        {
            message.Text = handler.Keywords[0];
            string response;

            IHandler result = handler.Handle(msj, out response);

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Se procedera con su registro como emprendedor."+"\n"+"\n"+"Ingrese su nombre:"
                ));
            
            message.Text = "NombreEmprendedor";
            handler.Handle(msj, out response);
            string opciones = "";
            int i =0;
            foreach (Rubro rubro in db.Rubros)
            {
                opciones = opciones + i.ToString() + " - " + rubro.Nombre +"\n";
                i++;
            }
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su nombre es: "+"NombreEmprendedor"+"\n"+"\n"+"Seleccione su rubro:\n" + opciones
                ));
            
            message.Text = "rubro";     //El usuario no ingresa un número
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "No se ha ingresado un número, ingrese un numero válido."
                ));

            message.Text = "80";    //El usuario se equivoca en el numero
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Usted ha ingresado un número incorrecto, por favor vuelva a intentarlo"
                ));
            
            message.Text = "0";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su rubro es: "+ db.Rubros[0].Nombre +"\n"+"\n"+"Ahora ingrese su ciudad:"
                ));

            message.Text = "montevideo";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su ciudad es: "+"montevideo"+"\n"+"\n"+"Ahora ingrese su calle:"
                ));
            
            message.Text = "calle 8";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su calle es: " + "calle 8" + "\n"+"\n"+"Ahora ingrese su especialización:"
                ));

            message.Text="madera";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Emprendedor NombreEmprendedor del rubro Prueba ha sido registrado correctamente!"+"\n"+$"Su domicilio a sido fijado a montevideo, calle 8" + "\n" + "\n" + "Recuerda que si desea agregar una habilitacion, debera utilizar el comando /AddHabilitacion"
                ));
        }

        /// <summary>
        /// Este test prueba como se procesan los mensajes para realizar el registro de una Empresa (usuario con una ID
        /// invitada).
        /// </summary>
        [Test]
        public void TestRegistroEmpresaHandle()
        {
            db.Invitados.Add("1454175798"); //Previa invitacion del administrador
            message.Text = handler.Keywords[0];
            string response;

            IHandler result = handler.Handle(msj, out response);

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su ID se encuentra en la lista de invitados para registrarse como Empresa"+"\n"+"Ingrese el nombre de la empresa:"
                ));
            
            message.Text = "NombreEmpresa";
            handler.Handle(msj, out response);
            string opciones = "";
            int i =0;
            foreach (Rubro rubro in db.Rubros)
            {
                opciones = opciones + i.ToString() + " - " + rubro.Nombre +"\n";
                i++;
            }
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su nombre es: "+"NombreEmpresa"+"\n"+"\n"+"Seleccione su rubro:\n" + opciones
                ));
            
            message.Text = "0";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su rubro es: "+ db.Rubros[0].Nombre +"\n"+"\n"+"Ahora ingrese su ciudad:"
                ));

            message.Text = "montevideo";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su ciudad es: "+"montevideo"+"\n"+"\n"+"Ahora ingrese su calle:"
                ));
            
            message.Text = "calle 8";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Su calle es: calle 8\n"+"\n"+"Ahora ingrese un teléfono de contacto:"
                ));
            
            message.Text = "+598 99 777 777";
            handler.Handle(msj, out response);
            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "NombreEmpresa has sido registrado correctamente!"+"\n"+$"su domicilio a sido fijado a calle 8, montevideo\nTEL: +598 99 777 777\n\nRecuerde que como empresa puede utilizar /PublicarOferta."
                ));
        }

        /// <summary>
        /// Este test representa lo que sucede cuando el usuario intenta registrarse otra vez.
        /// </summary>
        [Test]

        public void TestRegistroUsuarioRegistrado()
        {
            message.Text=handler.Keywords[0];
            db.Invitados.Remove("1454175798");
            string response;

            IHandler result = handler.Handle(msj, out response);

            Assert.That(result, Is.Not.Null);
            Assert.That(response, Is.EqualTo(
                "Usted ya se encuentra registrado."
                ));
        }

    }
}
