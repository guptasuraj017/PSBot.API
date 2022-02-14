using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSBot.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

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
                    
                    var jsonResult = JsonConvert.SerializeObject(JObject.Parse(commandJson));
                   
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

                var ps = PowerShell.Create().AddCommand(command.Value).AddParameters(parameters).Invoke();
                return ps;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
