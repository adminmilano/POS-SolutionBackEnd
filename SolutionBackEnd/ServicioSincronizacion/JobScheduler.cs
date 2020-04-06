using Milano.BackEnd.Business.Sincronizacion;
using Milano.BackEnd.Dto.Sincronizacion;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicioSincronizacion
{
    class JobScheduler
    {

        public void Iniciar()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                // Obtener la Periodicidad de ejecuciones
                SincronizacionBusiness ms = new SincronizacionBusiness();
                Periodicidad periodicidad = ms.ObtenerPeriodicidad();

                // Proceso de Ceder Apartados
                IJobDetail apartadosJob = JobBuilder.Create<ApartadosJob>().Build();
                ITrigger apartadosTrigger = TriggerBuilder.Create().WithSimpleSchedule(a => a.WithIntervalInSeconds(periodicidad.PeriodicidadProcesoCederApartados).RepeatForever()).Build();

                // Proceso de Motor de Sincronización            
                IJobDetail msJob = JobBuilder.Create<MSJob>().Build();
                ITrigger msTrigger = TriggerBuilder.Create().WithSimpleSchedule(a => a.WithIntervalInSeconds(periodicidad.PeriodicidadProcesoMotorSincronizacion).RepeatForever()).Build();

                // Proceso para Purgar Registros de Auditoría
                IJobDetail purgaJob = JobBuilder.Create<PurgaJob>().Build();
                ITrigger purgaTrigger = TriggerBuilder.Create().WithSimpleSchedule(a => a.WithIntervalInHours(4).RepeatForever()).Build();

                // Lanzamiento
                scheduler.ScheduleJob(apartadosJob, apartadosTrigger);
                scheduler.ScheduleJob(msJob, msTrigger);
                scheduler.ScheduleJob(purgaJob, purgaTrigger);
            }
            catch (Exception)
            {
                this.Parar();
                this.Iniciar();
            }
        }

        public void Parar()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Shutdown();
        }

    }
}
