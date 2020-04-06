using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Milano.BackEnd.Dto;
using System.Collections;
using Milano.BackEnd.Repository;
using Milano.BackEnd.Dto.Sales;
using System.Transactions;
using Milano.BackEnd.Business.MM;
using Milano.BackEnd.Business.ImpresionMM;
using System.Xml;
using Milano.BackEnd.Dto.General;

namespace Milano.BackEnd.Business
{
    /// <summary>
    /// Servicio para gestionar operaciones de venta
    /// </summary>
    public class SalesBusiness : BaseBusiness
    {
        /// <summary>
        /// Repositorio de ventas
        /// </summary>
        protected SalesRepository repository;

        /// <summary>
        /// Clase Business para ejecutar descuentos por mercancía dañada y picos de mercancía
        /// </summary>
        protected DescuentoMercanciaDaniadaBusiness descuentoMercanciaDaniadaBusiness;

        /// <summary>
        /// Repositorio para la obtención de promociones
        /// </summary>
        protected DescuentosPromocionesRepository descuentosPromocionesRepository;

        /// <summary>
        /// Atributo del token usuario
        /// </summary>
        protected TokenDto token;

        /// <summary>
        /// Constructor por default
        /// </summary>
        public SalesBusiness(TokenDto token)
        {
            this.repository = new SalesRepository();
            this.descuentosPromocionesRepository = new DescuentosPromocionesRepository();
            this.descuentoMercanciaDaniadaBusiness = new DescuentoMercanciaDaniadaBusiness(token);
            this.token = token;
        }

        /// <summary>
        /// Almacenamiento de una Linea Ticket
        /// </summary>
        /// <param name="lineaTicket">Objeto de peticion linea ticket de la venta</param>
        /// <returns></returns>
        public ResponseBussiness<OperacionLineaTicketVentaResponse> AgregarLineaTicketVenta(LineaTicket lineaTicket)
        {
            return tryCatch.SafeExecutor(() =>
            {
                OperacionLineaTicketVentaResponse response = repository.AgregarLineaTicketVenta(token.CodeStore, token.CodeBox, token.CodeEmployee, lineaTicket);
                return response;
            });
        }

        /// <summary>
        /// Cambio de Piezas de una Linea Ticket
        /// </summary>
        /// <param name="lineaTicket">Objeto de peticion linea ticket de la venta</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> CambiarPiezasLineaTicketVenta(LineaTicket lineaTicket)
        {
            return tryCatch.SafeExecutor(() =>
            {
                OperationResponse response = repository.CambiarPiezasLineaTicketVenta(token.CodeStore, token.CodeBox, token.CodeEmployee, lineaTicket);
                return response;
            });
        }

