using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nDumbster.Smtp;
using NHibernate.Util;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Email;
using Queo.Boards.Core.Infrastructure.Templating;
using Queo.Boards.Core.Infrastructure.Templating.DotLiquid;
using Queo.Boards.Core.Infrastructure.Templating.FileMessage;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public class EmailServiceTest : ServiceBaseTest {
        private SimpleSmtpServer _smtpServer;

        [TestCleanup]
        public void Cleanup() {
            _smtpServer.Stop();
        }

        [TestInitialize]
        public void Setup() {
            _smtpServer = SimpleSmtpServer.Start();
        }

        [TestMethod]
        public void TestCreateMessage() {
            EmailService emailService = GetEmailService(new StaticMailMessageProvider());

            MailMessage mailMessage = emailService.CreateMailMessage("test@test.com", new ModelMap(), "default", false);
            Assert.AreEqual("test@test.com", mailMessage.To.First().Address);
            Assert.AreEqual("Der Mailbetreff", mailMessage.Subject);
            Assert.AreEqual(@"Hallo Nutzer,

das ist eine Mail für dich.

Viele Grüße
Tester", mailMessage.Body);
        }

        [TestMethod]
        public void TestSendMailMessage() {
            EmailService emailService = GetEmailService(new StaticMailMessageProvider());
            MailMessage mailMessage = emailService.CreateMailMessage("test@test.com", new ModelMap(), "default", false);
            emailService.SendMessage(mailMessage);

            MailMessage receivedEmail = (MailMessage)_smtpServer.ReceivedEmail.FirstOrNull();
            Assert.AreEqual(1, _smtpServer.ReceivedEmailCount);
            Assert.AreEqual(mailMessage.Body, receivedEmail.Body);
            Assert.AreEqual(mailMessage.Subject, receivedEmail.Subject);
            Assert.AreEqual(mailMessage.To.ToString(), receivedEmail.To.ToString());
        }

        /// <summary>
        /// Testet das Versenden einer Nachricht mit DotLiquid-Template.
        /// </summary>
        [TestMethod]
        public void TestCreateMailWithDotLiquid() {

            //Given: Ein Template für DotLiquid
            EmailService emailService = GetEmailService(new FileMessageProvider("/Resources/TestTemplates/DotLiquid", new DotLiquidRenderContext()));

            //When: Eine Nachricht mit diesem Template verschickt werden soll
            string to = "info@queo-group.com";
            string templateEngine = "DotLiquid";
            User testUser = new User("testuser", new UserAdministrationDto(new List<string> {"Tester"}, true), new UserProfileDto("testuser@queo.de", "Test", "Nutzer", "queo", "Test", "01234/567890"));
            ModelMap modelMap = new ModelMap() {
                { "templateEngine", templateEngine },
                { "user", testUser }
            };
            MailMessage mailMessage = emailService.CreateMailMessage(to, modelMap, "TestCreateMailWithDotLiquid", false);

            //Then: Müssen alle Platzhalter korrekt ersetzt werden
            mailMessage.Body.Should().Contain(templateEngine);
            mailMessage.Body.Should().Contain(testUser.Firstname + " " + testUser.Lastname);
            mailMessage.Subject.Should().Contain(testUser.UserName);

        }

        private EmailService GetEmailService(IMessageProvider messageProvider) {
            EmailService emailService = new EmailService();
            emailService.EmailMessageProvider = messageProvider;
            emailService.EmailSenderAddress = "test@test.com";
            emailService.EmailSenderName = "tester";
            emailService.SmtpHostAddress = "localhost";
            emailService.SmtpPort = 25;

            return emailService;
        }
    }

    /// <summary>
    ///     Klasse für die Bereitstellungen von Testtexten in Tests.
    /// </summary>
    internal class StaticMailMessageProvider : IMessageProvider {
        private readonly Dictionary<string, string> _mailMessages = new Dictionary<string, string>() {
            {
                "default", @"Subject: Der Mailbetreff
Hallo Nutzer,

das ist eine Mail für dich.

Viele Grüße
Tester"
            }
        };

        /// <summary>
        ///     Rendert eine MailMessage aus dem angegebenen Template und verwendet dabei die Daten aus dem Model.
        /// </summary>
        /// <param name="templateName">Name des Templates</param>
        /// <param name="model">Daten für das Template</param>
        /// <returns></returns>
        public string RenderMessage(string templateName, ModelMap model) {
            if (_mailMessages.ContainsKey(templateName)) {
                return _mailMessages[templateName];
            }
            return _mailMessages["default"];
        }
    }
}