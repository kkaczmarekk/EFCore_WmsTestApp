using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.StatusValidator
{
    public class StatusValidatorHandler : IStatusValidator
    {
        private List<string> _errors;
        public StatusValidatorHandler()
        {
            _errors = new List<string>();
        }

        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        public bool IsValid => _errors.Count == 0;

        public IStatusValidator AddError(string errorValue)
        {
            _errors.Add(errorValue);

            return this;
        }

        public IStatusValidator CombineStatues(IStatusValidator? otherStatusValidator)
        {
            if (otherStatusValidator == null) return this;

            _errors.AddRange(otherStatusValidator.Errors);

            return this;
        }
    }

    public class StatusValidatorHandler<T> : StatusValidatorHandler, IStatusValidator<T>
        where T : class
    {
        T _result;

        public T Result => IsValid ? _result : null;

        public IStatusValidator<T> SetResult(T result)
        {
            _result = result;
            return this;
        }
    }

}
