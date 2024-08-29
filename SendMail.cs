using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Testing
{
    public class SendMail
    {
        public void S_Mail(string fullPath, string Addr)
        {
            Thread.Sleep(1000);
            try
            {
                var mail = new MailMessage()
                {
                    // ------------------------------------------수정이 필요한 부분-------------------------------------------
                    From = new MailAddress("whdgur0068@gmail.com"),
                    Subject = "제목",
                    Body = "본문"
                    // ------------------------------------------수정이 필요한 부분-------------------------------------------
                };
                Console.WriteLine("이메일 전송 준비!");

                mail.To.Add(new MailAddress(Addr));
                Attachment file = new Attachment(fullPath);
                mail.Attachments.Add(file);

                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    // ------------------------------------------수정이 필요한 부분-------------------------------------------
                    // 이메일 + 앱 비밀번호 (보안 - 검색에 앱 비밀번호)
                    // https://myaccount.google.com/apppasswords?pli=1&rapt=AEjHL4PHOpBOX5rHcaCavxoJc36EFKapLXPsilaXVF-0mmmut3nWc_aaDkZuoKXG7Zag9kn2eI5_0dkxidH20YPfx-oHMAIf96j0g8Y0d7wLtgFy0o43zRY
                    // ------------------------------------------수정이 필요한 부분-------------------------------------------
                    Credentials = new NetworkCredential("whdgur0068@gmail.com", "cmjn fbog dmpi tkiz")
                };

                //client.Send(mail);
                Console.WriteLine("이메일 전송 완료!");
                Console.WriteLine("다음 이메일 전송 대기");
            }
            catch (Exception ex)
            {
                Console.WriteLine("이메일 전송 실패");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
//cmjn fbog dmpi tkiz