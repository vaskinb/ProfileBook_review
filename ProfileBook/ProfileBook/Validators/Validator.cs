using System;
using System.Text.RegularExpressions;

namespace ProfileBook.Validators
{
    public class Validator
    {
        private const string LOGIN_REGEX_EXP =
              @"^(?=.*[A-Za-z0-9]$)[A-Za-z][A-Za-z\d.-]{4,16}$";
        private const string PASSWORD_REGEX_EXP =
              @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,16}$";

        static Validator()
        {
            Login = new Validator(LOGIN_REGEX_EXP);
            Password = new Validator(PASSWORD_REGEX_EXP);
        }

        private Validator(string pattern)
        {
            Pattern = pattern;
        }

        #region -- Public static properies --

 
        public static Validator Login { get; }
        public static Validator Password { get; }

        #endregion

        #region --- Public properties ---

        public static bool ValidateStringAs(string input, Validator rule)
        {
            return
                !string.IsNullOrEmpty(input) &&
                Regex.IsMatch(input, rule.Pattern);
        }

        #endregion

        #region --- Private properties ---

        private string Pattern { get; }

        #endregion
    }
}
