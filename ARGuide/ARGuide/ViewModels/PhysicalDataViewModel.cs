using ARGuide.Validators;
using Plugin.ValidationRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ARGuide.ViewModels
{
    public class PhysicalDataViewModel
    {
        ValidationUnit _unit1;        

        public PhysicalDataViewModel()
        {
            birthday=new ValidatableObject<string>();
            tall=new ValidatableObject<string>();
            weight=new ValidatableObject<string>();
            age=new ValidatableObject<string>();
            
                _unit1 = new ValidationUnit(birthday, tall, weight, age);

                AddValidations();                              
        }

        public ValidatableObject<string> birthday { get; set; }
        public ValidatableObject<string> tall { get; set; }
        public ValidatableObject<string> weight { get; set; }
        public ValidatableObject<string> age { get; set; }
       

        private void AddValidations()
        {
            birthday.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            tall.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            tall.Validations.Add(new NumericRule<string> { ValidationMessage = "請輸入數字" });
            weight.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            weight.Validations.Add(new NumericRule<string> { ValidationMessage = "請輸入數字" });
            age.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            age.Validations.Add(new NumericRule<string> { ValidationMessage = "請輸入數字" });
        }
        

        public bool Validate()
        {
            //var isValidName = _name.Validate();
            //var isValidLastname = _lastname.Validate();
            //var isValidEmail = _email.Validate();

            //return isValidName && isValidLastname && isValidEmail;        
                return _unit1.Validate();            
        }
    }
}
