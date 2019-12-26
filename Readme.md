# MyCompiler

Compiler written in C# based on the tutorials from [minsk](https://github.com/terrajobst/minsk).

[![compiler](https://dev.azure.com/marcstanlive/Opensource/_apis/build/status/132)](https://dev.azure.com/marcstanlive/Opensource/_build/definition?definitionId=132)

# Repl

Currently a repl is implemented to execute immediate statements.

Supported features:

* multiline commands
* variable assignments
* boolean operations
* mathematical operations
* paranthesis & operator precedence support
* if, while & for loop support

## Example commands

> (a = 5) * 5
> 
> 25

> 1 + 2 * 3
> 
> 7

> false || 1 * (3 - 2) == 1
> 
> True

> let x = 10
>
> 10

> var y = 5
>
> 5