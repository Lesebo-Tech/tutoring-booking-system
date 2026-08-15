using System;
using System.Net.Mail;

namespace TutoringBookingSystem.Services
{
    public class EmailService
    {
        public void SendBookingEmail(string toAddress, string subject, string body)
        {
            try
            {
                var message = new MailMessage();
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                var client = new SmtpClient(); // reads settings from Web.config automatically
                client.Send(message);
            }
            catch (Exception ex)
            {
                // Don't let email failure break the booking itself — just log it for now
                System.Diagnostics.Debug.WriteLine("Email send failed: " + ex.Message);
            }
        }
    }
}