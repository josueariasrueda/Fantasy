﻿using Fantasy.Shared.Responses;
using MailKit.Net.Smtp;
using MimeKit;

namespace Fantasy.Backend.Helpers;

public class MailHelper : IMailHelper
{
    private readonly IConfiguration _configuration;
    private readonly ISmtpClient _smtpClient;

    public MailHelper(IConfiguration configuration, ISmtpClient smtpClient)
    {
        _configuration = configuration;
        _smtpClient = smtpClient;
    }

    public ActionResponse<string> SendMail(string toName, string toEmail, string subject, string body, string language)
    {
        try
        {
            var from = _configuration["Mail:From"];
            var name = _configuration["Mail:NameEn"];
            if (language == "es")
            {
                name = _configuration["Mail:NameEs"];
            }
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(name, from));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            BodyBuilder bodyBuilder = new()
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            _smtpClient.Connect(smtp!, int.Parse(port!), false);
            _smtpClient.Authenticate(from!, password!);
            _smtpClient.Send(message);
            _smtpClient.Disconnect(true);

            return new ActionResponse<string> { WasSuccess = true };
        }
        catch (Exception ex)
        {
            return new ActionResponse<string>
            {
                WasSuccess = false,
                Message = ex.Message,
            };
        }
    }
}