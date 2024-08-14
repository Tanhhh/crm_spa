using DocumentFormat.OpenXml.Packaging;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


public static class SendMailNotGmail
{


    public static bool SendEmail(string port, string ssl, string smtp, string emailFrom, string SMTPuserName, string SMTPpassword,
                                     string SentTo, string subject, string body)
    {
        MimeMessage mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(emailFrom, emailFrom));
        mailMessage.Sender = new MailboxAddress(emailFrom, emailFrom);
        mailMessage.To.Add(new MailboxAddress(SentTo, SentTo));
        mailMessage.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = body;

        // Lấy đường dẫn của hình ảnh từ body và tạo LinkedResource tương ứng
        foreach (Match match in Regex.Matches(body, "<img.*?src=\"(.*?)\".*?>"))
        {
            string imageUrl = match.Groups[1].Value;
            string serverPath = HttpContext.Current.Server.MapPath(imageUrl);

            byte[] imageBytes;
            using (FileStream fileStream = File.OpenRead(serverPath))
            {
                imageBytes = new byte[fileStream.Length];
                fileStream.Read(imageBytes, 0, imageBytes.Length);
            }

            // Xác định loại MIME của ảnh
            string mimeType = GetMimeType(serverPath);
            var imagePart = new MimePart(mimeType)
            {
                Content = new MimeContent(new MemoryStream(imageBytes)),
                ContentTransferEncoding = ContentEncoding.Base64,
                ContentId = MimeUtils.GenerateMessageId()
            };

            builder.LinkedResources.Add(imagePart);

            // Thay thế src của thẻ <img> trong HTML bằng CID của hình ảnh
            builder.HtmlBody = builder.HtmlBody.Replace(imageUrl, $"cid:{imagePart.ContentId}");
        }

        var multipart = new Multipart("mixed");
        multipart.Add(builder.ToMessageBody());

    

        mailMessage.Body = multipart;

