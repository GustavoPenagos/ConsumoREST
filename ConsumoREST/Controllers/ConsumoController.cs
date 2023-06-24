using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Net.Http;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ConsumoREST.Model;


namespace ConsumoREST.Controllers
{
    [ApiController]
    public class ConsumoController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ConsumoController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost]
        [Route("/api/consumo")]
        public async Task<dynamic> Consumo(Cliente cliente, int opcion)
        {
            try
            {
                //ConnectionURL
                
                string url = _config.GetValue<string>("ConnectionURL:urlAPI") + "?opcion=" + opcion;
                HttpClient client = new HttpClient();
                var request = cliente;
                string json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var respondeContent = await response.Content.ReadAsStringAsync();
                Cliente? deserializeResponse = JsonSerializer.Deserialize<Cliente>(respondeContent);

                if(respondeContent != null)
                {
                    return new
                    {
                        status = Ok("correcto"),
                        result = new
                        {
                            message = deserializeResponse
                        }
                    };
                }
                else
                {
                    return new
                    {
                        status = Ok("incorrecto"),
                        result = new
                        {
                            message = new
                            {
                                xUrl = url,
                                xRequest = JsonSerializer.Serialize(request) ,
                                xContent = content,
                                xResponse = response,
                                xRespondeContent = deserializeResponse

                            }
                        }
                    };
                }

            }
            catch (Exception ex)
            {
                return new
                {
                    status = Ok("Error"),
                    result = new
                    {
                        message = ex.Message
                    }
                };
            }
            
        }

        
    }
}
