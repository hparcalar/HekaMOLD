using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.UseCases;
using HekaMOLD.MachineService.Helpers;
using HekaMOLD.MachineService.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HekaMOLD.MachineService
{
    class Program
    {
        static void Main(string[] args)
        {
            MachineModel[] machineList = new MachineModel[0];

            ApiHelper _api = new ApiHelper(ConfigurationManager.AppSettings["ApiUri"]);

            machineList = _api.GetData<MachineModel[]>("Common/GetMachineList").Result;
            machineList = machineList.Where(d => d.IsWatched == true).ToArray();

            //using (DefinitionsBO bObj = new DefinitionsBO())
            //{
            //    machineList = bObj.GetMachineList()
            //        .Where(d => d.IsWatched == true)
            //        .ToArray();
            //}

            foreach (var item in machineList)
            {
                DeviceListener device = new DeviceListener(item);
                device.Start();
            }

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
