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
using HekaMOLD.Business.Models.Constants;

namespace HekaMOLD.MachineService.Services
{
    public class DeviceListener : IDisposable
    {
        ApiHelper _apiDevice;
        MachineModel _machine;
        MachineStatus _status;
        bool _isRunning = false;
        bool _isPostureCheckRunning = false;
        bool _isMachineDbCheckRunning = false;
        MachineStatusType _machineStatus = MachineStatusType.Stopped;
        bool _lastResult = false;
        Task _tListen;
        Task _tPostureCheck;
        Task _tMachineDbCheck;

        public DeviceListener(MachineModel machine)
        {
            _machine = machine;
            _status = new MachineStatus
            {
                MachineId = machine.Id,
                LastCycleEnd = null,
                PostureExpirationCycleCount = machine.PostureExpirationCycleCount,
                PostureRequestSent = false,
            };
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

            _isPostureCheckRunning = true;
            _tPostureCheck = Task.Run(DoPostureCheck);

            _isMachineDbCheckRunning = true;
            _tMachineDbCheck = Task.Run(DoDbCheck);
        }

        public void Stop()
        {
            _isRunning = false;
            _isPostureCheckRunning = false;

            try
            {
                if (_tListen != null)
                    _tListen.Dispose();
            }
            catch (Exception)
            {

            }

            try
            {
                if (_tPostureCheck != null)
                    _tPostureCheck.Dispose();
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

        private async Task DoDbCheck()
        {
            while (_isMachineDbCheckRunning)
            {
                try
                {
                    using (ProductionBO bObj = new ProductionBO())
                    {
                        _machineStatus = bObj.GetMachineStatus(_machine.Id);
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(4000);
            }
        }
        private async Task DoListen()
        {
            while (_isRunning)
            {
                try
                {
                    if (_machineStatus != MachineStatusType.Running)
                        continue;

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

                            _lastResult = resultSatisfied;
                            _status.PostureRequestSent = false;
                            _status.LastCycleEnd = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(100);
            }
        }

        private async Task DoPostureCheck()
        {
            while (_isPostureCheckRunning)
            {
                try
                {
                    if (_status.PostureExpirationCycleCount > 0 && _status.PostureRequestSent != true)
                    {
                        if (_status.LastCycleEnd == null)
                        {
                            using (ProductionBO bObj = new ProductionBO())
                            {
                                var lastSignal = bObj.GetLastMachineSignal(_status.MachineId);
                                if (lastSignal != null)
                                {
                                    _status.LastCycleEnd = lastSignal.EndDate != null ?
                                        lastSignal.EndDate : DateTime.Now;
                                }
                                else
                                    _status.LastCycleEnd = DateTime.Now;
                            }
                        }

                        if (_status.LastCycleEnd != null)
                        {
                            using (ProductionBO bObj = new ProductionBO())
                            {
                                var activeWork = bObj.GetActiveWorkOrderOnMachine(_status.MachineId);
                                if (activeWork != null
                                    && activeWork.WorkOrder != null && (activeWork.WorkOrder.MoldTestCycle ?? 0) > 0)
                                {
                                    var maxDiffSeconds = activeWork.WorkOrder.MoldTestCycle.Value *
                                        _status.PostureExpirationCycleCount.Value;
                                    if ((DateTime.Now - _status.LastCycleEnd).Value.TotalSeconds > maxDiffSeconds)
                                    {
                                        bObj.SetMachineAsIsUpForPosture(_status.MachineId, true);
                                        _status.PostureRequestSent = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(7500);
            }
        }
    }
}