        try
        {
            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                if (ssl == "true")
                {
                    smtpClient.Connect(smtp, int.Parse(port), MailKit.Security.SecureSocketOptions.None);
                }
                else
                {
                    smtpClient.Connect(smtp, int.Parse(port), MailKit.Security.SecureSocketOptions.SslOnConnect);
                }

                smtpClient.Authenticate(SMTPuserName, SMTPpassword);
                smtpClient.Send(mailMessage);
                return true;
            }
        }
        catch (SmtpCommandException ex)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public static bool SendEmailAndFiles(string port, string ssl, string smtp, string emailFrom, string SMTPuserName, string SMTPpassword,
                                        string SentTo, string subject, string body, List<(string fileName, string filePath)> attachments)
    {
        try
        {
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailFrom, emailFrom));
            mailMessage.Sender = new MailboxAddress(emailFrom, emailFrom);
            mailMessage.To.Add(new MailboxAddress(SentTo, SentTo));
            mailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            // Lấy đường dẫn của hình ảnh từ body và tạo LinkedResource tương ứng
            foreach (Match match in Regex.Matches(body, "<img.*?src=\"(.*?)\".*?>"))
            {
                string imageUrl = match.Groups[1].Value;
                string serverPath = HttpContext.Current.Server.MapPath(imageUrl);

                byte[] imageBytes;
                using (FileStream fileStream = File.OpenRead(serverPath))
                {
                    imageBytes = new byte[fileStream.Length];
                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                }

                // Xác định loại MIME của ảnh
                string mimeType = GetMimeType(serverPath);
                var imagePart = new MimePart(mimeType)
                {
                    Content = new MimeContent(new MemoryStream(imageBytes)),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    ContentId = MimeUtils.GenerateMessageId()
                };

                builder.LinkedResources.Add(imagePart);

                // Thay thế src của thẻ <img> trong HTML bằng CID của hình ảnh
                builder.HtmlBody = builder.HtmlBody.Replace(imageUrl, $"cid:{imagePart.ContentId}");
            }

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());

            foreach (var attachment in attachments)
            {
                if (File.Exists(attachment.filePath))
                {
                    byte[] fileContent = File.ReadAllBytes(attachment.filePath);
                    var attachmentPart = new MimePart("application", "octet-stream")
                    {
                        Content = new MimeContent(new MemoryStream(fileContent), ContentEncoding.Default),
                        ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.fileName
                    };
                    multipart.Add(attachmentPart);
                }
            }

            mailMessage.Body = multipart;

            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                smtpClient.Connect(smtp, int.Parse(port), ssl == "true" ? MailKit.Security.SecureSocketOptions.None : MailKit.Security.SecureSocketOptions.SslOnConnect);
                smtpClient.Authenticate(SMTPuserName, SMTPpassword);
                smtpClient.Send(mailMessage);
            }

            foreach (var attachment in attachments)
            {
                try
                {
                    File.Delete(attachment.filePath);
                }
                catch (IOException ex)
                {
                    // Xử lý lỗi nếu cần
                }
            }

            return true;
        }
        catch (SmtpCommandException ex)
        {
            // Xử lý lỗi nếu cần
            return false;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu cần
            return false;
        }
    }

    private static string GetMimeType(string filePath)
    {
        // Hàm này trả về loại MIME dựa trên phần mở rộng của file
        var mimeType = "application/octet-stream";
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (extension == ".jpg" || extension == ".jpeg")
        {
            mimeType = "image/jpeg";
        }
        else if (extension == ".png")
        {
            mimeType = "image/png";
        }
        else if (extension == ".gif")
        {
            mimeType = "image/gif";
        }
        return mimeType;
    }

    public static bool SendEmailHangFire(string port, string ssl, string smtp, string emailFrom, string SMTPuserName, string SMTPpassword,
                                     string SentTo, string subject, string body, string basePath)
    {
        MimeMessage mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(emailFrom, emailFrom));
        mailMessage.Sender = new MailboxAddress(emailFrom, emailFrom);
        mailMessage.To.Add(new MailboxAddress(SentTo, SentTo));
        mailMessage.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = body;

        // Lấy đường dẫn của hình ảnh từ body và tạo LinkedResource tương ứng
        foreach (Match match in Regex.Matches(body, "<img.*?src=\"(.*?)\".*?>"))
        {
            string imageUrl = match.Groups[1].Value;
            string serverPath = Path.Combine(basePath, imageUrl.TrimStart('/').Replace('/', '\\'));

            byte[] imageBytes;
            using ( FileStream fileStream = File.OpenRead(serverPath))
            {
                imageBytes = new byte[fileStream.Length];
                fileStream.Read(imageBytes, 0, imageBytes.Length);
            }

            // Xác định loại MIME của ảnh
            string mimeType = GetMimeType(serverPath);
            var imagePart = new MimePart(mimeType)
            {
                Content = new MimeContent(new MemoryStream(imageBytes)),
                ContentTransferEncoding = ContentEncoding.Base64,
                ContentId = MimeUtils.GenerateMessageId()
            };

            builder.LinkedResources.Add(imagePart);

            // Thay thế src của thẻ <img> trong HTML bằng CID của hình ảnh
            builder.HtmlBody = builder.HtmlBody.Replace(imageUrl, $"cid:{imagePart.ContentId}");
        }

        var multipart = new Multipart("mixed");
        multipart.Add(builder.ToMessageBody());



        mailMessage.Body = multipart;

        try
        {
            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                if (ssl == "true")
                {
                    smtpClient.Connect(smtp, int.Parse(port), MailKit.Security.SecureSocketOptions.None);
                }
                else
                {
                    smtpClient.Connect(smtp, int.Parse(port), MailKit.Security.SecureSocketOptions.SslOnConnect);
                }

                smtpClient.Authenticate(SMTPuserName, SMTPpassword);
                smtpClient.Send(mailMessage);
                return true;
            }
        }
        catch (SmtpCommandException ex)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool SendEmailAndFilesHangFire(string port, string ssl, string smtp, string emailFrom, string SMTPuserName, string SMTPpassword,
                                      string SentTo, string subject, string body, List<(string fileName, string filePath)> attachments, string basePath)
    {
        try
        {
            MimeMessage mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(emailFrom, emailFrom));
            mailMessage.Sender = new MailboxAddress(emailFrom, emailFrom);
            mailMessage.To.Add(new MailboxAddress(SentTo, SentTo));
            mailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            // Lấy đường dẫn của hình ảnh từ body và tạo LinkedResource tương ứng
            foreach (Match match in Regex.Matches(body, "<img.*?src=\"(.*?)\".*?>"))
            {
        
                string imageUrl = match.Groups[1].Value;
                string serverPath = Path.Combine(basePath, imageUrl.TrimStart('/').Replace('/', '\\'));
                byte[] imageBytes;
                using (FileStream fileStream = File.OpenRead(serverPath))
                {
                    imageBytes = new byte[fileStream.Length];
                    fileStream.Read(imageBytes, 0, imageBytes.Length);
                }

                // Xác định loại MIME của ảnh
                string mimeType = GetMimeType(serverPath);
                var imagePart = new MimePart(mimeType)
                {
                    Content = new MimeContent(new MemoryStream(imageBytes)),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    ContentId = MimeUtils.GenerateMessageId()
                };

                builder.LinkedResources.Add(imagePart);

                // Thay thế src của thẻ <img> trong HTML bằng CID của hình ảnh
                builder.HtmlBody = builder.HtmlBody.Replace(imageUrl, $"cid:{imagePart.ContentId}");
            }

            var multipart = new Multipart("mixed");
            multipart.Add(builder.ToMessageBody());

            foreach (var attachment in attachments)
            {
                if (File.Exists(attachment.filePath))
                {
                    byte[] fileContent = File.ReadAllBytes(attachment.filePath);
                    var attachmentPart = new MimePart("application", "octet-stream")
                    {
                        Content = new MimeContent(new MemoryStream(fileContent), ContentEncoding.Default),
                        ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.fileName
                    };
                    multipart.Add(attachmentPart);
                }
            }

            mailMessage.Body = multipart;

            using (var smtpClient = new MailKit.Net.Smtp.SmtpClient())
            {
                smtpClient.Connect(smtp, int.Parse(port), ssl == "true" ? MailKit.Security.SecureSocketOptions.None : MailKit.Security.SecureSocketOptions.SslOnConnect);
                smtpClient.Authenticate(SMTPuserName, SMTPpassword);
                smtpClient.Send(mailMessage);
            }

            foreach (var attachment in attachments)
            {
                try
                {
                    File.Delete(attachment.filePath);
                }
                catch (IOException ex)
                {
                    // Xử lý lỗi nếu cần
                }
            }

            return true;
        }
        catch (SmtpCommandException ex)
        {
            // Xử lý lỗi nếu cần
            return false;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu cần
            return false;
        }
    }

}