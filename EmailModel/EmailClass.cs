using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Team8ADProjectSSIS.EmailModel
{
    public class EmailClass
    {
        SmtpClient client;
        
        String password = "woshishenaqq6!";
        public EmailClass() {
            client = new SmtpClient("smtp.gmail.com", 587);
        }

        public void SendTo(String address, String subject, String body)
        {
            client.Credentials = new System.Net.NetworkCredential(@"huangyuzhe2019@gmail.com", password);
            client.EnableSsl = true;
            //client.Timeout = 5000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage mm = new MailMessage("huangyuzhe2019@gmail.com", address);
            mm.Subject = subject;
            mm.Body = body;
            client.Send(mm);
        }

    }
}