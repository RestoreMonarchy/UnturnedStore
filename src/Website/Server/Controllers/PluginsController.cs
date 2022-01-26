﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using Website.Data.Repositories;
using Website.Shared.Params;
using Website.Shared.Results;

namespace Website.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginsController : ControllerBase
    {
        private readonly PluginsRepository loaderRepository;

        public PluginsController(PluginsRepository loaderRepository)
        {
            this.loaderRepository = loaderRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostPluginAsync([FromBody] GetPluginParams @params)
        {
            if (@params.ServerInfo == null)
            {
                return BadRequest(new GetPluginResult() 
                { 
                    ErrorMessage = "ServerInfo is required",
                    ReturnCode = 101
                });
            }

            if (@params.ServerInfo.Port > 65535)
            {
                return BadRequest(new GetPluginResult()
                {
                    ErrorMessage = "Invalid server port",
                    ReturnCode = 102
                });
            }

            @params.ServerInfo.Host = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            GetPluginResult result = await loaderRepository.GetPluginAsync(@params);

            if (result.ReturnCode != 0)
            {
                return BadRequest(result);
            }

            byte[] salt = { 82, 122, 43, 30, 47, 97, 4, 124, 31, 63, 108, 69, 83, 86, 125, 88, 98, 77, 111, 79, 71, 73, 100, 106, 8, 20, 95, 27, 38, 32, 61, 88 };
            var pdb = new Rfc2898DeriveBytes("The_Key", salt);

            using Aes aes = Aes.Create();

            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using MemoryStream ms = new();
            using CryptoStream cryptoStream = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(result.Version.Content, 0, result.Version.Content.Length);
            await cryptoStream.FlushFinalBlockAsync();

            result.Version.Content = ms.ToArray();

            Response.Headers.Add("DecryptKey", BitConverter.ToString(aes.Key));
            Response.Headers.Add("DecryptIV", BitConverter.ToString(aes.IV));
            Response.Headers.Add("PluginVersion", result.Version.Name);
            Response.Headers.Add("Changelog", result.Version.Changelog);
            return File(result.Version.Content, result.Version.ContentType, result.Version.FileName);
        }
    }
}