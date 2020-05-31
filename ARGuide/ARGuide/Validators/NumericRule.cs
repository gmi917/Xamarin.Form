using Plugin.ValidationRules.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ARGuide.Validators
{
    public class NumericRule<T> : IValidationRule<T>
    {        
        public string ValidationMessage { get; set; }
        public bool Check(T value)
        {
            bool result = false;            
            if (value == null)
            {
                return false;
            }
            else
            {
                var str = value as string;
                if (!string.IsNullOrEmpty(str))
                {
                    if (IsNumeric(str))
                    {
                        result = true;
                    }
                    else
                    {
                        ValidationMessage = "請輸入數字";
                        result = false;
                    }
                }
            }
            return result;
        }

        public bool IsNumeric(String strNumber)
        {
            Regex NumberPattern = new Regex("[^0-9.]");
            return !NumberPattern.IsMatch(strNumber);
        }
    }
}
