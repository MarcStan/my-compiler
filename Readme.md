# MyCompiler

Compiler written in C# based on the tutorials from [minsk](https://github.com/terrajobst/minsk).

# Repl

Currently a repl is implemented to execute immediate statements.

Supported features:

* multiline commands
* variable assignments
* boolean operations
* mathematical operations
* paranthesis & operator precedence support
* if, while & for loop support
* functions

## Example commands

``` csharp
(a = 5) * 5
> 25

1 + 2 * 3
> 7

false || 1 * (3 - 2) == 1
> True

let x = 10
> 10

var y = 5
> 5
```

``` csharp
{
  for int i = 0 to 10
  {
    if i == 10 break
    if i / 2 * 2 == 0 continue
    print(string(i))
  }
}
```
``` csharp
function hi(name: string)
{
  print("What's your name?")
  let name = input()
  print("Hello " + name)
}
```