        /// <summary>
        /// Cambio de Precio de una Linea Ticket
        /// </summary>
        /// <param name="cambiarPrecioRequest">Objeto de peticion linea ticket de la venta</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> CambiarPrecioLineaTicketVenta(CambiarPrecioRequest cambiarPrecioRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return repository.CambiarPrecioLineaTicketVenta(token.CodeStore, token.CodeBox, token.CodeEmployee, cambiarPrecioRequest);
            });
        }

        /// <summary>
        /// Eliminación de una Linea Ticket
        /// </summary>
        /// <param name="secuenciaOriginalLineaTicket">Secuencia original de la línea eliminada</param>  
        /// <param name="lineaTicket">Objeto de peticion linea ticket de la venta</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> EliminarLineaTicketVenta(int secuenciaOriginalLineaTicket, LineaTicket lineaTicket, int codigoRazon)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return repository.EliminarLineaTicketVenta(token.CodeStore, token.CodeBox, token.CodeEmployee, secuenciaOriginalLineaTicket, lineaTicket, codigoRazon);
            });
        }

        /// <summary>
        /// Cerrar una devolución sin pagos
        /// </summary>
        /// <param name="totalizarVentaRequest">Objeto de peticion de la venta a totalizar</param>        
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> CerrarDevolucionSinPagos(TotalizarVentaRequest totalizarVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                return imprimeTicketsMM.PrintTicket(totalizarVentaRequest.cabeceraVentaRequest.FolioOperacion, false);
            });
        }

        /// <summary>
        /// Totalización de venta
        /// </summary>
        /// <param name="totalizarVentaRequest">Objeto de peticion de la venta a totalizar</param>
        /// <returns></returns>
        public ResponseBussiness<TotalizarVentaResponse> TotalizarVenta(TotalizarVentaRequest totalizarVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                // Variables de control y respuesta
                TotalizarVentaResponse response = new TotalizarVentaResponse();
                decimal totalDescuentosAplicadosPorMotorPromociones = 0.0M;

                using (TransactionScope scope = new TransactionScope())
                {

                    // Se invoca a Totalizar la Venta/Devolución
                    response = repository.TotalizarVenta(totalizarVentaRequest, token.CodeStore, token.CodeBox, token.CodeEmployee);

                    // Se aplican promociones por motor de promociones 
                    List<DescuentoPromocionalVenta> DescuentosPromocionalesPosiblesVenta = new List<DescuentoPromocionalVenta>();
                    List<DescuentoPromocionalVenta> DescuentosPromocionalesAplicadosVenta = new List<DescuentoPromocionalVenta>();
                    List<DescuentoPromocionalVenta> DescuentosPromocionalesPosiblesLinea = new List<DescuentoPromocionalVenta>();
                    List<DescuentoPromocionalVenta> DescuentosPromocionalesAplicadosLinea = new List<DescuentoPromocionalVenta>();

                    // Eliminar promociones previas otorgados por motor de promociones
                    repository.EliminarPromocionesCab(response.FolioOperacion, token.CodeStore, token.CodeBox);
                    repository.EliminarPromocionesDet(response.FolioOperacion, token.CodeStore, token.CodeBox);

                    // Obtener las promociones vigentes
                    DescuentoPromocionalVenta[] descuentosPromocionalesEncontrados = descuentosPromocionesRepository.ObtenerPromocionesVenta(response.FolioOperacion, token.CodeStore, token.CodeBox);

                    // Agregar descuentos promocionales existentes en motor de promociones                                    
                    foreach (DescuentoPromocionalVenta descuentoPromocionalVenta in descuentosPromocionalesEncontrados)
                    {
                        // Validar si se trata de descuento promocional por linea
                        if (descuentoPromocionalVenta.Secuencia > 0)
                        {
                            if (descuentoPromocionalVenta.DescuentosPromocionalesFormaPago != null && descuentoPromocionalVenta.DescuentosPromocionalesFormaPago.Length > 0)
                            {
                                DescuentosPromocionalesPosiblesLinea.Add(descuentoPromocionalVenta);
                            }
                            else
                            {
                                DescuentosPromocionalesAplicadosLinea.Add(descuentoPromocionalVenta);
                            }
                        }
                        else
                        {
                            if (descuentoPromocionalVenta.DescuentosPromocionalesFormaPago != null && descuentoPromocionalVenta.DescuentosPromocionalesFormaPago.Length > 0)
                            {
                                DescuentosPromocionalesPosiblesVenta.Add(descuentoPromocionalVenta);
                            }
                            else
                            {
                                DescuentosPromocionalesAplicadosVenta.Add(descuentoPromocionalVenta);
                            }
                        }
                    }

                    response.DescuentosPromocionalesAplicadosVenta = DescuentosPromocionalesAplicadosVenta.ToArray();
                    response.DescuentosPromocionalesPosiblesVenta = DescuentosPromocionalesPosiblesVenta.ToArray();
                    response.DescuentosPromocionalesAplicadosLinea = DescuentosPromocionalesAplicadosLinea.ToArray();
                    response.DescuentosPromocionalesPosiblesLinea = DescuentosPromocionalesPosiblesLinea.ToArray();

                    // Se persisten las promociones aplicadas por venta                
                    foreach (var item in response.DescuentosPromocionalesAplicadosVenta)
                    {
                        OperationResponse responsePersist = new OperationResponse();
                        responsePersist = repository.PersistirPromocionesVenta(response.FolioOperacion, token.CodeStore, token.CodeBox, item.ImporteDescuento
                                                                , item.CodigoPromocionAplicado, item.DescripcionCodigoPromocionAplicado, item.PorcentajeDescuento, item.CodigoRazonDescuento, string.Empty);
                        totalDescuentosAplicadosPorMotorPromociones = totalDescuentosAplicadosPorMotorPromociones + item.ImporteDescuento;
                    }

                    // Se persisten las promociones aplicadas por linea                    
                    foreach (var item in response.DescuentosPromocionalesAplicadosLinea)
                    {
                        OperationResponse responsePersist = new OperationResponse();
                        responsePersist = repository.PersistirPromocionesLineaVenta(response.FolioOperacion, token.CodeStore, token.CodeBox, item.Secuencia, item.ImporteDescuento
                                                                , item.CodigoPromocionAplicado, item.DescripcionCodigoPromocionAplicado, item.PorcentajeDescuento, item.CodigoRazonDescuento, string.Empty);
                        totalDescuentosAplicadosPorMotorPromociones = totalDescuentosAplicadosPorMotorPromociones + item.ImporteDescuento;
                    }

                    // Se agrega ajuste necesario para DEVOLUCIONES-PROMOCIONES 
                    if (!(String.IsNullOrEmpty(totalizarVentaRequest.cabeceraVentaRequest.FolioDevolucion)))
                    {
                        // Validar si la devolución debe terminarse
                        if ((response.InformacionAsociadaDevolucion == null) &&
                        ((totalizarVentaRequest.cabeceraVentaRequest.DevolucionSaldoAFavor >= 0) &&
                        (totalizarVentaRequest.cabeceraVentaRequest.ClienteTieneSaldoPendientePagar == false)))
                        {
                            response.InformacionAsociadaDevolucion = repository.FinalizarVentaDevolucion(totalizarVentaRequest.cabeceraVentaRequest.FolioOperacion, totalizarVentaRequest.cabeceraVentaRequest.FolioDevolucion,
                            totalizarVentaRequest.cabeceraVentaRequest.FolioVentaOriginal, token.CodeBox, token.CodeStore, totalizarVentaRequest.cabeceraVentaRequest.CodigoMayorista, totalizarVentaRequest.cabeceraVentaRequest.ImporteVentaDescuentos, totalizarVentaRequest.cabeceraVentaRequest.ImporteDevolucionImpuestos,
                            totalizarVentaRequest.cabeceraVentaRequest.ImporteVentaBruto, totalizarVentaRequest.cabeceraVentaRequest.ImporteVentaImpuestos, totalizarVentaRequest.cabeceraVentaRequest.ImporteVentaNeto,
                            totalizarVentaRequest.cabeceraVentaRequest.ImporteDevolucionNeto, totalizarVentaRequest.cabeceraVentaRequest.ImporteDevolucionImpuestos, totalizarVentaRequest.cabeceraVentaRequest.DevolucionSaldoAFavor, totalizarVentaRequest.cabeceraVentaRequest.ImporteDevolucionTotal);
                        }

                        // Hacer los ajustes a las propiedades si la devolución fue cerrada
                        if (response.InformacionAsociadaDevolucion != null)
                        {
                            // No se regresan promociones
                            response.DescuentosPromocionalesAplicadosVenta = new DescuentoPromocionalVenta[0];
                            response.DescuentosPromocionalesPosiblesVenta = new DescuentoPromocionalVenta[0];
                            response.DescuentosPromocionalesAplicadosLinea = new DescuentoPromocionalVenta[0];
                            response.DescuentosPromocionalesPosiblesLinea = new DescuentoPromocionalVenta[0];
                            // No se regresan formas de pago
                            response.InformacionAsociadaFormasPago = null;
                            response.InformacionAsociadaFormasPagoMonedaExtranjera = null;
                        }
                    }

                    // Se termina el bloque de transacción
                    scope.Complete();
                }

                // Validar si debe imprimirse ticket por una devolución que no pasa a formas de pago                
                if (!(String.IsNullOrEmpty(totalizarVentaRequest.cabeceraVentaRequest.FolioDevolucion)))
                {
                    if (response.InformacionAsociadaDevolucion != null)
                    {
                        ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                        imprimeTicketsMM.PrintTicket(totalizarVentaRequest.cabeceraVentaRequest.FolioOperacion, false);
                    }
                }

                // Se regresa la respuesta final
                return response;
            });
        }

        /// <summary>
        /// Finalización de venta
        /// </summary>
        /// <param name="finalizarVentaRequest">Objeto de peticion de la venta a finalizar</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> FinalizarVenta(FinalizarVentaRequest finalizarVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return FinalizarVentaInternal(finalizarVentaRequest);
            });
        }

        private ResponseBussiness<OperationResponse> FinalizarVentaInternal(FinalizarVentaRequest finalizarVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                OperationResponse result = new OperationResponse();
                string mensajeVentaFinalizada = "Venta finalizada exitosamente. ";
                using (TransactionScope scope = new TransactionScope())
                {
                    result = repository.FinalizarVenta(token.CodeStore, token.CodeBox, token.CodeEmployee, finalizarVentaRequest, "REGULAR");

                    // Procesamos promociones que generan Cupones                    
                    CuponPromocionalVenta[] cuponesPromocionalesEncontrados = descuentosPromocionesRepository.ProcesarPromocionesCupones(finalizarVentaRequest.cabeceraVentaRequest.FolioOperacion, token.CodeStore, token.CodeBox);
                    foreach (var cupon in cuponesPromocionalesEncontrados)
                    {
                        CuponPersistirResponse cuponPersistirResponse = new CuponPersistirResponse();
                        cuponPersistirResponse = repository.PersistirCuponPromocionalGenerado(cupon);
                        mensajeVentaFinalizada += "Cupón Generado: " + cupon.MensajeCupon + " $" + cupon.ImporteDescuento + ". ";
                    }

                    // Procesamos descuentos por mercancía dañada o picos de mercancía
                    descuentoMercanciaDaniadaBusiness.ProcesarDescuentosExternosPicosMercancia(finalizarVentaRequest.FolioVenta);

                    // ******************************************* INVOCAR A MÉTODOS DE ACUERDO A CADA TIPO DE VENTA FINALIZADA

                    // TIPO DE VENTA/DEVOLUCIÓN REGULAR CONSIDERANDO TARJETAS DE REGALO
                    if (result.CodeNumber.Equals("332") && (finalizarVentaRequest.TipoCabeceraVenta == "1"))
                    {
                        OperationResponse respuesta = this.ActivarTarjetaRegalo(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            result.CodeDescription = mensajeVentaFinalizada;
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // TIPO DE VENTA PAGO TCMM
                    if (result.CodeNumber.Equals("332") && finalizarVentaRequest.TipoCabeceraVenta == "46")
                    {
                        OperationResponse respuesta = this.PagoTCMM(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            result.CodeDescription = mensajeVentaFinalizada;
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // TIPO DE VENTA MAYORISTA CONSIDERANDO TARJETAS DE REGALO
                    else if (result.CodeNumber.Equals("332") && finalizarVentaRequest.TipoCabeceraVenta == "2")
                    {
                        var respuesta = this.PagoVentaCreditoMayorista(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            // Se procesan las tarjetas de regalo en caso de aplicar
                            OperationResponse operationResponse = this.ActivarTarjetaRegalo(finalizarVentaRequest);
                            if (operationResponse.CodeNumber == "1")
                            {
                                result.CodeDescription = mensajeVentaFinalizada;
                                ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                                imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                                scope.Complete();
                            }
                            else
                            {
                                result.CodeDescription = respuesta.CodeDescription;
                            }
                        }
                        else
                        {
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // TIPO DE VENTA EMPLEADO CONSIDERANDO TARJETAS DE REGALO
                    else if (result.CodeNumber.Equals("332") && finalizarVentaRequest.TipoCabeceraVenta == "4")
                    {
                        OperationResponse respuesta = this.ActivarTarjetaRegalo(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            result.CodeDescription = mensajeVentaFinalizada;
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // TIPO DE VENTA TIEMPO AIRE
                    else if (result.CodeNumber.Equals("332") && finalizarVentaRequest.TipoCabeceraVenta == "5")
                    {
                        var respuesta = this.TiempoAire(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeNumber = respuesta.CodeNumber;
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // TIPO DE VENTA PAGO DE SERVICIOS
                    else if (result.CodeNumber.Equals("332") && finalizarVentaRequest.TipoCabeceraVenta == "6")
                    {
                        var respuesta = this.PagoServicios(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeNumber = respuesta.CodeNumber;
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                    // SE TRATA DE UNA DEVOLUCIÓN CONSIDERANDO TARJETAS DE REGALO
                    else if (result.CodeNumber.Equals("399"))
                    {
                        OperationResponse respuesta = this.ActivarTarjetaRegalo(finalizarVentaRequest);
                        if (respuesta.CodeNumber == "1")
                        {
                            result.CodeDescription = mensajeVentaFinalizada;
                            ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                            imprimeTicketsMM.PrintTicket(finalizarVentaRequest.FolioVenta, false);
                            scope.Complete();
                        }
                        else
                        {
                            result.CodeDescription = respuesta.CodeDescription;
                        }
                    }

                }
                return result;
            });
        }

        private OperationResponse PagoVentaCreditoMayorista(FinalizarVentaRequest request)
        {
            OperationResponse response = new OperationResponse();
            response.CodeNumber = "1";
            VentaResponse venta = this.repository.ObtenerVentaPorFolio(request.FolioVenta, 0);
            foreach (LineaTicket linea in venta.Lineas)
            {
                if (linea.TipoDetalleVenta == "43")
                {
                    PagoCreditoMayoristaRequest pago = new PagoCreditoMayoristaRequest();
                    pago.CodigoMayorista = venta.CodigoMayorista;
                    pago.FolioOperacionAsociada = venta.FolioVenta;
                    pago.ImportePago = linea.Articulo.PrecioConImpuestos;
                    response = new MayoristasBusiness(this.token).PagoCreditoMayorista(pago, venta.NumeroTransaccion);
                    response.CodeNumber = "1";
                }
            }
            return response;
        }

        private OperationResponse PagoTCMM(FinalizarVentaRequest request)
        {
            OperationResponse response = new OperationResponse();
            response.CodeNumber = "1";
            VentaResponse venta = this.repository.ObtenerVentaPorFolio(request.FolioVenta, 0);
            foreach (LineaTicket linea in venta.Lineas)
            {
                if (linea.TipoDetalleVenta == "46")
                {
                    PagoTCMMRequest pago = new PagoTCMMRequest();
                    pago.ModoEntrada = 12;
                    pago.NumeroCaja = token.CodeBox;
                    pago.NumeroTienda = token.CodeStore;
                    pago.Transaccion = venta.NumeroTransaccion;
                    pago.Importe = linea.Articulo.PrecioConImpuestos;
                    pago.NumeroTarjeta = linea.Articulo.InformacionPagoTCMM.NumeroTarjeta;
                    response = new MelodyMilanoBusiness(this.token).RealizarPago(pago);
                    if (response.CodeNumber == "1")
                    {
                        OperationResponse operationResponseAutoriacion = this.repository.RegistrarAutorizacionPagoTCMM(request.FolioVenta, pago.NumeroTarjeta, response.CodeDescription);
                    }
                }
            }
            return response;
        }

        private OperationResponse PagoServicios(FinalizarVentaRequest request)
        {
            OperationResponse response = new OperationResponse();
            response.CodeNumber = "1";
            VentaResponse venta = this.repository.ObtenerVentaPorFolio(request.FolioVenta, 0);
            PagoServiciosRequest pago = new PagoServiciosRequest();
            pago.Cuenta = venta.Lineas[0].Articulo.InformacionProveedorExternoAsociadaPS.Cuenta;

            pago.SkuCodePagoServicio = venta.Lineas[0].Articulo.InformacionProveedorExternoAsociadaPS.SkuCompania;
            pago.SkuCode = venta.Lineas[0].Articulo.Sku;
            pago.InfoAdicional = venta.Lineas[0].Articulo.InformacionProveedorExternoAsociadaPS.InfoAdicional;

            ResponseBussiness<OperationResponse> resultTA = new AdministracionPagoServiciosBusiness(this.token).PagoServicio(pago, float.Parse(venta.Lineas[0].Articulo.PrecioConImpuestos.ToString()), request.FolioVenta);

            if (resultTA.Data.CodeNumber != "1")
            {
                response.CodeNumber = resultTA.Data.CodeNumber;
                response.CodeDescription = resultTA.Data.CodeDescription;
            }
            return response;

        }

        private OperationResponse TiempoAire(FinalizarVentaRequest request)
        {
            OperationResponse response = new OperationResponse();
            response.CodeNumber = "1";
            VentaResponse venta = this.repository.ObtenerVentaPorFolio(request.FolioVenta, 0);
            TiempoAireRequest tiempoAireRequest = new TiempoAireRequest();
            tiempoAireRequest.Monto = float.Parse(venta.Lineas[0].ImporteVentaLineaNeto.ToString());
            tiempoAireRequest.Telefono = venta.Lineas[0].Articulo.InformacionProveedorExternoTA.NumeroTelefonico;
            tiempoAireRequest.SkuCode = venta.Lineas[0].Articulo.InformacionProveedorExternoTA.SkuCompania;
            ResponseBussiness<OperationResponse> resultTA = new TiempoAireBusiness(this.token).AddTiempoAire(tiempoAireRequest, venta.Lineas[0].Articulo.Sku, venta.FolioVenta);

            if (resultTA.Data.CodeNumber == "1")
            {
                repository.RegistrarRecargaTelefonicaExitosa(request.FolioVenta, this.token.CodeBox, this.token.CodeStore, tiempoAireRequest.Telefono, int.Parse(tiempoAireRequest.Monto.ToString()), resultTA.Data.CodeDescription);

            }
            else
            {
                response.CodeNumber = resultTA.Data.CodeNumber;
                response.CodeDescription = resultTA.Data.CodeDescription;
            }

            return response;
        }

        private OperationResponse ActivarTarjetaRegalo(FinalizarVentaRequest request)
        {
            OperationResponse response = new OperationResponse();
            response.CodeNumber = "1";
            TarjetaRegalosBusiness business = new TarjetaRegalosBusiness(this.token);
            foreach (InformacionFoliosTarjeta informacion in request.InformacionFoliosTarjeta)
            {
                var respuesta = business.ActivarTarjeta(this.token.CodeEmployee, informacion.FolioTarjeta.ToString(), request.FolioVenta);
                if (response.CodeNumber == "0")
                {
                    response.CodeDescription = respuesta.Data.CodeDescription;
                }
            }
            return response;
        }

        /// <summary>
        /// Anular una Venta
        /// </summary>
        /// <param name="anularTotalizarVentaRequest">Folio de venta y razón</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> AnularTotalizarVenta(AnularTotalizarVentaRequest anularTotalizarVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                OperationResponse operationResponse = repository.AnularVenta(anularTotalizarVentaRequest, this.token);
                ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                imprimeTicketsMM.PrintTicket(anularTotalizarVentaRequest.FolioVenta, false);
                return operationResponse;
            });
        }

        /// <summary>
        /// Eliminamos la linea de mayorista
        /// </summary>
        /// <param name="cabecera"></param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> EliminarLineaMayorista(CabeceraVentaRequest cabecera)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return repository.EliminarLineaMayorista(cabecera.FolioOperacion, this.token.CodeStore, this.token.CodeBox);
            });
        }


        /// <summary>
        /// Post-anular una Venta
        /// </summary>
        /// <param name="postAnularVentaRequest">Folio de venta y razón</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> PostAnularVenta(PostAnularVentaRequest postAnularVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                AnularTotalizarVentaRequest anularTotalizarVentaRequest = new AnularTotalizarVentaRequest();
                anularTotalizarVentaRequest.FolioVenta = postAnularVentaRequest.FolioVenta;
                anularTotalizarVentaRequest.CodigoRazon = postAnularVentaRequest.CodigoRazon;
                OperationResponse operationResponse = repository.AnularVenta(anularTotalizarVentaRequest, this.token);
                ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                imprimeTicketsMM.PrintTicket(postAnularVentaRequest.FolioVenta, false);
                return operationResponse;
            });
        }

        /// <summary>
        /// Suspender una Venta
        /// </summary>
        /// <param name="suspenderVentaRequest">Objeto de peticion de la venta a suspender</param>
        /// <returns></returns>
        public ResponseBussiness<OperationResponse> SuspenderVenta(SuspenderVentaRequest suspenderVentaRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                OperationResponse operationResponse = repository.SuspenderVenta(suspenderVentaRequest, token.CodeStore, token.CodeBox, token.CodeEmployee);
                ImprimeTicketsMM imprimeTicketsMM = new ImprimeTicketsMM(token);
                imprimeTicketsMM.PrintTicket(suspenderVentaRequest.cabeceraVentaRequest.FolioOperacion, false);
                return operationResponse;
            });
        }

        /// <summary>
        /// Busqueda de ventas por folio y fechas
        /// </summary>
        /// <param name="busquedaTransaccionRequest">Parámetros de búsqueda para las transacciones</param>
        /// <returns>Lista de ventas que cumplen con el criterio</returns>
        public ResponseBussiness<BusquedaTransaccionResponse[]> BuscarVentasPorFolioFecha(BusquedaTransaccionRequest busquedaTransaccionRequest)
        {
            return tryCatch.SafeExecutor(() =>
            {
                return repository.ObtenerVentas(busquedaTransaccionRequest);
            });
        }

        /// <summary>
        /// Busqueda de venta por folio
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="esDevolucion"></param>
        /// <returns>Venta correspondiente</returns>
        public ResponseBussiness<VentaResponse> BuscarVentaPorFolio(string folio, int esDevolucion)
        {
            return tryCatch.SafeExecutor(() =>
            {
                VentaResponse ventaResponse = repository.ObtenerVentaPorFolio(folio, esDevolucion);
                if (ventaResponse.NumeroNominaVentaEmpleado != 0)
                {
                    ventaResponse.InformacionEmpleadoMilano = new AdministracionVentaEmpleadoBusiness().Buscar(ventaResponse.NumeroNominaVentaEmpleado.ToString(), this.token.CodeStore.ToString(), this.token.CodeBox.ToString());
                }
                else if (ventaResponse.CodigoMayorista != 0)
                {
                    BusquedaMayoristaRequest busquedaMayorista = new BusquedaMayoristaRequest();
                    busquedaMayorista.CodigoMayorista = ventaResponse.CodigoMayorista;
                    busquedaMayorista.Nombre = "";
                    busquedaMayorista.SoloActivos = true;
                    ventaResponse.InformacionMayorista = new MayoristasBusiness(this.token).BusquedaMayorista(busquedaMayorista);
                }
                if (ventaResponse.CodigoEmpleadoVendedor > 0)
                {
                    EmployeeRequest employeeRequest = new EmployeeRequest();
                    employeeRequest.Code = ventaResponse.CodigoEmpleadoVendedor;
                    employeeRequest.Name = "";
                    ventaResponse.InformacionEmpleadoVendedor = new EmployeeBusiness(this.token).SearchEmployee(employeeRequest).Data[0];
                }
                return ventaResponse;
            });
        }

    }
}
