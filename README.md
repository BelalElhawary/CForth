# CForth
**cforth** is like forth programming language but in c#

**cforth** compiles your program into fasm assembly then compiles that assembly and link it to current platfrom

### Run
currently forth has two modes one for stack based code simlar to forth 
```
CForth examples/stack.cf -s -o build/out 
```
and one for c like syntax language
```
CForth examples/test.cf -o build/out 
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

### Supported platforms
- **Linux** amd64, x86_64