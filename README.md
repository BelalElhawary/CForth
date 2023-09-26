# CForth - v0.1
**cforth** is like forth programming language but in c#

**cforth** compiles its code to platform fasm assembly then compiles that assembly and generating the final executable

### Compile
currently forth has two modes one for stack based code simlar to forth 
```
CForth examples/stack.cf -s -o build/out 
```
and one for c like syntax language
```
CForth examples/test.cf -o build/out 
```
to run the porgram after compilation finished add -r
```
CForth examples/test.cf -r -o build/out 
```
### Example
#### Forth like synatx
```
10 10 == if
    0 if
        100 print
    else
        300 print
else
    200 print
```
output '300'
#### C like synatx
```
// this is comment

if(10 == 10)
{
    if(0)
    {
        print 100;
    }
    else
    {
        print 300;
    }
}
else
{
    print 200;
}
```

### TODO
**Version 0.1**
- [x] Compile unix assembly code
- [x] Print integers to standard output
- [x] If-else statement

**Version 0.2**
- [ ] While loops
- [ ] Strings

**Version 0.2.1**
- [ ] Functions
- [ ] Main entry

**Version 0.2.2**
- [ ] Structs

**Version 0.2.5**
- [ ] Add support for windows native assembly

### Supported platforms
- **Linux** amd64, x86_64
- **Windows** x64 !! windows platform assembly code is still under development the program will generate only unix assembly & syscalls right now