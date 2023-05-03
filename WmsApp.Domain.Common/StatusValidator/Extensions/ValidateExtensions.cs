using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.StatusValidator.Extensions
{
    public static class ValidateExtensions
    {
        public static void StringNullOrWhiteSpaceCheck(
            this IStatusValidator status,
            string checkString,
            string valueName)
        {
            if (string.IsNullOrWhiteSpace(checkString))
                status.AddError($"The \"{valueName}\" cannot be empty.");
        }

        public static void ObjectNullCheck(
            this IStatusValidator status,
            object valueForCheck,
            string valueName)
        {
            if (valueForCheck is null)
                status.AddError($"The \"{valueName}\" cannot be empty.");
        }


        public static void CollectionLoadCheck(
            this IStatusValidator status,
            IEnumerable? collection,
            string collectionName) 
        {
            if (collection == null)
                status.AddError($"The \"{collectionName}\" has not been loaded.");
        }

        public static void PropertyAttributeCheckByName<T>(
            this IStatusValidator status,
            object newValueToCheck,
            string propertyName)
            where T : class
        {
            if (newValueToCheck is null)
                return;

            var context = new ValidationContext(newValueToCheck, null, null);
            var validateResult = new List<ValidationResult>();

            var attributes = typeof(T)
                .GetProperty(propertyName)
                .GetCustomAttributes(false)
                .OfType<ValidationAttribute>()
                .ToArray();


            if (!Validator.TryValidateValue(newValueToCheck, context, validateResult, attributes))
                foreach (var validateError in validateResult)
                    status.AddError(propertyName + " | " + validateError.ErrorMessage);
        }

       
        public static void PropertyAttributeCheckByRef<T>(
            this IStatusValidator status,
            object newValueToCheck,
            object popertyToCheck,
            [CallerArgumentExpression("popertyToCheck")] string propertyName = null)
            where T : class
        {
            if (newValueToCheck is null)
                return;

            var context = new ValidationContext(newValueToCheck, null, null);
            var validateResult = new List<ValidationResult>();

            var attributes = typeof(T)
                .GetProperty(propertyName)
                .GetCustomAttributes(false)
                .OfType<ValidationAttribute>()
                .ToArray();

            if (!Validator.TryValidateValue(newValueToCheck, context, validateResult, attributes))
                foreach (var validateError in validateResult)
                    status.AddError(propertyName + " | " + validateError.ErrorMessage);
        }
    }
}
