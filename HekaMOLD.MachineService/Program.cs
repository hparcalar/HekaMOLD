using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.Business.UseCases;
using HekaMOLD.MachineService.Services;
using System;
using System.Collections.Generic;
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

            using (DefinitionsBO bObj = new DefinitionsBO())
            {
                machineList = bObj.GetMachineList()
                    .Where(d => d.IsWatched == true)
                    .ToArray();
            }

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
