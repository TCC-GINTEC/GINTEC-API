using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApiGintec.Application.Util;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiGintec.Application.Commom
{
    public class QRCodeService
    {
        public QRCodeResponse DesencriptarQRCode(string token)
        {            
            HttpClient httpClient = new();

            using StringContent jsonContent = new(
        JsonConvert.SerializeObject(new
        {
            mensagem = token.Replace(" ", "+")
        }),
        Encoding.UTF8,
        "application/json");

            using HttpResponseMessage response = httpClient.PostAsync("https://5q91oxvsj0.execute-api.us-east-1.amazonaws.com/default/Crypto/Descriptografar", jsonContent).Result;

            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            string result = JsonConvert.DeserializeObject<dynamic>(jsonResponse).token;
            return JsonConvert.DeserializeObject<QRCodeResponse>(result);
        }
    }
}