namespace ThridPartyServiceAccessor.Entities
{ 
    public record Result<T, E>
    {
        public readonly T Payload;
        public readonly E Exception;
        public bool IsSuccess {  get; set; }

        private Result(T payload)
        {
            IsSuccess = true;
            Payload = payload;
            Exception = default!;
        }

        private Result(E exception)
        {
            IsSuccess = false;
            Payload = default!;
            Exception = exception;
        }

        public (bool, T, E) Deconstruct()
        {
            return IsSuccess ? (IsSuccess, Payload, default!) : (IsSuccess, default!, Exception);
        }

        public static implicit operator Result<T, E>(Success<T> success)
        {
            return new Result<T, E>(success.Payload);
        }

        public static implicit operator Result<T, E>(Error<E> error)
        {
            return new Result<T, E>(error.Exception);
        }
    }

    public static class ResultHelper
    {
        public static Success<T> Success<T>(T result) => new(result);
        public static Error<E> Error<E>(E exception) => new(exception);
    }

    public record Success<T>
    {
        public readonly T Payload;
        public Success(T payload)
        {
            Payload = payload;
        }
    }

    public record Error<E>
    {
        public readonly E Exception;
        public Error(E exception)
        {
            Exception = exception;
        }
    }
}
