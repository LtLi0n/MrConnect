using System;

namespace LionLibrary.Commands
{
    public class ArgumentConversionException<T> : Exception
    {
        public Type Type => typeof(T);
        public ArgumentConversionException(string msg) : base(msg) { }
    }
}
