using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Milano.BackEnd.Dto;
using Milano.BackEnd.Dto.General;

namespace Milano.BackEnd.Repository
{
    /// <summary>
    /// Informacion de servicios externos
    /// </summary>
    public class InformacionServiciosExternosRepository : BaseRepository
    {
        /// <summary>
        /// Obtiene las credencialese información asociada  del servicio.
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns></returns>
        public CredencialesServicioExterno ObtenerCredenciales(int idServicio)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("@idServicio", idServicio);
            CredencialesServicioExterno credenciales = new CredencialesServicioExterno();
            foreach (var r in data.GetDataReader("[dbo].[sp_vanti_InformacionServicioExterno]", parameters))
            {
                credenciales.UserName = r.GetValue(0).ToString();
                credenciales.Password = r.GetValue(1).ToString();
                credenciales.NumeroIntentos = Convert.ToInt32(r.GetValue(2));
                credenciales.Licence = r.GetValue(3).ToString();
            }
            return credenciales;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InfoService ObtenerInfoServicioExterno(int idServicio)
        {
            InfoService infoService = new InfoService();
            var parameters = new Dictionary<string, object>();
            parameters.Add("@IdServicio", idServicio);
            foreach (var r in data.GetDataReader("[dbo].[sp_vanti_bo_ObtenerInfoServiciosExternosBackOfficeLoginServiceMilano]", parameters))
            {
                infoService.NameService = r.GetValue(0).ToString();
                infoService.UrlService = r.GetValue(1).ToString();
                infoService.UserName = r.GetValue(2).ToString();
                infoService.Password = r.GetValue(3).ToString();
            }
            return infoService;
        }
    }
}
