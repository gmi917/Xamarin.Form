using ARGuide.Validators;
using Plugin.ValidationRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ARGuide.ViewModels
{
    public class EditProfileViewModel
    {
        ValidationUnit _unit1;
        public EditProfileViewModel()
        {
            userName = new ValidatableObject<string>();
            //email = new ValidatableObject<string>();
            birthday = new ValidatableObject<string>();
            tall = new ValidatableObject<string>();
            weight = new ValidatableObject<string>();
            age = new ValidatableObject<string>();            
            
            _unit1 = new ValidationUnit(userName, birthday, tall,weight,age);
            AddValidations();
        }

        public ValidatableObject<string> userName { get; set; }        
        public ValidatableObject<string> birthday { get; set; }
        public ValidatableObject<string> tall { get; set; }
        public ValidatableObject<string> weight { get; set; }
        public ValidatableObject<string> age { get; set; }             
        private void AddValidations()
        {
            userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });            
            birthday.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            tall.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            weight.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            age.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });           
        }

        public bool Validate()
        {
            return _unit1.Validate();
        }
    }
}
