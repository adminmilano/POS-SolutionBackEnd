using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Milano.BackEnd.Dto;
using Milano.BackEnd.Dto.General;
using Milano.BackEnd.Repository;
using Milano.BackEnd.Dto.Sales;

namespace Milano.BackEnd.Business
{
    /// <summary>
    /// Administracion de tipos de cambio
    /// </summary>
    public class AdministracionTipoCambio : BaseBusiness
    {

        CredencialesServicioExterno credenciales;
        AdministracionTipoCambioRepository repository;
        InformacionServiciosExternosRepository externosRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdministracionTipoCambio()
        {
            credenciales = new CredencialesServicioExterno();
            credenciales = new InformacionServiciosExternosBusiness().ObtenerCredencialesCambioDivisa();
            repository = new AdministracionTipoCambioRepository();
            externosRepository = new InformacionServiciosExternosRepository();
        }

        /// <summary>
        /// Obtener el tipo de cambio
        /// </summary>
        /// <param name="tipoCambioRequest">codigo de moneda</param>
        /// <returns>valor del tipo de cambio</returns>
        public ResponseBussiness<TipoCambioResponse> ObtenerTipoCambio(TipoCambioRequest tipoCambioRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return this.GetTipoCambio(tipoCambioRequest);
            }, this.repository.ObtenerMensajeCambioDivisaNoDisponible().CodeDescription);
        }

        /// <summary>
        /// Catalogo de divisas
        /// </summary>
        /// <returns></returns>
        public ResponseBussiness<Divisa[]> ObtenerDivisas(int CodeStore)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return this.repository.ObtenerCatalogoDivisa(CodeStore);
            });
        }

        public TipoCambioResponse GetTipoCambio(TipoCambioRequest tipoCambioRequest)
        {
            TipoCambioResponse tipoCambioResponse = new TipoCambioResponse();
            CambioDivisaMilano cambio = new CambioDivisaMilano();
            InfoService inforService = externosRepository.ObtenerInfoServicioExterno(14);
            cambio = GetTipoCambioLocal(tipoCambioRequest);
            string codigoRespuesta = "000";
            decimal valorCambio = 0;
            if (tipoCambioRequest.CodigoTipoDivisa == "US")
            {
                //Inicializamos el proxy que nos ayudara a hacer la llamada al metodo de Sale
                ProxyTipoCambio.Service1SoapClient proxy = new ProxyTipoCambio.Service1SoapClient();
                proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(inforService.UrlService);                
                string xml = "<ExchangeRateRequest><credentials><username>{0}</username>"
                         + "<password>{1}</password><clientCode>{2}</clientCode></credentials>" +
                         "<exchangeInfo><currencyCode>{3}</currencyCode></exchangeInfo></ExchangeRateRequest> ";
                xml = string.Format(xml, this.credenciales.UserName, this.credenciales.Password, "", cambio.CodigoExterno);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode result = proxy.GetRate(doc);                
                codigoRespuesta = result.ChildNodes[0].ChildNodes[0].InnerText;
                if (codigoRespuesta == "000")
                {
                    if (!cambio.UsarMaximoValor)
                        valorCambio = decimal.Round(Convert.ToDecimal(result.ChildNodes[1].ChildNodes[5].ChildNodes[0].InnerText), 2);
                    else
                        valorCambio = decimal.Round(Convert.ToDecimal(result.ChildNodes[1].ChildNodes[5].ChildNodes[1].InnerText), 2);
                }
                else
                    throw new Exception(result.InnerText);
            }
            else
            {
                valorCambio = cambio.ValorCambio;
            }
            if (valorCambio > 0)
            {
                tipoCambioResponse.Mensaje = "Ok";
                tipoCambioResponse.TasaConversionVigente = valorCambio;
                tipoCambioResponse.ImporteMonedaNacional = tipoCambioRequest.ImporteMonedaNacional;
                // Ajuste para redondear a modo de cubrir el monto mínimo
                decimal multiplicador = Convert.ToDecimal(Math.Pow(10, Convert.ToDouble(2)));
                tipoCambioResponse.ImporteMonedaExtranjera = Math.Ceiling((tipoCambioRequest.ImporteMonedaNacional / valorCambio) * multiplicador) / multiplicador;
                // Ajuste para redondear a modo de cubrir el monto mínimo
            }
            return tipoCambioResponse;
        }

        private CambioDivisaMilano GetTipoCambioLocal(TipoCambioRequest tipoCambioRequest)
        {
            return repository.ObtenerTipoCambio(tipoCambioRequest.CodigoTipoDivisa);
        }

        /// <summary>
        ///     Método para obtener información del servicio externo Sale, usamos la comunicación via proxy
        /// </summary>
        /// <param name="pagoUtilizados"></param>
        /// <param name="folioVenta"></param>
        /// <param name="codigoEmpleado"></param>
        /// <returns></returns>
        //public OperationResponse GetSaleExternalService(FormaPagoUtilizado[] pagoUtilizados, string folioVenta, int codigoEmpleado)
        //{
        //    //Alcanzar el informacion servicos externos repository y alcanzar el metodo ObtenerInfoServicioExterno
        //    InformacionServiciosExternosRepository externosRepository = new InformacionServiciosExternosRepository();
        //    InfoService inforService = new InfoService();
        //    string codigoRespuesta = "000";
        //    OperationResponse operationResponse = new OperationResponse();

        //    //Por cada forma de pago utilizada ver si la forma de pago es en dolares
        //    foreach (var formaPago in pagoUtilizados)
        //    {
        //        string formaPagoString = formaPago.CodigoFormaPagoImporte.ToString();
        //        decimal cantiadMonedaExtranjera = formaPago.InformacionTipoCambio.ImporteMonedaExtranjera;
        //        decimal conversionVigente = formaPago.InformacionTipoCambio.TasaConversionVigente;
        //        string codigoCurrency = formaPago.InformacionTipoCambio.CodigoTipoDivisa;
        //        int purchaseAmountCurrency = 1;

        //        //Se agrega validacion para consumo del servicio sale que se desee llevar a cabo una venta.
        //        //Se crea una instancia del proxy para la comunicacion con el servicio.
        //        if (formaPagoString == "US")
        //        {
        //            inforService = externosRepository.ObtenerInfoServicioExterno(14);
        //            ProxyTipoCambio.Service1SoapClient proxy = new ProxyTipoCambio.Service1SoapClient();
        //            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(inforService.UrlService);        
        //            //Se crea el xml conforme lo indicado en la documentacion
        //            //23/11/2018 De momento esta mandando un error al no poder encontrar cliente
        //            string xml = "<SaleRequest>" +
        //            "               <credentials>" +
        //            "                   <username>{0}</username>" +
        //            "                   <password>{1}</password>" +
        //            "                   <clientCode>{2}</clientCode>" +
        //            "               </credentials>" +
        //            "               <transaction>" +
        //            "                   <storeTeller>{3}</storeTeller>" +
        //            "                   <currencyCode>{4}</currencyCode>" +
        //            "                   <receivedAmount>{5}</receivedAmount>" +
        //            "                   <purchaseAmount>{6}</purchaseAmount>" +
        //            "                   <usedExchangeRate>{7}</usedExchangeRate>" +
        //            "                   <purchaseAmountCurrency>{8}</purchaseAmountCurrency>" +
        //            "                   <idMerchantTransaction>{9}</idMerchantTransaction>" +
        //            "                   <posTimestamp>{10}</posTimestamp>" +
        //            "               </transaction>" +
        //            "           </SaleRequest> ";
        //            string fecha = DateTime.Now.ToString("yyyyMMddHHmmss") + "_000000";
        //            xml = string.Format(xml, this.credenciales.UserName, this.credenciales.Password, "3091", codigoEmpleado.ToString(), codigoCurrency, cantiadMonedaExtranjera, cantiadMonedaExtranjera, conversionVigente, purchaseAmountCurrency, folioVenta, fecha);
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(xml);
        //            XmlNode resultSale = proxy.Sale(doc);        
        //            codigoRespuesta = resultSale.SelectSingleNode("//responseCode").InnerText;//Obtenemos el codigo mediante el nombre del nodo, ya que childnode no funciono                    
        //            if (codigoRespuesta == "000")
        //            {
        //                operationResponse.CodeNumber = codigoRespuesta;
        //                operationResponse.CodeDescription = "OK";
        //            }
        //            else
        //                operationResponse.CodeNumber = codigoRespuesta;
        //            operationResponse.CodeDescription = "Error en servio de venta";
        //        }
        //    }
        //    return operationResponse;
        //}
    }
}
