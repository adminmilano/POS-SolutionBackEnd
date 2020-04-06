using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Milano.BackEnd.Dto;
using Milano.BackEnd.Dto.Sales;
using Milano.BackEnd.Repository;
using Milano.BackEnd.Repository.Sincronizacion;
using Milano.BackEnd.Dto.Sincronizacion;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Milano.BackEnd.Business.Sincronizacion
{

    /// <summary>
    /// Clase de negocio para el Motor de Sincronización
    /// </summary>
    public class SincronizacionBusiness : BaseBusiness
    {

        SincronizacionRepository repository;

        /// <summary>
        /// Constructor
        /// </summary>
        public SincronizacionBusiness()
        {
            repository = new SincronizacionRepository();
        }

        /// <summary>
        /// Ejecutar proceso de Sincronización
        /// </summary>
        private ResponseBussiness<int> SincronizarDatos()
        {
            return tryCatch.SafeExecutor(() =>
            {
                return this.repository.SincronizarDatos();
            });
        }

        /// <summary>
        /// Ejecutar proceso de Purga
        /// </summary>
        public ResponseBussiness<int> PurgarDatos()
        {
            return tryCatch.SafeExecutor(() =>
            {
                return this.repository.PurgarDatos();
            });
        }

        /// <summary>
        /// Obtener periodicidad proceso de Sincronización
        /// </summary>
        public ResponseBussiness<Periodicidad> ObtenerPeriodicidad()
        {
            return tryCatch.SafeExecutor(() =>
            {
                return this.repository.ObtenerPeriodicidad();
            });
        }

        /// <summary>
        /// Procesar una petición de sincronización
        /// </summary>
        public ResponseBussiness<ResultadoSincronizacion> ProcesarPeticionSincronizacion(SincronizacionRequest sincronizacionRequest)
        {
            ResultadoSincronizacion resultadoSincronizacion = new ResultadoSincronizacion();
            resultadoSincronizacion = this.repository.EjecutarSincronizacion(sincronizacionRequest);
            return resultadoSincronizacion;
        }

        /// <summary>
        /// Ejecutar una petición de sincronización
        /// </summary>
        public ResponseBussiness<int> SincronizarInformacion()
        {
            return tryCatch.SafeExecutor(() =>
            {
                InformacionSincronizador informacionSincronizador = repository.ObtenerInformacionDiscreparSincronizador();
                if ((informacionSincronizador.CodigoCajaOrigen == 0) && (informacionSincronizador.IdDestinoSiguienteProcesar == 0))
                {
                    // Invoca a sincronizador especial para BOOFICINAS
                    return EnviarPeticionSincronizacion();
                }
                else
                {
                    // Invoca a sincronizador primera version (el de siempre)
                    return SincronizarDatos();
                }
            });
        }

        /// <summary>
        /// Ejecutar una petición de sincronización
        /// </summary>
        private ResponseBussiness<int> EnviarPeticionSincronizacion()
        {
            return tryCatch.SafeExecutor(() =>
            {
                // Leer la información que debe sincronizarse
                ResultadoSincronizacion resultadoSincronizacion = new ResultadoSincronizacion();
                SincronizacionRequest sincronizacionRequest = repository.ObtenerDatosSincronizacion();
                String webServicePath = sincronizacionRequest.ServidorDestino + "/PosServiciosMIlano/Sincronizacion/Sincronizacionservice.svc/ejecutarProcesoSincronizacion";

                if (!String.IsNullOrEmpty(sincronizacionRequest.ServidorDestino))
                {
                    // ******************************* WS REMOTO
                    var webRequest = (HttpWebRequest)WebRequest.Create(webServicePath);
                    webRequest.Method = WebRequestMethods.Http.Post;
                    // Timeout de 30 minutos
                    webRequest.Timeout = 1800000;
                    webRequest.ContentType = "application/json";
                    var json = JsonConvert.SerializeObject(sincronizacionRequest);
                    try
                    {
                        // Hacer la petición al WS Remoto
                        using (var requestStream = webRequest.GetRequestStream())
                        {
                            using (var writer = new StreamWriter(requestStream))
                            {
                                writer.Write(json);
                            }
                        }

                        // Obtener la respuesta del WS Remoto
                        using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                        {
                            using (var responseStream = webResponse.GetResponseStream())
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    var responseData = reader.ReadToEnd();
                                    dynamic resultadoSincronizacionWS = JsonConvert.DeserializeObject(responseData);
                                    resultadoSincronizacion.UltimoIdSincronizado = resultadoSincronizacionWS.data.ultimoIdSincronizado;
                                    resultadoSincronizacion.MensajeAsociado = resultadoSincronizacionWS.data.mensajeAsociado;
                                    resultadoSincronizacion.IdServidorDestino = resultadoSincronizacionWS.data.idServidorDestino;
                                    resultadoSincronizacion.ServidorDestino = resultadoSincronizacionWS.data.servidorDestino;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        TryCatchBusinessExecutor tryCatch = new TryCatchBusinessExecutor();
                        tryCatch.AddErrorLog<OperationResponse>(exception.Message, exception.StackTrace, "Sincronización", exception.ToString(), "Error de sincronización");
                    }
                    // ******************************* WS REMOTO       

                    // Actualizar la información de tabla AUDITORIA_DESTINOS
                    repository.ActualizarAuditoriaDestinos(resultadoSincronizacion);
                }

                return 0;
            });
        }

    }
}
