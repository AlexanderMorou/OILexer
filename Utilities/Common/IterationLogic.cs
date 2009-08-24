using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Utilities.Common
{
    /// <summary>
    /// Translates an instance of <typeparamref name="TArgument"/> into an instance of <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of instance desired from <typeparamref name="TArgument"/>.</typeparam>
    /// <typeparam name="TArgument">The type of argument to be translated.</typeparam>
    /// <param name="argument">The <typeparamref name="TArgument"/> which contains information
    /// necessary to create a <typeparamref name="TResult"/>.</param>
    /// <returns>A value of <typeparamref name="TResult"/> as described by the 
    /// declared use of <see cref="TranslateArgument{TResult, TArgument}"/>.</returns>
    public delegate TResult TranslateArgument<TResult, TArgument>(TArgument argument);
    public delegate T ProcessArgument<T>(T arg);
}
