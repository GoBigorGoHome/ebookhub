using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using ebookhub.Calibre;
using ebookhub.Models;

namespace ebookhub.Controllers
{
    public class MailSender
    {
        private readonly SmtpOptions _smtpOptions;
        private readonly CalibreOptions _calibreOptions;

        public MailSender(IOptions<SmtpOptions> smtpoptions, IOptions<CalibreOptions> calibreOptions)
        {
            _smtpOptions = smtpoptions.Value;
            _calibreOptions = calibreOptions.Value;
        }

        public void SendFileByMail(EBookFile file, User recipient)
        {
            var path = Path.Combine(_calibreOptions.ContentFolder, file.RelativeFilePath);

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_smtpOptions.FromAddressTitle,
                _smtpOptions.FromAddress));
            mimeMessage.To.Add(new MailboxAddress("",
                recipient.KindleMail));
            mimeMessage.Subject = _smtpOptions.SubjectLine;

            var attachment = new MimePart("application", "vnd.amazon.mobi8-ebook")
            {
                ContentObject = new ContentObject(File.OpenRead(path), ContentEncoding.Binary),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path)
            };

            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            mimeMessage.Body = multipart;

            //var credential = ServiceAccountCredential();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, ch, e) => true;
                client.Connect(_smtpOptions.SmtpServer, _smtpOptions.SmtpServerPort, false);
                // easier than using OAuth2...
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_smtpOptions.FromAddress, _smtpOptions.Password);
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }

        private ServiceAccountCredential ServiceAccountCredential()
        {
            var certificate = new X509Certificate2(_smtpOptions.Certificate, _smtpOptions.CertificatePassword,
                X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential
                .Initializer(_smtpOptions.ServiceAccountMail)
                {
                    // Note: other scopes can be found here: https://developers.google.com/gmail/api/auth/scopes
                    Scopes = new[] { "https://mail.google.com/" },
                    User = "user"
                }.FromCertificate(certificate));

            //You can also use FromPrivateKey(privateKey) where privateKey
            // is the value of the fiel 'private_key' in your serviceName.json file

            bool result = credential.RequestAccessTokenAsync(CancellationToken.None).Result;
            return credential;
        }
    }
}
