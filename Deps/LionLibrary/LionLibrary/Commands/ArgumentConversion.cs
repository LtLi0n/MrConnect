using System;

namespace LionLibrary.Commands
{
    ///<summary><see cref="MandatoryArgumentsAttribute"/> and <see cref="OptionalArgumentsAttribute"/> addition has made these useful only for very rare cases.</summary>
    [Obsolete]
    public enum ArgumentConversion
    {
        ///<summary>Uses the default value if the arg is empty.</summary>
        Default,

        ///<summary>Expects arg to exists. Throws a <see cref="ArgumentConversionException{T}"/> if the arg is empty.</summary>
        [Obsolete]
        Mandatory
    }
}
