using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uploadimage.Api.Controllers
{
    [Route("UploadImage")]
    public class UploadImageController: Controller
    {
        static string GenerateConnStr(string ip = "127.0.0.1", int blobport = 10000, int queueport = 10001, int tableport = 10002)
        {
            return $"DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://{ip}:{blobport}/devstoreaccount1;TableEndpoint=http://{ip}:{tableport}/devstoreaccount1;QueueEndpoint=http://{ip}:{queueport}/devstoreaccount1;";
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            // define as variáveis
            string imageExtension = null;
            var httpRequest = HttpContext.Request;
            string name = null;

            // pega as dados vindo da requisição HTTP
            IFormFile postedFile = httpRequest.Form.Files["Image"];
            name = httpRequest.Form["ImageName"];

            // pega a extensão da imagem
            imageExtension = Path.GetExtension(postedFile.FileName);

            // define o caminho para salvar a imagem
            var filePath = name + imageExtension;

            // define a string de conexão com o Azure Storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(GenerateConnStr());

            // Cria o Blob Client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Pega uma referência para o container
            CloudBlobContainer container = blobClient.GetContainerReference("upload-image");

            // Pega uma referencia para o blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePath);

            try
            {
                // Cria ou sobescreve o blob com o conteudo do arquivo local
                using (var fileStream = postedFile.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                }
                return Ok();
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
    }
}
