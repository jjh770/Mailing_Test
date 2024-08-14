using System.Net;
using System.Net.Mail;

namespace Testing
{
    class SendMail
    {
        public void S_Mail(string fullPath)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress("whdgur0068@gmail.com"),
                Subject = "이메일 테스트",
                Body = "안녕하세요 이메일 테스트 중입니다."
            };

            mail.To.Add(new MailAddress("whdgur1068@naver.com"));
            Attachment file = new Attachment(fullPath);
            mail.Attachments.Add(file);

            var client = new SmtpClient()
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Credentials = new NetworkCredential("whdgur0068@gmail.com", "phml ekuc sesk dqwk")
            };

            client.Send(mail);
        }   
    }
}

//phml ekuc sesk dqwk