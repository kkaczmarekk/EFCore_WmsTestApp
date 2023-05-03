using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.StatusValidator
{
    public interface IStatusValidator
    {
        public IReadOnlyList<string> Errors { get;  }
        public bool IsValid { get; }
        public IStatusValidator AddError(string errorValue);
        public IStatusValidator CombineStatues(IStatusValidator? otherStatusValidator);

    }

    public interface IStatusValidator<T> : IStatusValidator
        where T : class
    {
        public IStatusValidator<T> SetResult (T result);

        public T Result { get; }
    }
}
