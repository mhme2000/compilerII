# Compiler LALG
Project related to the final work of the Compilers II discipline.

To run the program in mode compiler use ```dotnet run --project CompilerApp compiler``` or ```dotnet run --project CompilerApp {inputFileName} {outputFileName}```.

To run the program in mode executor use ```dotnet run --project CompilerApp executor``` or ```dotnet run --project CompilerApp {outputFileName}```.

 ### Table M

https://docs.google.com/spreadsheets/d/1Hw6wTRvfZEIzi949DexItEUtizgQWYkMV14frzvG2uw/edit?usp=sharing

### Grammar

```
Especification LALG
- Comements in LALG: in between { } or /* */


<programa> -> program ident <corpo> .
<corpo> -> <dc> begin <comandos> end
<dc> -> <dc_v> <mais_dc>  | λ
<mais_dc> -> ; <dc> | λ
<dc_v> ->  <tipo_var> : <variaveis>
<tipo_var> -> real | integer
<variaveis> -> ident <mais_var>
<mais_var> -> , <variaveis> | λ
<comandos> -> <comando> <mais_comandos>
<mais_comandos> -> ; <comandos> | λ

<comando> -> 	read (ident) |
							write (ident) |
							ident := <expressao>
							
<expressao> -> <termo> <outros_termos>
<termo> -> <op_un> <fator> <mais_fatores>
<op_un> -> - | λ
<fator> -> ident | numero_int | numero_real | (<expressao>)
<outros_termos> -> <op_ad> <termo> <outros_termos> | λ
<op_ad> -> + | -
<mais_fatores> -> <op_mul> <fator> <mais_fatores> | λ
<op_mul> -> * | /
```
