using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Milano.BackEnd.Dto;
using Milano.BackEnd.Repository;
using Milano.BackEnd.Repository.General;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using Milano.BackEnd.Dto.General;
using Milano.BackEnd.Business.LogMonitor;

namespace Milano.BackEnd.Business
{
    /// <summary>
    /// Admnistracion de tarjetas de regalo
    /// </summary>
    public class TarjetaBancariaBusiness : BaseBusiness
    {
        ProxyTarjetasRegalo.wsTarjetasRegaloSoapClient proxy;
        InformacionServiciosExternosRepository externosRepository;
        TarjetasRegaloRepository repository;
        InfoService inforService;
        TokenDto token;
        addEvent s = new addEvent();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        public TarjetaBancariaBusiness(TokenDto token)
        {
            inforService = new InfoService();
            externosRepository = new InformacionServiciosExternosRepository();
            inforService = externosRepository.ObtenerInfoServicioExterno(16);
            proxy = new ProxyTarjetasRegalo.wsTarjetasRegaloSoapClient();
            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(inforService.UrlService);
            repository = new TarjetasRegaloRepository();
            this.token = token;
        }

        /// <summary>
        /// Cobro con tarjeta bancaria
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <param name="numeroSesion">Número de sesión</param>
        /// <param name="folio">Folio de operación</param>
        /// <param name="mesesFinanciamiento">Número de meses que se tiene antes de hacer el primer pago</param>
        /// <param name="mesesParcialidades">Número de meses parciales del pago</param>
        /// <param name="codigoPromocion">Código de promoción bancario</param>
        /// <param name="montoPagado">Monto a pagar</param>
        /// <returns></returns>
        public PagoBancarioResponse CobroVisaMasterCard(string url, int numeroSesion, string folio, int mesesFinanciamiento, int mesesParcialidades, int codigoPromocion, decimal montoPagado, int secuencia)
        {
            s.LogEntry("  Entrando a PagoBancarioResponse CobroVisaMasterCard", 2);

            PagoBancarioResponse response = new PagoBancarioResponse();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/payvisamaster");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));

            s.LogEntry("  Entrando a PagoBancarioResponse Paso el request", 3);
            s.LogEntry($"    httpWebRequest.ToString:{  httpWebRequest.ToString() }", 3);

          

            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio,
                            transactionAmount = montoPagado,
                            financialMonths = mesesFinanciamiento,
                            paymentsPartial = mesesParcialidades,
                            promotion = codigoPromocion,
                            secuenciaPos = secuencia
                        });
                }
            }
            s.LogEntry("  Salio del serializer", 3);
            try
            {
                s.LogEntry(" --Pagina: " + httpWebRequest.Address.ToString(), 3);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                s.LogEntry(" Genero instancia de httpResponse ", 3);

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    s.LogEntry("   response.CodeNumber: " + response.CodeNumber, 3);
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.Authorization = Convert.ToString(token.SelectToken("authorization"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                    response.SePuedeRetirar = Convert.ToBoolean(token.SelectToken("isCashBack"));
                    response.SePuedePagarConPuntos = Convert.ToBoolean(token.SelectToken("isSaleWithPoints"));
                }
            }
            catch (WebException webException)
            {
                s.LogEntry("Fallo el TRY: " + webException.Message, 2);
                s.LogEntry("Fallo el TRY2: " +webException.InnerException.Message, 2);

                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
		/// Cobro con tarjeta bancaria
		/// </summary>
        /// <param name="url">URL del servicio de cobro</param>
		/// <param name="numeroSesion">Número de sesión</param>
        /// <param name="folio">Folio de operación</param>
		/// <param name="mesesFinanciamiento">Número de meses que se tiene antes de hacer el primer pago</param>
		/// <param name="mesesParcialidades">Número de meses parciales del pago</param>
		/// <param name="codigoPromocion">Código de promoción bancario</param>
		/// <param name="montoPagado">Monto a pagar</param>
		/// <returns></returns>
		public PagoBancarioResponse CobroAmericanExpress(string url, int numeroSesion, string folio, int mesesFinanciamiento, int mesesParcialidades, int codigoPromocion, decimal montoPagado, int secuencia)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/payamex");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio,
                            transactionAmount = montoPagado,
                            financialMonths = mesesFinanciamiento,
                            paymentsPartial = mesesParcialidades,
                            promotion = codigoPromocion,
                            secuenciaPos = secuencia
                        });
                }
            }
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.Authorization = Convert.ToString(token.SelectToken("authorization"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                    response.SePuedeRetirar = Convert.ToBoolean(token.SelectToken("isCashBack"));
                    response.SePuedePagarConPuntos = Convert.ToBoolean(token.SelectToken("isSaleWithPoints"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Cobro con tarjeta bancaria y retiro en efectivo
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <param name="numeroSesion">Número de sesión</param>
        /// <param name="folio">Folio de operación</param>
        /// <param name="mesesFinanciamiento">Número de meses que se tiene antes de hacer el primer pago</param>
        /// <param name="mesesParcialidades">Número de meses parciales del pago</param>
        /// <param name="codigoPromocion">Código de promoción bancario</param>
        /// <param name="montoPagado">Monto a pagar</param>
        /// <param name="montoCashBack">Monto a retirar</param>
        /// <returns></returns>
        public PagoBancarioResponse CobroConCashBack(string url, int numeroSesion, string folio, int mesesFinanciamiento, int mesesParcialidades, int codigoPromocion, decimal montoPagado, decimal montoCashBack, int secuencia)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/paywithcashback");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio,
                            transactionAmount = montoPagado,
                            cashBackAmount = montoCashBack,
                            financialMonths = mesesFinanciamiento,
                            paymentsPartial = mesesParcialidades,
                            promotion = codigoPromocion,
                            secuenciaPos = secuencia
                        });
                }
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.Authorization = Convert.ToString(token.SelectToken("authorization"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Cobro con puntos bancarios
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <param name="numeroSesion">Número de la sesión</param>
        /// <param name="folio">Folio de operación</param>
        /// <param name="montoPagado">Monto a pagar</param>
        /// <param name="pagarConPuntos">Indica si se desea pagar con puntos</param>
        /// <returns></returns>
        public PagoBancarioResponse CobroConPuntos(string url, int numeroSesion, string folio, decimal montoPagado, bool pagarConPuntos, int secuencia)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/paywithpoints");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio,
                            transactionAmount = montoPagado,
                            payWithPoints = pagarConPuntos,
                            secuenciaPos = secuencia
                        });
                }
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.Authorization = Convert.ToString(token.SelectToken("authorization"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Cobro con tarjeta bancaria y retiro en efectivo
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <param name="numeroSesion">Número de sesión</param>
        /// <param name="folio">Folio de operación</param>
        /// <param name="montoCashBack">Monto a retirar</param>
        /// <returns></returns>
        public PagoBancarioResponse CashBackAdvance(string url, int numeroSesion, string folio, decimal montoCashBack, int secuencia)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/cashback");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));
            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio,
                            cashBackAmount = montoCashBack,
                            secuenciaPos = secuencia
                        });
                }
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.Authorization = Convert.ToString(token.SelectToken("authorization"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Cancela la operación de cobro
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <returns></returns>
        public PagoBancarioResponse Cancelar(string url)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/cancelpayment");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Ejecuta una carga de Lalaves
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>                
        /// <returns>Resultado de la operación</returns>
        public OperationResponse CargarLlaves(string url)
        {
            OperationResponse response = new OperationResponse();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/loadkey");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToString(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToString(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }

        /// <summary>
        /// Obtiene el número de tarjeta mediante la pin pad
        /// </summary>
        /// <param name="url">URL del servicio de cobro</param>
        /// <param name="numeroSesion">Número de sesión</param>
        /// <param name="folio">Folio de operación</param>
        /// <returns></returns>
        public PagoBancarioResponse ObtenerTarjeta(string url, int numeroSesion, string folio)
        {
            PagoBancarioResponse response = new PagoBancarioResponse();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/paymentpinpad/getcard");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("tiempoEsperaWebApi"));

            var serializer = new Newtonsoft.Json.JsonSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                using (var textWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(textWriter,
                        new
                        {
                            sessionNumber = numeroSesion,
                            commerceReference = folio
                        });
                }
            }


            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                    response.CardNumber = Convert.ToString(token.SelectToken("cardNumber"));
                    response.TipoTarjeta = Convert.ToString(token.SelectToken("tipoTarjeta"));
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    JToken token = JObject.Parse(responseText);
                    response.CodeNumber = Convert.ToInt32(token.SelectToken("code"));
                    response.CodeDescription = Convert.ToString(token.SelectToken("message"));
                }
            }

            return response;
        }
    }
}