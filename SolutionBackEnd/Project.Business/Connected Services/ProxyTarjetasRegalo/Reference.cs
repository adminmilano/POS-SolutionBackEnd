﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Milano.BackEnd.Business.ProxyTarjetasRegalo {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Respuesta", Namespace="http://tempuri.org/")]
    [System.SerializableAttribute()]
    public partial class Respuesta : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string sErrorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string sMensajeField;
        
        private System.DateTime dtFechaField;
        
        private decimal dSaldoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string sEstatusField;
        
        private int iTiendaVendidaField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string sError {
            get {
                return this.sErrorField;
            }
            set {
                if ((object.ReferenceEquals(this.sErrorField, value) != true)) {
                    this.sErrorField = value;
                    this.RaisePropertyChanged("sError");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string sMensaje {
            get {
                return this.sMensajeField;
            }
            set {
                if ((object.ReferenceEquals(this.sMensajeField, value) != true)) {
                    this.sMensajeField = value;
                    this.RaisePropertyChanged("sMensaje");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public System.DateTime dtFecha {
            get {
                return this.dtFechaField;
            }
            set {
                if ((this.dtFechaField.Equals(value) != true)) {
                    this.dtFechaField = value;
                    this.RaisePropertyChanged("dtFecha");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public decimal dSaldo {
            get {
                return this.dSaldoField;
            }
            set {
                if ((this.dSaldoField.Equals(value) != true)) {
                    this.dSaldoField = value;
                    this.RaisePropertyChanged("dSaldo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string sEstatus {
            get {
                return this.sEstatusField;
            }
            set {
                if ((object.ReferenceEquals(this.sEstatusField, value) != true)) {
                    this.sEstatusField = value;
                    this.RaisePropertyChanged("sEstatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=5)]
        public int iTiendaVendida {
            get {
                return this.iTiendaVendidaField;
            }
            set {
                if ((this.iTiendaVendidaField.Equals(value) != true)) {
                    this.iTiendaVendidaField = value;
                    this.RaisePropertyChanged("iTiendaVendida");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ProxyTarjetasRegalo.wsTarjetasRegaloSoap")]
    public interface wsTarjetasRegaloSoap {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento sFolioVenta del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/RealizarVenta", ReplyAction="*")]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaResponse RealizarVenta(Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento FolioTarjeta del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ConsultaTarjeta", ReplyAction="*")]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaResponse ConsultaTarjeta(Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento FolioTarjeta del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ActivaTarjeta", ReplyAction="*")]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaResponse ActivaTarjeta(Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RealizarVentaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RealizarVenta", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequestBody Body;
        
        public RealizarVentaRequest() {
        }
        
        public RealizarVentaRequest(Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RealizarVentaRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int codigoTienda;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int codigoCaja;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public int codigoCajero;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public System.DateTime fechaVenta;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string sFolioVenta;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=5)]
        public int transaccion;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string FolioTarjeta;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=7)]
        public decimal montoPagado;
        
        public RealizarVentaRequestBody() {
        }
        
        public RealizarVentaRequestBody(int codigoTienda, int codigoCaja, int codigoCajero, System.DateTime fechaVenta, string sFolioVenta, int transaccion, string FolioTarjeta, decimal montoPagado) {
            this.codigoTienda = codigoTienda;
            this.codigoCaja = codigoCaja;
            this.codigoCajero = codigoCajero;
            this.fechaVenta = fechaVenta;
            this.sFolioVenta = sFolioVenta;
            this.transaccion = transaccion;
            this.FolioTarjeta = FolioTarjeta;
            this.montoPagado = montoPagado;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class RealizarVentaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="RealizarVentaResponse", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaResponseBody Body;
        
        public RealizarVentaResponse() {
        }
        
        public RealizarVentaResponse(Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class RealizarVentaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta RealizarVentaResult;
        
        public RealizarVentaResponseBody() {
        }
        
        public RealizarVentaResponseBody(Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta RealizarVentaResult) {
            this.RealizarVentaResult = RealizarVentaResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ConsultaTarjetaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ConsultaTarjeta", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequestBody Body;
        
        public ConsultaTarjetaRequest() {
        }
        
        public ConsultaTarjetaRequest(Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ConsultaTarjetaRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int codigoTienda;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int codigoCaja;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string FolioTarjeta;
        
        public ConsultaTarjetaRequestBody() {
        }
        
        public ConsultaTarjetaRequestBody(int codigoTienda, int codigoCaja, string FolioTarjeta) {
            this.codigoTienda = codigoTienda;
            this.codigoCaja = codigoCaja;
            this.FolioTarjeta = FolioTarjeta;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ConsultaTarjetaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ConsultaTarjetaResponse", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaResponseBody Body;
        
        public ConsultaTarjetaResponse() {
        }
        
        public ConsultaTarjetaResponse(Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ConsultaTarjetaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ConsultaTarjetaResult;
        
        public ConsultaTarjetaResponseBody() {
        }
        
        public ConsultaTarjetaResponseBody(Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ConsultaTarjetaResult) {
            this.ConsultaTarjetaResult = ConsultaTarjetaResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ActivaTarjetaRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ActivaTarjeta", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequestBody Body;
        
        public ActivaTarjetaRequest() {
        }
        
        public ActivaTarjetaRequest(Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ActivaTarjetaRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int codigoTienda;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int codigoCaja;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public int codigoCajero;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public System.DateTime fechaVenta;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string FolioTarjeta;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=5)]
        public int transaccion;
        
        public ActivaTarjetaRequestBody() {
        }
        
        public ActivaTarjetaRequestBody(int codigoTienda, int codigoCaja, int codigoCajero, System.DateTime fechaVenta, string FolioTarjeta, int transaccion) {
            this.codigoTienda = codigoTienda;
            this.codigoCaja = codigoCaja;
            this.codigoCajero = codigoCajero;
            this.fechaVenta = fechaVenta;
            this.FolioTarjeta = FolioTarjeta;
            this.transaccion = transaccion;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ActivaTarjetaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ActivaTarjetaResponse", Namespace="http://tempuri.org/", Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaResponseBody Body;
        
        public ActivaTarjetaResponse() {
        }
        
        public ActivaTarjetaResponse(Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class ActivaTarjetaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ActivaTarjetaResult;
        
        public ActivaTarjetaResponseBody() {
        }
        
        public ActivaTarjetaResponseBody(Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ActivaTarjetaResult) {
            this.ActivaTarjetaResult = ActivaTarjetaResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsTarjetasRegaloSoapChannel : Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsTarjetasRegaloSoapClient : System.ServiceModel.ClientBase<Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap>, Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap {
        
        public wsTarjetasRegaloSoapClient() {
        }
        
        public wsTarjetasRegaloSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsTarjetasRegaloSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsTarjetasRegaloSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsTarjetasRegaloSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaResponse Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap.RealizarVenta(Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequest request) {
            return base.Channel.RealizarVenta(request);
        }
        
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta RealizarVenta(int codigoTienda, int codigoCaja, int codigoCajero, System.DateTime fechaVenta, string sFolioVenta, int transaccion, string FolioTarjeta, decimal montoPagado) {
            Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequest inValue = new Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequest();
            inValue.Body = new Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaRequestBody();
            inValue.Body.codigoTienda = codigoTienda;
            inValue.Body.codigoCaja = codigoCaja;
            inValue.Body.codigoCajero = codigoCajero;
            inValue.Body.fechaVenta = fechaVenta;
            inValue.Body.sFolioVenta = sFolioVenta;
            inValue.Body.transaccion = transaccion;
            inValue.Body.FolioTarjeta = FolioTarjeta;
            inValue.Body.montoPagado = montoPagado;
            Milano.BackEnd.Business.ProxyTarjetasRegalo.RealizarVentaResponse retVal = ((Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap)(this)).RealizarVenta(inValue);
            return retVal.Body.RealizarVentaResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaResponse Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap.ConsultaTarjeta(Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequest request) {
            return base.Channel.ConsultaTarjeta(request);
        }
        
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ConsultaTarjeta(int codigoTienda, int codigoCaja, string FolioTarjeta) {
            Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequest inValue = new Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequest();
            inValue.Body = new Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaRequestBody();
            inValue.Body.codigoTienda = codigoTienda;
            inValue.Body.codigoCaja = codigoCaja;
            inValue.Body.FolioTarjeta = FolioTarjeta;
            Milano.BackEnd.Business.ProxyTarjetasRegalo.ConsultaTarjetaResponse retVal = ((Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap)(this)).ConsultaTarjeta(inValue);
            return retVal.Body.ConsultaTarjetaResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaResponse Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap.ActivaTarjeta(Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequest request) {
            return base.Channel.ActivaTarjeta(request);
        }
        
        public Milano.BackEnd.Business.ProxyTarjetasRegalo.Respuesta ActivaTarjeta(int codigoTienda, int codigoCaja, int codigoCajero, System.DateTime fechaVenta, string FolioTarjeta, int transaccion) {
            Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequest inValue = new Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequest();
            inValue.Body = new Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaRequestBody();
            inValue.Body.codigoTienda = codigoTienda;
            inValue.Body.codigoCaja = codigoCaja;
            inValue.Body.codigoCajero = codigoCajero;
            inValue.Body.fechaVenta = fechaVenta;
            inValue.Body.FolioTarjeta = FolioTarjeta;
            inValue.Body.transaccion = transaccion;
            Milano.BackEnd.Business.ProxyTarjetasRegalo.ActivaTarjetaResponse retVal = ((Milano.BackEnd.Business.ProxyTarjetasRegalo.wsTarjetasRegaloSoap)(this)).ActivaTarjeta(inValue);
            return retVal.Body.ActivaTarjetaResult;
        }
    }
}
