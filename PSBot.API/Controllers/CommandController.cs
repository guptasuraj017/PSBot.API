using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSBot.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace PSBot.API.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly IWebHostEnvironment _appEnvironment;
        public CommandController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        [HttpGet]
        [Route("GetAllCommands")]
        public string GetAllCommands()
        {
            try
            
            {
                var webRootpath = _appEnvironment.WebRootPath;
                var jsonFilePath =webRootpath+ @"\json\command.json";
                using (StreamReader r = new StreamReader(jsonFilePath)) 
                {
                    var commandJson = r.ReadToEnd();
                    var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(JObject.Parse(commandJson));
                    return jsonResult;
                }
                
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        [HttpPost]
        [Route("RunCommand")]
        public JsonResult RunCommand(Command command)
        {

            try
            {
                
                var result = RunCmdlets(command);
                var baseObject = result[0].BaseObject;
            
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public Collection<PSObject> RunCmdlets(Command command)
        {

            try
            {

                IDictionary parameters = new Dictionary<String, String>();
                foreach (var parameter in command.Params)
                {
                    parameters.Add(parameter.ParameterName, @parameter.ParameterValue);
                }

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddCommand(command.Value).AddParameters(parameters);
                    var outputCommand = GetPsCommand(ps);
                    return ps.Invoke();
                }
                
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        
        private static string GetPsCommand(PowerShell ps)
        {
            string cmdText = string.Empty;
            for (int i = 0; i < ps.Commands.Commands.Count; i++)
            {
                var cmd = ps.Commands.Commands[i];
                cmdText += cmd.CommandText;
                foreach (var param in cmd.Parameters)
                {
                    if (!string.IsNullOrEmpty(param.Name))
                        cmdText += " -" + param.Name + ":"; cmdText += param.Value;
                }
                if (cmd.IsEndOfStatement || i + 1 == ps.Commands.Commands.Count)
                    cmdText += Environment.NewLine;
                else
                    cmdText += "|";
            }
            return cmdText;
        }


    }
}
