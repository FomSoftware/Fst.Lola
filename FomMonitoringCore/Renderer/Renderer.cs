using System;
using System.IO;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace FomMonitoringCore.Renderer
{
    public class EmailChangedPasswordDto
    {
        public string FirstPart { get; set; }
        public string UsernameLabel { get; set; }
        public string PasswordLabel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastPart { get; set; }
        public string FooterText { get; set; }
    }

    public static class RazorViewToString
    {
        public static string RenderRazorEmailChangedPasswordViewToString(EmailChangedPasswordDto model)
        {
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "EmailTemplates", "ChangedPassword.cshtml");


            var config = new TemplateServiceConfiguration
            {
                Language = Language.CSharp, EncodedStringFactory = new RawStringFactory()
            };
            // .. configure your instance

            // VB.NET as template language.
            // Raw string encoding.
            config.EncodedStringFactory = new HtmlEncodedStringFactory(); // Html encoding.

            var service = RazorEngineService.Create(config);
            return service.RunCompile(File.ReadAllText(templateFilePath), "ChangedPassword", typeof(EmailChangedPasswordDto), model, null);

            
        }
    }
}