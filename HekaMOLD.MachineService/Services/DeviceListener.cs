using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HekaMOLD.Business.Models.DataTransfer.Production;
using HekaMOLD.MachineService.Helpers;
using HekaMOLD.MachineService.Models;
using DynamicExpresso;
using HekaMOLD.Business.UseCases;
using HekaMOLD.Business.Models.Operational;

namespace HekaMOLD.MachineService.Services
{
    public class DeviceListener : IDisposable
    {
        ApiHelper _apiDevice;
        MachineModel _machine;
        bool _isRunning = false;
        bool _lastResult = false;
        Task _tListen;

        public DeviceListener(MachineModel machine)
        {
            // http://192.168.127.254/api/
            _apiDevice = new ApiHelper(machine.DeviceIp);
            _apiDevice.AddHeader("Accept", "vdn.dac.v1");
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _isRunning = true;
            _tListen = Task.Run(DoListen);
        }

        public void Stop()
        {
            _isRunning = false;
            try
            {
                if (_tListen != null)
                    _tListen.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private async Task<int> GetFromDevice(string ioPort)
        {
            try
            {
                int result = -1;

                var ioResults = await _apiDevice.GetData<DigitalIOResult>("slot/0/io/" + ioPort.Substring(0, 2));

                if (ioPort.Substring(0, 2) == "di")
                {
                    var portResult = ioResults.io.di.FirstOrDefault(d => d.diIndex == Convert.ToInt32(ioPort.Replace("di", "")));
                    if (portResult != null)
                    {
                        result = portResult.diStatus;
                    }
                }
                else if (ioPort.Substring(0, 2) == "do")
                {
                    var portResult = ioResults.io.Do.FirstOrDefault(d => d.doIndex == Convert.ToInt32(ioPort.Replace("do", "")));
                    if (portResult != null)
                    {
                        result = portResult.doStatus;
                    }
                }

                return result;
            }
            catch (System.Exception)
            {
                Console.WriteLine("MOXA device is not accessible!");
            }

            return -1;
        }

        private async Task DoListen()
        {
            while (_isRunning)
            {
                try
                {
                    var variables = Regex.Matches(_machine.WatchCycleStartCondition, "\\[[^\\[\\]]+\\]");
                    if (variables.Count > 0)
                    {
                        var comparison = _machine.WatchCycleStartCondition;
                        foreach (Match item in variables)
                        {
                            var ioPort = item.Value.Replace("[", "").Replace("]", "")
                            .ToLower().Replace("ı", "i");
                            if (!string.IsNullOrEmpty(ioPort))
                            {
                                var result = await this.GetFromDevice(ioPort);
                                comparison = comparison.Replace(item.Value, result.ToString());
                            }
                        }

                        var expresso = new Interpreter();
                        bool resultSatisfied = false;

                        resultSatisfied = expresso.Eval<bool>(comparison);

                        if (resultSatisfied != _lastResult)
                        {
                            using (ProductionBO bObj = new ProductionBO())
                            {
                                BusinessResult bResult;

                                if (resultSatisfied)
                                    bResult = bObj.StartMachineCycle(_machine.Id);
                                else
                                    bResult = bObj.StopMachineCycle(_machine.Id);

                                if (bResult.Result)
                                    Console.WriteLine(string.Format("{0:[HH:mm:ss]}", DateTime.Now) +
                                        " " + _machine.MachineCode + ": " + (resultSatisfied ? "Cycle Started" : "Cycle End"));
                                else
                                    Console.WriteLine(string.Format("{0:[HH:mm:ss]}", DateTime.Now) +
                                        " " + _machine.MachineCode + ": HATA= " + bResult.ErrorMessage);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(100);
            }
        }
    }
}
