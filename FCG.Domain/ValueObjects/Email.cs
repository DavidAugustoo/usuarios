using Flunt.Notifications;
using FCG.Domain.Validation;
using System.Text.RegularExpressions;

namespace FCG.Domain.ValueObjects
{
    public class Email: Notifiable<Notification>
    {
        public string EmailAddress { get; set; }
        
        public Email(){ }
        public Email(string endereco)
        {
            this.EmailAddress = endereco;

            if (!string.IsNullOrEmpty(this.EmailAddress) && !ValidarFormatoEmail())
                AddNotification("Email", "O formato do Email é inválido");

        }

        public override string ToString()
        {
            return EmailAddress;
        }

        private bool ValidarFormatoEmail()
        {
            if (string.IsNullOrEmpty(this.EmailAddress))
                return true;

            string regexEmail = @"^([a-zA-Z0-9_\-\.+]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})$";
            return Regex.IsMatch(this.EmailAddress, regexEmail);
        }
    }
}