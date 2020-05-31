using ARGuide.Validators;
using Plugin.ValidationRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ARGuide.ViewModels
{
    public class MemberViewModel
    {
        ValidationUnit _unit1;
        public MemberViewModel()
        {
            userName = new ValidatableObject<string>();
            email = new ValidatableObject<string>();
            password = new ValidatableObject<string>();
            confirmPassword = new ValidatableObject<string>();
            serialNumber = new ValidatableObject<string>();
            
            
                _unit1 = new ValidationUnit(userName, email, password, confirmPassword);
                AddValidations();
           
            
            //AddValidations();
        }

        public ValidatableObject<string> userName { get; set; }
        public ValidatableObject<string> email { get; set; }
        public ValidatableObject<string> password { get; set; }
        public ValidatableObject<string> confirmPassword { get; set; }
        public ValidatableObject<string> serialNumber { get; set; }
        //public ValidatableObject<string> deviceNumber { get; set; }

        private void AddValidations()
        {
            userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            email.Validations.Add(new EmailRule<string> { ValidationMessage = "無效的email格式" });              
            password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            confirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });                    
        }

        public bool Validate()
        {
            return _unit1.Validate();
        }
    }
}
