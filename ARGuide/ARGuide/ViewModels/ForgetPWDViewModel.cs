using ARGuide.Validators;
using Plugin.ValidationRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace ARGuide.ViewModels
{
    public class ForgetPWDViewModel
    {
        ValidationUnit _unit1;
        public ForgetPWDViewModel()
        {
            email = new ValidatableObject<string>();
            _unit1 = new ValidationUnit(email);
            AddValidations();
        }

        public ValidatableObject<string> email { get; set; }

        private void AddValidations()
        {
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "必填欄位" });
            email.Validations.Add(new ForgetEmailRule<string> { ValidationMessage = "無效的email格式" });
        }

        public bool Validate()
        {
            return _unit1.Validate();
        }
    }
}
