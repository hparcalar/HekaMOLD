using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.UseCases.Core.Base;
using Heka.DataAccess.UnitOfWork;
using Heka.DataAccess.Context;

namespace PrintReplicator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    using (CoreSystemBO bObj = new CoreSystemBO())
                    {
                        var printers = bObj.GetPrinterList();
                        foreach (var prt in printers)
                        {
                            var queue = bObj.GetPrinterQueue(prt.Id);
                            foreach (var item in queue)
                            {
                                HekaEntities targetDb = new HekaEntities();

                                targetDb.WorkOrderSerial()
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                Thread.Sleep(1000);
            }


            //while (true)
            //{
            //    Console.ReadLine();
            //}
        }
    }
}
