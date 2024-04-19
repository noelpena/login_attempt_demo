using Dapper;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using LoginAttemptDemo.Models;
using Npgsql;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace LoginAttemptDemo.Data
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> SendAuthEmail(UserContainerModel user)
        {
            //Generate auth code
            var auth = new ResetAuthCodeModel(
                _employeeId: user.User.employee_id,
                _workEmail: user.User.work_email
            );

            //Save auth code to db
            using (var connection = new NpgsqlConnection(_config.GetConnectionString("Default")))
            {
                string sql = "INSERT INTO login_attempt_demo_authcodes (" +
                                    "employee_id," +
                                    "work_email," +
                                    "code," +
                                    "expiration_date" +
                                ") " +
                                "VALUES (" +
                                    "@employee_id, @work_email, @code, @expiration_date" +
                                ")";

                var affectedRows = await connection.ExecuteAsync(sql, auth);
                //var rowsAffected = await connection.InsertAsync<ResetAuthCodeModel>(auth);
            }

            string from = "admin@ezer.noelpena.com";
            string host = _config.GetSection("Resend")["Host"];
            string username = _config.GetSection("Resend")["User"];
            string password = _config.GetSection("Resend")["ApiKey"];
            //string recipient = _config.GetSection("Smtp")["Recipient"];
            string recipient = user.User.personal_email;

            int port = Int16.Parse(_config.GetSection("Resend")["Port"]);

            var sender = new SmtpSender(() => new SmtpClient(host)
            {
                EnableSsl = true,
                //DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 587, //port,
                Credentials = new NetworkCredential(username, password)
            });

            StringBuilder template = new();
            template.AppendLine("<p>Hello User,</p>");
            template.Append("<p>");
            template.Append("Here is your authorization code for your work account reset request. ");
            template.Append("</p>");

            template.Append("<p>");
            template.Append("<strong>@Model.AuthCode</strong>");
            template.Append("</p>");

            template.Append("<p>");
            template.Append("Please enter this code into the \"Reset Tool\".");
            template.Append("This authorization code will expire in 10 minutes.");
            template.Append("</p>");

            template.Append("<p>");
            template.Append("If you have questions please contact the administrator.");
            template.Append("</p>");

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            var email = await Email
                .From(from)
                .To(recipient)
                .Subject("Login Attempt Demo | Password Reset Authorization Code")
                .UsingTemplate(template.ToString(), new
                {
                    AuthCode = auth.code
                })
                .SendAsync();

            if (email.Successful)
            {
                return "";
            }
            else
            {
                return "";
            }
        }
    }
}
