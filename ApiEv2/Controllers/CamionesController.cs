using ApiEv2.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiEv2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class CamionesController : Controller
    {


        //USUARIOS


        //INSERTAR USUARIO

        [HttpPost]
        [Route("InsertarUsuario")]
        public IActionResult InsertarUsuarios([FromBody] Usuario usu)

        {
           
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    //VERIFICAR POR NOMBRE DE USUARIO QUE EXISTE
                    var contador = bd.Usuarios.Where(x
                    => x.Usuario1 == usu.Usuario1).Count();


                    //INSERTAR
                    if (contador == 0)
                    {
                        Usuario insertar = new Usuario();
                        insertar.Usuario1 = usu.Usuario1;
                        insertar.NomPersona = usu.NomPersona;
                        insertar.Password = usu.Password;
                        insertar.IdEstado = 1;
                        bd.Usuarios.Add(insertar);
                        bd.SaveChanges();




                        return StatusCode(StatusCodes.Status200OK,
                                  new
                                  {
                                      mensaje = "OK",
                                      respuesta = "Usuario registrado correctamente"
                                  });
                    }

                    else
                    {
                        
                        return StatusCode(StatusCodes.Status500InternalServerError,
                                 new
                                 {
                                     mensaje = "ERROR",
                                     respuesta = "Usuario ya se encuentra en el sistema"
                                 });
                    }
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }



        //LISTADO USUARIOS

        [HttpGet]
        [Route("ListadoUsuario")]
        public IActionResult ListadoUsuarios()
        {
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    var sql = (from u in bd.Usuarios
                               select new
                               {
                                   u.Usuario1,
                                   u.NomPersona,
                                   u.IdEstado
                               }).ToList();


                    return StatusCode(StatusCodes.Status200OK,
                                        new
                                        {
                                            mensaje = "OK",
                                            respuesta = sql
                                        });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }



        //EDITAR USUARIO
        [HttpPost]
        [Route("EditarDatosUsuario")]
        public IActionResult EditarDatosUsuarios([FromBody] Usuario usu)
        {
            
            try
            {
                if (usu.IdEstado != 0 && usu.IdEstado != 1 && usu.IdEstado != 2)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 1 o 2"
                        });
                }

                using (var bd = new EvaluacionWeb2Context())
                {
                    //Esta variable si o si tiene un dato
                    string nombreusuario = usu.Usuario1;

                    var contar = bd.Usuarios.Where(x => x.Usuario1 == nombreusuario);
                    if (contar.Count() == 0)//no encuentra registro
                    {
                        
                        return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "Usuario no encontrado"
                            });
                    }
                    else
                    {
                        var editar = bd.Usuarios.First(x => x.Usuario1 == nombreusuario);

                        if (!string.IsNullOrWhiteSpace(usu.Password))
                        {
                            editar.Password = usu.Password;
                        }
                        if (usu.IdEstado == 1 || usu.IdEstado == 2)
                        {
                            editar.IdEstado = usu.IdEstado;
                        }

                        bd.SaveChanges();

                        

                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Datos de usuario editados correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }




        // INICIAR SESIÓN
        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult InicioSesion([FromBody] IniciarSesion datos)
        {
            try
            {
                //COMPROBAR DATOS ENVIADOS
                if (string.IsNullOrWhiteSpace(datos.Usuario1) ||
                    string.IsNullOrWhiteSpace(datos.Password))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Debe ingresar usuario y contraseña"
                        });
                }

                //BUSCAR USUARIO
                Usuario? usuarioEncontrado;

                using (var bd = new EvaluacionWeb2Context())
                {
                    usuarioEncontrado = bd.Usuarios.FirstOrDefault(
                        x => x.Usuario1 == datos.Usuario1);
                }

                //VERIFICAR QUE EXISTA
                if (usuarioEncontrado == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Usuario no se encuentra en el sistema"
                        });
                }

                //COMPROBAR CONTRASEÑA
                if (usuarioEncontrado.Password != datos.Password)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Contraseña incorrecta."
                        });
                }

                //COMPROBAR HABILITADO
                if (usuarioEncontrado.IdEstado == 2)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El usuario se encuentra DESHABILITADO"
                        });
                }
                if (usuarioEncontrado.IdEstado != 1)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El usuario tiene un estado no válido"
                        });
                }

                //DEVOLVER DATOS DE USUARIO
                return StatusCode(StatusCodes.Status200OK,
                    new
                    {
                        mensaje = "OK",
                        respuesta = "Inicio de sesión correcto",
                        usuario = new
                        {
                            usuarioEncontrado.Usuario1,
                            usuarioEncontrado.NomPersona,
                            usuarioEncontrado.IdEstado
                        }
                    });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = "Ocurrió un error al iniciar sesión"
                    });
            }
        }





















        //CONDUCTOR


        //INSERTAR CONDUCTOR

        [HttpPost]
        [Route("InsertarConductor")]
        public IActionResult InsertarConductores([FromBody] Chofer condu)

        {

            try
            {
                //VALIDAR QUE EL ESTADO SEA 1 O 2
                if (condu.IdEstado < 1 || condu.IdEstado > 2)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 1 o 2"
                        });
                }

                using (var bd = new EvaluacionWeb2Context())
                {
                    //VERIFICAR POR ID DE CHOFER QUE EXISTE
                    var contador = bd.Chofers.Where(x
                    => x.NomChofer == condu.NomChofer).Count();


                    //INSERTAR
                    if (contador == 0)
                    {
                        Chofer insertar = new Chofer();
                        insertar.NomChofer = condu.NomChofer;
                        insertar.IdEstado = condu.IdEstado;

                        bd.Chofers.Add(insertar);
                        bd.SaveChanges();




                        return StatusCode(StatusCodes.Status200OK,
                                  new
                                  {
                                      mensaje = "OK",
                                      respuesta = "Conductor registrado correctamente"
                                  });
                    }

                    else
                    {

                        return StatusCode(StatusCodes.Status500InternalServerError,
                                 new
                                 {
                                     mensaje = "ERROR",
                                     respuesta = "Conductor ya se encuentra en el sistema"
                                 });
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }







        //EDITAR CONDUCTOR
        [HttpPost]
        [Route("EditarDatosConductor")]
        public IActionResult EditarDatosConductores([FromBody] Chofer condu)
        {
            
            try
            {
                if (condu.IdEstado != 0 && condu.IdEstado != 1 && condu.IdEstado != 2)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 1 o 2"
                        });
                }

                using (var bd = new EvaluacionWeb2Context())
                {
                    //Esta variable si o si tiene un dato
                    int idconductor = condu.IdChofer;

                    var contar = bd.Chofers.Where(x => x.IdChofer == idconductor);
                    if (contar.Count() == 0)
                    {
                        
                        return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "Conductor no encontrado"
                            });
                    }
                    else
                    {
                        var editar = bd.Chofers.First(x => x.IdChofer == idconductor);

                        if (!string.IsNullOrWhiteSpace(condu.NomChofer))
                        {
                            editar.NomChofer = condu.NomChofer;
                        }
                        if (condu.IdEstado == 1 || condu.IdEstado == 2)
                        {
                            editar.IdEstado = condu.IdEstado;
                        }

                        bd.SaveChanges();

                        

                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Datos de conductor editados correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }




        //LISTADO CONDUCTORES

        [HttpGet]
        [Route("ListadoConductor")]
        public IActionResult ListadoConductores()
        {
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    var sql = (from u in bd.Chofers
                               select new
                               {
                                   u.IdChofer,
                                   u.NomChofer,
                                   Estado = u.IdEstadoNavigation!.NomEstado
                               }).ToList();


                    return StatusCode(StatusCodes.Status200OK,
                                        new
                                        {
                                            mensaje = "OK",
                                            respuesta = sql
                                        });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }







        //CAMION

        //INSERTAR CAMION

        [HttpPost]
        [Route("InsertarCamion")]
        public IActionResult InsertarCamiones([FromBody] Camion cami)

        {

            try
            {
                //VALIDAR QUE EL ESTADO SEA 1 O 2
                if (cami.IdEstado < 1 || cami.IdEstado > 2)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 1 o 2"
                        });
                }


                using (var bd = new EvaluacionWeb2Context())
                {

                    //VERIFICAR QUE LA PLANTA EXISTA
                    var planta = bd.Planta.Where(x => x.IdPlanta == cami.IdPlanta).Count();

                    if (planta == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "La planta no existe"
                            });
                    }


                    //VERIFICAR QUE EL ID NO SE REPITA
                    var contador = bd.Camions.Where(x
                    => x.CodCamion == cami.CodCamion).Count();


                    //INSERTAR
                    if (contador == 0)
                    {
                        Camion insertar = new Camion();
                        insertar.CodCamion = cami.CodCamion;
                        insertar.IdPlanta = cami.IdPlanta;
                        insertar.IdEstado = cami.IdEstado;

                        bd.Camions.Add(insertar);
                        bd.SaveChanges();




                        return StatusCode(StatusCodes.Status200OK,
                                  new
                                  {
                                      mensaje = "OK",
                                      respuesta = "Camion registrado correctamente"
                                  });
                    }

                    else
                    {

                        return StatusCode(StatusCodes.Status500InternalServerError,
                                 new
                                 {
                                     mensaje = "ERROR",
                                     respuesta = "Camion ya se encuentra en el sistema"
                                 });
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }





        //EDITAR CAMION
        [HttpPost]
        [Route("EditarDatosCamion")]
        public IActionResult EditarDatosCamiones([FromBody] Camion cami)
        {

            try
            {

                if (cami.IdEstado != 1 && cami.IdEstado != 2)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 1 o 2"
                        });
                }


                using (var bd = new EvaluacionWeb2Context())
                {
                    
                    string idcamion = cami.CodCamion;

                    var contar = bd.Camions.Where(x => x.CodCamion == idcamion);
                    if (contar.Count() == 0)
                    {

                        return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "Camion no encontrado"
                            });
                    }
                    else
                    {
                        var editar = bd.Camions.First(x => x.CodCamion == idcamion);

                        
                        if (cami.IdEstado == 1 || cami.IdEstado == 2)
                        {
                            editar.IdEstado = cami.IdEstado;
                        }

                        bd.SaveChanges();



                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Datos de camion editados correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }





        //LISTADO CONDUCTORES

        [HttpGet]
        [Route("ListadoCamion")]
        public IActionResult ListadoCamiones()
        {
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    var sql = (from u in bd.Camions
                               select new
                               {
                                   u.CodCamion,
                                   Planta = u.IdPlantaNavigation!.NomPlanta,
                                   Estado = u.IdEstadoNavigation!.NomEstado
                               }).ToList();


                    return StatusCode(StatusCodes.Status200OK,
                                        new
                                        {
                                            mensaje = "OK",
                                            respuesta = sql
                                        });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }















        //SALIDAS

        //INSERTAR SALIDA

        [HttpPost]
        [Route("InsertarSalida")]
        public IActionResult InsertarSalidas([FromBody] Salidum sali)
        {
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    //VERIFICAR QUE LA PLANTA EXISTA
                    var planta = bd.Planta.Where(x => x.IdPlanta == sali.IdPlanta).Count();
                    if (planta == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new { mensaje = "ERROR", respuesta = "La planta no existe" });
                    }

                    //VERIFICAR QUE EL CAMION EXISTA
                    var camion = bd.Camions.Where(x => x.CodCamion == sali.CodCamion).Count();
                    if (camion == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new { mensaje = "ERROR", respuesta = "El camión no existe" });
                    }

                    //VERIFICAR QUE EL CHOFER EXISTA
                    var chofer = bd.Chofers.Where(x => x.IdChofer == sali.IdChofer).Count();
                    if (chofer == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new { mensaje = "ERROR", respuesta = "El conductor no existe" });
                    }

                    //VERIFICAR QUE EL USUARIO EXISTA
                    var usuario = bd.Usuarios.Where(x => x.Usuario1 == sali.Usuario).Count();
                    if (usuario == 0)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest,
                            new { mensaje = "ERROR", respuesta = "El usuario no existe" });
                    }

                    //INSERTAR
                    Salidum insertar = new Salidum();
                    insertar.FechaSalida = sali.FechaSalida;
                    insertar.IdPlanta = sali.IdPlanta;
                    insertar.CodCamion = sali.CodCamion;
                    insertar.IdChofer = sali.IdChofer;
                    insertar.IdEstadoSalida = 1;        
                    insertar.Usuario = sali.Usuario;    
                                                        

                    bd.Salida.Add(insertar);
                    bd.SaveChanges();

                    return StatusCode(StatusCodes.Status200OK,
                        new { mensaje = "OK", respuesta = "Salida registrada correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }





        //LISTADO SALIDAS

        [HttpGet]
        [Route("ListadoSalida")]
        public IActionResult ListadoSalidas()
        {
            try
            {
                using (var bd = new EvaluacionWeb2Context())
                {
                    var sql = (from u in bd.Salida
                               select new
                               {
                                   u.IdSalida,
                                   u.FechaSalida,
                                   Planta = u.IdPlantaNavigation!.NomPlanta,
                                   Camion = u.CodCamion,
                                   Chofer = u.IdChoferNavigation!.NomChofer,
                                   EstadoSalida = u.IdEstadoSalidaNavigation!.NomEstadoSalida,
                                   Usuario = u.UsuarioNavigation!.NomPersona
                               }).ToList();


                    return StatusCode(StatusCodes.Status200OK,
                                        new
                                        {
                                            mensaje = "OK",
                                            respuesta = sql
                                        });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }



        //EDITAR SALIDA
        [HttpPost]
        [Route("EditarDatosSalida")]
        public IActionResult EditarDatosSalidas([FromBody] Salidum sali)
        {

            try
            {

                if (sali.IdEstadoSalida != 2 && sali.IdEstadoSalida != 3)
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El estado debe ser 2 RETORNADO o 3 RECHAZADO"
                        });
                }

                using (var bd = new EvaluacionWeb2Context())
                {

                    int idsali = sali.IdSalida;

                    var contar = bd.Salida.Where(x => x.IdSalida == idsali);
                    if (contar.Count() == 0)
                    {

                        return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "salida no encontrada"
                            });
                    }
                    else
                    {
                        var editar = bd.Salida.First(x => x.IdSalida == idsali);

                        editar.IdEstadoSalida = sali.IdEstadoSalida;
                        bd.SaveChanges();



                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Datos de salida editados correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                                   new
                                   {
                                       mensaje = "ERROR",
                                       respuesta = ex.Message
                                   });
            }
        }











    }
}
