using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppSnacks.Validations
{
    public class Validator : IValidator
    {
        public string NameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneNumberError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string EmptyNameErrorMsg = "Please enter your name";
        private const string InvalidNameErrorMsg = "Please enter a valid name";
        private const string EmptyEmailErrorMsg = "Please enter your email";
        private const string InvalidEmailErrorMsg = "Please enter a valid email";
        private const string EmptyPhoneNumberErrorMsg = "Please enter your phone number";
        private const string InvalidPhoneNumberErrorMsg = "Please enter a valid phone number";
        private const string EmptyPasswordErrorMsg = "Please enter your password";
        private const string InvalidPasswordErrorMsg = "The password must be alphanumeric with a minimum length of 8 characters";

        public Task<bool> Validate(string name, string email, string phonenumber, string password)
        {
            var isNameValid = ValidateName(name);
            var isEmailValid = ValidateEmail(email);
            var isPhoneNumberValid = ValidatePhoneNumber(phonenumber);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isNameValid && isEmailValid && isPhoneNumberValid && isPasswordValid);
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phonenumber)
        {
            if (string.IsNullOrEmpty(phonenumber))
            {
                PhoneNumberError = EmptyPhoneNumberErrorMsg;
                return false;
            }

            if (phonenumber.Length < 9)
            {
                PhoneNumberError = InvalidPhoneNumberErrorMsg;
                return false;
            }

            PhoneNumberError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameError = EmptyNameErrorMsg;
                return false;
            }

            if (name.Length < 3)
            {
                NameError = InvalidNameErrorMsg;
                return false;
            }

            NameError = "";
            return true;
        }
    }
}
