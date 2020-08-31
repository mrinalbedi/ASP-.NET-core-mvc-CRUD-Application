using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using MBClassLibrary;
using System.Text.RegularExpressions;

namespace MBPatients.Models
{
    [ModelMetadataTypeAttribute(typeof(MBPatientMetaData))]
    public partial class Patient : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            PatientsContext _context = new PatientsContext();

            if (string.IsNullOrEmpty(FirstName) || FirstName == " ")
            {
                yield return new ValidationResult("First name is a required field and cannot be blank spaces", new[] { "FirstName" });
            }
            else
            {
                FirstName = FirstName.Trim();
                FirstName = MBValidations.MBCapitalize(FirstName);
            }
            if (string.IsNullOrEmpty(LastName) || LastName == " ")
            {
                yield return new ValidationResult("Last name is a required field and should not be blank space", new[] { "LastName" });
            }
            else
            {
                LastName = LastName.Trim();
                LastName = MBValidations.MBCapitalize(LastName);
            }
            if (string.IsNullOrEmpty(Gender) || Gender==" ")
            {
                yield return new ValidationResult("Gender is a required field and should not start with a blank space", new[] { "Gender" });
            }
            else
            {
                Gender = Gender.Trim();
               Gender= MBValidations.MBCapitalize(Gender);
            }
            if (!string.IsNullOrEmpty(Address))
            {
                Address = Address.Trim();
                Address = MBValidations.MBCapitalize(Address);
            }
            if (!string.IsNullOrEmpty(City))
            {
                City = City.Trim();
                City = MBValidations.MBCapitalize(City);
            }
            if (!string.IsNullOrEmpty(ProvinceCode))
            {
                ProvinceCode = ProvinceCode.Trim();
                Province Pro = new Province();
                string error = "";
                ProvinceCode = ProvinceCode.ToUpper();
                try
                {
                    Pro = _context.Province.Where(m => m.ProvinceCode == ProvinceCode).FirstOrDefault();
                    //var country = Pro.CountryCode;
                }
                catch (Exception e)
                {
                    error = e.GetBaseException().Message;
                    
                }
                if (Pro == null)
                {
                    yield return new ValidationResult(error, new[] { nameof(ProvinceCode) });
                }
                else
                {
                    if (PostalCode != null)
                    {
                        PostalCode = PostalCode.Trim();
                        bool val = false;
                        string USZipCode = PostalCode;
                        if (Pro.CountryCode == "CA")
                        {
                            var x = Pro.FirstPostalLetter;
                            char[] charArr = x.ToCharArray();
                            foreach (char ch in charArr)
                            {
                                if (Convert.ToChar(PostalCode.Substring(0, 1).ToUpper()) == ch)
                                    //if(PostalCode.StartsWith(ch))
                                        val = true;
                            }
                            if (!val)
                                yield return new ValidationResult("Postal code entered is not a valid code for the selected province", new[] { "PostalCode" });
                            if (!MBValidations.MBPostalCodeValidation(PostalCode))
                            {

                                yield return new ValidationResult("Postal code entered is not in valid format", new[] { "PostalCode" });
                            }
                            else
                            {
                               PostalCode= MBValidations.MBPostalCodeFormat(PostalCode);
                            }
                        }
                        if (Pro.CountryCode == "US")
                        {
                            if (!MBValidations.MBZipCodeValidation(ref USZipCode))
                                yield return new ValidationResult("Zip code entered is not in valid format", new[] { "PostalCode" });
                            else
                                PostalCode = USZipCode;

                        }
                    }
                }
            }

            if (Ohip != null)
            {

                Ohip = Ohip.ToUpper();
                Regex pattern = new Regex(@"^\d\d\d\d-\d\d\d-\d\d\d-[a-z][a-z]$", RegexOptions.IgnoreCase);
                if (!pattern.IsMatch(Ohip))
                {
                    yield return new ValidationResult("The value for OHIP entered is not in valid format", new[] { "Ohip" });
                }
            }
            if (HomePhone != null)
            {
                HomePhone = HomePhone.Trim();
                if (HomePhone.Length != 10)
                {
                    yield return new ValidationResult("The Home Phone Number entered should be exactly 10 digits", new[] { "HomePhone" });
                }
                HomePhone = HomePhone.Insert(3, "-");
            HomePhone = HomePhone.Insert(7, "-");
        }
            if(DateOfBirth!=null)
            {
                if(DateOfBirth>DateTime.Now)
                    yield return new ValidationResult("The date of Birth cannot be greater than current date", new[] { "DateOfBirth" });

            }
            if(Deceased)
            {
                if(DateOfDeath==null)
                    yield return new ValidationResult("The date of death is required if the deceased checkbox is chec", new[] { "DateOfDeath" });
            }
            else
            {
                DateOfDeath = null;
            }
            if(DateOfDeath!=null)
            {
                if(DateOfDeath>DateTime.Now|| DateOfDeath<DateOfBirth)
                    yield return new ValidationResult("The date of death cannot be greater than current date and before the Date of birth", new[] { "DateOfBirth" });

            }
           
            if(Gender!="M"&&Gender!="F"&&Gender!="X")
            {
                yield return new ValidationResult("The value for gender entered must be M, F or X", new[] { "Gender" });
            }
            
        yield return ValidationResult.Success;    
        }
    }
    public class MBPatientMetaData
    {
        public MBPatientMetaData()
        {
            
        }
        public int PatientId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name ="Last Name")]
        public string LastName { get; set; }
        
        [Display(Name ="Street Address")]
        public string Address { get; set; }
        
        public string City { get; set; }

        
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }


        
        public string Ohip { get; set; }

        [Display(Name ="Date Of Birth")]
       
        public DateTime? DateOfBirth { get; set; }


        public bool Deceased { get; set; }

       
        [Display(Name = "Date Of Death")]
        public DateTime? DateOfDeath { get; set; }



        [Display(Name = "Home Phone")]

        [StringLength(10,ErrorMessage ="Home Phone should be exactly 10 digits in length.",MinimumLength =10)]
       
        public string HomePhone { get; set; }
       


        [Required]
       
        public string Gender { get; set; }

    }
}
