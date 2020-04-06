using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Milano.BackEnd.Dto;
using Milano.BackEnd.Business;
using System.Text;
using Milano.BackEnd.Dto.Sales;
using Milano.BackEnd.Dto.FormasPago;
using Project.Services.LogMonitor;

namespace Project.Services.Sales
{

    /// <summary>
    /// Servicio que gestiona los cobros y cancelación de cobros
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PaymentProcessingService
    {

        /// <summary>
        /// Servicio para procesar una venta con una tarjeta bancaria VISA/MASTERCARD
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaVisaMaster")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaVisaMaster(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            LogRegister s = new LogRegister();

            s.LogEntry("  Inicio ProcesarMovimientoTarjetaBancariaVisaMaster", 2);

            s.LogEntry(request.ToString(), 4);
            TokenDto token = new TokenService().Get();
            //s.LogEntry($"Venta: {request.Venta.ToString()}, Retiro:{request.Retiro.ToString()}, Puntos:{request.Puntos.ToString() }", 4);

            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaVisaMaster(request);
            
            s.LogEntry("  Termina ProcesarMovimientoTarjetaBancariaVisaMaster", 2);
            return response;
        }

        /// <summary>
        /// Servicio para procesar una venta con una tarjeta bancaria AMERICAN EXPRESS
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaAmericanExpress")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaAmericanExpress(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaAmericanExpress(request);
            return response;
        }

        /// <summary>
        /// Servicio para procesar una venta con una tarjeta bancaria y retirar efectivo
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaCashBack")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaCashBack(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaCashBack(request);
            return response;
        }

        /// <summary>
        ///Servicio para procesar una venta con una tarjeta bancaria y pago con puntos
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaConPuntos")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaConPuntos(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaConPuntos(request);
            return response;
        }

        /// <summary>
        /// Servicio para procesar un retiro en efectivo
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaCashBackAdvance")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaCashBackAdvance(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaCashBackAdvance(request);
            return response;
        }


        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/cargarLlaves")]
        public ResponseBussiness<OperationResponse> CargarLlaves()
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).CargarLlaves();
            return response;
        }

        /// <summary>
        /// Servicio para cancelar la operación bancaria de cobro
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaBancariaCancelar")]
        public ResponseBussiness<PagoBancarioResponse> ProcesarMovimientoTarjetaBancariaCancelar(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaBancariaCancelar(request);
            return response;
        }

        /// <summary>
        /// Servicio para obtener la tarjeta MM
        /// </summary>
        /// <param name="request">Objeto que representa el objeto sobre el cual se procesará el movimiento</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/obtenerTarjetaBancariaMelody")]
        public ResponseBussiness<PagoBancarioResponse> ObtenerTarjetaBancariaMelody(ProcesarMovimientoTarjetaBancariaRequest request)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<PagoBancarioResponse> response = new PaymentProcessingBusiness(token).ObtenerTarjetaBancariaMelody(request);
            return response;
        }

        /// <summary>
        /// Realizar venta a empleado
        /// </summary>
        /// <param name="ventaEmpleadoRequest">Datos de la venta</param>
        /// <returns>Resultado de la venta</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoPagoVentaEmpleado")]
        public ResponseBussiness<OperationResponse> RealizarPagoVentaEmpleado(ProcesarMovimientoVentaEmpleadoRequest ventaEmpleadoRequest)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).ProcesarVentaEmpleado(ventaEmpleadoRequest);
            return response;
        }

        /// <summary>
        /// Pago con tarjeta de regalo
        /// </summary>
        /// <param name="procesarMovimientoTarjetaRegaloRequest">Dto con información del pago con tarjeta de regalo</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoTarjetaRegalo")]
        public ResponseBussiness<OperationResponse> RealizarPagoTarjetaRegalo(ProcesarMovimientoTarjetaRegaloRequest procesarMovimientoTarjetaRegaloRequest)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).ProcesarTarjetaRegalo(procesarMovimientoTarjetaRegaloRequest);
            return response;
        }

        /// <summary>
        /// Pago de forma de venta mayorista a cliente final
        /// </summary>
        /// <param name="procesarMovimientoMayorista">Dto con informacion del pago de mayorista</param>
        /// <returns>Resultado de la operación</returns>

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoMayorista")]
        public ResponseBussiness<OperationResponse> RealizarVentaoMayorista(ProcesarMovimientoMayorista procesarMovimientoMayorista)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).ProcesarVentaMayorista(procesarMovimientoMayorista);
            return response;
        }

        /// <summary>
        /// Pago con una Nota de Crédito
        /// </summary>
        /// <param name="procesarMovimientoNotaCreditoRequest">DTO con información del pago con nota de crédito</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoNotaCredito")]
        public ResponseBussiness<OperationResponse> RealizarPagoNotaCredito(ProcesarMovimientoNotaCreditoRequest procesarMovimientoNotaCreditoRequest)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).ProcesarNotaCredito(procesarMovimientoNotaCreditoRequest);
            return response;
        }

        /// <summary>
        /// Pago con un Cupón de Motor de Promociones
        /// </summary>
        /// <param name="procesarMovimientoRedencionCuponRequest">DTO con información del pago con cupon promocionalo</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoRedencionCuponPromocional")]
        public ResponseBussiness<ProcesarMovimientoRedencionCuponResponse> RealizarPagoCuponPromociones(ProcesarMovimientoRedencionCuponRequest procesarMovimientoRedencionCuponRequest)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<ProcesarMovimientoRedencionCuponResponse> response = new PaymentProcessingBusiness(token).ProcesarRedencionCuponPromocional(procesarMovimientoRedencionCuponRequest);
            return response;
        }

        //--NUEVA FORMA DE PAGO
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procesarMovimientoPagoPinPadMovilRequest">DTO</param>
        /// <returns>Resultado de la operación</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/procesarMovimientoPagoPinPadMovil")]
        public ResponseBussiness<OperationResponse> RealizarPagoPinPadMovil(ProcesarMovimientoPagoPinPadMovilRequest procesarMovimientoPagoPinPadMovilRequest)
        {
            TokenDto token = new TokenService().Get();
            ResponseBussiness<OperationResponse> response = new PaymentProcessingBusiness(token).ProcesarPagoPinPadMovil(procesarMovimientoPagoPinPadMovilRequest);
            return response;
        }
    }

}
