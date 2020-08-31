using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MBClassLibrary
{
    public static class MBValidations /*:ValidationAttribute*/
    {
        //string ErrorMessage="Invalid Postal cOde";
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    Regex pattern = new Regex(@"^[a-z]\d[a-z] ?\d[a-z]\d$", RegexOptions.IgnoreCase);
        //    if (value == null || value.ToString() == "" || pattern.IsMatch(value.ToString()))
        //        return ValidationResult.Success;

        //    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
        //}
        static MBValidations()
        {
           
        }
        public static string MBCapitalize(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            string x = input.ToLower().Trim();
            //if (x.Length == 1)
            //    x= x.ToUpper();
            //else
                x = Regex.Replace(x, @"(^\w)|(\s\w)", m => m.Value.ToUpper());

            return x;
        }

        public static string MBExtractDigits(string input)
        {
            string digits = string.Empty;
            if (input == null)
            {
                return string.Empty;
            }
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    digits += input[i];
                }
            }
            return digits;
        }
       
        public static bool MBPostalCodeValidation(string input)
        {
            Regex pattern = new Regex(@"^[a-r]\d[a-t]?\d[a-t]\d$", RegexOptions.IgnoreCase);

            if (input == null || input==string.Empty /*|| pattern.IsMatch(input.ToString())*/)
                return true;
            else if(input.Trim()=="")
                return false;
            else if(pattern.IsMatch(input.ToUpper().ToString()))
                return true;
            return false;
        }
        public static string MBPostalCodeFormat(string input)
        {
            if (MBPostalCodeValidation(input))
            {
                if (string.IsNullOrEmpty(input))
                {
                    return "The postal code cannot be blank";
                }
                else if (!input.Contains(" "))
                {
                   input= input.Insert(3, " ");
                    input=input.ToUpper();
                }
                return input;
            }
            return "The postal code is not valid";
        }
        public static bool MBZipCodeValidation(ref string input)
        {
            string digits = string.Empty;
            if (input==null || input==string.Empty)
            {
                input = string.Empty;
                return true;
            }
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    digits += input[i];
                }
            }
            if (digits.Length == 5)
            {
                input = digits;
                return true;
            }
            else if (digits.Length == 9)
            {
               input= input.Insert(5, "-");
                return true;
            }
            else
                return false;
        }


    }
}
